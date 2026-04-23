using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using Windows.ApplicationModel.DataTransfer;

namespace YtDlpDownloader;

public sealed partial class MainWindow : Window
{
    private ObservableCollection<DownloadTask> _tasks = new();
    private Dictionary<DownloadTask, Process> _processes = new();

    public MainWindow()
    {
        this.InitializeComponent();
        tasksList.ItemsSource = _tasks;
        LoadDownloadedRecords();
    }

    private void LoadDownloadedRecords()
    {
        var sorted = AppConfig.Records
            .OrderByDescending(r =>
            {
                if (DateTime.TryParse(r.DownloadTime, out var dt)) return dt;
                return DateTime.MinValue;
            })
            .ToList();

        foreach (var record in sorted)
        {
            _tasks.Add(new DownloadTask
            {
                Url = record.Url,
                Title = record.Title,
                Status = "已完成",
                Progress = 100,
                DownloadTime = record.DownloadTime,
                FileSize = record.FileSize,
                FilePath = record.FilePath
            });
        }
    }

    private async void btnDownload_Click(object sender, RoutedEventArgs e)
    {
        var url = txtUrl.Text?.Trim() ?? "";
        if (string.IsNullOrEmpty(url))
        {
            await ShowInfoAsync("请输入视频链接");
            return;
        }

        if (_tasks.Any(t => string.Equals(t.Url, url, StringComparison.OrdinalIgnoreCase) && t.Status != "已完成" && t.Status != "失败"))
        {
            await ShowInfoAsync("该链接正在下载中");
            return;
        }

        if (AppConfig.IsDownloaded(url))
        {
            await ShowInfoAsync("该链接已下载过");
            return;
        }

        var task = new DownloadTask
        {
            Url = url,
            Status = "等待中",
            Progress = 0
        };

        _tasks.Insert(0, task);
        txtUrl.Text = "";

        await Task.Run(() => DownloadVideo(task));
    }

