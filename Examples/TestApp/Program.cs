using MAX.Bot.Exceptions;
using MAX.Bot.Extensions;
using MAX.Bot.Interfaces;
using MAX.Bot.Interfaces.Models;
using MAX.Bot.Interfaces.Models.Request;
using MAX.Bot.Interfaces.Models.Request.Message;
using MAX.Bot.Interfaces.Models.Request.Message.Attachment;
using MAX.Bot.Interfaces.Models.Response;
using Microsoft.Extensions.DependencyInjection;
using Attachment = MAX.Bot.Interfaces.Models.Request.Message.Attachment.Attachment;

const string C_BOT_API = "";
const long C_TEST_CHAT_ID = -70581633278133;
const long C_TEST_USER_ID = 168973682;
const string C_TEST_WEBHOOK_URL = "https://your-domain.com/webhook";

var services = new ServiceCollection();
services.AddMaxBotClient(C_BOT_API, 30);

try
{
    var serviceProvider = services.BuildServiceProvider();
    var maxApiClient = serviceProvider.GetRequiredService<IMaxBotClient>();

    Console.WriteLine("Вызываем GetMeAsync...");
    var me = await maxApiClient.GetMeAsync();
    Console.WriteLine($"Успех! Бот: {me.FirstName} (ID: {me.Id})");

    Console.WriteLine("Вызываем SendMessageAsync...");
    await maxApiClient.SendMessageAsync(new SendMessageRequest()
    {
        ChatId = C_TEST_CHAT_ID,
        Text = "Отправка сообщения",
        Format = MessageFormat.Markdown,
    });

    Console.WriteLine("Вызываем SendMessageAsync-Attachment-InlineKeyboardPayload...");
    await maxApiClient.SendMessageAsync(new SendMessageRequest()
    {
        ChatId = C_TEST_CHAT_ID,
        Text = "Отправка сообщения с клавиатурой",
        Format = MessageFormat.Markdown,
        Attachments = new List<Attachment>
        {
            new InlineKeyboardAttachment
            {
                Payload = new InlineKeyboardPayload()
                {
                    Buttons = new List<List<Button>>()
                    {
                        // --- Ряд 1: Две кнопки ---
                        new List<Button>
                        {
                            new LinkButton
                            {
                                Text = "Открыть сайт",
                                Url = "https://saasoft.ru"
                            },
                            new CallbackButton
                            {
                                Text = "Подтвердить",
                                Payload = "confirm_action"
                            }
                        },
                        // --- Ряд 2: Одна большая кнопка ---
                        new List<Button>
                        {
                            new RequestGeoButton
                            {
                                Text = "Отправить геолокацию",
                                Quick = true
                            }
                        },
                        // --- Ряд 3: Одна большая кнопка ---
                        new List<Button>
                        {
                            new MessageButton()
                            {
                                Text = "Отправить текст",
                            }
                        }
                    }
                }
            }
        }
    });

    Console.WriteLine("Вызываем GetMessagesAsync...");
    var response = await maxApiClient.GetMessagesAsync(new GetMessagesRequest()
    {
        ChatId = C_TEST_CHAT_ID,
        Count = 50,
    });
    Console.WriteLine($"Получено {response?.Messages?.Length} сообщений:");

    if (response?.Messages is { Length: > 0 } messages)
    {
        var lastMessageId = messages.LastOrDefault()?.Body?.Mid;
        if (!string.IsNullOrWhiteSpace(lastMessageId))
        {
            Console.WriteLine("Вызываем GetMessageByIdAsync...");
            var responseById = await maxApiClient.GetMessageByIdAsync(lastMessageId);
            Console.WriteLine($"Получено {responseById?.Body?.Text}:");

            Console.WriteLine("Вызываем EditMessageByIdAsync...");
            var responseEdit = await maxApiClient.EditMessageByIdAsync(lastMessageId, new SendMessageRequest()
            {
                Text = "Изменил ТЕКСТ !!!",
                Format = MessageFormat.Markdown,
            });
            Console.WriteLine($"Изменено {responseEdit?.Success}:");
        }

        var firstMessageId = messages.FirstOrDefault()?.Body?.Mid;
        if (!string.IsNullOrWhiteSpace(firstMessageId))
        {
            Console.WriteLine("Вызываем DeleteMessageByIdAsync...");
            var responseDelete = await maxApiClient.DeleteMessageByIdAsync(firstMessageId);
            Console.WriteLine($"Удалено {responseDelete?.Success}:");
        }
    }

    Console.WriteLine("Вызываем GetChatsAsync...");
    var responseChats = await maxApiClient.GetChatsAsync(new GetChatsRequest()
    {
        Count = 1,
        Marker = null,
    });
    Console.WriteLine($"Получено {responseChats?.Chats?.Length} чатов:");

    Console.WriteLine("Вызываем GetChatMembersAsync...");
    var responseChatMembers = await maxApiClient.GetChatMembersAsync(new GetChatMembersRequest()
    {
        ChatId = C_TEST_CHAT_ID,
    });
    Console.WriteLine($"Получено {responseChatMembers?.Members?.Length} пользователей:");

    Console.WriteLine("Вызываем AddChatMemberAsync...");
    var isAdded = await maxApiClient.AddChatMemberAsync(new AddChatMemberRequest()
    {
        ChatId = C_TEST_CHAT_ID,
        UserIds = [C_TEST_USER_ID],
    });

    if (isAdded != null && isAdded.Success)
    {
        Console.WriteLine("Пользователь успешно добавлен в чат");
    }

    Console.WriteLine("Вызываем DeleteChatMemberAsync...");
    var isDeleted = await maxApiClient.DeleteChatMemberAsync(new DeleteChatMemberRequest()
    {
        ChatId = C_TEST_CHAT_ID,
        UserId = C_TEST_USER_ID,
    });

    if (isDeleted != null && isDeleted.Success)
    {
        Console.WriteLine("Пользователь успешно удален из чата");
    }

    Console.WriteLine("Проверяем GetMessageByIdAsync для тестового ID...");
    var messageId = response?.Messages?.Last()?.Body?.Mid;
    if (messageId != null)
    {
        var responseMessage = await maxApiClient.GetMessageByIdAsync(messageId);
        Console.WriteLine($"Получено сообщение по ID: {responseMessage?.Body?.Text}");
    }

    var filesDirectory = Path.Combine(AppContext.BaseDirectory, "Files");
    var textFilePath = Path.Combine(filesDirectory, "test.txt");
    var videoFilePath = Path.Combine(filesDirectory, "video.mp4");

    Console.WriteLine("Вызываем UploadsAsync для test.txt...");
    await using var textFileContent = File.OpenRead(textFilePath);
    var textFileToken = await maxApiClient.UploadsAsync(new UploadRequest()
    {
        Type = UploadType.File,
        Content = textFileContent,
        FileName = Path.GetFileName(textFilePath),
        ContentType = "text/plain",
    });
    Console.WriteLine($"Получен токен загруженного файла: {textFileToken}");

    Console.WriteLine("Отправляем сообщение с файлом test.txt...");
    await SendMessageWithAttachmentRetry(maxApiClient, new SendMessageRequest()
    {
        ChatId = C_TEST_CHAT_ID,
        Text = "Файл test.txt",
        Attachments = new List<Attachment>
        {
            new FileAttachment
            {
                Payload = new FilePayload
                {
                    Token = textFileToken,
                }
            }
        }
    });

    Console.WriteLine("Вызываем UploadsAsync для video.mp4...");
    await using var videoFileContent = File.OpenRead(videoFilePath);
    var videoToken = await maxApiClient.UploadsAsync(new UploadRequest()
    {
        Type = UploadType.Video,
        Content = videoFileContent,
        FileName = Path.GetFileName(videoFilePath),
        ContentType = "video/mp4",
    });
    Console.WriteLine($"Получен токен загруженного видео: {videoToken}");

    Console.WriteLine("Вызываем GetVideoAsync для video.mp4...");
    var videoInfo = await GetVideoInfoWithRetry(maxApiClient, videoToken);
    Console.WriteLine($"Видео: token={videoInfo.Token}, width={videoInfo.Width}, height={videoInfo.Height}, duration={videoInfo.Duration}");

    Console.WriteLine("Отправляем сообщение с видео video.mp4...");
    await SendMessageWithAttachmentRetry(maxApiClient, new SendMessageRequest()
    {
        ChatId = C_TEST_CHAT_ID,
        Text = "Видео video.mp4",
        Attachments = new List<Attachment>
        {
            new VideoAttachment
            {
                Payload = new VideoPayload
                {
                    Token = videoToken,
                }
            }
        }
    });

    Console.WriteLine("Вызываем GetUpdatesAsync...");
    var responseUpdates = await maxApiClient.GetUpdatesAsync(new GetUpdatesRequest()
    {
        Timeout = 2,
    });
    Console.WriteLine($"Маркер: {responseUpdates?.Marker}, Количество обновлений: {responseUpdates?.Updates.Count}");

    if (!string.IsNullOrWhiteSpace(C_TEST_WEBHOOK_URL))
    {
        Console.WriteLine("Вызываем SubscribeAsync...");
        var responseSubscribe = await maxApiClient.SubscribeAsync(new SubscriptionRequest()
        {
            Url = C_TEST_WEBHOOK_URL,
            UpdateTypes = new List<string>
            {
                UpdateTypes.MessageCreated,
                UpdateTypes.MessageCallback,
                UpdateTypes.BotStarted,
            },
            Secret = "test-secret_12345",
        });
        Console.WriteLine($"Подписка создана: {responseSubscribe.Success}, сообщение: {responseSubscribe.Message}");
    }
    else
    {
        Console.WriteLine("SubscribeAsync пропущен: задайте C_TEST_WEBHOOK_URL с публичным HTTPS endpoint.");
    }

    //var _ = maxApiClient.PollUpdatesWithCallback(
    //    async (update, client) =>
    //    {
    //        if (update is MessageCreatedUpdate messageCreated)
    //        {
    //            Console.WriteLine($"Сообщение: {messageCreated.Message?.Body?.Text}");
    //
    //            await client.SendMessageAsync(new SendMessageRequest
    //            {
    //                Text = messageCreated.Message?.Body?.Text,
    //                ChatId = -70581633278133,
    //            });
    //        }
    //    },
    //    limit: 100,
    //    timeout: 90,
    //    types: new List<string> { UpdateTypes.MessageCreated }
    //);
}
catch (Exception ex)
{
    Console.WriteLine($"Ошибка: {ex.Message}");
    Environment.Exit(1);
}

