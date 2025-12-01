using C_971.Models;
using C_971.ViewModels;

namespace C_971.Views;

public partial class AcademicTermListView : ContentPage
{
    public AcademicTermListView(AcademicTermListViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    private async void OnTermTapped(object sender, TappedEventArgs e)
    {
        var border = (Border)sender;
        var term = (AcademicTerm)border.BindingContext;
        var viewModel = (AcademicTermListViewModel)BindingContext;

        // Visual feedback animation
        await border.ScaleToAsync(0.95, 50);
        border.BackgroundColor = Color.FromArgb("#E3F2FD");
        await Task.Delay(100);
        await border.ScaleToAsync(1, 50);
        border.BackgroundColor = Colors.White;

        // Check if we're in remove mode
        if (viewModel.IsRemovingTerm)
        {
            // Show confirmation dialog
            bool confirm = await Shell.Current.DisplayAlertAsync(
                "Confirm Removal",
                $"Are you sure you want to remove '{term.Name}'?",
                "Yes",
                "No");

            if (confirm)
            {
                try
                {
                    // Delete from database
                    await viewModel.DeleteTermCommand.ExecuteAsync(term);

                    // Remove from UI
                    viewModel.AcademicTerms.Remove(term);
                    viewModel.IsRemovingTerm = false;

                    await Shell.Current.DisplayAlertAsync("Success", "Term removed successfully!", "OK");
                }
                catch (Exception ex)
                {
                    await Shell.Current.DisplayAlertAsync("Error", "Failed to remove term. Please try again.", "OK");
                }
            }
        }
        else
        {
            // Normal navigation behavior
            await Shell.Current.GoToAsync("CourseListView", new Dictionary<string, object>
        {
            { "term", term }
        });
        }
    }
}