using System.Text.Json.Serialization;
using MAX.Bot.Interfaces.Models.Request.Message;

namespace MAX.Bot.Interfaces.Models.Request;

/// <summary>
/// Запрос на ответ после нажатия пользователем callback-кнопки
/// </summary>
public record AnswerCallbackRequest
{
    /// <summary>
    /// Идентификатор callback-кнопки
    /// </summary>
    [JsonIgnore]
    public string CallbackId { get; set; } = string.Empty;

    /// <summary>
    /// Новое сообщение, если нужно изменить текущее сообщение
    /// </summary>
    [JsonPropertyName("message")]
    public NewMessageBody? Message { get; set; }

    /// <summary>
    /// Одноразовое уведомление для пользователя
    /// </summary>
    [JsonPropertyName("notification")]
    public string? Notification { get; set; }

    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(CallbackId))
            throw new ArgumentException("CallbackId не должен быть пустым.", nameof(CallbackId));

        if (Message is null && Notification is null)
            throw new ArgumentException("Укажите message и/или notification для ответа на callback.");

        if (Notification is not null && string.IsNullOrWhiteSpace(Notification))
            throw new ArgumentException("Notification не должен быть пустым.", nameof(Notification));

        Message?.Validate();
    }
}
