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
        private ObservableCollection<CourseNote> notes = [];

        [ObservableProperty]
        private Course course;

        [ObservableProperty]
        private string name = "Course Notes";

        [ObservableProperty]
        private CourseNote note;

        public ViewNotesViewModel(DatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        [RelayCommand]
        public async Task LoadNotesAsync()
        {
            if (Course == null)
            {
                Debug.WriteLine("Course is null in LoadNotesAsync");
                return;
            }
            _isLoading = true;
            Notes.Clear();
            var notesFromDb = await _databaseService.GetCourseNotesByCourseIdAsync(Course.Id);
            foreach (var note in notesFromDb)
            {
                Notes.Add((CourseNote)note);
            }
            _isLoading = false;
        }
    }
}