using C_971.Models;
using C_971.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Plugin.LocalNotification;
using System.Collections.ObjectModel;

namespace C_971.ViewModels
{
    [QueryProperty(nameof(Course), "course")]
    public partial class AssessmentsViewModel : ObservableObject
    {
        private readonly DatabaseService _database;

        // Core Properties
        [ObservableProperty]
        private Course course;

        [ObservableProperty]
        private string name = "Assessments";

        [ObservableProperty]
        private string searchText = string.Empty;

        [ObservableProperty]
        private CourseAssessment assessment;

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

        public bool IsNotRefreshing => !IsRefreshing;

        public bool IsNotAddingAssessment => !IsAddingAssessment;

        public bool IsNotRemovingAssessment => !IsRemovingAssessment;

        public bool IsNotSavingAssessment => !IsSavingAssessment;

        public bool IsNotDeletingAssessment => !IsDeletingAssessment;


        public bool IsNotEditing => !IsEditing && !IsSavingAssessment && !IsDeletingAssessment && !IsAddingAssessment && !IsRemovingAssessment && !IsLoadingAssessments && !IsRefreshing;

        public string EditButtonText => IsEditing ? "Add Assessment" : "Edit Assessments";

        [RelayCommand]
        private async Task GoBack()
        {
            try
            {
                // Use relative navigation (no leading slash)
                await Shell.Current.GoToAsync("CourseDetailsView", true, new Dictionary<string, object>
                {
                    ["course"] = Course
                });
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlertAsync("Navigation Error", $"Navigation back failed: {ex.Message}", "OK");
            }
        }

        [RelayCommand]
        private async Task Edit()
        {
            if (IsEditing)
            {
                // Save logic here
                await SaveAssessment(Assessment);
            }

            IsEditing = !IsEditing;
            OnPropertyChanged(nameof(EditButtonText));
        }

        public AssessmentsViewModel(DatabaseService database)
        {
            _database = database;
            _ = RequestNotificationPermissions();
        }

        public async Task OnAppearingAsync()
        {
            if (Assessments.Count == 0)
            {
                await LoadAssessmentsAsync();
            }
        }


        // Property Change Handlers
        partial void OnCourseChanged(Course value)
        {
            if (value != null)
            {
                Name = $"{value.Name} - Assessments";
                _ = LoadAssessmentsAsync();
            }
        }

        partial void OnIsEditingChanged(bool value)
        {
            OnPropertyChanged(nameof(IsNotEditing));
        }

        // Data Loading
        [RelayCommand]
        private async Task LoadAssessmentsAsync()
        {
            if (Course == null) return;

            IsRefreshing = true;
            try
            {
                var assessmentList = await _database.GetCourseAssessmentsAsync(Course.Id);

                Assessments.Clear();
                foreach (var assessment in assessmentList)
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
                IsRefreshing = false;
            }
        }

        // Assessment Management
        [RelayCommand]
        private async Task AddAssessment()
        {
            if (Course == null) return;

            try
            {
                var newAssessment = new CourseAssessment
                {
                    CourseId = Course.Id,
                    Name = "New Assessment",
                    Type = AssessmentType.Objective,
                    Status = AssessmentStatus.Pending,
                    StartDate = DateTime.Now,
                    EndDate = DateTime.Now.AddDays(7)
                };

                await _database.SaveCourseAssessmentAsync(newAssessment);
                Assessments.Add(newAssessment);

                await Shell.Current.DisplayAlertAsync("Success", "Assessment added successfully!", "OK");
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlertAsync("Error", $"Failed to add assessment: {ex.Message}", "OK");
                System.Diagnostics.Debug.WriteLine($"Add assessment error: {ex}");
            }
        }

        [RelayCommand]
        private async Task SaveAssessment(CourseAssessment assessment)
        {
            if (assessment == null) return;

            try
            {
                await _database.SaveCourseAssessmentAsync(assessment);

                // Update notifications if needed
                await UpdateAssessmentNotifications(assessment);

                await Shell.Current.DisplayAlertAsync("Success", "Assessment saved successfully!", "OK");
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlertAsync("Error", $"Failed to save assessment: {ex.Message}", "OK");
                System.Diagnostics.Debug.WriteLine($"Save assessment error: {ex}");
            }
        }

        [RelayCommand]
        private async Task DeleteAssessment(CourseAssessment assessment)
        {
            if (assessment == null) return;

            try
            {
                bool confirmed = await Shell.Current.DisplayAlertAsync(
                    "Delete Assessment",
                    $"Are you sure you want to delete '{assessment.Name}'?",
                    "Delete",
                    "Cancel");

                if (!confirmed) return;

                await _database.DeleteCourseAssessmentAsync(assessment);
                Assessments.Remove(assessment);

                // Cancel any related notifications
                CancelNotification($"assessment_{assessment.Id}");

                await Shell.Current.DisplayAlertAsync("Success", "Assessment deleted successfully.", "OK");
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlertAsync("Error", $"Failed to delete assessment: {ex.Message}", "OK");
                System.Diagnostics.Debug.WriteLine($"Delete assessment error: {ex}");
            }
        }

        // Notification Management
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

        private void CancelNotification(int id)
        {
            try
            {
                LocalNotificationCenter.Current.Cancel(id);
                System.Diagnostics.Debug.WriteLine($"Cancelled notification: {id}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to cancel notification: {ex.Message}");
            }
        }

        [RelayCommand]
        private void Search()
        {
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                Assessments = new ObservableCollection<CourseAssessment>(_allAssessments);
            }
            else
            {
                var filtered = _allAssessments
                    .Where(a => a.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase))
                    .ToList();
                Assessments = new ObservableCollection<CourseAssessment>(filtered);
            }
        }
    }
}