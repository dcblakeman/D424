namespace C_971.Views;
using C_971.Models;
using C_971.Services;
using C_971.ViewModels; 

public partial class LoginView : ContentPage
{
	public LoginView(LoginViewModel viewModel)
	{
        InitializeComponent();
        BindingContext = viewModel;
    }

    private void OnButtonPressed(object sender, EventArgs e)
    {
        if (sender is Button button)
        {
            button.BackgroundColor = Color.FromArgb("#808080");
        }
    }

    private void OnButtonReleased(object sender, EventArgs e)
    {
        if (sender is Button button)
        {
            button.BackgroundColor = Color.FromArgb("#1976D2");
        }
    }
}