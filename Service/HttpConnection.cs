namespace Service;
public class HttpConnection
{
    private readonly HttpService _httpService;
    public HttpConnection(HttpService httpService)
    {
        _httpService = httpService;
    }

     public async Task<HttpResponse<TResponse?>> SendGetRequest<TResponse>(
        string url,
        string authorization = "default",
        string? accessToken = null,
        Dictionary<string, string>? headers = null,
        Dictionary<string, string>? @params = null,
        string? accept = null
    )
    {
        return await _httpService.SendRequest<object, TResponse>(
            url,
            HttpMethod.Get,
            requestBody: null,
            authorization,
            accessToken,
            headers,
            @params,
            accept
        );
    }

    public async Task<HttpResponse<TResponse?>> SendPostRequest<TRequest, TResponse>(
        string url,
        TRequest request,
        string authorization = "default",
        string? accessToken = null,
        Dictionary<string, string>? headers = null,
        Dictionary<string, string>? @params = null
    )
    {
        return await _httpService.SendRequest<TRequest, TResponse>(
            url,
            HttpMethod.Post,
            requestBody: request,
            authorization,
            accessToken,
            headers,
            @params
        );
    }

    public async Task<HttpResponse<TResponse?>> SendPatchRequest<TRequest, TResponse>(
        string url,
        TRequest request,
        string authorization = "default",
        string? accessToken = null,
        Dictionary<string, string>? headers = null,
        Dictionary<string, string>? @params = null
    )
    {
        return await _httpService.SendRequest<TRequest, TResponse>(
            url,
            HttpMethod.Patch,
            requestBody: request,
            authorization,
            accessToken,
            headers,
            @params
        );
    }

    public async Task<HttpResponse<TResponse?>> SendPutRequest<TRequest, TResponse>(
        string url,
        TRequest request,
        string authorization = "default",
        string? accessToken = null,
        Dictionary<string, string>? headers = null,
        Dictionary<string, string>? @params = null
    )
    {
        return await _httpService.SendRequest<TRequest, TResponse>(
            url,
            HttpMethod.Put,
            requestBody: request,
            authorization,
            accessToken,
            headers,
            @params
        );
    }

    public async Task<HttpResponse<TResponse?>> SendDeleteRequest<TResponse>(
        string url,
        string authorization = "default",
        string? accessToken = null,
        Dictionary<string, string>? headers = null,
        Dictionary<string, string>? @params = null
    )
    {
        return await _httpService.SendRequest<object, TResponse>(
            url,
            HttpMethod.Delete,
            requestBody: null,
            authorization,
            accessToken,
            headers,
            @params
        );
    }
}