using ReaLTaiizor.Controls;
using ReaLTaiizor.Forms;

namespace YtDlpDownloader;

partial class Form1 : MaterialForm
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
        txtUrl = new MaterialTextBoxEdit();
        btnDownload = new MaterialButton();
        btnAdvancedSettings = new MaterialButton();
        dgvTasks = new DataGridView();
        colTitle = new DataGridViewTextBoxColumn();
        colStatus = new DataGridViewTextBoxColumn();
        colProgress = new DataGridViewTextBoxColumn();
        colFileSize = new DataGridViewTextBoxColumn();
        colDownloadTime = new DataGridViewTextBoxColumn();
        ((System.ComponentModel.ISupportInitialize)dgvTasks).BeginInit();
        SuspendLayout();

        // txtUrl
        txtUrl.AnimateReadOnly = false;
        txtUrl.AutoCompleteMode = AutoCompleteMode.None;
        txtUrl.AutoCompleteSource = AutoCompleteSource.None;
        txtUrl.BackgroundImageLayout = ImageLayout.None;
        txtUrl.CharacterCasing = CharacterCasing.Normal;
        txtUrl.Depth = 0;
        txtUrl.Font = new Font("Microsoft Sans Serif", 16F, FontStyle.Regular, GraphicsUnit.Pixel);
        txtUrl.HideSelection = true;
        txtUrl.Hint = "粘贴视频链接...";
        txtUrl.LeadingIcon = null;
        txtUrl.Location = new Point(20, 80);
        txtUrl.MaxLength = 32767;
        txtUrl.MouseState = ReaLTaiizor.Helper.MaterialDrawHelper.MaterialMouseState.OUT;
        txtUrl.Name = "txtUrl";
        txtUrl.PasswordChar = '\0';
        txtUrl.PrefixSuffixText = null;
        txtUrl.ReadOnly = false;
        txtUrl.RightToLeft = RightToLeft.No;
        txtUrl.SelectedText = "";
        txtUrl.SelectionLength = 0;
        txtUrl.SelectionStart = 0;
        txtUrl.ShortcutsEnabled = true;
        txtUrl.Size = new Size(620, 48);
        txtUrl.TabIndex = 0;
        txtUrl.TabStop = false;
        txtUrl.TextAlign = HorizontalAlignment.Left;
        txtUrl.TrailingIcon = null;
        txtUrl.UseSystemPasswordChar = false;

        // btnDownload
        btnDownload.AutoSize = false;
        btnDownload.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        btnDownload.Density = MaterialButton.MaterialButtonDensity.Default;
        btnDownload.Depth = 0;
        btnDownload.HighEmphasis = true;
        btnDownload.Icon = null;
        btnDownload.Location = new Point(660, 80);
        btnDownload.Margin = new Padding(4, 6, 4, 6);
        btnDownload.MouseState = ReaLTaiizor.Helper.MaterialDrawHelper.MaterialMouseState.HOVER;
        btnDownload.Name = "btnDownload";
        btnDownload.NoAccentTextColor = Color.Empty;
        btnDownload.Size = new Size(100, 48);
        btnDownload.TabIndex = 1;
        btnDownload.Text = "下载";
        btnDownload.Type = MaterialButton.MaterialButtonType.Contained;
        btnDownload.UseAccentColor = false;
        btnDownload.UseVisualStyleBackColor = true;
        btnDownload.Click += btnDownload_Click;

        // btnAdvancedSettings
        btnAdvancedSettings.AutoSize = false;
        btnAdvancedSettings.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        btnAdvancedSettings.Density = MaterialButton.MaterialButtonDensity.Default;
        btnAdvancedSettings.Depth = 0;
        btnAdvancedSettings.HighEmphasis = false;
        btnAdvancedSettings.Icon = null;
        btnAdvancedSettings.Location = new Point(770, 80);
        btnAdvancedSettings.Margin = new Padding(4, 6, 4, 6);
        btnAdvancedSettings.MouseState = ReaLTaiizor.Helper.MaterialDrawHelper.MaterialMouseState.HOVER;
        btnAdvancedSettings.Name = "btnAdvancedSettings";
        btnAdvancedSettings.NoAccentTextColor = Color.Empty;
        btnAdvancedSettings.Size = new Size(110, 48);
        btnAdvancedSettings.TabIndex = 2;
        btnAdvancedSettings.Text = "高级设置";
        btnAdvancedSettings.Type = MaterialButton.MaterialButtonType.Outlined;
        btnAdvancedSettings.UseAccentColor = false;
        btnAdvancedSettings.UseVisualStyleBackColor = true;
        btnAdvancedSettings.Click += btnAdvancedSettings_Click;

        // dgvTasks
        dgvTasks.AllowUserToAddRows = false;
        dgvTasks.AllowUserToDeleteRows = false;
        dgvTasks.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        dgvTasks.BackgroundColor = Color.FromArgb(50, 50, 50);
        dgvTasks.BorderStyle = BorderStyle.None;
        dgvTasks.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(55, 71, 79);
        dgvTasks.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
        dgvTasks.ColumnHeadersDefaultCellStyle.Font = new Font("Microsoft YaHei", 10F, FontStyle.Bold);
        dgvTasks.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        dgvTasks.Columns.AddRange(new DataGridViewColumn[] { colTitle, colStatus, colProgress, colFileSize, colDownloadTime });
        dgvTasks.Dock = DockStyle.None;
        dgvTasks.Location = new Point(20, 150);
        dgvTasks.MultiSelect = false;
        dgvTasks.Name = "dgvTasks";
        dgvTasks.ReadOnly = true;
        dgvTasks.RowHeadersVisible = false;
        dgvTasks.RowTemplate.Height = 36;
        dgvTasks.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dgvTasks.Size = new Size(860, 330);
        dgvTasks.TabIndex = 3;
        dgvTasks.DefaultCellStyle.BackColor = Color.FromArgb(50, 50, 50);
        dgvTasks.DefaultCellStyle.ForeColor = Color.White;
        dgvTasks.DefaultCellStyle.SelectionBackColor = Color.FromArgb(69, 90, 100);
        dgvTasks.DefaultCellStyle.SelectionForeColor = Color.White;
        dgvTasks.EnableHeadersVisualStyles = false;
        dgvTasks.GridColor = Color.FromArgb(70, 70, 70);

        // colTitle
        colTitle.DataPropertyName = "Title";
        colTitle.HeaderText = "标题";
        colTitle.MinimumWidth = 300;
        colTitle.Name = "colTitle";
        colTitle.ReadOnly = true;

        // colStatus
        colStatus.DataPropertyName = "Status";
        colStatus.HeaderText = "状态";
        colStatus.Name = "colStatus";
        colStatus.ReadOnly = true;
        colStatus.Width = 80;

        // colProgress
        colProgress.DataPropertyName = "Progress";
        colProgress.HeaderText = "进度";
        colProgress.Name = "colProgress";
        colProgress.ReadOnly = true;
        colProgress.Width = 100;

        // colDownloadTime
        colDownloadTime.DataPropertyName = "DownloadTime";
        colDownloadTime.HeaderText = "下载时间";
        colDownloadTime.Name = "colDownloadTime";
        colDownloadTime.ReadOnly = true;
        colDownloadTime.Width = 150;

        // colFileSize
        colFileSize.DataPropertyName = "FileSize";
        colFileSize.HeaderText = "文件大小";
        colFileSize.Name = "colFileSize";
        colFileSize.ReadOnly = true;
        colFileSize.Width = 100;

        // Form1
        AutoScaleDimensions = new SizeF(8F, 20F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(900, 500);
        Controls.Add(dgvTasks);
        Controls.Add(btnAdvancedSettings);
        Controls.Add(btnDownload);
        Controls.Add(txtUrl);
        Name = "Form1";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "yt-dlp 视频下载器";
        ((System.ComponentModel.ISupportInitialize)dgvTasks).EndInit();
        ResumeLayout(false);
    }

    private MaterialTextBoxEdit txtUrl;
    private MaterialButton btnDownload;
    private MaterialButton btnAdvancedSettings;
    private DataGridView dgvTasks;
    private DataGridViewTextBoxColumn colTitle;
    private DataGridViewTextBoxColumn colStatus;
    private DataGridViewTextBoxColumn colProgress;
    private DataGridViewTextBoxColumn colDownloadTime;
    private DataGridViewTextBoxColumn colFileSize;
}
