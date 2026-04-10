using C_971.Models;
using C_971.ViewModels;

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

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            if (BindingContext is CourseListViewModel viewModel)
            {
                await viewModel.LoadCoursesCommand.ExecuteAsync(null);
            }
        }

        private async void OnCourseTapped(object sender, EventArgs e)
        {
            CourseListViewModel viewModel = (CourseListViewModel)BindingContext;
            Border border = (Border)sender;
            Course newCourse = (Course)border.BindingContext;
            //CourseListViewModel viewModel = (CourseListViewModel)BindingContext;

            // Visual feedback animation
            _ = await border.ScaleToAsync(0.95, 50);
            border.BackgroundColor = Color.FromArgb("#E3F2FD");
            await Task.Delay(100);
            _ = await border.ScaleToAsync(1, 50);
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
                    _ = viewModel.Courses.Remove(newCourse);
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
