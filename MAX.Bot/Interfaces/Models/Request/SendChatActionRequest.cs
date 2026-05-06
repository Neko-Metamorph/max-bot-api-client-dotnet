using System.Text.Json.Serialization;

namespace MAX.Bot.Interfaces.Models.Request;

/// <summary>
/// Запрос на отправку действия бота в групповой чат.
/// </summary>
public record SendChatActionRequest
{
    /// <summary>
    /// Действие, отправляемое участникам чата.
    /// </summary>
    [JsonPropertyName("action")]
    public string Action { get; set; } = string.Empty;

    /// <summary>
    /// Проверить параметры запроса.
    /// </summary>
    public void Validate()
    {
        if (!SenderActions.IsValid(Action))
        {
            throw new ArgumentException(
                "Недопустимое действие бота. Допустимые значения: typing_on, sending_photo, sending_video, sending_audio, sending_file.",
                nameof(Action));
        }
    }
}

/// <summary>
/// Действия бота, которые можно отправить участникам чата.
/// </summary>
public static class SenderActions
{
    /// <summary>
    /// Бот набирает сообщение.
    /// </summary>
    public const string TypingOn = "typing_on";

    /// <summary>
    /// Бот отправляет фото.
    /// </summary>
    public const string SendingPhoto = "sending_photo";

    /// <summary>
    /// Бот отправляет видео.
    /// </summary>
    public const string SendingVideo = "sending_video";

    /// <summary>
    /// Бот отправляет аудиофайл.
    /// </summary>
    public const string SendingAudio = "sending_audio";

    /// <summary>
    /// Бот отправляет файл.
    /// </summary>
    public const string SendingFile = "sending_file";

    /// <summary>
    /// Проверить, поддерживается ли действие API MAX.
    /// </summary>
    public static bool IsValid(string? action)
    {
        return action is
            TypingOn or
            SendingPhoto or
            SendingVideo or
            SendingAudio or
            SendingFile;
    }
}
