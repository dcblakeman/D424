
using C_971.Models;
using C_971.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Plugin.LocalNotification;
using System.Collections.ObjectModel;
using Waher.Script.Constants;

namespace C_971.ViewModels
{
    [QueryProperty(nameof(Course), "course")]
    [QueryProperty(nameof(Term), "term")]
    [QueryProperty(nameof(User), "user")]
    public partial class PerformanceAssessmentViewModel : ObservableObject
    {
        private readonly DatabaseService _database;
        private readonly NotificationService _notification;

        // Core Properties
        [ObservableProperty]
        public User user;

        [ObservableProperty]
        public User newUser;

        [ObservableProperty]
        public Course course;

        [ObservableProperty]
        public Course newCourse;

        [ObservableProperty]
        public AcademicTerm term;

        [ObservableProperty]
        public AcademicTerm newTerm;

        [ObservableProperty]
        public string viewName = "Performance Assessment";

        [ObservableProperty]
        public string searchText = string.Empty;

        [ObservableProperty]
        public CourseAssessment assessment = new();

        [ObservableProperty]
        public int assessmentId = 0;

        [ObservableProperty]
        public string assessmentName = string.Empty;

        [ObservableProperty]
        public AssessmentType assessmentType = AssessmentType.Performance;

        [ObservableProperty]
        public AssessmentStatus assessmentStatus = AssessmentStatus.Pending;

        [ObservableProperty]
        public string assessmentDescription = string.Empty;

        [ObservableProperty]
        public DateTime assessmentStartDate = DateTime.Today;

        [ObservableProperty]
        public DateTime assessmentEndDate = DateTime.Today.AddMonths(6);

        [ObservableProperty]
        public bool assessmentStartDateNotifications = false;

        [ObservableProperty]
        public bool assessmentEndDateNotifications = false;

        [ObservableProperty]
        public DateTime assessmentStartDateReminderTime = DateTime.Now;

        [ObservableProperty]
        public DateTime assessmentEndDateReminderTime = DateTime.Now;

        [ObservableProperty]
        public int startDateNotificationDays = 1; // Default to 1 day

        [ObservableProperty]
        public int endDateNotificationDays = 1;   // Default to 1 day

        [ObservableProperty]
        public bool assessmentIsActive = false;

        [ObservableProperty]
        public int assessmentCourseId = 0;

        [ObservableProperty]
        public FinalGrade assessmentGrade = FinalGrade.NotGraded;

        [ObservableProperty]
        public ObservableCollection<CourseAssessment> assessments = [];

        public List<CourseAssessment> _allPerformanceAssessments = [];

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

        [ObservableProperty]
        private bool isSearching;

        [ObservableProperty]
        private bool isLoading;

        public bool IsNotLoading => !isLoading;

        public bool IsNotSearching => !IsSearching;

        public bool IsNotEditing => !IsEditing;

        public string EditButtonText => IsEditing ? "Save Assessment" : "Edit Assessment";
        public string AddButtonText => IsAddingAssessment ? "Save Assessment" : "Add Assessment";
        public string BackButtonText => IsEditing || IsSearching ? "Cancel" : "Back";

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

        //Assessment StatusOptions
        [ObservableProperty]
        private List<AssessmentStatus> assessmentStatusOptions =
        [
            AssessmentStatus.Pending,
            AssessmentStatus.InProgress,
            AssessmentStatus.Completed,
            AssessmentStatus.Overdue
        ];

        [ObservableProperty]
        private List<AssessmentType> assessmentTypeOptions =
        [
            AssessmentType.Objective,
            AssessmentType.Performance
        ];

        public PerformanceAssessmentViewModel(DatabaseService database, NotificationService notification)
        {
            _database = database;
            _notification = notification;
            IsSearching = false;
            IsEditing = false;
            IsLoading = false;
        }

        partial void OnUserChanged(User value)
        {
            NewUser = value;
        }

        partial void OnTermChanged(AcademicTerm value)
        {
            NewTerm = value;

        }

        partial void OnCourseChanged(Course value)
        {
            NewCourse = value;
            AssessmentCourseId = NewCourse.Id;
            AssessmentType = AssessmentType.Performance;

            // Populate assessment properties
            _ = PopulateAssessmentProperties();
        }

