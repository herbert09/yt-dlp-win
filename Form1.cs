using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using ReaLTaiizor.Controls;
using ReaLTaiizor.Forms;
using ReaLTaiizor.Manager;

namespace YtDlpDownloader;

public partial class Form1 : MaterialForm
{
    private BindingList<DownloadTask> _tasks = new BindingList<DownloadTask>();
    private Dictionary<DownloadTask, Process> _processes = new Dictionary<DownloadTask, Process>();

    public Form1()
    {
        InitializeComponent();
        MaterialSkinManager.Instance.AddFormToManage(this);
        MaterialSkinManager.Instance.EnforceBackcolorOnAllComponents = false;
        AppConfig.Load();
        LoadDownloadedRecords();
        SetupGrid();
        dgvTasks.CellPainting += DgvTasks_CellPainting;
    }

    private void SetupGrid()
    {
        dgvTasks.AutoGenerateColumns = false;
        dgvTasks.DataSource = _tasks;

        var doubleBufferedType = dgvTasks.GetType().GetProperty("DoubleBuffered", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
        doubleBufferedType?.SetValue(dgvTasks, true);

        var cms = new ContextMenuStrip();
        var pauseItem = new ToolStripMenuItem("暂停", null, (s, e) => PauseSelectedTask());
        var resumeItem = new ToolStripMenuItem("继续", null, (s, e) => ResumeSelectedTask());
        var deleteItem = new ToolStripMenuItem("删除任务", null, (s, e) => DeleteSelectedTask());
        var openDirItem = new ToolStripMenuItem("打开所在目录", null, (s, e) => OpenSelectedTaskDir());
        var copyLinkItem = new ToolStripMenuItem("复制链接", null, (s, e) => CopySelectedTaskUrl());

        cms.Items.Add(pauseItem);
        cms.Items.Add(resumeItem);
        cms.Items.Add(new ToolStripSeparator());
        cms.Items.Add(copyLinkItem);
        cms.Items.Add(openDirItem);
        cms.Items.Add(deleteItem);

        cms.Opening += (s, e) =>
        {
            if (dgvTasks.SelectedRows.Count == 0) { e.Cancel = true; return; }
            var task = dgvTasks.SelectedRows[0].DataBoundItem as DownloadTask;
            if (task == null) { e.Cancel = true; return; }

            pauseItem.Visible = task.Status == "下载中";
            resumeItem.Visible = task.Status == "已暂停";
        };

        dgvTasks.ContextMenuStrip = cms;
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

    private void DgvTasks_CellPainting(object? sender, DataGridViewCellPaintingEventArgs e)
    {
        if (e.ColumnIndex < 0 || e.RowIndex < 0) return;
        var col = dgvTasks.Columns[e.ColumnIndex];
        if (col == null || col.Name != "colProgress") return;
        if (e.Graphics == null || e.CellStyle?.Font == null) return;

        e.Paint(e.CellBounds, DataGridViewPaintParts.All & ~DataGridViewPaintParts.ContentForeground);

        var value = e.Value as int? ?? 0;
        var bounds = e.CellBounds;
        bounds.Inflate(-2, -2);

        using (var backBrush = new SolidBrush(Color.LightGray))
        {
            e.Graphics.FillRectangle(backBrush, bounds);
        }

        var progressWidth = (int)(bounds.Width * (value / 100.0));
        if (progressWidth > 0)
        {
            using var foreBrush = new SolidBrush(Color.ForestGreen);
            e.Graphics.FillRectangle(foreBrush, bounds.X, bounds.Y, progressWidth, bounds.Height);
        }

        var text = $"{value}%";
        using var textBrush = new SolidBrush(Color.Black);
        var size = e.Graphics.MeasureString(text, e.CellStyle.Font);
        var textX = bounds.X + (bounds.Width - size.Width) / 2;
        var textY = bounds.Y + (bounds.Height - size.Height) / 2;
        e.Graphics.DrawString(text, e.CellStyle.Font, textBrush, textX, textY);

        e.Handled = true;
    }

    private async void btnDownload_Click(object sender, EventArgs e)
    {
        var url = txtUrl.Text.Trim();
        if (string.IsNullOrEmpty(url))
        {
            MessageBox.Show("请输入视频链接", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        if (_tasks.Any(t => string.Equals(t.Url, url, StringComparison.OrdinalIgnoreCase) && t.Status != "已完成" && t.Status != "失败"))
        {
            MessageBox.Show("该链接正在下载中", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        if (AppConfig.IsDownloaded(url))
        {
            MessageBox.Show("该链接已下载过", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        var task = new DownloadTask
        {
            Url = url,
            Status = "等待中",
            Progress = 0
        };

        _tasks.Insert(0, task);
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
                MessageBox.Show(task.ErrorMessage, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                MessageBox.Show(task.ErrorMessage, "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                MessageBox.Show($"下载失败：{task.ErrorMessage}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
        if (dgvTasks.SelectedRows.Count == 0) return;
        var task = dgvTasks.SelectedRows[0].DataBoundItem as DownloadTask;
        if (task == null) return;

        if (_processes.TryGetValue(task, out var process) && !process.HasExited)
        {
            task.IsCancelled = true;
            process.Kill();
        }
    }

    private async void ResumeSelectedTask()
    {
        if (dgvTasks.SelectedRows.Count == 0) return;
        var task = dgvTasks.SelectedRows[0].DataBoundItem as DownloadTask;
        if (task == null) return;

        task.Status = "等待中";
        task.Progress = 0;
        task.IsCancelled = false;
        await Task.Run(() => DownloadVideo(task));
    }

    private void DeleteSelectedTask()
    {
        if (dgvTasks.SelectedRows.Count == 0) return;
        var task = dgvTasks.SelectedRows[0].DataBoundItem as DownloadTask;
        if (task == null) return;

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

    private void OpenSelectedTaskDir()
    {
        if (dgvTasks.SelectedRows.Count == 0) return;
        var task = dgvTasks.SelectedRows[0].DataBoundItem as DownloadTask;
        if (task == null) return;

        if (!string.IsNullOrEmpty(task.FilePath) && File.Exists(task.FilePath))
        {
            Process.Start(new ProcessStartInfo("explorer.exe", $"/select,\"{task.FilePath}\"") { UseShellExecute = true });
        }
        else if (!string.IsNullOrEmpty(task.FilePath) && Directory.Exists(task.FilePath))
        {
            Process.Start(new ProcessStartInfo("explorer.exe", task.FilePath) { UseShellExecute = true });
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
        if (dgvTasks.SelectedRows.Count == 0) return;
        var task = dgvTasks.SelectedRows[0].DataBoundItem as DownloadTask;
        if (task == null) return;

        Clipboard.SetText(task.Url);
    }

    private void btnAdvancedSettings_Click(object sender, EventArgs e)
    {
        using var form = new SettingsForm();
        form.ShowDialog(this);
    }
}
