using C_971.ViewModels;

namespace C_971.Views;

public partial class PerformanceAssessmentView : ContentPage
{
	public PerformanceAssessmentView(PerformanceAssessmentViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}