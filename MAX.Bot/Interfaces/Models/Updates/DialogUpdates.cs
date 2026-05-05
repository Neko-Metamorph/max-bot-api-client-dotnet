using System.Text.Json.Serialization;

namespace MAX.Bot.Interfaces.Models;

/// <summary>
/// Пользователь очистил историю сообщений
/// </summary>
public record DialogClearedUpdate : Update
{
    public DialogClearedUpdate()
    {
        UpdateType = UpdateTypes.DialogCleared;
    }

    /// <summary>
    /// ID чата, где произошло событие
    /// </summary>
    [JsonPropertyName("chat_id")]
    public long ChatId { get; set; }

    /// <summary>
    /// Пользователь, который очистил историю сообщений
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
/// Пользователь отключил уведомления о новых сообщениях в диалоге с ботом
/// </summary>
public record DialogMutedUpdate : Update
{
    public DialogMutedUpdate()
    {
        UpdateType = UpdateTypes.DialogMuted;
    }

    /// <summary>
    /// ID чата, где произошло событие
    /// </summary>
    [JsonPropertyName("chat_id")]
    public long ChatId { get; set; }

    /// <summary>
    /// Пользователь, который отключил уведомления
    /// </summary>
    [JsonPropertyName("user")]
    public User? User { get; set; }

    /// <summary>
    /// Время в формате Unix, до наступления которого диалог был отключён
    /// </summary>
    [JsonPropertyName("muted_until")]
    public long MutedUntil { get; set; }

    /// <summary>
    /// Текущий язык пользователя в формате IETF BCP 47
    /// </summary>
    [JsonPropertyName("user_locale")]
    public string? UserLocale { get; set; }
}

/// <summary>
/// Пользователь включил уведомления о новых сообщениях в диалоге с ботом
/// </summary>
public record DialogUnmutedUpdate : Update
{
    public DialogUnmutedUpdate()
    {
        UpdateType = UpdateTypes.DialogUnmuted;
    }

    /// <summary>
    /// ID чата, где произошло событие
    /// </summary>
    [JsonPropertyName("chat_id")]
    public long ChatId { get; set; }

    /// <summary>
    /// Пользователь, который включил уведомления
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
/// Пользователь удалил чат
/// </summary>
public record DialogRemovedUpdate : Update
{
    public DialogRemovedUpdate()
    {
        UpdateType = UpdateTypes.DialogRemoved;
    }

    /// <summary>
    /// ID чата, где произошло событие
    /// </summary>
    [JsonPropertyName("chat_id")]
    public long ChatId { get; set; }

    /// <summary>
    /// Пользователь, который удалил чат
    /// </summary>
    [JsonPropertyName("user")]
    public User? User { get; set; }

    /// <summary>
    /// Текущий язык пользователя в формате IETF BCP 47
    /// </summary>
    [JsonPropertyName("user_locale")]
    public string? UserLocale { get; set; }
}
