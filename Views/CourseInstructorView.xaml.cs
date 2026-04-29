
using C_971.Models;
using C_971.ViewModels;

namespace C_971.Views;

public partial class CourseInstructorView : ContentPage
{
    public CourseInstructorView(CourseInstructorViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is CourseInstructorViewModel viewModel)
        {
            await viewModel.LoadInstructorsCommand.ExecuteAsync(null);
        }
    }

    public async void OnInstructorTapped(object sender, EventArgs e)
    {
        Border border = (Border)sender;
        CourseInstructor instructor = (CourseInstructor)border.BindingContext;
        CourseInstructorViewModel viewModel = (CourseInstructorViewModel)BindingContext;

        // Check if we're in remove mode
        if (viewModel.IsRemovingInstructor)
        {
            // Use ViewModel's delete command instead of duplicating logic
            await viewModel.DeleteInstructorCommand.ExecuteAsync(instructor);
        }
        else
        {
            //Assign the selected instructor to the course
            viewModel.NewCourse.InstructorId = instructor.Id;

            //Update the course in the databaes with the instructorid
            viewModel.NewCourse = await viewModel.UpdateCourseAsync(viewModel.NewCourse);

            viewModel.NewTerm = viewModel.Term;
            viewModel.NewCourse = viewModel.Course;
            viewModel.NewUser = viewModel.User;

            await Shell.Current.GoToAsync("..", true, new Dictionary<string, object>
            {
                ["user"] = viewModel.NewUser,
                ["course"] = viewModel.NewCourse,
                ["instructor"] = instructor,
                ["term"] = viewModel.NewTerm
            });
        }
    }
}
