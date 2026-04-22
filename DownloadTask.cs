using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace YtDlpDownloader;

public class DownloadTask : INotifyPropertyChanged
{
    private string _status = "等待中";
    private int _progress = 0;
    private string _title = "";
    private string _downloadTime = "";
    private string _errorMessage = "";
    private string _fileSize = "";

    public string Url { get; set; } = "";
    public bool IsCancelled { get; set; }

    public string Title
    {
        get => _title;
        set { if (_title == value) return; _title = value; OnPropertyChanged(); }
    }

    public string Status
    {
        get => _status;
        set { if (_status == value) return; _status = value; OnPropertyChanged(); }
    }

    public int Progress
    {
        get => _progress;
        set { if (_progress == value) return; _progress = value; OnPropertyChanged(); OnPropertyChanged(nameof(ProgressDouble)); }
    }

    public double ProgressDouble => _progress / 100.0;

    public string DownloadTime
    {
        get => _downloadTime;
        set { if (_downloadTime == value) return; _downloadTime = value; OnPropertyChanged(); }
    }

    public string FileSize
    {
        get => _fileSize;
        set { if (_fileSize == value) return; _fileSize = value; OnPropertyChanged(); }
    }

    public string FilePath { get; set; } = "";

    public string ErrorMessage
    {
        get => _errorMessage;
        set { if (_errorMessage == value) return; _errorMessage = value; OnPropertyChanged(); }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
