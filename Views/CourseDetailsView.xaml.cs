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
}