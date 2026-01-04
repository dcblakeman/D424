using C_971.Models;
using C_971.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;

namespace C_971.ViewModels
{
    [QueryProperty(nameof(Course), "course")]
    [QueryProperty(nameof(UserId), "userid")]
    [QueryProperty(nameof(Term), "term")]
    public partial class ViewNotesViewModel : ObservableObject
    {
        private readonly DatabaseService _database;
        private bool _isLoading;

        [ObservableProperty]
        private bool isRefreshing;

        [ObservableProperty]
        public Course course;

        [ObservableProperty]
        public Course newCourse;

        [ObservableProperty]
        public int userId;

        [ObservableProperty]
        public int newUserId;

        [ObservableProperty]
        public AcademicTerm term;

        [ObservableProperty]
        public AcademicTerm newTerm;

        public ObservableCollection<CourseNote> CourseNotesList { get; private set; } = [];

		// Backing field for all notes
        [ObservableProperty]
		private List<CourseNote> notes = [];

		[ObservableProperty]
        private string viewName = "View Notes";

        [ObservableProperty]
        private CourseNote newNote;

        public ViewNotesViewModel(DatabaseService databaseService)
        {
            _database = databaseService;
        }

        partial void OnUserIdChanged(int value)
        {
            NewUserId = value;
        }
        partial void OnTermChanged(AcademicTerm value)
        {
            NewTerm = value;
        }

        partial void OnCourseChanged(Course value)
        {
            NewCourse = value;
            _ = LoadCourseNotes();
        }

        [RelayCommand]
        private async Task LoadCourseNotes()
        {
            if (NewCourse?.Id <= 0) return;

            IsRefreshing = true;
            {
                Notes = (List<CourseNote>)await _database.GetCourseNotesByCourseIdAsync(NewCourse.Id);

				// Add all notes at once
				CourseNotesList = new ObservableCollection<CourseNote>(Notes);
				OnPropertyChanged(nameof(CourseNotesList));
			}
        }
        [RelayCommand]
        private async Task GoBack()
        {
            try
            {
                // Use relative navigation (no leading slash)
                await Shell.Current.GoToAsync("CourseDetailsView", true, new Dictionary<string, object>
                {
                    ["course"] = NewCourse,
                    ["userid"] = NewUserId,
                    ["term"] = NewTerm
                });
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlertAsync("Navigation Error", $"Navigation back failed: {ex.Message}", "OK");
            }
        }
    }
}