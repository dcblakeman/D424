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
    [QueryProperty(nameof(Term), "term")]
    [QueryProperty(nameof(Instructor), "instructor")]
    [QueryProperty(nameof(User), "user")]
    public partial class CourseDetailsViewModel : ObservableObject
    {
        private readonly DatabaseService _database;
        private readonly NotificationService _notification;
        private readonly PermissionService _permission;

        [ObservableProperty]
        public User user = new();

        [ObservableProperty]
        public User newUser;

        [ObservableProperty]
        private Course course;

        // Core Properties
        [ObservableProperty]
        private Course newCourse = new();

        [ObservableProperty]
        private AcademicTerm term;

        [ObservableProperty]
        private AcademicTerm newTerm;

        [ObservableProperty]
        private int newCourseId = 0;

        [ObservableProperty]
        private string newCourseName = string.Empty;

        [ObservableProperty]
        public string newCourseDescription = string.Empty;

        [ObservableProperty]
        public DateTime newCourseStartDate = DateTime.Today;

        [ObservableProperty]
        public DateTime newCourseEndDate = DateTime.Today.AddMonths(6);

        [ObservableProperty]
        public CourseStatus newCourseStatus = CourseStatus.Planned;

        [ObservableProperty]
        public FinalGrade newCourseGrade = FinalGrade.NotGraded;

        [ObservableProperty]
        public bool newCourseStartDateNotifications = false;

        [ObservableProperty]
        public bool newCourseEndDateNotifications = false;

        [ObservableProperty]
        private int startDateNotificationDays = 1; // Default to 1 day

        [ObservableProperty]
        private int endDateNotificationDays = 1;   // Default to 1 day

        [ObservableProperty]
        public int newCourseTermId = 0;

        [ObservableProperty]
        public int newCourseInstructorId = 0;

        [ObservableProperty]
        private CourseInstructor instructor;

        [ObservableProperty]
        private CourseInstructor newInstructor;

        [ObservableProperty]
        private string viewName = "Course Details";

        // UI State
        [ObservableProperty]
        private bool isEditing;

        public bool IsNotEditing => !IsEditing;

        private bool _isLoadingData = false; // Add this flag

        // Dynamic UI Properties
        public string Title => IsEditing ? "Edit Course Details" : "Course Details";
        public string EditButtonText => IsEditing ? "Save Changes" : "Edit Course Details";
        public string EditButtonColor => IsEditing ? "#4CAF50" : "#2196F3";

        // Static Collections
        public ObservableCollection<CourseStatus> StatusOptions { get; set; } =
        [
            CourseStatus.NotEnrolled,
            CourseStatus.InProgress,
            CourseStatus.Completed,
            CourseStatus.Dropped,
            CourseStatus.Planned
        ];

        public ObservableCollection<FinalGrade> GradeOptions { get; set; } =
        [
            FinalGrade.A,
            FinalGrade.AMinus,
            FinalGrade.BPlus,
            FinalGrade.B,
            FinalGrade.BMinus,
            FinalGrade.CPlus,
            FinalGrade.C,
            FinalGrade.CMinus,
            FinalGrade.DPlus,
            FinalGrade.D,
            FinalGrade.DMinus,
            FinalGrade.F,
            FinalGrade.NotGraded
        ];

        public ObservableCollection<AssessmentType> AssessmentTypeOptions { get; set; } =
        [
            AssessmentType.Objective,
            AssessmentType.Performance
        ];

        public CourseDetailsViewModel(DatabaseService database, NotificationService notification, PermissionService permission)
        {
            _database = database;
            _notification = notification;
            _permission = permission;

        }

        partial void OnNewCourseStartDateNotificationsChanged(bool value)
        {
            if (_isLoadingData) return; // Skip during data loading

            if (value && Course?.Id > 0)
            {
                _ = Task.Run(async () => await HandleStartDateNotificationToggle());
            }
            else if (!value && Course?.Id > 0)
            {
                _ = Task.Run(async () => await _notification.CancelNotificationAsync(Course.Id));
            }
        }

        partial void OnNewCourseEndDateNotificationsChanged(bool value)
        {
            if (_isLoadingData) return; // Skip during data loading

            if (value && Course?.Id > 0)
            {
                _ = Task.Run(async () => await HandleEndDateNotificationToggle());
            }
            else if (!value && Course?.Id > 0)
            {
                _ = Task.Run(async () => await _notification.CancelNotificationAsync(10000 + Course.Id));
            }
        }

        [RelayCommand]
        public async Task ToggleStartNotificationsAsync()
        {
            if (NewCourseStartDateNotifications && Course?.Id > 0)
            {
                await HandleStartDateNotificationToggle();
            }
            else if (!NewCourseStartDateNotifications && Course?.Id > 0)
            {
                await _notification.CancelNotificationAsync(Course.Id);
            }
        }

        [RelayCommand]
        public async Task ToggleEndNotificationsAsync()
        {
            if (NewCourseEndDateNotifications && Course?.Id > 0)
            {
                await HandleEndDateNotificationToggle();
            }
            else if (!NewCourseEndDateNotifications && Course?.Id > 0)
            {
                await _notification.CancelNotificationAsync(10000 + Course.Id);
            }
        }

        private async Task HandleStartDateNotificationToggle()
        {
            try
            {
                // Ask user for number of days
                string daysInput = await MainThread.InvokeOnMainThreadAsync(async () =>
                    await Shell.Current.DisplayPromptAsync(
                        "Start Date Notification",
                        "How many days in advance would you like to be notified?",
                        "OK",
                        "Cancel",
                        "Enter number of days",
                        3, // Max length
                        Keyboard.Numeric,
                        "1" // Default value
                    ));

                if (string.IsNullOrWhiteSpace(daysInput))
                {
                    // User canceled - turn switch back off
                    MainThread.BeginInvokeOnMainThread(() => NewCourseStartDateNotifications = false);
                    return;
                }

                if (!int.TryParse(daysInput, out int days) || days < 1 || days > 30)
                {
                    await MainThread.InvokeOnMainThreadAsync(async () =>
                        await Shell.Current.DisplayAlertAsync("Invalid Input",
                            "Please enter a number between 1 and 30 days.", "OK"));

                    MainThread.BeginInvokeOnMainThread(() => NewCourseStartDateNotifications = false);
                    return;
                }

                // Store the preference
                StartDateNotificationDays = days;

                // Calculate reminder date
                DateTime reminderDate = Course.StartDate.AddDays(-days);

                if (reminderDate <= DateTime.Now)
                {
                    await MainThread.InvokeOnMainThreadAsync(async () =>
                        await Shell.Current.DisplayAlertAsync("Cannot Set Reminder",
                            $"This assessment starts too soon for a {days}-day advance notice."
                            + $"Please change the start date and save before setting notifications.", "OK"));

                    MainThread.BeginInvokeOnMainThread(() => NewCourseStartDateNotifications = false);
                    return;
                }

                // Schedule notification
                bool success = await _notification.ScheduleCourseStartReminderAsync(NewCourse, reminderDate);

                if (!success)
                {
                    MainThread.BeginInvokeOnMainThread(() => NewCourseStartDateNotifications = false);
                    await MainThread.InvokeOnMainThreadAsync(async () =>
                        await Shell.Current.DisplayAlertAsync("Permission Required",
                            "Please enable notifications in your device settings.", "OK"));
                }
                else
                {
                    await MainThread.InvokeOnMainThreadAsync(async () =>
                        await Shell.Current.DisplayAlertAsync("Reminder Set",
                            $"You'll be reminded {days} day(s) before the assessment starts.", "OK"));
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlertAsync("Alert", $"Error handling start date notification: {ex.Message}", "OK");
                MainThread.BeginInvokeOnMainThread(() => NewCourseStartDateNotifications = false);
            }
        }

        partial void OnUserChanged(User value)
        {
            NewUser = value;
        }
        // Property Change Handlers
        partial void OnCourseChanged(Course value)
        {
            NewCourse = value;
            IsEditing = false;
            _ = LoadInstructorAsync();
            _ = PopulateFields();
        }

        partial void OnTermChanged(AcademicTerm value)
        {
            NewTerm = value;
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
        public async Task SaveCourse()
        {
            try
            {
                //Refresh the page
                OnPropertyChanged(nameof(NewCourse));

                // Update course properties
                NewCourse.Id = NewCourseId;
                NewCourse.Name = NewCourseName;
                NewCourse.Description = NewCourseDescription;
                NewCourse.StartDate = NewCourseStartDate;
                NewCourse.EndDate = NewCourseEndDate;
                NewCourse.StartDateNotifications = NewCourseStartDateNotifications;
                NewCourse.Status = NewCourseStatus;
                NewCourse.Grade = NewCourseGrade;
                NewCourse.EndDateNotifications = NewCourseEndDateNotifications;
                NewCourse.InstructorId = NewCourseInstructorId;

                // Update notifications after saving
                await ScheduleCourseNotifications();

                // **ADD THIS: Auto-enroll the current user in the new course**
                UserCourse userCourse = new()
                {
                    UserId = NewUser.Id,  // Use your User property
                    CourseId = NewCourse.Id
                };
                await _database.SaveUserCourseAsync(userCourse);

                // Save to the database
                _ = await _database.SaveCourseAsync(NewCourse);
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
            if (NewCourse == null)
            {
                return;
            }

            try
            {
                await Shell.Current.GoToAsync($"{nameof(CourseInstructorView)}", true, new Dictionary<string, object>
                {
                    ["course"] = NewCourse,
                    ["term"] = NewTerm,
                    ["user"] = NewUser,
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
            if (NewCourse == null)
            {
                return;
            }

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

        [RelayCommand]
        private async Task AddNote()
        {
            if (NewCourse == null)
            {
                return;
            }

            try
            {
                await Shell.Current.GoToAsync($"{nameof(AddNoteView)}", new Dictionary<string, object>
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

        [RelayCommand]
        private async Task ViewNotes()
        {
            if (NewCourse == null)
            {
                return;
            }

            try
            {
                await Shell.Current.GoToAsync($"{nameof(ViewNotesView)}", true, new Dictionary<string, object>
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

                await Shell.Current.GoToAsync($"{nameof(CourseListView)}", true, new Dictionary<string, object>
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

        public class RefreshCoursesMessage { }

        // Notification Management
        private async Task ScheduleCourseNotifications()
        {
            if (NewCourse == null)
            {
                return;
            }

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
                NotificationRequest notification = new()
                {
                    NotificationId = id.GetHashCode(),
                    Title = title,
                    Description = message,
                    Schedule = new NotificationRequestSchedule
                    {
                        NotifyTime = notifyTime
                    }
                };

                _ = await LocalNotificationCenter.Current.Show(notification);
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
                _ = LocalNotificationCenter.Current.Cancel(id.GetHashCode());
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to cancel notification: {ex.Message}");
            }
        }

        private async Task HandleEndDateNotificationToggle()
        {
            try
            {
                string daysInput = await MainThread.InvokeOnMainThreadAsync(async () =>
                    await Shell.Current.DisplayPromptAsync(
                        "Due Date Notification",
                        "How many days in advance would you like to be notified?",
                        "OK",
                        "Cancel",
                        "Enter number of days",
                        3,
                        Keyboard.Numeric,
                        "1"
                    ));

                if (string.IsNullOrWhiteSpace(daysInput))
                {
                    MainThread.BeginInvokeOnMainThread(() => NewCourseEndDateNotifications = false);
                    return;
                }

                if (!int.TryParse(daysInput, out int days) || days < 1 || days > 30)
                {
                    await MainThread.InvokeOnMainThreadAsync(async () =>
                        await Shell.Current.DisplayAlertAsync("Invalid Input",
                            "Please enter a number between 1 and 30 days.", "OK"));

                    MainThread.BeginInvokeOnMainThread(() => NewCourseEndDateNotifications = false);
                    return;
                }

                EndDateNotificationDays = days;

                DateTime reminderDate = NewCourse.EndDate.AddDays(-days);

                if (reminderDate <= DateTime.Now)
                {
                    await MainThread.InvokeOnMainThreadAsync(async () =>
                        await Shell.Current.DisplayAlertAsync("Cannot Set Reminder",
                            $"This assessment is due too soon for a {days}-day advance notice." +
                            $"Please change the start date and save before setting notifications.", "OK"));

                    MainThread.BeginInvokeOnMainThread(() => NewCourseEndDateNotifications = false);
                    return;
                }

                bool success = await _notification.ScheduleCourseDueReminderAsync(NewCourse, reminderDate);

                if (!success)
                {
                    MainThread.BeginInvokeOnMainThread(() => NewCourseEndDateNotifications = false);
                    await MainThread.InvokeOnMainThreadAsync(async () =>
                        await Shell.Current.DisplayAlertAsync("Permission Required",
                            "Please enable notifications in your device settings.", "OK"));
                }
                else
                {
                    await MainThread.InvokeOnMainThreadAsync(async () =>
                        await Shell.Current.DisplayAlertAsync("Reminder Set",
                            $"You'll be reminded {days} day(s) before the assessment is due.", "OK"));
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error handling end date notification: {ex.Message}");
                MainThread.BeginInvokeOnMainThread(() => NewCourseEndDateNotifications = false);
            }
        }

        [RelayCommand]
        public async Task TestNotificationAsync()
        {
            // Schedule notification for 30 seconds from now
            DateTime testTime = DateTime.Now.AddSeconds(30);

            bool success = false;
            try
            {
                NotificationRequest request = new()
                {
                    NotificationId = 9999,
                    Title = "Test Notification",
                    Description = "This is a test notification scheduled 30 seconds from now.",
                    Schedule = { NotifyTime = testTime }
                };
                _ = await LocalNotificationCenter.Current.Show(request);
                success = true;
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlertAsync("Error", $"Failed to schedule test notification: {ex.Message}", "OK");

            }


            if (success)
            {
                await Shell.Current.DisplayAlertAsync("Test Scheduled",
                    "Notification will appear in 30 seconds. Close the app completely!", "OK");
            }
        }

        public async Task<bool> ScheduleTestNotificationAsync(string title, string message, int secondsFromNow = 10)
        {
            if (!await EnsurePermissionAsync())
            {
                return false;
            }

            DateTime testTime = DateTime.Now.AddSeconds(secondsFromNow);

            System.Diagnostics.Debug.WriteLine($"Scheduling test notification for: {testTime:HH:mm:ss}");
            System.Diagnostics.Debug.WriteLine($"Current time: {DateTime.Now:HH:mm:ss}");

            NotificationRequest request = new()
            {
                NotificationId = 9999, // Test ID
                Title = $"TEST: {title}",
                Subtitle = "EduTrack Test",
                Description = message,
                Schedule = { NotifyTime = testTime }
            };

            _ = await LocalNotificationCenter.Current.Show(request);
            return true;
        }

        private async Task<bool> EnsurePermissionAsync()
        {
            return await _permission.HasNotificationPermissionAsync() ||
                   await _permission.RequestNotificationPermissionAsync();
        }

        private async Task LoadInstructorAsync()
        {
            if (NewCourse == null)
            {
                return;
            }

            try
            {
                CourseInstructor instructor = await _database.GetInstructorByIdAsync(NewCourse.InstructorId);
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

        public async Task PopulateFields()
        {
            try
            {
                _isLoadingData = true;

                if (NewCourse != null)
                {
                    NewCourseId = NewCourse.Id;
                    NewCourseName = NewCourse.Name;
                    NewCourseDescription = NewCourse.Description;
                    NewCourseStartDate = NewCourse.StartDate;
                    NewCourseEndDate = NewCourse.EndDate;
                    NewCourseStatus = NewCourse.Status;
                    NewCourseStartDateNotifications = NewCourse.StartDateNotifications;
                    NewCourseEndDateNotifications = NewCourse.EndDateNotifications;
                    NewCourseTermId = NewCourse.TermId;
                    NewCourseGrade = NewCourse.Grade;
                    NewCourseInstructorId = NewCourse.InstructorId;
                    await LoadInstructorAsync();
                }
            }
            finally
            {
                _isLoadingData = false;
            }

        }
    }
}

