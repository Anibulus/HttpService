using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using DTO;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
//using Newtonsoft.Json;

namespace Service;

public class HttpService
{
    private readonly ILogger<HttpService> _logger;

    public HttpService(ILoggerFactory logger)
    {
        _logger = logger.CreateLogger<HttpService>();
    }

    private readonly Dictionary<string, string> authorize =
        new() { { "default", "Bearer" } };





    private void LoadHeaders(ref HttpClient client, Dictionary<string, string>? headers = null)
    {
        // Add optional headers
        foreach (var header in headers ?? new())
            client.DefaultRequestHeaders.Add(header.Key, header.Value);
    }

    private void LoadParams(ref string url, Dictionary<string, string>? @params = null)
    {
        if (@params is not null)
            url = QueryHelpers.AddQueryString(url, (IDictionary<string, string?>)@params);
    }

    private void LoadAccessToken(ref HttpClient client, string authorization, string? accessToken = null)
    {
        if (!string.IsNullOrEmpty(accessToken))
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                authorize[authorization],
                accessToken
            );
    }

    private void SetMediaAccept<TRequest>(ref HttpClient client, string? accept = null)
    {
       string mediaType = typeof(TRequest) switch
            {
                //Type t when t == typeof(StreamContent) => "image/jpeg",
                //Type t when t == typeof(FormContent) => "x-www-form-urlencoded",
                _ => "application/json"
            };
            mediaType = (accept != null) ? accept : mediaType;
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType));
    }


    public async Task<HttpResponse<TResponse?>> SendRequest<TRequest, TResponse>(
        string url,
        HttpMethod httpMethod,
        TRequest? requestBody = default,
        string authorization = "default",
        string? accessToken = null,
        Dictionary<string, string>? headers = null,
        Dictionary<string, string>? @params = null,
        string? accept = null
    )
    {
        var client = new HttpClient
        {
            Timeout = TimeSpan.FromMinutes(60)
        };

        try
        {
            SetMediaAccept<TRequest>(ref client, accept);
            LoadHeaders(ref client, headers);
            LoadParams(ref url, @params);
            LoadAccessToken(ref client, authorization, accessToken);

            // Convert HttpMethod to the corresponding method string
            var method = httpMethod.Method;
            var request = new HttpRequestMessage(new HttpMethod(method), url);

            // Add request body if applicable (for POST, PUT, PATCH)
            if (
                new HashSet<HttpMethod>
                {
                    HttpMethod.Post,
                    HttpMethod.Put,
                    HttpMethod.Patch
                }.Contains(httpMethod)
            )
            {
                if (requestBody is not null)
                {
                    if (typeof(TRequest) == typeof(MemoryStream))
                    {
                        using (var stream = new MemoryStream((byte[])(object)requestBody))
                        {
                            request.Content = new StreamContent(stream);
                            request.Content.Headers.ContentType = new MediaTypeHeaderValue(
                                "image/jpeg"
                            );
                        }
                    }
                    else if (typeof(TRequest) == typeof(FormContent<string, string>))
                    {
                        request.Content = new FormUrlEncodedContent(
                            (FormContent<string, string>)(object)requestBody
                        );
                    }
                    else if (typeof(TRequest) == typeof(HttpRequestMessage))
                    {
                        request = (HttpRequestMessage)(object)requestBody;
                    }
                    else
                    {
                        request.Content = new StringContent(
                            JsonSerializer.Serialize(requestBody),
                            Encoding.UTF8,
                            "application/json"
                        );
                    }
                }
            }

            // Send the request to the API and get the response
            var response = await client.SendAsync(request);

            return await ProcessResponse<TResponse>(response);
        }
        catch
        {
            return new HttpResponse<TResponse?>(false, null, default);
        }
    }


    private async Task<HttpResponse<TResponse?>> ProcessResponse<TResponse>(HttpResponseMessage response)
    {
        var responseContent = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
            return new HttpResponse<TResponse?>(false, response.Headers.ToDictionary(h => h.Key, h => h.Value), response.StatusCode, default);

        _logger.LogInformation($"Response status code: {response.StatusCode}");
        _logger.LogInformation($"Response {response.Content}");

        TResponse? responseObject = default;
        if (typeof(TResponse?) == typeof(string))
            responseObject = (TResponse?)(object)responseContent;
        else if (typeof(TResponse) == typeof(byte[]))
            responseObject = (TResponse?)
                (object)(await response.Content.ReadAsByteArrayAsync());
        else if (typeof(TResponse) == typeof(Stream))
            responseObject = (TResponse?)
                (object)(await response.Content.ReadAsStreamAsync());
        else
            responseObject = JsonSerializer.Deserialize<TResponse?>(responseContent);

        return new HttpResponse<TResponse?>(true, response.Headers.ToDictionary(h => h.Key, h => h.Value), response.StatusCode, responseObject);
    }
}
