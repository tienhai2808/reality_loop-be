namespace Application.WritingSystems.Models;

public sealed record WritingSystemItemReadingDto(string Id, string Code, string Transcription, string TranscriptionSystem,
    string? AudioUrl, int SortOrder, long Version, DateTimeOffset UpdatedAt);
