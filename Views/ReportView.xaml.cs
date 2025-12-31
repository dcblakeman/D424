using C_971.ViewModels;

namespace C_971.Views;

public partial class ReportView : ContentPage
{
	public ReportView(ReportViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
    }
}