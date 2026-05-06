using System.Text.Json.Serialization;

namespace MAX.Bot.Interfaces.Models.Request;

/// <summary>
/// Запрос на прикрепление изображения. Поля являются взаимоисключающими.
/// </summary>
public record PhotoAttachmentRequestPayload
{
    /// <summary>
    /// Внешний URL изображения.
    /// </summary>
    [JsonPropertyName("url")]
    public string? Url { get; set; }

    /// <summary>
    /// Токен существующего изображения.
    /// </summary>
    [JsonPropertyName("token")]
    public string? Token { get; set; }

    /// <summary>
    /// Токены, полученные после загрузки изображений.
    /// </summary>
    [JsonPropertyName("photos")]
    public Dictionary<string, PhotoToken>? Photos { get; set; }

    /// <summary>
    /// Проверить параметры изображения.
    /// </summary>
    public void Validate()
    {
        var providedValuesCount = 0;

        if (Url != null)
            providedValuesCount++;

        if (Token != null)
            providedValuesCount++;

        if (Photos != null)
            providedValuesCount++;

        if (providedValuesCount == 0)
            throw new ArgumentException("Для изображения нужно задать одно из полей: Url, Token или Photos.");

        if (providedValuesCount > 1)
            throw new ArgumentException("Поля Url, Token и Photos являются взаимоисключающими.");

        if (Url != null)
            ValidateUrl(Url);

        if (Token != null && string.IsNullOrWhiteSpace(Token))
            throw new ArgumentException("Токен изображения не должен быть пустым.", nameof(Token));

        if (Photos != null)
            ValidatePhotos(Photos);
    }

    private static void ValidateUrl(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
            throw new ArgumentException("URL изображения не должен быть пустым.", nameof(Url));

        if (!Uri.TryCreate(url, UriKind.Absolute, out var uri) ||
            (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
        {
            throw new ArgumentException("URL изображения должен быть абсолютным HTTP/HTTPS URL.", nameof(Url));
        }
    }

    private static void ValidatePhotos(Dictionary<string, PhotoToken> photos)
    {
        if (photos.Count == 0)
            throw new ArgumentException("Список токенов изображений не должен быть пустым.", nameof(Photos));

        foreach (var photo in photos)
        {
            if (string.IsNullOrWhiteSpace(photo.Key))
                throw new ArgumentException("Ключ токена изображения не должен быть пустым.", nameof(Photos));

            if (string.IsNullOrWhiteSpace(photo.Value.Token))
                throw new ArgumentException("Токен изображения не должен быть пустым.", nameof(Photos));
        }
    }
}

/// <summary>
/// Токен загруженного изображения.
/// </summary>
public record PhotoToken
{
    /// <summary>
    /// Токен изображения.
    /// </summary>
    [JsonPropertyName("token")]
    public string Token { get; set; } = string.Empty;
}
