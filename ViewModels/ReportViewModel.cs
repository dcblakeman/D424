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
            Shell.Current.DisplayAlertAsync("Updated Values", $"New User: {NewUser}", "OK");
        }

        partial void OnCourseChanged(Course value)
        {
            NewCourse = value;
        }

        partial void OnTermChanged(AcademicTerm value)
        {
            NewTerm = value;
        }

        [RelayCommand]
        public async Task GenerateUserAssessmentReport()
        {
            try
            {
                if (Term.Id == 0)
                {
                    await Shell.Current.DisplayAlertAsync("No Term Selected",
                        "Please select a term first.", "OK");
                    return;
                }

                var assessments = await _database.GetAssessmentsForUserAndTermAsync(NewUser.Id, NewTerm.Id);

                if (!assessments.Any())
                {
                    await Shell.Current.DisplayAlertAsync("No Assessments",
                        "No assessments found for the selected term.", "OK");
                    return;
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

                //Output ReportText to .text file
                string folderPath = await GetOrSelectReportsFolder();
                if (!string.IsNullOrEmpty(folderPath))
                {
                    string fileName = $"My_Assessments_Report_{DateTime.Now:yyyyMMdd_HHmmss}.txt";
                    string filePath = Path.Combine(folderPath, fileName);
                    await File.WriteAllTextAsync(filePath, ReportText);
                    await Shell.Current.DisplayAlertAsync("Report Saved",
                        $"Report saved to:\n{filePath}", "OK");
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlertAsync("Error", ex.Message, "OK");
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

        [RelayCommand]
        public async Task SaveAssessmentsReport()
        {
            try
            {
                var assessments = await _database.GetUserAssessmentsAsync(NewUser.Id);

                if (assessments == null || assessments.Count == 0)
                {
                    await Shell.Current.DisplayAlertAsync("No Data",
                        "No assessments found for this user.", "OK");
                    return;
                }

                string content = GenerateAssessmentsReportContent(assessments);
                await SaveReportToFile(content, "Assessments_Report");
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlertAsync("Error",
                    $"Could not generate assessments report: {ex.Message}", "OK");
            }
        }

        private async Task SaveReportToFile(string content, string v)
        {
            string folderPath = await GetOrSelectReportsFolder();
            if (!string.IsNullOrEmpty(folderPath))
            {
                string fileName = $"{v}_{DateTime.Now:yyyyMMdd_HHmmss}.txt";
                string filePath = Path.Combine(folderPath, fileName);
                await File.WriteAllTextAsync(filePath, content);
                await Shell.Current.DisplayAlertAsync("Report Saved",
                    $"Report saved to:\n{filePath}", "OK");
            }
        }

        [RelayCommand]
        public async Task GenerateCoursesReportToFile()
        {
            try
            {
                IsBusy = true;

                var userCourses = await _database.GetUserCoursesWithDetailsAsync(1);

                if (userCourses.Count == 0)
                {
                    await Shell.Current.DisplayAlertAsync("No Courses", "No course enrollments found.", "OK");
                    return;
                }

                // Get or select persistent folder
                string folderPath = await GetOrSelectReportsFolder();

                if (!string.IsNullOrEmpty(folderPath))
                {
                    string report = GenerateCoursesReport(userCourses);
                    string fileName = $"My_Courses_Report_{DateTime.Now:yyyyMMdd_HHmmss}.txt";
                    string filePath = Path.Combine(folderPath, fileName);

                    await File.WriteAllTextAsync(filePath, report);

                    await Shell.Current.DisplayAlertAsync("Report Saved",
                        $"Saved to: {filePath}\nTotal Courses: {userCourses.Count}", "OK");
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlertAsync("Error", ex.Message, "OK");
            }
            finally
            {
                IsBusy = false;
            }
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
        public async Task ChangeReportsFolder()
        {
            try
            {
                var folderResult = await FolderPicker.PickAsync(CancellationToken.None);

                if (folderResult != null && folderResult.IsSuccessful)
                {
                    // Save the new folder path
                    Preferences.Set(REPORTS_FOLDER_KEY, folderResult.Folder.Path);

                    await Shell.Current.DisplayAlertAsync("Folder Updated",
                        $"Reports will now be saved to:\n{folderResult.Folder.Path}", "OK");
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlertAsync("Error", ex.Message, "OK");
            }
        }

        private async Task<string> GetOrSelectReportsFolder()
        {
            string savedPath = Preferences.Get(REPORTS_FOLDER_KEY, string.Empty);

            if (!string.IsNullOrEmpty(savedPath) && Directory.Exists(savedPath))
            {
                return savedPath;
            }

            // Need to select a folder
            var folderResult = await FolderPicker.PickAsync(CancellationToken.None);
            if (folderResult?.IsSuccessful == true)
            {
                Preferences.Set(REPORTS_FOLDER_KEY, folderResult.Folder.Path);
                return folderResult.Folder.Path;
            }

            return string.Empty;
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
