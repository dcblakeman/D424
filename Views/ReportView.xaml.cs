using C_971.ViewModels;

namespace C_971.Views;

public partial class ReportView : ContentPage
{
	public ReportView(ReportViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
    }

    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);

        if (BindingContext is ReportViewModel viewModel)
        {
            viewModel.NewCourse = viewModel.Course;
            viewModel.NewUser = viewModel.User;
            viewModel.NewTerm = viewModel.Term;
        }
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is ReportViewModel viewModel)
        {
            await viewModel.OnAppearingAsync();
        }
    }

}