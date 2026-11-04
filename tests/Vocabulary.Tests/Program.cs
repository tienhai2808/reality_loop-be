using Npgsql;
using Vocabulary.Tests;

DomainChecks.Run();
Console.WriteLine($"Passed {Check.Count} vocabulary Domain checks.");
var database = Environment.GetEnvironmentVariable("VOCABULARY_TEST_DATABASE");
if (database is null)
{
    Console.WriteLine("Skipped PostgreSQL checks. Set VOCABULARY_TEST_DATABASE to an EMPTY isolated database named vocabulary_tests.");
    return;
}
if (new NpgsqlConnectionStringBuilder(database).Database != "vocabulary_tests")
    throw new InvalidOperationException("Only an isolated database named vocabulary_tests is allowed.");
await DatabaseChecks.RunAsync(database);
Console.WriteLine($"Passed {Check.Count} vocabulary Domain/EF/PostgreSQL checks.");
await ApiChecks.RunAsync(database);
Console.WriteLine($"Passed {Check.Count} vocabulary Domain/EF/PostgreSQL/HTTP checks.");
