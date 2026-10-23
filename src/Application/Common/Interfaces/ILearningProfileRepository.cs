using Application.LearningProfiles.Models;
using Domain.LearningProfiles;

namespace Application.Common.Interfaces;

public interface ILearningProfileRepository
{
    Task<LearningProfile?> FindOwnedAsync(long userId, long id, CancellationToken ct);
    Task<LearningProfileDto?> GetOwnedAsync(long userId, long id, CancellationToken ct);
    Task<LearningProfilePage> GetPageAsync(long userId, int pageSize, long? after, CancellationToken ct);
    Task CreateAsync(LearningProfile profile, CancellationToken ct);
    Task UpdatePreferencesAsync(LearningProfile profile, long expectedVersion, CancellationToken ct);
    Task DeleteAsync(long userId, long id, long version, CancellationToken ct);
    Task<CurrentLearningProfileDto?> GetCurrentAsync(long userId, CancellationToken ct);
    Task<CurrentLearningProfileDto> SetCurrentAsync(long userId, long profileId, DateTimeOffset now, CancellationToken ct);
}
