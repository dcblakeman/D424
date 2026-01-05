using C_971.ViewModels;

namespace C_971.Views;

public partial class AssessmentSelectionView : ContentPage
{
	public AssessmentSelectionView(AssessmentSelectionViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
    }

    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);

        if (BindingContext is AssessmentSelectionViewModel viewModel)
        {
            viewModel.NewCourse = viewModel.Course;
            viewModel.NewUser = viewModel.User;
            viewModel.NewTerm = viewModel.Term;
            await Shell.Current.DisplayAlertAsync("User Info", $"Logged in as: {viewModel.NewTerm}", "OK");
        }
    }
}