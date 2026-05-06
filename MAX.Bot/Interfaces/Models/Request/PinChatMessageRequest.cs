using System.Text.Json.Serialization;

namespace MAX.Bot.Interfaces.Models.Request;

/// <summary>
/// Запрос на закрепление сообщения в групповом чате.
/// </summary>
public record PinChatMessageRequest
{
    /// <summary>
    /// ID сообщения, которое нужно закрепить. Соответствует Message.body.mid.
    /// </summary>
    [JsonPropertyName("message_id")]
    public string MessageId { get; set; } = string.Empty;

    /// <summary>
    /// Если true, участники получат уведомление с системным сообщением о закреплении.
    /// </summary>
    [JsonPropertyName("notify")]
    public bool? Notify { get; set; }

    /// <summary>
    /// Проверить параметры запроса.
    /// </summary>
    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(MessageId))
            throw new ArgumentException("ID сообщения для закрепления не должен быть пустым.", nameof(MessageId));
    }
}
