using System.Text.Json.Serialization;
using MAX.Bot.Interfaces.Models.Request.Message.Link;

namespace MAX.Bot.Interfaces.Models.Request.Message;

/// <summary>
/// Новое тело сообщения для редактирования или ответа на callback
/// </summary>
public record NewMessageBody
{
    /// <summary>
    /// Новый текст сообщения
    /// </summary>
    [JsonPropertyName("text")]
    public string? Text { get; set; }

    /// <summary>
    /// Вложения сообщения. Если пусто, все вложения будут удалены
    /// </summary>
    [JsonPropertyName("attachments")]
    public List<Attachment.Attachment>? Attachments { get; set; }

    /// <summary>
    /// Ссылка на сообщение
    /// </summary>
    [JsonPropertyName("link")]
    public NewMessageLink? Link { get; set; }

    /// <summary>
    /// Уведомлять ли участников чата о сообщении
    /// </summary>
    [JsonPropertyName("notify")]
    public bool? Notify { get; set; }

    /// <summary>
    /// Форматирование текста сообщения
    /// </summary>
    [JsonPropertyName("format")]
    public MessageFormat? Format { get; set; }

    public void Validate()
    {
        if (Text is { Length: > 4000 })
            throw new ArgumentException("Текст сообщения не должен превышать 4000 символов.", nameof(Text));
    }
}
