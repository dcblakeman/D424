namespace C_971.Views;
using C_971.ViewModels;

public partial class CourseInstructorView : ContentPage
{
	public CourseInstructorView(CourseInstructorViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}