using System.Text;
using Dapr.Client;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDaprClient();

var app = builder.Build();

await app.StartAsync();

var daprClient = app.Services.GetRequiredService<DaprClient>();

await daprClient.WaitForSidecarAsync();

Console.WriteLine("Sending requests...");

// note: string
await daprClient.InvokeBindingAsync("sftp", "create", "Test", new Dictionary<string, string>
{
    { "fileName", $"/{Guid.CreateVersion7().ToString()}-string" },
});

// note: Base64
await daprClient.InvokeBindingAsync("sftp", "create", Convert.ToBase64String(Encoding.UTF8.GetBytes("Test")), new Dictionary<string, string>
{
    { "fileName", $"/{Guid.CreateVersion7().ToString()}-base64" },
});

// note: byte[]
await daprClient.InvokeBindingAsync("sftp", "create", Encoding.UTF8.GetBytes("Test"), new Dictionary<string, string>
{
    { "fileName", $"/{Guid.CreateVersion7().ToString()}-bytes" },
});

// note: Streams not supported
// await daprClient.InvokeBindingAsync("sftp", "create", new MemoryStream(Encoding.UTF8.GetBytes("Test")), new Dictionary<string, string>
// {
//     { "fileName", $"/{Guid.CreateVersion7().ToString()}-stream" },
// });
