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
    [QueryProperty(nameof(Instructor), "instructor")]
    public partial class CourseDetailsViewModel : ObservableObject
    {
        private readonly DatabaseService _database;

        // Core Properties
        [ObservableProperty]
        private Course course;

        [ObservableProperty]
        private CourseInstructor instructor;

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
        partial void OnCourseChanged(Course value)
        {
            if (value != null)
            {
                Name = $"{value.Name} - Details";
                IsEditing = false;
                System.Diagnostics.Debug.WriteLine($"Course loaded: {value.Id} - {value.Name}");
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
            if (Course == null) return;

            try
            {
                //Display each field's value in alert
                await Shell.Current.DisplayAlertAsync("Course Details",
                    $"ID: {Course.Id}\n" +
                    $"Name: {Course.Name}\n" +
                    $"Start Date: {Course.StartDate}\n" +
                    $"End Date: {Course.EndDate}\n" +
                    $"Status: {Course.Status}\n" +
                    $"Instructor: {Course.InstructorId}\n" +
                    $"Start Date Notifications: {Course.StartDateNotifications}\n" +
                    $"End Date Notifications: {Course.EndDateNotifications}",
                    "OK");

                //Refresh the page
                OnPropertyChanged(nameof(Course));

                // Update notifications after saving
                await ScheduleCourseNotifications();

                // Save to the database
                await _database.SaveCourseAsync(Course);
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
            if (Course == null) return;

            try
            {
                await Shell.Current.GoToAsync($"{nameof(CourseInstructorView)}", true, new Dictionary<string, object>
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
        private async Task ViewAssessments()
        {
            if (Course == null) return;

            try
            {
                // Save any changes before navigating
                if (IsEditing)
                {
                    await SaveCourse();
                    IsEditing = false;
                }

                await Shell.Current.GoToAsync("AssessmentsView", new Dictionary<string, object>
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
        private async Task AddNote()
        {
            if (Course == null) return;

            try
            {
                await Shell.Current.GoToAsync($"{nameof(AddNoteView)}", new Dictionary<string, object>
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
        private async Task ViewNotes()
        {
            if (Course == null) return;

            try
            {
                await Shell.Current.GoToAsync($"{nameof(ViewNotesView)}", new Dictionary<string, object>
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
                    ["course"] = Course
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
            if (Course == null) return;

            try
            {
                // Cancel existing notifications
                CancelNotification($"course_start_{Course.Id}");
                CancelNotification($"course_end_{Course.Id}");

                // Schedule start date notification
                if (Course.StartDateNotifications && Course.StartDate > DateTime.Now)
                {
                    await ScheduleNotification(
                        $"course_start_{Course.Id}",
                        $"Course Starting: {Course.Name}",
                        $"Your course '{Course.Name}' starts today!",
                        Course.StartDate);
                }

                // Schedule end date notification
                if (Course.EndDateNotifications && Course.EndDate > DateTime.Now)
                {
                    await ScheduleNotification(
                        $"course_end_{Course.Id}",
                        $"Course Ending: {Course.Name}",
                        $"Your course '{Course.Name}' ends today!",
                        Course.EndDate);
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
    }
}