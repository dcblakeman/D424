using C_971.ViewModels;

namespace C_971.Views;

public partial class AssessmentsView : ContentPage
{
	public AssessmentsView(AssessmentsViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}