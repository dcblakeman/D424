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
    public partial class CourseDetailsViewModel : ObservableObject
    {
        private DatabaseService _databaseService;

        public ObservableCollection<string> StatusOptions { get; } = new ObservableCollection<string>
        {
            "NotEnrolled",
            "InProgress",
            "Completed",
            "Dropped",
            "Planned"
        };

        public ObservableCollection<string> AssessmentTypeOptions { get; } = new ObservableCollection<string>
        {
            "Objective",
            "Performance"
        };

        public string Title => IsEditing ? "Edit Course Details" : "Course Details"; // Dynamic title based on mode

        [ObservableProperty]
        Course course;

        [ObservableProperty]
        private CourseInstructor instructor = new CourseInstructor();

        public ObservableCollection<CourseAssessment> Assessment { get; set; } = new ObservableCollection<CourseAssessment>();

        [ObservableProperty]
        private bool isEditing;

        public bool IsNotEditing => !IsEditing;

        [ObservableProperty]
        int termId;

        [ObservableProperty]
        string id;

        [ObservableProperty]
        string name = "Course Details";

        public string EditButtonText => IsEditing ? "Save Changes" : "Edit Course Details";
        public string EditButtonColor => IsEditing ? "#4CAF50" : "#2196F3";
        [RelayCommand]
        async Task ToggleEdit()
        {
            if (IsEditing)
            {
                //// Validate before saving
                //if (!await ValidateCourse())
                //{
                //    return; // Don't toggle edit mode if validation fails
                //}

                // Save changes
                await SaveCourseDetails();
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

        //Get Instructor Command
        //Use search bar to see if Insturctor exists in database
        [RelayCommand]
        //Go to the CourseInstructorView
        async Task GetInstructor()
        {
            try
            {
                if (IsEditing)
                {
                    await SaveCourseDetails();
                }
                else
                {
                    IsEditing = false;
                    OnPropertyChanged(nameof(IsNotEditing));
                }
            }
            catch
            {
                await Shell.Current.DisplayAlertAsync("Unable to change screens", "Make sure values are correct", "OK");
            }
            finally
            {
                //Go to CourseInstructorview
                await Shell.Current.GoToAsync($"{nameof(CourseInstructorView)}", true, new Dictionary<string, object>
                {
                    { "course", Course }
                });
            }
        }

        //View Assessments Command
        [RelayCommand]
        private async Task ViewAssessments()
        {
            try
            {
                // Go back to courselistview with the term context
                await Shell.Current.GoToAsync("AssessmentsView", new Dictionary<string, object>
                {
                    ["course"] = Course       // Pass the actual Course object
                });
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlertAsync("Navigation Error", $"Navigation back failed: {ex.Message}", "OK");
            }
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
            try
            {
                // Save changes to the course via the service
                if (_databaseService != null && Course != null)
                {
                    await _databaseService.UpdateCourse(Course);

                    // Schedule or cancel course date notifications
                    await ScheduleCourseNotifications();

                    await Shell.Current.DisplayAlertAsync("Success", "Course updated successfully!", "OK");
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlertAsync("Error", $"Failed to save course details: {ex.Message}", "OK");

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
        }

        // Schedule assignment notifications
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

        public void Initialize()
        {
            // Initialize properties
            Course = new Course(); // Ensure Course is not null
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
        public async Task GoBack()
        {
            try
            {
                // Go back to courselistview with the term context
                await Shell.Current.GoToAsync("CourseListView", new Dictionary<string, object>
                {
                    ["course"] = Course       // Pass the actual Course object
                });
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlertAsync("Navigation Error", $"Navigation back failed: {ex.Message}", "OK");
            }
        }


    }
}