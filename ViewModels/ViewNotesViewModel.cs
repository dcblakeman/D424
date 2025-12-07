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
        private readonly DatabaseService _databaseService;
        private bool _isLoading;

        [ObservableProperty]
        public Course course;

        public ObservableCollection<CourseNote> CourseNotesList { get; private set; } = new ObservableCollection<CourseNote>();

        [ObservableProperty]
        private string name = "Course Notes";

        [ObservableProperty]
        private CourseNote note;

        public ViewNotesViewModel(DatabaseService databaseService)
        {
            _databaseService = databaseService;
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
            if (Course?.Id > 0)
            {
                var notes = await _databaseService.GetCourseNotesByCourseIdAsync(Course.Id);
                CourseNotesList.Clear();

                // Add all notes at once
                foreach (var note in notes)
                    CourseNotesList.Add(note);
            }
        }
    }
}