using Service;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddTransient<HttpConnection>();
builder.Services.AddTransient<HttpService>();

var app = builder.Build();

var httpService = app.Services.GetService<HttpConnection>();
var response = await httpService.SendGetRequest<string>(url:"https://google.com/");
Console.WriteLine(response.Data);

app.MapGet("/", () => "Hello World!");

app.Run();
