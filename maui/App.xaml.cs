namespace YtDlpDownloader;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
        AppConfig.Load();
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        return new Window(new AppShell()) { Title = "YtDlpDownloader" };
    }
}
