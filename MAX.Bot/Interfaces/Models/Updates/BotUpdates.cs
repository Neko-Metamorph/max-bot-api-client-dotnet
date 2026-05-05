using System.Text.Json.Serialization;

namespace MAX.Bot.Interfaces.Models;

/// <summary>
/// Бот добавлен в чат или канал
/// </summary>
public record BotAddedToChatUpdate : Update
{
    public BotAddedToChatUpdate()
    {
        UpdateType = UpdateTypes.BotAdded;
    }

    /// <summary>
    /// ID чата, куда был добавлен бот
    /// </summary>
    [JsonPropertyName("chat_id")]
    public long ChatId { get; set; }

    /// <summary>
    /// Пользователь, добавивший бота в чат
    /// </summary>
    [JsonPropertyName("user")]
    public User? User { get; set; }

    /// <summary>
    /// Указывает, что бот добавлен в канал, а не в чат
    /// </summary>
    [JsonPropertyName("is_channel")]
    public bool IsChannel { get; set; }
}

/// <summary>
/// Пользователь начал или возобновил общение с ботом
/// </summary>
public record BotStartedUpdate : Update
{
    public BotStartedUpdate()
    {
        UpdateType = UpdateTypes.BotStarted;
    }

    /// <summary>
    /// ID диалога, где произошло событие
    /// </summary>
    [JsonPropertyName("chat_id")]
    public long ChatId { get; set; }

    /// <summary>
    /// Пользователь, который нажал кнопку Start
    /// </summary>
    [JsonPropertyName("user")]
    public User? User { get; set; }

    /// <summary>
    /// Дополнительные данные из диплинков, переданные при запуске бота
    /// </summary>
    [JsonPropertyName("payload")]
    public string? Payload { get; set; }

    /// <summary>
    /// Текущий язык пользователя в формате IETF BCP 47
    /// </summary>
    [JsonPropertyName("user_locale")]
    public string? UserLocale { get; set; }
}

/// <summary>
/// Пользователь остановил бота
/// </summary>
public record BotStoppedUpdate : Update
{
    public BotStoppedUpdate()
    {
        UpdateType = UpdateTypes.BotStopped;
    }

    /// <summary>
    /// ID диалога, где произошло событие
    /// </summary>
    [JsonPropertyName("chat_id")]
    public long ChatId { get; set; }

    /// <summary>
    /// Пользователь, который остановил бота
    /// </summary>
    [JsonPropertyName("user")]
    public User? User { get; set; }

    /// <summary>
    /// Текущий язык пользователя в формате IETF BCP 47
    /// </summary>
    [JsonPropertyName("user_locale")]
    public string? UserLocale { get; set; }
}

/// <summary>
/// Бот удалён из чата или канала
/// </summary>
public record BotRemovedFromChatUpdate : Update
{
    public BotRemovedFromChatUpdate()
    {
        UpdateType = UpdateTypes.BotRemoved;
    }

    /// <summary>
    /// ID чата, откуда был удалён бот
    /// </summary>
    [JsonPropertyName("chat_id")]
    public long ChatId { get; set; }

    /// <summary>
    /// Пользователь, удаливший бота из чата
    /// </summary>
    [JsonPropertyName("user")]
    public User? User { get; set; }

    /// <summary>
    /// Указывает, что бот удалён из канала, а не из чата
    /// </summary>
    [JsonPropertyName("is_channel")]
    public bool IsChannel { get; set; }
}
