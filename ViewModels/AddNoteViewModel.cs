using C_971.Models;
using C_971.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace C_971.ViewModels
{
    [QueryProperty(nameof(NewTerm), "term")]
    [QueryProperty(nameof(NewCourse), "course")]
    [QueryProperty(nameof(NewUserId), "userid")]
    public partial class AddNoteViewModel : ObservableObject
    {
        private readonly DatabaseService _database;

        [ObservableProperty]
        private int newUserId;

        [ObservableProperty]
        private AcademicTerm newTerm;

        // Core Properties
        [ObservableProperty]
        private Course newCourse;

        [ObservableProperty]
        private CourseNote newNote = new CourseNote();

        [ObservableProperty]
        private string viewName = "Add Note";

        // Form Properties
        [ObservableProperty]
        private string newNoteContent = string.Empty;

        [ObservableProperty]
        private DateTime newCreatedDate = DateTime.Now;

        public AddNoteViewModel(DatabaseService database)
        {
            _database = database;
        }

        // Commands
        [RelayCommand]
        private async Task SaveNote()
        {
            if (!ValidateNote()) return;

            try
            {
                NewNote.CreatedDate = DateTime.Now;
                NewNote.NoteContent = NewNoteContent.Trim();
                NewNote.CourseId = NewCourse.Id;

                await _database.SaveCourseNoteAsync(NewNote);

                await Shell.Current.DisplayAlertAsync("Success", "Note saved successfully!", "OK");
                await GoBack();
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlertAsync("Error", $"Failed to save note: {ex.Message}", "OK");
                System.Diagnostics.Debug.WriteLine($"Save note error: {ex}");
            }
        }

        [RelayCommand]
        private async Task Cancel()
        {
            bool hasContent = !string.IsNullOrWhiteSpace(NewNoteContent);

            if (hasContent)
            {
                bool confirmed = await Shell.Current.DisplayAlertAsync(
                    "Discard Changes",
                    "Are you sure you want to discard this note?",
                    "Discard",
                    "Continue Editing");

                if (!confirmed) return;
            }

            await GoBack();
        }

        [RelayCommand]
        private async Task GoBack()
        {
            try
            {
                // Navigate back to previous page with course context
                await Shell.Current.GoToAsync("CourseDetailsView", true, new Dictionary<string, object>
                {
                    ["course"] = NewCourse,
                    ["term"] = NewTerm,
                    ["userid"] = NewUserId
                });
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlertAsync("Navigation Error", $"Navigation failed: {ex.Message}", "OK");
                System.Diagnostics.Debug.WriteLine($"Navigation error: {ex}");
            }
        }

        // Helper Methods
        private bool ValidateNote()
        {
            if (string.IsNullOrWhiteSpace(NewNoteContent))
            {
                _ = Shell.Current.DisplayAlertAsync("Error", "Please enter note content", "OK");
                return false;
            }

            if (NewCourse == null)
            {
                _ = Shell.Current.DisplayAlertAsync("Error", "Course information not found", "OK");
                return false;
            }

            if (NewNoteContent.Trim().Length < 3)
            {
                _ = Shell.Current.DisplayAlertAsync("Error", "Note must be at least 3 characters long", "OK");
                return false;
            }

            return true;
        }
    }
}