    private async void btnSettings_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new SettingsDialog();
        dialog.XamlRoot = this.Content.XamlRoot;
        await dialog.ShowAsync();
    }

    private void Pause_Click(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement fe && fe.Tag is DownloadTask task)
            PauseTask(task);
    }

    private void Resume_Click(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement fe && fe.Tag is DownloadTask task)
            _ = ResumeTaskAsync(task);
    }

    private async void Copy_Click(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement fe && fe.Tag is DownloadTask task)
        {
            var package = new DataPackage();
            package.SetText(task.Url);
            Clipboard.SetContent(package);
            await ShowInfoAsync("链接已复制");
        }
    }

    private void Open_Click(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement fe && fe.Tag is DownloadTask task)
            OpenTaskDir(task);
    }

    private void Delete_Click(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement fe && fe.Tag is DownloadTask task)
            DeleteTask(task);
    }

    private string BuildArguments(string url, string outputDir, string? uploader = null)
    {
        var sb = new StringBuilder();
        sb.Append("--no-warnings --newline --no-colors --progress");

        if (!string.IsNullOrWhiteSpace(AppConfig.Settings.Proxy))
            sb.Append($" --proxy \"{AppConfig.Settings.Proxy}\"");

        var format = AppConfig.Settings.Format;
        var quality = AppConfig.Settings.Quality;

        if (format == "mp3" || format == "m4a" || format == "wav")
        {
            sb.Append(" -x");
            sb.Append($" --audio-format {format}");
            sb.Append($" --audio-quality {AppConfig.Settings.AudioQuality}");
        }
        else
        {
            string qualityFilter = quality switch
            {
                "2160" => "bestvideo[height<=2160]+bestaudio/best[height<=2160]",
                "1080" => "bestvideo[height<=1080]+bestaudio/best[height<=1080]",
                "720" => "bestvideo[height<=720]+bestaudio/best[height<=720]",
                "480" => "bestvideo[height<=480]+bestaudio/best[height<=480]",
                "360" => "bestvideo[height<=360]+bestaudio/best[height<=360]",
                _ => "bestvideo*+bestaudio/best"
            };
            sb.Append($" -f \"{qualityFilter}\"");
            if (format != "default")
                sb.Append($" --merge-output-format {format}");
        }

        if (!string.IsNullOrWhiteSpace(AppConfig.Settings.SubtitleLang))
        {
            sb.Append(" --write-subs --write-auto-subs");
            sb.Append($" --sub-langs {AppConfig.Settings.SubtitleLang}");
            if (format != "mp3" && format != "m4a" && format != "wav")
                sb.Append(" --embed-subs");
        }

        if (AppConfig.Settings.WriteMetadata)
            sb.Append(" --add-metadata --embed-thumbnail");

        if (!string.IsNullOrWhiteSpace(AppConfig.Settings.CookiesPath) && File.Exists(AppConfig.Settings.CookiesPath))
            sb.Append($" --cookies \"{AppConfig.Settings.CookiesPath}\"");

        if (!string.IsNullOrWhiteSpace(uploader))
        {
            var safe = uploader.Replace("\"", "'").Replace('\n', ' ').Replace('\r', ' ');
            sb.Append($" -P \"{outputDir}\" -o \"{safe} - %(title)s.%(ext)s\"");
        }
        else
        {
            sb.Append($" -P \"{outputDir}\" -o \"%(title)s.%(ext)s\"");
        }
        sb.Append($" \"{url}\"");

        return sb.ToString();
    }

    private void DownloadVideo(DownloadTask task)
    {
        var outputDir = AppConfig.Settings.OutputDirectory;
        if (!Directory.Exists(outputDir))
            Directory.CreateDirectory(outputDir);

        var proxy = AppConfig.Settings.Proxy;

        task.Status = "获取信息中";

        var (uploader, title) = GetVideoInfo(task.Url, proxy);
        if (string.IsNullOrEmpty(title))
        {
            task.Status = "失败";
            task.ErrorMessage = "无法获取视频信息，请检查链接或代理设置";
            return;
        }

        task.Title = string.IsNullOrEmpty(uploader) ? title : $"[{uploader}] {title}";

        if (AppConfig.IsDownloaded(task.Url) && task.Status != "已暂停")
        {
            task.Status = "失败";
            task.ErrorMessage = "该链接已下载过";
            return;
        }

        task.Status = "下载中";

        var psi = new ProcessStartInfo
        {
            FileName = AppConfig.Settings.YtDlpPath,
            Arguments = BuildArguments(task.Url, outputDir, uploader),
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };
        psi.EnvironmentVariables["PYTHONUNBUFFERED"] = "1";

        using var process = new Process { StartInfo = psi };
        _processes[task] = process;

        var errorBuilder = new StringBuilder();
        var progressRegex = new Regex(@"(\d+(?:\.\d+)?)%", RegexOptions.Compiled);
        var ansiRegex = new Regex(@"\x1B\[[0-9;]*[A-Za-z]", RegexOptions.Compiled);

        void HandleLine(string? rawLine)
        {
            if (string.IsNullOrEmpty(rawLine)) return;
            var line = ansiRegex.Replace(rawLine, "");
            var match = progressRegex.Match(line);
            if (match.Success && double.TryParse(match.Groups[1].Value, out var pct))
            {
                var newProgress = (int)pct;
                if (task.Progress != newProgress)
                    task.Progress = newProgress;
            }
            else if (line.Contains("ERROR:", StringComparison.OrdinalIgnoreCase))
            {
                errorBuilder.AppendLine(line);
            }
        }

        process.OutputDataReceived += (s, e) => HandleLine(e.Data);
        process.ErrorDataReceived += (s, e) => HandleLine(e.Data);

        process.Start();
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();
        process.WaitForExit();

        _processes.Remove(task);

        if (process.ExitCode != 0)
        {
            if (task.IsCancelled)
            {
                task.Status = "已暂停";
                return;
            }
            task.Status = "失败";
            task.ErrorMessage = errorBuilder.ToString();
            return;
        }

        var latestFile = GetLatestFile(outputDir);
        var fileSize = latestFile != null ? FormatFileSize(latestFile.Length) : "";

        task.Status = "已完成";
        task.Progress = 100;
        task.DownloadTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        task.FileSize = fileSize;
        task.FilePath = latestFile?.FullName ?? "";

        var record = new DownloadRecord
        {
            Url = task.Url,
            Title = task.Title,
            DownloadTime = task.DownloadTime,
            FilePath = latestFile?.FullName ?? outputDir,
            FileSize = fileSize
        };

        AppConfig.Records.Add(record);
        AppConfig.SaveRecords();
    }

    private (string Uploader, string Title) GetVideoInfo(string url, string proxy)
    {
        var psi = new ProcessStartInfo
        {
            FileName = AppConfig.Settings.YtDlpPath,
            Arguments = $"--no-warnings --proxy \"{proxy}\" --print \"%(uploader)s\" --print \"%(title)s\" \"{url}\"",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var process = new Process { StartInfo = psi };
        process.Start();
        var output = process.StandardOutput.ReadToEnd();
        process.WaitForExit();

        if (process.ExitCode != 0) return ("", "");

        var lines = output.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
        if (lines.Length >= 2) return (lines[0].Trim(), lines[1].Trim());
        if (lines.Length == 1) return ("", lines[0].Trim());
        return ("", "");
    }

    private FileInfo? GetLatestFile(string outputDir)
    {
        try
        {
            return Directory.GetFiles(outputDir)
                .Select(f => new FileInfo(f))
                .OrderByDescending(fi => fi.LastWriteTime)
                .FirstOrDefault();
        }
        catch { }
        return null;
    }

    private string FormatFileSize(long bytes)
    {
        string[] sizes = { "B", "KB", "MB", "GB", "TB" };
        int order = 0;
        double len = bytes;
        while (len >= 1024 && order < sizes.Length - 1)
        {
            order++;
            len /= 1024;
        }
        return $"{len:0.##} {sizes[order]}";
    }

    private void PauseTask(DownloadTask task)
    {
        if (_processes.TryGetValue(task, out var process) && !process.HasExited)
        {
            task.IsCancelled = true;
            process.Kill();
        }
    }

    private async Task ResumeTaskAsync(DownloadTask task)
    {
        task.Status = "等待中";
        task.Progress = 0;
        task.IsCancelled = false;
        await Task.Run(() => DownloadVideo(task));
    }

    private void DeleteTask(DownloadTask task)
    {
        if (_processes.TryGetValue(task, out var process) && !process.HasExited)
        {
            task.IsCancelled = true;
            process.Kill();
        }
        _processes.Remove(task);
        _tasks.Remove(task);

        var record = AppConfig.Records.FirstOrDefault(r => string.Equals(r.Url, task.Url, StringComparison.OrdinalIgnoreCase));
        if (record != null)
        {
            AppConfig.Records.Remove(record);
            AppConfig.SaveRecords();
        }
    }

    private void OpenTaskDir(DownloadTask task)
    {
        string? path = null;
        if (!string.IsNullOrEmpty(task.FilePath) && File.Exists(task.FilePath))
            path = Path.GetDirectoryName(task.FilePath);
        else if (!string.IsNullOrEmpty(task.FilePath) && Directory.Exists(task.FilePath))
            path = task.FilePath;
        else
            path = AppConfig.Settings.OutputDirectory;

        if (!string.IsNullOrEmpty(path) && Directory.Exists(path))
        {
            Process.Start(new ProcessStartInfo("explorer.exe", path) { UseShellExecute = true });
        }
    }

    private async Task ShowInfoAsync(string message)
    {
        var dialog = new ContentDialog
        {
            Title = "提示",
            Content = message,
            CloseButtonText = "确定",
            XamlRoot = this.Content.XamlRoot
        };
        await dialog.ShowAsync();
    }
}
