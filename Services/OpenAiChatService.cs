using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace C_971.Services;

public sealed class OpenAiChatService
{
    private const string ResponsesEndpoint = "https://api.openai.com/v1/responses";

    private readonly HttpClient _httpClient;

    public OpenAiChatService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<OpenAiChatReply> GetReplyAsync(
        string apiKey,
        string userMessage,
        string? previousResponseId,
        CancellationToken cancellationToken = default)
    {
        using HttpRequestMessage request = new(HttpMethod.Post, ResponsesEndpoint);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

        Dictionary<string, object?> payload = new()
        {
            ["model"] = Constants.OpenAiDefaultModel,
            ["instructions"] =
                "You are Codex, a warm and practical study assistant inside College Course Tracker. " +
                "Help with study planning, assessment prep, organization, motivation, and clear explanations. " +
                "Be supportive, concise, and honest. Do not pretend to see the app database or user information " +
                "unless the user pastes it into chat.",
            ["input"] = userMessage,
            ["max_output_tokens"] = 500
        };

        if (!string.IsNullOrWhiteSpace(previousResponseId))
        {
            payload["previous_response_id"] = previousResponseId;
        }

        string jsonPayload = JsonSerializer.Serialize(payload);
        request.Content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

        using HttpResponseMessage response = await _httpClient.SendAsync(request, cancellationToken);
        string responseJson = await response.Content.ReadAsStringAsync(cancellationToken);

        using JsonDocument document = JsonDocument.Parse(responseJson);
        JsonElement root = document.RootElement;

        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException(GetErrorMessage(root, response.ReasonPhrase));
        }

        string messageText = ExtractOutputText(root);
        string responseId = root.TryGetProperty("id", out JsonElement idElement)
            ? idElement.GetString() ?? string.Empty
            : string.Empty;

        if (string.IsNullOrWhiteSpace(messageText))
        {
            messageText = "I received a response, but it did not include readable text.";
        }

        return new OpenAiChatReply(responseId, messageText);
    }

    private static string GetErrorMessage(JsonElement root, string? defaultMessage)
    {
        if (root.TryGetProperty("error", out JsonElement errorElement) &&
            errorElement.TryGetProperty("message", out JsonElement messageElement))
        {
            return messageElement.GetString() ?? defaultMessage ?? "Request failed.";
        }

        return defaultMessage ?? "Request failed.";
    }

    private static string ExtractOutputText(JsonElement root)
    {
        if (root.TryGetProperty("output_text", out JsonElement outputTextElement) &&
            outputTextElement.ValueKind == JsonValueKind.String)
        {
            string? outputText = outputTextElement.GetString();
            if (!string.IsNullOrWhiteSpace(outputText))
            {
                return outputText;
            }
        }

        if (!root.TryGetProperty("output", out JsonElement outputElement) ||
            outputElement.ValueKind != JsonValueKind.Array)
        {
            return string.Empty;
        }

        StringBuilder builder = new();

        foreach (JsonElement item in outputElement.EnumerateArray())
        {
            if (!item.TryGetProperty("content", out JsonElement contentElement) ||
                contentElement.ValueKind != JsonValueKind.Array)
            {
                continue;
            }

            foreach (JsonElement contentItem in contentElement.EnumerateArray())
            {
                if (!contentItem.TryGetProperty("type", out JsonElement typeElement))
                {
                    continue;
                }

                string? type = typeElement.GetString();
                if (type is not ("output_text" or "text"))
                {
                    continue;
                }

                if (!contentItem.TryGetProperty("text", out JsonElement textElement))
                {
                    continue;
                }

                string? text = textElement.GetString();
                if (!string.IsNullOrWhiteSpace(text))
                {
                    if (builder.Length > 0)
                    {
                        _ = builder.AppendLine().AppendLine();
                    }

                    _ = builder.Append(text.Trim());
                }
            }
        }

        return builder.ToString();
    }
}

public sealed record OpenAiChatReply(string ResponseId, string MessageText);
