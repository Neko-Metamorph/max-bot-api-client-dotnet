using System.Text.Json.Serialization;

namespace MAX.Bot.Interfaces.Models;

/// <summary>
/// Пользователь нажал на кнопку в чате или канале
/// </summary>
public record MessageCallbackUpdate : Update
{
    public MessageCallbackUpdate()
    {
        UpdateType = UpdateTypes.MessageCallback;
    }

    /// <summary>
    /// Объект callback от нажатой кнопки
    /// </summary>
    [JsonPropertyName("callback")]
    public Callback? Callback { get; set; }

    /// <summary>
    /// Изначальное сообщение со встроенной клавиатурой. Может быть null, если оно было удалено
    /// </summary>
    [JsonPropertyName("message")]
    public Message? Message { get; set; }

    /// <summary>
    /// Текущий язык пользователя в формате IETF BCP 47
    /// </summary>
    [JsonPropertyName("user_locale")]
    public string? UserLocale { get; set; }
}

/// <summary>
/// Пользователь отправил новое сообщение или опубликовал пост
/// </summary>
public record MessageCreatedUpdate : Update
{
    public MessageCreatedUpdate()
    {
        UpdateType = UpdateTypes.MessageCreated;
    }

    /// <summary>
    /// Новое созданное сообщение
    /// </summary>
    [JsonPropertyName("message")]
    public Message? Message { get; set; }

    /// <summary>
    /// Текущий язык пользователя в формате IETF BCP 47. Доступно только в диалогах
    /// </summary>
    [JsonPropertyName("user_locale")]
    public string? UserLocale { get; set; }
}

/// <summary>
/// Пользователь отредактировал сообщение в чате или канале
/// </summary>
public record MessageEditedUpdate : Update
{
    public MessageEditedUpdate()
    {
        UpdateType = UpdateTypes.MessageEdited;
    }

    /// <summary>
    /// Отредактированное сообщение
    /// </summary>
    [JsonPropertyName("message")]
    public Message? Message { get; set; }
}

/// <summary>
/// Пользователь удалил сообщение из чата или канала
/// </summary>
public record MessageRemovedUpdate : Update
{
    public MessageRemovedUpdate()
    {
        UpdateType = UpdateTypes.MessageRemoved;
    }

    /// <summary>
    /// ID удалённого сообщения
    /// </summary>
    [JsonPropertyName("message_id")]
    public string MessageId { get; set; } = string.Empty;

    /// <summary>
    /// ID чата, где сообщение было удалено
    /// </summary>
    [JsonPropertyName("chat_id")]
    public long ChatId { get; set; }

    /// <summary>
    /// Пользователь, удаливший сообщение
    /// </summary>
    [JsonPropertyName("user_id")]
    public long UserId { get; set; }
}
