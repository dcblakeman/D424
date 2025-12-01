using C_971.Models;
using C_971.Services;
using C_971.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace C_971.ViewModels
{
    [QueryProperty(nameof(AcademicTerm), "term")]
    public partial class CourseListViewModel : ObservableObject
    {
        public ObservableCollection<Course> Courses { get; private set; } = [];

        private readonly DatabaseService _databaseService;

        [ObservableProperty]
        private AcademicTerm academicTerm;

        [ObservableProperty]
        private bool isAddingCourse;

        [ObservableProperty]
        private bool isRemovingCourse;

        public bool IsNotAddingCourse => !IsAddingCourse && !IsRemovingCourse;

        [ObservableProperty]
        private string newCourseName = string.Empty;

        [ObservableProperty]
        private DateTime newCourseStartDate = DateTime.Now;

        [ObservableProperty]
        private DateTime newCourseEndDate = DateTime.Now.AddMonths(3);

        [ObservableProperty]
        private string newCourseStatus = "Planned";

        [ObservableProperty]
        private bool isLoading = false;

        [ObservableProperty]
        private string name = "                  Courses";

        [ObservableProperty]
        private bool isRefreshing;

        [ObservableProperty]
        private string emptyStateMessage = string.Empty;

        public ObservableCollection<string> StatusOptions { get; } = new ObservableCollection<string>
        {
            "Not Enrolled",
            "In Progress",
            "Completed",
            "Dropped",
            "Planned"
        };

        public CourseListViewModel(DatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        //private void GetCourseListAsync()
        //{
        //    GetCourseListAsync(_databaseService);
        //}

        // Called automatically when AcademicTerm property is set via QueryProperty
        //async Task OnAcademicTermChanged(AcademicTerm value)
        //{
        //    if (value != null)
        //    {
        //        await GetCourseListAsync();
        //    }
        //}

        //[RelayCommand]
        //public async Task GetCourseListAsync(DatabaseService _databaseService)
        //{
        //    if (IsLoading)
        //        return;

        //    try
        //    {
        //        IsLoading = true;
        //        Courses.Clear();

        //        // Filter courses by the academic term
        //        List<Task> courses;
        //        if (AcademicTerm != null)
        //        {
        //            // Get only courses for this specific term
        //            courses = await _databaseService.GetCoursesForTerm(AcademicTerm.Id);
        //            Name = $"{AcademicTerm.Name} - Courses";
        //        }
        //        else
        //        {
        //            // Get all courses if no term is selected
        //            await _databaseService.GetAllCourses();
        //            Name = "All Courses";
        //        }

        //        foreach (var course in courses)
        //        {
        //            Courses.Add(course);
        //        }

        //        // If no courses found, show helpful message
        //        if (!courses.Any())
        //        {
        //            emptyStateMessage = "No courses found. Tap 'Add Course' to get started.";
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        // Handle exceptions (e.g., log them)
        //        System.Diagnostics.Debug.WriteLine(ex);
        //        emptyStateMessage = "An error occurred while loading courses.";
        //        //Shell.Current.DisplayAlertAsync("Alert", "No courses added yet", ex.Message);
        //    }
        //    finally
        //    {
        //        IsLoading = false;
        //        IsRefreshing = false;
        //    }
        //}

        [RelayCommand]
        async Task GetCourseDetails(Course course)
        {
            if (course == null)
                return;

            // Navigate to the details page
            await Shell.Current.GoToAsync($"{nameof(CourseDetailsView)}", true, new Dictionary<string, object>
            {
                { "course", course }
            });
        }

        [RelayCommand]
        private async Task LoadCoursesAsync()
        {
            try
            {
                IsLoading = true;

                // Check if we have a current term
                if (AcademicTerm == null)
                {
                    System.Diagnostics.Debug.WriteLine("AcademicTerm is null!");
                    return;
                }

                // Use CurrentTerm.Id (instance property)
                var courses = await _databaseService.GetCoursesByTermAsync(AcademicTerm.Id);

                System.Diagnostics.Debug.WriteLine($"Loaded {courses.Count} courses for term: {AcademicTerm.Name}");

                Courses.Clear();
                foreach (var course in courses)
                {
                    Courses.Add(course);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to load courses: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        void AddCourse()
        {
            IsAddingCourse = true;
            OnPropertyChanged(nameof(IsNotAddingCourse));
        }

        [RelayCommand]
        void RemoveCourse()
        {
            IsRemovingCourse = true;
            OnPropertyChanged(nameof(IsNotAddingCourse));
        }

        [RelayCommand]
        void CancelRemoveCourse()
        {
            IsRemovingCourse = false;
            OnPropertyChanged(nameof(IsNotAddingCourse));
        }

        [RelayCommand]
        async Task SaveNewCourse()
        {
            if (string.IsNullOrWhiteSpace(NewCourseName))
            {
                await Shell.Current.DisplayAlertAsync("Error", "Please enter a course name", "OK");
                return;
            }

            if (NewCourseEndDate <= NewCourseStartDate)
            {
                await Shell.Current.DisplayAlertAsync("Error", "End date must be after start date", "OK");
                return;
            }

            // Parse the string to enum
            if (Enum.TryParse<CourseStatus>(NewCourseStatus.Replace(" ", ""), out var status))
            {
                // Generate the next ID
                int nextId = Courses.Any() ? Courses.Max(c => c.Id) + 1 : 1;

                // Create new course
                var newCourse = new Course
                {
                    Id = nextId,
                    Name = NewCourseName,
                    StartDate = NewCourseStartDate,
                    EndDate = NewCourseEndDate,
                    Status = status,
                    TermId = AcademicTerm.Id
                };

                //Add to database
                await _databaseService.AddCourse(newCourse);

                // Exit editing mode
                IsAddingCourse = false;
                OnPropertyChanged(nameof(IsNotAddingCourse));

                await Shell.Current.DisplayAlertAsync("Success", "Course added successfully!", "OK");

                // Refresh Screen
                await LoadCoursesAsync();




























































































            }
            else
            {
                await Shell.Current.DisplayAlertAsync("Error", "Invalid course status selected", "OK");
            }
        }

        [RelayCommand]
        void CancelAddCourse()
        {
            IsAddingCourse = false;
            OnPropertyChanged(nameof(IsNotAddingCourse));
        }

        partial void OnIsAddingCourseChanged(bool value)
        {
            OnPropertyChanged(nameof(IsNotAddingCourse));
        }

        partial void OnIsRemovingCourseChanged(bool value)
        {
            OnPropertyChanged(nameof(IsNotAddingCourse));
        }

        [RelayCommand]
        private async Task SaveCourseAsync(Course course)
        {
            try
            {
                await _databaseService.SaveCourseAsync(course);
                await LoadCoursesAsync(); // Refresh the list
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to save course: {ex.Message}");
            }
        }
    }
}