        [RelayCommand]
        private async Task GoBack()
        {
            if (IsEditing)
            {
                IsEditing = false;
                OnPropertyChanged(nameof(IsNotEditing));
                OnPropertyChanged(nameof(EditButtonText));
                OnPropertyChanged(nameof(BackButtonText));
            }
            else if (IsSearching)
            {
                IsSearching = false;
                OnPropertyChanged(nameof(IsNotSearching));
                OnPropertyChanged(nameof(BackButtonText));
            }
            else
            {
                try
                {
                    // Use relative navigation (no leading slash)
                    await Shell.Current.GoToAsync("AssessmentSelectionView", true, new Dictionary<string, object>
                    {
                        ["term"] = NewTerm,
                        ["course"] = NewCourse,
                        ["user"] = NewUser
                    });
                }
                catch (Exception ex)
                {
                    await Shell.Current.DisplayAlertAsync("Navigation Error", $"Navigation back failed: {ex.Message}", "OK");
                }
            }
        }

        partial void OnAssessmentStartDateNotificationsChanged(bool value)
        {
            if (value) // Only if toggled ON and we have a saved assessment
            {
                _ = Task.Run(async () => await HandleStartDateNotificationToggle());
            }
            else if (!value) // Toggled OFF
            {
                _ = Task.Run(async () => await _notification.CancelNotificationAsync(Assessment.Id));
            }
        }

        partial void OnAssessmentEndDateNotificationsChanged(bool value)
        {
            if (value) // Only if toggled ON and we have a saved assessment
            {
                _ = Task.Run(async () => await HandleEndDateNotificationToggle());
            }
            else if (!value) // Toggled OFF
            {
                _ = Task.Run(async () => await _notification.CancelNotificationAsync(10000 + Assessment.Id));
            }
        }

