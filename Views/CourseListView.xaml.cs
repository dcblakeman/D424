using C_971.Models;
using C_971.ViewModels;
using Microsoft.Maui.Controls;

namespace C_971.Views
{
    public partial class CourseListView : ContentPage
    {
        public CourseListView(CourseListViewModel courseListViewModel)
        {
            InitializeComponent();
            BindingContext = courseListViewModel;
        }

        protected override async void OnNavigatedTo(NavigatedToEventArgs args)
        {
            base.OnNavigatedTo(args);

            if (BindingContext is CourseListViewModel viewModel)
            {
                viewModel.NewUser = viewModel.User;
                viewModel.NewTerm = viewModel.Term;
                viewModel.NewCourse = viewModel.Course;
                await viewModel.LoadCoursesAsync(viewModel.NewTerm);
            }
        }

        private async void OnCourseTapped(object sender, EventArgs e)
        {
            var border = (Border)sender;
            var newCourse = (Course)border.BindingContext;
            var viewModel = (CourseListViewModel)BindingContext;

            // Visual feedback animation
            await border.ScaleToAsync(0.95, 50);
            border.BackgroundColor = Color.FromArgb("#E3F2FD");
            await Task.Delay(100);
            await border.ScaleToAsync(1, 50);
            border.BackgroundColor = Colors.White;

            // Check if we're in remove mode
            if (viewModel.IsRemovingCourse)
            {
                // Show confirmation dialog
                bool confirm = await Shell.Current.DisplayAlertAsync(
                    "Confirm Removal",
                    $"Are you sure you want to remove '{newCourse.Name}'?",
                    "Yes",
                    "No");

                if (confirm)
                {
                    // Remove from collection
                    viewModel.Courses.Remove(newCourse);
                    viewModel.RemoveCourseCommand.Execute(newCourse);

                    // Exit remove mode
                    viewModel.IsRemovingCourse = false;

                    await Shell.Current.DisplayAlertAsync("Success", "Course removed successfully!", "OK");
                }
            }
            else
            {
                // Normal navigation behavior
                await Shell.Current.GoToAsync("CourseDetailsView", new Dictionary<string, object>
                {
                    ["user"] = viewModel.NewUser,
                    ["course"] = viewModel.NewCourse = newCourse,
                    ["term"] = viewModel.NewTerm
                });
            }
        }
    }
}
