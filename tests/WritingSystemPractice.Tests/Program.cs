using Npgsql;
using WritingSystemPractice.Tests;

DomainChecks.Run();
Console.WriteLine($"Passed {Check.Count} Domain checks.");
var database = Environment.GetEnvironmentVariable("WRITING_SYSTEM_PRACTICE_TEST_DATABASE");
if (database is null)
{
    Console.WriteLine("Skipped PostgreSQL checks. Set WRITING_SYSTEM_PRACTICE_TEST_DATABASE to an EMPTY isolated database named writing_system_practice_tests.");
    return;
}
if (new NpgsqlConnectionStringBuilder(database).Database != "writing_system_practice_tests")
    throw new InvalidOperationException("Only an isolated database named writing_system_practice_tests is allowed.");
await DatabaseChecks.RunAsync(database);
Console.WriteLine($"Passed {Check.Count} Domain/EF/PostgreSQL checks.");
await ApiChecks.RunAsync(database);
Console.WriteLine($"Passed {Check.Count} Domain/EF/PostgreSQL/HTTP checks.");
