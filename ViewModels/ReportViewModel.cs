using C_971.Models;
using C_971.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace C_971.ViewModels
{
    [QueryProperty(nameof(NewUserId), "newuserid")]
    [QueryProperty(nameof(NewCourse), "newcourse")]
    public partial class ReportViewModel : ObservableObject
    {

        private DatabaseService _database;

        [ObservableProperty]
        private int newUserId;

        [ObservableProperty]
        private Course newCourse;

        [ObservableProperty]
        private string reportText;

        private string filePath = @"F:\source\d424-software-engineering-capstone\Reports";

        // Alternative file path for testing
        string docPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

        public ReportViewModel(DatabaseService database)
        {
            _database = database;
        }

        partial void OnNewUserIdChanged(int value)
        {
            NewUserId = value;
        }

        partial void OnNewCourseChanged(Course value)
        {
            NewCourse = value;
        }
         
        // Commands
        [RelayCommand]
        private async Task GoBack()
        {
            try
            {
                // Use relative navigation (no leading slash)
                await Shell.Current.GoToAsync("AssessmentSelectionView", true, new Dictionary<string, object>
                {
                    ["course"] = NewCourse,
                    ["newuserid"] = NewUserId
                });
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlertAsync("Navigation Error", $"Navigation back failed: {ex.Message}", "OK");
            }
        }

        [RelayCommand]
        private async Task GenerateAllCoursesReport()
        {
            // Get all of the courses in a list
            //for each course, generate a report
            //var courses = _database.GetCoursesByUserId(NewUserId);

            if (NewCourse == null) return;
            StringBuilder reportBuilder = new StringBuilder();
            
            //Add date and time first
            reportBuilder.AppendLine($"Report Generated on: {DateTime.Now}");
            reportBuilder.AppendLine($"Report for Course: {NewCourse.Name}");
            reportBuilder.AppendLine($"Course ID: {NewCourse.Id}");
            reportBuilder.AppendLine($"User ID: {NewUserId}");
            reportBuilder.AppendLine("--------------------------------------------------");
            reportBuilder.AppendLine("This is a placeholder for the detailed report content.");
            reportBuilder.AppendLine("You can expand this method to include actual data analysis and reporting logic.");
            reportBuilder.AppendLine("--------------------------------------------------");
            reportBuilder.AppendLine($"Report generated on: {DateTime.Now}");
            ReportText = reportBuilder.ToString();

            //Output report to a text file
            File.AppendAllBytes(Path.Combine(filePath, $"CourseReport_{NewCourse.Id}_{DateTime.Now:yyyyMMdd_HHmmss}.txt"), Encoding.UTF8.GetBytes(ReportText));
        }
    }
}
