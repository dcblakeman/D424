
using C_971.Models;
using C_971.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace C_971.ViewModels
{
    [QueryProperty(nameof(Term), "term")]
    [QueryProperty(nameof(User), "user")]
    public partial class CourseListViewModel : ObservableObject
    {
        private readonly DatabaseService _database;

        [ObservableProperty]
        private bool isPopulated = false;

        [ObservableProperty]
        private string viewName = "Course List";

        // Core Properties
        [ObservableProperty]
        private AcademicTerm term = null!;

        [ObservableProperty]
        private AcademicTerm newTerm = null!;

        [ObservableProperty]
        private User user = new();

        [ObservableProperty]
        private User newUser = null!;

        [ObservableProperty]
        private Course course = null!;

        [ObservableProperty]
        private Course newCourse = new();

        // New Course Form
        [ObservableProperty]
        private string newCourseName = string.Empty;

        [ObservableProperty]
        private string newCourseDescription = string.Empty;

        [ObservableProperty]
        private DateTime newCourseStartDate = DateTime.Now;

        [ObservableProperty]
        private DateTime newCourseEndDate = DateTime.Now.AddMonths(6);

        [ObservableProperty]
        private CourseStatus newCourseStatus = CourseStatus.Planned;

        [ObservableProperty]
        private FinalGrade newCourseGrade = FinalGrade.NotGraded;

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
        private ObservableCollection<Course> courses = [];

        private List<Course> _allCourses = [];

        // Static Collections
        public ObservableCollection<CourseStatus> StatusOptions { get; } =
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

        public CourseListViewModel(DatabaseService database)
        {
            _database = database;
            _ = LoadCoursesAsync(Term);
        }

        // Property Change Handlers
        partial void OnTermChanged(AcademicTerm value)
        {
            NewTerm = value;
            _ = LoadCoursesAsync(value);
        }

        partial void OnUserChanged(User value)
        {
            NewUser = value;
        }

        partial void OnCourseChanged(Course value)
        {
            NewCourse = value;
        }

        partial void OnSearchTextChanged(string value)
        {
            ApplySearchFilter();
        }

        partial void OnIsAddingCourseChanged(bool value)
        {
            OnPropertyChanged(nameof(IsNotAddingCourse));
        }

        // Initialization
        public async Task OnAppearingAsync()
        {
            if (Courses.Count == 0)
            {
                await LoadCoursesAsync(NewTerm);
            }
        }

        // Search and Filter
        [RelayCommand]
        private void Search()
        {
            ApplySearchFilter();
        }

        private void ApplySearchFilter()
        {
            IEnumerable<Course> filteredCourses = string.IsNullOrWhiteSpace(SearchText)
                ? _allCourses
                : _allCourses.Where(c => c.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase));

            Courses = new ObservableCollection<Course>(filteredCourses);
        }

        // Data Loading
        [RelayCommand]
        public async Task LoadCoursesAsync(AcademicTerm term)
        {
            //Courses.Clear();
            if (term == null)
            {
                return;
            }

            if (term != null)
            {
                _allCourses = await _database.GetCoursesByTermIdAsync(term.Id);
                ApplySearchFilter();
            }

            IsPopulated = true;

        }

        [RelayCommand]
        private async Task GoBack()
        {
            try
            {
                await Shell.Current.GoToAsync("..", true, new Dictionary<string, object>
                {
                    ["term"] = NewTerm,
                    ["user"] = NewUser,
                    ["course"] = NewCourse
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
            if (!ValidateNewCourse())
            {
                return;
            }

            try
            {
                if (NewTerm == null)
                {
                    await Shell.Current.DisplayAlertAsync("Error", "No academic term is selected for this course.", "OK");
                    return;
                }

                if (NewUser == null)
                {
                    await Shell.Current.DisplayAlertAsync("Error", "No user is available for this course.", "OK");
                    return;
                }

                Course courseToSave = new()
                {
                    Id = 0,
                    Name = NewCourseName.Trim(),
                    Description = NewCourseDescription?.Trim() ?? string.Empty,
                    StartDate = NewCourseStartDate,
                    EndDate = NewCourseEndDate,
                    Status = NewCourseStatus,
                    Grade = NewCourseGrade,
                    TermId = NewTerm.Id
                };

                // Insert Course Into Database
                _ = await _database.SaveCourseAsync(courseToSave);
                NewCourse = courseToSave;

                // **ADD THIS: Auto-enroll the current user in the new course**
                UserCourse userCourse = new()
                {
                    UserId = NewUser.Id,
                    CourseId = courseToSave.Id
                };
                await _database.SaveUserCourseAsync(userCourse);

                SearchText = string.Empty;
                await LoadCoursesAsync(NewTerm);
                await Shell.Current.DisplayAlertAsync("Success", "Course added successfully!", "OK");

                ClearForm();
                IsAddingCourse = false;
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
            NewCourseDescription = string.Empty;
            NewCourseGrade = FinalGrade.NotGraded;
            NewCourse = new Course();
        }

        [RelayCommand]
        public async Task RemoveCourse()
        {
            IsRemovingCourse = true;
        }

        [RelayCommand]
        public async Task CancelRemoveCourse()
        {
            IsRemovingCourse = false;
            await Task.CompletedTask;
        }
    }
}
