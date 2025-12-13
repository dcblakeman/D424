using C_971.Models;
using C_971.Services;
using C_971.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Plugin.LocalNotification;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace C_971.ViewModels
{
    [QueryProperty(nameof(Course), "course")]
    public partial class AssessmentsViewModel : ObservableObject
    {
        DatabaseService _database;

        [ObservableProperty]
        Course _course;

        [ObservableProperty]
        bool isEditing;

        [ObservableProperty]
        private bool isLoading = false;

        [ObservableProperty]
        private bool isNotEditing;

        [ObservableProperty]
        private bool isRefreshing;


        public ObservableCollection<CourseAssessment> Assessment { get; set; } = new ObservableCollection<CourseAssessment>();


        public AssessmentsViewModel(DatabaseService database) 
        { 
            _database = database;
        }

        partial void OnIsEditingChanged(bool value)
        {
            OnPropertyChanged(nameof(IsNotEditing));
        }

        private async Task ScheduleNotification(string id, string title, string message, DateTime notifyTime)
        {
            try
            {
                var notification = new NotificationRequest
                {
                    NotificationId = id.GetHashCode(),
                    Title = title,
                    Description = message,
                    Schedule =
                    {
                        NotifyTime = notifyTime
                    }
                };
                await LocalNotificationCenter.Current.Show(notification);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to schedule notification: {ex.Message}");
            }
        }

        private void CancelNotification(string id)
        {
            try
            {
                LocalNotificationCenter.Current.Cancel(id.GetHashCode());
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to cancel notification: {ex.Message}");
            }
        }

        private async Task RequestNotificationPermissions()
        {
            await LocalNotificationCenter.Current.RequestNotificationPermission();
        }

        [RelayCommand]
        public async Task GoBack()
        {
            try
            {
                // Go back to coursedetailsview with the course context
                await Shell.Current.GoToAsync($"{nameof(CourseDetailsView)}", true, new Dictionary<string, object>
                {
                    ["course"] = Course       // Pass the actual Course object
                });
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlertAsync("Navigation Error", $"Navigation back failed: {ex.Message}", "OK");
            }
        }

        //Add Ids for assessments
        public void AddAssessmentIds(int assessment1Id, int assessment2Id)
        {
            if (Assessment.Count >= 2)
            {
                Assessment[0].Id = assessment1Id;
                Assessment[1].Id = assessment2Id;
            }

        }

        //Generate Ids for assessments based off the max existing assessment Id in the database
        public async Task GenerateAssessmentIds()
        {
            if (_database != null)
            {
                int maxId = await _database.GetMaxAssessmentIdAsync();
                if (Assessment.Count >= 2)
                {
                    Assessment[0].Id = maxId + 1;
                    Assessment[1].Id = maxId + 2;
                }
            }

        }
    }
}

