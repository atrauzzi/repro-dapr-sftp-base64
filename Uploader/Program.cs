using System.Text;
using System.Text.Json;
using Dapr.Client;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDaprClient();

var app = builder.Build();

await app.StartAsync();

var daprClient = app.Services.GetRequiredService<DaprClient>();

await daprClient.WaitForSidecarAsync();

do
{
    var request = new DaprSftpRequest
    {
        operation = "create",
        data = Convert.ToBase64String(Encoding.UTF8.GetBytes("Test")),
        metadata = new Dictionary<string, string>
        {
            { "fileName", $"/{Guid.CreateVersion7().ToString()}" },
        },
    };

    var json = JsonSerializer.Serialize(request, new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    });

    Console.WriteLine("Sending request...");
    await daprClient.InvokeBindingAsync("sftp", "create", request.data, request.metadata.AsReadOnly());
    
    await Task.Delay(2000);

} while (true);

public class DaprSftpRequest
{
    public required string operation { get; init; }
    public required string data { get; init; }
    public IDictionary<string, string> metadata { get; init; } = new Dictionary<string, string>();
}
