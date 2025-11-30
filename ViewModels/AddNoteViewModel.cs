using C_971.Models;
using C_971.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace C_971.ViewModels
{
    [QueryProperty(nameof(Course), "course")]
    public partial class AddNoteViewModel : BaseViewModel
    {
        private readonly CourseService courseService;

        [ObservableProperty]
        private Course course;

        [ObservableProperty]
        private string noteContent = string.Empty;

        [ObservableProperty]
        private DateTime createdDate = DateTime.Now;

        public AddNoteViewModel(CourseService courseService)
        {
            this.courseService = courseService;
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

            var newNote = new CourseNote
            {
                Id = (Course.Notes?.Count ?? 0) + 1,
                NoteContent = NoteContent,
                CreatedDate = CreatedDate,
                CourseId = Course.Id,
                Course = Course
            };

            if (Course.Notes == null)
            {
                Course.Notes = new ObservableCollection<CourseNote>();
            }

            Course.Notes.Add(newNote);

            System.Diagnostics.Debug.WriteLine($"Note added. Total notes for course {Course.Name}: {Course.Notes.Count}");

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