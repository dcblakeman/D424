using C_971.ViewModels;

namespace C_971.Views;

public partial class ObjectiveAssessmentView : ContentPage
{
	public ObjectiveAssessmentView(ObjectiveAssessmentViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
    }
}