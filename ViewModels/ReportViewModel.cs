using C_971.Models;
using C_971.Services;
using CommunityToolkit.Maui.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Storage;
using System;
using System.Collections.Generic;
using System.Text;

namespace C_971.ViewModels
{
    [QueryProperty(nameof(Course), "course")]
    [QueryProperty(nameof(Term), "term")]
    [QueryProperty(nameof(User), "user")]
    public partial class ReportViewModel : ObservableObject
    {

        private DatabaseService _database;

        [ObservableProperty]
        public User user = new();

        [ObservableProperty]
        public User newUser;

        [ObservableProperty]
        private Course course;

        [ObservableProperty]
        private Course newCourse;

        [ObservableProperty]
        private AcademicTerm term;

        [ObservableProperty]
        private AcademicTerm newTerm;

        [ObservableProperty]
        private string reportText;

        [ObservableProperty]
        private bool isBusy;

        private const string REPORTS_FOLDER_KEY = "reports_default_folder";

        public ReportViewModel(DatabaseService database)
        {
            _database = database;
        }

        partial void OnUserChanged(User value)
        {
            NewUser = value;
        }

        partial void OnCourseChanged(Course value)
        {
            NewCourse = value;
        }

        partial void OnTermChanged(AcademicTerm value)
        {
            NewTerm = value;
            Shell.Current.DisplayAlertAsync("Updated Values", $"New Term: {Term}", "OK");
        }

        [RelayCommand]
        public async Task<string> GenerateUserAssessmentReport()
        {
            try
            {
                if (Term.Id == 0)
                {
                    await Shell.Current.DisplayAlertAsync("No Term Selected",
                        "Please select a term first.", "OK");
                    return string.Empty;
                }

                var assessments = await _database.GetAssessmentsForUserAndTermAsync(NewUser.Id, NewTerm.Id);

                await Shell.Current.DisplayAlertAsync("Wait", $"Number of assessments: {assessments.Count}", "OK");

                if (!assessments.Any())
                {
                    await Shell.Current.DisplayAlertAsync("No Assessments",
                        "No assessments found for the selected term.", "OK");
                    return string.Empty;
                }

                // Generate Report
                foreach (CourseAssessment assessment in assessments)
                {
                    ReportText += $"Assessment: {assessment.Name}\n" +
                                    $"Type: {assessment.Type}\n" +
                                    $"Start Date: {assessment.StartDate:d}\n" +
                                    $"End Date: {assessment.EndDate:d}\n" +
                                    $"Status: {assessment.Status}\n\n";
                }

                return ReportText;
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlertAsync("Error", ex.Message, "OK");
                return string.Empty;
            }
        }

