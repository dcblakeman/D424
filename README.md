# College Course Tracker

College Course Tracker is a .NET MAUI mobile app for organizing academic terms, courses, assessments, notes, instructors, and reminders in one place.

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

- `Views/` - MAUI pages
- `ViewModels/` - MVVM presentation logic
- `Models/` - domain models and data shapes
- `Services/` - persistence, permissions, notifications, and app services
- `Resources/` - icons, splash assets, fonts, and images
- `Store/` - release and store submission notes

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
- Current store bundle is generated locally and not committed to the repository

## Notes

This repository contains the source code and supporting course materials for the app. Generated build artifacts such as `.aab` and `.apk` files are intentionally excluded from source control.
    
