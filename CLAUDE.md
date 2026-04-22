# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

YtDlpDownloader is a .NET 8 WinForms desktop application wrapping [yt-dlp](https://github.com/yt-dlp/yt-dlp) for downloading videos from YouTube, Bilibili, and other supported sites. It uses [AntdUI](https://github.com/AntdUI/AntdUI) for Ant Design-style UI components.

## Build & Run

```bash
dotnet restore
dotnet build -c Release
dotnet run
```

Compiled output goes to `bin/Release/net8.0-windows/`.

## Architecture

### Key Files

| File | Purpose |
|------|---------|
| [Form1.cs](Form1.cs) | Main window: URL input, download orchestration, task list, right-click context menu |
| [Form1.Designer.cs](Form1.Designer.cs) | UI layout — `AntdUI.PageHeader`, `Input`, `Button`, `Table` |
| [DownloadTask.cs](DownloadTask.cs) | Task model with `INotifyPropertyChanged` via `AntdUI.NotifyProperty` |
| [AppConfig.cs](AppConfig.cs) | `AppSettings` model + `AppConfig` static class for settings/records persistence |
| [SettingsForm.cs](SettingsForm.cs) | Advanced settings dialog (quality, format, proxy, subtitles, cookies, yt-dlp path) |
| [DownloadRecord.cs](DownloadRecord.cs) | Download history record model |
| [Program.cs](Program.cs) | Entry point |

### Data Flow

1. User pastes URL → clicks Download → `btnDownload_Click` creates a `DownloadTask` and queues it
2. `DownloadVideo()` runs on a background thread via `Task.Run`
3. yt-dlp is spawned as a child `Process`; stdout/stderr are parsed for progress percentage via regex
4. Progress updates are marshalled back to UI thread via `Invoke()`
5. Completed downloads are persisted to `downloaded.csv`; settings to `settings.json`

### Configuration

- **Settings**: serialized as JSON to `settings.json` (see `AppConfig.SaveSettings` / `Load`)
- **Records**: CSV format in `downloaded.csv` with quoted fields (legacy JSON format `downloaded.json` is auto-migrated)
- `AppConfig.Settings` is a singleton `AppSettings` instance accessed statically throughout the app

### yt-dlp Integration

yt-dlp runs as an external process. The `BuildArguments()` method in [Form1.cs](Form1.cs) constructs CLI args based on current settings (quality, format, subtitles, metadata, cookies, proxy). `PYTHONUNBUFFERED=1` is set in the process environment for real-time output.

### UI Framework

Uses AntdUI components (`AntdUI.Window`, `AntdUI.Table`, `AntdUI.Input`, etc.) — not standard WinForms controls. The `Table` component is bound to a `BindingList<DownloadTask>` for automatic data binding.

## Important Notes

- All UI updates from background threads must use `Invoke()` to marshal to the UI thread
- The `_processes` dictionary maps `DownloadTask` → `Process` for pause/resume/delete operations
- Pausing works by setting `IsCancelled = true` and killing the yt-dlp process; resuming creates a new process
- Duplicate download prevention is based on URL matching via `AppConfig.IsDownloaded()`
- File naming includes uploader prefix: `[Uploader] Title.ext`
