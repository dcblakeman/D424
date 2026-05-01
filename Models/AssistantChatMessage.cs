namespace C_971.Models;

public sealed class AssistantChatMessage
{
    public AssistantChatMessage(bool isUser, string text)
    {
        IsUser = isUser;
        Text = text;
        Timestamp = DateTime.Now;
    }

    public bool IsUser { get; }

    public string Text { get; }

    public DateTime Timestamp { get; }

    public string Speaker => IsUser ? "You" : "Codex";

    public string BubbleBackground => IsUser ? "#E3F2FD" : "#F3E5F5";

    public string BubbleStroke => IsUser ? "#1E88E5" : "#7E57C2";

    public string TimestampText => Timestamp.ToString("h:mm tt");
}
