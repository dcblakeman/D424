namespace C_971.Views;

using C_971.Models;
using C_971.ViewModels;

public partial class CourseInstructorView : ContentPage
{
	public CourseInstructorView(CourseInstructorViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is CourseInstructorViewModel viewModel)
        {
            await viewModel.LoadInstructorsCommand.ExecuteAsync(null);
        }
    }

    public async void OnInstructorTapped(object sender, EventArgs e)
    {
        var border = (Border)sender;
        var instructor = (CourseInstructor)border.BindingContext;
        var viewModel = (CourseInstructorViewModel)BindingContext;

        // Visual feedback animation
        await AnimateTapFeedback(border);

        // Check if we're in remove mode
        if (viewModel.IsRemovingInstructor)
        {
            // Use ViewModel's delete command instead of duplicating logic
            await viewModel.DeleteInstructorCommand.ExecuteAsync(instructor);
        }
        else
        {
            // Navigate to detail view - only pass course since that's what CourseDetailsView expects
            await Shell.Current.GoToAsync(nameof(CourseDetailsView), new Dictionary<string, object>
            {
                ["course"] = viewModel.Course
            });
        }
    }

    private async Task AnimateTapFeedback(Border border)
    {
        try
        {
            // Non-blocking animation
            var originalColor = border.BackgroundColor;

            await border.ScaleToAsync(0.95, 50);
            border.BackgroundColor = Color.FromArgb("#E3F2FD");

            await Task.Delay(100);

            await border.ScaleToAsync(1, 50);
            border.BackgroundColor = originalColor;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Animation error: {ex.Message}");
        }
    }
}