using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using C_971.Models;
using C_971.Services;

namespace C_971.ViewModels
{
    [QueryProperty(nameof(TermId), "termId")]
    public partial class AcademicTermListViewModel : ObservableObject
    {
        private readonly IDatabaseService _database;

        // Observable Properties
        [ObservableProperty]
        private string name = "                     Academic Terms";

        [ObservableProperty]
        private ObservableCollection<AcademicTerm> academicTerms = new();

        [ObservableProperty]
        private bool isAddingTerm;

        [ObservableProperty]
        private bool isRemovingTerm;

        [ObservableProperty]
        private bool isRefreshing;

        [ObservableProperty]
        private string searchText = string.Empty;

        [ObservableProperty]
        private string newTermName = string.Empty;

        [ObservableProperty]
        private DateTime newTermStartDate = DateTime.Now;

        [ObservableProperty]
        private DateTime newTermEndDate = DateTime.Now.AddMonths(6);

        [ObservableProperty]
        private int termId;

        // Computed Property
        public bool IsNotAddingTerm => !IsAddingTerm && !IsRemovingTerm;

        public AcademicTermListViewModel(IDatabaseService database)
        {
            _database = database;
        }

        private async Task InitializeAsync()
        {
            await _database.InitializeAsync();
            await LoadAcademicTermsAsync();
        }

        private async Task EnsureInitializedAsync()
        {
            if (AcademicTerms.Count == 0)
            {
                await InitializeAsync();
            }
        }

        private async Task OnSearchTextChangedAsync(string oldValue, string newValue)
        {
            await EnsureInitializedAsync();
            if (string.IsNullOrWhiteSpace(newValue))
            {
                await LoadAcademicTermsAsync();
            }
            else
            {
                var filteredTerms = await _database.SearchTermsByNameAsync(newValue);
                AcademicTerms.Clear();
                foreach (var term in filteredTerms)
                {
                    AcademicTerms.Add(term);
                }
            }
        }

        public async Task OnAppearingAsync()
        {
            await EnsureInitializedAsync();
        }

        // Load terms from database
        [RelayCommand]
        private async Task LoadAcademicTermsAsync()
        {
            IsRefreshing = true;
            try
            {
                var terms = await _database.GetTermsAsync();
                AcademicTerms.Clear();
                foreach (var term in terms)
                {
                    AcademicTerms.Add(term);
                }
            }
            finally
            {
                IsRefreshing = false;
            }
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
        private async Task SearchAsync()
        {
            var allTerms = await _database.GetTermsAsync();

            AcademicTerms.Clear();

            var filteredTerms = string.IsNullOrWhiteSpace(SearchText)
                ? allTerms
                : allTerms.Where(t => t.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase));

            foreach (var term in filteredTerms)
            {
                AcademicTerms.Add(term);
            }
        }

        partial void OnSearchTextChanged(string value)
        {
            // Auto-search when text changes
            SearchCommand.Execute(null);
        }

        [RelayCommand]
        void AddTerm()
        {
            IsAddingTerm = true;
            OnPropertyChanged(nameof(IsNotAddingTerm));
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

            try
            {
                // Create new term
                var newTerm = new AcademicTerm
                {
                    Name = NewTermName,
                    StartDate = NewTermStartDate,
                    EndDate = NewTermEndDate
                };

                // Save to database
                await _database.SaveTermAsync(newTerm);

                // Add to collection
                AcademicTerms.Add(newTerm);

                // Exit editing mode
                IsAddingTerm = false;
                OnPropertyChanged(nameof(IsNotAddingTerm));

                await Shell.Current.DisplayAlertAsync("Success", "Term added successfully!", "OK");
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlertAsync("Error", $"Failed to save term: {ex.Message}", "OK");
            }
        }

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
        public async Task ExitApp()
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

        [RelayCommand]
        public async Task DeleteTermAsync(AcademicTerm term)
        {
            if (term == null) return;

            try
            {
                // Delete from database
                await _database.DeleteTermAsync(term);
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlertAsync("Error", $"Failed to delete term: {ex.Message}", "OK");
                throw; // Re-throw so the UI doesn't update if database delete failed
            }
        }
    }
}