        private async Task HandleStartDateNotificationToggle()
        {
            if (IsLoading) return;
            try
            {
                // Calculate suggested reminder date (1 day before start)
                DateTime suggestedDate = Assessment.StartDate.AddDays(-1);
                string defaultDateTime = suggestedDate.ToString("MM/dd/yyyy hh:mm tt");

                // Ask user for specific date and time
                string dateTimeInput = await MainThread.InvokeOnMainThreadAsync(async () =>
                    await Shell.Current.DisplayPromptAsync(
                        "Start Date Notification",
                        $"When would you like to be reminded?\nAssessment starts: {Assessment.StartDate:MM/dd/yyyy}\n\nEnter date and time (MM/dd/yyyy hh:mm AM/PM):",
                        "OK",
                        "Cancel",
                        "MM/dd/yyyy hh:mm AM/PM",
                        25, // Max length for datetime string
                        Keyboard.Default,
                        defaultDateTime // Default to 1 day before at same time
                    ));

                if (string.IsNullOrWhiteSpace(dateTimeInput))
                {
                    // User canceled - turn switch back off
                    MainThread.BeginInvokeOnMainThread(() => AssessmentStartDateNotifications = false);
                    return;
                }

                // Try to parse the user input
                if (!DateTime.TryParse(dateTimeInput, out DateTime reminderDate))
                {
                    await MainThread.InvokeOnMainThreadAsync(async () =>
                        await Shell.Current.DisplayAlertAsync("Invalid Input",
                            "Please enter a valid date and time in the format: MM/dd/yyyy hh:mm AM/PM\n\nExample: 01/15/2024 2:30 PM", "OK"));

                    MainThread.BeginInvokeOnMainThread(() => AssessmentStartDateNotifications = false);
                    return;
                }

                // Validate that reminder date is before start date and in the future
                if (reminderDate >= Assessment.StartDate)
                {
                    await MainThread.InvokeOnMainThreadAsync(async () =>
                        await Shell.Current.DisplayAlertAsync("Invalid Date",
                            "Reminder date must be before the assessment start date.", "OK"));

                    MainThread.BeginInvokeOnMainThread(() => AssessmentStartDateNotifications = false);
                    return;
                }

                if (reminderDate <= DateTime.Now)
                {
                    await MainThread.InvokeOnMainThreadAsync(async () =>
                        await Shell.Current.DisplayAlertAsync("Cannot Set Reminder",
                            "Reminder date must be in the future.", "OK"));

                    MainThread.BeginInvokeOnMainThread(() => AssessmentStartDateNotifications = false);
                    return;
                }

                // Store the chosen reminder date
                AssessmentStartDateReminderTime = reminderDate;

                // Schedule notification
                bool success = await _notification.ScheduleAssessmentStartReminderAsync(Assessment, reminderDate);

                if (!success)
                {
                    MainThread.BeginInvokeOnMainThread(() => AssessmentStartDateNotifications = false);
                    await MainThread.InvokeOnMainThreadAsync(async () =>
                        await Shell.Current.DisplayAlertAsync("Permission Required",
                            "Please enable notifications in your device settings.", "OK"));
                }
                else
                {
                    await MainThread.InvokeOnMainThreadAsync(async () =>
                        await Shell.Current.DisplayAlertAsync("Reminder Set",
                            $"You'll be reminded on {reminderDate:MM/dd/yyyy} at {reminderDate:h:mm tt} before the assessment starts.", "OK"));
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error handling start date notification: {ex.Message}");
                MainThread.BeginInvokeOnMainThread(() => AssessmentStartDateNotifications = false);
            }
        }

        private async Task HandleEndDateNotificationToggle()
        {
            if (IsLoading) return;
            try
            {
                // Calculate suggested reminder date (1 day before due date)
                DateTime suggestedDate = Assessment.EndDate.AddDays(-1);
                string defaultDateTime = suggestedDate.ToString("MM/dd/yyyy hh:mm tt");

                string dateTimeInput = await MainThread.InvokeOnMainThreadAsync(async () =>
                    await Shell.Current.DisplayPromptAsync(
                        "Due Date Notification",
                        $"When would you like to be reminded?\nAssessment due: {Assessment.EndDate:MM/dd/yyyy}\n\nEnter date and time (MM/dd/yyyy hh:mm AM/PM):",
                        "OK",
                        "Cancel",
                        "MM/dd/yyyy hh:mm AM/PM",
                        25, // Max length for datetime string
                        Keyboard.Default,
                        defaultDateTime // Default to 1 day before at same time
                    ));

                if (string.IsNullOrWhiteSpace(dateTimeInput))
                {
                    MainThread.BeginInvokeOnMainThread(() => AssessmentEndDateNotifications = false);
                    return;
                }

                if (!DateTime.TryParse(dateTimeInput, out DateTime reminderDate))
                {
                    await MainThread.InvokeOnMainThreadAsync(async () =>
                        await Shell.Current.DisplayAlertAsync("Invalid Input",
                            "Please enter a valid date and time in the format: MM/dd/yyyy hh:mm AM/PM\n\nExample: 01/15/2024 2:30 PM", "OK"));

                    MainThread.BeginInvokeOnMainThread(() => AssessmentEndDateNotifications = false);
                    return;
                }

                // Validate that reminder date is before due date and in the future
                if (reminderDate >= Assessment.EndDate)
                {
                    await MainThread.InvokeOnMainThreadAsync(async () =>
                        await Shell.Current.DisplayAlertAsync("Invalid Date",
                            "Reminder date must be before the assessment due date.", "OK"));

                    MainThread.BeginInvokeOnMainThread(() => AssessmentEndDateNotifications = false);
                    return;
                }

                if (reminderDate <= DateTime.Now)
                {
                    await MainThread.InvokeOnMainThreadAsync(async () =>
                        await Shell.Current.DisplayAlertAsync("Cannot Set Reminder",
                            "Reminder date must be in the future.", "OK"));

                    MainThread.BeginInvokeOnMainThread(() => AssessmentEndDateNotifications = false);
                    return;
                }

                // Store the chosen reminder date
                AssessmentEndDateReminderTime = reminderDate;

                bool success = await _notification.ScheduleAssessmentDueReminderAsync(Assessment, reminderDate);

                if (!success)
                {
                    MainThread.BeginInvokeOnMainThread(() => AssessmentEndDateNotifications = false);
                    await MainThread.InvokeOnMainThreadAsync(async () =>
                        await Shell.Current.DisplayAlertAsync("Permission Required",
                            "Please enable notifications in your device settings.", "OK"));
                }
                else
                {
                    await MainThread.InvokeOnMainThreadAsync(async () =>
                        await Shell.Current.DisplayAlertAsync("Reminder Set",
                            $"You'll be reminded on {reminderDate:MM/dd/yyyy} at {reminderDate:h:mm tt} before the assessment is due.", "OK"));
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error handling end date notification: {ex.Message}");
                MainThread.BeginInvokeOnMainThread(() => AssessmentEndDateNotifications = false);
            }
        }


        partial void OnIsEditingChanged(bool value)
        {
            OnPropertyChanged(nameof(IsNotEditing));
            OnPropertyChanged(nameof(EditButtonText));
            OnPropertyChanged(nameof(BackButtonText));
        }

        [RelayCommand]
        private async Task EditAssessment()
        {
            if (IsEditing)
            {
                // Currently editing, so save changes
                await SaveAssessment();
                IsEditing = false;  // Exit edit mode after saving
            }
            else
            {
                // Not editing, so enter edit mode
                IsEditing = true;
            }
            OnPropertyChanged(nameof(IsNotEditing));
        }

        [RelayCommand]
        private async Task SaveAssessment()
        {
            try
            {
                if (Assessment != null && !IsLoading)
                {
                    Assessment.IsActive = false;
                    Assessment.StartDateNotifications = false;
                    Assessment.EndDateNotifications = false;
                    _ = await _database.SaveCourseAssessmentAsync(Assessment);

                    Assessment.Id = AssessmentId;
                    Assessment.CourseId = AssessmentCourseId;
                    Assessment.Name = AssessmentName;
                    Assessment.Type = AssessmentType.Performance;
                    Assessment.Status = AssessmentStatus;
                    Assessment.StartDate = AssessmentStartDate;
                    Assessment.EndDate = AssessmentEndDate;
                    Assessment.Description = AssessmentDescription;
                    AssessmentIsActive = true;
                    Assessment.IsActive = AssessmentIsActive;
                    Assessment.Grade = AssessmentGrade;
                    Assessment.StartDateNotifications = AssessmentStartDateNotifications;
                    Assessment.EndDateNotifications = AssessmentEndDateNotifications;
                }
                else
                {
                    Assessment = new CourseAssessment
                    {
                        Id = 0,
                        CourseId = AssessmentCourseId,
                        Name = AssessmentName,
                        Type = AssessmentType.Performance,
                        Status = AssessmentStatus,
                        StartDate = AssessmentStartDate,
                        EndDate = AssessmentEndDate,
                        Description = AssessmentDescription,
                        StartDateNotifications = AssessmentStartDateNotifications,
                        EndDateNotifications = AssessmentEndDateNotifications,
                        Grade = AssessmentGrade
                    };
                    AssessmentIsActive = true;
                    Assessment.IsActive = AssessmentIsActive;
                }

                await Task.Delay(100);

                _ = await _database.SaveCourseAssessmentAsync(Assessment);

                AssessmentId = Assessment.Id;
                await UpdateAssessmentNotifications(Assessment);
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
            if (assessment?.EndDate == null)
            {
                return;
            }

            try
            {
                // Cancel existing notification
                CancelNotification($"assessment_{assessment.Id}");

                // Schedule new notification if assessment is not completed and due date is in future
                if (assessment.Status != AssessmentStatus.Completed && assessment.EndDate > DateTime.Now)
                {
                    DateTime notifyTime = assessment.EndDate.AddDays(-1); // Notify 1 day before

                    if (notifyTime > DateTime.Now)
                    {
                        await ScheduleNotification(
                            $"assessment_{assessment.Id}",
                            "Assessment Due Tomorrow",
                            $"{assessment.Name} for {NewCourse?.Name ?? "course"} is due tomorrow",
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
                System.Diagnostics.Debug.WriteLine($"Cancelled notification: {id}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to cancel notification: {ex.Message}");
            }
        }

        [RelayCommand]
        internal async Task DeleteAssessment()
        {
            if (Assessment == null)
            {
                return;
            }

            bool answer = await Shell.Current.DisplayAlertAsync(
                "Delete Assessment",
                $"Are you sure you want to delete '{Assessment.Name}'?",
                "Yes",
                "No");

            if (answer)
            {
                try
                {
                    await _database.DeleteAssessmentAsync(Assessment.Id);
                    await Shell.Current.DisplayAlertAsync("Success", "Assessment deleted successfully.", "OK");

                    //Clear the Assessment Fields
                    AssessmentId = 0;
                    AssessmentName = string.Empty;
                    AssessmentType = AssessmentType.Performance;
                    AssessmentDescription = string.Empty;
                    AssessmentStartDate = DateTime.Now;
                    AssessmentEndDate = DateTime.Now.AddMonths(6);
                    AssessmentStatus = AssessmentStatus.Pending;
                    AssessmentCourseId = NewCourse.Id;
                    AssessmentGrade = FinalGrade.NotGraded;

                    await Task.Delay(100);
                    AssessmentStartDateNotifications = false;
                    AssessmentEndDateNotifications = false;

                }
                catch (Exception ex)
                {
                    await Shell.Current.DisplayAlertAsync("Error", $"Failed to delete assessment: {ex.Message}", "OK");
                }
            }
        }

        [RelayCommand]
        public async Task FindAssessment()
        {
            // Show the search bar and switch to search mode
            IsSearching = true;
            OnPropertyChanged(nameof(IsNotSearching));

            _ = LoadAllPerformanceAssessments();

            SearchText = string.Empty; // Clear any existing search text
        }

        partial void OnSearchTextChanged(string value)
        {
            ApplySearchFilter();
        }

        // Search and Filter
        [RelayCommand]
        private void Search()
        {
            ApplySearchFilter();
        }

        private void ApplySearchFilter()
        {
            Assessments.Clear();

            IEnumerable<CourseAssessment> filteredAssessments = string.IsNullOrWhiteSpace(SearchText)
                ? _allPerformanceAssessments
                : _allPerformanceAssessments.Where(c => c.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase));

            foreach (CourseAssessment? assessment in filteredAssessments)
            {
                Assessments.Add(assessment);
            }
        }

        private async Task PopulateAssessmentProperties()
        {
            Assessment = await _database.GetAssessmentbyCourseIdAndTypeAndIsActive(NewCourse.Id, AssessmentType.Performance, Assessment.IsActive = true);

            if (Assessment != null)
            {
                try
                {
                    AssessmentId = Assessment.Id;
                    AssessmentName = Assessment.Name;
                    AssessmentDescription = Assessment.Description;
                    AssessmentType = Assessment.Type;
                    AssessmentStatus = Assessment.Status;
                    AssessmentStartDate = Assessment.StartDate;
                    AssessmentEndDate = Assessment.EndDate;
                    AssessmentIsActive = Assessment.IsActive;
                    AssessmentCourseId = NewCourse.Id;
                    AssessmentGrade = Assessment.Grade;

                    await Task.Delay(100);
                    AssessmentStartDateNotifications = Assessment.StartDateNotifications;
                    AssessmentEndDateNotifications = Assessment.EndDateNotifications;
                }
                catch (Exception ex)
                {
                    await Shell.Current.DisplayAlertAsync("Error", $"Failed to populate assessment properties: {ex.Message}", "OK");
                    System.Diagnostics.Debug.WriteLine($"PopulateAssessmentProperties error: {ex}");
                }
                finally
                {
                    IsLoading = false;
                }

            }
        }

        [RelayCommand]
        private async Task LoadAllPerformanceAssessments()
        {
            IsRefreshing = true;
            try
            {
                // Load ALL Performance assessments for search functionality
                _allPerformanceAssessments = await _database.GetAssessmentsByTypeAsync(AssessmentType.Performance);
                ApplySearchFilter();
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlertAsync("Debug Error", ex.Message, "OK");
            }
            finally
            {
                IsRefreshing = false;
            }
        }

        [RelayCommand]
        private async Task AddAssessment()
        {
            IsSearching = false;
            IsEditing = true;

            OnPropertyChanged(nameof(AddButtonText));
            OnPropertyChanged(nameof(IsNotSearching));
            OnPropertyChanged(nameof(IsNotEditing));

            // Clear existing assessment properties for new entry
            AssessmentId = 0;
            AssessmentName = string.Empty;
            AssessmentType = AssessmentType.Performance;
            AssessmentDescription = string.Empty;
            AssessmentStartDate = DateTime.Now;
            AssessmentEndDate = DateTime.Now.AddMonths(6);
            AssessmentStatus = AssessmentStatus.Pending;
            AssessmentIsActive = true;
            AssessmentCourseId = NewCourse.Id;
            AssessmentGrade = FinalGrade.NotGraded;
        }
    }
}