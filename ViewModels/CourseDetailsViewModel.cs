using C_971.Models;
using C_971.Services;
using C_971.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Plugin.LocalNotification;
using System.Collections.ObjectModel;

namespace C_971.ViewModels
{
    [QueryProperty(nameof(NewCourse), "course")]
    [QueryProperty(nameof(NewInstructor), "instructor")]
    public partial class CourseDetailsViewModel : ObservableObject
    {
        private readonly DatabaseService _database;

        // Core Properties
        [ObservableProperty]
        private Course newCourse;

        [ObservableProperty]
        private CourseInstructor newInstructor;

        [ObservableProperty]
        private string name = "Course Details";

        // UI State
        [ObservableProperty]
        private bool isEditing;

        public bool IsNotEditing => !IsEditing;

        // Dynamic UI Properties
        public string Title => IsEditing ? "Edit Course Details" : "Course Details";
        public string EditButtonText => IsEditing ? "Save Changes" : "Edit Course Details";
        public string EditButtonColor => IsEditing ? "#4CAF50" : "#2196F3";

        // Static Collections
        public ObservableCollection<CourseStatus> StatusOptions { get; } = new ObservableCollection<CourseStatus>
        {
            CourseStatus.NotEnrolled,
            CourseStatus.InProgress,
            CourseStatus.Completed,
            CourseStatus.Dropped,
            CourseStatus.Planned
        };

        public ObservableCollection<AssessmentType> AssessmentTypeOptions { get; } = new ObservableCollection<AssessmentType>
        {
            AssessmentType.Objective,
            AssessmentType.Performance
        };

        public CourseDetailsViewModel(DatabaseService databaseService)
        {
            _database = databaseService;
            _ = RequestNotificationPermissions();
        }

        // Property Change Handlers
        partial void OnNewCourseChanged(Course value)
        {
            if (value != null)
            {
                Name = $"{value.Name} - Details";
                IsEditing = false;
                _= LoadInstructorAsync();
            }
        }

        partial void OnIsEditingChanged(bool value)
        {
            OnPropertyChanged(nameof(IsNotEditing));
            OnPropertyChanged(nameof(Title));
            OnPropertyChanged(nameof(EditButtonText));
            OnPropertyChanged(nameof(EditButtonColor));
        }

        // Course Management
        [RelayCommand]
        private async Task ToggleEdit()
        {
            if (IsEditing)
            {
                // Save changes when exiting edit mode
                await SaveCourse();
            }

            IsEditing = !IsEditing;
        }

        [RelayCommand]
        private async Task SaveCourse()
        {
            if (NewCourse == null) return;

            try
            {
                //Display each field's value in alert
                await Shell.Current.DisplayAlertAsync("Course Details",
                    $"ID: {NewCourse.Id}\n" +
                    $"Name: {NewCourse.Name}\n" +
                    $"Start Date: {NewCourse.StartDate}\n" +
                    $"End Date: {NewCourse.EndDate}\n" +
                    $"Status: {NewCourse.Status}\n" +
                    $"Instructor: {NewCourse.InstructorId}\n" +
                    $"Start Date Notifications: {NewCourse.StartDateNotifications}\n" +
                    $"End Date Notifications: {NewCourse.EndDateNotifications}",
                    "OK");

                //Refresh the page
                OnPropertyChanged(nameof(NewCourse));

                // Update notifications after saving
                await ScheduleCourseNotifications();

                // Save to the database
                await _database.SaveCourseAsync(NewCourse);
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlertAsync("Error", $"Failed to save course: {ex.Message}", "OK");
                System.Diagnostics.Debug.WriteLine($"Save error: {ex}");
            }
        }

        // Navigation Commands
        [RelayCommand]
        private async Task GetInstructor()
        {
            if (NewCourse == null) return;

            try
            {
                await Shell.Current.GoToAsync($"{nameof(CourseInstructorView)}", true, new Dictionary<string, object>
                {
                    ["course"] = NewCourse
                });
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlertAsync("Navigation Error", $"Navigation failed: {ex.Message}", "OK");
            }
        }

