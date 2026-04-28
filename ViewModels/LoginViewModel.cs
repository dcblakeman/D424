using C_971.Models;
using C_971.Services;
using C_971.Utilities;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace C_971.ViewModels;

[QueryProperty(nameof(User), "user")]
public partial class LoginViewModel : ObservableObject
{
    private readonly DatabaseService _database;

    [ObservableProperty]
    private string viewName = "Login";

    [ObservableProperty]
    private User user = new();

    [ObservableProperty]
    private User newUser = null!;

    [ObservableProperty]
    private string newRegisterUserEmail = string.Empty;

    [ObservableProperty]
    private string newRegisterUserPassword = string.Empty;

    [ObservableProperty]
    private string newLoginUserEmail = string.Empty;

    [ObservableProperty]
    private string newLoginUserPassword = string.Empty;

    public LoginViewModel(DatabaseService database)
    {
        _database = database;
    }

    internal async Task<int> GetUserIdByEmailAsync(string email)
    {
        int userId = await _database.GetUserIdByEmailAsync(email);
        return userId;
    }

    [RelayCommand]
    public async Task LoginUser()
    {
        try
        {
            // Verify that email is not empty
            if (string.IsNullOrWhiteSpace(NewLoginUserEmail))
            {
                await Shell.Current.DisplayAlertAsync("Error", "Please enter your email.", "OK");
                return;
            }

            //Validate email format using regex
            if (!ValidationRules.IsValidEmail(NewLoginUserEmail))
            {
                await Shell.Current.DisplayAlertAsync("Error", "Please enter a valid email address.", "OK");
                return;
            }

            //Verify that password is not empty
            if (string.IsNullOrWhiteSpace(NewLoginUserPassword))
            {
                await Shell.Current.DisplayAlertAsync("Error", "Please enter your password.", "OK");
                return;
            }
        }
        catch
        {
            await Shell.Current.DisplayAlertAsync("Error", "Please enter your email.", "OK");
            return;
        }

        

        bool isAuthenticated = await AuthenticateAsync(NewLoginUserEmail, NewLoginUserPassword);

        if (isAuthenticated)
        {
            User? authenticatedUser = await _database.GetUserByEmailAsync(NewLoginUserEmail);
            if (authenticatedUser is null)
            {
                await Shell.Current.DisplayAlertAsync("Error", "Your account could not be loaded.", "OK");
                return;
            }

            NewUser = authenticatedUser;

            // Pass the dictionary directly as an argument to GoToAsync
            await Shell.Current.GoToAsync("AcademicTermListView", true, new Dictionary<string, object>
            {
                ["user"] = authenticatedUser
            });
        }
        else
        {
            await Shell.Current.DisplayAlertAsync("Error", "Invalid email or password. Please try again.", "OK");
        }
    }

    [RelayCommand]
    public async Task RegisterUser()
    {
        try
        {
            // Verify that the email is not empty
            if (string.IsNullOrWhiteSpace(NewRegisterUserEmail))
            {
                await Shell.Current.DisplayAlertAsync("Error", "Please enter an email address.", "OK");
                return;
            }

            // Validate email format using regex
            if (!ValidationRules.IsValidEmail(NewRegisterUserEmail))
            {
                await Shell.Current.DisplayAlertAsync("Error", "Please enter a valid email address.", "OK");
                return;
            }

            //Verify that password is not empty
            if (string.IsNullOrWhiteSpace(NewRegisterUserPassword))
            {
                await Shell.Current.DisplayAlertAsync("Error", "Please enter a password.", "OK");
                return;
            }
        }
        catch
        {
            await Shell.Current.DisplayAlertAsync("Error", "Please enter an email address.", "OK");
        }

        bool isRegistered = await IsEmailRegisteredAsync(NewRegisterUserEmail);

        if (isRegistered)
        {
            await Shell.Current.DisplayAlertAsync("Error", "Email is already registered. Please use a different email.", "OK");
            return;
        }

        //Register user in database
        _ = await _database.CreateUserAsync(NewRegisterUserEmail, NewRegisterUserPassword);

        await Shell.Current.DisplayAlertAsync("Success", "Registration successful. You can now log in.", "OK");
    }

    internal async Task<bool> IsEmailRegisteredAsync(string email)
    {
        bool isRegistered = await _database.IsEmailRegisteredAsync(email);
        return isRegistered;
    }

    internal async Task<bool> AuthenticateAsync(string email, string password)
    {
        bool isAuthenticated = await _database.AuthenticateUserAsync(email, password);
        return isAuthenticated;
    }

    [RelayCommand]
    public async Task Exit()
    {
        bool confirmed = await Shell.Current.DisplayAlertAsync(
            "Exit Application",
            "Are you sure you want to exit the application?",
            "Yes",
            "No");

        if (confirmed)
        {
            Application.Current?.Quit();
        }
    }

}
