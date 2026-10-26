namespace Application.Pronunciation.Models;

public sealed record PronunciationExampleDto(string Id, string Code, string Text, string Transcription,
    string TranscriptionSystem, string? AudioUrl, int SortOrder, long Version, DateTimeOffset UpdatedAt);
