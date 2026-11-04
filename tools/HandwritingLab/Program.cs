namespace HandwritingLab;

internal static class Program
{
    public static async Task Main(string[] args)
    {
        var port = 5387;
        if (args.Length > 0 && (args.Length != 2 || args[0] != "--port" || !int.TryParse(args[1], out port)))
            throw new ArgumentException("Usage: HandwritingLab [--port 5387]");
        await using var app = LabApplication.Build(port);
        await app.RunAsync();
    }
}
