using System.Text.Json.Serialization;

namespace MAX.Bot.Interfaces.Models;

/// <summary>
/// Webhook-подписка бота на обновления
/// </summary>
public record Subscription
{
    /// <summary>
    /// URL HTTPS-endpoint webhook
    /// </summary>
    [JsonPropertyName("url")]
    public string Url { get; set; } = string.Empty;

    /// <summary>
    /// Unix-время создания подписки
    /// </summary>
    [JsonPropertyName("time")]
    public long Time { get; set; }

    /// <summary>
    /// Типы обновлений, на которые подписан бот
    /// </summary>
    [JsonPropertyName("update_types")]
    public List<string>? UpdateTypes { get; set; }
}
