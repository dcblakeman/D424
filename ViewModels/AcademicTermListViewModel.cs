using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using C_971.Models;
using C_971.Services;
using System.Collections.ObjectModel;

namespace C_971.ViewModels
{
    public partial class AcademicTermListViewModel : BaseViewModel
    {
        private readonly CourseService _courseService;

        [ObservableProperty]
        private ObservableCollection<AcademicTerm> academicTerms;

        [ObservableProperty]
        private bool isAddingTerm;

        [ObservableProperty]
        public bool isRemovingTerm;

        public bool IsNotAddingTerm => !IsAddingTerm && !IsRemovingTerm;

        [ObservableProperty]
        private string newTermName = string.Empty;

        [ObservableProperty]
        private DateTime newTermStartDate = DateTime.Now;

        [ObservableProperty]
        private DateTime newTermEndDate = DateTime.Now.AddMonths(6);


        public AcademicTermListViewModel(CourseService courseService)
        {
            Name = "                      Academic Terms";
            _courseService = courseService;
            AcademicTerms = _courseService.GetAcademicTerms();
        }

        [RelayCommand]
        private async Task NavigateToTermCourses(AcademicTerm term)
        {
            if (term == null) return;

            await Shell.Current.GoToAsync("CourseListView", new Dictionary<string, object>
            {
                { "term", term }
            });
        }

        [RelayCommand]
        void AddTerm()
        {
            IsAddingTerm = true;
            OnPropertyChanged(nameof(IsNotAddingTerm));

            // Reset form fields
            NewTermName = string.Empty;
            NewTermStartDate = DateTime.Now;
            NewTermEndDate = DateTime.Now.AddMonths(3);
        }

        [RelayCommand]
        async Task SaveNewTerm()
        {
            if (string.IsNullOrWhiteSpace(NewTermName))
            {
                await Shell.Current.DisplayAlertAsync("Error", "Please enter a term name", "OK");
                return;
            }

            if (NewTermEndDate <= NewTermStartDate)
            {
                await Shell.Current.DisplayAlertAsync("Error", "End date must be after start date", "OK");
                return;
            }

            // Create new term and add to collection
            var newTerm = new AcademicTerm
            {
                Name = NewTermName,
                StartDate = NewTermStartDate,
                EndDate = NewTermEndDate
                // Add other required properties
            };

            // Add to your service/database
            // academicTermService.AddTerm(newTerm);

            // Add to collection
            AcademicTerms.Add(newTerm);

            // Exit editing mode
            IsAddingTerm = false;
            OnPropertyChanged(nameof(IsNotAddingTerm));

            await Shell.Current.DisplayAlertAsync("Success", "Term added successfully!", "OK");
        } // End of SaveNewTerm

        [RelayCommand]
        void CancelAddTerm()
        {
            IsAddingTerm = false;
            OnPropertyChanged(nameof(IsNotAddingTerm));
        }

        partial void OnIsAddingTermChanged(bool value)
        {
            OnPropertyChanged(nameof(IsNotAddingTerm));
        }

        [RelayCommand]
        void RemoveTerm()
        {
            IsRemovingTerm = true;
            OnPropertyChanged(nameof(IsNotAddingTerm));
        }

        [RelayCommand]
        void CancelRemoveTerm()
        {
            IsRemovingTerm = false;
            OnPropertyChanged(nameof(IsNotAddingTerm));
        }

        partial void OnIsRemovingTermChanged(bool value)
        {
            OnPropertyChanged(nameof(IsNotAddingTerm));
        }

        [RelayCommand]
        async Task ExitApp()
        {
            bool confirm = await Shell.Current.DisplayAlertAsync(
                "Exit Application",
                "Are you sure you want to exit the application?",
                "Yes",
                "No");

            if (confirm)
            {
                Application.Current?.Quit();
            }
        }
    }
}