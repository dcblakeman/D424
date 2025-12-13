using C_971.Models;
using C_971.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace C_971.ViewModels
{
    [QueryProperty(nameof(Course), "course")]
    public partial class CourseInstructorViewModel : ObservableObject
    {
        [ObservableProperty]
        public string name = "Course Instructor";

        private DatabaseService _databaseService;

        [ObservableProperty]
        private bool _isLoading;

        [ObservableProperty]
        private bool isRefreshing;

        [ObservableProperty]
        private bool isAddingCourseInstructor;

        [ObservableProperty]
        private bool isRemovingCourseInstructor;

        [ObservableProperty]
        private int instructorId;

        [ObservableProperty]
        private string instructorName;

        [ObservableProperty]
        private string instructorPhone;

        [ObservableProperty]
        private string instructorEmail;

        public bool IsNotAddingCourseInstructor => !IsAddingCourseInstructor && !IsRemovingCourseInstructor;

        [ObservableProperty]
        public Course course;

        public CourseInstructorViewModel(DatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        [RelayCommand]
        private async Task AddCourseInstructor()
        {
            if (string.IsNullOrWhiteSpace(InstructorName) ||
                string.IsNullOrWhiteSpace(InstructorPhone) ||
                string.IsNullOrWhiteSpace(InstructorEmail))
            {
                await Shell.Current.DisplayAlertAsync("Input Error", "All fields are required.", "OK");
                return;
            }
            IsAddingCourseInstructor = true;

            //Generate the next available ID
            InstructorId = await _databaseService.GetNextCourseInstructorIdAsync();

            var newInstructor = new CourseInstructor
            {
                Id = InstructorId,
                Name = InstructorName,
                Phone = InstructorPhone,
                Email = InstructorEmail
            };
            try
            {
                await _databaseService.AddCourseInstructorAsync(newInstructor);
                await Shell.Current.DisplayAlertAsync("Success", "Course instructor added successfully.", "OK");
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlertAsync("Error", $"Failed to add course instructor: {ex.Message}", "OK");
            }
            finally
            {
                IsAddingCourseInstructor = false;
            }
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
