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

    //protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    //{
    //    base.OnNavigatedTo(args);

    //    // Check if the source contains AssessmentSelectionView
    //    if (args.PreviousPage is AssessmentSelectionView)
    //    {
    //        if (BindingContext is PerformanceAssessmentViewModel viewModel)
    //        {
    //            await viewModel.GoBackCommand.ExecuteAsync(null);
    //        }
    //    }
    //}

    private async void OnAssessmentTapped(object sender, EventArgs e)
    {
        Border border = (Border)sender;
        CourseAssessment assessment = (CourseAssessment)border.BindingContext;
        PerformanceAssessmentViewModel viewModel = (PerformanceAssessmentViewModel)BindingContext;

        // Visual feedback animation
        _ = await border.ScaleToAsync(0.95, 50);
        border.BackgroundColor = Color.FromArgb("#E3F2FD");
        await Task.Delay(100);
        _ = await border.ScaleToAsync(1, 50);
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
                _ = viewModel.Assessments.Remove(assessment);

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
                viewModel.Assessment.StartDateNotifications = false;
                viewModel.Assessment.EndDateNotifications = false;
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

            await Task.Delay(100); // Small delay for UI responsiveness
            viewModel.AssessmentStartDateNotifications = assessment.StartDateNotifications;
            viewModel.AssessmentEndDateNotifications = assessment.EndDateNotifications;
            viewModel.AssessmentIsActive = true;

            //Save Assessment to database
            await viewModel.SaveAssessmentCommand.ExecuteAsync(null);

            //End Searching
            await viewModel.GoBackCommand.ExecuteAsync(null);

        }
    }

}