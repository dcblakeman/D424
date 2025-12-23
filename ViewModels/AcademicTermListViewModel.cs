using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using C_971.Models;
using C_971.Services;

namespace C_971.ViewModels
{
    public partial class AcademicTermListViewModel : ObservableObject
    {
        private readonly DatabaseService _database;

        // Core Properties
        [ObservableProperty]
        private string viewName = "Academic Terms";

        [ObservableProperty]
        private AcademicTerm newTerm = new AcademicTerm();

        [ObservableProperty]
        private ObservableCollection<AcademicTerm> academicTerms = new();

        // UI State
        [ObservableProperty]
        private bool isAddingTerm;

        [ObservableProperty]
        private bool isRemovingTerm;

        [ObservableProperty]
        private bool isRefreshing;

        public bool IsNotAddingTerm => !IsAddingTerm && !IsRemovingTerm;

        // Search
        [ObservableProperty]
        private string searchText = string.Empty;

        private List<AcademicTerm> _allTerms = new();

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
            Console.WriteLine($"Database Path: {C_971.Constants.DatabasePath}");
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
            AcademicTerms.Clear();

            var filteredTerms = string.IsNullOrWhiteSpace(SearchText)
                ? _allTerms
                : _allTerms.Where(t => t.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase));

            foreach (var term in filteredTerms)
            {
                AcademicTerms.Add(term);
            }
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
            if (!ValidateNewTerm()) return;

            try
            {
                NewTerm.Name = NewTermName;
                NewTerm.StartDate = NewTermStartDate;
                NewTerm.EndDate = NewTermEndDate;

                await _database.SaveTermAsync(NewTerm);

                // Add to cache and refresh display
                _allTerms.Add(NewTerm);
                ApplySearchFilter();

                ClearForm();
                IsAddingTerm = false;

                await Shell.Current.DisplayAlertAsync("Success", "Term added successfully!", "OK");
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
            if (term == null) return;

            try
            {
                await _database.DeleteTermAsync(term);

                // Remove from cache and UI
                _allTerms.Remove(term);
                AcademicTerms.Remove(term);

                // Exit remove mode
                IsRemovingTerm = false;

                await Shell.Current.DisplayAlertAsync("Success", "Term deleted successfully.", "OK");
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlertAsync("Error", $"Failed to delete term: {ex.Message}", "OK");
            }
        }

        // Application Management
        [RelayCommand]
        public async Task ExitApp()
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
            NewTermName = string.Empty;
            NewTermStartDate = DateTime.Now;
            NewTermEndDate = DateTime.Now.AddMonths(6);
        }
    }
}