using System.Text.Json.Serialization;
using MAX.Bot.Interfaces.Models;

namespace MAX.Bot.Interfaces.Models.Request;

/// <summary>
/// Запрос на назначение администраторов группового чата.
/// </summary>
public record AddChatAdminsRequest
{
    /// <summary>
    /// Список пользователей, которые получат права администратора чата.
    /// </summary>
    [JsonPropertyName("admins")]
    public List<ChatAdmin> Admins { get; set; } = new();

    /// <summary>
    /// Указатель на следующую страницу данных.
    /// </summary>
    [JsonPropertyName("marker")]
    public long? Marker { get; set; }

    /// <summary>
    /// Проверить параметры запроса.
    /// </summary>
    public void Validate()
    {
        if (Admins.Count == 0)
            throw new ArgumentException("Список администраторов не должен быть пустым.", nameof(Admins));

        var userIds = new HashSet<long>();

        foreach (var admin in Admins)
        {
            admin.Validate();

            if (!userIds.Add(admin.UserId))
                throw new ArgumentException($"Пользователь {admin.UserId} указан в списке администраторов повторно.", nameof(Admins));
        }
    }
}

/// <summary>
/// Пользователь, которому назначаются права администратора.
/// </summary>
public record ChatAdmin
{
    /// <summary>
    /// Идентификатор пользователя.
    /// </summary>
    [JsonPropertyName("user_id")]
    public long UserId { get; set; }

    /// <summary>
    /// Права администратора.
    /// </summary>
    [JsonPropertyName("permissions")]
    public List<string> Permissions { get; set; } = new();

    /// <summary>
    /// Заголовок, который будет показан на клиенте.
    /// </summary>
    [JsonPropertyName("alias")]
    public string? Alias { get; set; }

    /// <summary>
    /// Проверить параметры администратора.
    /// </summary>
    public void Validate()
    {
        if (UserId <= 0)
            throw new ArgumentException("ID пользователя должен быть положительным числом.", nameof(UserId));

        if (Permissions.Count == 0)
            throw new ArgumentException("Список прав администратора не должен быть пустым.", nameof(Permissions));

        var uniquePermissions = new HashSet<string>(StringComparer.Ordinal);

        foreach (var permission in Permissions)
        {
            if (string.IsNullOrWhiteSpace(permission))
                throw new ArgumentException("Право администратора не должно быть пустым.", nameof(Permissions));

            if (!IsSupportedPermission(permission))
                throw new ArgumentException($"Неподдерживаемое право администратора: {permission}.", nameof(Permissions));

            if (!uniquePermissions.Add(permission))
                throw new ArgumentException($"Право администратора {permission} указано повторно.", nameof(Permissions));
        }

        if (Alias is not null && string.IsNullOrWhiteSpace(Alias))
            throw new ArgumentException("Alias администратора не должен быть пустым.", nameof(Alias));
    }

    private static bool IsSupportedPermission(string permission)
    {
        return permission is
            ChatAdminPermission.ReadAllMessages or
            ChatAdminPermission.AddRemoveMembers or
            ChatAdminPermission.AddAdmins or
            ChatAdminPermission.ChangeChatInfo or
            ChatAdminPermission.PinMessage or
            ChatAdminPermission.Write or
            ChatAdminPermission.CanCall or
            ChatAdminPermission.EditLink or
            ChatAdminPermission.PostEditDeleteMessage or
            ChatAdminPermission.EditMessage or
            ChatAdminPermission.DeleteMessage;
    }
}
