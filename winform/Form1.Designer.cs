using AntdUI;

namespace YtDlpDownloader;

partial class Form1 : AntdUI.Window
{
    private System.ComponentModel.IContainer components = null;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        pageHeader = new AntdUI.PageHeader();
        txtUrl = new AntdUI.Input();
        btnDownload = new AntdUI.Button();
        btnAdvancedSettings = new AntdUI.Button();
        tableTasks = new AntdUI.Table();
        SuspendLayout();

        // pageHeader
        pageHeader.Dock = DockStyle.Top;
        pageHeader.Name = "pageHeader";
        pageHeader.Size = new Size(980, 44);
        pageHeader.TabIndex = 0;
        pageHeader.Text = "yt-dlp 视频下载器";
        pageHeader.ShowButton = true;
        pageHeader.DividerShow = true;

        // txtUrl
        txtUrl.Location = new Point(20, 56);
        txtUrl.Name = "txtUrl";
        txtUrl.PlaceholderText = "粘贴视频链接...";
        txtUrl.Size = new Size(670, 48);
        txtUrl.TabIndex = 1;
        txtUrl.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

        // btnDownload
        btnDownload.Location = new Point(710, 56);
        btnDownload.Name = "btnDownload";
        btnDownload.Size = new Size(110, 48);
        btnDownload.TabIndex = 2;
        btnDownload.Text = "下载";
        btnDownload.Type = AntdUI.TTypeMini.Primary;
        btnDownload.Click += btnDownload_Click;
        btnDownload.Anchor = AnchorStyles.Top | AnchorStyles.Right;

        // btnAdvancedSettings
        btnAdvancedSettings.Location = new Point(830, 56);
        btnAdvancedSettings.Name = "btnAdvancedSettings";
        btnAdvancedSettings.Size = new Size(130, 48);
        btnAdvancedSettings.TabIndex = 3;
        btnAdvancedSettings.Text = "高级设置";
        btnAdvancedSettings.Click += btnAdvancedSettings_Click;
        btnAdvancedSettings.Anchor = AnchorStyles.Top | AnchorStyles.Right;

        // tableTasks
        tableTasks.Location = new Point(20, 116);
        tableTasks.Name = "tableTasks";
        tableTasks.Size = new Size(940, 500);
        tableTasks.TabIndex = 4;
        tableTasks.FixedHeader = true;
        tableTasks.Bordered = true;
        tableTasks.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

        // Form1
        AutoScaleDimensions = new SizeF(8F, 20F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(980, 640);
        Controls.Add(tableTasks);
        Controls.Add(btnAdvancedSettings);
        Controls.Add(btnDownload);
        Controls.Add(txtUrl);
        Controls.Add(pageHeader);
        Name = "Form1";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "yt-dlp 视频下载器";
        this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
        ResumeLayout(false);
    }

    private AntdUI.PageHeader pageHeader;
    private AntdUI.Input txtUrl;
    private AntdUI.Button btnDownload;
    private AntdUI.Button btnAdvancedSettings;
    private AntdUI.Table tableTasks;
}
