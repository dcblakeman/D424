using C_971.ViewModels;

namespace C_971.Views;

public partial class CourseDetailsView : ContentPage
{
	public CourseDetailsView(CourseDetailsViewModel	courseDetailsViewModel)
	{
		InitializeComponent();
		BindingContext = courseDetailsViewModel;
    }
    protected override bool OnBackButtonPressed()
    {
        Shell.Current.GoToAsync("..");
        return true; // Prevents default back behavior
    }
    //protected override async void OnAppearing()
    //{
    //    base.OnAppearing();

    //    if (BindingContext is CourseDetailsViewModel viewModel)
    //    {
    //        // Check if this is a new course that needs instructor ID
    //        if (viewModel.Course.Id == 0 && viewModel.Course.InstructorId == 0)
    //        {
    //            await viewModel.GenerateInstructorIdAsync();
    //        }
    //    }
    //}
}