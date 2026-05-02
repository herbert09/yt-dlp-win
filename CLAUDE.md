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


# CLAUDE.md

Behavioral guidelines to reduce common LLM coding mistakes. Merge with project-specific instructions as needed.

**Tradeoff:** These guidelines bias toward caution over speed. For trivial tasks, use judgment.

## 1. Think Before Coding

**Don't assume. Don't hide confusion. Surface tradeoffs.**

Before implementing:
- State your assumptions explicitly. If uncertain, ask.
- If multiple interpretations exist, present them - don't pick silently.
- If a simpler approach exists, say so. Push back when warranted.
- If something is unclear, stop. Name what's confusing. Ask.

## 2. Simplicity First

**Minimum code that solves the problem. Nothing speculative.**

- No features beyond what was asked.
- No abstractions for single-use code.
- No "flexibility" or "configurability" that wasn't requested.
- No error handling for impossible scenarios.
- If you write 200 lines and it could be 50, rewrite it.

Ask yourself: "Would a senior engineer say this is overcomplicated?" If yes, simplify.

## 3. Surgical Changes

**Touch only what you must. Clean up only your own mess.**

When editing existing code:
- Don't "improve" adjacent code, comments, or formatting.
- Don't refactor things that aren't broken.
- Match existing style, even if you'd do it differently.
- If you notice unrelated dead code, mention it - don't delete it.

When your changes create orphans:
- Remove imports/variables/functions that YOUR changes made unused.
- Don't remove pre-existing dead code unless asked.

The test: Every changed line should trace directly to the user's request.

## 4. Goal-Driven Execution

**Define success criteria. Loop until verified.**

Transform tasks into verifiable goals:
- "Add validation" → "Write tests for invalid inputs, then make them pass"
- "Fix the bug" → "Write a test that reproduces it, then make it pass"
- "Refactor X" → "Ensure tests pass before and after"

For multi-step tasks, state a brief plan:
```
1. [Step] → verify: [check]
2. [Step] → verify: [check]
3. [Step] → verify: [check]
```

Strong success criteria let you loop independently. Weak criteria ("make it work") require constant clarification.

---

**These guidelines are working if:** fewer unnecessary changes in diffs, fewer rewrites due to overcomplication, and clarifying questions come before implementation rather than after mistakes.