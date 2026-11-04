using Domain.Common;
using Domain.WritingSystems;

namespace Domain.Handwriting;

public sealed class HandwritingTemplate : Entity
{
    private HandwritingTemplate()
    {
    }

    public long WritingSystemItemId { get; private init; }
    public int Revision { get; private init; }
    public string Symbol { get; private init; } = null!;
    public InkDrawing Drawing { get; private init; } = null!;
    public int DrawingFormatVersion { get; private init; }
    public string SourceUrl { get; private init; } = null!;
    public string SourceRevision { get; private init; } = null!;
    public string Attribution { get; private init; } = null!;
    public string License { get; private init; } = null!;
    public string LicenseUrl { get; private init; } = null!;
    public bool IsPublished { get; private set; }
    public long Version { get; private set; }
    public DateTimeOffset CreatedAt { get; private init; }
    public DateTimeOffset UpdatedAt { get; private set; }

    public static HandwritingTemplate Create(long id, WritingSystemItem item, int revision, InkDrawing drawing,
        string sourceUrl, string sourceRevision, string attribution, string license, string licenseUrl,
        bool isPublished, DateTimeOffset now)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);
        ArgumentNullException.ThrowIfNull(item);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(revision);
        ArgumentNullException.ThrowIfNull(drawing);
        if (drawing.Strokes.Count == 0 || drawing.Strokes.Any(x => x.Count < 2 || !x.Any(p => p != x[0])))
            throw new ArgumentException("Reference strokes need at least two distinct points.", nameof(drawing));
        return new()
        {
            Id = id,
            WritingSystemItemId = item.Id,
            Revision = revision,
            Symbol = item.Symbol,
            Drawing = drawing,
            DrawingFormatVersion = InkDrawing.FormatVersion,
            SourceUrl = HttpsUrl(sourceUrl),
            SourceRevision = FoundationContentValidation.RequiredText(sourceRevision, 128, nameof(sourceRevision)),
            Attribution = FoundationContentValidation.RequiredText(attribution, 1000, nameof(attribution)),
            License = FoundationContentValidation.RequiredText(license, 128, nameof(license)),
            LicenseUrl = HttpsUrl(licenseUrl),
            IsPublished = isPublished,
            Version = 1,
            CreatedAt = now.ToUniversalTime(),
            UpdatedAt = now.ToUniversalTime()
        };
    }

    // Geometry/provenance are immutable: corrections require a new row with a new revision.
    public void SetPublished(bool isPublished, DateTimeOffset now)
    {
        if (now < UpdatedAt) throw new ArgumentException("The update cannot be backdated.", nameof(now));
        if (isPublished == IsPublished) return;
        var change = FoundationContentValidation.Next(Version, UpdatedAt, now);
        IsPublished = isPublished;
        Version = change.Version;
        UpdatedAt = change.UpdatedAt;
    }

    private static string HttpsUrl(string value)
    {
        var url = FoundationContentValidation.RequiredText(value, 1024, nameof(value));
        if (url.Any(char.IsWhiteSpace) || url.Any(char.IsControl) || url.Contains('\\') ||
            !Uri.TryCreate(url, UriKind.Absolute, out var uri) || uri.Scheme != Uri.UriSchemeHttps ||
            string.IsNullOrEmpty(uri.Host) || !string.IsNullOrEmpty(uri.UserInfo) || !string.IsNullOrEmpty(uri.Fragment))
            throw new ArgumentException("An absolute HTTPS URL without credentials or fragments is required.", nameof(value));
        return url;
    }
}
