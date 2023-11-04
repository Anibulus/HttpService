using System.Net;

public class HttpResponse<T>
{
    public HttpResponse(bool isSuccess, HttpStatusCode? statusCode, T? data)
    {
        IsSuccess = isSuccess;
        StatusCode = statusCode;
        Headers = new Dictionary<string, IEnumerable<string>>();
        Data = data;
    }

    public HttpResponse(bool isSuccess, Dictionary<string, IEnumerable<string>> headers, HttpStatusCode? statusCode, T? data)
    {
        IsSuccess = isSuccess;
        Headers = headers;
        StatusCode = statusCode;
        Data = data;
    }

    public bool IsSuccess { get; }
    public Dictionary<string, IEnumerable<string>> Headers { get; }
    public HttpStatusCode? StatusCode { get; }
    public T? Data { get; }
}