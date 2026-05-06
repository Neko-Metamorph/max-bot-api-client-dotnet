using System.Text.Json.Serialization;

namespace MAX.Bot.Interfaces.Models.Request;

/// <summary>
/// Запрос на изменение информации о групповом чате.
/// </summary>
public record UpdateChatRequest
{
    /// <summary>
    /// Иконка чата.
    /// </summary>
    [JsonPropertyName("icon")]
    public PhotoAttachmentRequestPayload? Icon { get; set; }

    /// <summary>
    /// Название чата. От 1 до 200 символов.
    /// </summary>
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    /// <summary>
    /// ID сообщения для закрепления в чате.
    /// </summary>
    [JsonPropertyName("pin")]
    public string? Pin { get; set; }

    /// <summary>
    /// Если true, участники получат системное уведомление об изменении.
    /// </summary>
    [JsonPropertyName("notify")]
    public bool? Notify { get; set; }

    /// <summary>
    /// Проверить параметры запроса.
    /// </summary>
    public void Validate()
    {
        if (Icon == null && Title == null && Pin == null && Notify == null)
            throw new ArgumentException("Нужно задать хотя бы одно поле для изменения чата.");

        if (Title != null && (string.IsNullOrWhiteSpace(Title) || Title.Length > 200))
            throw new ArgumentException("Название чата должно быть от 1 до 200 символов.", nameof(Title));

        if (Pin != null && string.IsNullOrWhiteSpace(Pin))
            throw new ArgumentException("ID сообщения для закрепления не должен быть пустым.", nameof(Pin));

        Icon?.Validate();
    }
}
