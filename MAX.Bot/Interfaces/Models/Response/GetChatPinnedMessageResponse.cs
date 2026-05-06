using System.Text.Json.Serialization;

namespace MAX.Bot.Interfaces.Models.Response;

/// <summary>
/// Ответ на получение закрепленного сообщения в групповом чате.
/// </summary>
public record GetChatPinnedMessageResponse
{
    /// <summary>
    /// Закрепленное сообщение. Может быть null, если в чате нет закрепленного сообщения.
    /// </summary>
    [JsonPropertyName("message")]
    public Message? Message { get; set; }
}
