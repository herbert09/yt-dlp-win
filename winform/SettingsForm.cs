namespace YtDlpDownloader;

public partial class SettingsForm : AntdUI.Window
{
    public SettingsForm()
    {
        InitializeComponent();
        LoadSettings();
    }

    private void LoadSettings()
    {
        cmbQuality.SelectedValue = AppConfig.Settings.Quality;
        if (cmbQuality.SelectedValue == null) cmbQuality.Text = "best";

        cmbFormat.SelectedValue = AppConfig.Settings.Format;
        if (cmbFormat.SelectedValue == null) cmbFormat.Text = "default";

        cmbAudioQuality.SelectedValue = AppConfig.Settings.AudioQuality;
        if (cmbAudioQuality.SelectedValue == null) cmbAudioQuality.Text = "0";

        cmbSubtitles.SelectedValue = AppConfig.Settings.SubtitleLang switch
        {
            "zh-CN" => "中文",
            "en" => "英文",
            _ => "不下载"
        };
        if (cmbSubtitles.SelectedValue == null) cmbSubtitles.Text = "不下载";
        chkMetadata.Checked = AppConfig.Settings.WriteMetadata;
        txtCookies.Text = AppConfig.Settings.CookiesPath;
        txtOutputDir.Text = AppConfig.Settings.OutputDirectory;
        txtProxy.Text = AppConfig.Settings.Proxy;
        txtYtDlpPath.Text = AppConfig.Settings.YtDlpPath;
    }

    private void btnBrowseCookies_Click(object sender, EventArgs e)
    {
        using var ofd = new OpenFileDialog
        {
            Filter = "文本文件|*.txt|所有文件|*.*",
            Title = "选择 Cookies 文件"
        };
        if (ofd.ShowDialog() == DialogResult.OK)
        {
            txtCookies.Text = ofd.FileName;
        }
    }

    private void btnBrowseOutputDir_Click(object sender, EventArgs e)
    {
        using var fbd = new FolderBrowserDialog();
        if (fbd.ShowDialog() == DialogResult.OK)
        {
            txtOutputDir.Text = fbd.SelectedPath;
        }
    }

    private void btnOK_Click(object sender, EventArgs e)
    {
        AppConfig.Settings.Quality = cmbQuality.SelectedValue?.ToString() ?? "best";
        AppConfig.Settings.Format = cmbFormat.SelectedValue?.ToString() ?? "default";
        AppConfig.Settings.AudioQuality = cmbAudioQuality.SelectedValue?.ToString() ?? "0";
        AppConfig.Settings.SubtitleLang = cmbSubtitles.SelectedValue?.ToString() switch
        {
            "中文" => "zh-CN",
            "英文" => "en",
            _ => ""
        };
        AppConfig.Settings.WriteMetadata = chkMetadata.Checked;
        AppConfig.Settings.CookiesPath = txtCookies.Text.Trim();
        AppConfig.Settings.OutputDirectory = txtOutputDir.Text.Trim();
        AppConfig.Settings.Proxy = txtProxy.Text.Trim();
        AppConfig.Settings.YtDlpPath = txtYtDlpPath.Text.Trim();
        AppConfig.SaveSettings();
        DialogResult = DialogResult.OK;
        Close();
    }

    private void btnBrowseYtDlp_Click(object sender, EventArgs e)
    {
        using var ofd = new OpenFileDialog
        {
            Filter = "可执行文件|*.exe|所有文件|*.*",
            Title = "选择 yt-dlp 程序"
        };
        if (ofd.ShowDialog() == DialogResult.OK)
        {
            txtYtDlpPath.Text = ofd.FileName;
        }
    }

    private void btnCancel_Click(object sender, EventArgs e)
    {
        DialogResult = DialogResult.Cancel;
        Close();
    }
}
