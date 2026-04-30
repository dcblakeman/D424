
using C_971.Models;
using C_971.Services;
using C_971.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace C_971.ViewModels
{
    [QueryProperty(nameof(Term), "term")]
    [QueryProperty(nameof(Course), "course")]
    [QueryProperty(nameof(User), "user")]
    public class AssessmentSelectionViewModel : ObservableObject
    {
        private readonly DatabaseService _database;
        public AssessmentSelectionViewModel(DatabaseService database)
        {
            _database = database;
        }

        [ObservableProperty]
        private User user = null!;

        [ObservableProperty]
        private User newUser = null!;

        [ObservableProperty]
        private Course course = null!;

        [ObservableProperty]
        private Course newCourse = null!;

        [ObservableProperty]
        private AcademicTerm term = null!;

        [ObservableProperty]
        private AcademicTerm newTerm = null!;

        [ObservableProperty]
        private string viewName = "Assessments | Reports";

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
        }

        private async Task GoBack()
        {x`
            try
            {
                await Shell.Current.GoToAsync("CourseDetailsView", true, new Dictionary<string, object>
                {
                    ["course"] = NewCourse,
                    ["term"] = NewTerm,
                    ["user"] = NewUser
                });
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlertAsync("Navigation Error", $"Navigation failed: {ex.Message}", "OK");
            }
        }

        private async Task NavigateToPerformanceAssessmentView()
        {
            try
            {
                await Shell.Current.GoToAsync("PerformanceAssessmentView", true, new Dictionary<string, object>
                {
                    ["term"] = NewTerm,
                    ["course"] = NewCourse,
                    ["user"] = NewUser
                });
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlertAsync("Navigation Error", $"Navigation failed: {ex.Message}", "OK");
            }
        }

        private async Task NavigateToObjectiveAssessmentView()
        {
            try
            {
                await Shell.Current.GoToAsync("ObjectiveAssessmentView", true, new Dictionary<string, object>
                {
                    ["term"] = NewTerm,
                    ["course"] = NewCourse,
                    ["user"] = NewUser
                });
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlertAsync("Navigation Error", $"Navigation failed: {ex.Message}", "OK");
            }
        }

        private async Task NavigateToReportView()
        {
            if (NewCourse == null)
            {
                return;
            }

            try
            {
                await Shell.Current.GoToAsync("ReportView", true, new Dictionary<string, object>
                {
                    ["term"] = NewTerm,
                    ["course"] = NewCourse,
                    ["user"] = NewUser
                });
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlertAsync("Navigation Error", $"Navigation failed: {ex.Message}", "OK");
            }
        }
    }
}
