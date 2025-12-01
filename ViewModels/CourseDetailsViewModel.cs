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
    [QueryProperty(nameof(Course), ("course"))]
    public partial class CourseDetailsViewModel : ObservableObject
    {
        private DatabaseService _databaseService;

        public ObservableCollection<string> StatusOptions { get; private set; }
        public ObservableCollection<string> AssessmentTypeOptions { get; private set; }
        public string Title => IsEditing ? "Edit Course Details" : "Course Details"; // Dynamic title based on mode
        public Course Course { get; set; }

        [ObservableProperty]
        private CourseInstructor instructor = new CourseInstructor();

        public ObservableCollection<CourseAssessment> Assessment { get; set; } = new ObservableCollection<CourseAssessment>();

        [ObservableProperty]
        private bool isEditing;

        public bool IsNotEditing => !IsEditing;

        [ObservableProperty]
        private int termId;

        [ObservableProperty]
        private string id;

        public string EditButtonText => IsEditing ? "Save Changes" : "Edit Course Details";
        public string EditButtonColor => IsEditing ? "#4CAF50" : "#2196F3";

        [RelayCommand]
        async Task ToggleEdit()
        {
            if (IsEditing)
            {
                // Validate before saving
                if (!await ValidateCourse())
                {
                    return; // Don't toggle edit mode if validation fails
                }

                // Save changes
                await SaveCourseDetails();
                await Shell.Current.DisplayAlertAsync("Success", "Changes saved!", "OK");
            }

            // Only toggle if we're entering edit mode OR if validation passed
            IsEditing = !IsEditing;
            OnPropertyChanged(nameof(IsNotEditing));
            OnPropertyChanged(nameof(EditButtonText));
            OnPropertyChanged(nameof(EditButtonColor));
        }
        partial void OnIsEditingChanged(bool value)
        {
            OnPropertyChanged(nameof(IsNotEditing));
        }
        private async Task<bool> ValidateCourse()
        {
            // Validate TermId
            if (Course.TermId == 0)
            {
                await Shell.Current.DisplayAlertAsync("Validation Error", "Please select a valid Term ID. Term ID cannot be 0.", "OK");
                return false;
            }

            // Validate other required fields
            if (string.IsNullOrWhiteSpace(Course.Name))
            {
                await Shell.Current.DisplayAlertAsync("Validation Error", "Course name is required.", "OK");
                return false;
            }

            if (Course.EndDate <= Course.StartDate)
            {
                await Shell.Current.DisplayAlertAsync("Validation Error", "End date must be after start date.", "OK");
                return false;
            }

            // Validate Instructor Information
            if (Instructor.Id > 0)
            {
                await Shell.Current.DisplayAlertAsync("Validation Error", "Instructor Id is required.", "OK");
                return false;
            }

            if (string.IsNullOrWhiteSpace(Instructor.Name))
            {
                await Shell.Current.DisplayAlertAsync("Validation Error", "Instructor name is required.", "OK");
                return false;
            }

            if (string.IsNullOrWhiteSpace(Instructor.Email))
            {
                await Shell.Current.DisplayAlertAsync("Validation Error", "Instructor email is required.", "OK");
                return false;
            }

            // Validate email format
            if (!IsValidEmail(Instructor.Email))
            {
                await Shell.Current.DisplayAlertAsync("Validation Error", "Please enter a valid email address for the instructor.", "OK");
                return false;
            }

            if (string.IsNullOrWhiteSpace(Instructor.Phone))
            {
                await Shell.Current.DisplayAlertAsync("Validation Error", "Instructor phone number is required.", "OK");
                return false;
            }

            // Validate phone format (basic validation)
            if (!IsValidPhone(Instructor.Phone))
            {
                await Shell.Current.DisplayAlertAsync("Validation Error", "Please enter a valid phone number for the instructor.", "OK");
                return false;
            }

            return true; // All validation passed
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private bool IsValidPhone(string phone)
        {
            // Remove any formatting characters
            string cleanPhone = new(phone.Where(char.IsDigit).ToArray());

            // Check if it's a valid length (10 digits for US format)
            return cleanPhone.Length >= 10 && cleanPhone.Length <= 15;
        }
        private async Task SaveCourseDetails()
        {

            // Save changes to the course via the service
            if (_databaseService != null && Course != null)
            {
                _databaseService.UpdateCourse(Course);

                // Schedule or cancel course date notifications
                await ScheduleCourseNotifications();

                await Shell.Current.DisplayAlertAsync("Success", "Course updated successfully!", "OK");
            }
        }

        private async Task ScheduleCourseNotifications()
        {
            // Cancel existing course notifications
            CancelNotification($"course_start_{Course.Id}");
            CancelNotification($"course_end_{Course.Id}");

            // Schedule new course notifications if enabled
            if (Course.StartDateNotifications && Course.StartDate > DateTime.Now)
            {
                await ScheduleNotification(
                    $"course_start_{Course.Id}",
                    $"Course Starting: {Course.Name}",
                    $"Your course '{Course.Name}' starts today!",
                    Course.StartDate
                );
            }

            if (Course.EndDateNotifications && Course.EndDate > DateTime.Now)
            {
                await ScheduleNotification(
                    $"course_end_{Course.Id}",
                    $"Course Ending: {Course.Name}",
                    $"Your course '{Course.Name}' ends today!",
                    Course.EndDate
                );
            }

            // Handle assessment notifications
            {
                for (int i = 0; i < 2; i++)
                {
                    // Cancel existing assessment notifications
                    CancelNotification($"assessment_start_{Assessment[i].Id}");
                    CancelNotification($"assessment_end_{Assessment[i].Id}");

                    // Schedule new assessment notifications if enabled
                    if (Assessment[i].StartDateNotifications && Assessment[i].StartDate > DateTime.Now)
                    {
                        await ScheduleNotification(
                            $"assessment_start_{Assessment[i].Id}",
                            $"Assessment Starting: {Assessment[i].Name}",
                            $"Your assessment '{Assessment[i].Name}' starts today!",
                            Assessment[i].StartDate
                        );
                    }

                    if (Assessment[i].EndDateNotifications && Assessment[i].EndDate > DateTime.Now)
                    {
                        await ScheduleNotification(
                            $"assessment_end_{Assessment[i].Id}",
                            $"Assessment Due: {Assessment[i].Name}",
                            $"Your assessment '{Assessment[i].Name}' is due today!",
                            Assessment[i].EndDate
                        );
                    }
                }
            }
        }

        private async Task ScheduleNotification(string id, string title, string message, DateTime scheduleTime)
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
                        NotifyTime = scheduleTime
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

        public void Initialize()
        {
            // Initialize properties
            Course = new Course(); // Ensure Course is not null
            StatusOptions = ["In Progress", "Completed", "Dropped", "Planned"];
            AssessmentTypeOptions = ["Objective", "Performance"];
            IsEditing = false; // Start in display mode

            // Request notification permissions
            _ = RequestNotificationPermissions();
        }

        public CourseDetailsViewModel(DatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        private async Task RequestNotificationPermissions()
        {
            await LocalNotificationCenter.Current.RequestNotificationPermission();
        }

        [RelayCommand]
        async Task AddNote()
        {
            await Shell.Current.GoToAsync($"{nameof(AddNoteView)}", true, new Dictionary<string, object>
            {
                { "course", Course }
            });
        }

        [RelayCommand]
        async Task ViewNotes()
        {
            await Shell.Current.GoToAsync($"{nameof(ViewNotesView)}", true, new Dictionary<string, object>
            {
                { "course", Course }
            });
        }

        [RelayCommand]
        private async Task GoBack()
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}