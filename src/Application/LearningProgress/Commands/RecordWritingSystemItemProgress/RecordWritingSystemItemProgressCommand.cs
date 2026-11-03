using Application.LearningProgress.Models;

namespace Application.LearningProgress.Commands.RecordWritingSystemItemProgress;

public sealed record RecordWritingSystemItemProgressCommand(long ProfileId, string SystemCode, long ItemId,
    WritingSystemItemProgressAction Action, long Version);
