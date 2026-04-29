using C_971.Models;
using C_971.Services;
using C_971.Utilities;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace C_971.ViewModels
{
    [QueryProperty(nameof(Course), "course")]
    [QueryProperty(nameof(Term), "term")]
    [QueryProperty(nameof(Instructor), "instructor")]
    [QueryProperty(nameof(User), "user")]
    public partial class CourseInstructorViewModel : ObservableObject
    {
        private readonly DatabaseService _database;

        // Core Properties

        [ObservableProperty]
        private User user = new();

        [ObservableProperty]
        private User newUser = null!;

        [ObservableProperty]
        private Course course = new();

        [ObservableProperty]
        private Course newCourse = null!;

        [ObservableProperty]
        private AcademicTerm term = null!;

        [ObservableProperty]
        private AcademicTerm newTerm = null!;

        [ObservableProperty]
        private CourseInstructor instructor = null!;

        [ObservableProperty]
        private CourseInstructor newInstructor = null!;

        // New Instructor Form
        [ObservableProperty]
        private int newInstructorId = 0;

        [ObservableProperty]
        private string newInstructorName = string.Empty;

        [ObservableProperty]
        private string newInstructorPhone = string.Empty;

        [ObservableProperty]
        private string newInstructorEmail = string.Empty;

        [ObservableProperty]
        private string viewName = "Course Instructor";

        // UI State
        [ObservableProperty]
        private bool isAddingInstructor;

        [ObservableProperty]
        private bool isRemovingInstructor;

        [ObservableProperty]
        private bool isRefreshing;

        public bool IsNotAddingInstructor => !IsAddingInstructor && !IsRemovingInstructor;

        public bool IsNotRemovingInstructor => !IsRemovingInstructor;

        // Collections
        [ObservableProperty]
        private ObservableCollection<CourseInstructor> instructors = [];

        // Holds Instructor list for searching
        private List<CourseInstructor> _allInstructors = [];

        [ObservableProperty]
        private string searchText = string.Empty;

        public CourseInstructorViewModel(DatabaseService database)
        {
            _database = database;
        }

        // Property Change Handlers
        partial void OnCourseChanged(Course value)
        {
            NewCourse = value;
            _ = LoadInstructorsAsync();
        }

        partial void OnInstructorChanged(CourseInstructor value)
        {
            NewInstructor = value;
        }

        partial void OnUserChanged(User value)
        {
            NewUser = value;
        }

        partial void OnIsAddingInstructorChanged(bool value)
        {
            OnPropertyChanged(nameof(IsNotAddingInstructor));
        }

        partial void OnIsRemovingInstructorChanged(bool value)
        {
            OnPropertyChanged(nameof(IsNotAddingInstructor));
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

        // Data Loading
        [RelayCommand]
        private async Task LoadInstructorsAsync()
        {
            IsRefreshing = true;
            try
            {
                _allInstructors = await _database.GetCourseInstructorsAsync();
                ApplySearchFilter();
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlertAsync("Error", $"Failed to load instructors: {ex.Message}", "OK");
                System.Diagnostics.Debug.WriteLine($"Load instructors error: {ex}");
            }
            finally
            {
                IsRefreshing = false;
            }
        }

        // Instructor Management
        [RelayCommand]
        private void StartAddingInstructor()
        {
            IsAddingInstructor = true;
        }

        [RelayCommand]
        private void CancelAddInstructor()
        {
            IsAddingInstructor = false;
            ClearForm();
        }

        [RelayCommand]
        private void RemoveInstructor()
        {
            IsRemovingInstructor = true;
        }

        [RelayCommand]
        private void CancelRemoveInstructor()
        {
            IsRemovingInstructor = false;
        }


        [RelayCommand]
        private async Task AddCourseInstructor()
        {
            NewInstructor = new CourseInstructor();
            if (!ValidateInstructor())
            {
                return;
            }

            try
            {
                NewInstructor.Name = NewInstructorName.Trim();
                NewInstructor.Phone = NewInstructorPhone.Trim();
                NewInstructor.Email = NewInstructorEmail.Trim().ToLowerInvariant();

                _ = await _database.SaveCourseInstructorAsync(NewInstructor);

                // Add to cache and refresh display
                _allInstructors.Add(NewInstructor);
                ApplySearchFilter();

                ClearForm();
                IsAddingInstructor = false;

                await Shell.Current.DisplayAlertAsync("Success", "Instructor added successfully!", "OK");
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlertAsync("Error", $"Failed to add instructor: {ex.Message}", "OK");
                System.Diagnostics.Debug.WriteLine($"Add instructor error: {ex}");
            }
        }

        [RelayCommand]
        private async Task DeleteInstructor(CourseInstructor instructor)
        {
            if (instructor == null)
            {
                return;
            }

            try
            {
                bool confirmed = await Shell.Current.DisplayAlertAsync(
                    "Delete Instructor",
                    $"Are you sure you want to delete '{instructor.Name}'?",
                    "Delete",
                    "Cancel");

                if (!confirmed)
                {
                    return;
                }

                _ = await _database.DeleteCourseInstructorAsync(instructor);

                // Remove from cache and refresh display
                _ = _allInstructors.Remove(instructor);
                _ = Instructors.Remove(instructor);

                IsRemovingInstructor = false;

                await Shell.Current.DisplayAlertAsync("Success", "Instructor deleted successfully.", "OK");
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlertAsync("Error", $"Failed to delete instructor: {ex.Message}", "OK");
                System.Diagnostics.Debug.WriteLine($"Delete instructor error: {ex}");
            }
        }

        // Navigation
        [RelayCommand]
        private async Task GoBack()
        {
            try
            {
                await Shell.Current.GoToAsync("..", true, new Dictionary<string, object>
                {
                    ["course"] = NewCourse,
                    ["instructor"] = NewInstructor,
                    ["term"] = NewTerm,
                    ["user"] = NewUser
                });
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlertAsync("Navigation Error", $"Navigation failed: {ex.Message}", "OK");
                System.Diagnostics.Debug.WriteLine($"Navigation error: {ex}");
            }
        }

        // Helper Methods
        private bool ValidateInstructor()
        {
            if (string.IsNullOrWhiteSpace(NewInstructorName))
            {
                _ = Shell.Current.DisplayAlertAsync("Validation Error", "Instructor name is required", "OK");
                return false;
            }

            if (string.IsNullOrWhiteSpace(NewInstructorEmail))
            {
                _ = Shell.Current.DisplayAlertAsync("Validation Error", "Email address is required", "OK");
                return false;
            }

            if (string.IsNullOrWhiteSpace(NewInstructorPhone))
            {
                _ = Shell.Current.DisplayAlertAsync("Validation Error", "Phone number is required", "OK");
                return false;
            }

            // Basic email validation
            if (!ValidationRules.IsValidEmail(NewInstructorEmail))
            {
                _ = Shell.Current.DisplayAlertAsync("Validation Error", "Please enter a valid email address", "OK");
                return false;
            }

            // Check for duplicate email
            if (Instructors.Any(i => i.Email.Equals(NewInstructorEmail.Trim(), StringComparison.OrdinalIgnoreCase)))
            {
                _ = Shell.Current.DisplayAlertAsync("Validation Error", "An instructor with this email already exists", "OK");
                return false;
            }

            // Check for duplicate phone number
            if (Instructors.Any(i => i.Phone.Equals(NewInstructorPhone.Trim(), StringComparison.OrdinalIgnoreCase)))
            {
                _ = Shell.Current.DisplayAlertAsync("Validation Error", "An instructor with this phone number already exists", "OK");
                return false;
            }

            // Check for correct phone number format
            if (!ValidationRules.IsValidPhone(NewInstructorPhone))
            {
                _ = Shell.Current.DisplayAlertAsync("Validation Error", "Please enter a valid phone number", "OK");
                return false;
            }

            return true;
        }

        private void ClearForm()
        {
            NewInstructorName = string.Empty;
            NewInstructorPhone = string.Empty;
            NewInstructorEmail = string.Empty;

            NewInstructor = new CourseInstructor();
        }

        private void ApplySearchFilter()
        {
            Instructors.Clear();

            IEnumerable<CourseInstructor> filteredInstructors = string.IsNullOrWhiteSpace(SearchText)
                ? _allInstructors
                : _allInstructors.Where(i =>
                    i.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                    i.Email.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                    i.Phone.Contains(SearchText, StringComparison.OrdinalIgnoreCase));

            foreach (CourseInstructor? instructor in filteredInstructors)
            {
                Instructors.Add(instructor);
            }
        }

        internal async Task<Course> UpdateCourseAsync(Course course)
        {
            try
            {
                _ = await _database.SaveCourseAsync(course);
                return course;
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlertAsync("Error", $"Failed to update course: {ex.Message}", "OK");
                System.Diagnostics.Debug.WriteLine($"Update course error: {ex}");
                throw;
            }
        }
    }
}
