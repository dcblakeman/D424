using C_971.Models;
using C_971.Services;
using CommunityToolkit.Maui.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Storage;
using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

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
        private string reportTitle;

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
            Shell.Current.DisplayAlertAsync("Updated Values", $"New Course: {NewCourse}", "OK");
        }

        partial void OnTermChanged(AcademicTerm value)
        {
            NewTerm = value;
            Shell.Current.DisplayAlertAsync("Updated Values", $"New Term: {NewTerm}", "OK");
        }


        // Generate a report to output all of the assessments with grades
        [RelayCommand]
        public async Task GenerateAssessmentReport()
        {
            ReportTitle= "Assessment_Report";
            ReportText = string.Empty;

            try
            {
                var assessments = await _database.GetAssessmentsForUserAndTermAsync(NewUser.Id, NewTerm.Id);

                await Shell.Current.DisplayAlertAsync("Wait", $"Number of assessments: {assessments.Count}", "OK");

                if (!assessments.Any())
                {
                    await Shell.Current.DisplayAlertAsync("No Assessments",
                        "No assessments found for the selected term.", "OK");
                }

                // Generate Report
                foreach (CourseAssessment assessment in assessments)
                {
                    ReportText += $"Assessment: {assessment.Name}\n" +
                                    $"Type: {assessment.Type}\n" +
                                    $"Start Date: {assessment.StartDate:d}\n" +
                                    $"End Date: {assessment.EndDate:d}\n" +
                                    $"Status: {assessment.Status}\n\n" + 
                                    $"Grade: {assessment.Grade}\n\n";
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlertAsync("Error", ex.Message, "OK");
            }
            await ReportViewModel.SaveReport(ReportText, ReportTitle);
        }

        // Generate a report to output all of the courses with grades
        [RelayCommand]
        public async Task GenerateCourseReport()
        {
            ReportTitle = "Course_Report";
            ReportText = string.Empty;
            try
            {
                var courses = await _database.GetCoursesWithDetailsAsync(NewUser.Id, NewTerm.Id);

                // Generate Report
                foreach (Course course in courses)
                {
                    ReportText += $"Course: {course.Name}\n" +
                                    $"Start Date: {course.StartDate:d}\n" +
                                    $"End Date: {course.EndDate:d}\n" +
                                    $"Status: {course.Status}\n\n" +
                                    $"Grade: {course.Grade}\n\n";
                }

                await Shell.Current.DisplayAlertAsync("Course Report", ReportText, "OK");
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlertAsync("Error", $"Failed to generate report: {ex.Message}", "OK");
            }

            await ReportViewModel.SaveReport(ReportText, ReportTitle);
        }

        [RelayCommand]
        public async Task GenerateCoursesWithAssessmentsReport()
        {
            ReportTitle = "Courses_And_Assessments_Report";
            ReportText = string.Empty; // Clear previous report

            try
            {
                var coursesWithAssessments = await _database.GetCoursesWithAssessmentsAsync(User.Id, Term.Id);

                // Generate Report Header
                ReportText += $"=== COURSES AND ASSESSMENTS REPORT ===\n";
                ReportText += $"Term: {Term.Name}\n";
                ReportText += $"Generated: {DateTime.Now:yyyy-MM-dd HH:mm}\n";
                ReportText += $"Total Courses: {coursesWithAssessments.Count}\n\n";

                // Generate Report Body
                foreach (var courseWithAssessments in coursesWithAssessments)
                {
                    var course = courseWithAssessments.Course;
                    var assessments = courseWithAssessments.Assessments;

                    ReportText += $"COURSE: {course.Name}\n" +
                                 $"Description: {course.Description}\n" +
                                 $"Start Date: {course.StartDate:d}\n" +
                                 $"End Date: {course.EndDate:d}\n" +
                                 $"Status: {course.Status}\n" +
                                 $"Grade: {course.Grade}\n\n";

                    if (assessments.Count == 0)
                    {
                        ReportText += "No assessments found for this course.\n\n";
                    }
                    else
                    {
                        ReportText += $"ASSESSMENTS ({assessments.Count}):\n";
                        foreach (var assessment in assessments)
                        {
                            ReportText += $"  • {assessment.Name}\n" +
                                         $"    Type: {assessment.Type}\n" +
                                         $"    Start: {assessment.StartDate:d}\n" +
                                         $"    Due: {assessment.EndDate:d}\n" +
                                         $"    Score: {assessment.Grade}\n\n";
                        }
                    }

                    ReportText += "".PadRight(50, '-') + "\n\n";
                }

                await Shell.Current.DisplayAlertAsync("Courses & Assessments Report", ReportText, "OK");
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlertAsync("Error", $"Failed to generate report: {ex.Message}", "OK");
            }

            await ReportViewModel.SaveReport(ReportText, ReportTitle);
        }

        public static async Task SaveReport(string reportContent, string reportTitle)
        {
            try
            {

                // DEBUG: Check if content is actually there
                if (string.IsNullOrEmpty(reportContent))
                {
                    await Shell.Current.DisplayAlertAsync("Debug", "Report content is empty!", "OK");
                    return;
                }

                var stream = new MemoryStream(Encoding.UTF8.GetBytes(reportContent));

                var fileName = $"{reportTitle}_{DateTime.Now:yyyyMMdd_HHmmss}.txt";
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

        internal async Task OnAppearingAsync()
        {
            NewUser = User;
            NewTerm = Term;
            NewCourse = Course;
        }
    }
}
