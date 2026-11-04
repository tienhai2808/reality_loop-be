using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using HandwritingLab.Contracts;
using HandwritingLab.Models;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.Extensions.DependencyInjection;

namespace HandwritingLab.Tests;

internal static class HttpChecks
{
    public static async Task RunAsync(TemplateCatalog catalog)
    {
        await using var app = LabApplication.Build(0);
        await app.StartAsync();
        var url = app.Services.GetRequiredService<IServer>().Features.Get<IServerAddressesFeature>()!.Addresses.Single();
        using var client = new HttpClient { BaseAddress = new Uri(url) };
        var json = new JsonSerializerOptions(JsonSerializerDefaults.Web) { Converters = { new JsonStringEnumConverter() } };
        var html = await client.GetAsync("/");
        Check.That(html.IsSuccessStatusCode && html.Content.Headers.ContentType?.MediaType == "text/html", "Lab UI is bundled and accessible.");
        Check.That(html.Headers.CacheControl?.NoStore == true && html.Headers.Contains("Content-Security-Policy"), "Lab uses no-store and a same-origin resource policy.");
        Check.That((await client.GetAsync("/app.js")).IsSuccessStatusCode && (await client.GetAsync("/style.css")).IsSuccessStatusCode, "UI assets are bundled, not dependent on CDN.");
        Check.That((await client.GetStringAsync("/data-license")).Contains("Creative Commons", StringComparison.OrdinalIgnoreCase), "Original data license is available.");
        var response = await client.GetAsync(LabApplication.ApiBase + "/templates");
        var data = (await response.Content.ReadFromJsonAsync<CatalogResponse>(json))!;
        Check.That(data.Dataset.Characters.Count == 4 && data.Targets.Count == 5 && data.Dataset.License == "CC BY-SA 3.0", "Catalog carries coverage, provenance and license.");
        var target = catalog.Find("ao")!;
        var request = new GradeRequest { TargetId = target.Id, Characters = target.CharacterIds.Select(id => catalog.Character(id).Strokes).ToArray() };
        var graded = await client.PostAsJsonAsync(LabApplication.ApiBase + "/grade", request);
        var result = (await graded.Content.ReadFromJsonAsync<GradeResult>(json))!;
        Check.That(graded.IsSuccessStatusCode && result.Passed && result.Characters.Count == 2, "Server grades each character of a word independently.");
        var empty = await client.PostAsJsonAsync(LabApplication.ApiBase + "/grade", new GradeRequest { TargetId = "a", Characters = [Array.Empty<IReadOnlyList<InkPoint>>() ] });
        Check.That((await empty.Content.ReadFromJsonAsync<GradeResult>(json))!.Characters[0].Issues.Contains(HandwritingIssue.MissingDrawing), "Empty but valid handwriting gets grading feedback.");
        foreach (var raw in new[]
        {
            "{}", "null", "{", "{\"targetId\":\"a\"}", "{\"targetId\":null,\"characters\":[]}",
            "{\"targetId\":\"unknown\",\"characters\":[]}", "{\"targetId\":\"a\",\"characters\":null}",
            "{\"targetId\":\"a\",\"characters\":[null]}", "{\"targetId\":\"a\",\"characters\":[[null]]}",
            "{\"targetId\":\"a\",\"characters\":[[[]]]}",
            "{\"targetId\":\"a\",\"characters\":[[[{\"x\":0.5}]]]}",
            "{\"targetId\":\"a\",\"characters\":[[[{\"x\":\"0.5\",\"y\":0.5}]]]}",
            "{\"targetId\":\"a\",\"characters\":[[[{\"x\":2,\"y\":0.5}]]]}",
            "{\"targetId\":\"a\",\"characters\":[[]],\"passed\":true}",
            "{\"targetId\":\"a\",\"characters\":[[]],\"tolerance\":1}",
            "{\"targetId\":\"a\",\"characters\":[[[{\"x\":0.5,\"y\":0.5,\"extra\":1}]]]}"
        })
        {
            var invalid = await client.PostAsync(LabApplication.ApiBase + "/grade", new StringContent(raw, Encoding.UTF8, "application/json"));
            Check.That((int)invalid.StatusCode == 400, "Invalid/malformed/unbounded client data returns 400.");
            Check.That(invalid.Content.Headers.ContentType?.MediaType == "application/problem+json", "Validation uses ProblemDetails.");
        }
        var large = await client.PostAsync(LabApplication.ApiBase + "/grade", new StringContent(new string(' ', 270000) + "{}", Encoding.UTF8, "application/json"));
        Check.That((int)large.StatusCode == 413, "Kestrel enforces a 256 KiB payload ceiling.");
        Check.That(large.Content.Headers.ContentType?.MediaType == "application/problem+json", "Oversized drawing errors still return a readable JSON problem.");
        var plain = await client.PostAsync(LabApplication.ApiBase + "/grade", new StringContent("{}"));
        Check.That((int)plain.StatusCode == 415, "Non-JSON submissions return 415.");
        Check.That(plain.Content.Headers.ContentType?.MediaType == "application/problem+json", "Unsupported content type returns a JSON problem.");
        using var crossSite = new HttpRequestMessage(HttpMethod.Post, LabApplication.ApiBase + "/grade") { Content = JsonContent.Create(request) };
        crossSite.Headers.Add("Origin", "https://example.org");
        Check.That((int)(await client.SendAsync(crossSite)).StatusCode == 403, "Foreign-origin submissions cannot call the local lab.");
        using var rebinding = new HttpRequestMessage(HttpMethod.Get, LabApplication.ApiBase + "/templates");
        rebinding.Headers.Host = "untrusted.example:5387";
        Check.That((int)(await client.SendAsync(rebinding)).StatusCode == 403, "Unexpected Host cannot expose the local tool through DNS rebinding.");
        using var sameOrigin = new HttpRequestMessage(HttpMethod.Post, LabApplication.ApiBase + "/grade") { Content = JsonContent.Create(request) };
        sameOrigin.Headers.Add("Origin", url);
        Check.That((await client.SendAsync(sameOrigin)).IsSuccessStatusCode, "Same-origin browser drawing submissions work.");
        await app.StopAsync();
    }
}
