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
}