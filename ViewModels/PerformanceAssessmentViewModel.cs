using C_971.Models;
using C_971.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Plugin.LocalNotification;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace C_971.ViewModels
{
    [QueryProperty(nameof(Course), "course")]
    public partial class PerformanceAssessmentViewModel : ObservableObject
    {
        private readonly DatabaseService _database;

        // Core Properties
        [ObservableProperty]
        private Course course;

        [ObservableProperty]
        private string name = "Performance Assessment";


        [ObservableProperty]
        private string searchText = string.Empty;

        [ObservableProperty]
        private CourseAssessment assessment;

        [ObservableProperty]
        private ObservableCollection<CourseAssessment> assessments = new();

        private List<CourseAssessment> _allAssessments = new();

        // UI State
        [ObservableProperty]
        private bool isEditing;

        [ObservableProperty]
        private bool isRefreshing;

        [ObservableProperty]
        private bool isAddingAssessment;

        [ObservableProperty]
        private bool isRemovingAssessment;

        [ObservableProperty]
        private bool isSavingAssessment;

        [ObservableProperty]
        private bool isDeletingAssessment;

        [ObservableProperty]
        private bool isLoadingAssessments;

        public bool IsNotRefreshing => !IsRefreshing;

        public bool IsNotAddingAssessment => !IsAddingAssessment;

        public bool IsNotRemovingAssessment => !IsRemovingAssessment;

        public bool IsNotSavingAssessment => !IsSavingAssessment;

        public bool IsNotDeletingAssessment => !IsDeletingAssessment;


        public bool IsNotEditing => !IsEditing && !IsSavingAssessment && !IsDeletingAssessment && !IsAddingAssessment && !IsRemovingAssessment && !IsLoadingAssessments && !IsRefreshing;

        public string EditButtonText => IsEditing ? "Add Assessment" : "Edit Assessment";
 

        public PerformanceAssessmentViewModel(DatabaseService database)
        {
            _database = database;
            _ = RequestNotificationPermissions();
        }

        [RelayCommand]
        private async Task GoBack()
        {
            try
            {
                // Use relative navigation (no leading slash)
                await Shell.Current.GoToAsync("AssessmentSelectionView", true, new Dictionary<string, object>
                {
                    ["course"] = Course
                });
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlertAsync("Navigation Error", $"Navigation back failed: {ex.Message}", "OK");
            }
        }

        private async Task RequestNotificationPermissions()
        {
            try
            {
                await LocalNotificationCenter.Current.RequestNotificationPermission();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to request notification permissions: {ex.Message}");
            }
        }

        [RelayCommand]
        private async Task Edit()
        {
            if (IsEditing)
            {
                // Save logic here
                await SaveAssessment(Assessment);
            }

            IsEditing = !IsEditing;
            OnPropertyChanged(nameof(EditButtonText));
        }

        partial void OnIsEditingChanged(bool value)
        {
            OnPropertyChanged(nameof(IsNotEditing));
        }

        [RelayCommand]
        private async Task SaveAssessment(CourseAssessment assessment)
        {
            if (assessment == null) return;

            try
            {
                await _database.SaveCourseAssessmentAsync(assessment);

                // Update notifications if needed
                await UpdateAssessmentNotifications(assessment);

                await Shell.Current.DisplayAlertAsync("Success", "Assessment saved successfully!", "OK");
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlertAsync("Error", $"Failed to save assessment: {ex.Message}", "OK");
                System.Diagnostics.Debug.WriteLine($"Save assessment error: {ex}");
            }
        }

        private async Task UpdateAssessmentNotifications(CourseAssessment assessment)
        {
            if (assessment?.EndDate == null) return;

            try
            {
                // Cancel existing notification
                CancelNotification($"assessment_{assessment.Id}");

                // Schedule new notification if assessment is not completed and due date is in future
                if (assessment.Status != AssessmentStatus.Completed && assessment.EndDate > DateTime.Now)
                {
                    var notifyTime = assessment.EndDate.AddDays(-1); // Notify 1 day before

                    if (notifyTime > DateTime.Now)
                    {
                        await ScheduleNotification(
                            $"assessment_{assessment.Id}",
                            "Assessment Due Tomorrow",
                            $"{assessment.Name} for {Course?.Name ?? "course"} is due tomorrow",
                            notifyTime);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to update notifications: {ex.Message}");
            }
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
                    Schedule = new NotificationRequestSchedule
                    {
                        NotifyTime = notifyTime
                    }
                };

                await LocalNotificationCenter.Current.Show(notification);
                System.Diagnostics.Debug.WriteLine($"Scheduled notification: {title} at {notifyTime}");
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
                System.Diagnostics.Debug.WriteLine($"Cancelled notification: {id}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to cancel notification: {ex.Message}");
            }
        }

    }
}
