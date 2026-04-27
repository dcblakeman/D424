# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

This is C_971 (EduTrack) — a WGU D424 Software Engineering Capstone project. It's a .NET MAUI cross-platform mobile/desktop app for tracking college courses, terms, assessments, and instructors. Built with C# using strict MVVM with CommunityToolkit.Mvvm source generators.

## Build & Run Commands

```bash
# Build the project
dotnet build C_971.csproj

# Run on Windows (default target on Windows dev machine)
dotnet run --project C_971.csproj -f net10.0-windows10.0.19041.0

# Run on Android (requires emulator or device)
dotnet run --project C_971.csproj -f net10.0-android

# Clean build artifacts
dotnet clean C_971.csproj
```

There is no test project, linter, or formatter configured. The `.editorconfig` only suppresses CS8602 nullable warnings.

## Architecture

### MVVM Pattern

- **Views** (`Views/`): XAML files with minimal code-behind. Code-behind only sets `BindingContext` and handles `OnAppearing` lifecycle.
- **ViewModels** (`ViewModels/`): All inherit `ObservableObject`. Use `[ObservableProperty]` for bindable properties, `[RelayCommand]` for commands, `[QueryProperty]` for receiving navigation parameters. No business logic in views.
- **Models** (`Models/`): sqlite-net-pcl table-mapped POCOs with navigation properties. Enums in `Enums.cs`.
- **Services** (`Services/`): Registered in DI. Singletons except Login/AddNote/ViewNotes ViewModels which are transients.

### Navigation

Shell-based navigation via `AppShell.xaml.cs` route registration. ViewModels pass data through `Dictionary<string, object>` parameters in `GoToAsync`. Each ViewModel uses a dual-property pattern (e.g., `Course`/`NewCourse`, `User`/`NewUser`) to track original vs. working copies for edit/save semantics.

Flow: `LoginView` → `AcademicTermListView` → `CourseListView` → `CourseDetailsView` → branches to `AddNoteView`, `ViewNotesView`, `CourseInstructorView`, `AssessmentSelectionView` → `PerformanceAssessmentView`/`ObjectiveAssessmentView` → `ReportView`.

### Database

SQLite via `sqlite-net-pcl`. Database file: `CollegeCourseTracker.db3` in `FileSystem.AppDataDirectory`. Schema is lazy-initialized in `DatabaseService.InitializeAsync()` via `CreateTableAsync<T>()`. 7 tables: `academic_term`, `course`, `course_note`, `course_assessment`, `course_instructor`, `user`, `user_course`. Some queries use raw SQL for joins (see `GetCoursesWithDetailsAsync`, `GetAssessmentsForUserAndTermAsync`).

### Services

- **DatabaseService**: Monolithic data access layer — all CRUD for every entity lives here.
- **NotificationService**: Schedules/cancels local notifications using `Plugin.LocalNotification`. Notification IDs use offset patterns (1000+course.Id for start, 2000+ for end, 10000+ for assessment due).
- **PermissionService**: Android 13+ notification permission handling.

## Key Conventions

- CommunityToolkit.Mvvm source generators are used throughout — property changes auto-raise `PropertyChanged` via `[ObservableProperty]` partial methods (e.g., `OnCourseNameChanged` partial method fires when `CourseName` changes).
- Passwords are hashed with BCrypt (`BCrypt.Net-Next`).
- `CourseService.cs` exists on disk but is **excluded from compilation** (`<Compile Remove>` in .csproj) — do not reference it.
- Dual-property pattern on ViewModels: `X` holds the original value, `NewX` holds the working/edit copy. Changes are committed by assigning `NewX` back to `X` on save.

## Target Frameworks

.NET 10.0 MAUI targeting Android (min 21), iOS (min 15), MacCatalyst (min 15), Windows (min 10.0.17763). Windows is configured as unpackaged (`WindowsPackageType=None`).