using Application.Common.Interfaces;
using IdGen;

namespace Infrastructure.IdGeneration;

internal sealed class SnowflakeIdGenerator(IdGenerator generator) : IIdGenerator
{
    public long NewId() => generator.CreateId();
}