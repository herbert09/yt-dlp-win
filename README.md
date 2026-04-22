# YtDlpDownloader

基于 [.NET 8](https://dotnet.microsoft.com/) WinForms + [yt-dlp](https://github.com/yt-dlp/yt-dlp) 的桌面视频下载工具，使用 [AntdUI](https://github.com/AntdUI/AntdUI) 实现 Ant Design 风格界面。

## 功能特性

- **视频下载**：支持 YouTube、Bilibili 等 yt-dlp 支持的所有站点
- **画质选择**：best / 4K / 1080P / 720P / 480P / 360P
- **格式转换**：mp4、webm、mkv、mp3、m4a、wav
- **字幕下载**：支持手动字幕 + 平台自动生成字幕（AI 语音识别），可嵌入视频
- **博主名称**：下载的文件名自动带上 `[博主名]` 前缀
- **下载管理**：暂停、继续、删除任务，按下载时间倒序排列
- **右键菜单**：复制链接、打开所在目录并自动选中文件
- **历史记录**：启动时自动加载已下载记录，删除任务会同步清理记录
- **Ant Design 风格**：清新现代、无边框窗体、矢量图形绘制

## 环境要求

- Windows 10 / 11
- [.NET 8 Runtime](https://dotnet.microsoft.com/download/dotnet/8.0)
- [yt-dlp](https://github.com/yt-dlp/yt-dlp/releases)（已安装并在 PATH 中，或在设置里指定完整路径）
- （可选）[FFmpeg](https://ffmpeg.org/download.html)（用于合并音视频、嵌入字幕）

## 使用方法

1. 克隆仓库并编译运行，或下载 Release 压缩包解压运行
2. 首次使用建议点击**高级设置**，配置：
   - 保存目录
   - 代理地址（如有需要）
   - yt-dlp 程序路径（如果不在系统 PATH 中）
3. 粘贴视频链接，点击**下载**
4. 在历史列表中右键任务可进行更多操作

## 项目结构

```
YtDlpDownloader/
├── AppConfig.cs          # 配置管理与历史记录持久化
├── DownloadRecord.cs     # 下载记录模型
├── DownloadTask.cs       # 下载任务模型（支持数据绑定）
├── Form1.cs              # 主界面（AntdUI.Window）
├── Form1.Designer.cs     # 主界面布局
├── Program.cs            # 程序入口
├── SettingsForm.cs       # 高级设置窗口
├── SettingsForm.Designer.cs
└── YtDlpDownloader.csproj
```

## 构建

```bash
dotnet restore
dotnet build -c Release
```

编译输出位于 `bin/Release/net8.0-windows/`。

## 配置说明

配置保存在程序同目录下的 `settings.json` 中：

| 配置项 | 说明 |
|--------|------|
| `OutputDirectory` | 下载文件保存目录 |
| `Proxy` | 代理地址，如 `socks5://127.0.0.1:10808` |
| `Quality` | 视频画质 |
| `Format` | 输出格式 |
| `AudioQuality` | 音频质量（仅音频格式时生效） |
| `SubtitleLang` | 字幕语言：`zh-CN`（中文）、`en`（英文）、空字符串（不下载） |
| `WriteMetadata` | 是否写入元数据并嵌入封面 |
| `CookiesPath` | Cookies 文件路径 |
| `YtDlpPath` | yt-dlp 可执行文件路径，默认为 `yt-dlp` |

## 依赖

- [AntdUI](https://www.nuget.org/packages/AntdUI/) 2.3.9 — Ant Design WinForms UI 组件库
- [yt-dlp](https://github.com/yt-dlp/yt-dlp) — 视频下载核心引擎

## 许可证

MIT
