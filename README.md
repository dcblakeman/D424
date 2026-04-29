# College Course Tracker

College Course Tracker is a .NET MAUI mobile app for organizing academic terms, courses, assessments, notes, instructors, and reminders in one place.

## Download

- Android APK: [com.dcblakeman.collegecoursetracker-v1.0.3.apk](Releases/v1.0.3/com.dcblakeman.collegecoursetracker-v1.0.3.apk)
- Android App Bundle: [com.dcblakeman.collegecoursetracker-v1.0.3.aab](Releases/v1.0.3/com.dcblakeman.collegecoursetracker-v1.0.3.aab)

## What it does

- Create and manage academic terms
- Add courses with status and date tracking
- Track objective and performance assessments
- Save and review course notes
- Store instructor details
- Generate reports from saved academic data
- Use reminders to stay on top of important dates

## Tech stack

- .NET MAUI
- C#
- SQLite
- CommunityToolkit.MVVM
- Plugin.LocalNotification

## Project structure

- `Docs/` - wireframes, release prep, and supporting course documents
- `Views/` - MAUI pages
- `ViewModels/` - MVVM presentation logic
- `Models/` - domain models and data shapes
- `Services/` - persistence, permissions, notifications, and app services
- `Resources/` - icons, splash assets, fonts, and images

## Local development

### Build for Windows

```powershell
dotnet build C_971.csproj -f net10.0-windows10.0.19041.0
```

### Run on Windows

```powershell
dotnet run --project C_971.csproj -f net10.0-windows10.0.19041.0
```

### Build Android release bundle

```powershell
dotnet publish C_971.csproj -f net10.0-android -c Release
```

## Release status

- App name: `College Course Tracker`
- Android application ID: `com.dcblakeman.collegecoursetracker`
- Current Android package version: `v1.0.3`

## Notes

This repository contains the source code and supporting course materials for the app. The latest Android release artifacts are tracked under `Releases/`.
    
