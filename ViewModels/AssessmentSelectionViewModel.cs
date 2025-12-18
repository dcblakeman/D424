using C_971.Models;
using C_971.Services;
using C_971.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Plugin.LocalNotification;
using System.Collections.ObjectModel;

namespace C_971.ViewModels
{
    [QueryProperty(nameof(Course), "course")]
    public partial class AssessmentSelectionViewModel : ObservableObject
    {
        private readonly DatabaseService _database;
        public AssessmentSelectionViewModel(DatabaseService database) 
        {
            _database = database;
        }

        [ObservableProperty]
        private Course course;

        [ObservableProperty]
        private string name = "Assessment Selection";

        [RelayCommand]
        private async Task GoBack()
        {
            try
            {
                await Shell.Current.GoToAsync("CourseDetailsView", true, new Dictionary<string, object>
                {
                    ["course"] = Course
                });
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlertAsync("Navigation Error", $"Navigation failed: {ex.Message}", "OK");
            }
        }

        [RelayCommand]
        private async Task GoToPerformanceAssessmentView()
        {

        }
    }
}
