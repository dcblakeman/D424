using C_971.ViewModels;

namespace C_971.Views;

public partial class CourseDetailsView : ContentPage
{
	public CourseDetailsView(CourseDetailsViewModel	courseDetailsViewModel)
	{
		InitializeComponent();
		BindingContext = courseDetailsViewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is CourseDetailsViewModel viewModel)
        {
            await viewModel.OnAppearingAsync();
        }
    }
}