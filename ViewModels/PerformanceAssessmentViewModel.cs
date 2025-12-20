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
        private string assessmentName = String.Empty;

        [ObservableProperty]
        private AssessmentType assessmentType = AssessmentType.Performance;

        [ObservableProperty]
        private AssessmentStatus assessmentStatus = AssessmentStatus.Pending;

        [ObservableProperty]
        private DateTime assessmentStartDate = DateTime.Today;

        [ObservableProperty]
        private DateTime assessmentEndDate = DateTime.Today.AddMonths(6);

        [ObservableProperty]
        private string assessmentDescription = String.Empty;

        [ObservableProperty]
        private string name = "Performance Assessment";

        [ObservableProperty]
        private string searchText = string.Empty;

        [ObservableProperty]
        private CourseAssessment assessment = new();

        [ObservableProperty]
        private int assessmentId;

        [ObservableProperty]
        private int courseId;

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

        [ObservableProperty]
        private bool isSearching;

        public bool IsNotSearching => !IsSearching;

        public bool IsNotRefreshing => !IsRefreshing;

        public bool IsNotAddingAssessment => !IsAddingAssessment;

        public bool IsNotRemovingAssessment => !IsRemovingAssessment;

        public bool IsNotSavingAssessment => !IsSavingAssessment;

        public bool IsNotDeletingAssessment => !IsDeletingAssessment;

        public bool IsNotEditing => !IsEditing;


        //public bool IsNotEditing => !IsEditing && !IsSavingAssessment && !IsDeletingAssessment && !IsAddingAssessment && !IsRemovingAssessment && !IsLoadingAssessments && !IsRefreshing;

        //public string EditButtonText => IsEditing ? "Save Assessment" : "Edit Assessment";

        //public string BackButtonText => IsEditing ? "Cancel" : "Back";

        public string EditButtonText => IsEditing ? "Save Assessment" : "Edit Assessment";
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

        public PerformanceAssessmentViewModel(DatabaseService database)
        {
            _database = database;
            _ = RequestNotificationPermissions();
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
                        ["course"] = Course
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
                Assessment.CourseId = CourseId;
                Assessment.Name = AssessmentName;
                Assessment.Type = AssessmentType.Performance;
                Assessment.Status = AssessmentStatus; // FIX: assign a single AssessmentStatus, not the list
                Assessment.StartDate = AssessmentStartDate;
                Assessment.EndDate = AssessmentEndDate;
                Assessment.Description = AssessmentDescription;
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
            OnPropertyChanged(nameof(BackButtonText));

            SearchText = "";
        }

        [RelayCommand]
        public async Task Search()
        {
            IsSearching = true;
            OnPropertyChanged(nameof(IsNotSearching));
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                Assessments = new ObservableCollection<CourseAssessment>(_allAssessments);
            }
            else
            {
                var filtered = _allAssessments.FindAll(a => a.Name.IndexOf(SearchText, StringComparison.OrdinalIgnoreCase) >= 0);
                Assessments = new ObservableCollection<CourseAssessment>(filtered);
            }
        }

        partial void OnCourseChanged(Course value)
        {
            if (value != null)
            {
                CourseId = value.Id;
                AssessmentType = AssessmentType.Performance;

                // Load existing assessments for this course
                _ = LoadCourseAssessments();
            }
        }

        partial void OnAssessmentChanged(CourseAssessment? oldValue, CourseAssessment newValue)
        {
            // Update UI for Assessment ID
            AssessmentId = Assessment.Id;

        }

        [RelayCommand]
        private async Task LoadCourseAssessments()
        {
            try
            {
                IsLoadingAssessments = true;

                // Get all assessments for this course
                var courseAssessments = await _database.GetCourseAssessmentsByCourseIdAsync(Course.Id);

                _allAssessments = courseAssessments?.ToList() ?? new List<CourseAssessment>();
                Assessments.Clear();

                foreach (var assessment in _allAssessments)
                {
                    Assessments.Add(assessment);
                }

            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlertAsync("Error", $"Failed to load assessments: {ex.Message}", "OK");
                System.Diagnostics.Debug.WriteLine($"Load assessments error: {ex}");
            }
            finally
            {
                IsLoadingAssessments = false;
                OnPropertyChanged(nameof(IsNotEditing)); // Update UI state
            }
        }
    }
}