static async Task SendMessageWithAttachmentRetry(IMaxBotClient maxApiClient, SendMessageRequest request)
{
    var retryDelays = new[]
    {
        TimeSpan.FromSeconds(2),
        TimeSpan.FromSeconds(5),
        TimeSpan.FromSeconds(10),
    };

    for (var attempt = 0; ; attempt++)
    {
        try
        {
            await maxApiClient.SendMessageAsync(request);
            return;
        }
        catch (MaxBotClientException ex) when (IsAttachmentNotReady(ex) && attempt < retryDelays.Length)
        {
            var delay = retryDelays[attempt];
            Console.WriteLine($"Вложение еще обрабатывается, повтор через {delay.TotalSeconds:0} сек...");
            await Task.Delay(delay);
        }
    }
}

static bool IsAttachmentNotReady(MaxBotClientException ex)
{
    return ex.Message.Contains("attachment.not.ready", StringComparison.OrdinalIgnoreCase) ||
           ex.Message.Contains("not.processed", StringComparison.OrdinalIgnoreCase);
}

static async Task<VideoInfoResponse> GetVideoInfoWithRetry(IMaxBotClient maxApiClient, string videoToken)
{
    var retryDelays = new[]
    {
        TimeSpan.FromSeconds(2),
        TimeSpan.FromSeconds(5),
        TimeSpan.FromSeconds(10),
    };

    for (var attempt = 0; ; attempt++)
    {
        try
        {
            return await maxApiClient.GetVideoAsync(videoToken);
        }
        catch (MaxBotClientException ex) when (IsAttachmentNotReady(ex) && attempt < retryDelays.Length)
        {
            var delay = retryDelays[attempt];
            Console.WriteLine($"Видео еще обрабатывается, повтор через {delay.TotalSeconds:0} сек...");
            await Task.Delay(delay);
        }
    }
}
