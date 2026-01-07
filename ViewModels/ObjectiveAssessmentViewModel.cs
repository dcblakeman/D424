using C_971.Models;
using C_971.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Plugin.LocalNotification;
using System.Collections.ObjectModel;

namespace C_971.ViewModels
{
    [QueryProperty(nameof(Course), "course")]
    [QueryProperty(nameof(Term), "term")]
    [QueryProperty(nameof(User), "user")]
    public partial class ObjectiveAssessmentViewModel : ObservableObject
    {
        private readonly DatabaseService _database;
        private readonly NotificationService _notification;

        // Core Properties

        [ObservableProperty]
        public User user;

        [ObservableProperty]
        public User newUser;

        [ObservableProperty]
        private Course course;

        [ObservableProperty]
        private Course newCourse;

        [ObservableProperty]
        private AcademicTerm term;

        [ObservableProperty]
        private AcademicTerm newTerm;

        [ObservableProperty]
        private string viewName = "Objective Assessment";

        [ObservableProperty]
        private string searchText = string.Empty;

        [ObservableProperty]
        private CourseAssessment assessment = new();

        [ObservableProperty]
        public int assessmentId = 0;

        [ObservableProperty]
        private string assessmentName = String.Empty;

        [ObservableProperty]
        private AssessmentType assessmentType = AssessmentType.Objective;

        [ObservableProperty]
        private AssessmentStatus assessmentStatus = AssessmentStatus.Pending;

        [ObservableProperty]
        private string assessmentDescription = String.Empty;

        [ObservableProperty]
        private DateTime assessmentStartDate = DateTime.Now;

        [ObservableProperty]
        private DateTime assessmentEndDate = DateTime.Now.AddMonths(6);

        [ObservableProperty]
        public bool assessmentStartDateNotifications = true;

        [ObservableProperty]
        public bool assessmentEndDateNotifications = true;

        [ObservableProperty]
        public bool assessmentIsActive = false;

        [ObservableProperty]
        private int assessmentCourseId = 0;

        [ObservableProperty]
        private ObservableCollection<CourseAssessment> assessments = new();

        private List<CourseAssessment> _allObjectiveAssessments = new();

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

        public bool IsNotSearching => !IsSearching;

        public bool IsNotEditing => !IsEditing;

        public string EditButtonText => IsEditing ? "Save Assessment" : "Edit Assessment";
        public string AddButtonText => IsAddingAssessment ? "Save Assessment" : "Add Assessment";
        public string BackButtonText => IsEditing || IsSearching ? "Cancel" : "Back";


        //Assessment StatusOptions
        [ObservableProperty]
        private List<AssessmentStatus> assessmentStatusOptions = new()
        {
            AssessmentStatus.Pending,
            AssessmentStatus.InProgress,
            AssessmentStatus.Completed,
            AssessmentStatus.Overdue
        };

        [ObservableProperty]
        private List<AssessmentType> assessmentTypeOptions = new()
        {
            AssessmentType.Objective,
            AssessmentType.Performance
        };

        public ObjectiveAssessmentViewModel(DatabaseService database, NotificationService notification)
        {
            _database = database;
            _notification = notification;
            IsSearching = false; IsSearching = false;
        }

        partial void OnUserChanged(User value)
        {
            NewUser = value;
            Shell.Current.DisplayAlertAsync("Updated Values", $"New User: {NewUser}", "OK");
        }

        partial void OnTermChanged(AcademicTerm value)
        {
            NewTerm = value;
        }

