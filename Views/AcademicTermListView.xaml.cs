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

    private async void OnTermTapped(object sender, EventArgs e)
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
                // Remove from collection
                viewModel.AcademicTerms.Remove(term);

                // Exit remove mode
                viewModel.IsRemovingTerm = false;

                await Shell.Current.DisplayAlertAsync("Success", "Term removed successfully!", "OK");
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