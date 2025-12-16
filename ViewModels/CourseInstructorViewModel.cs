using C_971.Models;
using C_971.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace C_971.ViewModels
{
    [QueryProperty(nameof(Course), "course")]
    public partial class CourseInstructorViewModel : ObservableObject
    {
        private readonly DatabaseService _database;

        // Core Properties
        [ObservableProperty]
        private Course course;

        [ObservableProperty]
        private string name = "Course Instructor";

        // UI State
        [ObservableProperty]
        private bool isAddingInstructor;

        [ObservableProperty]
        private bool isRefreshing;

        public bool IsNotAddingInstructor => !IsAddingInstructor;

        // Collections
        [ObservableProperty]
        private ObservableCollection<CourseInstructor> instructors = new();

        // New Instructor Form
        [ObservableProperty]
        private string instructorName = string.Empty;

        [ObservableProperty]
        private string instructorPhone = string.Empty;

        [ObservableProperty]
        private string instructorEmail = string.Empty;

        [ObservableProperty]
        private string searchText = string.Empty;

        public CourseInstructorViewModel(DatabaseService databaseService)
        {
            _database = databaseService;
        }

        // Property Change Handlers
        partial void OnCourseChanged(Course value)
        {
            if (value != null)
            {
                Name = $"{value.Name} - Instructors";
                _ = LoadInstructorsAsync();
            }
        }

        partial void OnIsAddingInstructorChanged(bool value)
        {
            OnPropertyChanged(nameof(IsNotAddingInstructor));
        }

        // Data Loading
        [RelayCommand]
        private async Task LoadInstructorsAsync()
        {
            if (Course == null) return;

            IsRefreshing = true;
            try
            {
                var instructorList = await _database.GetCourseInstructorsAsync();

                Instructors.Clear();
                foreach (var instructor in instructorList)
                {
                    Instructors.Add(instructor);
                }
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
        private async Task AddCourseInstructor()
        {
            if (!ValidateInstructor()) return;

            try
            {
                var newInstructor = new CourseInstructor
                {
                    Name = InstructorName.Trim(),
                    Phone = InstructorPhone.Trim(),
                    Email = InstructorEmail.Trim().ToLowerInvariant()
                };

                await _database.SaveCourseInstructorAsync(newInstructor);
                Instructors.Add(newInstructor);

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
        private async Task EditInstructor(CourseInstructor instructor)
        {
            if (instructor == null) return;

            try
            {
                // For now, just show instructor details
                // You could navigate to an edit page or show inline editing
                await Shell.Current.DisplayAlertAsync(
                    "Instructor Details",
                    $"Name: {instructor.Name}\nEmail: {instructor.Email}\nPhone: {instructor.Phone}",
                    "OK");
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlertAsync("Error", $"Failed to show instructor details: {ex.Message}", "OK");
            }
        }

        [RelayCommand]
        private async Task DeleteInstructor(CourseInstructor instructor)
        {
            if (instructor == null) return;

            try
            {
                bool confirmed = await Shell.Current.DisplayAlertAsync(
                    "Delete Instructor",
                    $"Are you sure you want to delete '{instructor.Name}'?",
                    "Delete",
                    "Cancel");

                if (!confirmed) return;

                await _database.DeleteCourseInstructorAsync(instructor);
                Instructors.Remove(instructor);

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
                await Shell.Current.GoToAsync("CourseDetailsview", true, new Dictionary<string, object>
                {
                    ["course"] = Course
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
            if (string.IsNullOrWhiteSpace(InstructorName))
            {
                _ = Shell.Current.DisplayAlertAsync("Validation Error", "Instructor name is required", "OK");
                return false;
            }

            if (string.IsNullOrWhiteSpace(InstructorEmail))
            {
                _ = Shell.Current.DisplayAlertAsync("Validation Error", "Email address is required", "OK");
                return false;
            }

            if (string.IsNullOrWhiteSpace(InstructorPhone))
            {
                _ = Shell.Current.DisplayAlertAsync("Validation Error", "Phone number is required", "OK");
                return false;
            }

            // Basic email validation
            if (!IsValidEmail(InstructorEmail))
            {
                _ = Shell.Current.DisplayAlertAsync("Validation Error", "Please enter a valid email address", "OK");
                return false;
            }

            // Check for duplicate email
            if (Instructors.Any(i => i.Email.Equals(InstructorEmail.Trim(), StringComparison.OrdinalIgnoreCase)))
            {
                _ = Shell.Current.DisplayAlertAsync("Validation Error", "An instructor with this email already exists", "OK");
                return false;
            }

            return true;
        }

        private static bool IsValidEmail(string email)
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

        private void ClearForm()
        {
            InstructorName = string.Empty;
            InstructorPhone = string.Empty;
            InstructorEmail = string.Empty;
        }

        [RelayCommand]
        private async Task Search()
        {
            //Search through instructor list
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                await LoadInstructorsAsync();
            }
            else
            {
                var filteredInstructors = Instructors.Where(i =>
                    i.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                    i.Email.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                    i.Phone.Contains(SearchText, StringComparison.OrdinalIgnoreCase)).ToList();
                Instructors.Clear();
                foreach (var instructor in filteredInstructors)
                {
                    Instructors.Add(instructor);
                }
            }

        }
    }
}