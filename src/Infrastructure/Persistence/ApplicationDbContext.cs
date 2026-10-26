using Infrastructure.Persistence.Entities;
using Domain.Users;
using Domain.AI;
using Domain.LearningProfiles;
using Domain.Languages;
using Domain.WritingSystems;
using Domain.Pronunciation;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public sealed class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<OAuthAccount> OAuthAccounts => Set<OAuthAccount>();
    public DbSet<Session> Sessions => Set<Session>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<AiConnection> AiConnections => Set<AiConnection>();
    public DbSet<AiConnectionCreation> AiConnectionCreations => Set<AiConnectionCreation>();
    public DbSet<LearningProfile> LearningProfiles => Set<LearningProfile>();
    public DbSet<LearningProfileSelection> LearningProfileSelections => Set<LearningProfileSelection>();

    public DbSet<Language> Languages => Set<Language>();
    public DbSet<LanguageTranslation> LanguageTranslations => Set<LanguageTranslation>();
    public DbSet<WritingSystem> WritingSystems => Set<WritingSystem>();
    public DbSet<WritingSystemTranslation> WritingSystemTranslations => Set<WritingSystemTranslation>();
    public DbSet<WritingSystemItem> WritingSystemItems => Set<WritingSystemItem>();
    public DbSet<WritingSystemItemReading> WritingSystemItemReadings => Set<WritingSystemItemReading>();
    public DbSet<PronunciationRule> PronunciationRules => Set<PronunciationRule>();
    public DbSet<PronunciationRuleTranslation> PronunciationRuleTranslations => Set<PronunciationRuleTranslation>();
    public DbSet<PronunciationExample> PronunciationExamples => Set<PronunciationExample>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(ApplicationDbContext).Assembly);
    }
}
