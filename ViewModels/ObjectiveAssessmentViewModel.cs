using C_971.Models;
using C_971.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Plugin.LocalNotification;
using System.Collections.ObjectModel;

namespace C_971.ViewModels
{
    [QueryProperty(nameof(NewCourse), "course")]
    public partial class ObjectiveAssessmentViewModel : ObservableObject
    {
        private readonly DatabaseService _database;

        // Core Properties
        [ObservableProperty]
        private Course newCourse;

        [ObservableProperty]
        private int courseId;

        [ObservableProperty]
        private string name = "Objective Assessment";

        [ObservableProperty]
        private string searchText = string.Empty;

        [ObservableProperty]
        private CourseAssessment assessment;

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
        private ObservableCollection<CourseAssessment> assessments = new();

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

        private IEnumerable<CourseAssessment> _allObjectiveAssessments;

        public bool IsNotSearching => !IsSearching;

        public bool IsNotRefreshing => !IsRefreshing;

        public bool IsNotAddingAssessment => !IsAddingAssessment;

        public bool IsNotRemovingAssessment => !IsRemovingAssessment;

        public bool IsNotSavingAssessment => !IsSavingAssessment;

        public bool IsNotDeletingAssessment => !IsDeletingAssessment;

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


        public ObjectiveAssessmentViewModel(DatabaseService database)
        {
            _database = database;
            _ = RequestNotificationPermissions();
            IsSearching = false;
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
                        ["course"] = NewCourse
                    });
                }
                catch (Exception ex)
                {
                    await Shell.Current.DisplayAlertAsync("Navigation Error", $"Navigation back failed: {ex.Message}", "OK");
                }
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
            if (AssessmentId == 0)
            {
                await Shell.Current.DisplayAlertAsync("Info", $"Course ID: {CourseId}", "OK");
                //Save the new assessment
                Assessment = new CourseAssessment
                {
                    CourseId = CourseId,
                    Name = AssessmentName,
                    Type = AssessmentType.Objective,
                    Status = AssessmentStatus,
                    StartDate = AssessmentStartDate,
                    EndDate = AssessmentEndDate,
                    Description = AssessmentDescription,
                    StartDateNotifications = AssessmentStartDateNotifications,
                    EndDateNotifications = AssessmentEndDateNotifications
                };
            }

            try
            {
                await _database.SaveCourseAssessmentAsync(Assessment);

                AssessmentId = Assessment.Id;

                // Update notifications if needed

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

                    // Navigate back since the assessment no longer exists
                    await Shell.Current.GoToAsync("..");
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

        async partial void OnAssessmentChanging(CourseAssessment assessment)
        {
            if (assessmentId == 0) return;
            assessment.IsActive = false;
            await SaveAssessment();
        }

        async partial void OnAssessmentChanged(CourseAssessment assessemnt)
        {
            if (AssessmentId == 0) return;
            Assessment.IsActive = true;
        }

        partial void OnNewCourseChanged(Course value)
        {
            if (value != null)
            {
                CourseId = value.Id;
                AssessmentType = AssessmentType.Performance;

                // Load existing assessments for this course
                _ = LoadAllObjectiveAssessments();

                // Populate assessment properties
                _ = PopulateAssessmentProperties();
            }
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
            Assessment = await _database.GetAssessmentbyCourseIdAndType(CourseId, AssessmentType.Objective);

            if (Assessment != null && AssessmentType == AssessmentType.Objective)
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

                    await Shell.Current.DisplayAlertAsync("Alert", $"Populated UI properties for assessment: {AssessmentName}", "OK");

                }
                catch (Exception ex)
                {
                    await Shell.Current.DisplayAlertAsync("Error", $"Failed to populate assessment properties: {ex.Message}", "OK");
                    System.Diagnostics.Debug.WriteLine($"PopulateAssessmentProperties error: {ex}");
                }

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
            // Navigate to a new PerformanceAssessmentViewModel with empty assessment
            var newAssessment = new CourseAssessment
            {
                CourseId = CourseId,
                Type = AssessmentType.Performance
            };
            try
            {
                await Shell.Current.GoToAsync("PerformanceAssessmentView", true, new Dictionary<string, object>
                {
                    ["course"] = NewCourse,
                    ["assessment"] = newAssessment
                });
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlertAsync("Navigation Error", $"Failed to navigate to add assessment: {ex.Message}", "OK");
            }
        }
    }
}
