using System.Text;
using System.Text.Json;

namespace YtDlpDownloader;

public class AppSettings
{
    public string Proxy { get; set; } = "socks5://127.0.0.1:10808";
    public string OutputDirectory { get; set; } = Path.Combine(AppContext.BaseDirectory, "downloads");
    public string Quality { get; set; } = "best";
    public string Format { get; set; } = "default";
    public string AudioQuality { get; set; } = "0";
    public string SubtitleLang { get; set; } = "";
    public bool WriteMetadata { get; set; } = false;
    public string CookiesPath { get; set; } = "";
    public string YtDlpPath { get; set; } = "yt-dlp";
}

public static class AppConfig
{
    private static readonly string ConfigPath = Path.Combine(AppContext.BaseDirectory, "settings.json");
    private static readonly string RecordsPath = Path.Combine(AppContext.BaseDirectory, "downloaded.csv");
    private static readonly string LegacyRecordsPath = Path.Combine(AppContext.BaseDirectory, "downloaded.json");

    public static AppSettings Settings { get; set; } = new AppSettings();
    public static List<DownloadRecord> Records { get; set; } = new List<DownloadRecord>();

    public static void Load()
    {
        if (File.Exists(ConfigPath))
        {
            try
            {
                var json = File.ReadAllText(ConfigPath);
                var loaded = JsonSerializer.Deserialize<AppSettings>(json);
                if (loaded != null) Settings = loaded;
            }
            catch { }
        }

        if (File.Exists(RecordsPath))
        {
            try { LoadRecordsFromCsv(); }
            catch { }
        }
        else if (File.Exists(LegacyRecordsPath))
        {
            try
            {
                var json = File.ReadAllText(LegacyRecordsPath);
                var loaded = JsonSerializer.Deserialize<List<DownloadRecord>>(json);
                if (loaded != null)
                {
                    Records = loaded;
                    SaveRecords();
                }
            }
            catch { }
        }

        if (!Directory.Exists(Settings.OutputDirectory))
            Directory.CreateDirectory(Settings.OutputDirectory);
    }

    private static void LoadRecordsFromCsv()
    {
        var lines = File.ReadAllLines(RecordsPath);
        if (lines.Length <= 1) return;
        Records.Clear();
        for (int i = 1; i < lines.Length; i++)
        {
            var parts = ParseCsvLine(lines[i]);
            if (parts.Length < 5) continue;
            Records.Add(new DownloadRecord
            {
                Url = parts[0],
                Title = parts[1],
                FilePath = parts[2],
                DownloadTime = parts[3],
                FileSize = parts.Length > 4 ? parts[4] : ""
            });
        }
    }

    private static string[] ParseCsvLine(string line)
    {
        var result = new List<string>();
        bool inQuotes = false;
        var sb = new StringBuilder();
        for (int i = 0; i < line.Length; i++)
        {
            char c = line[i];
            if (c == '"')
            {
                if (inQuotes && i + 1 < line.Length && line[i + 1] == '"')
                {
                    sb.Append('"');
                    i++;
                }
                else
                {
                    inQuotes = !inQuotes;
                }
            }
            else if (c == ',' && !inQuotes)
            {
                result.Add(sb.ToString());
                sb.Clear();
            }
            else
            {
                sb.Append(c);
            }
        }
        result.Add(sb.ToString());
        return result.ToArray();
    }

    private static string EscapeCsv(string? s)
    {
        if (string.IsNullOrEmpty(s)) return "";
        return s.Replace("\"", "\"\"");
    }

    public static void SaveSettings()
    {
        var json = JsonSerializer.Serialize(Settings, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(ConfigPath, json);
    }

    public static void SaveRecords()
    {
        var lines = new List<string> { "Url,Title,FilePath,DownloadTime,FileSize" };
        foreach (var r in Records)
        {
            lines.Add($"\"{EscapeCsv(r.Url)}\",\"{EscapeCsv(r.Title)}\",\"{EscapeCsv(r.FilePath)}\",\"{EscapeCsv(r.DownloadTime)}\",\"{EscapeCsv(r.FileSize)}\"");
        }
        File.WriteAllLines(RecordsPath, lines);
    }

    public static bool IsDownloaded(string url)
    {
        return Records.Any(r => string.Equals(r.Url, url, StringComparison.OrdinalIgnoreCase));
    }
}
