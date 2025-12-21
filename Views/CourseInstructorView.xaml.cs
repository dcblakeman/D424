namespace C_971.Views;

using C_971.Models;
using C_971.ViewModels;

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
        var border = (Border)sender;
        var instructor = (CourseInstructor)border.BindingContext;
        var viewModel = (CourseInstructorViewModel)BindingContext;

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

            // Navigate to detail view - only pass course since that's what CourseDetailsView expects
            await Shell.Current.GoToAsync(nameof(CourseDetailsView), new Dictionary<string, object>
            {
                ["course"] = viewModel.NewCourse
            });
        }
    }
}