using System.Text;
using System.Text.Json;
using System.Web;
using System.Net.Http.Headers;
using MAX.Bot.Exceptions;
using MAX.Bot.Interfaces;
using MAX.Bot.Interfaces.Models;
using MAX.Bot.Interfaces.Models.Request;
using MAX.Bot.Interfaces.Models.Request.Message;
using MAX.Bot.Interfaces.Models.Response;

namespace MAX.Bot;

public class MaxBotClient : IMaxBotClient
{
    private readonly HttpClient _httpClient;
    private readonly string _baseUrl = "https://platform-api.max.ru";

    public CancellationToken GlobalCancelToken { get; }

    public MaxBotClient(string token, HttpClient httpClient, CancellationToken cancellationToken = default)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));

        _httpClient.BaseAddress = new Uri(_baseUrl);

        if (!_httpClient.DefaultRequestHeaders.Contains("Authorization"))
            _httpClient.DefaultRequestHeaders.Add("Authorization", token);

        GlobalCancelToken = cancellationToken;
    }

    public MaxBotClient(string token, int timeoutSeconds = 30, CancellationToken cancellationToken = default)
    {
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri(_baseUrl),
            Timeout = TimeSpan.FromSeconds(timeoutSeconds)
        };

        _httpClient.DefaultRequestHeaders.Add("Authorization", token);

        GlobalCancelToken = cancellationToken;
    }

    private async Task<T> SendRequestAsync<T>(HttpMethod method, string endpoint, object? data = null,
        CancellationToken cancellationToken = default)
    {
        var requestUri = new Uri(_httpClient.BaseAddress!, endpoint);
        var request = new HttpRequestMessage(method, requestUri);

        if (data != null && (method == HttpMethod.Post || method == HttpMethod.Put))
        {
            var json = JsonSerializer.Serialize(data, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
            });
            request.Content = new StringContent(json, Encoding.UTF8, "application/json");
        }

        using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, GlobalCancelToken);
        var response = await _httpClient.SendAsync(request, cts.Token);
        var responseContent = await response.Content.ReadAsStringAsync(cts.Token);

        if (!response.IsSuccessStatusCode)
        {
            throw new MaxBotClientException(
                $"HTTP {response.StatusCode}: {responseContent}",
                response.StatusCode);
        }

        return JsonSerializer.Deserialize<T>(responseContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        }) ?? throw new InvalidOperationException("Не удалось десериализовать ответ");
    }

    public async Task<User> GetMeAsync(CancellationToken cancellationToken = default)
    {
        return await SendRequestAsync<User>(
            HttpMethod.Get, "/me", null, cancellationToken);
    }

    public async Task<SendMessageResponse> SendMessageAsync(SendMessageRequest request, CancellationToken cancellationToken = default)
    {
        var queryParams = new List<string>();

        if (request.UserId.HasValue)
            queryParams.Add($"user_id={request.UserId.Value}");

        if (request.ChatId.HasValue)
            queryParams.Add($"chat_id={request.ChatId.Value}");

        if (request.DisableLinkPreview.HasValue)
            queryParams.Add($"disable_link_preview={request.DisableLinkPreview.Value.ToString().ToLower()}");

        var queryString = queryParams.Any() ? $"?{string.Join("&", queryParams)}" : "";

        return await SendRequestAsync<SendMessageResponse>(
            HttpMethod.Post, $"/messages{queryString}", request, cancellationToken);
    }

    public async Task<GetMessagesResponse> GetMessagesAsync(GetMessagesRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        request.Validate();

        var queryParams = new Dictionary<string, string>();

        if (request.ChatId.HasValue)
            queryParams["chat_id"] = request.ChatId.Value.ToString();

        if (request.MessageIds is { Count: > 0 })
            queryParams["message_ids"] = string.Join(",", request.MessageIds);

        if (request.Count.HasValue)
            queryParams["count"] = request.Count.Value.ToString();

        if (request.From.HasValue)
            queryParams["from"] = request.From.Value.ToString();

        if (request.To.HasValue)
            queryParams["to"] = request.To.Value.ToString();

        var queryString = queryParams.Any()
            ? "?" + string.Join("&", queryParams.Select(kvp => $"{kvp.Key}={HttpUtility.UrlEncode(kvp.Value)}"))
            : "";

        return await SendRequestAsync<GetMessagesResponse>(HttpMethod.Get, $"/messages{queryString}", null, cancellationToken);
    }

    public async Task<Message> GetMessageByIdAsync(string messageId, CancellationToken cancellationToken = default)
    {
        return await SendRequestAsync<Message>(HttpMethod.Get, $"/messages/{messageId}", null, cancellationToken);
    }

    public async Task<VideoInfoResponse> GetVideoAsync(string videoToken, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(videoToken))
            throw new ArgumentException("Токен видео-вложения не должен быть пустым.", nameof(videoToken));

        if (!IsVideoTokenValid(videoToken))
            throw new ArgumentException("Токен видео-вложения может содержать только латинские буквы, цифры, '_' и '-'.", nameof(videoToken));

        return await SendRequestAsync<VideoInfoResponse>(
            HttpMethod.Get,
            $"/videos/{Uri.EscapeDataString(videoToken)}",
            null,
            cancellationToken);
    }

    public async Task<BaseResponse> EditMessageByIdAsync(string messageId, SendMessageRequest messageRequest, CancellationToken cancellationToken = default)
    {
        return await SendRequestAsync<BaseResponse>(HttpMethod.Put, $"/messages?message_id={messageId}", messageRequest, cancellationToken);
    }

    public async Task<BaseResponse> DeleteMessageByIdAsync(string messageId, CancellationToken cancellationToken = default)
    {
        return await SendRequestAsync<BaseResponse>(HttpMethod.Delete, $"/messages?message_id={messageId}", null, cancellationToken);
    }

    public async Task<GetChatsResponse> GetChatsAsync(GetChatsRequest request, CancellationToken cancellationToken = default)
    {
        var queryParams = new Dictionary<string, string>();

        if (request.Count.HasValue)
            queryParams["count"] = request.Count.Value.ToString();

        if (request.Marker.HasValue)
            queryParams["marker"] = request.Marker.Value.ToString();

        var queryString = queryParams.Any()
            ? "?" + string.Join("&", queryParams.Select(kvp => $"{kvp.Key}={HttpUtility.UrlEncode(kvp.Value)}"))
            : "";

        return await SendRequestAsync<GetChatsResponse>(HttpMethod.Get, $"/chats{queryString}", null, cancellationToken);
    }

    public async Task<GetChatMembersResponse> GetChatMembersAsync(GetChatMembersRequest request, CancellationToken cancellationToken = default)
    {
        var queryParams = new Dictionary<string, string>();

        if (request.UserIds != null)
            queryParams["message_ids"] = string.Join(",", request.UserIds!);

        if (request.Marker.HasValue)
            queryParams["marker"] = request.Marker.Value.ToString();

        if (request.Count.HasValue)
            queryParams["count"] = request.Count.Value.ToString();

        var queryString = queryParams.Any()
            ? "?" + string.Join("&", queryParams.Select(kvp => $"{kvp.Key}={HttpUtility.UrlEncode(kvp.Value)}"))
            : "";

        return await SendRequestAsync<GetChatMembersResponse>(
            HttpMethod.Get,
            $"/chats/{request.ChatId.ToString()}/members{queryString}",
            null,
            cancellationToken
        );
    }

    public async Task<BaseResponse> DeleteChatMemberAsync(DeleteChatMemberRequest request, CancellationToken cancellationToken = default)
    {
        var queryParams = new Dictionary<string, string>();

        if (request?.UserId != null)
            queryParams["user_id"] = request.UserId.ToString();

        if (request?.Block != null && request.Block.HasValue)
            queryParams["block"] = request.Block.Value.ToString();

        var queryString = queryParams.Any()
            ? "?" + string.Join("&", queryParams.Select(kvp => $"{kvp.Key}={HttpUtility.UrlEncode(kvp.Value)}"))
            : "";

        return await SendRequestAsync<BaseResponse>(
            HttpMethod.Delete,
            $"/chats/{request?.ChatId.ToString()}/members{queryString}",
            null,
            cancellationToken
        );
    }

    public async Task<BaseResponse> AddChatMemberAsync(AddChatMemberRequest request, CancellationToken cancellationToken = default)
    {
        return await SendRequestAsync<BaseResponse>(
            HttpMethod.Post,
            $"/chats/{request.ChatId.ToString()}/members",
            request,
            cancellationToken
        );
    }

    public async Task<string> UploadsAsync(UploadRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        request.Validate();

        var uploadType = request.GetUploadTypeValue();
        var uploadInfo = await SendRequestAsync<UploadResponse>(
            HttpMethod.Post,
            $"/uploads?type={HttpUtility.UrlEncode(uploadType)}",
            null,
            cancellationToken);

        if (string.IsNullOrWhiteSpace(uploadInfo.Url))
            throw new InvalidOperationException("API MAX вернул пустой URL для загрузки файла");

        var uploadUri = CreateUploadUri(uploadInfo.Url);
        var uploadResponse = await UploadFileAsync(uploadUri, request, cancellationToken);
        return ResolveUploadToken(request, uploadInfo, uploadResponse);
    }

    public async Task<GetUpdatesResponse> GetUpdatesAsync(
        GetUpdatesRequest request,
        CancellationToken cancellationToken = default)
    {
        var queryParams = new Dictionary<string, string>();

        if (request.Limit.HasValue)
            queryParams["limit"] = request.Limit.Value.ToString();

        if (request.Timeout.HasValue)
            queryParams["timeout"] = request.Timeout.Value.ToString();

        if (request.Marker.HasValue)
            queryParams["marker"] = request.Marker.Value.ToString();

        if (request.Types != null && request.Types.Count != 0)
            queryParams["types"] = string.Join(",", request.Types);

        var queryString = queryParams.Any()
            ? "?" + string.Join("&", queryParams.Select(kvp => $"{kvp.Key}={HttpUtility.UrlEncode(kvp.Value)}"))
            : "";

        return await SendRequestAsync<GetUpdatesResponse>(
            HttpMethod.Get,
            $"/updates{queryString}",
            null,
            cancellationToken);
    }

    public async Task<BaseResponse> SubscribeAsync(
        SubscriptionRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        request.Validate();

        return await SendRequestAsync<BaseResponse>(
            HttpMethod.Post,
            "/subscriptions",
            request,
            cancellationToken);
    }

    public async Task PollUpdatesWithCallback(
        Func<Update, IMaxBotClient, Task> callback,
        int? limit = 100,
        int? timeout = 30,
        long? marker = null,
        List<string>? types = null,
        CancellationToken cancellationToken = default)
    {
        long? currentMarker = marker;

        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                var response = await GetUpdatesAsync(new GetUpdatesRequest
                {
                    Limit = limit,
                    Timeout = timeout,
                    Marker = currentMarker,
                    Types = types
                }, cancellationToken);

                foreach (var update in response.Updates)
                {
                    await callback.Invoke(update, this);
                }

                currentMarker = response.Marker;
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
                await Task.Delay(5000, cancellationToken);
            }
        }
    }

    private async Task<UploadResponse> UploadFileAsync(
        Uri uploadUri,
        UploadRequest request,
        CancellationToken cancellationToken)
    {
        using var fileContent = new StreamContent(new NonDisposingStream(request.Content!));

        if (!string.IsNullOrWhiteSpace(request.ContentType))
            fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse(request.ContentType);

        using var multipartContent = new MultipartFormDataContent();
        multipartContent.Add(fileContent, "data", request.FileName);

        using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, GlobalCancelToken);
        var response = await _httpClient.PostAsync(uploadUri, multipartContent, cts.Token);
        var responseContent = await response.Content.ReadAsStringAsync(cts.Token);

        if (!response.IsSuccessStatusCode)
        {
            throw new MaxBotClientException(
                $"HTTP {response.StatusCode}: {responseContent}",
                response.StatusCode);
        }

        if (string.IsNullOrWhiteSpace(responseContent))
            return new UploadResponse();

        if (!IsJsonResponse(responseContent))
        {
            if (request.Type is UploadType.Video or UploadType.Audio)
                return new UploadResponse();

            throw new InvalidOperationException(
                $"API MAX вернул не JSON-ответ после загрузки файла: {CreateResponsePreview(responseContent)}");
        }

        return JsonSerializer.Deserialize<UploadResponse>(responseContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        }) ?? throw new InvalidOperationException("Не удалось десериализовать ответ загрузки файла");
    }

    private static Uri CreateUploadUri(string url)
    {
        if (!Uri.TryCreate(url, UriKind.Absolute, out var uploadUri) ||
            (uploadUri.Scheme != Uri.UriSchemeHttps && uploadUri.Scheme != Uri.UriSchemeHttp))
        {
            throw new InvalidOperationException("API MAX вернул некорректный URL для загрузки файла");
        }

        return uploadUri;
    }

    private static string ResolveUploadToken(
        UploadRequest request,
        UploadResponse uploadInfo,
        UploadResponse uploadResponse)
    {
        var token = request.Type switch
        {
            UploadType.Video or UploadType.Audio => uploadInfo.Token ?? uploadResponse.Token,
            UploadType.Image or UploadType.File => uploadResponse.Token ?? uploadInfo.Token,
            _ => null
        };

        if (!string.IsNullOrWhiteSpace(token))
            return token;

        var expectedStep = request.Type is UploadType.Video or UploadType.Audio
            ? "первого шага загрузки video/audio"
            : "ответа загрузки image/file";

        throw new InvalidOperationException($"API MAX не вернул token из {expectedStep}.");
    }

    private static bool IsJsonResponse(string responseContent)
    {
        var trimmed = responseContent.TrimStart();
        return trimmed.StartsWith('{') || trimmed.StartsWith('[');
    }

    private static bool IsVideoTokenValid(string videoToken)
    {
        return videoToken.All(c => char.IsAsciiLetterOrDigit(c) || c is '_' or '-');
    }

    private static string CreateResponsePreview(string responseContent)
    {
        const int maxLength = 200;

        var normalized = responseContent
            .Replace("\r", string.Empty)
            .Replace("\n", " ");

        return normalized.Length <= maxLength
            ? normalized
            : normalized[..maxLength] + "...";
    }

    private sealed class NonDisposingStream : Stream
    {
        private readonly Stream _inner;

        public NonDisposingStream(Stream inner)
        {
            _inner = inner;
        }

        public override bool CanRead => _inner.CanRead;
        public override bool CanSeek => _inner.CanSeek;
        public override bool CanWrite => _inner.CanWrite;
        public override long Length => _inner.Length;

        public override long Position
        {
            get => _inner.Position;
            set => _inner.Position = value;
        }

        public override void Flush()
        {
            _inner.Flush();
        }

        public override Task FlushAsync(CancellationToken cancellationToken)
        {
            return _inner.FlushAsync(cancellationToken);
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return _inner.Read(buffer, offset, count);
        }

        public override ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
        {
            return _inner.ReadAsync(buffer, cancellationToken);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return _inner.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            _inner.SetLength(value);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            _inner.Write(buffer, offset, count);
        }

        public override ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default)
        {
            return _inner.WriteAsync(buffer, cancellationToken);
        }

        protected override void Dispose(bool disposing)
        {
        }
    }
}
