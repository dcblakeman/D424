using C_971.ViewModels;

namespace C_971.Views;

public partial class AiAssistantView : ContentPage
{
    public AiAssistantView(AiAssistantViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is AiAssistantViewModel viewModel)
        {
            await viewModel.InitializeAsync();
        }
    }
}
