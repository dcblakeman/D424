using C_971.Models;
using C_971.Services;
using C_971.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Plugin.LocalNotification;
using System.Collections.ObjectModel;

namespace C_971.ViewModels
{
    [QueryProperty(nameof(NewTerm), "term")]
    [QueryProperty(nameof(NewCourse), "course")]
    [QueryProperty(nameof(NewUserId), "userid")]
    public partial class AssessmentSelectionViewModel : ObservableObject
    {
        private readonly DatabaseService _database;
        public AssessmentSelectionViewModel(DatabaseService database) 
        {
            _database = database;
        }

        [ObservableProperty]
        private int newUserId;

        [ObservableProperty]
        private Course newCourse;

        [ObservableProperty]
        private AcademicTerm newTerm;

        [ObservableProperty]
        private string viewName = "Assessment Selection / Reports";

        [RelayCommand]
        private async Task GoBack()
        {
            try
            {
                await Shell.Current.GoToAsync("CourseDetailsView", true, new Dictionary<string, object>
                {
                    ["course"] = NewCourse,
                    ["userid"] = NewUserId,
                    ["term"] = NewTerm
                });
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlertAsync("Navigation Error", $"Navigation failed: {ex.Message}", "OK");
            }
        }

        [RelayCommand]
        private async Task NavigateToPerformanceAssessmentView()
        {
            if (NewCourse == null) return;

            try
            {
                await Shell.Current.GoToAsync($"{nameof(PerformanceAssessmentView)}", new Dictionary<string, object>
                {
                    ["course"] = NewCourse,
                    ["userId"] = NewUserId,
                    ["term"] = NewTerm
                });
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlertAsync("Navigation Error", $"Navigation failed: {ex.Message}", "OK");
            }
        }

        [RelayCommand]
        private async Task NavigateToObjectiveAssessmentView()
        {
            if (NewCourse == null) return;

            try
            {
                await Shell.Current.GoToAsync($"{nameof(ObjectiveAssessmentView)}", new Dictionary<string, object>
                {
                    ["term"] = NewTerm,
                    ["course"] = NewCourse,
                    ["userid"] = NewUserId
                });
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlertAsync("Navigation Error", $"Navigation failed: {ex.Message}", "OK");
            }
        }

        [RelayCommand]
        private async Task NavigateToReportView()
        {
            if (NewCourse == null) return;
            try
            {
                await Shell.Current.GoToAsync($"///{nameof(ReportView)}", true, new Dictionary<string, object>
                {
                    ["term"] = NewTerm,
                    ["course"] = NewCourse,
                    ["newuserid"] = NewUserId
                });
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlertAsync("Navigation Error", $"Navigation failed: {ex.Message}", "OK");
            }
        }
    }
}
