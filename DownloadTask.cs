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
        set { _title = value; OnPropertyChanged(); }
    }

    public string Status
    {
        get => _status;
        set { _status = value; OnPropertyChanged(); }
    }

    public int Progress
    {
        get => _progress;
        set { _progress = value; OnPropertyChanged(); }
    }

    public string DownloadTime
    {
        get => _downloadTime;
        set { _downloadTime = value; OnPropertyChanged(); }
    }

    public string FileSize
    {
        get => _fileSize;
        set { _fileSize = value; OnPropertyChanged(); }
    }

    public string FilePath { get; set; } = "";

    public string ErrorMessage
    {
        get => _errorMessage;
        set { _errorMessage = value; OnPropertyChanged(); }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
