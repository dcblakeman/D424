using C_971.ViewModels;

namespace C_971.Views;

public partial class AddNoteView : ContentPage
{
    public AddNoteView(AddNoteViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}