using GrammarPractice.Tests;
using Npgsql;

DomainChecks.Run();
Console.WriteLine($"Passed {Check.Count} Domain checks.");
var database = Environment.GetEnvironmentVariable("GRAMMAR_PRACTICE_TEST_DATABASE");
if (database is null)
{
    Console.WriteLine("PostgreSQL checks require an empty isolated database named grammar_practice_tests.");
    return;
}
if (new NpgsqlConnectionStringBuilder(database).Database != "grammar_practice_tests")
    throw new InvalidOperationException("Only an isolated database named grammar_practice_tests is allowed.");
await DatabaseChecks.RunAsync(database);
Console.WriteLine($"Passed {Check.Count} Domain/EF/PostgreSQL checks.");
await ApiChecks.RunAsync(database);
Console.WriteLine($"Passed {Check.Count} Domain/EF/PostgreSQL/HTTP checks.");