        [RelayCommand]
        private async Task ViewAssessments()
        {
            if (NewCourse == null) return;

            try
            {
                // Save any changes before navigating
                if (IsEditing)
                {
                    await SaveCourse();
                    IsEditing = false;
                }

                await Shell.Current.GoToAsync($"{nameof(AssessmentSelectionView)}", new Dictionary<string, object>
                {
                    ["course"] = NewCourse
                });
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlertAsync("Navigation Error", $"Navigation failed: {ex.Message}", "OK");
            }
        }

        [RelayCommand]
        private async Task AddNote()
        {
            if (NewCourse == null) return;

            try
            {
                await Shell.Current.GoToAsync($"{nameof(AddNoteView)}", new Dictionary<string, object>
                {
                    ["course"] = NewCourse
                });
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlertAsync("Navigation Error", $"Navigation failed: {ex.Message}", "OK");
            }
        }

        [RelayCommand]
        private async Task ViewNotes()
        {
            if (NewCourse == null) return;

            try
            {
                await Shell.Current.GoToAsync($"{nameof(ViewNotesView)}", new Dictionary<string, object>
                {
                    ["course"] = NewCourse
                });
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlertAsync("Navigation Error", $"Navigation failed: {ex.Message}", "OK");
            }
        }

        [RelayCommand]
        private async Task GoBack()
        {
            try
            {
                // Save changes before going back if in edit mode
                if (IsEditing)
                {
                    await SaveCourse();
                }

                await Shell.Current.GoToAsync("CourseListView", true, new Dictionary<string, object>
                {
                    ["course"] = NewCourse
                });
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlertAsync("Navigation Error", $"Navigation failed: {ex.Message}", "OK");
            }
        }

        // Notification Management
        private async Task ScheduleCourseNotifications()
        {
            if (NewCourse == null) return;

            try
            {
                // Cancel existing notifications
                CancelNotification($"course_start_{NewCourse.Id}");
                CancelNotification($"course_end_{NewCourse.Id}");

                // Schedule start date notification
                if (NewCourse.StartDateNotifications && NewCourse.StartDate > DateTime.Now)
                {
                    await ScheduleNotification(
                        $"course_start_{NewCourse.Id}",
                        $"Course Starting: {NewCourse.Name}",
                        $"Your course '{NewCourse.Name}' starts today!",
                        NewCourse.StartDate);
                }

                // Schedule end date notification
                if (NewCourse.EndDateNotifications && NewCourse.EndDate > DateTime.Now)
                {
                    await ScheduleNotification(
                        $"course_end_{NewCourse.Id}",
                        $"Course Ending: {NewCourse.Name}",
                        $"Your course '{NewCourse.Name}' ends today!",
                        NewCourse.EndDate);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to schedule course notifications: {ex.Message}");
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
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to cancel notification: {ex.Message}");
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

        private async Task LoadInstructorAsync()
        {
            if (NewCourse == null) return;
            try
            {
                var instructor = await _database.GetInstructorByIdAsync(NewCourse.InstructorId);
                NewInstructor = instructor;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to load instructor: {ex.Message}");
            }
        }

        [RelayCommand]
        private async Task RefreshData()
        {
            if (NewCourse?.Id > 0)
            {
                try
                {
                    // Reload course from database to get latest InstructorId
                    NewCourse = await _database.GetCourseByIdAsync(NewCourse.Id);

                    // Load the instructor data
                    await LoadInstructorAsync();

                    System.Diagnostics.Debug.WriteLine($"Refreshed course and instructor data");
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Failed to refresh data: {ex.Message}");
                }
            }
        }

        public async Task OnAppearingAsync()
        {
            // Refresh data when returning to this page
            await RefreshData();
        }
    }
}