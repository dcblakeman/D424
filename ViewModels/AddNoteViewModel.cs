
using C_971.Models;
using C_971.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace C_971.ViewModels
{
    [QueryProperty(nameof(Term), "term")]
    [QueryProperty(nameof(Course), "course")]
    [QueryProperty(nameof(User), "user")]
    public partial class AddNoteViewModel : ObservableObject
    {
        private readonly DatabaseService _database;

        [ObservableProperty]
        private User user = new();

        [ObservableProperty]
        private User newUser = null!;

        [ObservableProperty]
        private int newUserId;

        [ObservableProperty]
        private AcademicTerm term = null!;

        [ObservableProperty]
        private AcademicTerm newTerm = null!;

        // Core Properties
        [ObservableProperty]
        private Course course = null!;

        [ObservableProperty]
        private Course newCourse = null!;

        [ObservableProperty]
        private CourseNote newNote = new();

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

        partial void OnUserChanged(User value)
        {
            NewUser = value;
        }

        partial void OnCourseChanged(Course value)
        {
            NewCourse = value;
        }

        partial void OnTermChanged(AcademicTerm value)
        {
            NewTerm = value;
        }

        // Commands
        [RelayCommand]
        private async Task SaveNote()
        {
            if (!ValidateNote())
            {
                return;
            }

            try
            {
                NewNote.Id = 0;
                NewNote.CreatedDate = DateTime.Now;
                NewNote.NoteContent = NewNoteContent.Trim();
                NewNote.CourseId = NewCourse.Id;

                _ = await _database.SaveCourseNoteAsync(NewNote);

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

                if (!confirmed)
                {
                    return;
                }
            }

            await GoBack();
        }

        [RelayCommand]
        private async Task GoBack()
        {
            try
            {
                await Shell.Current.GoToAsync("..", true, new Dictionary<string, object>
                {
                    ["course"] = NewCourse,
                    ["term"] = NewTerm,
                    ["user"] = NewUser
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
