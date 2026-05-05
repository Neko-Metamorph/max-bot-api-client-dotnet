using System.Net.Http.Headers;
using System.Text.Json.Serialization;

namespace MAX.Bot.Interfaces.Models.Request;

/// <summary>
/// Модель запроса на загрузку файла
/// </summary>
public record UploadRequest
{
    private const long MaxFileSizeBytes = 4L * 1024 * 1024 * 1024;

    /// <summary>
    /// Тип загружаемого файла
    /// </summary>
    [JsonPropertyName("type")]
    public UploadType Type { get; set; } = UploadType.Unknown;

    /// <summary>
    /// Поток с содержимым файла
    /// </summary>
    [JsonIgnore]
    public Stream? Content { get; set; }

    /// <summary>
    /// Имя файла для multipart-загрузки
    /// </summary>
    [JsonIgnore]
    public string FileName { get; set; } = string.Empty;

    /// <summary>
    /// MIME-тип файла
    /// </summary>
    [JsonIgnore]
    public string? ContentType { get; set; }

    /// <summary>
    /// Получить строковое значение типа загрузки для API MAX
    /// </summary>
    public string GetUploadTypeValue()
    {
        return Type switch
        {
            UploadType.Image => "image",
            UploadType.Video => "video",
            UploadType.Audio => "audio",
            UploadType.File => "file",
            UploadType.Unknown => throw new ArgumentException(
                "Тип загружаемого файла не задан. Допустимые значения: Image, Video, Audio, File. Значение photo больше не поддерживается.",
                nameof(Type)),
            _ => throw new ArgumentOutOfRangeException(
                nameof(Type),
                Type,
                "Неподдерживаемый тип загружаемого файла. Допустимые значения: Image, Video, Audio, File.")
        };
    }

    /// <summary>
    /// Проверить параметры загрузки файла
    /// </summary>
    public void Validate()
    {
        _ = GetUploadTypeValue();

        if (Content == null)
            throw new ArgumentException("Не задан поток с содержимым файла.", nameof(Content));

        if (!Content.CanRead)
            throw new ArgumentException("Поток с содержимым файла должен поддерживать чтение.", nameof(Content));

        if (string.IsNullOrWhiteSpace(FileName))
            throw new ArgumentException("Не задано имя файла.", nameof(FileName));

        if (!string.IsNullOrWhiteSpace(ContentType) && !MediaTypeHeaderValue.TryParse(ContentType, out _))
            throw new ArgumentException("Некорректный MIME-тип файла.", nameof(ContentType));

        if (Content.CanSeek && Content.Length - Content.Position > MaxFileSizeBytes)
            throw new ArgumentException("Размер файла не должен превышать 4 ГБ.", nameof(Content));
    }
}
