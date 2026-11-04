using Handwriting.Tests;
using Npgsql;

DomainChecks.Run();
Console.WriteLine($"Passed {Check.Count} Domain checks.");
ScoringChecks.Run();
Console.WriteLine($"Passed {Check.Count} Domain/scoring checks.");
var database = Environment.GetEnvironmentVariable("HANDWRITING_TEST_DATABASE");
if (database is null)
{
    Console.WriteLine("Skipped PostgreSQL checks. Set HANDWRITING_TEST_DATABASE to an EMPTY isolated database named handwriting_tests.");
    return;
}
if (new NpgsqlConnectionStringBuilder(database).Database != "handwriting_tests")
    throw new InvalidOperationException("Only an isolated database named handwriting_tests is allowed.");
await DatabaseChecks.RunAsync(database);
Console.WriteLine($"Passed {Check.Count} Domain/EF/PostgreSQL checks.");
await ApiChecks.RunAsync(database);
Console.WriteLine($"Passed {Check.Count} Domain/EF/PostgreSQL/HTTP checks.");
