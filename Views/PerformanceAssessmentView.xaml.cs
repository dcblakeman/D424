using C_971.Models;
using C_971.ViewModels;

namespace C_971.Views;

public partial class PerformanceAssessmentView : ContentPage
{
	public PerformanceAssessmentView(PerformanceAssessmentViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}


    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is PerformanceAssessmentViewModel viewModel)
        {
            // Fix: Pass the instance property, not the type
            await viewModel.LoadAllPerformanceAssessmentsCommand.ExecuteAsync(null);
        }
    }

    private async void OnAssessmentTapped(object sender, EventArgs e)
    {
        var border = (Border)sender;
        var assessment = (CourseAssessment)border.BindingContext;
        var viewModel = (PerformanceAssessmentViewModel)BindingContext;

        // Visual feedback animation
        await border.ScaleToAsync(0.95, 50);
        border.BackgroundColor = Color.FromArgb("#E3F2FD");
        await Task.Delay(100);
        await border.ScaleToAsync(1, 50);
        border.BackgroundColor = Colors.White;

        // Check if we're in remove mode
        if (viewModel.IsDeletingAssessment)
        {
            // Show confirmation dialog
            bool confirm = await Shell.Current.DisplayAlertAsync(
                "Confirm Removal",
                $"Are you sure you want to remove '{assessment.Name}'?",
                "Yes",
                "No");

            if (confirm)
            {
                // Remove from collection
                viewModel.Assessments.Remove(assessment);

                // Exit remove mode
                viewModel.IsDeletingAssessment = false;

                await Shell.Current.DisplayAlertAsync("Success", "Course removed successfully!", "OK");
            }
        }
        else
        {
            // Normal navigation behavior
            //await Shell.Current.GoToAsync("PerformanceAssessmentView", new Dictionary<string, object>
            //{
            //    ["course"] = Course]
            //});
        }
    }

}