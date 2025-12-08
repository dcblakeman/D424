using C_971.Models;
using C_971.Services;
using C_971.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace C_971.ViewModels
{
    [QueryProperty(nameof(Term), "term")]
    [QueryProperty(nameof(TermId), "termId")]
    [QueryProperty(nameof(Course), "course")]
    public partial class CourseListViewModel : ObservableObject
    {
        public ObservableCollection<Course> Courses { get; private set; } = [];

        private readonly DatabaseService _databaseService;

        [ObservableProperty]
        private AcademicTerm term;

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
        private string name = "                            Courses";

        [ObservableProperty]
        private bool isRefreshing;

        [ObservableProperty]
        private string emptyStateMessage = string.Empty;

        [ObservableProperty]
        private int termId;

        [ObservableProperty]
        private Course course;

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

        partial void OnTermChanged(AcademicTerm value)
        {
            if (value != null)
            {
                TermId = value.Id; // NOW Term is available!
                _ = LoadCoursesAsync();
            }
        }

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
                IsRefreshing = true;

                // Get the term ID from either source
                int termId = Course?.TermId ?? TermId;

                // Load courses for the term
                var courses = await _databaseService.GetCoursesByTermAsync(termId);

                // Update the collection
                Courses.Clear();
                foreach (var course in courses)
                {
                    Courses.Add(course);
                }
            }
            catch (Exception ex)
            {
                // Handle errors
                await Shell.Current.DisplayAlertAsync("Error", $"Failed to load courses: {ex.Message}", "OK");
            }
            finally
            {
                // Always reset loading states at the END
                IsLoading = false;
                IsRefreshing = false;
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
                    TermId = Term.Id
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

        [RelayCommand]
        private async Task GoBack()
        {
            try
            {
                // Use relative navigation (no leading slash)
                await Shell.Current.GoToAsync("//AcademicTermListView", new Dictionary<string, object>
                {
                    ["termId"] = TermId
                });
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlertAsync("Navigation Error", $"Navigation back failed: {ex.Message}", "OK");
            }
        }
    }
}