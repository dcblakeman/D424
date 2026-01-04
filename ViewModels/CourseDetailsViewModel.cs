using C_971.Models;
using C_971.Services;
using C_971.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Plugin.LocalNotification;
using System.Collections.ObjectModel;

namespace C_971.ViewModels
{
    [QueryProperty(nameof(Course), "course")]
    [QueryProperty(nameof(Term), "term")]
    [QueryProperty(nameof(Instructor), "instructor")]
    [QueryProperty(nameof(UserId), "userid")]
    public partial class CourseDetailsViewModel : ObservableObject
    {
        private readonly DatabaseService _database;

        [ObservableProperty]
        private Course course;

        // Core Properties
        [ObservableProperty]
        private Course newCourse;

        [ObservableProperty]
        private AcademicTerm term;

        [ObservableProperty]
        private AcademicTerm newTerm;

        [ObservableProperty]
        private int userId;

        [ObservableProperty]
        private int newUserId;

        [ObservableProperty]
        private int newCourseId = 0;

        [ObservableProperty]
        private string newCourseName = String.Empty;

        [ObservableProperty]
        public string newCourseDescription = String.Empty;

        [ObservableProperty]
        public DateTime newCourseStartDate = DateTime.Today;

        [ObservableProperty]
        public DateTime newCourseEndDate = DateTime.Today.AddMonths(6);

        [ObservableProperty]
        public CourseStatus newCourseStatus = CourseStatus.Planned;

        [ObservableProperty]
        public bool newCourseStartDateNotifications = true;

        [ObservableProperty]
        public bool newCourseEndDateNotifications = true;

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

        // Dynamic UI Properties
        public string Title => IsEditing ? "Edit Course Details" : "Course Details";
        public string EditButtonText => IsEditing ? "Save Changes" : "Edit Course Details";
        public string EditButtonColor => IsEditing ? "#4CAF50" : "#2196F3";

        // Static Collections
        public ObservableCollection<CourseStatus> StatusOptions { get; set; } = new ObservableCollection<CourseStatus>
        {
            CourseStatus.NotEnrolled,
            CourseStatus.InProgress,
            CourseStatus.Completed,
            CourseStatus.Dropped,
            CourseStatus.Planned
        };

        public ObservableCollection<AssessmentType> AssessmentTypeOptions { get; set; } = new ObservableCollection<AssessmentType>
        {
            AssessmentType.Objective,
            AssessmentType.Performance
        };

        public CourseDetailsViewModel(DatabaseService databaseService)
        {
            _database = databaseService;
            _ = RequestNotificationPermissions();
        }

        partial void OnUserIdChanged(int value)
        {
            NewUserId = value;
        }
        // Property Change Handlers
        partial void OnCourseChanged(Course value)
        {
            NewCourse = value;
            IsEditing = false;
            _ = LoadInstructorAsync();
            _= PopulateFields();
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
                NewCourse.EndDateNotifications = NewCourseEndDateNotifications;
                NewCourse.InstructorId = NewCourseInstructorId;

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
        private async Task AddNote()
        {
            if (NewCourse == null) return;

            try
            {
                await Shell.Current.GoToAsync($"{nameof(AddNoteView)}", new Dictionary<string, object>
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
        private async Task ViewNotes()
        {
            if (NewCourse == null) return;

            try
            {
                await Shell.Current.GoToAsync($"{nameof(ViewNotesView)}", true, new Dictionary<string, object>
                {
                    ["course"] = Course,
                    ["userid"] = UserId,
                    ["term"] = NewTerm
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
                    ["course"] = Course,
                    ["term"] = Term,
                    ["userid"] = UserId
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

        public async Task PopulateFields()
        {
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
                NewCourseInstructorId = (int)NewCourse.InstructorId;
                await LoadInstructorAsync();
            }
        }
    }
}