using System.Text.Json.Serialization;
using MAX.Bot.Interfaces.JsonConverters;

namespace MAX.Bot.Interfaces.Models;

/// <summary>
/// Модель обновления
/// </summary>
[JsonConverter(typeof(UpdateJsonConverter))]
public record Update
{
    /// <summary>
    /// Тип обновления (UpdateTypes)
    /// </summary>
    [JsonPropertyName("update_type")]
    public string UpdateType { get; set; } = string.Empty;

    /// <summary>
    /// Unix-время, когда произошло событие
    /// </summary>
    [JsonPropertyName("timestamp")]
    public long Timestamp { get; set; }
}

/// <summary>
/// Типы обновлений (событий) в системе
/// </summary>
public static class UpdateTypes
{
    /// <summary>
    /// Бот добавлен в чат или канал
    /// </summary>
    public const string BotAdded = "bot_added";

    /// <summary>
    /// Пользователь начал или возобновил общение с ботом
    /// </summary>
    public const string BotStarted = "bot_started";

    /// <summary>
    /// Пользователь остановил бота
    /// </summary>
    public const string BotStopped = "bot_stopped";

    /// <summary>
    /// Бот удалён из чата или канала
    /// </summary>
    public const string BotRemoved = "bot_removed";

    /// <summary>
    /// Пользователь изменил название чата или канала
    /// </summary>
    public const string ChatTitleChanged = "chat_title_changed";

    /// <summary>
    /// Пользователь очистил историю диалога с ботом
    /// </summary>
    public const string DialogCleared = "dialog_cleared";

    /// <summary>
    /// Пользователь отключил уведомления в диалоге с ботом
    /// </summary>
    public const string DialogMuted = "dialog_muted";

    /// <summary>
    /// Пользователь включил уведомления в диалоге с ботом
    /// </summary>
    public const string DialogUnmuted = "dialog_unmuted";

    /// <summary>
    /// Пользователь удалил диалог с ботом
    /// </summary>
    public const string DialogRemoved = "dialog_removed";

    /// <summary>
    /// Обратный вызов (callback) от кнопки сообщения
    /// </summary>
    public const string MessageCallback = "message_callback";

    /// <summary>
    /// Создание нового сообщения
    /// </summary>
    public const string MessageCreated = "message_created";

    /// <summary>
    /// Редактирование существующего сообщения
    /// </summary>
    public const string MessageEdited = "message_edited";

    /// <summary>
    /// Удаление сообщения
    /// </summary>
    public const string MessageRemoved = "message_removed";

    /// <summary>
    /// В чат или канал добавлен новый пользователь
    /// </summary>
    public const string UserAdded = "user_added";

    /// <summary>
    /// Пользователь удалён или покинул чат или канал
    /// </summary>
    public const string UserRemoved = "user_removed";

    /// <summary>
    /// Устаревшее имя константы. Используйте <see cref="MessageRemoved"/>.
    /// </summary>
    [Obsolete("Use MessageRemoved.")]
    public const string MessageDeleted = MessageRemoved;

    /// <summary>
    /// Устаревшее имя константы. Используйте <see cref="UserAdded"/>.
    /// </summary>
    [Obsolete("Use UserAdded.")]
    public const string ChatMemberJoined = UserAdded;

    /// <summary>
    /// Устаревшее имя константы. Используйте <see cref="UserRemoved"/>.
    /// </summary>
    [Obsolete("Use UserRemoved.")]
    public const string ChatMemberLeft = UserRemoved;
}
