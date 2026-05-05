using System.Text.Json;
using System.Text.Json.Serialization;
using MAX.Bot.Interfaces.Models;

namespace MAX.Bot.Interfaces.JsonConverters;

/// <summary>
/// Десериализует Update в конкретную модель по полю update_type.
/// </summary>
public sealed class UpdateJsonConverter : JsonConverter<Update>
{
    public override Update Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
            throw new JsonException("Update должен быть JSON-объектом.");

        using var document = JsonDocument.ParseValue(ref reader);
        var root = document.RootElement;
        var updateType = GetRequiredString(root, "update_type");

        return updateType switch
        {
            UpdateTypes.BotAdded => ReadBotAdded(root, options),
            UpdateTypes.BotStarted => ReadBotStarted(root, options),
            UpdateTypes.BotStopped => ReadBotStopped(root, options),
            UpdateTypes.BotRemoved => ReadBotRemoved(root, options),
            UpdateTypes.ChatTitleChanged => ReadChatTitleChanged(root, options),
            UpdateTypes.DialogCleared => ReadDialogCleared(root, options),
            UpdateTypes.DialogMuted => ReadDialogMuted(root, options),
            UpdateTypes.DialogUnmuted => ReadDialogUnmuted(root, options),
            UpdateTypes.DialogRemoved => ReadDialogRemoved(root, options),
            UpdateTypes.MessageCallback => ReadMessageCallback(root, options),
            UpdateTypes.MessageCreated => ReadMessageCreated(root, options),
            UpdateTypes.MessageEdited => ReadMessageEdited(root, options),
            UpdateTypes.MessageRemoved => ReadMessageRemoved(root),
            UpdateTypes.UserAdded => ReadUserAdded(root, options),
            UpdateTypes.UserRemoved => ReadUserRemoved(root, options),
            _ => ReadBase(root, updateType)
        };
    }

    public override void Write(Utf8JsonWriter writer, Update value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        WriteBase(writer, value);

        switch (value)
        {
            case BotAddedToChatUpdate update:
                WriteChatUserChannelFields(writer, update.ChatId, update.User, update.IsChannel, options);
                break;
            case BotStartedUpdate update:
                WriteChatUserFields(writer, update.ChatId, update.User, options);
                WriteStringIfNotNull(writer, "payload", update.Payload);
                WriteStringIfNotNull(writer, "user_locale", update.UserLocale);
                break;
            case BotStoppedUpdate update:
                WriteChatUserFields(writer, update.ChatId, update.User, options);
                WriteStringIfNotNull(writer, "user_locale", update.UserLocale);
                break;
            case BotRemovedFromChatUpdate update:
                WriteChatUserChannelFields(writer, update.ChatId, update.User, update.IsChannel, options);
                break;
            case ChatTitleChangedUpdate update:
                WriteChatUserFields(writer, update.ChatId, update.User, options);
                WriteStringIfNotNull(writer, "title", update.Title);
                break;
            case DialogClearedUpdate update:
                WriteChatUserFields(writer, update.ChatId, update.User, options);
                WriteStringIfNotNull(writer, "user_locale", update.UserLocale);
                break;
            case DialogMutedUpdate update:
                WriteChatUserFields(writer, update.ChatId, update.User, options);
                writer.WriteNumber("muted_until", update.MutedUntil);
                WriteStringIfNotNull(writer, "user_locale", update.UserLocale);
                break;
            case DialogUnmutedUpdate update:
                WriteChatUserFields(writer, update.ChatId, update.User, options);
                WriteStringIfNotNull(writer, "user_locale", update.UserLocale);
                break;
            case DialogRemovedUpdate update:
                WriteChatUserFields(writer, update.ChatId, update.User, options);
                WriteStringIfNotNull(writer, "user_locale", update.UserLocale);
                break;
            case MessageCallbackUpdate update:
                WriteObject(writer, "callback", update.Callback, options);
                WriteObject(writer, "message", update.Message, options);
                WriteStringIfNotNull(writer, "user_locale", update.UserLocale);
                break;
            case MessageCreatedUpdate update:
                WriteObject(writer, "message", update.Message, options);
                WriteStringIfNotNull(writer, "user_locale", update.UserLocale);
                break;
            case MessageEditedUpdate update:
                WriteObject(writer, "message", update.Message, options);
                break;
            case MessageRemovedUpdate update:
                writer.WriteString("message_id", update.MessageId);
                writer.WriteNumber("chat_id", update.ChatId);
                writer.WriteNumber("user_id", update.UserId);
                break;
            case UserAddedToChatUpdate update:
                WriteChatUserChannelFields(writer, update.ChatId, update.User, update.IsChannel, options);
                WriteNullableNumber(writer, "inviter_id", update.InviterId);
                break;
            case UserRemovedFromChatUpdate update:
                WriteChatUserChannelFields(writer, update.ChatId, update.User, update.IsChannel, options);
                WriteNullableNumber(writer, "admin_id", update.AdminId);
                break;
        }

        writer.WriteEndObject();
    }

    private static Update ReadBase(JsonElement root, string updateType)
    {
        return FillBase(root, new Update
        {
            UpdateType = updateType
        });
    }

    private static BotAddedToChatUpdate ReadBotAdded(JsonElement root, JsonSerializerOptions options)
    {
        return FillBase(root, new BotAddedToChatUpdate
        {
            ChatId = GetInt64(root, "chat_id"),
            User = DeserializeProperty<User>(root, "user", options),
            IsChannel = GetBoolean(root, "is_channel")
        });
    }

    private static BotStartedUpdate ReadBotStarted(JsonElement root, JsonSerializerOptions options)
    {
        return FillBase(root, new BotStartedUpdate
        {
            ChatId = GetInt64(root, "chat_id"),
            User = DeserializeProperty<User>(root, "user", options),
            Payload = GetString(root, "payload"),
            UserLocale = GetString(root, "user_locale")
        });
    }

    private static BotStoppedUpdate ReadBotStopped(JsonElement root, JsonSerializerOptions options)
    {
        return FillBase(root, new BotStoppedUpdate
        {
            ChatId = GetInt64(root, "chat_id"),
            User = DeserializeProperty<User>(root, "user", options),
            UserLocale = GetString(root, "user_locale")
        });
    }

    private static BotRemovedFromChatUpdate ReadBotRemoved(JsonElement root, JsonSerializerOptions options)
    {
        return FillBase(root, new BotRemovedFromChatUpdate
        {
            ChatId = GetInt64(root, "chat_id"),
            User = DeserializeProperty<User>(root, "user", options),
            IsChannel = GetBoolean(root, "is_channel")
        });
    }

    private static ChatTitleChangedUpdate ReadChatTitleChanged(JsonElement root, JsonSerializerOptions options)
    {
        return FillBase(root, new ChatTitleChangedUpdate
        {
            ChatId = GetInt64(root, "chat_id"),
            User = DeserializeProperty<User>(root, "user", options),
            Title = GetString(root, "title")
        });
    }

    private static DialogClearedUpdate ReadDialogCleared(JsonElement root, JsonSerializerOptions options)
    {
        return FillBase(root, new DialogClearedUpdate
        {
            ChatId = GetInt64(root, "chat_id"),
            User = DeserializeProperty<User>(root, "user", options),
            UserLocale = GetString(root, "user_locale")
        });
    }

    private static DialogMutedUpdate ReadDialogMuted(JsonElement root, JsonSerializerOptions options)
    {
        return FillBase(root, new DialogMutedUpdate
        {
            ChatId = GetInt64(root, "chat_id"),
            User = DeserializeProperty<User>(root, "user", options),
            MutedUntil = GetInt64(root, "muted_until"),
            UserLocale = GetString(root, "user_locale")
        });
    }

    private static DialogUnmutedUpdate ReadDialogUnmuted(JsonElement root, JsonSerializerOptions options)
    {
        return FillBase(root, new DialogUnmutedUpdate
        {
            ChatId = GetInt64(root, "chat_id"),
            User = DeserializeProperty<User>(root, "user", options),
            UserLocale = GetString(root, "user_locale")
        });
    }

    private static DialogRemovedUpdate ReadDialogRemoved(JsonElement root, JsonSerializerOptions options)
    {
        return FillBase(root, new DialogRemovedUpdate
        {
            ChatId = GetInt64(root, "chat_id"),
            User = DeserializeProperty<User>(root, "user", options),
            UserLocale = GetString(root, "user_locale")
        });
    }

    private static MessageCallbackUpdate ReadMessageCallback(JsonElement root, JsonSerializerOptions options)
    {
        return FillBase(root, new MessageCallbackUpdate
        {
            Callback = DeserializeProperty<Callback>(root, "callback", options),
            Message = DeserializeProperty<Message>(root, "message", options),
            UserLocale = GetString(root, "user_locale")
        });
    }

    private static MessageCreatedUpdate ReadMessageCreated(JsonElement root, JsonSerializerOptions options)
    {
        return FillBase(root, new MessageCreatedUpdate
        {
            Message = DeserializeProperty<Message>(root, "message", options),
            UserLocale = GetString(root, "user_locale")
        });
    }

    private static MessageEditedUpdate ReadMessageEdited(JsonElement root, JsonSerializerOptions options)
    {
        return FillBase(root, new MessageEditedUpdate
        {
            Message = DeserializeProperty<Message>(root, "message", options)
        });
    }

    private static MessageRemovedUpdate ReadMessageRemoved(JsonElement root)
    {
        return FillBase(root, new MessageRemovedUpdate
        {
            MessageId = GetString(root, "message_id") ?? string.Empty,
            ChatId = GetInt64(root, "chat_id"),
            UserId = GetInt64(root, "user_id")
        });
    }

    private static UserAddedToChatUpdate ReadUserAdded(JsonElement root, JsonSerializerOptions options)
    {
        return FillBase(root, new UserAddedToChatUpdate
        {
            ChatId = GetInt64(root, "chat_id"),
            User = DeserializeProperty<User>(root, "user", options),
            InviterId = GetNullableInt64(root, "inviter_id"),
            IsChannel = GetBoolean(root, "is_channel")
        });
    }

    private static UserRemovedFromChatUpdate ReadUserRemoved(JsonElement root, JsonSerializerOptions options)
    {
        return FillBase(root, new UserRemovedFromChatUpdate
        {
            ChatId = GetInt64(root, "chat_id"),
            User = DeserializeProperty<User>(root, "user", options),
            AdminId = GetNullableInt64(root, "admin_id"),
            IsChannel = GetBoolean(root, "is_channel")
        });
    }

    private static T FillBase<T>(JsonElement root, T update)
        where T : Update
    {
        update.UpdateType = GetRequiredString(root, "update_type");
        update.Timestamp = GetInt64(root, "timestamp");
        return update;
    }

    private static string GetRequiredString(JsonElement root, string propertyName)
    {
        return GetString(root, propertyName)
            ?? throw new JsonException($"В объекте Update отсутствует обязательное поле {propertyName}.");
    }

    private static string? GetString(JsonElement root, string propertyName)
    {
        if (!root.TryGetProperty(propertyName, out var property) || property.ValueKind is JsonValueKind.Null or JsonValueKind.Undefined)
            return null;

        return property.GetString();
    }

    private static long GetInt64(JsonElement root, string propertyName)
    {
        if (!root.TryGetProperty(propertyName, out var property) || property.ValueKind is JsonValueKind.Null or JsonValueKind.Undefined)
            return default;

        return property.GetInt64();
    }

    private static long? GetNullableInt64(JsonElement root, string propertyName)
    {
        if (!root.TryGetProperty(propertyName, out var property) || property.ValueKind is JsonValueKind.Null or JsonValueKind.Undefined)
            return null;

        return property.GetInt64();
    }

    private static bool GetBoolean(JsonElement root, string propertyName)
    {
        if (!root.TryGetProperty(propertyName, out var property) || property.ValueKind is JsonValueKind.Null or JsonValueKind.Undefined)
            return default;

        return property.GetBoolean();
    }

    private static T? DeserializeProperty<T>(JsonElement root, string propertyName, JsonSerializerOptions options)
    {
        if (!root.TryGetProperty(propertyName, out var property) || property.ValueKind is JsonValueKind.Null or JsonValueKind.Undefined)
            return default;

        return property.Deserialize<T>(options);
    }

    private static void WriteBase(Utf8JsonWriter writer, Update value)
    {
        writer.WriteString("update_type", value.UpdateType);
        writer.WriteNumber("timestamp", value.Timestamp);
    }

    private static void WriteChatUserFields(Utf8JsonWriter writer, long chatId, User? user, JsonSerializerOptions options)
    {
        writer.WriteNumber("chat_id", chatId);
        WriteObject(writer, "user", user, options);
    }

    private static void WriteChatUserChannelFields(Utf8JsonWriter writer, long chatId, User? user, bool isChannel, JsonSerializerOptions options)
    {
        WriteChatUserFields(writer, chatId, user, options);
        writer.WriteBoolean("is_channel", isChannel);
    }

    private static void WriteObject<T>(Utf8JsonWriter writer, string propertyName, T? value, JsonSerializerOptions options)
    {
        writer.WritePropertyName(propertyName);

        if (value is null)
        {
            writer.WriteNullValue();
            return;
        }

        JsonSerializer.Serialize(writer, value, options);
    }

    private static void WriteStringIfNotNull(Utf8JsonWriter writer, string propertyName, string? value)
    {
        if (value is not null)
            writer.WriteString(propertyName, value);
    }

    private static void WriteNullableNumber(Utf8JsonWriter writer, string propertyName, long? value)
    {
        if (value.HasValue)
            writer.WriteNumber(propertyName, value.Value);
        else
            writer.WriteNull(propertyName);
    }
}
