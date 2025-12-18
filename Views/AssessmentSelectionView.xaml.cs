using C_971.ViewModels;

namespace C_971.Views;

public partial class AssessmentSelectionView : ContentPage
{
	public AssessmentSelectionView(AssessmentSelectionViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
    }
}