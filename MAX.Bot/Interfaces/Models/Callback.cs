using System.Text.Json.Serialization;

namespace MAX.Bot.Interfaces.Models;

/// <summary>
/// Объект, отправленный боту, когда пользователь нажимает кнопку
/// </summary>
public record Callback
{
    /// <summary>
    /// Unix-время, когда пользователь нажал кнопку
    /// </summary>
    [JsonPropertyName("timestamp")]
    public long Timestamp { get; set; }

    /// <summary>
    /// Текущий ID клавиатуры
    /// </summary>
    [JsonPropertyName("callback_id")]
    public string CallbackId { get; set; } = string.Empty;

    /// <summary>
    /// Токен кнопки
    /// </summary>
    [JsonPropertyName("payload")]
    public string? Payload { get; set; }

    /// <summary>
    /// Пользователь, нажавший на кнопку
    /// </summary>
    [JsonPropertyName("user")]
    public User? User { get; set; }
}
