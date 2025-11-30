using C_971.Models;
using C_971.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace C_971.ViewModels
{
    [QueryProperty(nameof(Course), "course")]
    public partial class ViewNotesViewModel : BaseViewModel
    {
        private readonly CourseService courseService;

        [ObservableProperty]
        private Course course;

        [ObservableProperty]
        private ObservableCollection<CourseNote> notes = new ObservableCollection<CourseNote>();

        public ViewNotesViewModel(CourseService courseService)
        {
            this.courseService = courseService;
            Name = "View Notes";
        }

        // This gets called automatically when Course is set via QueryProperty
        partial void OnCourseChanged(Course value)
        {
            if (value != null)
            {
                LoadNotes();
            }
        }

        private void LoadNotes()
        {
            Notes.Clear();

            if (Course?.Notes != null)
            {
                foreach (var note in Course.Notes)
                {
                    Notes.Add(note);
                }
                System.Diagnostics.Debug.WriteLine($"Loaded {Notes.Count} notes for course {Course.Name}");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"No notes found for course {Course?.Name}");
            }
        }
    }
}