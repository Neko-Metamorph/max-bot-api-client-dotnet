using System.Text.Json.Serialization;

namespace MAX.Bot.Interfaces.Models.Request;

/// <summary>
/// Запрос на удаление Webhook-подписки
/// </summary>
public record DeleteSubscriptionRequest
{
    /// <summary>
    /// URL Webhook, который нужно удалить из подписок
    /// </summary>
    [JsonPropertyName("url")]
    public string Url { get; set; } = string.Empty;

    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(Url))
            throw new ArgumentException("URL Webhook не должен быть пустым.", nameof(Url));

        if (!Uri.TryCreate(Url, UriKind.Absolute, out var uri) || uri.Scheme != Uri.UriSchemeHttps)
            throw new ArgumentException("URL Webhook должен быть абсолютным HTTPS URL и начинаться с https://.", nameof(Url));

        if (!uri.IsDefaultPort && uri.Port != 443)
            throw new ArgumentException("URL Webhook должен использовать HTTPS-порт 443.", nameof(Url));
    }
}
