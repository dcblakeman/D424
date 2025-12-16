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
    public partial class ViewNotesViewModel : ObservableObject
    {
        private readonly DatabaseService _database;
        private bool _isLoading;

        [ObservableProperty]
        private bool isRefreshing;

		[ObservableProperty]
        public Course course;

        public ObservableCollection<CourseNote> CourseNotesList { get; private set; } = [];

		// Backing field for all notes
        [ObservableProperty]
		private List<CourseNote> notes = [];

		[ObservableProperty]
        private string name = "Course Notes";

        [ObservableProperty]
        private CourseNote note;

        public ViewNotesViewModel(DatabaseService databaseService)
        {
            _database = databaseService;
        }

        partial void OnCourseChanged(Course value)
        {
            // Called automatically when Course property changes
            if (value?.Id > 0)
            {
                _ = LoadCourseNotes();
            }
        }

        [RelayCommand]
        private async Task LoadCourseNotes()
        {
            if (Course?.Id <= 0) return;

            IsRefreshing = true;
            {
                Notes = (List<CourseNote>)await _database.GetCourseNotesByCourseIdAsync(Course.Id);

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
                    ["course"] = Course
                });
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlertAsync("Navigation Error", $"Navigation back failed: {ex.Message}", "OK");
            }
        }
    }
}