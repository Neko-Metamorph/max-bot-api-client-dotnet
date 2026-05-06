using System.Text.Json.Serialization;

namespace MAX.Bot.Interfaces.Models.Response;

/// <summary>
/// Ответ на получение списка Webhook-подписок
/// </summary>
public record GetSubscriptionsResponse
{
    /// <summary>
    /// Список текущих подписок
    /// </summary>
    [JsonPropertyName("subscriptions")]
    public List<Subscription> Subscriptions { get; set; } = new();
}
