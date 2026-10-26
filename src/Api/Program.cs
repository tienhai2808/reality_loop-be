using Api;
using Application;
using Infrastructure;
using Infrastructure.Persistence.Seeding.Audio;

const string initializeAudioArgument = "--initialize-foundation-audio";
var initializeAudio = args.Contains(initializeAudioArgument, StringComparer.Ordinal);
var builder = WebApplication.CreateBuilder(args.Where(arg => arg != initializeAudioArgument).ToArray());
if (initializeAudio)
{
    builder.Services.AddFoundationAudioInitialization(builder.Configuration);
    await using var initializerApp = builder.Build();
    using var cancellation = new CancellationTokenSource(TimeSpan.FromMinutes(15));
    Console.CancelKeyPress += (_, eventArgs) =>
    {
        eventArgs.Cancel = true;
        cancellation.Cancel();
    };
    await using var scope = initializerApp.Services.CreateAsyncScope();
    await scope.ServiceProvider.GetRequiredService<FoundationAudioInitializer>()
        .InitializeAsync(cancellation.Token);
    return;
}

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApi(builder.Configuration);

var app = builder.Build();
app.UseApi();
app.Run();

public partial class Program;