        partial void OnCourseChanged(Course value)
        {
            NewCourse = value;
            AssessmentCourseId = NewCourse.Id;
            AssessmentType = AssessmentType.Objective;

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
                        ["course"] = NewCourse,
                        ["term"] = NewTerm,
                        ["user"] = NewUser
                    });
                }
                catch (Exception ex)
                {
                    await Shell.Current.DisplayAlertAsync("Navigation Error", $"Navigation back failed: {ex.Message}", "OK");
                }
            }
        }


        [RelayCommand]
        public async Task SetReminderAsync()
        {
            var reminderDate = Assessment.EndDate.AddDays(-1);

            if (reminderDate <= DateTime.Now)
            {
                await Shell.Current.DisplayAlertAsync("Cannot Set Reminder",
                    "This assessment is due too soon.", "OK");
                return;
            }

            var success = await _notification.ScheduleAssessmentReminderAsync(Assessment, reminderDate);

            var message = success
                ? $"Reminder set for {reminderDate:MM/dd/yyyy}"
                : "Permission required. Check device settings.";

            await Shell.Current.DisplayAlertAsync("Reminder", message, "OK");
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
                if (Assessment != null)
                {
                    Assessment.IsActive = false;
                    await _database.SaveCourseAssessmentAsync(Assessment);

                    Assessment.Id = AssessmentId;
                    Assessment.CourseId = AssessmentCourseId;
                    Assessment.Name = AssessmentName;
                    Assessment.Type = AssessmentType.Objective;
                    Assessment.Status = AssessmentStatus;
                    Assessment.StartDate = AssessmentStartDate;
                    Assessment.EndDate = AssessmentEndDate;
                    Assessment.Description = AssessmentDescription;
                    Assessment.StartDateNotifications = AssessmentStartDateNotifications;
                    Assessment.EndDateNotifications = AssessmentEndDateNotifications;
                    AssessmentIsActive = true;
                    Assessment.IsActive = AssessmentIsActive;
                } 
                else
                {
                    Assessment = new CourseAssessment();
                    Assessment.Id = 0;
                    Assessment.CourseId = AssessmentCourseId;
                    Assessment.Name = AssessmentName;
                    Assessment.Type = AssessmentType.Objective;
                    Assessment.Status = AssessmentStatus;
                    Assessment.StartDate = AssessmentStartDate;
                    Assessment.EndDate = AssessmentEndDate;
                    Assessment.Description = AssessmentDescription;
                    Assessment.StartDateNotifications = AssessmentStartDateNotifications;
                    Assessment.EndDateNotifications = AssessmentEndDateNotifications;
                    AssessmentIsActive = true;
                    Assessment.IsActive = AssessmentIsActive;
                }

                Assessment.IsActive = true;
                await _database.SaveCourseAssessmentAsync(Assessment);

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

        [RelayCommand]
        internal async Task DeleteAssessment()
        {
            if (Assessment == null) return;

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
                    AssessmentName = String.Empty;
                    AssessmentType = AssessmentType.Objective;
                    AssessmentDescription = String.Empty;
                    AssessmentStartDate = DateTime.Now;
                    AssessmentEndDate = DateTime.Now.AddMonths(6);
                    AssessmentStatus = AssessmentStatus.Pending;
                    AssessmentCourseId = NewCourse.Id;

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
            IsSearching = true; // This will show the search bar
            OnPropertyChanged(nameof(IsNotSearching));

            _ = LoadAllObjectiveAssessments();

            SearchText = string.Empty; // Clear any existing search textq
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

            var filteredAssessments = string.IsNullOrWhiteSpace(SearchText)
                ? _allObjectiveAssessments
                : _allObjectiveAssessments.Where(c => c.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase));

            foreach (var assessment in filteredAssessments)
            {
                Assessments.Add(assessment);
            }
        }
        private async Task PopulateAssessmentProperties()
        {
            Assessment = await _database.GetAssessmentbyCourseIdAndTypeAndIsActive(AssessmentCourseId, AssessmentType.Objective, Assessment.IsActive = true);

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
                    AssessmentStartDateNotifications = Assessment.StartDateNotifications;
                    AssessmentEndDateNotifications = Assessment.EndDateNotifications;
                    AssessmentIsActive = Assessment.IsActive;
                    AssessmentCourseId = Assessment.CourseId;

                    await Shell.Current.DisplayAlertAsync("Alert", $"Populated UI properties for assessment: {AssessmentName}", "OK");
                    return;
                }
                catch (Exception)
                {
                    await Shell.Current.DisplayAlertAsync("Error", $"Failed to populate assessment properties", "OK");
                }
            } else
            {
                
            }
                
        }

        [RelayCommand]
        private async Task LoadAllObjectiveAssessments()
        {
            IsRefreshing = true;
            try
            {
                // Load ALL Performance assessments for search functionality
                _allObjectiveAssessments = await _database.GetAssessmentsByTypeAsync(AssessmentType.Objective);
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
            AssessmentName = String.Empty;
            AssessmentType = AssessmentType.Objective;
            AssessmentDescription = String.Empty;
            AssessmentStartDate = DateTime.Now;
            AssessmentEndDate = DateTime.Now.AddMonths(6);
            AssessmentStatus = AssessmentStatus.Pending;
            AssessmentIsActive = true;
        }
    }
}
