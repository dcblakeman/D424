using C_971.Models;
using C_971.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace C_971.ViewModels;

[QueryProperty(nameof(User), "user")]
public partial class AiAssistantViewModel : ObservableObject
{
    private readonly OpenAiChatService _openAiChatService;

    private string? _previousResponseId;
    private bool _hasInitialized;

    [ObservableProperty]
    private User user = new();

    [ObservableProperty]
    private string viewName = Constants.AiAssistantTitle;

    [ObservableProperty]
    private string apiKey = string.Empty;

    [ObservableProperty]
    private string userPrompt = string.Empty;

    [ObservableProperty]
    private bool isSending;

    [ObservableProperty]
    private ObservableCollection<AssistantChatMessage> messages = [];

    public AiAssistantViewModel(OpenAiChatService openAiChatService)
    {
        _openAiChatService = openAiChatService;
    }

    public bool IsNotSending => !IsSending;

    partial void OnIsSendingChanged(bool value)
    {
        OnPropertyChanged(nameof(IsNotSending));
    }

    public async Task InitializeAsync()
    {
        if (_hasInitialized)
        {
            return;
        }

        _hasInitialized = true;

        try
        {
            ApiKey = await SecureStorage.Default.GetAsync(Constants.OpenAiApiKeyStorageKey) ?? string.Empty;
        }
        catch
        {
            ApiKey = string.Empty;
        }

        AddWelcomeMessage();
    }

    [RelayCommand]
    private async Task SaveApiKey()
    {
        if (string.IsNullOrWhiteSpace(ApiKey))
        {
            await Shell.Current.DisplayAlertAsync("Missing Key", "Paste your OpenAI API key before saving it.", "OK");
            return;
        }

        try
        {
            await SecureStorage.Default.SetAsync(Constants.OpenAiApiKeyStorageKey, ApiKey.Trim());
            await Shell.Current.DisplayAlertAsync("Saved", "Your API key was saved locally on this device.", "OK");
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlertAsync("Storage Error", $"I couldn't save the key securely on this device: {ex.Message}", "OK");
        }
    }

    [RelayCommand]
    private async Task ClearApiKey()
    {
        bool confirmed = await Shell.Current.DisplayAlertAsync(
            "Clear API Key",
            "Remove the saved OpenAI API key from this device?",
            "Remove",
            "Cancel");

        if (!confirmed)
        {
            return;
        }

        SecureStorage.Default.Remove(Constants.OpenAiApiKeyStorageKey);
        ApiKey = string.Empty;
        await Shell.Current.DisplayAlertAsync("Removed", "The saved API key was removed from this device.", "OK");
    }

    [RelayCommand]
    private async Task OpenApiKeyPortal()
    {
        try
        {
            await Launcher.Default.OpenAsync(new Uri(Constants.OpenAiApiKeyPortalUrl));
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlertAsync("Link Error", $"Unable to open the API key page right now: {ex.Message}", "OK");
        }
    }

    [RelayCommand]
    private async Task SendPrompt()
    {
        if (IsSending)
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(ApiKey))
        {
            await Shell.Current.DisplayAlertAsync("Missing Key", "Paste your OpenAI API key first, then save it or use it for this session.", "OK");
            return;
        }

        if (string.IsNullOrWhiteSpace(UserPrompt))
        {
            await Shell.Current.DisplayAlertAsync("Missing Prompt", "Type a message for Codex first.", "OK");
            return;
        }

        string promptToSend = UserPrompt.Trim();
        Messages.Add(new AssistantChatMessage(true, promptToSend));
        UserPrompt = string.Empty;
        IsSending = true;

        try
        {
            OpenAiChatReply reply = await _openAiChatService.GetReplyAsync(
                ApiKey.Trim(),
                promptToSend,
                _previousResponseId);

            _previousResponseId = reply.ResponseId;
            Messages.Add(new AssistantChatMessage(false, reply.MessageText));
        }
        catch (Exception ex)
        {
            Messages.Add(new AssistantChatMessage(false,
                $"I hit a snag talking to OpenAI: {ex.Message}"));
        }
        finally
        {
            IsSending = false;
        }
    }

    [RelayCommand]
    private void ClearConversation()
    {
        _previousResponseId = null;
        Messages.Clear();
        AddWelcomeMessage();
    }

    [RelayCommand]
    private async Task GoBack()
    {
        await Shell.Current.GoToAsync("..");
    }

    private void AddWelcomeMessage()
    {
        if (Messages.Count > 0)
        {
            return;
        }

        Messages.Add(new AssistantChatMessage(
            false,
            "Hi, I'm Codex. I can help you study, plan, organize notes, and think through assessments. " +
            "I do not automatically read your courses or database yet, so paste in the details you want help with."));
    }
}
