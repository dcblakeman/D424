using C_971.Models;
using C_971.Services;
using C_971.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
namespace C_971.ViewModels
{
    [QueryProperty(nameof(Course), "course")]
    public partial class AddNoteViewModel : ObservableObject
    {
        private DatabaseService _database;

        [ObservableProperty]
        private int newNoteId;

        [ObservableProperty]
        private CourseNote newNote;

        [ObservableProperty]
        private string newNoteContent = string.Empty;

        [ObservableProperty]
        private Course course;

        [ObservableProperty]
        private DateTime createdDate = DateTime.Now;

        [ObservableProperty]
        private string name = "Add Note";

        public AddNoteViewModel(DatabaseService database)
        {
            _database = database;  
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
            if (string.IsNullOrWhiteSpace(NewNoteContent))
            {
                await Shell.Current.DisplayAlertAsync("Error", "Please enter a note", "OK");
                return;
            }

            if (Course == null)
            {
                await Shell.Current.DisplayAlertAsync("Error", "Course not found", "OK");
                return;
            }

            //Generate new note Id
            NewNoteId = await _database.GetNextCourseNoteIdAsync();


            // Save to database
            NewNote = new CourseNote
            {
                Id = NewNoteId,
                CreatedDate = DateTime.Now,
                NoteContent = NewNoteContent,
                CourseId = Course.Id
            };

            //Add new note to Course Notes list by Id
            if(NewNote == null) 
            {
                await Shell.Current.DisplayAlertAsync("Error", "Note creation failed", "OK");
                return;
            } else
            {
                try
                {
                    await _database.SaveCourseNoteAsync(NewNote);
                } catch (Exception ex)
                {
                    await Shell.Current.DisplayAlertAsync("Error", $"Failed to save note: {ex.Message}", "OK");
                    return;
                }

                await Shell.Current.DisplayAlertAsync("Success", "Note saved!", "OK");
            }

            await GoBack();
        }

        [RelayCommand]
        private async Task GoBack()
        {
            try
            {
                // Use relative navigation (no leading slash)
                await Shell.Current.GoToAsync("CourseDetailsView", new Dictionary<string, object>
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