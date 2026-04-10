using C_971.ViewModels;

namespace C_971.Views;

public partial class ViewNotesView : ContentPage
{
    public ViewNotesView(ViewNotesViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}

