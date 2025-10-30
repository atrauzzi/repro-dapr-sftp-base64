using System.Reflection;
using CommunityToolkit.Aspire.Hosting.Dapr;
using Projects;

var executingPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)
    ?? throw new("Where am I?");

var builder = DistributedApplication.CreateBuilder(args);

var sftp = builder
    // see: https://hub.docker.com/r/southrivertech/titansftp
    .AddContainer("sftp", "southrivertech/titansftp")
    .WithVolume("sftp-data", "/var/southriver")
    .WithHttpsEndpoint(22001, targetPort: 41443)
    .WithEndpoint(22002, targetPort: 22, name: "sftp");

builder
    .AddProject<Uploader>("uploader")
    .WaitFor(sftp)
    .WithDaprSidecar(new DaprSidecarOptions
    {
        LogLevel = "debug",
        ResourcesPaths = [ Path.Join(executingPath, "resources") ],
    });

builder.Build().Run();
