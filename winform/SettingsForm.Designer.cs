namespace YtDlpDownloader;

partial class SettingsForm
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

    #region Windows Form Designer generated code

    private void InitializeComponent()
    {
        tableLayoutPanel1 = new TableLayoutPanel();
        lblQuality = new AntdUI.Label();
        cmbQuality = new AntdUI.Select();
        lblFormat = new AntdUI.Label();
        cmbFormat = new AntdUI.Select();
        lblAudioQuality = new AntdUI.Label();
        cmbAudioQuality = new AntdUI.Select();
        lblSubtitles = new AntdUI.Label();
        cmbSubtitles = new AntdUI.Select();
        chkMetadata = new AntdUI.Checkbox();
        lblCookies = new AntdUI.Label();
        txtCookies = new AntdUI.Input();
        btnBrowseCookies = new AntdUI.Button();
        lblOutputDir = new AntdUI.Label();
        txtOutputDir = new AntdUI.Input();
        btnBrowseOutputDir = new AntdUI.Button();
        lblProxy = new AntdUI.Label();
        txtProxy = new AntdUI.Input();
        lblYtDlpPath = new AntdUI.Label();
        txtYtDlpPath = new AntdUI.Input();
        btnBrowseYtDlp = new AntdUI.Button();
        panelButtons = new Panel();
        btnOK = new AntdUI.Button();
        btnCancel = new AntdUI.Button();
        tableLayoutPanel1.SuspendLayout();
        panelButtons.SuspendLayout();
        SuspendLayout();
        // 
        // tableLayoutPanel1
        // 
        tableLayoutPanel1.ColumnCount = 3;
        tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 100F));
        tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 100F));
        tableLayoutPanel1.Controls.Add(lblQuality, 0, 0);
        tableLayoutPanel1.Controls.Add(cmbQuality, 1, 0);
        tableLayoutPanel1.Controls.Add(lblFormat, 0, 1);
        tableLayoutPanel1.Controls.Add(cmbFormat, 1, 1);
        tableLayoutPanel1.Controls.Add(lblAudioQuality, 0, 2);
        tableLayoutPanel1.Controls.Add(cmbAudioQuality, 1, 2);
        tableLayoutPanel1.Controls.Add(lblSubtitles, 0, 3);
        tableLayoutPanel1.Controls.Add(cmbSubtitles, 1, 3);
        tableLayoutPanel1.SetColumnSpan(cmbSubtitles, 2);
        tableLayoutPanel1.Controls.Add(chkMetadata, 0, 4);
        tableLayoutPanel1.SetColumnSpan(chkMetadata, 2);
        tableLayoutPanel1.Controls.Add(lblCookies, 0, 5);
        tableLayoutPanel1.Controls.Add(txtCookies, 1, 5);
        tableLayoutPanel1.Controls.Add(btnBrowseCookies, 2, 5);
        tableLayoutPanel1.Controls.Add(lblOutputDir, 0, 6);
        tableLayoutPanel1.Controls.Add(txtOutputDir, 1, 6);
        tableLayoutPanel1.Controls.Add(btnBrowseOutputDir, 2, 6);
        tableLayoutPanel1.Controls.Add(lblProxy, 0, 7);
        tableLayoutPanel1.Controls.Add(txtProxy, 1, 7);
        tableLayoutPanel1.SetColumnSpan(txtProxy, 2);
        tableLayoutPanel1.Controls.Add(lblYtDlpPath, 0, 8);
        tableLayoutPanel1.Controls.Add(txtYtDlpPath, 1, 8);
        tableLayoutPanel1.Controls.Add(btnBrowseYtDlp, 2, 8);
        tableLayoutPanel1.Dock = DockStyle.Fill;
        tableLayoutPanel1.Location = new Point(0, 0);
        tableLayoutPanel1.Name = "tableLayoutPanel1";
        tableLayoutPanel1.Padding = new Padding(10);
        tableLayoutPanel1.RowCount = 9;
        tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
        tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
        tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
        tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
        tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
        tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
        tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
        tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
        tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
        tableLayoutPanel1.Size = new Size(500, 400);
        tableLayoutPanel1.TabIndex = 0;
        // 
        // lblQuality
        // 
        lblQuality.Anchor = AnchorStyles.Right;
        lblQuality.AutoSize = true;
        lblQuality.Location = new Point(52, 19);
        lblQuality.Name = "lblQuality";
        lblQuality.Size = new Size(38, 20);
        lblQuality.TabIndex = 0;
        lblQuality.Text = "画质";
        // 
        // cmbQuality
        // 
        cmbQuality.Dock = DockStyle.Fill;
        cmbQuality.Items.AddRange(new AntdUI.SelectItem[] { new AntdUI.SelectItem("best"), new AntdUI.SelectItem("2160"), new AntdUI.SelectItem("1080"), new AntdUI.SelectItem("720"), new AntdUI.SelectItem("480"), new AntdUI.SelectItem("360") });
        cmbQuality.Location = new Point(113, 13);
        cmbQuality.Name = "cmbQuality";
        cmbQuality.Size = new Size(274, 28);
        cmbQuality.TabIndex = 1;
        // 
        // lblFormat
        // 
        lblFormat.Anchor = AnchorStyles.Right;
        lblFormat.AutoSize = true;
        lblFormat.Location = new Point(52, 59);
        lblFormat.Name = "lblFormat";
        lblFormat.Size = new Size(38, 20);
        lblFormat.TabIndex = 2;
        lblFormat.Text = "格式";
        // 
        // cmbFormat
        // 
        cmbFormat.Dock = DockStyle.Fill;
        cmbFormat.Items.AddRange(new AntdUI.SelectItem[] { new AntdUI.SelectItem("default"), new AntdUI.SelectItem("mp4"), new AntdUI.SelectItem("webm"), new AntdUI.SelectItem("mkv"), new AntdUI.SelectItem("mp3"), new AntdUI.SelectItem("m4a"), new AntdUI.SelectItem("wav") });
        cmbFormat.Location = new Point(113, 53);
        cmbFormat.Name = "cmbFormat";
        cmbFormat.Size = new Size(274, 28);
        cmbFormat.TabIndex = 3;
        // 
        // lblAudioQuality
        // 
        lblAudioQuality.Anchor = AnchorStyles.Right;
        lblAudioQuality.AutoSize = true;
        lblAudioQuality.Location = new Point(52, 99);
        lblAudioQuality.Name = "lblAudioQuality";
        lblAudioQuality.Size = new Size(38, 20);
        lblAudioQuality.TabIndex = 4;
        lblAudioQuality.Text = "音质";
        // 
        // cmbAudioQuality
        // 
        cmbAudioQuality.Dock = DockStyle.Fill;
        cmbAudioQuality.Items.AddRange(new AntdUI.SelectItem[] { new AntdUI.SelectItem("0"), new AntdUI.SelectItem("2"), new AntdUI.SelectItem("5"), new AntdUI.SelectItem("9") });
        cmbAudioQuality.Location = new Point(113, 93);
        cmbAudioQuality.Name = "cmbAudioQuality";
        cmbAudioQuality.Size = new Size(274, 28);
        cmbAudioQuality.TabIndex = 5;
        // 
        // lblSubtitles
        // 
        lblSubtitles.Anchor = AnchorStyles.Right;
        lblSubtitles.AutoSize = true;
        lblSubtitles.Location = new Point(52, 139);
        lblSubtitles.Name = "lblSubtitles";
        lblSubtitles.Size = new Size(38, 20);
        lblSubtitles.TabIndex = 6;
        lblSubtitles.Text = "字幕";
        // 
        // cmbSubtitles
        // 
        cmbSubtitles.Dock = DockStyle.Fill;
        cmbSubtitles.Items.AddRange(new AntdUI.SelectItem[] { new AntdUI.SelectItem("不下载"), new AntdUI.SelectItem("中文"), new AntdUI.SelectItem("英文") });
        cmbSubtitles.Location = new Point(113, 133);
        cmbSubtitles.Name = "cmbSubtitles";
        cmbSubtitles.Size = new Size(364, 28);
        cmbSubtitles.TabIndex = 7;
        // 
        // chkMetadata
        // 
        chkMetadata.AutoSize = true;
        chkMetadata.Dock = DockStyle.Fill;
        chkMetadata.Location = new Point(13, 173);
        chkMetadata.Name = "chkMetadata";
        chkMetadata.Size = new Size(196, 34);
        chkMetadata.TabIndex = 8;
        chkMetadata.Text = "写入元数据并嵌入封面";

        // 
        // lblCookies
        // 
        lblCookies.Anchor = AnchorStyles.Right;
        lblCookies.AutoSize = true;
        lblCookies.Location = new Point(22, 216);
        lblCookies.Name = "lblCookies";
        lblCookies.Size = new Size(87, 20);
        lblCookies.TabIndex = 9;
        lblCookies.Text = "Cookies文件";
        // 
        // txtCookies
        // 
        txtCookies.Dock = DockStyle.Fill;
        txtCookies.Location = new Point(113, 213);
        txtCookies.Name = "txtCookies";
        txtCookies.Size = new Size(274, 27);
        txtCookies.TabIndex = 10;
        // 
        // btnBrowseCookies
        // 
        btnBrowseCookies.Dock = DockStyle.Fill;
        btnBrowseCookies.Location = new Point(393, 213);
        btnBrowseCookies.Name = "btnBrowseCookies";
        btnBrowseCookies.Size = new Size(94, 34);
        btnBrowseCookies.TabIndex = 11;
        btnBrowseCookies.Text = "浏览...";

        btnBrowseCookies.Click += btnBrowseCookies_Click;
        // 
        // lblOutputDir
        // 
        lblOutputDir.Anchor = AnchorStyles.Right;
        lblOutputDir.AutoSize = true;
        lblOutputDir.Location = new Point(52, 256);
        lblOutputDir.Name = "lblOutputDir";
        lblOutputDir.Size = new Size(58, 20);
        lblOutputDir.TabIndex = 12;
        lblOutputDir.Text = "保存目录";
        // 
        // txtOutputDir
        // 
        txtOutputDir.Dock = DockStyle.Fill;
        txtOutputDir.Location = new Point(113, 253);
        txtOutputDir.Name = "txtOutputDir";
        txtOutputDir.ReadOnly = true;
        txtOutputDir.Size = new Size(274, 27);
        txtOutputDir.TabIndex = 13;
        // 
        // btnBrowseOutputDir
        // 
        btnBrowseOutputDir.Dock = DockStyle.Fill;
        btnBrowseOutputDir.Location = new Point(393, 253);
        btnBrowseOutputDir.Name = "btnBrowseOutputDir";
        btnBrowseOutputDir.Size = new Size(94, 34);
        btnBrowseOutputDir.TabIndex = 14;
        btnBrowseOutputDir.Text = "浏览...";

        btnBrowseOutputDir.Click += btnBrowseOutputDir_Click;
        // 
        // lblProxy
        // 
        lblProxy.Anchor = AnchorStyles.Right;
        lblProxy.AutoSize = true;
        lblProxy.Location = new Point(52, 296);
        lblProxy.Name = "lblProxy";
        lblProxy.Size = new Size(58, 20);
        lblProxy.TabIndex = 15;
        lblProxy.Text = "代理地址";
        // 
        // txtProxy
        // 
        txtProxy.Dock = DockStyle.Fill;
        txtProxy.Location = new Point(113, 293);
        txtProxy.Name = "txtProxy";
        txtProxy.Size = new Size(364, 27);
        txtProxy.TabIndex = 16;
        // 
        // lblYtDlpPath
        // 
        lblYtDlpPath.Anchor = AnchorStyles.Right;
        lblYtDlpPath.AutoSize = true;
        lblYtDlpPath.Location = new Point(22, 336);
        lblYtDlpPath.Name = "lblYtDlpPath";
        lblYtDlpPath.Size = new Size(87, 20);
        lblYtDlpPath.TabIndex = 17;
        lblYtDlpPath.Text = "yt-dlp路径";
        // 
        // txtYtDlpPath
        // 
        txtYtDlpPath.Dock = DockStyle.Fill;
        txtYtDlpPath.Location = new Point(113, 333);
        txtYtDlpPath.Name = "txtYtDlpPath";
        txtYtDlpPath.Size = new Size(274, 27);
        txtYtDlpPath.TabIndex = 18;
        // 
        // btnBrowseYtDlp
        // 
        btnBrowseYtDlp.Dock = DockStyle.Fill;
        btnBrowseYtDlp.Location = new Point(393, 333);
        btnBrowseYtDlp.Name = "btnBrowseYtDlp";
        btnBrowseYtDlp.Size = new Size(94, 34);
        btnBrowseYtDlp.TabIndex = 19;
        btnBrowseYtDlp.Text = "浏览...";

        btnBrowseYtDlp.Click += btnBrowseYtDlp_Click;
        // 
        // panelButtons
        // 
        panelButtons.Controls.Add(btnCancel);
        panelButtons.Controls.Add(btnOK);
        panelButtons.Dock = DockStyle.Bottom;
        panelButtons.Location = new Point(0, 400);
        panelButtons.Name = "panelButtons";
        panelButtons.Size = new Size(500, 50);
        panelButtons.TabIndex = 1;
        // 
        // btnOK
        // 
        btnOK.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        btnOK.Location = new Point(306, 10);
        btnOK.Name = "btnOK";
        btnOK.Size = new Size(80, 30);
        btnOK.TabIndex = 0;
        btnOK.Text = "确定";
        btnOK.Type = AntdUI.TTypeMini.Primary;
        btnOK.Click += btnOK_Click;
        // 
        // btnCancel
        // 
        btnCancel.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        btnCancel.Location = new Point(396, 10);
        btnCancel.Name = "btnCancel";
        btnCancel.Size = new Size(80, 30);
        btnCancel.TabIndex = 1;
        btnCancel.Text = "取消";

        btnCancel.Click += btnCancel_Click;
        // 
        // SettingsForm
        // 
        AutoScaleDimensions = new SizeF(8F, 20F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(500, 450);
        Controls.Add(tableLayoutPanel1);
        Controls.Add(panelButtons);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "SettingsForm";
        StartPosition = FormStartPosition.CenterParent;
        Text = "高级设置";
        tableLayoutPanel1.ResumeLayout(false);
        tableLayoutPanel1.PerformLayout();
        panelButtons.ResumeLayout(false);
        ResumeLayout(false);
    }

    #endregion

    private TableLayoutPanel tableLayoutPanel1;
    private AntdUI.Label lblQuality;
    private AntdUI.Select cmbQuality;
    private AntdUI.Label lblFormat;
    private AntdUI.Select cmbFormat;
    private AntdUI.Label lblAudioQuality;
    private AntdUI.Select cmbAudioQuality;
    private AntdUI.Label lblSubtitles;
    private AntdUI.Select cmbSubtitles;
    private AntdUI.Checkbox chkMetadata;
    private AntdUI.Label lblCookies;
    private AntdUI.Input txtCookies;
    private AntdUI.Button btnBrowseCookies;
    private AntdUI.Label lblOutputDir;
    private AntdUI.Input txtOutputDir;
    private AntdUI.Button btnBrowseOutputDir;
    private AntdUI.Label lblProxy;
    private AntdUI.Input txtProxy;
    private AntdUI.Label lblYtDlpPath;
    private AntdUI.Input txtYtDlpPath;
    private AntdUI.Button btnBrowseYtDlp;
    private Panel panelButtons;
    private AntdUI.Button btnOK;
    private AntdUI.Button btnCancel;
}
