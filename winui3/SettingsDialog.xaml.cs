using Microsoft.UI.Xaml.Controls;

namespace YtDlpDownloader;

public sealed partial class SettingsDialog : ContentDialog
{
    public SettingsDialog()
    {
        this.InitializeComponent();
        LoadSettings();
    }

    private void LoadSettings()
    {
        txtProxy.Text = AppConfig.Settings.Proxy;
        txtOutputDir.Text = AppConfig.Settings.OutputDirectory;
        txtCookies.Text = AppConfig.Settings.CookiesPath;
        txtYtDlpPath.Text = AppConfig.Settings.YtDlpPath;
        chkMetadata.IsChecked = AppConfig.Settings.WriteMetadata;

        cmbQuality.ItemsSource = new[] { "best", "2160", "1080", "720", "480", "360" };
        cmbQuality.SelectedItem = AppConfig.Settings.Quality;

        cmbFormat.ItemsSource = new[] { "default", "mp4", "webm", "mkv", "mp3", "m4a", "wav" };
        cmbFormat.SelectedItem = AppConfig.Settings.Format;

        cmbAudioQuality.ItemsSource = new[] { "0", "2", "5", "7" };
        cmbAudioQuality.SelectedItem = AppConfig.Settings.AudioQuality;

        cmbSubtitles.ItemsSource = new[] { "不下载", "中文", "英文" };
        cmbSubtitles.SelectedItem = AppConfig.Settings.SubtitleLang switch
        {
            "zh-CN" => "中文",
            "en" => "英文",
            _ => "不下载"
        };
    }

    private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
    {
        AppConfig.Settings.Proxy = txtProxy.Text.Trim();
        AppConfig.Settings.OutputDirectory = txtOutputDir.Text.Trim();
        AppConfig.Settings.CookiesPath = txtCookies.Text.Trim();
        AppConfig.Settings.YtDlpPath = txtYtDlpPath.Text.Trim();
        AppConfig.Settings.WriteMetadata = chkMetadata.IsChecked ?? false;
        AppConfig.Settings.Quality = cmbQuality.SelectedItem?.ToString() ?? "best";
        AppConfig.Settings.Format = cmbFormat.SelectedItem?.ToString() ?? "default";
        AppConfig.Settings.AudioQuality = cmbAudioQuality.SelectedItem?.ToString() ?? "0";
        AppConfig.Settings.SubtitleLang = cmbSubtitles.SelectedItem?.ToString() switch
        {
            "中文" => "zh-CN",
            "英文" => "en",
            _ => ""
        };
        AppConfig.SaveSettings();
    }
}
