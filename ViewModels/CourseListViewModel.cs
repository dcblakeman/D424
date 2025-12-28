using C_971.Models;
using C_971.Services;
using C_971.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace C_971.ViewModels
{
    [QueryProperty(nameof(NewTerm), "term")]
    [QueryProperty(nameof(NewCourse), "course")]
    public partial class CourseListViewModel : ObservableObject
    {
        private readonly DatabaseService _database;

        [ObservableProperty]
        private string viewName = "Course List";

        // Core Properties
        [ObservableProperty]
        private AcademicTerm newTerm = new AcademicTerm();

        [ObservableProperty]
        private Course newCourse = new Course(); 

        // New Course Form
        [ObservableProperty]
        private string newCourseName = string.Empty;

        [ObservableProperty]
        private DateTime newCourseStartDate = DateTime.Now;

        [ObservableProperty]
        private DateTime newCourseEndDate = DateTime.Now.AddMonths(6);

        [ObservableProperty]
        private CourseStatus newCourseStatus = CourseStatus.Planned;

        // UI State
        [ObservableProperty]
        private bool isAddingCourse;

        [ObservableProperty]
        private bool isRemovingCourse;

        [ObservableProperty]
        private bool isRefreshing;

        public bool IsNotAddingCourse => !IsAddingCourse;

        // Search
        [ObservableProperty]
        private string searchText = string.Empty;

        [ObservableProperty]
        private ObservableCollection<Course> courses = new();

        private List<Course> _allCourses = new List<Course>();

        // Static Collections
        public ObservableCollection<CourseStatus> StatusOptions { get; } = new ObservableCollection<CourseStatus>
        {
            CourseStatus.NotEnrolled,
            CourseStatus.InProgress,
            CourseStatus.Completed,
            CourseStatus.Dropped,
            CourseStatus.Planned
        };

        public CourseListViewModel(DatabaseService database)
        {
            _database = database;
        }

        // Property Change Handlers
        partial void OnNewTermChanged(AcademicTerm value)
        {
            if (value != null)
            {
                NewTerm = value;
                _ = LoadCoursesAsync(NewTerm);
            }
        }

        //partial void OnNewCourseChanged(Course value)
        //{
        //    // Refresh when returning from course details
        //    if (value != null)
        //    {
        //        _ = LoadCoursesAsync();
        //    }
        //}

        partial void OnSearchTextChanged(string value)
        {
            ApplySearchFilter();
        }

        partial void OnIsAddingCourseChanged(bool value)
        {
            OnPropertyChanged(nameof(IsNotAddingCourse));
        }

        // Search and Filter
        [RelayCommand]
        private void Search()
        {
            ApplySearchFilter();
        }

        private void ApplySearchFilter()
        {
            Courses.Clear();

            var filteredCourses = string.IsNullOrWhiteSpace(SearchText)
                ? _allCourses
                : _allCourses.Where(c => c.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase));

            foreach (var course in filteredCourses)
            {
                Courses.Add(course);
            }
        }

        // Data Loading
        [RelayCommand]
        private async Task LoadCoursesAsync(AcademicTerm term)
        {
            if(term == null) return;
            IsRefreshing = true;
            try
            {
                _allCourses = await _database.GetCoursesByTermIdAsync(term.Id);
                ApplySearchFilter();
            }
            finally
            {
                IsRefreshing = false;
            }
        }

        // Navigation
        [RelayCommand]
        private async Task GetCourseDetails(Course course)
        {
            if (course == null) return;

            await Shell.Current.GoToAsync($"{nameof(CourseDetailsView)}", true, new Dictionary<string, object>
            {
                { "course", course }
            });
        }

        [RelayCommand]
        private async Task GoBack()
        {
            try
            {
                await Shell.Current.GoToAsync("//AcademicTermListView", new Dictionary<string, object>
                {
                    ["term"] = NewTerm
                });
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlertAsync("Navigation Error", $"Navigation back failed: {ex.Message}", "OK");
            }
        }

        // Course Management
        [RelayCommand]
        private void AddCourse()
        {
            IsAddingCourse = true;
        }

        [RelayCommand]
        private void CancelAddCourse()
        {
            IsAddingCourse = false;
            ClearForm();
        }

        [RelayCommand]
        private async Task SaveNewCourse()
        {
            if (!ValidateNewCourse()) return;

            try
            {
                NewCourse.Name = NewCourseName;
                NewCourse.StartDate = NewCourseStartDate;
                NewCourse.EndDate = NewCourseEndDate;
                NewCourse.Status = NewCourseStatus;
                NewCourse.TermId = NewTerm.Id;

                await _database.SaveCourseAsync(NewCourse);

                ClearForm();
                IsAddingCourse = false;

                await Shell.Current.DisplayAlertAsync("Success", "Course added successfully!", "OK");
                await LoadCoursesAsync(NewTerm);
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlertAsync("Error", $"Failed to add course: {ex.Message}", "OK");
                System.Diagnostics.Debug.WriteLine($"Add course error: {ex}");
            }
        }

        // Helper Methods
        private bool ValidateNewCourse()
        {
            if (string.IsNullOrWhiteSpace(NewCourseName))
            {
                _ = Shell.Current.DisplayAlertAsync("Error", "Please enter a course name", "OK");
                return false;
            }

            if (NewCourseEndDate <= NewCourseStartDate)
            {
                _ = Shell.Current.DisplayAlertAsync("Error", "End date must be after start date", "OK");
                return false;
            }

            return true;
        }

        private void ClearForm()
        {
            NewCourseName = string.Empty;
            NewCourseStartDate = DateTime.Now;
            NewCourseEndDate = DateTime.Now.AddMonths(6);
            NewCourseStatus = CourseStatus.Planned;
        }
         
        [RelayCommand]
        private void RemoveCourse(Course newCourse)
        {
            IsRemovingCourse = true;
            _database.DeleteCourseAsync(newCourse);
        }
    }
}