using C_971.Models;
using C_971.Services;
using C_971.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
namespace C_971.ViewModels
{
    [QueryProperty(nameof(Course), "course")]
    public partial class AddNoteViewModel(DatabaseService databaseService) : ObservableObject
    {
        private readonly DatabaseService _database = databaseService;

        [ObservableProperty]
        private ObservableCollection<CourseNote> courseNote = [];

        [ObservableProperty]
        private Course course;

        [ObservableProperty]
        private string noteContent = string.Empty;

        [ObservableProperty]
        private DateTime createdDate = DateTime.Now;

        [RelayCommand]
        async Task LoadCourseNotes()
        {
            if (Course == null)
            {
                await Shell.Current.DisplayAlertAsync("Error", "Course not found", "OK");
                return;
            }

            var notes = await _database.GetCourseNotesByCourseIdAsync(Course.Id);
            CourseNote.Clear();
            foreach (var note in notes)
            {
                CourseNote.Add((CourseNote)note);
            }
        }

        [RelayCommand]
        async Task Clear()
        {
            NoteContent = string.Empty;
            CreatedDate = DateTime.Now;
        }

        [RelayCommand]
        async Task RefreshNotes()
        {
            await LoadCourseNotes();
        }

        [RelayCommand]
        async Task DeleteNote(CourseNote note)
        {
            if (note == null)
            {
                await Shell.Current.DisplayAlertAsync("Error", "Note not found", "OK");
                return;
            }
            await _database.DeleteCourseNoteAsync(note);
            CourseNote.Remove(note);
            await Shell.Current.DisplayAlertAsync("Success", "Note deleted", "OK");
        }

        [RelayCommand]
        async Task EditNote(CourseNote note)
        {
            if (note == null)
            {
                await Shell.Current.DisplayAlertAsync("Error", "Note not found", "OK");
                return;
            }
            // Navigate to edit note page with the selected note
            await Shell.Current.GoToAsync(nameof(ViewNotesView), true, new Dictionary<string, object>
            {
                { "note", note }
            });
        }

        async Task LoadNotesForCourse()
        {
            if (Course == null)
                return;
            var notes = await _database.GetCourseNotesByCourseIdAsync(Course.Id);
            CourseNote.Clear();
            foreach (var note in notes)
            {
                CourseNote.Add((CourseNote)note);
            }
        }
        partial void OnCourseChanged(Course value)
        {
            if (value != null)
            {
                System.Diagnostics.Debug.WriteLine($"Course received: {value.Name}");
            }
        }

        [RelayCommand]
        async Task SaveNote()
        {
            if (string.IsNullOrWhiteSpace(NoteContent))
            {
                await Shell.Current.DisplayAlertAsync("Error", "Please enter a note", "OK");
                return;
            }

            if (Course == null)
            {
                await Shell.Current.DisplayAlertAsync("Error", "Course not found", "OK");
                return;
            }

            _ = new CourseNote
            {
                NoteContent = NoteContent,
                CreatedDate = CreatedDate,
                CourseId = Course.Id,
            };

            System.Diagnostics.Debug.WriteLine($"Note added. Total notes for course {Course.Name}: {CourseNote.Count}");

            await Shell.Current.DisplayAlertAsync("Success", "Note saved!", "OK");

            NoteContent = string.Empty;
            CreatedDate = DateTime.Now;

            await Shell.Current.GoToAsync("..");
        }

        [RelayCommand]
        async Task Cancel()
        {
            NoteContent = string.Empty;
            CreatedDate = DateTime.Now;
            await Shell.Current.GoToAsync("..");
        }
    }
}