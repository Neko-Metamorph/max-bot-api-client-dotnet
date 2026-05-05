using System.Text.Json.Serialization;

namespace MAX.Bot.Interfaces.Models;

/// <summary>
/// Пользователь изменил название чата или канала
/// </summary>
public record ChatTitleChangedUpdate : Update
{
    public ChatTitleChangedUpdate()
    {
        UpdateType = UpdateTypes.ChatTitleChanged;
    }

    /// <summary>
    /// ID чата, где произошло событие
    /// </summary>
    [JsonPropertyName("chat_id")]
    public long ChatId { get; set; }

    /// <summary>
    /// Пользователь, который изменил название
    /// </summary>
    [JsonPropertyName("user")]
    public User? User { get; set; }

    /// <summary>
    /// Новое название
    /// </summary>
    [JsonPropertyName("title")]
    public string? Title { get; set; }
}

/// <summary>
/// Пользователь добавлен в чат или канал
/// </summary>
public record UserAddedToChatUpdate : Update
{
    public UserAddedToChatUpdate()
    {
        UpdateType = UpdateTypes.UserAdded;
    }

    /// <summary>
    /// ID чата, где произошло событие
    /// </summary>
    [JsonPropertyName("chat_id")]
    public long ChatId { get; set; }

    /// <summary>
    /// Пользователь, добавленный в чат
    /// </summary>
    [JsonPropertyName("user")]
    public User? User { get; set; }

    /// <summary>
    /// Пользователь, который добавил нового пользователя в чат. Может быть null, если пользователь присоединился по ссылке
    /// </summary>
    [JsonPropertyName("inviter_id")]
    public long? InviterId { get; set; }

    /// <summary>
    /// Указывает, что пользователь добавлен в канал, а не в чат
    /// </summary>
    [JsonPropertyName("is_channel")]
    public bool IsChannel { get; set; }
}

/// <summary>
/// Пользователь удалён или покинул чат или канал
/// </summary>
public record UserRemovedFromChatUpdate : Update
{
    public UserRemovedFromChatUpdate()
    {
        UpdateType = UpdateTypes.UserRemoved;
    }

    /// <summary>
    /// ID чата, где произошло событие
    /// </summary>
    [JsonPropertyName("chat_id")]
    public long ChatId { get; set; }

    /// <summary>
    /// Пользователь, удалённый из чата
    /// </summary>
    [JsonPropertyName("user")]
    public User? User { get; set; }

    /// <summary>
    /// Администратор, который удалил пользователя из чата. Может быть null, если пользователь покинул чат сам
    /// </summary>
    [JsonPropertyName("admin_id")]
    public long? AdminId { get; set; }

    /// <summary>
    /// Указывает, что пользователь удалён из канала, а не из чата
    /// </summary>
    [JsonPropertyName("is_channel")]
    public bool IsChannel { get; set; }
}
