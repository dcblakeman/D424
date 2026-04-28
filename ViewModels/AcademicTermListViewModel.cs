using C_971.Models;
using C_971.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace C_971.ViewModels
{
    [QueryProperty(nameof(UserId), "userId")]
    [QueryProperty(nameof(User), "user")]
    public partial class AcademicTermListViewModel : ObservableObject
    {
        private readonly DatabaseService _database;

        // Core Properties
        [ObservableProperty]
        private string viewName = "Academic Terms";

        [ObservableProperty]
        private AcademicTerm newTerm = new();

        [ObservableProperty]
        private User user = new();

        [ObservableProperty]
        private User newUser = null!;

        [ObservableProperty]
        private int userId;[ObservableProperty]
        private int newUserId;

        [ObservableProperty]
        private ObservableCollection<AcademicTerm> academicTerms = [];

        // UI State
        [ObservableProperty]
        private bool isAddingTerm;[ObservableProperty]
        private bool isRemovingTerm;

        [ObservableProperty]
        private bool isRefreshing;public bool IsNotAddingTerm => !IsAddingTerm;

        // Search
        [ObservableProperty]
        private string searchText = string.Empty;

        private List<AcademicTerm> _allTerms = [];

        // New Term Form
        [ObservableProperty]
        private string newTermName = string.Empty;

        [ObservableProperty]
        private DateTime newTermStartDate = DateTime.Now;

        [ObservableProperty]
        private DateTime newTermEndDate = DateTime.Now.AddMonths(6);

        public AcademicTermListViewModel(DatabaseService database)
        {
            _database = database;
        }

        partial void OnUserChanged(User value)
        {
            NewUser = value;
        }

        // Initialization
        public async Task OnAppearingAsync()
        {
            if (AcademicTerms.Count == 0)
            {
                await LoadTermsAsync();
            }
        }

        // Property Change Handlers
        partial void OnSearchTextChanged(string value)
        {
            ApplySearchFilter();
        }

        partial void OnIsAddingTermChanged(bool value)
        {
            OnPropertyChanged(nameof(IsNotAddingTerm));
        }

        partial void OnIsRemovingTermChanged(bool value)
        {
            OnPropertyChanged(nameof(IsNotAddingTerm));
        }

        // Search and Filter
        [RelayCommand]
        private void Search()
        {
            ApplySearchFilter();
        }

        private void ApplySearchFilter()
        {
            IEnumerable<AcademicTerm> filteredTerms = string.IsNullOrWhiteSpace(SearchText)
                ? _allTerms
                : _allTerms.Where(t => t.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase));
            AcademicTerms = new ObservableCollection<AcademicTerm>(filteredTerms);
        }

        // Data Loading
        [RelayCommand]
        private async Task LoadTermsAsync()
        {
            IsRefreshing = true;
            try
            {
                _allTerms = await _database.GetTermsAsync();
                ApplySearchFilter();
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlertAsync("Error", $"Failed to load terms: {ex.Message}", "OK");
            }
            finally
            {
                IsRefreshing = false;
            }
        }

        // Term Management
        [RelayCommand]
        private void AddTerm()
        {
            IsAddingTerm = true;
        }

        [RelayCommand]
        private void CancelAddTerm()
        {
            IsAddingTerm = false;
            ClearForm();
        }

        [RelayCommand]
        private async Task SaveNewTerm()
        {
            if (!ValidateNewTerm())
            {
                return;
            }

            try
            {
                AcademicTerm termToSave = new()
                {
                    Name = NewTermName.Trim(),
                    StartDate = NewTermStartDate,
                    EndDate = NewTermEndDate,
                    CreatedDate = DateTime.Now
                };

                _ = await _database.SaveTermAsync(termToSave);
                NewTerm = termToSave;
                SearchText = string.Empty;
                await LoadTermsAsync();
                await Shell.Current.DisplayAlertAsync("Success", "Term added successfully!", "OK");

                ClearForm();
                IsAddingTerm = false;
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlertAsync("Error", $"Failed to save term: {ex.Message}", "OK");
            }
        }

        [RelayCommand]
        public void RemoveTerm()
        {
            IsRemovingTerm = true;
        }

        [RelayCommand]
        public void CancelRemoveTerm()
        {
            IsRemovingTerm = false;
        }

        [RelayCommand]
        public async Task DeleteTerm(AcademicTerm term)
        {
            if (term == null)
            {
                return;
            }

            try
            {
                _ = await _database.DeleteTermAsync(term);

                // Remove from cache and UI
                _ = _allTerms.Remove(term);
                _ = AcademicTerms.Remove(term);

                // Exit remove mode
                IsRemovingTerm = false;

                await Shell.Current.DisplayAlertAsync("Success", "Term deleted successfully.", "OK");
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlertAsync("Error", $"Failed to delete term: {ex.Message}", "OK");
            }
        }

        // Helper Methods
        private bool ValidateNewTerm()
        {
            if (string.IsNullOrWhiteSpace(NewTermName))
            {
                _ = Shell.Current.DisplayAlertAsync("Error", "Please enter a term name", "OK");
                return false;
            }

            if (NewTermEndDate <= NewTermStartDate)
            {
                _ = Shell.Current.DisplayAlertAsync("Error", "End date must be after start date", "OK");
                return false;
            }

            // Check for duplicate names
            if (_allTerms.Any(t => t.Name.Equals(NewTermName, StringComparison.OrdinalIgnoreCase)))
            {
                _ = Shell.Current.DisplayAlertAsync("Error", "A term with this name already exists", "OK");
                return false;
            }

            return true;
        }

        private void ClearForm()
        {
            NewTerm = new AcademicTerm();
            NewTermName = string.Empty;
            NewTermStartDate = DateTime.Now;
            NewTermEndDate = DateTime.Now.AddMonths(6);
        }

        [RelayCommand]
        public async Task Logout()
        {
            // Navigate back to login page
            await Shell.Current.GoToAsync("///LoginView", true);
        }
    }
}
