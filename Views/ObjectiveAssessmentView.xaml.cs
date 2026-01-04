using C_971.Models;
using C_971.ViewModels;

namespace C_971.Views;

public partial class ObjectiveAssessmentView : ContentPage
{
	public ObjectiveAssessmentView(ObjectiveAssessmentViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
    }

    private async void OnAssessmentTapped(object sender, EventArgs e)
    {
        var border = (Border)sender;
        var assessment = (CourseAssessment)border.BindingContext;
        var viewModel = (ObjectiveAssessmentViewModel)BindingContext;

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
            if (viewModel.Assessment != null)
            {
                //Deactivate Current Assessment
                viewModel.Assessment.IsActive = false;
                viewModel.SaveAssessmentCommand.Execute(null);
            }

            //Assign Assessment to ViewModel property
            viewModel.Assessment = assessment;
            viewModel.AssessmentId = assessment.Id;
            viewModel.AssessmentName = assessment.Name;
            viewModel.AssessmentType = assessment.Type;
            viewModel.AssessmentStatus = assessment.Status;
            viewModel.AssessmentStartDate = assessment.StartDate;
            viewModel.AssessmentEndDate = assessment.EndDate;
            viewModel.AssessmentDescription = assessment.Description;
            viewModel.NewCourse.Id = assessment.CourseId;
            viewModel.AssessmentStartDateNotifications = assessment.StartDateNotifications;
            viewModel.AssessmentEndDateNotifications = assessment.EndDateNotifications;
            viewModel.AssessmentIsActive = assessment.IsActive;

            //Save Assessment to database
            await viewModel.SaveAssessmentCommand.ExecuteAsync(null);

            //End Searching
            await viewModel.GoBackCommand.ExecuteAsync(null);
        }
    }
}