        private string GenerateAssessmentsReportContent(List<UserAssessment> assessments)
        {
            var report = new StringBuilder();

            // Header
            report.AppendLine("ASSESSMENTS REPORT");
            report.AppendLine($"Generated: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            report.AppendLine($"Total Assessments: {assessments.Count}");
            report.AppendLine(new string('=', 60));
            report.AppendLine();

            if (assessments.Count == 0)
            {
                report.AppendLine("No assessments found.");
                return report.ToString();
            }

            // Group by course
            var groupedByCourse = assessments.GroupBy(a => a.UserCourse?.Course?.Name ?? "Unknown Course")
                                            .OrderBy(g => g.Key);

            foreach (var courseGroup in groupedByCourse)
            {
                report.AppendLine($"COURSE: {courseGroup.Key}");
                report.AppendLine(new string('-', 40));

                foreach (var assessment in courseGroup.OrderBy(a => a.Assessment?.Name))
                {
                    report.AppendLine($"  Assessment: {assessment.Assessment?.Name ?? "Unknown Assessment"}");
                    report.AppendLine($"  Type: {assessment.Assessment?.Type ?? AssessmentType.Unknown}");
                    report.AppendLine($"  Grade: {assessment.Grade ?? FinalGrade.NotGraded}");
                    report.AppendLine($"  Status: {(assessment.IsCompleted ? "Completed" : "In Progress")}");

                    if (assessment.CompletedDate.HasValue)
                    {
                        report.AppendLine($"  Completed: {assessment.CompletedDate.Value:yyyy-MM-dd}");
                    }

                    report.AppendLine();
                }
                report.AppendLine();
            }

            // Summary Statistics
            report.AppendLine("SUMMARY STATISTICS");
            report.AppendLine(new string('=', 30));
            report.AppendLine($"Total Assessments: {assessments.Count}");
            report.AppendLine($"Completed: {assessments.Count(a => a.IsCompleted)}");
            report.AppendLine($"In Progress: {assessments.Count(a => !a.IsCompleted)}");
            report.AppendLine($"Graded: {assessments.Count(a => a.Grade.HasValue)}");

            // Grade breakdown (if you want it)
            var gradedAssessments = assessments.Where(a => a.Grade.HasValue).ToList();
            if (gradedAssessments.Count > 0)
            {
                report.AppendLine();
                report.AppendLine("GRADE BREAKDOWN");
                report.AppendLine(new string('-', 20));

                var gradeGroups = gradedAssessments.GroupBy(a => a.Grade.Value)
                                                  .OrderByDescending(g => g.Key);

                foreach (var gradeGroup in gradeGroups)
                {
                    report.AppendLine($"{gradeGroup.Key}: {gradeGroup.Count()} assessment(s)");
                }
            }

            return report.ToString();
        }

        // Ran in GenerateCoursesReportToFile Method
        private string GenerateCoursesReport(List<UserCourse> userCourses)
        {
            StringBuilder reportBuilder = new StringBuilder();
            reportBuilder.AppendLine("=== My Courses Report ===");
            reportBuilder.AppendLine($"Generated on: {DateTime.Now}");
            reportBuilder.AppendLine();
            foreach (var uc in userCourses)
            {
                reportBuilder.AppendLine($"Course: {uc.Course.Name}");
                reportBuilder.AppendLine($"Term: {uc.Course.Term.Name}");
                reportBuilder.AppendLine($"Status: {uc.Status}");
                reportBuilder.AppendLine($"Start Date: {uc.StartDate:d}");
                reportBuilder.AppendLine($"End Date: {uc.EndDate:d}");
                reportBuilder.AppendLine($"Grade: {uc.Grade ?? FinalGrade.NotGraded}");
                reportBuilder.AppendLine(new string('-', 30));
            }
            return reportBuilder.ToString();
        }

        [RelayCommand]
        public async Task ViewReports()
        {
            try
            {
                string reportsPath = Preferences.Get(REPORTS_FOLDER_KEY, string.Empty);
                var files = Directory.GetFiles(reportsPath);

                if (files.Length == 0)
                {
                    await Shell.Current.DisplayAlertAsync("No Reports",
                        "No reports have been generated yet.", "OK");
                    return;
                }

                string fileList = string.Join("\n", files.Select(Path.GetFileName));
                await Shell.Current.DisplayAlertAsync("Available Reports",
                    $"Found {files.Length} reports:\n\n{fileList}", "OK");
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlertAsync("Error", ex.Message, "OK");
            }
        }

        [RelayCommand]
        private async Task ShareReports()
        {
            try
            {
                string reportsPath = Preferences.Get(REPORTS_FOLDER_KEY, string.Empty);
                var files = Directory.GetFiles(reportsPath).ToList();

                if (files.Count > 0)
                {
                    await Share.RequestAsync(new ShareMultipleFilesRequest
                    {
                        Title = "EduTrack Reports",
                        Files = files.Select(f => new ShareFile(f)).ToList()
                    });
                }
                else
                {
                    await Shell.Current.DisplayAlertAsync("No Reports",
                        "No reports to share.", "OK");
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlertAsync("Error", ex.Message, "OK");
            }
        }
        [RelayCommand]
        private async Task ShowCurrentReportsFolder()
        {
            string savedPath = Preferences.Get(REPORTS_FOLDER_KEY, "Not set");

            if (savedPath == "Not set")
            {
                await Shell.Current.DisplayAlertAsync("Reports Folder",
                    "No default folder has been set. You'll be prompted to choose one when generating a report.", "OK");
            }
            else if (Directory.Exists(savedPath))
            {
                await Shell.Current.DisplayAlertAsync("Current Reports Folder", savedPath, "OK");
            }
            else
            {
                await Shell.Current.DisplayAlertAsync("Reports Folder",
                    $"Previously set folder no longer exists:\n{savedPath}\n\nYou'll be prompted to choose a new one.", "OK");
            }
        }

        [RelayCommand]
        public async Task SaveAssessmentReport()
        {
            try
            {
                var reportContent = await GenerateUserAssessmentReport();

                // DEBUG: Check if content is actually there
                if (string.IsNullOrEmpty(reportContent))
                {
                    await Shell.Current.DisplayAlertAsync("Debug", "Report content is empty!", "OK");
                    return;
                }

                var stream = new MemoryStream(Encoding.UTF8.GetBytes(reportContent));

                var fileName = $"AssessmentReport_{DateTime.Now:yyyyMMdd_HHmmss}.txt";
                var result = await FileSaver.Default.SaveAsync(fileName, stream, CancellationToken.None);

                if (result.IsSuccessful)
                {
                    await Shell.Current.DisplayAlertAsync("Success",
                        $"Report saved successfully!", "OK");
                }
                else
                {
                    await Shell.Current.DisplayAlertAsync("Error",
                        "Failed to save report", "OK");
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlertAsync("Error", ex.Message, "OK");
            }
        }

        [RelayCommand]
        public async Task OpenReport()
        {
            try
            {
                // Open file picker to let user select a report file
                var customFileType = new FilePickerFileType(
                    new Dictionary<DevicePlatform, IEnumerable<string>>
                    {
                        { DevicePlatform.iOS, new[] { "public.text" } },
                        { DevicePlatform.Android, new[] { "text/plain" } },
                        { DevicePlatform.WinUI, new[] { ".txt" } },
                        { DevicePlatform.MacCatalyst, new[] { "txt" } }
                    });

                var result = await FilePicker.PickAsync(new PickOptions
                {
                    FileTypes = customFileType,
                    PickerTitle = "Select Assessment Report"
                });

                if (result != null)
                {
                    await Launcher.OpenAsync(new OpenFileRequest
                    {
                        File = new ReadOnlyFile(result.FullPath)
                    });
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlertAsync("Error", ex.Message, "OK");
            }
        }

        [RelayCommand]
        public async Task ResetReportsFolder()
        {
            Preferences.Remove(REPORTS_FOLDER_KEY);
            await Shell.Current.DisplayAlertAsync("Reset Complete",
                "Reports folder has been reset. You'll be prompted to choose a new folder next time.", "OK");
        }

        [RelayCommand]
        private async Task GoBack()
        {
            try
            {
                // Use relative navigation (no leading slash)
                await Shell.Current.GoToAsync("AssessmentSelectionView", true, new Dictionary<string, object>
                {
                    ["course"] = NewCourse,
                    ["term"] = NewTerm,
                    ["user"] = NewUser
                });
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlertAsync("Navigation Error", $"Navigation back failed: {ex.Message}", "OK");
            }
        }
    }
}
