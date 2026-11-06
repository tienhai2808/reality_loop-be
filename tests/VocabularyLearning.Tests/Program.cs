using Npgsql;
using VocabularyLearning.Tests;

DomainChecks.Run();
Console.WriteLine($"Passed {Check.Count} Domain checks.");
SchedulerChecks.Run();
Console.WriteLine($"Passed {Check.Count} Domain/scheduler checks.");
var database = Environment.GetEnvironmentVariable("VOCABULARY_LEARNING_TEST_DATABASE");
if (database is null)
{
    Console.WriteLine("Skipped PostgreSQL checks. Use an EMPTY isolated database named vocabulary_learning_tests.");
    return;
}
if (new NpgsqlConnectionStringBuilder(database).Database != "vocabulary_learning_tests")
    throw new InvalidOperationException("Only an isolated database named vocabulary_learning_tests is allowed.");
await DatabaseChecks.RunAsync(database);
Console.WriteLine($"Passed {Check.Count} Domain/EF/PostgreSQL checks.");
await ApiChecks.RunAsync(database);
Console.WriteLine($"Passed {Check.Count} Domain/scheduler/EF/PostgreSQL/HTTP checks.");
