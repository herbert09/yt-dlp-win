using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;

namespace YtDlpDownloader;

public partial class Form1 : AntdUI.Window
{
    private BindingList<DownloadTask> _tasks = new BindingList<DownloadTask>();
    private Dictionary<DownloadTask, Process> _processes = new Dictionary<DownloadTask, Process>();
    private DownloadTask? _selectedTask;

    public Form1()
    {
        InitializeComponent();
        AppConfig.Load();
        LoadDownloadedRecords();
        SetupGrid();
    }

    private void SetupGrid()
    {
        tableTasks.Columns = new AntdUI.ColumnCollection
        {
            new AntdUI.Column("Title", "标题") { Width = "40%" },
            new AntdUI.Column("Status", "状态", AntdUI.ColumnAlign.Center) { Width = "12%" },
            new AntdUI.Column("Progress", "进度", AntdUI.ColumnAlign.Center)
            {
                Width = "12%",
                Render = (value, record, index) =>
                {
                    if (record is DownloadTask task)
                        return new AntdUI.CellProgress(task.Progress / 100f).SetSize(80, 6);
                    return new AntdUI.CellProgress(0).SetSize(80, 6);
                }
            },
            new AntdUI.Column("FileSize", "文件大小", AntdUI.ColumnAlign.Center) { Width = "16%" },
            new AntdUI.Column("DownloadTime", "下载时间", AntdUI.ColumnAlign.Center) { Width = "20%" }
        };

        tableTasks.DataSource = _tasks;

        tableTasks.CellClick += (s, e) =>
        {
            if (e.Record is DownloadTask task) _selectedTask = task;
        };

        tableTasks.MouseClick += (s, e) =>
        {
            if (e.Button == MouseButtons.Right && _selectedTask != null)
            {
                var pauseVisible = _selectedTask.Status == "下载中";
                var resumeVisible = _selectedTask.Status == "已暂停";

                var items = new List<AntdUI.IContextMenuStripItem>();
                if (pauseVisible) items.Add(new AntdUI.ContextMenuStripItem("暂停").SetIcon("PauseOutlined"));
                if (resumeVisible) items.Add(new AntdUI.ContextMenuStripItem("继续").SetIcon("PlayCircleOutlined"));
                items.Add(new AntdUI.ContextMenuStripItemDivider());
                items.Add(new AntdUI.ContextMenuStripItem("复制链接").SetIcon("CopyOutlined"));
                items.Add(new AntdUI.ContextMenuStripItem("打开所在目录").SetIcon("FolderOpenOutlined"));
                items.Add(new AntdUI.ContextMenuStripItemDivider());
                items.Add(new AntdUI.ContextMenuStripItem("删除任务").SetIcon("DeleteOutlined"));

                AntdUI.ContextMenuStrip.open(tableTasks, it =>
                {
                    switch (it.Text)
                    {
                        case "暂停": PauseSelectedTask(); break;
                        case "继续": ResumeSelectedTask(); break;
                        case "复制链接": CopySelectedTaskUrl(); break;
                        case "打开所在目录": OpenSelectedTaskDir(); break;
                        case "删除任务": DeleteSelectedTask(); break;
                    }
                }, items.ToArray());
            }
        };
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

    private async void btnDownload_Click(object sender, EventArgs e)
    {
        var url = txtUrl.Text.Trim();
        if (string.IsNullOrEmpty(url))
        {
            AntdUI.Message.info(this, "请输入视频链接");
            return;
        }

        if (_tasks.Any(t => string.Equals(t.Url, url, StringComparison.OrdinalIgnoreCase) && t.Status != "已完成" && t.Status != "失败"))
        {
            AntdUI.Message.info(this, "该链接正在下载中");
            return;
        }

        if (AppConfig.IsDownloaded(url))
        {
            AntdUI.Message.info(this, "该链接已下载过");
            return;
        }

        var task = new DownloadTask
        {
            Url = url,
            Status = "等待中",
            Progress = 0
        };

        _tasks.Insert(0, task);
        tableTasks.Refresh();
        txtUrl.Clear();

        await Task.Run(() => DownloadVideo(task));
    }

    private string BuildArguments(string url, string outputDir, string? uploader = null)
    {
        var sb = new StringBuilder();
        sb.Append("--no-warnings --newline --no-colors --progress");

        if (!string.IsNullOrWhiteSpace(AppConfig.Settings.Proxy))
        {
            sb.Append($" --proxy \"{AppConfig.Settings.Proxy}\"");
        }

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
            {
                sb.Append($" --merge-output-format {format}");
            }
        }

        if (!string.IsNullOrWhiteSpace(AppConfig.Settings.SubtitleLang))
        {
            sb.Append(" --write-subs --write-auto-subs");
            sb.Append($" --sub-langs {AppConfig.Settings.SubtitleLang}");
            if (format != "mp3" && format != "m4a" && format != "wav")
            {
                sb.Append(" --embed-subs");
            }
        }

        if (AppConfig.Settings.WriteMetadata)
        {
            sb.Append(" --add-metadata --embed-thumbnail");
        }

        if (!string.IsNullOrWhiteSpace(AppConfig.Settings.CookiesPath) && File.Exists(AppConfig.Settings.CookiesPath))
        {
            sb.Append($" --cookies \"{AppConfig.Settings.CookiesPath}\"");
        }

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
        {
            Directory.CreateDirectory(outputDir);
        }

        var proxy = AppConfig.Settings.Proxy;

        Invoke(() => task.Status = "获取信息中");

        var (uploader, title) = GetVideoInfo(task.Url, proxy);
        if (string.IsNullOrEmpty(title))
        {
            Invoke(() =>
            {
                task.Status = "失败";
                task.ErrorMessage = "无法获取视频信息，请检查链接或代理设置";
                AntdUI.Message.error(this, task.ErrorMessage);
            });
            return;
        }

        Invoke(() => task.Title = string.IsNullOrEmpty(uploader) ? title : $"[{uploader}] {title}");

        if (AppConfig.IsDownloaded(task.Url) && task.Status != "已暂停")
        {
            Invoke(() =>
            {
                task.Status = "失败";
                task.ErrorMessage = "该链接已下载过";
                AntdUI.Message.info(this, task.ErrorMessage);
            });
            return;
        }

        Invoke(() => task.Status = "下载中");

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
                {
                    Invoke(() => task.Progress = newProgress);
                }
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
                Invoke(() => task.Status = "已暂停");
                return;
            }

