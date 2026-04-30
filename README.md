# College Course Tracker

College Course Tracker is a .NET MAUI mobile app for organizing academic terms, courses, assessments, notes, instructors, and reminders in one place.

## Download

- Android APK: [com.dcblakeman.collegecoursetracker-v1.0.3.apk](Releases/v1.0.3/com.dcblakeman.collegecoursetracker-v1.0.3.apk)
- Android App Bundle: [com.dcblakeman.collegecoursetracker-v1.0.3.aab](Releases/v1.0.3/com.dcblakeman.collegecoursetracker-v1.0.3.aab)
- GitLab Pages deployment: [College Course Tracker deployment page](https://wgu-gitlab-environment.gitlab.io/student-repos/dcblakeman/d424-software-engineering-capstone/)

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
- `deploy/` - GitLab Pages automation, export scripts, and deployment site assets
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

## Deployment

College Course Tracker is deployed as a public distribution portal through GitLab Pages. The deployment publishes a live HTTPS page with the current Android install link, release metadata, and repository references.

### Deployment automation

- `deploy/build-pages.ps1` generates the static Pages site
- `.gitlab-ci.yml` publishes the generated `public/` directory on pushes to `master`
- `deploy/Dockerfile` packages the same deployment site into an Nginx container image for portable hosting
- `deploy/export-project.ps1` creates a compressed project export for submission and archival

## Release status

- App name: `College Course Tracker`
- Android application ID: `com.dcblakeman.collegecoursetracker`
- Current Android package version: `v1.0.3`

## Notes

This repository contains the source code and supporting course materials for the app. The latest Android release artifacts are tracked under `Releases/`.
    
