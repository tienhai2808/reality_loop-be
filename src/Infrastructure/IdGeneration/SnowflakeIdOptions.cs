namespace Infrastructure.IdGeneration;

public sealed class SnowflakeIdOptions
{
    public const string SectionName = "SnowflakeId";

    public int GeneratorId { get; init; }
}