            Invoke(() =>
            {
                task.Status = "失败";
                task.ErrorMessage = errorBuilder.ToString();
                AntdUI.Message.error(this, $"下载失败：{task.ErrorMessage}");
            });
            return;
        }

        var latestFile = GetLatestFile(outputDir);
        var fileSize = latestFile != null ? FormatFileSize(latestFile.Length) : "";

        Invoke(() =>
        {
            task.Status = "已完成";
            task.Progress = 100;
            task.DownloadTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            task.FileSize = fileSize;
            task.FilePath = latestFile?.FullName ?? "";
        });

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
        if (lines.Length >= 2)
        {
            return (lines[0].Trim(), lines[1].Trim());
        }
        if (lines.Length == 1)
        {
            return ("", lines[0].Trim());
        }
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

    private void PauseSelectedTask()
    {
        if (_selectedTask == null) return;

        if (_processes.TryGetValue(_selectedTask, out var process) && !process.HasExited)
        {
            _selectedTask.IsCancelled = true;
            process.Kill();
        }
    }

    private async void ResumeSelectedTask()
    {
        if (_selectedTask == null) return;

        _selectedTask.Status = "等待中";
        _selectedTask.Progress = 0;
        _selectedTask.IsCancelled = false;
        await Task.Run(() => DownloadVideo(_selectedTask));
    }

    private void DeleteSelectedTask()
    {
        if (_selectedTask == null) return;

        if (_processes.TryGetValue(_selectedTask, out var process) && !process.HasExited)
        {
            _selectedTask.IsCancelled = true;
            process.Kill();
        }
        _processes.Remove(_selectedTask);
        _tasks.Remove(_selectedTask);

        var record = AppConfig.Records.FirstOrDefault(r => string.Equals(r.Url, _selectedTask.Url, StringComparison.OrdinalIgnoreCase));
        if (record != null)
        {
            AppConfig.Records.Remove(record);
            AppConfig.SaveRecords();
        }
    }

    private void OpenSelectedTaskDir()
    {
        if (_selectedTask == null) return;

        if (!string.IsNullOrEmpty(_selectedTask.FilePath) && File.Exists(_selectedTask.FilePath))
        {
            Process.Start(new ProcessStartInfo("explorer.exe", $"/select,\"{_selectedTask.FilePath}\"") { UseShellExecute = true });
        }
        else if (!string.IsNullOrEmpty(_selectedTask.FilePath) && Directory.Exists(_selectedTask.FilePath))
        {
            Process.Start(new ProcessStartInfo("explorer.exe", _selectedTask.FilePath) { UseShellExecute = true });
        }
        else
        {
            var path = AppConfig.Settings.OutputDirectory;
            if (Directory.Exists(path))
            {
                Process.Start(new ProcessStartInfo("explorer.exe", path) { UseShellExecute = true });
            }
        }
    }

    private void CopySelectedTaskUrl()
    {
        if (_selectedTask == null) return;
        Clipboard.SetText(_selectedTask.Url);
    }

    private void btnAdvancedSettings_Click(object sender, EventArgs e)
    {
        using var form = new SettingsForm();
        form.ShowDialog(this);
    }
}
