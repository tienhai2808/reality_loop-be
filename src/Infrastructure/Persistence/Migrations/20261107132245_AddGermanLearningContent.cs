using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddGermanLearningContent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "languages",
                columns: new[] { "id", "code", "created_at", "is_enabled", "native_name", "sort_order", "updated_at", "version" },
                values: new object[] { 1563703640076578816L, "de", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, "Deutsch", 1, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L });

            migrationBuilder.InsertData(
                table: "grammar_points",
                columns: new[] { "id", "code", "created_at", "is_published", "language_id", "native_name", "pattern", "sort_order", "updated_at", "version" },
                values: new object[,]
                {
                    { 1563703640076581016L, "statement-v2", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, "Verbzweitstellung", "Vorfeld + finites Verb + weitere Satzteile", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076581017L, "yes-no-questions", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, "Ja-Nein-Fragen", "Finites Verb + Subjekt + weitere Satzteile?", 1, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076581018L, "w-questions", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, "W-Fragen", "Fragewort + finites Verb + Subjekt + ...?", 2, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076581019L, "personal-pronouns", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, "Personalpronomen", "ich, du, er, sie, es, wir, ihr, sie, Sie", 3, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076581020L, "sein-present", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, "sein im Präsens", "ich bin, du bist, er/sie/es ist, wir sind, ihr seid, sie/Sie sind", 4, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076581021L, "haben-present", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, "haben im Präsens", "ich habe, du hast, er/sie/es hat, wir haben, ihr habt, sie/Sie haben", 5, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076581022L, "regular-present", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, "Regelmäßige Verben", "Verbstamm + Personalendung", 6, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076581023L, "verb-stem-changes", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, "Stammvokalwechsel", "du / er, sie, es + veränderter Verbstamm", 7, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076581024L, "formal-address", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, "Anrede mit du und Sie", "du / ihr / Sie", 8, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076581025L, "noun-capitals", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, "Großschreibung von Nomen", "Nomen mit großem Anfangsbuchstaben", 9, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076581026L, "definite-articles", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, "Bestimmter Artikel im Nominativ", "der / die / das; Plural die", 10, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076581027L, "indefinite-articles", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, "Unbestimmter Artikel im Nominativ", "ein / eine / ein", 11, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076581028L, "plural-nouns", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, "Pluralformen", "Nomen + gelernte Pluralform", 12, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076581029L, "negation-kein", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, "Negation mit kein", "kein / keine + Nomen", 13, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076581030L, "negation-nicht", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, "Negation mit nicht", "nicht + negierter Satzteil", 14, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076581031L, "possessives", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, "Possessivartikel", "mein / dein / sein / ihr / unser / euer / Ihr + Nomen", 15, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076581032L, "predicate-adjectives", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, "Prädikative Adjektive", "Subjekt + sein + Adjektiv", 16, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076581033L, "accusative", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, "Akkusativ", "Subjekt + Verb + Akkusativobjekt", 17, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076581034L, "accusative-pronouns", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, "Personalpronomen im Akkusativ", "mich, dich, ihn, sie, es, uns, euch, sie, Sie", 18, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076581035L, "dative", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, "Dativ", "Verb oder Präposition + Dativ", 19, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076581036L, "dative-pronouns", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, "Personalpronomen im Dativ", "mir, dir, ihm, ihr, ihm, uns, euch, ihnen, Ihnen", 20, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076581037L, "dative-prepositions", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, "Präpositionen mit Dativ", "mit / nach / bei / seit / von / zu / aus + Dativ", 21, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076581038L, "accusative-prepositions", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, "Präpositionen mit Akkusativ", "für / ohne / durch / gegen / um + Akkusativ", 22, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076581039L, "local-in-an-auf", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, "Ortsangaben mit Wechselpräpositionen", "wo + Dativ; wohin + Akkusativ", 23, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076581040L, "time-prepositions", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, "Zeitangaben", "am + Tag; um + Uhrzeit; im + Monat", 24, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076581041L, "clock-time", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, "Uhrzeiten", "Es ist ... Uhr.", 25, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076581042L, "numbers", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, "Zahlenbildung", "Einheit + und + Zehner", 26, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076581043L, "modal-can", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, "Modalverb können", "Subjekt + können + ... + Infinitiv", 27, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076581044L, "modal-must", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, "Modalverb müssen", "Subjekt + müssen + ... + Infinitiv", 28, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076581045L, "modal-permission", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, "Modalverb dürfen", "Subjekt + dürfen + ... + Infinitiv", 29, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076581046L, "modal-want", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, "wollen und möchte", "ich will / ich möchte + ... + Infinitiv", 30, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076581047L, "separable-verbs", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, "Trennbare Verben", "Präfix + Stamm; im Hauptsatz Präfix am Ende", 31, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076581048L, "imperative", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, "Imperativ", "Komm! Kommt! Kommen Sie!", 32, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076581049L, "word-order-time", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, "Zeitangaben im Satz", "Zeitangabe im Vorfeld + Verb + Subjekt", 33, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076581050L, "coordination", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, "und, oder, aber", "Hauptsatz + und/oder/aber + Hauptsatz", 34, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076581051L, "because-weil", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, "Nebensatz mit weil", "weil + Subjekt + ... + finites Verb", 35, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076581052L, "subordinate-dass", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, "Nebensatz mit dass", "dass + Subjekt + ... + finites Verb", 36, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076581053L, "perfekt-haben", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, "Perfekt mit haben", "haben + ... + Partizip II", 37, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076581054L, "perfekt-sein", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, "Perfekt mit sein", "sein + ... + Partizip II", 38, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076581055L, "past-participles", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, "Partizip II", "ge- + Stamm + -t / gelernte unregelmäßige Form", 39, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076581056L, "past-sein-haben", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, "Präteritum von sein und haben", "war / hatte", 40, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076581057L, "future-present", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, "Zukunft mit Präsens", "Präsens + Zeitangabe", 41, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076581058L, "comparative", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, "Komparativ", "Adjektiv + -er; ... als ...", 42, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076581059L, "superlative", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, "Superlativ", "am + Adjektiv + -sten/-esten", 43, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076581060L, "adjectives-before-nouns", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, "Adjektive vor Nomen", "Artikel + Adjektivendung + Nomen", 44, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076581061L, "demonstratives", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, "Demonstrativartikel", "dieser / diese / dieses + Nomen", 45, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076581062L, "quantity-words", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, "Mengenangaben", "viel / viele; wenig / wenige", 46, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076581063L, "likes-gern", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, "Vorlieben mit gern", "Subjekt + Verb + gern", 47, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076581064L, "polite-requests", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, "Höfliche Bitten", "Könnten Sie bitte ...? / Ich hätte gern ...", 48, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076581065L, "existence-es-gibt", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, "es gibt", "es gibt + Akkusativ", 49, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L }
                });

            migrationBuilder.InsertData(
                table: "language_translations",
                columns: new[] { "id", "created_at", "description", "language_id", "locale", "name", "updated_at", "version" },
                values: new object[,]
                {
                    { 1563703640076578817L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Học chữ cái, phát âm, từ vựng và ngữ pháp tiếng Đức nhập môn.", 1563703640076578816L, "vi", "Tiếng Đức", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076578818L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Study the alphabet, pronunciation, introductory German vocabulary and grammar.", 1563703640076578816L, "en", "German", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L }
                });

            migrationBuilder.InsertData(
                table: "pronunciation_rules",
                columns: new[] { "id", "code", "created_at", "is_published", "language_id", "native_name", "sort_order", "updated_at", "version" },
                values: new object[,]
                {
                    { 1563703640076579316L, "vowel-length", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, "Lange und kurze Vokale", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579317L, "umlauts", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, "Umlautvokale", 1, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579318L, "ei-and-ie", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, "ei und ie", 2, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579319L, "diphthongs", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, "au, eu und äu", 3, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579320L, "ich-ach", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, "Ich-Laut und Ach-Laut", 4, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579321L, "sch-sp-st", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, "sch, sp und st", 5, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579322L, "z-w-v", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, "z, w und v", 6, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579323L, "s-ss-eszett", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, "s, ss und ß", 7, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579324L, "final-devoicing", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, "Auslautverhärtung", 8, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579325L, "r-variants", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, "Aussprache des r", 9, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579326L, "silent-h", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, "Dehnungs-h", 10, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579327L, "word-stress", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, "Wortakzent", 11, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L }
                });

            migrationBuilder.InsertData(
                table: "vocabulary_entries",
                columns: new[] { "id", "audio_url", "code", "created_at", "is_published", "language_id", "reading", "sort_order", "term", "updated_at", "version" },
                values: new object[,]
                {
                    { 1563703640076579816L, null, "hello", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, null, 0, "Hallo", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579817L, null, "morning", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, null, 1, "Guten Morgen", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579818L, null, "day-greeting", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, null, 2, "Guten Tag", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579819L, null, "evening-greeting", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, null, 3, "Guten Abend", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579820L, null, "goodbye", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, null, 4, "Auf Wiedersehen", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579821L, null, "bye", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, null, 5, "Tschüss", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579822L, null, "thanks", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, null, 6, "Danke", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579823L, null, "please", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, null, 7, "bitte", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579824L, null, "sorry", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, null, 8, "Entschuldigung", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579825L, null, "yes", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, null, 9, "ja", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579826L, null, "no", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, null, 10, "nein", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579827L, null, "i", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, null, 11, "ich", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579828L, null, "you-informal", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, null, 12, "du", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579829L, null, "he", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, null, 13, "er", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579830L, null, "she-they", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, null, 14, "sie", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579831L, null, "it", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, null, 15, "es", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579832L, null, "we", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, null, 16, "wir", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579833L, null, "you-plural", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, null, 17, "ihr", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579834L, null, "you-formal", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, null, 18, "Sie", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579835L, null, "who", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, null, 19, "wer", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579836L, null, "what", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, null, 20, "was", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579837L, null, "where", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, null, 21, "wo", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579838L, null, "when", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, null, 22, "wann", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579839L, null, "how", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, null, 23, "wie", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579840L, null, "zero", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, null, 24, "null", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579841L, null, "one", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, null, 25, "eins", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579842L, null, "two", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, null, 26, "zwei", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579843L, null, "three", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, null, 27, "drei", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579844L, null, "four", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, null, 28, "vier", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579845L, null, "five", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, null, 29, "fünf", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579846L, null, "six", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, null, 30, "sechs", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579847L, null, "seven", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, null, 31, "sieben", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579848L, null, "eight", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, null, 32, "acht", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579849L, null, "nine", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, null, 33, "neun", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579850L, null, "ten", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, null, 34, "zehn", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579851L, null, "eleven", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, null, 35, "elf", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579852L, null, "twelve", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, null, 36, "zwölf", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579853L, null, "twenty", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, null, 37, "zwanzig", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579854L, null, "hundred", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, null, 38, "hundert", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579855L, null, "monday", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, null, 39, "Montag", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579856L, null, "tuesday", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, null, 40, "Dienstag", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579857L, null, "wednesday", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, null, 41, "Mittwoch", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579858L, null, "thursday", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, null, 42, "Donnerstag", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579859L, null, "friday", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, null, 43, "Freitag", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579860L, null, "saturday", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, null, 44, "Samstag", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579861L, null, "sunday", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, null, 45, "Sonntag", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579862L, null, "today", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, null, 46, "heute", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579863L, null, "tomorrow", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, null, 47, "morgen", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579864L, null, "yesterday", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, null, 48, "gestern", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579865L, null, "now", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, null, 49, "jetzt", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579866L, null, "always", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, null, 50, "immer", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579867L, null, "often", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, null, 51, "oft", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579868L, null, "sometimes", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, null, 52, "manchmal", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579869L, null, "never", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, null, 53, "nie", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579870L, null, "day", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, null, 54, "Tag", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579871L, null, "week", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, null, 55, "Woche", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579872L, null, "month", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, null, 56, "Monat", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579873L, null, "year", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, null, 57, "Jahr", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579874L, null, "time", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, null, 58, "Zeit", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579875L, null, "hour", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, null, 59, "Stunde", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579876L, null, "minute", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, null, 60, "Minute", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579877L, null, "family", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, null, 61, "Familie", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579878L, null, "mother", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, null, 62, "Mutter", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579879L, null, "father", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, null, 63, "Vater", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579880L, null, "brother", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, null, 64, "Bruder", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579881L, null, "sister", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, null, 65, "Schwester", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579882L, null, "child", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, null, 66, "Kind", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579883L, null, "man", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, null, 67, "Mann", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579884L, null, "woman", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, null, 68, "Frau", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579885L, null, "friend-male", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, null, 69, "Freund", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579886L, null, "friend-female", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, null, 70, "Freundin", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579887L, null, "name", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, null, 71, "Name", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579888L, null, "person", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, null, 72, "Person", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579889L, null, "teacher-male", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, null, 73, "Lehrer", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579890L, null, "teacher-female", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, null, 74, "Lehrerin", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579891L, null, "student", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, null, 75, "Student", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579892L, null, "school", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, null, 76, "Schule", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579893L, null, "university", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, null, 77, "Universität", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579894L, null, "work", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, null, 78, "Arbeit", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579895L, null, "company", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, null, 79, "Firma", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579896L, null, "doctor", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, null, 80, "Arzt", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579897L, null, "hospital", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, null, 81, "Krankenhaus", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579898L, null, "pharmacy", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, null, 82, "Apotheke", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579899L, null, "bank", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, null, 83, "Bank", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579900L, null, "station", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, null, 84, "Bahnhof", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579901L, null, "airport", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, null, 85, "Flughafen", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579902L, null, "city", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, null, 86, "Stadt", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579903L, null, "country", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, null, 87, "Land", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579904L, null, "street", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, null, 88, "Straße", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579905L, null, "house", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, null, 89, "Haus", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579906L, null, "apartment", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, null, 90, "Wohnung", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579907L, null, "room", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, null, 91, "Zimmer", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579908L, null, "kitchen", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, null, 92, "Küche", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579909L, null, "bathroom", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, null, 93, "Bad", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579910L, null, "hotel", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, null, 94, "Hotel", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579911L, null, "restaurant", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, null, 95, "Restaurant", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579912L, null, "supermarket", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, null, 96, "Supermarkt", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579913L, null, "shop", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, null, 97, "Geschäft", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579914L, null, "water", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, null, 98, "Wasser", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579915L, null, "coffee", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, null, 99, "Kaffee", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579916L, null, "tea", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, null, 100, "Tee", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579917L, null, "milk", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, null, 101, "Milch", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579918L, null, "bread", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, null, 102, "Brot", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579919L, null, "roll", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, null, 103, "Brötchen", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579920L, null, "egg", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, null, 104, "Ei", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579921L, null, "cheese", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, null, 105, "Käse", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579922L, null, "meat", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, null, 106, "Fleisch", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579923L, null, "fish", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, null, 107, "Fisch", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579924L, null, "apple", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, null, 108, "Apfel", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579925L, null, "banana", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, null, 109, "Banane", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579926L, null, "potato", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, null, 110, "Kartoffel", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579927L, null, "rice", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, null, 111, "Reis", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579928L, null, "soup", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, null, 112, "Suppe", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579929L, null, "breakfast", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, null, 113, "Frühstück", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579930L, null, "money", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, null, 114, "Geld", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579931L, null, "price", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, null, 115, "Preis", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579932L, null, "ticket", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, null, 116, "Fahrkarte", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579933L, null, "car", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, null, 117, "Auto", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579934L, null, "bus", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, null, 118, "Bus", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579935L, null, "train", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, null, 119, "Zug", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579936L, null, "bicycle", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, null, 120, "Fahrrad", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579937L, null, "book", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, null, 121, "Buch", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579938L, null, "phone", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, null, 122, "Telefon", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579939L, null, "key", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, null, 123, "Schlüssel", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579940L, null, "bag", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, null, 124, "Tasche", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579941L, null, "table", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, null, 125, "Tisch", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579942L, null, "chair", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, null, 126, "Stuhl", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579943L, null, "door", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, null, 127, "Tür", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579944L, null, "window", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, null, 128, "Fenster", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579945L, null, "be", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, null, 129, "sein", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579946L, null, "have", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, null, 130, "haben", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579947L, null, "go", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, null, 131, "gehen", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579948L, null, "come", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, null, 132, "kommen", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579949L, null, "live", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, null, 133, "wohnen", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579950L, null, "be-called", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, null, 134, "heißen", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579951L, null, "speak", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, null, 135, "sprechen", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579952L, null, "learn", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, null, 136, "lernen", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579953L, null, "work-verb", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, null, 137, "arbeiten", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579954L, null, "make", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, null, 138, "machen", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579955L, null, "buy", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, null, 139, "kaufen", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579956L, null, "pay", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, null, 140, "bezahlen", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579957L, null, "cost", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, null, 141, "kosten", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579958L, null, "need", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, null, 142, "brauchen", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579959L, null, "search", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, null, 143, "suchen", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579960L, null, "find", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, null, 144, "finden", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579961L, null, "eat", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, null, 145, "essen", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579962L, null, "drink", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, null, 146, "trinken", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579963L, null, "cook", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, null, 147, "kochen", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579964L, null, "read", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, null, 148, "lesen", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579965L, null, "write", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, null, 149, "schreiben", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579966L, null, "hear", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, null, 150, "hören", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579967L, null, "see", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, null, 151, "sehen", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579968L, null, "sleep", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, null, 152, "schlafen", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579969L, null, "drive", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, null, 153, "fahren", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579970L, null, "take", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, null, 154, "nehmen", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579971L, null, "give", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, null, 155, "geben", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579972L, null, "help", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, null, 156, "helfen", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579973L, null, "ask", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, null, 157, "fragen", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579974L, null, "answer", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, null, 158, "antworten", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579975L, null, "understand", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, null, 159, "verstehen", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579976L, null, "know", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, null, 160, "wissen", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579977L, null, "can", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, null, 161, "können", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579978L, null, "must", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, null, 162, "müssen", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579979L, null, "want", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, null, 163, "wollen", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579980L, null, "may", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, null, 164, "dürfen", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579981L, null, "should", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, null, 165, "sollen", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579982L, null, "like", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, null, 166, "mögen", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579983L, null, "good", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, null, 167, "gut", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579984L, null, "bad", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, null, 168, "schlecht", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579985L, null, "large", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, null, 169, "groß", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579986L, null, "small", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, null, 170, "klein", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579987L, null, "new", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, null, 171, "neu", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579988L, null, "old", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, null, 172, "alt", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579989L, null, "beautiful", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, null, 173, "schön", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579990L, null, "expensive", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, null, 174, "teuer", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579991L, null, "inexpensive", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, null, 175, "billig", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579992L, null, "hot", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, null, 176, "heiß", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579993L, null, "cold", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, null, 177, "kalt", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579994L, null, "warm", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, null, 178, "warm", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579995L, null, "fast", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, null, 179, "schnell", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579996L, null, "slow", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, null, 180, "langsam", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579997L, null, "right", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, null, 181, "richtig", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579998L, null, "wrong", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, null, 182, "falsch", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579999L, null, "important", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, null, 183, "wichtig", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076580000L, null, "tired", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, null, 184, "müde", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076580001L, null, "happy", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, null, 185, "glücklich", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076580002L, null, "ill", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, null, 186, "krank", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076580003L, null, "healthy", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, null, 187, "gesund", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076580004L, null, "open", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, null, 188, "offen", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076580005L, null, "closed", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, null, 189, "geschlossen", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076580006L, null, "here", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, null, 190, "hier", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076580007L, null, "there", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, null, 191, "dort", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076580008L, null, "left", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, null, 192, "links", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076580009L, null, "right-direction", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, null, 193, "rechts", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076580010L, null, "very", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, null, 194, "sehr", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076580011L, null, "also", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, null, 195, "auch", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076580012L, null, "only", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, null, 196, "nur", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076580013L, null, "and", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, null, 197, "und", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076580014L, null, "or", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, null, 198, "oder", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076580015L, null, "but", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, null, 199, "aber", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L }
                });

            migrationBuilder.InsertData(
                table: "writing_systems",
                columns: new[] { "id", "code", "created_at", "is_published", "language_id", "native_name", "sort_order", "updated_at", "version" },
                values: new object[] { 1563703640076578826L, "latin", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), true, 1563703640076578816L, "Lateinische Schrift", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L });

            migrationBuilder.InsertData(
                table: "grammar_point_translations",
                columns: new[] { "id", "created_at", "explanation", "grammar_point_id", "locale", "name", "updated_at", "usage_note", "version" },
                values: new object[,]
                {
                    { 1563703640076581116L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Trong câu trần thuật chính, động từ đã chia đứng ở vị trí thành phần thứ hai. Heute lerne ich Deutsch. nghĩa là Hôm nay tôi học tiếng Đức.", 1563703640076581016L, "vi", "Động từ ở vị trí thứ hai", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Vị trí thứ hai tính theo thành phần câu, có thể gồm nhiều từ.", 1L },
                    { 1563703640076581117L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "In a main declarative clause, the finite verb occupies the second constituent position. Heute lerne ich Deutsch. means Today I am learning German.", 1563703640076581016L, "en", "Verb-second statements", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "The second position counts constituents, which may contain several words.", 1L },
                    { 1563703640076581118L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Đặt động từ đã chia trước chủ ngữ. Lernen Sie Deutsch? nghĩa là Ông/bà có học tiếng Đức không?", 1563703640076581017L, "vi", "Câu hỏi có hoặc không", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Kết thúc câu hỏi viết bằng dấu hỏi.", 1L },
                    { 1563703640076581119L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Place the finite verb before the subject. Lernen Sie Deutsch? means Are you learning German, in formal address.", 1563703640076581017L, "en", "Yes-no questions", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Use a question mark in writing.", 1L },
                    { 1563703640076581120L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Đặt từ để hỏi ở đầu, theo sau là động từ đã chia. Wo wohnen Sie? nghĩa là Ông/bà sống ở đâu?", 1563703640076581018L, "vi", "Câu hỏi với từ để hỏi", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "wer hỏi người, was hỏi sự việc, wo hỏi vị trí, wohin hỏi hướng đến.", 1L },
                    { 1563703640076581121L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Put the question word first, followed by the finite verb. Wo wohnen Sie? means Where do you live, in formal address.", 1563703640076581018L, "en", "Questions with question words", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "wer asks about people, was about things, wo about location and wohin about destination.", 1L },
                    { 1563703640076581122L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Chọn đại từ theo ngôi, số và quan hệ giao tiếp. Wir lernen. nghĩa là Chúng tôi học.", 1563703640076581019L, "vi", "Đại từ nhân xưng", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "sie có thể chỉ cô ấy hoặc họ. Sie viết hoa dùng để xưng hô lịch sự.", 1L },
                    { 1563703640076581123L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Choose the pronoun by person, number and form of address. Wir lernen. means We are learning.", 1563703640076581019L, "en", "Personal pronouns", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "sie can mean she or they. Capitalized Sie is formal you.", 1L },
                    { 1563703640076581124L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Dùng sein để giới thiệu hoặc miêu tả trạng thái. Ich bin müde. nghĩa là Tôi mệt.", 1563703640076581020L, "vi", "Động từ sein ở hiện tại", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "sein chia bất quy tắc, cần học từng dạng.", 1L },
                    { 1563703640076581125L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Use sein to identify someone or describe a state. Ich bin müde. means I am tired.", 1563703640076581020L, "en", "Present tense of sein", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "sein is irregular, so learn its forms individually.", 1L },
                    { 1563703640076581126L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Dùng haben để nói có hoặc sở hữu. Ich habe ein Buch. nghĩa là Tôi có một quyển sách.", 1563703640076581021L, "vi", "Động từ haben ở hiện tại", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Dạng du và er/sie/es bỏ b của thân động từ.", 1L },
                    { 1563703640076581127L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Use haben to express having or possessing something. Ich habe ein Buch. means I have a book.", 1563703640076581021L, "en", "Present tense of haben", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "The du and er/sie/es forms drop the b from the stem.", 1L },
                    { 1563703640076581128L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Bỏ -en của nguyên mẫu rồi thêm đuôi theo chủ ngữ. Với lernen, các dạng là lerne, lernst, lernt, lernen, lernt, lernen.", 1563703640076581022L, "vi", "Động từ thường ở hiện tại", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Các đuôi cơ bản theo ich, du, er/sie/es, wir, ihr, sie/Sie là -e, -st, -t, -en, -t, -en.", 1L },
                    { 1563703640076581129L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Remove -en from the infinitive and add the personal ending. lernen gives lerne, lernst, lernt, lernen, lernt and lernen.", 1563703640076581022L, "en", "Regular present-tense verbs", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "The basic endings for ich, du, er/sie/es, wir, ihr and sie/Sie are -e, -st, -t, -en, -t and -en.", 1L },
                    { 1563703640076581130L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Một số động từ đổi nguyên âm ở ngôi du và er/sie/es. sprechen có du sprichst và er spricht; fahren có du fährst và sie fährt.", 1563703640076581023L, "vi", "Biến đổi nguyên âm thân động từ", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Học dạng ngôi ba cùng nguyên mẫu để nhận diện nhóm biến đổi.", 1L },
                    { 1563703640076581131L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Some verbs change their stem vowel with du and er/sie/es. sprechen has du sprichst and er spricht; fahren has du fährst and sie fährt.", 1563703640076581023L, "en", "Stem-vowel changes", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Learn the third-person singular together with the infinitive.", 1L },
                    { 1563703640076581132L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Dùng du với một người trong quan hệ thân mật, ihr với nhiều người thân mật và Sie khi xưng hô lịch sự. Wie heißen Sie? nghĩa là Ông/bà tên gì?", 1563703640076581024L, "vi", "Xưng hô thân mật và lịch sự", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Động từ với Sie có dạng như ngôi ba số nhiều. Viết hoa cả đại từ Sie và dạng sở hữu Ihr khi xưng hô lịch sự.", 1L },
                    { 1563703640076581133L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Use du for one person informally, ihr for several people informally and Sie for formal address. Wie heißen Sie? means What is your name, formally.", 1563703640076581024L, "en", "Informal and formal address", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Verbs with Sie use the third-person plural form. Capitalize formal Sie and possessive Ihr.", 1L },
                    { 1563703640076581134L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Danh từ tiếng Đức bắt đầu bằng chữ hoa. Das ist ein Buch. nghĩa là Đó là một quyển sách.", 1563703640076581025L, "vi", "Viết hoa danh từ", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Quy tắc viết hoa danh từ áp dụng cả giữa câu.", 1L },
                    { 1563703640076581135L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "German nouns begin with a capital letter. Das ist ein Buch. means That is a book.", 1563703640076581025L, "en", "Capitalizing nouns", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Noun capitalization also applies in the middle of a sentence.", 1L },
                    { 1563703640076581136L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Học danh từ cùng giống ngữ pháp và mạo từ. der Tisch, die Tür và das Buch lần lượt là cái bàn, cánh cửa và quyển sách.", 1563703640076581026L, "vi", "Mạo từ xác định và giống", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Giống ngữ pháp không luôn suy ra từ giới tính hoặc nghĩa. Số nhiều Nominativ dùng die.", 1L },
                    { 1563703640076581137L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Learn a noun together with its grammatical gender and article. Examples are der Tisch, die Tür and das Buch.", 1563703640076581026L, "en", "Definite articles and gender", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Grammatical gender cannot always be inferred from meaning or natural gender. Nominative plural uses die.", 1L },
                    { 1563703640076581138L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Dùng ein với danh từ giống đực hoặc trung ở Nominativ, eine với giống cái. Das ist eine Tasche. nghĩa là Đây là một chiếc túi.", 1563703640076581027L, "vi", "Mạo từ bất định", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Mạo từ bất định không có dạng số nhiều tương ứng với ein.", 1L },
                    { 1563703640076581139L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Use ein with masculine and neuter nominative nouns, and eine with feminine nouns. Das ist eine Tasche. means This is a bag.", 1563703640076581027L, "en", "Indefinite articles", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "There is no corresponding plural form of the indefinite article ein.", 1L },
                    { 1563703640076581140L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Tiếng Đức có nhiều cách tạo số nhiều. das Buch thành die Bücher, die Tasche thành die Taschen.", 1563703640076581028L, "vi", "Số nhiều danh từ", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Học dạng số nhiều cùng mỗi danh từ, tránh áp dụng một đuôi chung cho mọi từ.", 1L },
                    { 1563703640076581141L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "German has several plural patterns. das Buch becomes die Bücher, and die Tasche becomes die Taschen.", 1563703640076581028L, "en", "Plural nouns", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Learn the plural with each noun instead of applying one ending to every word.", 1L },
                    { 1563703640076581142L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Dùng kein thay cho mạo từ bất định hoặc trước danh từ không có mạo từ để phủ định. Ich habe kein Auto. nghĩa là Tôi không có ô tô.", 1563703640076581029L, "vi", "Phủ định danh từ bằng kein", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "kein thay đổi đuôi theo giống, số và cách, như keine Zeit và keinen Kaffee.", 1L },
                    { 1563703640076581143L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Use kein in place of an indefinite article or before a noun without an article to negate it. Ich habe kein Auto. means I do not have a car.", 1563703640076581029L, "en", "Negating nouns with kein", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "kein changes ending by gender, number and case, as in keine Zeit and keinen Kaffee.", 1L },
                    { 1563703640076581144L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Dùng nicht để phủ định tính từ hoặc thành phần khác phù hợp. Das Buch ist nicht teuer. nghĩa là Quyển sách không đắt.", 1563703640076581030L, "vi", "Phủ định bằng nicht", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Vị trí nicht thay đổi theo phạm vi phủ định. Trong câu chính, phủ định toàn mệnh đề thường đặt về cuối, trước phần vị ngữ còn lại.", 1L },
                    { 1563703640076581145L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Use nicht to negate an adjective or another appropriate constituent. Das Buch ist nicht teuer. means The book is not expensive.", 1563703640076581030L, "en", "Negation with nicht", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "The position of nicht depends on its scope. Clause negation in a main clause commonly appears late, before remaining predicate material.", 1L },
                    { 1563703640076581146L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Từ sở hữu cho biết vật hoặc người thuộc về ai. Das ist meine Tasche. nghĩa là Đây là túi của tôi.", 1563703640076581031L, "vi", "Từ sở hữu đứng trước danh từ", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Đuôi thay đổi theo giống, số và cách của danh từ đi sau.", 1L },
                    { 1563703640076581147L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Possessive determiners identify whose person or thing is meant. Das ist meine Tasche. means This is my bag.", 1563703640076581031L, "en", "Possessive determiners", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "The ending changes with the gender, number and case of the following noun.", 1L },
                    { 1563703640076581148L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Tính từ dùng sau sein không nhận đuôi biến cách. Die Wohnung ist klein. nghĩa là Căn hộ nhỏ.", 1563703640076581032L, "vi", "Tính từ sau sein", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Phân biệt với tính từ đứng trước danh từ, như eine kleine Wohnung.", 1L },
                    { 1563703640076581149L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "An adjective used after sein has no inflectional ending. Die Wohnung ist klein. means The apartment is small.", 1563703640076581032L, "en", "Predicate adjectives", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Distinguish this from an adjective before a noun, as in eine kleine Wohnung.", 1L },
                    { 1563703640076581150L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Nhiều động từ như kaufen, sehen và haben có tân ngữ Akkusativ. Ich kaufe einen Apfel. nghĩa là Tôi mua một quả táo.", 1563703640076581033L, "vi", "Tân ngữ Akkusativ", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Mạo từ giống đực đổi der thành den, ein thành einen. die và das giữ nguyên ở Akkusativ.", 1L },
                    { 1563703640076581151L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Many verbs, including kaufen, sehen and haben, take an accusative object. Ich kaufe einen Apfel. means I am buying an apple.", 1563703640076581033L, "en", "Accusative objects", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Masculine der becomes den and ein becomes einen. die and das stay unchanged in the accusative.", 1L },
                    { 1563703640076581152L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Thay danh từ làm tân ngữ Akkusativ bằng đại từ tương ứng. Ich sehe ihn. nghĩa là Tôi nhìn thấy anh ấy.", 1563703640076581034L, "vi", "Đại từ Akkusativ", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Dạng đại từ được quyết định bởi chức năng trong câu, không chỉ bởi từ đứng gần nó.", 1L },
                    { 1563703640076581153L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Replace an accusative noun object with the matching pronoun. Ich sehe ihn. means I see him.", 1563703640076581034L, "en", "Accusative personal pronouns", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "A pronoun's form follows its role in the sentence, rather than merely a nearby word.", 1L },
                    { 1563703640076581154L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Một số động từ và giới từ yêu cầu Dativ. Ich helfe dem Kind. nghĩa là Tôi giúp đứa trẻ.", 1563703640076581035L, "vi", "Cách Dativ", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Mạo từ xác định Dativ là dem, der, dem, den theo giống đực, cái, trung và số nhiều. Danh từ số nhiều thường thêm -n nếu phù hợp.", 1L },
                    { 1563703640076581155L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Some verbs and prepositions require the dative. Ich helfe dem Kind. means I help the child.", 1563703640076581035L, "en", "The dative case", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Dative definite articles are dem, der, dem and den for masculine, feminine, neuter and plural. Plural nouns commonly add -n where applicable.", 1L },
                    { 1563703640076581156L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Dùng đại từ Dativ với động từ hoặc giới từ yêu cầu cách này. Kannst du mir helfen? nghĩa là Bạn giúp tôi được không?", 1563703640076581036L, "vi", "Đại từ Dativ", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Dạng lịch sự Ihnen được viết hoa.", 1L },
                    { 1563703640076581157L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Use a dative pronoun where a verb or preposition requires it. Kannst du mir helfen? means Can you help me?", 1563703640076581036L, "en", "Dative personal pronouns", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Formal Ihnen is capitalized.", 1L },
                    { 1563703640076581158L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Các giới từ mit, nach, bei, seit, von, zu và aus đi với Dativ. Ich fahre mit dem Bus. nghĩa là Tôi đi bằng xe buýt.", 1563703640076581037L, "vi", "Giới từ đi với Dativ", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Học giới từ theo nghĩa và cụm từ thường dùng, như aus Vietnam hoặc zum Bahnhof.", 1L },
                    { 1563703640076581159L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "The prepositions mit, nach, bei, seit, von, zu and aus take the dative. Ich fahre mit dem Bus. means I travel by bus.", 1563703640076581037L, "en", "Dative prepositions", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Learn prepositions with their meanings and common phrases, such as aus Vietnam or zum Bahnhof.", 1L },
                    { 1563703640076581160L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Các giới từ für, ohne, durch, gegen và um đi với Akkusativ. Das Geschenk ist für meinen Bruder. nghĩa là Món quà dành cho anh/em trai tôi.", 1563703640076581038L, "vi", "Giới từ đi với Akkusativ", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Phân biệt um trong giờ giấc với các cách dùng khác của cùng giới từ.", 1L },
                    { 1563703640076581161L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "The prepositions für, ohne, durch, gegen and um take the accusative. Das Geschenk ist für meinen Bruder. means The gift is for my brother.", 1563703640076581038L, "en", "Accusative prepositions", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Distinguish the time use of um from its other uses.", 1L },
                    { 1563703640076581162L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Với giới từ hai cách như in, an và auf, dùng Dativ cho vị trí và Akkusativ cho đích đến được xác lập. Ich bin in der Schule. và Ich gehe in die Schule. diễn đạt ở trường và đi vào/đến trường.", 1563703640076581039L, "vi", "Vị trí và đích đến", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Có chuyển động chưa đủ để chọn Akkusativ. Cần xem giới từ diễn đạt vị trí hay đích đến.", 1L },
                    { 1563703640076581163L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "With two-way prepositions such as in, an and auf, use dative for location and accusative for an expressed destination. Compare Ich bin in der Schule. with Ich gehe in die Schule.", 1563703640076581039L, "en", "Location and destination", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Movement alone does not determine the accusative. Check whether the preposition describes location or a destination.", 1L },
                    { 1563703640076581164L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Dùng am với ngày, um với giờ và im với tháng hoặc mùa. Am Montag lerne ich. nghĩa là Tôi học vào thứ hai.", 1563703640076581040L, "vi", "Giới từ thời gian", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "am là an dem và im là in dem trong các cách dùng tương ứng.", 1L },
                    { 1563703640076581165L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Use am with days, um with clock times and im with months or seasons. Am Montag lerne ich. means I study on Monday.", 1563703640076581040L, "en", "Time prepositions", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "am contracts an dem and im contracts in dem in the corresponding expressions.", 1L },
                    { 1563703640076581166L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Có thể nói giờ chính thức bằng số giờ rồi Uhr và số phút. Es ist acht Uhr dreißig. nghĩa là Bây giờ là tám giờ ba mươi.", 1563703640076581041L, "vi", "Nói giờ", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Trong cách nói thông thường, halb neun là tám giờ rưỡi, hướng tới giờ thứ chín.", 1L },
                    { 1563703640076581167L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Formal clock time can use the hour, Uhr and the minutes. Es ist acht Uhr dreißig. means It is eight thirty.", 1563703640076581041L, "en", "Telling the time", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "In everyday speech, halb neun means half past eight, approaching the ninth hour.", 1L },
                    { 1563703640076581168L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Từ 21 đến 99, đọc hàng đơn vị trước hàng chục, nối bằng und. einundzwanzig là 21.", 1563703640076581042L, "vi", "Số đếm", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Có dạng đặc biệt như sechzehn, siebzehn, dreißig và sechzig.", 1L },
                    { 1563703640076581169L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "For 21 to 99, the unit precedes the tens and is joined with und. einundzwanzig is 21.", 1563703640076581042L, "en", "Forming numbers", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Special forms include sechzehn, siebzehn, dreißig and sechzig.", 1L },
                    { 1563703640076581170L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Động từ tình thái đã chia đứng ở vị trí hai, động từ chính nguyên mẫu ở cuối. Ich kann Deutsch sprechen. nghĩa là Tôi có thể nói tiếng Đức.", 1563703640076581043L, "vi", "Khả năng với können", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Các dạng hiện tại là kann, kannst, kann, können, könnt, können.", 1L },
                    { 1563703640076581171L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "The conjugated modal occupies second position and the main infinitive goes at the end. Ich kann Deutsch sprechen. means I can speak German.", 1563703640076581043L, "en", "Ability with können", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "The present forms are kann, kannst, kann, können, könnt and können.", 1L },
                    { 1563703640076581172L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Dùng müssen để nói điều cần hoặc phải làm. Ich muss arbeiten. nghĩa là Tôi phải làm việc.", 1563703640076581044L, "vi", "Nghĩa vụ với müssen", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "nicht müssen nghĩa là không cần phải, thường không có nghĩa bị cấm.", 1L },
                    { 1563703640076581173L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Use müssen for necessity or obligation. Ich muss arbeiten. means I have to work.", 1563703640076581044L, "en", "Obligation with müssen", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "nicht müssen means not having to do something, generally not a prohibition.", 1L },
                    { 1563703640076581174L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Dùng dürfen để nói được phép. Darf ich hier sitzen? nghĩa là Tôi ngồi đây được không?", 1563703640076581045L, "vi", "Xin phép và điều cấm", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Du darfst hier nicht rauchen. diễn đạt Bạn không được hút thuốc ở đây.", 1L },
                    { 1563703640076581175L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Use dürfen for permission. Darf ich hier sitzen? means May I sit here?", 1563703640076581045L, "en", "Permission with dürfen", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Du darfst hier nicht rauchen. means You are not allowed to smoke here.", 1L },
                    { 1563703640076581176L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "wollen diễn đạt mong muốn hoặc ý định. möchte thường dùng lịch sự hơn khi đề nghị hoặc gọi món. Ich möchte einen Tee. nghĩa là Tôi muốn một cốc trà.", 1563703640076581046L, "vi", "Mong muốn", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "möchte là một dạng của mögen, không phải nguyên mẫu độc lập.", 1L },
                    { 1563703640076581177L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "wollen expresses a wish or intention. möchte is often more polite for requests or ordering. Ich möchte einen Tee. means I would like a tea.", 1563703640076581046L, "en", "Wants and polite wishes", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "möchte is a form of mögen, not a separate infinitive.", 1L },
                    { 1563703640076581178L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Trong câu chính hiện tại, tiền tố tách của động từ được đưa về cuối. Ich stehe um sieben Uhr auf. nghĩa là Tôi thức dậy lúc bảy giờ.", 1563703640076581047L, "vi", "Động từ tách tiền tố", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Nguyên mẫu là aufstehen. Khi đi cùng động từ tình thái, nguyên mẫu giữ nguyên ở cuối.", 1L },
                    { 1563703640076581179L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "In a present-tense main clause, a separable prefix moves to the end. Ich stehe um sieben Uhr auf. means I get up at seven.", 1563703640076581047L, "en", "Separable verbs", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "The infinitive is aufstehen. With a modal verb the infinitive stays intact at the end.", 1L },
                    { 1563703640076581180L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Chọn dạng yêu cầu theo du, ihr hoặc Sie. Kommen Sie bitte! nghĩa là Xin ông/bà hãy đến.", 1563703640076581048L, "vi", "Câu mệnh lệnh và yêu cầu", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Dạng Sie giữ đại từ sau động từ. bitte giúp lời yêu cầu lịch sự hơn nhưng ngữ cảnh vẫn quan trọng.", 1L },
                    { 1563703640076581181L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Choose the imperative for du, ihr or Sie. Kommen Sie bitte! means Please come, in formal address.", 1563703640076581048L, "en", "Imperatives and requests", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "The Sie form retains the pronoun after the verb. bitte softens a request, while context still matters.", 1L },
                    { 1563703640076581182L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Khi thời gian đứng đầu câu chính, chủ ngữ thường đứng sau động từ để giữ vị trí hai. Morgen arbeite ich. nghĩa là Ngày mai tôi làm việc.", 1563703640076581049L, "vi", "Đưa thời gian lên đầu câu", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Có thể nói Ich arbeite morgen. với cùng thông tin cơ bản nhưng trọng tâm khác.", 1L },
                    { 1563703640076581183L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "When a time expression begins the main clause, the subject commonly follows the verb to preserve verb-second order. Morgen arbeite ich. means I work tomorrow.", 1563703640076581049L, "en", "Fronting a time expression", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Ich arbeite morgen. gives similar basic information with a different focus.", 1L },
                    { 1563703640076581184L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "und, oder và aber nối các câu chính. Ich lerne Deutsch und meine Schwester lernt Englisch. nghĩa là Tôi học tiếng Đức và chị/em gái tôi học tiếng Anh.", 1563703640076581050L, "vi", "Nối câu chính", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Các liên từ này không chiếm vị trí thành phần đầu của mệnh đề được nối.", 1L },
                    { 1563703640076581185L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "und, oder and aber connect main clauses. Ich lerne Deutsch und meine Schwester lernt Englisch. means I study German and my sister studies English.", 1563703640076581050L, "en", "Coordinating clauses", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "These conjunctions do not occupy the first constituent position of the connected clause.", 1L },
                    { 1563703640076581186L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Trong mệnh đề phụ dùng weil, động từ đã chia đứng cuối. Ich lerne, weil ich Zeit habe. nghĩa là Tôi học vì tôi có thời gian.", 1563703640076581051L, "vi", "Lý do với weil", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Dùng dấu phẩy để ngăn mệnh đề phụ với mệnh đề chính.", 1L },
                    { 1563703640076581187L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "In a subordinate clause introduced by weil, the finite verb goes at the end. Ich lerne, weil ich Zeit habe. means I study because I have time.", 1563703640076581051L, "en", "Reasons with weil", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Separate the subordinate and main clauses with a comma.", 1L },
                    { 1563703640076581188L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Dùng dass để dẫn một nội dung phát biểu hoặc suy nghĩ. Ich weiß, dass du hier wohnst. nghĩa là Tôi biết bạn sống ở đây.", 1563703640076581052L, "vi", "Mệnh đề với dass", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Phân biệt liên từ dass với mạo từ hoặc đại từ das.", 1L },
                    { 1563703640076581189L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Use dass to introduce the content of a statement or thought. Ich weiß, dass du hier wohnst. means I know that you live here.", 1563703640076581052L, "en", "Clauses with dass", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Distinguish the conjunction dass from the article or pronoun das.", 1L },
                    { 1563703640076581190L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Nhiều động từ dùng haben và phân từ hai để kể việc đã xảy ra. Ich habe Deutsch gelernt. nghĩa là Tôi đã học tiếng Đức.", 1563703640076581053L, "vi", "Quá khứ Perfekt với haben", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Trợ động từ chia theo chủ ngữ; phân từ thường ở cuối câu chính.", 1L },
                    { 1563703640076581191L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Many verbs use haben and a past participle to report past events. Ich habe Deutsch gelernt. means I learned German.", 1563703640076581053L, "en", "The perfect with haben", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "The auxiliary agrees with the subject; the participle usually ends the main clause.", 1L },
                    { 1563703640076581192L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Một nhóm động từ, thường chỉ đổi vị trí hoặc trạng thái không có tân ngữ Akkusativ, dùng sein. Ich bin nach Hause gegangen. nghĩa là Tôi đã về nhà.", 1563703640076581054L, "vi", "Quá khứ Perfekt với sein", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Học trợ động từ cùng động từ. Không phải mọi động từ có chuyển động đều luôn dùng sein.", 1L },
                    { 1563703640076581193L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "A group of verbs, often intransitive changes of place or state, uses sein. Ich bin nach Hause gegangen. means I went home.", 1563703640076581054L, "en", "The perfect with sein", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Learn the auxiliary with the verb. Not every verb involving movement always uses sein.", 1L },
                    { 1563703640076581194L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Động từ thường thường có ge- và -t, như gelernt. Động từ mạnh có dạng cần học riêng, như gesehen và geschrieben.", 1563703640076581055L, "vi", "Phân từ hai", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Động từ tận cùng -ieren thường không thêm ge-, như studiert. Tiền tố tách và không tách cũng ảnh hưởng cách tạo.", 1L },
                    { 1563703640076581195L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Regular verbs commonly form the participle with ge- and -t, as in gelernt. Strong verbs have forms to learn individually, such as gesehen and geschrieben.", 1563703640076581055L, "en", "Past participles", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Verbs ending in -ieren generally omit ge-, as in studiert. Separable and inseparable prefixes also affect formation.", 1L },
                    { 1563703640076581196L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "war và hatte thường dùng để kể trạng thái hoặc việc có trong quá khứ. Ich war müde. nghĩa là Tôi đã mệt.", 1563703640076581056L, "vi", "Quá khứ của sein và haben", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Các dạng cần chia theo chủ ngữ, như du warst và wir hatten.", 1L },
                    { 1563703640076581197L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "war and hatte commonly describe past states or possession. Ich war müde. means I was tired.", 1563703640076581056L, "en", "Past forms of sein and haben", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Inflect these forms for the subject, as in du warst and wir hatten.", 1L },
                    { 1563703640076581198L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Có thể dùng hiện tại cùng thời gian tương lai để nói kế hoạch. Morgen fahre ich nach Berlin. nghĩa là Ngày mai tôi đi Berlin.", 1563703640076581057L, "vi", "Diễn đạt tương lai bằng hiện tại", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Thời gian hoặc ngữ cảnh giúp xác định nghĩa tương lai.", 1L },
                    { 1563703640076581199L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "The present tense with a future time expression can describe a plan. Morgen fahre ich nach Berlin. means I am going to Berlin tomorrow.", 1563703640076581057L, "en", "Future meaning with the present", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "The time expression or context establishes the future meaning.", 1L },
                    { 1563703640076581200L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Nhiều tính từ thêm -er ở so sánh hơn. Dieses Buch ist billiger als das andere. nghĩa là Quyển sách này rẻ hơn quyển kia.", 1563703640076581058L, "vi", "So sánh hơn", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Có biến âm và dạng bất quy tắc, như groß → größer và gut → besser.", 1L },
                    { 1563703640076581201L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Many adjectives add -er for the comparative. Dieses Buch ist billiger als das andere. means This book is cheaper than the other one.", 1563703640076581058L, "en", "Comparatives", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Some forms add an umlaut or are irregular, as in groß → größer and gut → besser.", 1L },
                    { 1563703640076581202L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Dạng trạng ngữ hoặc vị ngữ thường dùng am và -sten hoặc -esten. Dieses Zimmer ist am kleinsten. nghĩa là Phòng này nhỏ nhất.", 1563703640076581059L, "vi", "So sánh nhất", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "gut có dạng am besten. Dạng đứng trước danh từ còn nhận đuôi theo ngữ pháp của cụm danh từ.", 1L },
                    { 1563703640076581203L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Adverbial or predicative superlatives commonly use am with -sten or -esten. Dieses Zimmer ist am kleinsten. means This room is the smallest.", 1563703640076581059L, "en", "Superlatives", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "gut has am besten. A superlative before a noun also takes the appropriate noun-phrase ending.", 1L },
                    { 1563703640076581204L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Tính từ trước danh từ nhận đuôi theo mạo từ, giống, số và cách. ein kleines Zimmer nghĩa là một căn phòng nhỏ.", 1563703640076581060L, "vi", "Tính từ đứng trước danh từ", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Học các mẫu như der kleine Tisch, ein kleiner Tisch và mit einem kleinen Tisch.", 1L },
                    { 1563703640076581205L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "An adjective before a noun takes an ending based on the determiner, gender, number and case. ein kleines Zimmer means a small room.", 1563703640076581060L, "en", "Adjectives before nouns", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Learn patterns such as der kleine Tisch, ein kleiner Tisch and mit einem kleinen Tisch.", 1L },
                    { 1563703640076581206L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "dieser, diese và dieses giúp chỉ rõ đối tượng. Dieses Buch ist neu. nghĩa là Quyển sách này mới.", 1563703640076581061L, "vi", "Từ chỉ định", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Đuôi thay đổi theo giống, số và cách, tương tự nhóm mạo từ xác định.", 1L },
                    { 1563703640076581207L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "dieser, diese and dieses identify a particular referent. Dieses Buch ist neu. means This book is new.", 1563703640076581061L, "en", "Demonstrative determiners", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "The endings change with gender, number and case, similarly to definite determiners.", 1L },
                    { 1563703640076581208L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "viel thường dùng với lượng không đếm được, viele với danh từ đếm được số nhiều. viel Wasser và viele Bücher nghĩa là nhiều nước và nhiều sách.", 1563703640076581062L, "vi", "Từ chỉ lượng", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Các từ này cũng thay đổi theo cấu trúc và chức năng. Ví dụ này giới thiệu dạng cơ bản.", 1L },
                    { 1563703640076581209L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "viel commonly describes an uncountable amount and viele countable plural items. Compare viel Wasser and viele Bücher.", 1563703640076581062L, "en", "Quantity expressions", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Their forms also depend on structure and grammatical role. These examples introduce the basic pattern.", 1L },
                    { 1563703640076581210L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Đặt gern trong câu để diễn đạt thích thực hiện hành động. Ich lese gern. nghĩa là Tôi thích đọc sách.", 1563703640076581063L, "vi", "Thích làm gì với gern", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "So sánh mức thích bằng lieber và am liebsten.", 1L },
                    { 1563703640076581211L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Use gern to express enjoying an activity. Ich lese gern. means I like reading.", 1563703640076581063L, "en", "Liking activities with gern", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Compare preferences with lieber and am liebsten.", 1L },
                    { 1563703640076581212L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Könnten Sie bitte giúp lời nhờ mềm hơn. Könnten Sie bitte langsam sprechen? nghĩa là Ông/bà có thể nói chậm được không?", 1563703640076581064L, "vi", "Nhờ và yêu cầu lịch sự", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Ich hätte gern thường dùng khi gọi món hoặc yêu cầu một thứ, như Ich hätte gern einen Kaffee.", 1L },
                    { 1563703640076581213L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Könnten Sie bitte softens a request. Könnten Sie bitte langsam sprechen? means Could you please speak slowly?", 1563703640076581064L, "en", "Polite requests", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Ich hätte gern is common for ordering or requesting an item, as in Ich hätte gern einen Kaffee.", 1L },
                    { 1563703640076581214L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Dùng es gibt để thông báo có hoặc tồn tại một thứ gì. Hier gibt es einen Bahnhof. nghĩa là Ở đây có một nhà ga.", 1563703640076581065L, "vi", "Có một thứ gì ở đâu", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Danh từ sau es gibt ở Akkusativ, kể cả khi động từ vẫn là gibt.", 1L },
                    { 1563703640076581215L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Use es gibt to state that something exists or is available. Hier gibt es einen Bahnhof. means There is a station here.", 1563703640076581065L, "en", "es gibt for existence", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "The noun after es gibt is accusative, while the verb remains gibt.", 1L }
                });

            migrationBuilder.InsertData(
                table: "pronunciation_examples",
                columns: new[] { "id", "audio_url", "code", "created_at", "pronunciation_rule_id", "sort_order", "text", "transcription", "transcription_system", "updated_at", "version" },
                values: new object[,]
                {
                    { 1563703640076579416L, null, "example-0", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1563703640076579316L, 0, "Beet", "beːt", "ipa", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579417L, null, "example-1", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1563703640076579316L, 1, "Bett", "bɛt", "ipa", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579418L, null, "example-2", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1563703640076579316L, 2, "Miete", "ˈmiːtə", "ipa", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579419L, null, "example-3", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1563703640076579316L, 3, "Mitte", "ˈmɪtə", "ipa", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579420L, null, "example-4", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1563703640076579316L, 4, "Ofen", "ˈoːfən", "ipa", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579421L, null, "example-5", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1563703640076579317L, 0, "fünf", "fʏnf", "ipa", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579422L, null, "example-6", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1563703640076579317L, 1, "schön", "ʃøːn", "ipa", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579423L, null, "example-7", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1563703640076579317L, 2, "Tür", "tyːɐ̯", "ipa", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579424L, null, "example-8", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1563703640076579317L, 3, "Bär", "bɛːɐ̯", "ipa", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579425L, null, "example-9", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1563703640076579317L, 4, "Mütter", "ˈmʏtɐ", "ipa", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579426L, null, "example-10", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1563703640076579318L, 0, "Eis", "aɪs", "ipa", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579427L, null, "example-11", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1563703640076579318L, 1, "Reise", "ˈʁaɪzə", "ipa", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579428L, null, "example-12", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1563703640076579318L, 2, "Wein", "vaɪn", "ipa", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579429L, null, "example-13", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1563703640076579318L, 3, "Liebe", "ˈliːbə", "ipa", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579430L, null, "example-14", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1563703640076579318L, 4, "viel", "fiːl", "ipa", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579431L, null, "example-15", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1563703640076579319L, 0, "Haus", "haʊs", "ipa", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579432L, null, "example-16", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1563703640076579319L, 1, "heute", "ˈhɔʏtə", "ipa", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579433L, null, "example-17", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1563703640076579319L, 2, "neu", "nɔʏ", "ipa", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579434L, null, "example-18", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1563703640076579319L, 3, "Häuser", "ˈhɔʏzɐ", "ipa", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579435L, null, "example-19", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1563703640076579319L, 4, "Auto", "ˈaʊto", "ipa", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579436L, null, "example-20", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1563703640076579320L, 0, "ich", "ɪç", "ipa", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579437L, null, "example-21", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1563703640076579320L, 1, "nicht", "nɪçt", "ipa", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579438L, null, "example-22", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1563703640076579320L, 2, "Nacht", "naxt", "ipa", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579439L, null, "example-23", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1563703640076579320L, 3, "Buch", "buːx", "ipa", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579440L, null, "example-24", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1563703640076579320L, 4, "Küche", "ˈkʏçə", "ipa", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579441L, null, "example-25", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1563703640076579321L, 0, "Schule", "ˈʃuːlə", "ipa", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579442L, null, "example-26", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1563703640076579321L, 1, "spielen", "ˈʃpiːlən", "ipa", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579443L, null, "example-27", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1563703640076579321L, 2, "Straße", "ˈʃtʁaːsə", "ipa", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579444L, null, "example-28", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1563703640076579321L, 3, "Fisch", "fɪʃ", "ipa", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579445L, null, "example-29", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1563703640076579321L, 4, "Stadt", "ʃtat", "ipa", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579446L, null, "example-30", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1563703640076579322L, 0, "Zeit", "tsaɪt", "ipa", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579447L, null, "example-31", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1563703640076579322L, 1, "Wasser", "ˈvasɐ", "ipa", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579448L, null, "example-32", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1563703640076579322L, 2, "Vater", "ˈfaːtɐ", "ipa", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579449L, null, "example-33", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1563703640076579322L, 3, "vier", "fiːɐ̯", "ipa", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579450L, null, "example-34", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1563703640076579322L, 4, "Vase", "ˈvaːzə", "ipa", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579451L, null, "example-35", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1563703640076579323L, 0, "Sonne", "ˈzɔnə", "ipa", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579452L, null, "example-36", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1563703640076579323L, 1, "groß", "ɡʁoːs", "ipa", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579453L, null, "example-37", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1563703640076579323L, 2, "Fuß", "fuːs", "ipa", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579454L, null, "example-38", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1563703640076579323L, 3, "Fluss", "flʊs", "ipa", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579455L, null, "example-39", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1563703640076579323L, 4, "heißen", "ˈhaɪsən", "ipa", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579456L, null, "example-40", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1563703640076579324L, 0, "Tag", "taːk", "ipa", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579457L, null, "example-41", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1563703640076579324L, 1, "Hund", "hʊnt", "ipa", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579458L, null, "example-42", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1563703640076579324L, 2, "lieb", "liːp", "ipa", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579459L, null, "example-43", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1563703640076579324L, 3, "Bad", "baːt", "ipa", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579460L, null, "example-44", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1563703640076579324L, 4, "Zug", "tsuːk", "ipa", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579461L, null, "example-45", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1563703640076579325L, 0, "rot", "ʁoːt", "ipa", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579462L, null, "example-46", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1563703640076579325L, 1, "Brot", "bʁoːt", "ipa", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579463L, null, "example-47", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1563703640076579325L, 2, "Mutter", "ˈmʊtɐ", "ipa", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579464L, null, "example-48", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1563703640076579325L, 3, "vier", "fiːɐ̯", "ipa", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579465L, null, "example-49", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1563703640076579325L, 4, "Lehrer", "ˈleːʁɐ", "ipa", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579466L, null, "example-50", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1563703640076579326L, 0, "sehen", "ˈzeːən", "ipa", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579467L, null, "example-51", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1563703640076579326L, 1, "gehen", "ˈɡeːən", "ipa", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579468L, null, "example-52", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1563703640076579326L, 2, "Uhr", "uːɐ̯", "ipa", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579469L, null, "example-53", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1563703640076579326L, 3, "wohnen", "ˈvoːnən", "ipa", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579470L, null, "example-54", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1563703640076579326L, 4, "fahren", "ˈfaːʁən", "ipa", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579471L, null, "example-55", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1563703640076579327L, 0, "bezahlen", "bəˈtsaːlən", "ipa", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579472L, null, "example-56", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1563703640076579327L, 1, "verstehen", "fɛɐ̯ˈʃteːən", "ipa", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579473L, null, "example-57", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1563703640076579327L, 2, "aufstehen", "ˈaʊfˌʃteːən", "ipa", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579474L, null, "example-58", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1563703640076579327L, 3, "arbeiten", "ˈaʁbaɪtən", "ipa", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L },
                    { 1563703640076579475L, null, "example-59", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1563703640076579327L, 4, "lernen", "ˈlɛʁnən", "ipa", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L }
                });

            migrationBuilder.InsertData(
                table: "pronunciation_rule_translations",
                columns: new[] { "id", "created_at", "explanation", "locale", "name", "pronunciation_rule_id", "updated_at", "usage_note", "version" },
                values: new object[,]
                {
                    { 1563703640076579366L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Độ dài và chất lượng nguyên âm có thể phân biệt từ. So sánh Beet với Bett và Miete với Mitte. Dấu ː trong IPA biểu thị độ dài.", "vi", "Nguyên âm dài và ngắn", 1563703640076579316L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Không suy ra toàn bộ cách đọc chỉ từ một chữ cái. Học độ dài cùng từng từ và ngữ cảnh.", 1L },
                    { 1563703640076579367L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Vowel length and quality can distinguish words. Compare Beet with Bett and Miete with Mitte. IPA ː marks length.", "en", "Long and short vowels", 1563703640076579316L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Do not infer the entire pronunciation from a single letter. Learn length with the word and context.", 1L },
                    { 1563703640076579368L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "ä, ö và ü biểu thị những nguyên âm cần phân biệt với a, o và u. Với ö và ü, giữ môi tròn trong khi đưa lưỡi về phía trước.", "vi", "Nguyên âm ä, ö, ü", 1563703640076579317L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Cả ö và ü đều có biến thể dài/ngắn. ä cũng thay đổi theo từ và cách nói chuẩn vùng miền.", 1L },
                    { 1563703640076579369L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "ä, ö and ü represent vowels to distinguish from a, o and u. For ö and ü, round the lips while keeping the tongue forward.", "en", "Umlaut vowels", 1563703640076579317L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Both ö and ü have long and short variants. ä also varies by word and regional standard.", 1L },
                    { 1563703640076579370L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "ei thường biểu thị âm đôi /aɪ/, còn ie thường biểu thị nguyên âm /iː/. So sánh Wein và viel.", "vi", "Phân biệt ei và ie", 1563703640076579318L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Đây là mẫu đọc thông dụng. Từ mượn và một số cách viết riêng có thể khác.", 1L },
                    { 1563703640076579371L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "ei commonly represents the diphthong /aɪ/, while ie commonly represents /iː/. Compare Wein and viel.", "en", "Distinguishing ei and ie", 1563703640076579318L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "These are common patterns. Loanwords and individual spellings can differ.", 1L },
                    { 1563703640076579372L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "au thường đọc /aʊ/, eu và äu thường đọc /ɔʏ/. So sánh Haus với Häuser.", "vi", "Âm đôi au, eu và äu", 1563703640076579319L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Giữ hai phần của âm đôi trong cùng một âm tiết.", 1L },
                    { 1563703640076579373L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "au commonly represents /aʊ/, while eu and äu commonly represent /ɔʏ/. Compare Haus and Häuser.", "en", "Diphthongs au, eu and äu", 1563703640076579319L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Keep the two vowel movements within one syllable.", 1L },
                    { 1563703640076579374L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "ch thường đọc /ç/ sau nguyên âm trước như i, và /x/ sau a, o, u. So sánh ich với Buch.", "vi", "Hai cách đọc ch", 1563703640076579320L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Đây là hướng dẫn cơ bản. ch ở đầu từ hoặc trong từ mượn có thể có cách đọc khác.", 1L },
                    { 1563703640076579375L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "ch commonly represents /ç/ after front vowels such as i, and /x/ after a, o or u. Compare ich and Buch.", "en", "The ich and ach sounds", 1563703640076579320L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "This is an introductory guide. Word-initial ch and loanwords can have other pronunciations.", 1L },
                    { 1563703640076579376L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "sch thường đọc /ʃ/. sp và st thường bắt đầu bằng /ʃp/ và /ʃt/ ở đầu thân từ, như spielen và Straße.", "vi", "Các nhóm sch, sp, st", 1563703640076579321L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Không áp dụng máy móc cách đọc đầu từ cho mọi sp hoặc st ở giữa và cuối từ.", 1L },
                    { 1563703640076579377L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "sch commonly represents /ʃ/. At the start of a word stem, sp and st commonly represent /ʃp/ and /ʃt/, as in spielen and Straße.", "en", "The groups sch, sp and st", 1563703640076579321L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Do not apply the word-initial pattern mechanically to every medial or final sp or st.", 1L },
                    { 1563703640076579378L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "z thường đọc /ts/, w thường đọc /v/. v đọc /f/ trong nhiều từ gốc Đức như Vater nhưng có thể đọc /v/ trong từ mượn như Vase.", "vi", "Cách đọc z, w và v", 1563703640076579322L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Tên chữ cái khác âm của chữ trong từ. V có tên /faʊ/ nhưng từ Vater bắt đầu bằng /f/.", 1L },
                    { 1563703640076579379L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "z commonly represents /ts/ and w commonly /v/. v represents /f/ in many native words such as Vater, but can represent /v/ in loanwords such as Vase.", "en", "The letters z, w and v", 1563703640076579322L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Letter names differ from their sounds within words. V is named /faʊ/, but Vater begins with /f/.", 1L },
                    { 1563703640076579380L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "s trước nguyên âm thường đọc /z/ ở đầu âm tiết trong nhiều cách nói chuẩn. ss và ß thường đọc /s/. Cách viết ss/ß liên quan độ dài nguyên âm trước đó.", "vi", "Phân biệt s, ss và ß", 1563703640076579323L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Sau nguyên âm ngắn thường viết ss, sau nguyên âm dài hoặc âm đôi thường viết ß. Quy ước tiếng Đức ở Thụy Sĩ dùng ss thay ß.", 1L },
                    { 1563703640076579381L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Syllable-initial s before a vowel commonly represents /z/ in many standard varieties. ss and ß commonly represent /s/. Their spelling relates to the preceding vowel's length.", "en", "Distinguishing s, ss and ß", 1563703640076579323L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "A short vowel commonly precedes ss, while a long vowel or diphthong commonly precedes ß. Swiss German spelling uses ss in place of ß.", 1L },
                    { 1563703640076579382L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Các phụ âm b, d và g thường vô thanh ở cuối âm tiết. Tag kết thúc bằng /k/, Hund bằng /t/ và lieb bằng /p/.", "vi", "Vô thanh hóa phụ âm cuối", 1563703640076579324L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Cách viết vẫn giữ phụ âm gốc. Biến đổi dạng từ có thể làm âm hữu thanh xuất hiện lại.", 1L },
                    { 1563703640076579383L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "b, d and g commonly devoice at syllable endings. Tag ends in /k/, Hund in /t/ and lieb in /p/.", "en", "Final devoicing", 1563703640076579324L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "The spelling retains the underlying consonant. Another form of the word can reveal voicing again.", 1L },
                    { 1563703640076579384L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Tiếng Đức chuẩn chấp nhận nhiều cách phát âm r. Trong nhiều cách nói, r sau nguyên âm được nguyên âm hóa, như đuôi -er trong Mutter.", "vi", "Âm r và các biến thể", 1563703640076579325L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Ví dụ dùng một cách phiên âm rộng thông dụng. Khác biệt vùng miền phù hợp vẫn có thể đúng.", 1L },
                    { 1563703640076579385L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Standard German allows several realizations of r. In many varieties, post-vocalic r is vocalized, as in the ending -er in Mutter.", "en", "Pronouncing r", 1563703640076579325L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "The examples use one common broad transcription. Appropriate regional variants can also be correct.", 1L },
                    { 1563703640076579386L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Trong nhiều từ, h sau nguyên âm chỉ độ dài và không được đọc như h ở đầu từ. Ví dụ gồm wohnen và fahren.", "vi", "Chữ h chỉ độ dài nguyên âm", 1563703640076579326L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Phân biệt với h đầu từ trong Haus, được phát âm. Đối chiếu toàn bộ từ.", 1L },
                    { 1563703640076579387L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "In many words, h after a vowel signals length and is not pronounced like word-initial h. Examples include wohnen and fahren.", "en", "Length-marking h", 1563703640076579326L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Distinguish word-initial h in Haus, which is pronounced. Check the whole word.", 1L },
                    { 1563703640076579388L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Học trọng âm cùng từ mới. IPA ˈ đặt trước âm tiết mang trọng âm chính. Tiền tố và nguồn gốc từ có thể ảnh hưởng vị trí trọng âm.", "vi", "Trọng âm từ", 1563703640076579327L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Tiền tố tách thường mang trọng âm; nhiều tiền tố không tách như be- và ver- thường không mang trọng âm chính.", 1L },
                    { 1563703640076579389L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Learn stress with new words. IPA ˈ precedes the syllable with primary stress. Prefixes and a word's origin can affect stress placement.", "en", "Word stress", 1563703640076579327L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Separable prefixes are commonly stressed; many inseparable prefixes such as be- and ver- commonly lack primary stress.", 1L }
                });

            migrationBuilder.InsertData(
                table: "vocabulary_senses",
                columns: new[] { "id", "code", "created_at", "part_of_speech", "sort_order", "updated_at", "version", "vocabulary_entry_id" },
                values: new object[,]
                {
                    { 1563703640076580116L, "hello", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Interjection", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076579816L },
                    { 1563703640076580117L, "morning", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Interjection", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076579817L },
                    { 1563703640076580118L, "day-greeting", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Interjection", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076579818L },
                    { 1563703640076580119L, "evening-greeting", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Interjection", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076579819L },
                    { 1563703640076580120L, "goodbye", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Interjection", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076579820L },
                    { 1563703640076580121L, "bye", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Interjection", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076579821L },
                    { 1563703640076580122L, "thanks", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Interjection", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076579822L },
                    { 1563703640076580123L, "please", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Interjection", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076579823L },
                    { 1563703640076580124L, "sorry", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Interjection", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076579824L },
                    { 1563703640076580125L, "yes", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Interjection", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076579825L },
                    { 1563703640076580126L, "no", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Interjection", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076579826L },
                    { 1563703640076580127L, "i", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Pronoun", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076579827L },
                    { 1563703640076580128L, "you-informal", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Pronoun", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076579828L },
                    { 1563703640076580129L, "he", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Pronoun", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076579829L },
                    { 1563703640076580130L, "she-they", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Pronoun", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076579830L },
                    { 1563703640076580131L, "it", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Pronoun", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076579831L },
                    { 1563703640076580132L, "we", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Pronoun", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076579832L },
                    { 1563703640076580133L, "you-plural", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Pronoun", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076579833L },
                    { 1563703640076580134L, "you-formal", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Pronoun", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076579834L },
                    { 1563703640076580135L, "who", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Pronoun", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076579835L },
                    { 1563703640076580136L, "what", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Pronoun", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076579836L },
                    { 1563703640076580137L, "where", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Adverb", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076579837L },
                    { 1563703640076580138L, "when", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Adverb", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076579838L },
                    { 1563703640076580139L, "how", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Adverb", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076579839L },
                    { 1563703640076580140L, "zero", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Numeral", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076579840L },
                    { 1563703640076580141L, "one", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Numeral", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076579841L },
                    { 1563703640076580142L, "two", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Numeral", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076579842L },
                    { 1563703640076580143L, "three", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Numeral", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076579843L },
                    { 1563703640076580144L, "four", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Numeral", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076579844L },
                    { 1563703640076580145L, "five", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Numeral", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076579845L },
                    { 1563703640076580146L, "six", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Numeral", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076579846L },
                    { 1563703640076580147L, "seven", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Numeral", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076579847L },
                    { 1563703640076580148L, "eight", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Numeral", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076579848L },
                    { 1563703640076580149L, "nine", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Numeral", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076579849L },
                    { 1563703640076580150L, "ten", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Numeral", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076579850L },
                    { 1563703640076580151L, "eleven", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Numeral", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076579851L },
                    { 1563703640076580152L, "twelve", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Numeral", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076579852L },
                    { 1563703640076580153L, "twenty", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Numeral", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076579853L },
                    { 1563703640076580154L, "hundred", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Numeral", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076579854L },
                    { 1563703640076580155L, "monday", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Noun", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076579855L },
                    { 1563703640076580156L, "tuesday", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Noun", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076579856L },
                    { 1563703640076580157L, "wednesday", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Noun", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076579857L },
                    { 1563703640076580158L, "thursday", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Noun", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076579858L },
                    { 1563703640076580159L, "friday", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Noun", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076579859L },
                    { 1563703640076580160L, "saturday", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Noun", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076579860L },
                    { 1563703640076580161L, "sunday", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Noun", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076579861L },
                    { 1563703640076580162L, "today", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Adverb", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076579862L },
                    { 1563703640076580163L, "tomorrow", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Adverb", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076579863L },
                    { 1563703640076580164L, "yesterday", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Adverb", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076579864L },
                    { 1563703640076580165L, "now", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Adverb", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076579865L },
                    { 1563703640076580166L, "always", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Adverb", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076579866L },
                    { 1563703640076580167L, "often", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Adverb", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076579867L },
                    { 1563703640076580168L, "sometimes", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Adverb", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076579868L },
                    { 1563703640076580169L, "never", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Adverb", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076579869L },
                    { 1563703640076580170L, "day", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Noun", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076579870L },
                    { 1563703640076580171L, "week", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Noun", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076579871L },
                    { 1563703640076580172L, "month", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Noun", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076579872L },
                    { 1563703640076580173L, "year", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Noun", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076579873L },
                    { 1563703640076580174L, "time", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Noun", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076579874L },
                    { 1563703640076580175L, "hour", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Noun", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076579875L },
                    { 1563703640076580176L, "minute", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Noun", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076579876L },
                    { 1563703640076580177L, "family", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Noun", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076579877L },
                    { 1563703640076580178L, "mother", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Noun", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076579878L },
                    { 1563703640076580179L, "father", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Noun", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076579879L },
                    { 1563703640076580180L, "brother", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Noun", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076579880L },
                    { 1563703640076580181L, "sister", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Noun", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076579881L },
                    { 1563703640076580182L, "child", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Noun", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076579882L },
                    { 1563703640076580183L, "man", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Noun", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076579883L },
                    { 1563703640076580184L, "woman", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Noun", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076579884L },
                    { 1563703640076580185L, "friend-male", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Noun", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076579885L },
                    { 1563703640076580186L, "friend-female", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Noun", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076579886L },
                    { 1563703640076580187L, "name", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Noun", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076579887L },
                    { 1563703640076580188L, "person", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Noun", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076579888L },
                    { 1563703640076580189L, "teacher-male", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Noun", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076579889L },
                    { 1563703640076580190L, "teacher-female", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Noun", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076579890L },
                    { 1563703640076580191L, "student", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Noun", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076579891L },
                    { 1563703640076580192L, "school", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Noun", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076579892L },
                    { 1563703640076580193L, "university", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Noun", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076579893L },
                    { 1563703640076580194L, "work", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Noun", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076579894L },
                    { 1563703640076580195L, "company", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Noun", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076579895L },
                    { 1563703640076580196L, "doctor", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Noun", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076579896L },
                    { 1563703640076580197L, "hospital", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Noun", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076579897L },
                    { 1563703640076580198L, "pharmacy", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Noun", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076579898L },
                    { 1563703640076580199L, "bank", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Noun", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076579899L },
                    { 1563703640076580200L, "station", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Noun", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076579900L },
                    { 1563703640076580201L, "airport", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Noun", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076579901L },
                    { 1563703640076580202L, "city", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Noun", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076579902L },
                    { 1563703640076580203L, "country", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Noun", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076579903L },
                    { 1563703640076580204L, "street", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Noun", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076579904L },
                    { 1563703640076580205L, "house", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Noun", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076579905L },
                    { 1563703640076580206L, "apartment", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Noun", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076579906L },
                    { 1563703640076580207L, "room", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Noun", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076579907L },
                    { 1563703640076580208L, "kitchen", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Noun", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076579908L },
                    { 1563703640076580209L, "bathroom", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Noun", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076579909L },
                    { 1563703640076580210L, "hotel", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Noun", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076579910L },
                    { 1563703640076580211L, "restaurant", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Noun", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076579911L },
                    { 1563703640076580212L, "supermarket", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Noun", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076579912L },
                    { 1563703640076580213L, "shop", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Noun", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076579913L },
                    { 1563703640076580214L, "water", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Noun", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076579914L },
                    { 1563703640076580215L, "coffee", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Noun", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076579915L },
                    { 1563703640076580216L, "tea", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Noun", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076579916L },
                    { 1563703640076580217L, "milk", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Noun", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076579917L },
                    { 1563703640076580218L, "bread", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Noun", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076579918L },
                    { 1563703640076580219L, "roll", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Noun", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076579919L },
                    { 1563703640076580220L, "egg", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Noun", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076579920L },
                    { 1563703640076580221L, "cheese", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Noun", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076579921L },
                    { 1563703640076580222L, "meat", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Noun", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076579922L },
                    { 1563703640076580223L, "fish", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Noun", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076579923L },
                    { 1563703640076580224L, "apple", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Noun", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076579924L },
                    { 1563703640076580225L, "banana", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Noun", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076579925L },
                    { 1563703640076580226L, "potato", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Noun", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076579926L },
                    { 1563703640076580227L, "rice", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Noun", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076579927L },
                    { 1563703640076580228L, "soup", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Noun", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076579928L },
                    { 1563703640076580229L, "breakfast", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Noun", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076579929L },
                    { 1563703640076580230L, "money", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Noun", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076579930L },
                    { 1563703640076580231L, "price", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Noun", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076579931L },
                    { 1563703640076580232L, "ticket", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Noun", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076579932L },
                    { 1563703640076580233L, "car", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Noun", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076579933L },
                    { 1563703640076580234L, "bus", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Noun", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076579934L },
                    { 1563703640076580235L, "train", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Noun", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076579935L },
                    { 1563703640076580236L, "bicycle", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Noun", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076579936L },
                    { 1563703640076580237L, "book", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Noun", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076579937L },
                    { 1563703640076580238L, "phone", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Noun", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076579938L },
                    { 1563703640076580239L, "key", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Noun", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076579939L },
                    { 1563703640076580240L, "bag", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Noun", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076579940L },
                    { 1563703640076580241L, "table", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Noun", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076579941L },
                    { 1563703640076580242L, "chair", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Noun", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076579942L },
                    { 1563703640076580243L, "door", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Noun", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076579943L },
                    { 1563703640076580244L, "window", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Noun", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076579944L },
                    { 1563703640076580245L, "be", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Verb", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076579945L },
                    { 1563703640076580246L, "have", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Verb", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076579946L },
                    { 1563703640076580247L, "go", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Verb", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076579947L },
                    { 1563703640076580248L, "come", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Verb", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076579948L },
                    { 1563703640076580249L, "live", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Verb", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076579949L },
                    { 1563703640076580250L, "be-called", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Verb", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076579950L },
                    { 1563703640076580251L, "speak", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Verb", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076579951L },
                    { 1563703640076580252L, "learn", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Verb", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076579952L },
                    { 1563703640076580253L, "work-verb", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Verb", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076579953L },
                    { 1563703640076580254L, "make", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Verb", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076579954L },
                    { 1563703640076580255L, "buy", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Verb", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076579955L },
                    { 1563703640076580256L, "pay", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Verb", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076579956L },
                    { 1563703640076580257L, "cost", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Verb", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076579957L },
                    { 1563703640076580258L, "need", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Verb", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076579958L },
                    { 1563703640076580259L, "search", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Verb", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076579959L },
                    { 1563703640076580260L, "find", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Verb", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076579960L },
                    { 1563703640076580261L, "eat", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Verb", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076579961L },
                    { 1563703640076580262L, "drink", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Verb", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076579962L },
                    { 1563703640076580263L, "cook", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Verb", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076579963L },
                    { 1563703640076580264L, "read", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Verb", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076579964L },
                    { 1563703640076580265L, "write", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Verb", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076579965L },
                    { 1563703640076580266L, "hear", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Verb", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076579966L },
                    { 1563703640076580267L, "see", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Verb", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076579967L },
                    { 1563703640076580268L, "sleep", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Verb", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076579968L },
                    { 1563703640076580269L, "drive", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Verb", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076579969L },
                    { 1563703640076580270L, "take", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Verb", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076579970L },
                    { 1563703640076580271L, "give", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Verb", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076579971L },
                    { 1563703640076580272L, "help", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Verb", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076579972L },
                    { 1563703640076580273L, "ask", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Verb", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076579973L },
                    { 1563703640076580274L, "answer", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Verb", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076579974L },
                    { 1563703640076580275L, "understand", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Verb", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076579975L },
                    { 1563703640076580276L, "know", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Verb", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076579976L },
                    { 1563703640076580277L, "can", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Auxiliary", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076579977L },
                    { 1563703640076580278L, "must", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Auxiliary", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076579978L },
                    { 1563703640076580279L, "want", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Auxiliary", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076579979L },
                    { 1563703640076580280L, "may", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Auxiliary", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076579980L },
                    { 1563703640076580281L, "should", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Auxiliary", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076579981L },
                    { 1563703640076580282L, "like", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Auxiliary", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076579982L },
                    { 1563703640076580283L, "good", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Adjective", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076579983L },
                    { 1563703640076580284L, "bad", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Adjective", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076579984L },
                    { 1563703640076580285L, "large", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Adjective", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076579985L },
                    { 1563703640076580286L, "small", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Adjective", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076579986L },
                    { 1563703640076580287L, "new", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Adjective", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076579987L },
                    { 1563703640076580288L, "old", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Adjective", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076579988L },
                    { 1563703640076580289L, "beautiful", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Adjective", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076579989L },
                    { 1563703640076580290L, "expensive", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Adjective", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076579990L },
                    { 1563703640076580291L, "inexpensive", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Adjective", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076579991L },
                    { 1563703640076580292L, "hot", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Adjective", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076579992L },
                    { 1563703640076580293L, "cold", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Adjective", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076579993L },
                    { 1563703640076580294L, "warm", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Adjective", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076579994L },
                    { 1563703640076580295L, "fast", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Adjective", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076579995L },
                    { 1563703640076580296L, "slow", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Adjective", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076579996L },
                    { 1563703640076580297L, "right", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Adjective", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076579997L },
                    { 1563703640076580298L, "wrong", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Adjective", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076579998L },
                    { 1563703640076580299L, "important", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Adjective", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076579999L },
                    { 1563703640076580300L, "tired", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Adjective", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076580000L },
                    { 1563703640076580301L, "happy", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Adjective", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076580001L },
                    { 1563703640076580302L, "ill", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Adjective", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076580002L },
                    { 1563703640076580303L, "healthy", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Adjective", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076580003L },
                    { 1563703640076580304L, "open", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Adjective", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076580004L },
                    { 1563703640076580305L, "closed", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Adjective", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076580005L },
                    { 1563703640076580306L, "here", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Adverb", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076580006L },
                    { 1563703640076580307L, "there", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Adverb", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076580007L },
                    { 1563703640076580308L, "left", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Adverb", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076580008L },
                    { 1563703640076580309L, "right-direction", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Adverb", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076580009L },
                    { 1563703640076580310L, "very", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Adverb", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076580010L },
                    { 1563703640076580311L, "also", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Adverb", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076580011L },
                    { 1563703640076580312L, "only", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Adverb", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076580012L },
                    { 1563703640076580313L, "and", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Conjunction", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076580013L },
                    { 1563703640076580314L, "or", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Conjunction", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076580014L },
                    { 1563703640076580315L, "but", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Conjunction", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076580015L }
                });

            migrationBuilder.InsertData(
                table: "writing_system_items",
                columns: new[] { "id", "code", "column_index", "created_at", "row_index", "sort_order", "symbol", "updated_at", "version", "writing_system_id" },
                values: new object[,]
                {
                    { 1563703640076578916L, "upper-a", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 0, 0, "A", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076578826L },
                    { 1563703640076578917L, "upper-b", 1, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 0, 1, "B", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076578826L },
                    { 1563703640076578918L, "upper-c", 2, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 0, 2, "C", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076578826L },
                    { 1563703640076578919L, "upper-d", 3, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 0, 3, "D", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076578826L },
                    { 1563703640076578920L, "upper-e", 4, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 0, 4, "E", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076578826L },
                    { 1563703640076578921L, "upper-f", 5, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 0, 5, "F", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076578826L },
                    { 1563703640076578922L, "upper-g", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1, 6, "G", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076578826L },
                    { 1563703640076578923L, "upper-h", 1, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1, 7, "H", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076578826L },
                    { 1563703640076578924L, "upper-i", 2, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1, 8, "I", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076578826L },
                    { 1563703640076578925L, "upper-j", 3, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1, 9, "J", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076578826L },
                    { 1563703640076578926L, "upper-k", 4, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1, 10, "K", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076578826L },
                    { 1563703640076578927L, "upper-l", 5, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1, 11, "L", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076578826L },
                    { 1563703640076578928L, "upper-m", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 2, 12, "M", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076578826L },
                    { 1563703640076578929L, "upper-n", 1, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 2, 13, "N", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076578826L },
                    { 1563703640076578930L, "upper-o", 2, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 2, 14, "O", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076578826L },
                    { 1563703640076578931L, "upper-p", 3, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 2, 15, "P", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076578826L },
                    { 1563703640076578932L, "upper-q", 4, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 2, 16, "Q", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076578826L },
                    { 1563703640076578933L, "upper-r", 5, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 2, 17, "R", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076578826L },
                    { 1563703640076578934L, "upper-s", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 3, 18, "S", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076578826L },
                    { 1563703640076578935L, "upper-t", 1, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 3, 19, "T", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076578826L },
                    { 1563703640076578936L, "upper-u", 2, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 3, 20, "U", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076578826L },
                    { 1563703640076578937L, "upper-v", 3, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 3, 21, "V", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076578826L },
                    { 1563703640076578938L, "upper-w", 4, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 3, 22, "W", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076578826L },
                    { 1563703640076578939L, "upper-x", 5, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 3, 23, "X", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076578826L },
                    { 1563703640076578940L, "upper-y", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 4, 24, "Y", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076578826L },
                    { 1563703640076578941L, "upper-z", 1, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 4, 25, "Z", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076578826L },
                    { 1563703640076578942L, "upper-ae", 2, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 4, 26, "Ä", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076578826L },
                    { 1563703640076578943L, "upper-oe", 3, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 4, 27, "Ö", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076578826L },
                    { 1563703640076578944L, "upper-ue", 4, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 4, 28, "Ü", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076578826L },
                    { 1563703640076578945L, "upper-eszett", 5, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 4, 29, "ẞ", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076578826L },
                    { 1563703640076578946L, "lower-a", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 5, 30, "a", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076578826L },
                    { 1563703640076578947L, "lower-b", 1, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 5, 31, "b", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076578826L },
                    { 1563703640076578948L, "lower-c", 2, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 5, 32, "c", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076578826L },
                    { 1563703640076578949L, "lower-d", 3, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 5, 33, "d", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076578826L },
                    { 1563703640076578950L, "lower-e", 4, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 5, 34, "e", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076578826L },
                    { 1563703640076578951L, "lower-f", 5, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 5, 35, "f", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076578826L },
                    { 1563703640076578952L, "lower-g", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 6, 36, "g", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076578826L },
                    { 1563703640076578953L, "lower-h", 1, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 6, 37, "h", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076578826L },
                    { 1563703640076578954L, "lower-i", 2, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 6, 38, "i", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076578826L },
                    { 1563703640076578955L, "lower-j", 3, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 6, 39, "j", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076578826L },
                    { 1563703640076578956L, "lower-k", 4, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 6, 40, "k", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076578826L },
                    { 1563703640076578957L, "lower-l", 5, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 6, 41, "l", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076578826L },
                    { 1563703640076578958L, "lower-m", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 7, 42, "m", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076578826L },
                    { 1563703640076578959L, "lower-n", 1, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 7, 43, "n", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076578826L },
                    { 1563703640076578960L, "lower-o", 2, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 7, 44, "o", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076578826L },
                    { 1563703640076578961L, "lower-p", 3, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 7, 45, "p", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076578826L },
                    { 1563703640076578962L, "lower-q", 4, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 7, 46, "q", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076578826L },
                    { 1563703640076578963L, "lower-r", 5, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 7, 47, "r", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076578826L },
                    { 1563703640076578964L, "lower-s", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 8, 48, "s", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076578826L },
                    { 1563703640076578965L, "lower-t", 1, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 8, 49, "t", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076578826L },
                    { 1563703640076578966L, "lower-u", 2, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 8, 50, "u", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076578826L },
                    { 1563703640076578967L, "lower-v", 3, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 8, 51, "v", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076578826L },
                    { 1563703640076578968L, "lower-w", 4, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 8, 52, "w", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076578826L },
                    { 1563703640076578969L, "lower-x", 5, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 8, 53, "x", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076578826L },
                    { 1563703640076578970L, "lower-y", 0, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 9, 54, "y", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076578826L },
                    { 1563703640076578971L, "lower-z", 1, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 9, 55, "z", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076578826L },
                    { 1563703640076578972L, "lower-ae", 2, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 9, 56, "ä", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076578826L },
                    { 1563703640076578973L, "lower-oe", 3, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 9, 57, "ö", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076578826L },
                    { 1563703640076578974L, "lower-ue", 4, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 9, 58, "ü", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076578826L },
                    { 1563703640076578975L, "lower-eszett", 5, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 9, 59, "ß", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076578826L }
                });

            migrationBuilder.InsertData(
                table: "writing_system_translations",
                columns: new[] { "id", "created_at", "description", "locale", "name", "updated_at", "version", "writing_system_id" },
                values: new object[,]
                {
                    { 1563703640076578836L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "26 chữ cái A–Z cùng Ä, Ö, Ü và ẞ, có cả chữ hoa và chữ thường, tổng cộng 60 mục luyện. Phiên âm IPA là tên chữ cái, cách đọc trong từ cần đối chiếu quy tắc phát âm.", "vi", "Chữ cái tiếng Đức", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076578826L },
                    { 1563703640076578837L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "The 26 letters A–Z plus Ä, Ö, Ü and ẞ in upper and lower case, giving 60 practice items. IPA transcriptions name the letters. Consult pronunciation rules for their sounds within words.", "en", "German alphabet", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076578826L }
                });

            migrationBuilder.InsertData(
                table: "handwriting_templates",
                columns: new[] { "id", "attribution", "created_at", "drawing", "drawing_format_version", "is_published", "license", "license_url", "revision", "source_revision", "source_url", "symbol", "updated_at", "version", "writing_system_item_id" },
                values: new object[,]
                {
                    { 1563703640076579116L, "The Hershey Fonts were originally created by Dr. A. V. Hershey while working at the U. S. National Bureau of Standards. The format of the Font data in this distribution was originally created by James Hurt, Cognition, Inc., 900 Technology Park Drive, Billerica, MA 01821 (mit-eddie!ci-dandelion!hurt). SVG conversion by Windell H. Oskay. RealityLoop sampled/normalized paths and adapted the capital eszett reference.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "[[{\"x\":0.5,\"y\":0.15},{\"x\":0.233535,\"y\":0.85}],[{\"x\":0.5,\"y\":0.15},{\"x\":0.766465,\"y\":0.85}],[{\"x\":0.332931,\"y\":0.617372},{\"x\":0.667069,\"y\":0.617372}]]", 1, true, "Hershey font redistribution license", "https://github.com/techninja/hersheytextjs/blob/262f4782cd412ee539eb43d4d9d9b92562d7590b/svg_fonts/HersheySans1.svg", 1, "262f4782cd412ee539eb43d4d9d9b92562d7590b+de-latin-v1", "https://github.com/techninja/hersheytextjs/blob/262f4782cd412ee539eb43d4d9d9b92562d7590b/svg_fonts/HersheySans1.svg", "A", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076578916L },
                    { 1563703640076579117L, "The Hershey Fonts were originally created by Dr. A. V. Hershey while working at the U. S. National Bureau of Standards. The format of the Font data in this distribution was originally created by James Hurt, Cognition, Inc., 900 Technology Park Drive, Billerica, MA 01821 (mit-eddie!ci-dandelion!hurt). SVG conversion by Windell H. Oskay. RealityLoop sampled/normalized paths and adapted the capital eszett reference.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "[[{\"x\":0.266314,\"y\":0.15},{\"x\":0.266314,\"y\":0.85}],[{\"x\":0.266314,\"y\":0.15},{\"x\":0.566616,\"y\":0.15},{\"x\":0.666012,\"y\":0.183837},{\"x\":0.699849,\"y\":0.217674},{\"x\":0.733686,\"y\":0.283233},{\"x\":0.733686,\"y\":0.350906},{\"x\":0.699849,\"y\":0.416465},{\"x\":0.666012,\"y\":0.450302},{\"x\":0.566616,\"y\":0.484139}],[{\"x\":0.266314,\"y\":0.484139},{\"x\":0.566616,\"y\":0.484139},{\"x\":0.666012,\"y\":0.516918},{\"x\":0.699849,\"y\":0.549698},{\"x\":0.733686,\"y\":0.617372},{\"x\":0.733686,\"y\":0.716767},{\"x\":0.699849,\"y\":0.783384},{\"x\":0.666012,\"y\":0.816692},{\"x\":0.566616,\"y\":0.85},{\"x\":0.266314,\"y\":0.85}]]", 1, true, "Hershey font redistribution license", "https://github.com/techninja/hersheytextjs/blob/262f4782cd412ee539eb43d4d9d9b92562d7590b/svg_fonts/HersheySans1.svg", 1, "262f4782cd412ee539eb43d4d9d9b92562d7590b+de-latin-v1", "https://github.com/techninja/hersheytextjs/blob/262f4782cd412ee539eb43d4d9d9b92562d7590b/svg_fonts/HersheySans1.svg", "B", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076578917L },
                    { 1563703640076579118L, "The Hershey Fonts were originally created by Dr. A. V. Hershey while working at the U. S. National Bureau of Standards. The format of the Font data in this distribution was originally created by James Hurt, Cognition, Inc., 900 Technology Park Drive, Billerica, MA 01821 (mit-eddie!ci-dandelion!hurt). SVG conversion by Windell H. Oskay. RealityLoop sampled/normalized paths and adapted the capital eszett reference.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "[[{\"x\":0.750076,\"y\":0.317069},{\"x\":0.716239,\"y\":0.250453},{\"x\":0.649622,\"y\":0.183837},{\"x\":0.583006,\"y\":0.15},{\"x\":0.449773,\"y\":0.15},{\"x\":0.383157,\"y\":0.183837},{\"x\":0.316541,\"y\":0.250453},{\"x\":0.282704,\"y\":0.317069},{\"x\":0.249924,\"y\":0.416465},{\"x\":0.249924,\"y\":0.583535},{\"x\":0.282704,\"y\":0.682931},{\"x\":0.316541,\"y\":0.750076},{\"x\":0.383157,\"y\":0.816692},{\"x\":0.449773,\"y\":0.85},{\"x\":0.583006,\"y\":0.85},{\"x\":0.649622,\"y\":0.816692},{\"x\":0.716239,\"y\":0.750076},{\"x\":0.750076,\"y\":0.682931}]]", 1, true, "Hershey font redistribution license", "https://github.com/techninja/hersheytextjs/blob/262f4782cd412ee539eb43d4d9d9b92562d7590b/svg_fonts/HersheySans1.svg", 1, "262f4782cd412ee539eb43d4d9d9b92562d7590b+de-latin-v1", "https://github.com/techninja/hersheytextjs/blob/262f4782cd412ee539eb43d4d9d9b92562d7590b/svg_fonts/HersheySans1.svg", "C", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076578918L },
                    { 1563703640076579119L, "The Hershey Fonts were originally created by Dr. A. V. Hershey while working at the U. S. National Bureau of Standards. The format of the Font data in this distribution was originally created by James Hurt, Cognition, Inc., 900 Technology Park Drive, Billerica, MA 01821 (mit-eddie!ci-dandelion!hurt). SVG conversion by Windell H. Oskay. RealityLoop sampled/normalized paths and adapted the capital eszett reference.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "[[{\"x\":0.266314,\"y\":0.15},{\"x\":0.266314,\"y\":0.85}],[{\"x\":0.266314,\"y\":0.15},{\"x\":0.5,\"y\":0.15},{\"x\":0.600453,\"y\":0.183837},{\"x\":0.666012,\"y\":0.250453},{\"x\":0.699849,\"y\":0.317069},{\"x\":0.733686,\"y\":0.416465},{\"x\":0.733686,\"y\":0.583535},{\"x\":0.699849,\"y\":0.682931},{\"x\":0.666012,\"y\":0.750076},{\"x\":0.600453,\"y\":0.816692},{\"x\":0.5,\"y\":0.85},{\"x\":0.266314,\"y\":0.85}]]", 1, true, "Hershey font redistribution license", "https://github.com/techninja/hersheytextjs/blob/262f4782cd412ee539eb43d4d9d9b92562d7590b/svg_fonts/HersheySans1.svg", 1, "262f4782cd412ee539eb43d4d9d9b92562d7590b+de-latin-v1", "https://github.com/techninja/hersheytextjs/blob/262f4782cd412ee539eb43d4d9d9b92562d7590b/svg_fonts/HersheySans1.svg", "D", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076578919L },
                    { 1563703640076579120L, "The Hershey Fonts were originally created by Dr. A. V. Hershey while working at the U. S. National Bureau of Standards. The format of the Font data in this distribution was originally created by James Hurt, Cognition, Inc., 900 Technology Park Drive, Billerica, MA 01821 (mit-eddie!ci-dandelion!hurt). SVG conversion by Windell H. Oskay. RealityLoop sampled/normalized paths and adapted the capital eszett reference.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "[[{\"x\":0.283233,\"y\":0.15},{\"x\":0.283233,\"y\":0.85}],[{\"x\":0.283233,\"y\":0.15},{\"x\":0.716767,\"y\":0.15}],[{\"x\":0.283233,\"y\":0.484139},{\"x\":0.549698,\"y\":0.484139}],[{\"x\":0.283233,\"y\":0.85},{\"x\":0.716767,\"y\":0.85}]]", 1, true, "Hershey font redistribution license", "https://github.com/techninja/hersheytextjs/blob/262f4782cd412ee539eb43d4d9d9b92562d7590b/svg_fonts/HersheySans1.svg", 1, "262f4782cd412ee539eb43d4d9d9b92562d7590b+de-latin-v1", "https://github.com/techninja/hersheytextjs/blob/262f4782cd412ee539eb43d4d9d9b92562d7590b/svg_fonts/HersheySans1.svg", "E", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076578920L },
                    { 1563703640076579121L, "The Hershey Fonts were originally created by Dr. A. V. Hershey while working at the U. S. National Bureau of Standards. The format of the Font data in this distribution was originally created by James Hurt, Cognition, Inc., 900 Technology Park Drive, Billerica, MA 01821 (mit-eddie!ci-dandelion!hurt). SVG conversion by Windell H. Oskay. RealityLoop sampled/normalized paths and adapted the capital eszett reference.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "[[{\"x\":0.283233,\"y\":0.15},{\"x\":0.283233,\"y\":0.85}],[{\"x\":0.283233,\"y\":0.15},{\"x\":0.716767,\"y\":0.15}],[{\"x\":0.283233,\"y\":0.484139},{\"x\":0.549698,\"y\":0.484139}]]", 1, true, "Hershey font redistribution license", "https://github.com/techninja/hersheytextjs/blob/262f4782cd412ee539eb43d4d9d9b92562d7590b/svg_fonts/HersheySans1.svg", 1, "262f4782cd412ee539eb43d4d9d9b92562d7590b+de-latin-v1", "https://github.com/techninja/hersheytextjs/blob/262f4782cd412ee539eb43d4d9d9b92562d7590b/svg_fonts/HersheySans1.svg", "F", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076578921L },
                    { 1563703640076579122L, "The Hershey Fonts were originally created by Dr. A. V. Hershey while working at the U. S. National Bureau of Standards. The format of the Font data in this distribution was originally created by James Hurt, Cognition, Inc., 900 Technology Park Drive, Billerica, MA 01821 (mit-eddie!ci-dandelion!hurt). SVG conversion by Windell H. Oskay. RealityLoop sampled/normalized paths and adapted the capital eszett reference.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "[[{\"x\":0.750076,\"y\":0.317069},{\"x\":0.716239,\"y\":0.250453},{\"x\":0.649622,\"y\":0.183837},{\"x\":0.583006,\"y\":0.15},{\"x\":0.449773,\"y\":0.15},{\"x\":0.383157,\"y\":0.183837},{\"x\":0.316541,\"y\":0.250453},{\"x\":0.282704,\"y\":0.317069},{\"x\":0.249924,\"y\":0.416465},{\"x\":0.249924,\"y\":0.583535},{\"x\":0.282704,\"y\":0.682931},{\"x\":0.316541,\"y\":0.750076},{\"x\":0.383157,\"y\":0.816692},{\"x\":0.449773,\"y\":0.85},{\"x\":0.583006,\"y\":0.85},{\"x\":0.649622,\"y\":0.816692},{\"x\":0.716239,\"y\":0.750076},{\"x\":0.750076,\"y\":0.682931},{\"x\":0.750076,\"y\":0.583535}],[{\"x\":0.583006,\"y\":0.583535},{\"x\":0.750076,\"y\":0.583535}]]", 1, true, "Hershey font redistribution license", "https://github.com/techninja/hersheytextjs/blob/262f4782cd412ee539eb43d4d9d9b92562d7590b/svg_fonts/HersheySans1.svg", 1, "262f4782cd412ee539eb43d4d9d9b92562d7590b+de-latin-v1", "https://github.com/techninja/hersheytextjs/blob/262f4782cd412ee539eb43d4d9d9b92562d7590b/svg_fonts/HersheySans1.svg", "G", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076578922L },
                    { 1563703640076579123L, "The Hershey Fonts were originally created by Dr. A. V. Hershey while working at the U. S. National Bureau of Standards. The format of the Font data in this distribution was originally created by James Hurt, Cognition, Inc., 900 Technology Park Drive, Billerica, MA 01821 (mit-eddie!ci-dandelion!hurt). SVG conversion by Windell H. Oskay. RealityLoop sampled/normalized paths and adapted the capital eszett reference.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "[[{\"x\":0.266314,\"y\":0.15},{\"x\":0.266314,\"y\":0.85}],[{\"x\":0.733686,\"y\":0.15},{\"x\":0.733686,\"y\":0.85}],[{\"x\":0.266314,\"y\":0.484139},{\"x\":0.733686,\"y\":0.484139}]]", 1, true, "Hershey font redistribution license", "https://github.com/techninja/hersheytextjs/blob/262f4782cd412ee539eb43d4d9d9b92562d7590b/svg_fonts/HersheySans1.svg", 1, "262f4782cd412ee539eb43d4d9d9b92562d7590b+de-latin-v1", "https://github.com/techninja/hersheytextjs/blob/262f4782cd412ee539eb43d4d9d9b92562d7590b/svg_fonts/HersheySans1.svg", "H", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076578923L },
                    { 1563703640076579124L, "The Hershey Fonts were originally created by Dr. A. V. Hershey while working at the U. S. National Bureau of Standards. The format of the Font data in this distribution was originally created by James Hurt, Cognition, Inc., 900 Technology Park Drive, Billerica, MA 01821 (mit-eddie!ci-dandelion!hurt). SVG conversion by Windell H. Oskay. RealityLoop sampled/normalized paths and adapted the capital eszett reference.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "[[{\"x\":0.5,\"y\":0.15},{\"x\":0.5,\"y\":0.85}]]", 1, true, "Hershey font redistribution license", "https://github.com/techninja/hersheytextjs/blob/262f4782cd412ee539eb43d4d9d9b92562d7590b/svg_fonts/HersheySans1.svg", 1, "262f4782cd412ee539eb43d4d9d9b92562d7590b+de-latin-v1", "https://github.com/techninja/hersheytextjs/blob/262f4782cd412ee539eb43d4d9d9b92562d7590b/svg_fonts/HersheySans1.svg", "I", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076578924L },
                    { 1563703640076579125L, "The Hershey Fonts were originally created by Dr. A. V. Hershey while working at the U. S. National Bureau of Standards. The format of the Font data in this distribution was originally created by James Hurt, Cognition, Inc., 900 Technology Park Drive, Billerica, MA 01821 (mit-eddie!ci-dandelion!hurt). SVG conversion by Windell H. Oskay. RealityLoop sampled/normalized paths and adapted the capital eszett reference.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "[[{\"x\":0.666012,\"y\":0.15},{\"x\":0.666012,\"y\":0.682931},{\"x\":0.633233,\"y\":0.783384},{\"x\":0.600453,\"y\":0.816692},{\"x\":0.532779,\"y\":0.85},{\"x\":0.467221,\"y\":0.85},{\"x\":0.399547,\"y\":0.816692},{\"x\":0.366767,\"y\":0.783384},{\"x\":0.333988,\"y\":0.682931},{\"x\":0.333988,\"y\":0.617372}]]", 1, true, "Hershey font redistribution license", "https://github.com/techninja/hersheytextjs/blob/262f4782cd412ee539eb43d4d9d9b92562d7590b/svg_fonts/HersheySans1.svg", 1, "262f4782cd412ee539eb43d4d9d9b92562d7590b+de-latin-v1", "https://github.com/techninja/hersheytextjs/blob/262f4782cd412ee539eb43d4d9d9b92562d7590b/svg_fonts/HersheySans1.svg", "J", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076578925L },
                    { 1563703640076579126L, "The Hershey Fonts were originally created by Dr. A. V. Hershey while working at the U. S. National Bureau of Standards. The format of the Font data in this distribution was originally created by James Hurt, Cognition, Inc., 900 Technology Park Drive, Billerica, MA 01821 (mit-eddie!ci-dandelion!hurt). SVG conversion by Windell H. Oskay. RealityLoop sampled/normalized paths and adapted the capital eszett reference.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "[[{\"x\":0.266314,\"y\":0.15},{\"x\":0.266314,\"y\":0.85}],[{\"x\":0.733686,\"y\":0.15},{\"x\":0.266314,\"y\":0.617372}],[{\"x\":0.433384,\"y\":0.450302},{\"x\":0.733686,\"y\":0.85}]]", 1, true, "Hershey font redistribution license", "https://github.com/techninja/hersheytextjs/blob/262f4782cd412ee539eb43d4d9d9b92562d7590b/svg_fonts/HersheySans1.svg", 1, "262f4782cd412ee539eb43d4d9d9b92562d7590b+de-latin-v1", "https://github.com/techninja/hersheytextjs/blob/262f4782cd412ee539eb43d4d9d9b92562d7590b/svg_fonts/HersheySans1.svg", "K", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076578926L },
                    { 1563703640076579127L, "The Hershey Fonts were originally created by Dr. A. V. Hershey while working at the U. S. National Bureau of Standards. The format of the Font data in this distribution was originally created by James Hurt, Cognition, Inc., 900 Technology Park Drive, Billerica, MA 01821 (mit-eddie!ci-dandelion!hurt). SVG conversion by Windell H. Oskay. RealityLoop sampled/normalized paths and adapted the capital eszett reference.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "[[{\"x\":0.300151,\"y\":0.15},{\"x\":0.300151,\"y\":0.85}],[{\"x\":0.300151,\"y\":0.85},{\"x\":0.699849,\"y\":0.85}]]", 1, true, "Hershey font redistribution license", "https://github.com/techninja/hersheytextjs/blob/262f4782cd412ee539eb43d4d9d9b92562d7590b/svg_fonts/HersheySans1.svg", 1, "262f4782cd412ee539eb43d4d9d9b92562d7590b+de-latin-v1", "https://github.com/techninja/hersheytextjs/blob/262f4782cd412ee539eb43d4d9d9b92562d7590b/svg_fonts/HersheySans1.svg", "L", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076578927L },
                    { 1563703640076579128L, "The Hershey Fonts were originally created by Dr. A. V. Hershey while working at the U. S. National Bureau of Standards. The format of the Font data in this distribution was originally created by James Hurt, Cognition, Inc., 900 Technology Park Drive, Billerica, MA 01821 (mit-eddie!ci-dandelion!hurt). SVG conversion by Windell H. Oskay. RealityLoop sampled/normalized paths and adapted the capital eszett reference.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "[[{\"x\":0.233535,\"y\":0.15},{\"x\":0.233535,\"y\":0.85}],[{\"x\":0.233535,\"y\":0.15},{\"x\":0.5,\"y\":0.85}],[{\"x\":0.766465,\"y\":0.15},{\"x\":0.5,\"y\":0.85}],[{\"x\":0.766465,\"y\":0.15},{\"x\":0.766465,\"y\":0.85}]]", 1, true, "Hershey font redistribution license", "https://github.com/techninja/hersheytextjs/blob/262f4782cd412ee539eb43d4d9d9b92562d7590b/svg_fonts/HersheySans1.svg", 1, "262f4782cd412ee539eb43d4d9d9b92562d7590b+de-latin-v1", "https://github.com/techninja/hersheytextjs/blob/262f4782cd412ee539eb43d4d9d9b92562d7590b/svg_fonts/HersheySans1.svg", "M", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076578928L },
                    { 1563703640076579129L, "The Hershey Fonts were originally created by Dr. A. V. Hershey while working at the U. S. National Bureau of Standards. The format of the Font data in this distribution was originally created by James Hurt, Cognition, Inc., 900 Technology Park Drive, Billerica, MA 01821 (mit-eddie!ci-dandelion!hurt). SVG conversion by Windell H. Oskay. RealityLoop sampled/normalized paths and adapted the capital eszett reference.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "[[{\"x\":0.266314,\"y\":0.15},{\"x\":0.266314,\"y\":0.85}],[{\"x\":0.266314,\"y\":0.15},{\"x\":0.733686,\"y\":0.85}],[{\"x\":0.733686,\"y\":0.15},{\"x\":0.733686,\"y\":0.85}]]", 1, true, "Hershey font redistribution license", "https://github.com/techninja/hersheytextjs/blob/262f4782cd412ee539eb43d4d9d9b92562d7590b/svg_fonts/HersheySans1.svg", 1, "262f4782cd412ee539eb43d4d9d9b92562d7590b+de-latin-v1", "https://github.com/techninja/hersheytextjs/blob/262f4782cd412ee539eb43d4d9d9b92562d7590b/svg_fonts/HersheySans1.svg", "N", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076578929L },
                    { 1563703640076579130L, "The Hershey Fonts were originally created by Dr. A. V. Hershey while working at the U. S. National Bureau of Standards. The format of the Font data in this distribution was originally created by James Hurt, Cognition, Inc., 900 Technology Park Drive, Billerica, MA 01821 (mit-eddie!ci-dandelion!hurt). SVG conversion by Windell H. Oskay. RealityLoop sampled/normalized paths and adapted the capital eszett reference.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "[[{\"x\":0.433384,\"y\":0.15},{\"x\":0.366767,\"y\":0.183837},{\"x\":0.300151,\"y\":0.250453},{\"x\":0.266314,\"y\":0.317069},{\"x\":0.233535,\"y\":0.416465},{\"x\":0.233535,\"y\":0.583535},{\"x\":0.266314,\"y\":0.682931},{\"x\":0.300151,\"y\":0.750076},{\"x\":0.366767,\"y\":0.816692},{\"x\":0.433384,\"y\":0.85},{\"x\":0.566616,\"y\":0.85},{\"x\":0.633233,\"y\":0.816692},{\"x\":0.699849,\"y\":0.750076},{\"x\":0.733686,\"y\":0.682931},{\"x\":0.766465,\"y\":0.583535},{\"x\":0.766465,\"y\":0.416465},{\"x\":0.733686,\"y\":0.317069},{\"x\":0.699849,\"y\":0.250453},{\"x\":0.633233,\"y\":0.183837},{\"x\":0.566616,\"y\":0.15},{\"x\":0.433384,\"y\":0.15}]]", 1, true, "Hershey font redistribution license", "https://github.com/techninja/hersheytextjs/blob/262f4782cd412ee539eb43d4d9d9b92562d7590b/svg_fonts/HersheySans1.svg", 1, "262f4782cd412ee539eb43d4d9d9b92562d7590b+de-latin-v1", "https://github.com/techninja/hersheytextjs/blob/262f4782cd412ee539eb43d4d9d9b92562d7590b/svg_fonts/HersheySans1.svg", "O", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076578930L },
                    { 1563703640076579131L, "The Hershey Fonts were originally created by Dr. A. V. Hershey while working at the U. S. National Bureau of Standards. The format of the Font data in this distribution was originally created by James Hurt, Cognition, Inc., 900 Technology Park Drive, Billerica, MA 01821 (mit-eddie!ci-dandelion!hurt). SVG conversion by Windell H. Oskay. RealityLoop sampled/normalized paths and adapted the capital eszett reference.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "[[{\"x\":0.266314,\"y\":0.15},{\"x\":0.266314,\"y\":0.85}],[{\"x\":0.266314,\"y\":0.15},{\"x\":0.566616,\"y\":0.15},{\"x\":0.666012,\"y\":0.183837},{\"x\":0.699849,\"y\":0.217674},{\"x\":0.733686,\"y\":0.283233},{\"x\":0.733686,\"y\":0.383686},{\"x\":0.699849,\"y\":0.450302},{\"x\":0.666012,\"y\":0.484139},{\"x\":0.566616,\"y\":0.516918},{\"x\":0.266314,\"y\":0.516918}]]", 1, true, "Hershey font redistribution license", "https://github.com/techninja/hersheytextjs/blob/262f4782cd412ee539eb43d4d9d9b92562d7590b/svg_fonts/HersheySans1.svg", 1, "262f4782cd412ee539eb43d4d9d9b92562d7590b+de-latin-v1", "https://github.com/techninja/hersheytextjs/blob/262f4782cd412ee539eb43d4d9d9b92562d7590b/svg_fonts/HersheySans1.svg", "P", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076578931L },
                    { 1563703640076579132L, "The Hershey Fonts were originally created by Dr. A. V. Hershey while working at the U. S. National Bureau of Standards. The format of the Font data in this distribution was originally created by James Hurt, Cognition, Inc., 900 Technology Park Drive, Billerica, MA 01821 (mit-eddie!ci-dandelion!hurt). SVG conversion by Windell H. Oskay. RealityLoop sampled/normalized paths and adapted the capital eszett reference.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "[[{\"x\":0.439172,\"y\":0.15},{\"x\":0.378345,\"y\":0.180896},{\"x\":0.317517,\"y\":0.241724},{\"x\":0.286621,\"y\":0.302552},{\"x\":0.25669,\"y\":0.39331},{\"x\":0.25669,\"y\":0.545862},{\"x\":0.286621,\"y\":0.636621},{\"x\":0.317517,\"y\":0.697931},{\"x\":0.378345,\"y\":0.758759},{\"x\":0.439172,\"y\":0.789172},{\"x\":0.560828,\"y\":0.789172},{\"x\":0.621655,\"y\":0.758759},{\"x\":0.682483,\"y\":0.697931},{\"x\":0.713379,\"y\":0.636621},{\"x\":0.74331,\"y\":0.545862},{\"x\":0.74331,\"y\":0.39331},{\"x\":0.713379,\"y\":0.302552},{\"x\":0.682483,\"y\":0.241724},{\"x\":0.621655,\"y\":0.180896},{\"x\":0.560828,\"y\":0.15},{\"x\":0.439172,\"y\":0.15}],[{\"x\":0.529931,\"y\":0.667517},{\"x\":0.713379,\"y\":0.85}]]", 1, true, "Hershey font redistribution license", "https://github.com/techninja/hersheytextjs/blob/262f4782cd412ee539eb43d4d9d9b92562d7590b/svg_fonts/HersheySans1.svg", 1, "262f4782cd412ee539eb43d4d9d9b92562d7590b+de-latin-v1", "https://github.com/techninja/hersheytextjs/blob/262f4782cd412ee539eb43d4d9d9b92562d7590b/svg_fonts/HersheySans1.svg", "Q", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076578932L },
                    { 1563703640076579133L, "The Hershey Fonts were originally created by Dr. A. V. Hershey while working at the U. S. National Bureau of Standards. The format of the Font data in this distribution was originally created by James Hurt, Cognition, Inc., 900 Technology Park Drive, Billerica, MA 01821 (mit-eddie!ci-dandelion!hurt). SVG conversion by Windell H. Oskay. RealityLoop sampled/normalized paths and adapted the capital eszett reference.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "[[{\"x\":0.266314,\"y\":0.15},{\"x\":0.266314,\"y\":0.85}],[{\"x\":0.266314,\"y\":0.15},{\"x\":0.566616,\"y\":0.15},{\"x\":0.666012,\"y\":0.183837},{\"x\":0.699849,\"y\":0.217674},{\"x\":0.733686,\"y\":0.283233},{\"x\":0.733686,\"y\":0.350906},{\"x\":0.699849,\"y\":0.416465},{\"x\":0.666012,\"y\":0.450302},{\"x\":0.566616,\"y\":0.484139},{\"x\":0.266314,\"y\":0.484139}],[{\"x\":0.5,\"y\":0.484139},{\"x\":0.733686,\"y\":0.85}]]", 1, true, "Hershey font redistribution license", "https://github.com/techninja/hersheytextjs/blob/262f4782cd412ee539eb43d4d9d9b92562d7590b/svg_fonts/HersheySans1.svg", 1, "262f4782cd412ee539eb43d4d9d9b92562d7590b+de-latin-v1", "https://github.com/techninja/hersheytextjs/blob/262f4782cd412ee539eb43d4d9d9b92562d7590b/svg_fonts/HersheySans1.svg", "R", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076578933L },
                    { 1563703640076579134L, "The Hershey Fonts were originally created by Dr. A. V. Hershey while working at the U. S. National Bureau of Standards. The format of the Font data in this distribution was originally created by James Hurt, Cognition, Inc., 900 Technology Park Drive, Billerica, MA 01821 (mit-eddie!ci-dandelion!hurt). SVG conversion by Windell H. Oskay. RealityLoop sampled/normalized paths and adapted the capital eszett reference.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "[[{\"x\":0.733157,\"y\":0.250453},{\"x\":0.666541,\"y\":0.183837},{\"x\":0.566088,\"y\":0.15},{\"x\":0.432855,\"y\":0.15},{\"x\":0.333459,\"y\":0.183837},{\"x\":0.266843,\"y\":0.250453},{\"x\":0.266843,\"y\":0.317069},{\"x\":0.299622,\"y\":0.383686},{\"x\":0.333459,\"y\":0.416465},{\"x\":0.400075,\"y\":0.450302},{\"x\":0.599924,\"y\":0.516918},{\"x\":0.666541,\"y\":0.549698},{\"x\":0.69932,\"y\":0.583535},{\"x\":0.733157,\"y\":0.650151},{\"x\":0.733157,\"y\":0.750076},{\"x\":0.666541,\"y\":0.816692},{\"x\":0.566088,\"y\":0.85},{\"x\":0.432855,\"y\":0.85},{\"x\":0.333459,\"y\":0.816692},{\"x\":0.266843,\"y\":0.750076}]]", 1, true, "Hershey font redistribution license", "https://github.com/techninja/hersheytextjs/blob/262f4782cd412ee539eb43d4d9d9b92562d7590b/svg_fonts/HersheySans1.svg", 1, "262f4782cd412ee539eb43d4d9d9b92562d7590b+de-latin-v1", "https://github.com/techninja/hersheytextjs/blob/262f4782cd412ee539eb43d4d9d9b92562d7590b/svg_fonts/HersheySans1.svg", "S", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076578934L },
                    { 1563703640076579135L, "The Hershey Fonts were originally created by Dr. A. V. Hershey while working at the U. S. National Bureau of Standards. The format of the Font data in this distribution was originally created by James Hurt, Cognition, Inc., 900 Technology Park Drive, Billerica, MA 01821 (mit-eddie!ci-dandelion!hurt). SVG conversion by Windell H. Oskay. RealityLoop sampled/normalized paths and adapted the capital eszett reference.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "[[{\"x\":0.499471,\"y\":0.15},{\"x\":0.499471,\"y\":0.85}],[{\"x\":0.266843,\"y\":0.15},{\"x\":0.733157,\"y\":0.15}]]", 1, true, "Hershey font redistribution license", "https://github.com/techninja/hersheytextjs/blob/262f4782cd412ee539eb43d4d9d9b92562d7590b/svg_fonts/HersheySans1.svg", 1, "262f4782cd412ee539eb43d4d9d9b92562d7590b+de-latin-v1", "https://github.com/techninja/hersheytextjs/blob/262f4782cd412ee539eb43d4d9d9b92562d7590b/svg_fonts/HersheySans1.svg", "T", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076578935L },
                    { 1563703640076579136L, "The Hershey Fonts were originally created by Dr. A. V. Hershey while working at the U. S. National Bureau of Standards. The format of the Font data in this distribution was originally created by James Hurt, Cognition, Inc., 900 Technology Park Drive, Billerica, MA 01821 (mit-eddie!ci-dandelion!hurt). SVG conversion by Windell H. Oskay. RealityLoop sampled/normalized paths and adapted the capital eszett reference.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "[[{\"x\":0.266314,\"y\":0.15},{\"x\":0.266314,\"y\":0.650151},{\"x\":0.300151,\"y\":0.750076},{\"x\":0.366767,\"y\":0.816692},{\"x\":0.467221,\"y\":0.85},{\"x\":0.532779,\"y\":0.85},{\"x\":0.633233,\"y\":0.816692},{\"x\":0.699849,\"y\":0.750076},{\"x\":0.733686,\"y\":0.650151},{\"x\":0.733686,\"y\":0.15}]]", 1, true, "Hershey font redistribution license", "https://github.com/techninja/hersheytextjs/blob/262f4782cd412ee539eb43d4d9d9b92562d7590b/svg_fonts/HersheySans1.svg", 1, "262f4782cd412ee539eb43d4d9d9b92562d7590b+de-latin-v1", "https://github.com/techninja/hersheytextjs/blob/262f4782cd412ee539eb43d4d9d9b92562d7590b/svg_fonts/HersheySans1.svg", "U", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076578936L },
                    { 1563703640076579137L, "The Hershey Fonts were originally created by Dr. A. V. Hershey while working at the U. S. National Bureau of Standards. The format of the Font data in this distribution was originally created by James Hurt, Cognition, Inc., 900 Technology Park Drive, Billerica, MA 01821 (mit-eddie!ci-dandelion!hurt). SVG conversion by Windell H. Oskay. RealityLoop sampled/normalized paths and adapted the capital eszett reference.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "[[{\"x\":0.233535,\"y\":0.15},{\"x\":0.5,\"y\":0.85}],[{\"x\":0.766465,\"y\":0.15},{\"x\":0.5,\"y\":0.85}]]", 1, true, "Hershey font redistribution license", "https://github.com/techninja/hersheytextjs/blob/262f4782cd412ee539eb43d4d9d9b92562d7590b/svg_fonts/HersheySans1.svg", 1, "262f4782cd412ee539eb43d4d9d9b92562d7590b+de-latin-v1", "https://github.com/techninja/hersheytextjs/blob/262f4782cd412ee539eb43d4d9d9b92562d7590b/svg_fonts/HersheySans1.svg", "V", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076578937L },
                    { 1563703640076579138L, "The Hershey Fonts were originally created by Dr. A. V. Hershey while working at the U. S. National Bureau of Standards. The format of the Font data in this distribution was originally created by James Hurt, Cognition, Inc., 900 Technology Park Drive, Billerica, MA 01821 (mit-eddie!ci-dandelion!hurt). SVG conversion by Windell H. Oskay. RealityLoop sampled/normalized paths and adapted the capital eszett reference.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "[[{\"x\":0.166918,\"y\":0.15},{\"x\":0.33293,\"y\":0.85}],[{\"x\":0.498943,\"y\":0.15},{\"x\":0.33293,\"y\":0.85}],[{\"x\":0.498943,\"y\":0.15},{\"x\":0.666012,\"y\":0.85}],[{\"x\":0.833082,\"y\":0.15},{\"x\":0.666012,\"y\":0.85}]]", 1, true, "Hershey font redistribution license", "https://github.com/techninja/hersheytextjs/blob/262f4782cd412ee539eb43d4d9d9b92562d7590b/svg_fonts/HersheySans1.svg", 1, "262f4782cd412ee539eb43d4d9d9b92562d7590b+de-latin-v1", "https://github.com/techninja/hersheytextjs/blob/262f4782cd412ee539eb43d4d9d9b92562d7590b/svg_fonts/HersheySans1.svg", "W", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076578938L },
                    { 1563703640076579139L, "The Hershey Fonts were originally created by Dr. A. V. Hershey while working at the U. S. National Bureau of Standards. The format of the Font data in this distribution was originally created by James Hurt, Cognition, Inc., 900 Technology Park Drive, Billerica, MA 01821 (mit-eddie!ci-dandelion!hurt). SVG conversion by Windell H. Oskay. RealityLoop sampled/normalized paths and adapted the capital eszett reference.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "[[{\"x\":0.266843,\"y\":0.15},{\"x\":0.733157,\"y\":0.85}],[{\"x\":0.733157,\"y\":0.15},{\"x\":0.266843,\"y\":0.85}]]", 1, true, "Hershey font redistribution license", "https://github.com/techninja/hersheytextjs/blob/262f4782cd412ee539eb43d4d9d9b92562d7590b/svg_fonts/HersheySans1.svg", 1, "262f4782cd412ee539eb43d4d9d9b92562d7590b+de-latin-v1", "https://github.com/techninja/hersheytextjs/blob/262f4782cd412ee539eb43d4d9d9b92562d7590b/svg_fonts/HersheySans1.svg", "X", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076578939L },
                    { 1563703640076579140L, "The Hershey Fonts were originally created by Dr. A. V. Hershey while working at the U. S. National Bureau of Standards. The format of the Font data in this distribution was originally created by James Hurt, Cognition, Inc., 900 Technology Park Drive, Billerica, MA 01821 (mit-eddie!ci-dandelion!hurt). SVG conversion by Windell H. Oskay. RealityLoop sampled/normalized paths and adapted the capital eszett reference.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "[[{\"x\":0.233535,\"y\":0.15},{\"x\":0.5,\"y\":0.484139},{\"x\":0.5,\"y\":0.85}],[{\"x\":0.766465,\"y\":0.15},{\"x\":0.5,\"y\":0.484139}]]", 1, true, "Hershey font redistribution license", "https://github.com/techninja/hersheytextjs/blob/262f4782cd412ee539eb43d4d9d9b92562d7590b/svg_fonts/HersheySans1.svg", 1, "262f4782cd412ee539eb43d4d9d9b92562d7590b+de-latin-v1", "https://github.com/techninja/hersheytextjs/blob/262f4782cd412ee539eb43d4d9d9b92562d7590b/svg_fonts/HersheySans1.svg", "Y", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076578940L },
                    { 1563703640076579141L, "The Hershey Fonts were originally created by Dr. A. V. Hershey while working at the U. S. National Bureau of Standards. The format of the Font data in this distribution was originally created by James Hurt, Cognition, Inc., 900 Technology Park Drive, Billerica, MA 01821 (mit-eddie!ci-dandelion!hurt). SVG conversion by Windell H. Oskay. RealityLoop sampled/normalized paths and adapted the capital eszett reference.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "[[{\"x\":0.733157,\"y\":0.15},{\"x\":0.266843,\"y\":0.85}],[{\"x\":0.266843,\"y\":0.15},{\"x\":0.733157,\"y\":0.15}],[{\"x\":0.266843,\"y\":0.85},{\"x\":0.733157,\"y\":0.85}]]", 1, true, "Hershey font redistribution license", "https://github.com/techninja/hersheytextjs/blob/262f4782cd412ee539eb43d4d9d9b92562d7590b/svg_fonts/HersheySans1.svg", 1, "262f4782cd412ee539eb43d4d9d9b92562d7590b+de-latin-v1", "https://github.com/techninja/hersheytextjs/blob/262f4782cd412ee539eb43d4d9d9b92562d7590b/svg_fonts/HersheySans1.svg", "Z", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076578941L },
                    { 1563703640076579142L, "The Hershey Fonts were originally created by Dr. A. V. Hershey while working at the U. S. National Bureau of Standards. The format of the Font data in this distribution was originally created by James Hurt, Cognition, Inc., 900 Technology Park Drive, Billerica, MA 01821 (mit-eddie!ci-dandelion!hurt). SVG conversion by Windell H. Oskay. RealityLoop sampled/normalized paths and adapted the capital eszett reference.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "[[{\"x\":0.5,\"y\":0.342998},{\"x\":0.307002,\"y\":0.85}],[{\"x\":0.5,\"y\":0.342998},{\"x\":0.692998,\"y\":0.85}],[{\"x\":0.378993,\"y\":0.68151},{\"x\":0.621007,\"y\":0.68151}],[{\"x\":0.43337,\"y\":0.15},{\"x\":0.409628,\"y\":0.174508},{\"x\":0.43337,\"y\":0.199015},{\"x\":0.457877,\"y\":0.174508},{\"x\":0.43337,\"y\":0.15}],[{\"x\":0.61488,\"y\":0.15},{\"x\":0.590372,\"y\":0.174508},{\"x\":0.61488,\"y\":0.199015},{\"x\":0.638621,\"y\":0.174508},{\"x\":0.61488,\"y\":0.15}]]", 1, true, "Hershey font redistribution license", "https://github.com/techninja/hersheytextjs/blob/262f4782cd412ee539eb43d4d9d9b92562d7590b/svg_fonts/HersheySans1.svg", 1, "262f4782cd412ee539eb43d4d9d9b92562d7590b+de-latin-v1", "https://github.com/techninja/hersheytextjs/blob/262f4782cd412ee539eb43d4d9d9b92562d7590b/svg_fonts/HersheySans1.svg", "Ä", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076578942L },
                    { 1563703640076579143L, "The Hershey Fonts were originally created by Dr. A. V. Hershey while working at the U. S. National Bureau of Standards. The format of the Font data in this distribution was originally created by James Hurt, Cognition, Inc., 900 Technology Park Drive, Billerica, MA 01821 (mit-eddie!ci-dandelion!hurt). SVG conversion by Windell H. Oskay. RealityLoop sampled/normalized paths and adapted the capital eszett reference.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "[[{\"x\":0.451751,\"y\":0.342998},{\"x\":0.403501,\"y\":0.367505},{\"x\":0.355252,\"y\":0.415755},{\"x\":0.330744,\"y\":0.464004},{\"x\":0.307002,\"y\":0.535996},{\"x\":0.307002,\"y\":0.657002},{\"x\":0.330744,\"y\":0.728993},{\"x\":0.355252,\"y\":0.777626},{\"x\":0.403501,\"y\":0.825875},{\"x\":0.451751,\"y\":0.85},{\"x\":0.548249,\"y\":0.85},{\"x\":0.596499,\"y\":0.825875},{\"x\":0.644748,\"y\":0.777626},{\"x\":0.669256,\"y\":0.728993},{\"x\":0.692998,\"y\":0.657002},{\"x\":0.692998,\"y\":0.535996},{\"x\":0.669256,\"y\":0.464004},{\"x\":0.644748,\"y\":0.415755},{\"x\":0.596499,\"y\":0.367505},{\"x\":0.548249,\"y\":0.342998},{\"x\":0.451751,\"y\":0.342998}],[{\"x\":0.404267,\"y\":0.15},{\"x\":0.380525,\"y\":0.174508},{\"x\":0.404267,\"y\":0.199015},{\"x\":0.428775,\"y\":0.174508},{\"x\":0.404267,\"y\":0.15}],[{\"x\":0.595733,\"y\":0.15},{\"x\":0.571225,\"y\":0.174508},{\"x\":0.595733,\"y\":0.199015},{\"x\":0.619475,\"y\":0.174508},{\"x\":0.595733,\"y\":0.15}]]", 1, true, "Hershey font redistribution license", "https://github.com/techninja/hersheytextjs/blob/262f4782cd412ee539eb43d4d9d9b92562d7590b/svg_fonts/HersheySans1.svg", 1, "262f4782cd412ee539eb43d4d9d9b92562d7590b+de-latin-v1", "https://github.com/techninja/hersheytextjs/blob/262f4782cd412ee539eb43d4d9d9b92562d7590b/svg_fonts/HersheySans1.svg", "Ö", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076578943L },
                    { 1563703640076579144L, "The Hershey Fonts were originally created by Dr. A. V. Hershey while working at the U. S. National Bureau of Standards. The format of the Font data in this distribution was originally created by James Hurt, Cognition, Inc., 900 Technology Park Drive, Billerica, MA 01821 (mit-eddie!ci-dandelion!hurt). SVG conversion by Windell H. Oskay. RealityLoop sampled/normalized paths and adapted the capital eszett reference.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "[[{\"x\":0.330744,\"y\":0.342998},{\"x\":0.330744,\"y\":0.705252},{\"x\":0.355252,\"y\":0.777626},{\"x\":0.403501,\"y\":0.825875},{\"x\":0.476258,\"y\":0.85},{\"x\":0.523742,\"y\":0.85},{\"x\":0.596499,\"y\":0.825875},{\"x\":0.644748,\"y\":0.777626},{\"x\":0.669256,\"y\":0.705252},{\"x\":0.669256,\"y\":0.342998}],[{\"x\":0.399672,\"y\":0.15},{\"x\":0.37593,\"y\":0.174508},{\"x\":0.399672,\"y\":0.199015},{\"x\":0.424179,\"y\":0.174508},{\"x\":0.399672,\"y\":0.15}],[{\"x\":0.600328,\"y\":0.15},{\"x\":0.575821,\"y\":0.174508},{\"x\":0.600328,\"y\":0.199015},{\"x\":0.62407,\"y\":0.174508},{\"x\":0.600328,\"y\":0.15}]]", 1, true, "Hershey font redistribution license", "https://github.com/techninja/hersheytextjs/blob/262f4782cd412ee539eb43d4d9d9b92562d7590b/svg_fonts/HersheySans1.svg", 1, "262f4782cd412ee539eb43d4d9d9b92562d7590b+de-latin-v1", "https://github.com/techninja/hersheytextjs/blob/262f4782cd412ee539eb43d4d9d9b92562d7590b/svg_fonts/HersheySans1.svg", "Ü", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076578944L },
                    { 1563703640076579145L, "The Hershey Fonts were originally created by Dr. A. V. Hershey while working at the U. S. National Bureau of Standards. The format of the Font data in this distribution was originally created by James Hurt, Cognition, Inc., 900 Technology Park Drive, Billerica, MA 01821 (mit-eddie!ci-dandelion!hurt). SVG conversion by Windell H. Oskay. RealityLoop sampled/normalized paths and adapted the capital eszett reference.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "[[{\"x\":0.241395,\"y\":0.85},{\"x\":0.241395,\"y\":0.314955},{\"x\":0.384074,\"y\":0.154441},{\"x\":0.393395,\"y\":0.152926},{\"x\":0.402947,\"y\":0.151724},{\"x\":0.412698,\"y\":0.150836},{\"x\":0.422617,\"y\":0.150261},{\"x\":0.432673,\"y\":0.15},{\"x\":0.442836,\"y\":0.150052},{\"x\":0.453073,\"y\":0.150418},{\"x\":0.463355,\"y\":0.151097},{\"x\":0.47365,\"y\":0.15209},{\"x\":0.483926,\"y\":0.153396},{\"x\":0.494154,\"y\":0.155016},{\"x\":0.504302,\"y\":0.156949},{\"x\":0.514339,\"y\":0.159196},{\"x\":0.524233,\"y\":0.161756},{\"x\":0.533955,\"y\":0.16463},{\"x\":0.543472,\"y\":0.167817},{\"x\":0.552755,\"y\":0.171318},{\"x\":0.561771,\"y\":0.175132},{\"x\":0.57049,\"y\":0.17926},{\"x\":0.578881,\"y\":0.183702},{\"x\":0.586912,\"y\":0.188456},{\"x\":0.594554,\"y\":0.193525},{\"x\":0.601774,\"y\":0.198907},{\"x\":0.608542,\"y\":0.204602},{\"x\":0.614826,\"y\":0.210611},{\"x\":0.620596,\"y\":0.216933},{\"x\":0.625821,\"y\":0.223569},{\"x\":0.630469,\"y\":0.230518},{\"x\":0.63451,\"y\":0.237781},{\"x\":0.637913,\"y\":0.245357},{\"x\":0.640646,\"y\":0.253247},{\"x\":0.642679,\"y\":0.26145},{\"x\":0.644016,\"y\":0.269707},{\"x\":0.644703,\"y\":0.277759},{\"x\":0.64477,\"y\":0.285612},{\"x\":0.644246,\"y\":0.293271},{\"x\":0.64316,\"y\":0.30074},{\"x\":0.641542,\"y\":0.308025},{\"x\":0.639421,\"y\":0.31513},{\"x\":0.636827,\"y\":0.322061},{\"x\":0.633788,\"y\":0.328822},{\"x\":0.630334,\"y\":0.335417},{\"x\":0.626496,\"y\":0.341853},{\"x\":0.622301,\"y\":0.348134},{\"x\":0.61778,\"y\":0.354264},{\"x\":0.612961,\"y\":0.36025},{\"x\":0.607875,\"y\":0.366094},{\"x\":0.60255,\"y\":0.371803},{\"x\":0.597017,\"y\":0.377382},{\"x\":0.591303,\"y\":0.382835},{\"x\":0.58544,\"y\":0.388167},{\"x\":0.579456,\"y\":0.393383},{\"x\":0.57338,\"y\":0.398488},{\"x\":0.567242,\"y\":0.403487},{\"x\":0.561072,\"y\":0.408384},{\"x\":0.554898,\"y\":0.413186},{\"x\":0.54875,\"y\":0.417896},{\"x\":0.542658,\"y\":0.422519},{\"x\":0.536651,\"y\":0.427061},{\"x\":0.530758,\"y\":0.431526},{\"x\":0.525009,\"y\":0.435919},{\"x\":0.519433,\"y\":0.440245},{\"x\":0.514059,\"y\":0.444509},{\"x\":0.508917,\"y\":0.448716},{\"x\":0.524541,\"y\":0.45057},{\"x\":0.539643,\"y\":0.452783},{\"x\":0.554226,\"y\":0.455349},{\"x\":0.568291,\"y\":0.458261},{\"x\":0.581841,\"y\":0.461512},{\"x\":0.594876,\"y\":0.465097},{\"x\":0.607398,\"y\":0.469008},{\"x\":0.61941,\"y\":0.473239},{\"x\":0.630912,\"y\":0.477784},{\"x\":0.641906,\"y\":0.482635},{\"x\":0.652394,\"y\":0.487788},{\"x\":0.662377,\"y\":0.493234},{\"x\":0.671858,\"y\":0.498967},{\"x\":0.680837,\"y\":0.504981},{\"x\":0.689316,\"y\":0.51127},{\"x\":0.697298,\"y\":0.517826},{\"x\":0.704783,\"y\":0.524644},{\"x\":0.711773,\"y\":0.531716},{\"x\":0.718271,\"y\":0.539037},{\"x\":0.724276,\"y\":0.546599},{\"x\":0.729792,\"y\":0.554396},{\"x\":0.73482,\"y\":0.562422},{\"x\":0.739361,\"y\":0.57067},{\"x\":0.743418,\"y\":0.579133},{\"x\":0.746991,\"y\":0.587806},{\"x\":0.750082,\"y\":0.596681},{\"x\":0.752693,\"y\":0.605752},{\"x\":0.754826,\"y\":0.615012},{\"x\":0.756482,\"y\":0.624455},{\"x\":0.757662,\"y\":0.634075},{\"x\":0.75837,\"y\":0.643864},{\"x\":0.758605,\"y\":0.653817},{\"x\":0.758319,\"y\":0.665364},{\"x\":0.757469,\"y\":0.676594},{\"x\":0.756063,\"y\":0.687504},{\"x\":0.754112,\"y\":0.69809},{\"x\":0.751625,\"y\":0.708351},{\"x\":0.748612,\"y\":0.718281},{\"x\":0.745084,\"y\":0.727878},{\"x\":0.741049,\"y\":0.737139},{\"x\":0.736518,\"y\":0.74606},{\"x\":0.7315,\"y\":0.754638},{\"x\":0.726006,\"y\":0.762871},{\"x\":0.720044,\"y\":0.770753},{\"x\":0.713626,\"y\":0.778284},{\"x\":0.70676,\"y\":0.785458},{\"x\":0.699456,\"y\":0.792272},{\"x\":0.691724,\"y\":0.798725},{\"x\":0.683575,\"y\":0.804811},{\"x\":0.675017,\"y\":0.810529},{\"x\":0.666061,\"y\":0.815874},{\"x\":0.656717,\"y\":0.820844},{\"x\":0.646993,\"y\":0.825435},{\"x\":0.636901,\"y\":0.829644},{\"x\":0.626449,\"y\":0.833468},{\"x\":0.615648,\"y\":0.836903},{\"x\":0.604507,\"y\":0.839946},{\"x\":0.593036,\"y\":0.842593},{\"x\":0.581246,\"y\":0.844843},{\"x\":0.569145,\"y\":0.846691},{\"x\":0.556743,\"y\":0.848134},{\"x\":0.544052,\"y\":0.849168},{\"x\":0.531079,\"y\":0.849792},{\"x\":0.517835,\"y\":0.85},{\"x\":0.401908,\"y\":0.85}]]", 1, true, "Hershey font redistribution license", "https://github.com/techninja/hersheytextjs/blob/262f4782cd412ee539eb43d4d9d9b92562d7590b/svg_fonts/HersheySans1.svg", 1, "262f4782cd412ee539eb43d4d9d9b92562d7590b+de-latin-v1", "https://github.com/techninja/hersheytextjs/blob/262f4782cd412ee539eb43d4d9d9b92562d7590b/svg_fonts/HersheySans1.svg", "ẞ", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076578945L },
                    { 1563703640076579146L, "The Hershey Fonts were originally created by Dr. A. V. Hershey while working at the U. S. National Bureau of Standards. The format of the Font data in this distribution was originally created by James Hurt, Cognition, Inc., 900 Technology Park Drive, Billerica, MA 01821 (mit-eddie!ci-dandelion!hurt). SVG conversion by Windell H. Oskay. RealityLoop sampled/normalized paths and adapted the capital eszett reference.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "[[{\"x\":0.8,\"y\":0.15},{\"x\":0.8,\"y\":0.85}],[{\"x\":0.8,\"y\":0.300794},{\"x\":0.7,\"y\":0.199206},{\"x\":0.6,\"y\":0.15},{\"x\":0.449206,\"y\":0.15},{\"x\":0.350794,\"y\":0.199206},{\"x\":0.249206,\"y\":0.300794},{\"x\":0.2,\"y\":0.45},{\"x\":0.2,\"y\":0.55},{\"x\":0.249206,\"y\":0.7},{\"x\":0.350794,\"y\":0.8},{\"x\":0.449206,\"y\":0.85},{\"x\":0.6,\"y\":0.85},{\"x\":0.7,\"y\":0.8},{\"x\":0.8,\"y\":0.7}]]", 1, true, "Hershey font redistribution license", "https://github.com/techninja/hersheytextjs/blob/262f4782cd412ee539eb43d4d9d9b92562d7590b/svg_fonts/HersheySans1.svg", 1, "262f4782cd412ee539eb43d4d9d9b92562d7590b+de-latin-v1", "https://github.com/techninja/hersheytextjs/blob/262f4782cd412ee539eb43d4d9d9b92562d7590b/svg_fonts/HersheySans1.svg", "a", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076578946L },
                    { 1563703640076579147L, "The Hershey Fonts were originally created by Dr. A. V. Hershey while working at the U. S. National Bureau of Standards. The format of the Font data in this distribution was originally created by James Hurt, Cognition, Inc., 900 Technology Park Drive, Billerica, MA 01821 (mit-eddie!ci-dandelion!hurt). SVG conversion by Windell H. Oskay. RealityLoop sampled/normalized paths and adapted the capital eszett reference.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "[[{\"x\":0.300151,\"y\":0.15},{\"x\":0.300151,\"y\":0.85}],[{\"x\":0.300151,\"y\":0.484139},{\"x\":0.367825,\"y\":0.416465},{\"x\":0.433384,\"y\":0.383686},{\"x\":0.533837,\"y\":0.383686},{\"x\":0.600453,\"y\":0.416465},{\"x\":0.667069,\"y\":0.484139},{\"x\":0.699849,\"y\":0.583535},{\"x\":0.699849,\"y\":0.650151},{\"x\":0.667069,\"y\":0.750076},{\"x\":0.600453,\"y\":0.816692},{\"x\":0.533837,\"y\":0.85},{\"x\":0.433384,\"y\":0.85},{\"x\":0.367825,\"y\":0.816692},{\"x\":0.300151,\"y\":0.750076}]]", 1, true, "Hershey font redistribution license", "https://github.com/techninja/hersheytextjs/blob/262f4782cd412ee539eb43d4d9d9b92562d7590b/svg_fonts/HersheySans1.svg", 1, "262f4782cd412ee539eb43d4d9d9b92562d7590b+de-latin-v1", "https://github.com/techninja/hersheytextjs/blob/262f4782cd412ee539eb43d4d9d9b92562d7590b/svg_fonts/HersheySans1.svg", "b", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076578947L },
                    { 1563703640076579148L, "The Hershey Fonts were originally created by Dr. A. V. Hershey while working at the U. S. National Bureau of Standards. The format of the Font data in this distribution was originally created by James Hurt, Cognition, Inc., 900 Technology Park Drive, Billerica, MA 01821 (mit-eddie!ci-dandelion!hurt). SVG conversion by Windell H. Oskay. RealityLoop sampled/normalized paths and adapted the capital eszett reference.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "[[{\"x\":0.8,\"y\":0.300794},{\"x\":0.7,\"y\":0.199206},{\"x\":0.6,\"y\":0.15},{\"x\":0.449206,\"y\":0.15},{\"x\":0.350794,\"y\":0.199206},{\"x\":0.249206,\"y\":0.300794},{\"x\":0.2,\"y\":0.45},{\"x\":0.2,\"y\":0.55},{\"x\":0.249206,\"y\":0.7},{\"x\":0.350794,\"y\":0.8},{\"x\":0.449206,\"y\":0.85},{\"x\":0.6,\"y\":0.85},{\"x\":0.7,\"y\":0.8},{\"x\":0.8,\"y\":0.7}]]", 1, true, "Hershey font redistribution license", "https://github.com/techninja/hersheytextjs/blob/262f4782cd412ee539eb43d4d9d9b92562d7590b/svg_fonts/HersheySans1.svg", 1, "262f4782cd412ee539eb43d4d9d9b92562d7590b+de-latin-v1", "https://github.com/techninja/hersheytextjs/blob/262f4782cd412ee539eb43d4d9d9b92562d7590b/svg_fonts/HersheySans1.svg", "c", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076578948L },
                    { 1563703640076579149L, "The Hershey Fonts were originally created by Dr. A. V. Hershey while working at the U. S. National Bureau of Standards. The format of the Font data in this distribution was originally created by James Hurt, Cognition, Inc., 900 Technology Park Drive, Billerica, MA 01821 (mit-eddie!ci-dandelion!hurt). SVG conversion by Windell H. Oskay. RealityLoop sampled/normalized paths and adapted the capital eszett reference.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "[[{\"x\":0.699849,\"y\":0.15},{\"x\":0.699849,\"y\":0.85}],[{\"x\":0.699849,\"y\":0.484139},{\"x\":0.633233,\"y\":0.416465},{\"x\":0.566616,\"y\":0.383686},{\"x\":0.466163,\"y\":0.383686},{\"x\":0.400604,\"y\":0.416465},{\"x\":0.332931,\"y\":0.484139},{\"x\":0.300151,\"y\":0.583535},{\"x\":0.300151,\"y\":0.650151},{\"x\":0.332931,\"y\":0.750076},{\"x\":0.400604,\"y\":0.816692},{\"x\":0.466163,\"y\":0.85},{\"x\":0.566616,\"y\":0.85},{\"x\":0.633233,\"y\":0.816692},{\"x\":0.699849,\"y\":0.750076}]]", 1, true, "Hershey font redistribution license", "https://github.com/techninja/hersheytextjs/blob/262f4782cd412ee539eb43d4d9d9b92562d7590b/svg_fonts/HersheySans1.svg", 1, "262f4782cd412ee539eb43d4d9d9b92562d7590b+de-latin-v1", "https://github.com/techninja/hersheytextjs/blob/262f4782cd412ee539eb43d4d9d9b92562d7590b/svg_fonts/HersheySans1.svg", "d", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076578949L },
                    { 1563703640076579150L, "The Hershey Fonts were originally created by Dr. A. V. Hershey while working at the U. S. National Bureau of Standards. The format of the Font data in this distribution was originally created by James Hurt, Cognition, Inc., 900 Technology Park Drive, Billerica, MA 01821 (mit-eddie!ci-dandelion!hurt). SVG conversion by Windell H. Oskay. RealityLoop sampled/normalized paths and adapted the capital eszett reference.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "[[{\"x\":0.2,\"y\":0.45},{\"x\":0.8,\"y\":0.45},{\"x\":0.8,\"y\":0.35},{\"x\":0.750794,\"y\":0.25},{\"x\":0.7,\"y\":0.199206},{\"x\":0.6,\"y\":0.15},{\"x\":0.449206,\"y\":0.15},{\"x\":0.350794,\"y\":0.199206},{\"x\":0.249206,\"y\":0.300794},{\"x\":0.2,\"y\":0.45},{\"x\":0.2,\"y\":0.55},{\"x\":0.249206,\"y\":0.7},{\"x\":0.350794,\"y\":0.8},{\"x\":0.449206,\"y\":0.85},{\"x\":0.6,\"y\":0.85},{\"x\":0.7,\"y\":0.8},{\"x\":0.8,\"y\":0.7}]]", 1, true, "Hershey font redistribution license", "https://github.com/techninja/hersheytextjs/blob/262f4782cd412ee539eb43d4d9d9b92562d7590b/svg_fonts/HersheySans1.svg", 1, "262f4782cd412ee539eb43d4d9d9b92562d7590b+de-latin-v1", "https://github.com/techninja/hersheytextjs/blob/262f4782cd412ee539eb43d4d9d9b92562d7590b/svg_fonts/HersheySans1.svg", "e", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076578950L },
                    { 1563703640076579151L, "The Hershey Fonts were originally created by Dr. A. V. Hershey while working at the U. S. National Bureau of Standards. The format of the Font data in this distribution was originally created by James Hurt, Cognition, Inc., 900 Technology Park Drive, Billerica, MA 01821 (mit-eddie!ci-dandelion!hurt). SVG conversion by Windell H. Oskay. RealityLoop sampled/normalized paths and adapted the capital eszett reference.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "[[{\"x\":0.633233,\"y\":0.15},{\"x\":0.565559,\"y\":0.15},{\"x\":0.5,\"y\":0.183837},{\"x\":0.466163,\"y\":0.283233},{\"x\":0.466163,\"y\":0.85}],[{\"x\":0.366767,\"y\":0.383686},{\"x\":0.599396,\"y\":0.383686}]]", 1, true, "Hershey font redistribution license", "https://github.com/techninja/hersheytextjs/blob/262f4782cd412ee539eb43d4d9d9b92562d7590b/svg_fonts/HersheySans1.svg", 1, "262f4782cd412ee539eb43d4d9d9b92562d7590b+de-latin-v1", "https://github.com/techninja/hersheytextjs/blob/262f4782cd412ee539eb43d4d9d9b92562d7590b/svg_fonts/HersheySans1.svg", "f", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076578951L },
                    { 1563703640076579152L, "The Hershey Fonts were originally created by Dr. A. V. Hershey while working at the U. S. National Bureau of Standards. The format of the Font data in this distribution was originally created by James Hurt, Cognition, Inc., 900 Technology Park Drive, Billerica, MA 01821 (mit-eddie!ci-dandelion!hurt). SVG conversion by Windell H. Oskay. RealityLoop sampled/normalized paths and adapted the capital eszett reference.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "[[{\"x\":0.700151,\"y\":0.15},{\"x\":0.700151,\"y\":0.683737},{\"x\":0.667322,\"y\":0.784342},{\"x\":0.633434,\"y\":0.817171},{\"x\":0.566717,\"y\":0.85},{\"x\":0.466112,\"y\":0.85},{\"x\":0.400454,\"y\":0.817171}],[{\"x\":0.700151,\"y\":0.250605},{\"x\":0.633434,\"y\":0.182829},{\"x\":0.566717,\"y\":0.15},{\"x\":0.466112,\"y\":0.15},{\"x\":0.400454,\"y\":0.182829},{\"x\":0.332678,\"y\":0.250605},{\"x\":0.299849,\"y\":0.350151},{\"x\":0.299849,\"y\":0.416868},{\"x\":0.332678,\"y\":0.516944},{\"x\":0.400454,\"y\":0.583661},{\"x\":0.466112,\"y\":0.61702},{\"x\":0.566717,\"y\":0.61702},{\"x\":0.633434,\"y\":0.583661},{\"x\":0.700151,\"y\":0.516944}]]", 1, true, "Hershey font redistribution license", "https://github.com/techninja/hersheytextjs/blob/262f4782cd412ee539eb43d4d9d9b92562d7590b/svg_fonts/HersheySans1.svg", 1, "262f4782cd412ee539eb43d4d9d9b92562d7590b+de-latin-v1", "https://github.com/techninja/hersheytextjs/blob/262f4782cd412ee539eb43d4d9d9b92562d7590b/svg_fonts/HersheySans1.svg", "g", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076578952L },
                    { 1563703640076579153L, "The Hershey Fonts were originally created by Dr. A. V. Hershey while working at the U. S. National Bureau of Standards. The format of the Font data in this distribution was originally created by James Hurt, Cognition, Inc., 900 Technology Park Drive, Billerica, MA 01821 (mit-eddie!ci-dandelion!hurt). SVG conversion by Windell H. Oskay. RealityLoop sampled/normalized paths and adapted the capital eszett reference.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "[[{\"x\":0.316541,\"y\":0.15},{\"x\":0.316541,\"y\":0.85}],[{\"x\":0.316541,\"y\":0.516918},{\"x\":0.416994,\"y\":0.416465},{\"x\":0.48361,\"y\":0.383686},{\"x\":0.583006,\"y\":0.383686},{\"x\":0.65068,\"y\":0.416465},{\"x\":0.683459,\"y\":0.516918},{\"x\":0.683459,\"y\":0.85}]]", 1, true, "Hershey font redistribution license", "https://github.com/techninja/hersheytextjs/blob/262f4782cd412ee539eb43d4d9d9b92562d7590b/svg_fonts/HersheySans1.svg", 1, "262f4782cd412ee539eb43d4d9d9b92562d7590b+de-latin-v1", "https://github.com/techninja/hersheytextjs/blob/262f4782cd412ee539eb43d4d9d9b92562d7590b/svg_fonts/HersheySans1.svg", "h", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076578953L },
                    { 1563703640076579154L, "The Hershey Fonts were originally created by Dr. A. V. Hershey while working at the U. S. National Bureau of Standards. The format of the Font data in this distribution was originally created by James Hurt, Cognition, Inc., 900 Technology Park Drive, Billerica, MA 01821 (mit-eddie!ci-dandelion!hurt). SVG conversion by Windell H. Oskay. RealityLoop sampled/normalized paths and adapted the capital eszett reference.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "[[{\"x\":0.468182,\"y\":0.181313},{\"x\":0.499495,\"y\":0.213636},{\"x\":0.531818,\"y\":0.181313},{\"x\":0.499495,\"y\":0.15},{\"x\":0.468182,\"y\":0.181313}],[{\"x\":0.499495,\"y\":0.404545},{\"x\":0.499495,\"y\":0.85}]]", 1, true, "Hershey font redistribution license", "https://github.com/techninja/hersheytextjs/blob/262f4782cd412ee539eb43d4d9d9b92562d7590b/svg_fonts/HersheySans1.svg", 1, "262f4782cd412ee539eb43d4d9d9b92562d7590b+de-latin-v1", "https://github.com/techninja/hersheytextjs/blob/262f4782cd412ee539eb43d4d9d9b92562d7590b/svg_fonts/HersheySans1.svg", "i", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076578954L },
                    { 1563703640076579155L, "The Hershey Fonts were originally created by Dr. A. V. Hershey while working at the U. S. National Bureau of Standards. The format of the Font data in this distribution was originally created by James Hurt, Cognition, Inc., 900 Technology Park Drive, Billerica, MA 01821 (mit-eddie!ci-dandelion!hurt). SVG conversion by Windell H. Oskay. RealityLoop sampled/normalized paths and adapted the capital eszett reference.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "[[{\"x\":0.524151,\"y\":0.173768},{\"x\":0.548686,\"y\":0.198302},{\"x\":0.572453,\"y\":0.173768},{\"x\":0.548686,\"y\":0.15},{\"x\":0.524151,\"y\":0.173768}],[{\"x\":0.548686,\"y\":0.343209},{\"x\":0.548686,\"y\":0.753779},{\"x\":0.524151,\"y\":0.826232},{\"x\":0.475849,\"y\":0.85},{\"x\":0.427547,\"y\":0.85}]]", 1, true, "Hershey font redistribution license", "https://github.com/techninja/hersheytextjs/blob/262f4782cd412ee539eb43d4d9d9b92562d7590b/svg_fonts/HersheySans1.svg", 1, "262f4782cd412ee539eb43d4d9d9b92562d7590b+de-latin-v1", "https://github.com/techninja/hersheytextjs/blob/262f4782cd412ee539eb43d4d9d9b92562d7590b/svg_fonts/HersheySans1.svg", "j", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076578955L },
                    { 1563703640076579156L, "The Hershey Fonts were originally created by Dr. A. V. Hershey while working at the U. S. National Bureau of Standards. The format of the Font data in this distribution was originally created by James Hurt, Cognition, Inc., 900 Technology Park Drive, Billerica, MA 01821 (mit-eddie!ci-dandelion!hurt). SVG conversion by Windell H. Oskay. RealityLoop sampled/normalized paths and adapted the capital eszett reference.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "[[{\"x\":0.316541,\"y\":0.15},{\"x\":0.316541,\"y\":0.85}],[{\"x\":0.65068,\"y\":0.383686},{\"x\":0.316541,\"y\":0.716767}],[{\"x\":0.449773,\"y\":0.583535},{\"x\":0.683459,\"y\":0.85}]]", 1, true, "Hershey font redistribution license", "https://github.com/techninja/hersheytextjs/blob/262f4782cd412ee539eb43d4d9d9b92562d7590b/svg_fonts/HersheySans1.svg", 1, "262f4782cd412ee539eb43d4d9d9b92562d7590b+de-latin-v1", "https://github.com/techninja/hersheytextjs/blob/262f4782cd412ee539eb43d4d9d9b92562d7590b/svg_fonts/HersheySans1.svg", "k", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076578956L },
                    { 1563703640076579157L, "The Hershey Fonts were originally created by Dr. A. V. Hershey while working at the U. S. National Bureau of Standards. The format of the Font data in this distribution was originally created by James Hurt, Cognition, Inc., 900 Technology Park Drive, Billerica, MA 01821 (mit-eddie!ci-dandelion!hurt). SVG conversion by Windell H. Oskay. RealityLoop sampled/normalized paths and adapted the capital eszett reference.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "[[{\"x\":0.5,\"y\":0.15},{\"x\":0.5,\"y\":0.85}]]", 1, true, "Hershey font redistribution license", "https://github.com/techninja/hersheytextjs/blob/262f4782cd412ee539eb43d4d9d9b92562d7590b/svg_fonts/HersheySans1.svg", 1, "262f4782cd412ee539eb43d4d9d9b92562d7590b+de-latin-v1", "https://github.com/techninja/hersheytextjs/blob/262f4782cd412ee539eb43d4d9d9b92562d7590b/svg_fonts/HersheySans1.svg", "l", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076578957L },
                    { 1563703640076579158L, "The Hershey Fonts were originally created by Dr. A. V. Hershey while working at the U. S. National Bureau of Standards. The format of the Font data in this distribution was originally created by James Hurt, Cognition, Inc., 900 Technology Park Drive, Billerica, MA 01821 (mit-eddie!ci-dandelion!hurt). SVG conversion by Windell H. Oskay. RealityLoop sampled/normalized paths and adapted the capital eszett reference.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "[[{\"x\":0.15,\"y\":0.277594},{\"x\":0.15,\"y\":0.722406}],[{\"x\":0.15,\"y\":0.404683},{\"x\":0.245821,\"y\":0.308862},{\"x\":0.309366,\"y\":0.277594},{\"x\":0.404179,\"y\":0.277594},{\"x\":0.468732,\"y\":0.308862},{\"x\":0.5,\"y\":0.404683},{\"x\":0.5,\"y\":0.722406}],[{\"x\":0.5,\"y\":0.404683},{\"x\":0.595821,\"y\":0.308862},{\"x\":0.658357,\"y\":0.277594},{\"x\":0.754179,\"y\":0.277594},{\"x\":0.817723,\"y\":0.308862},{\"x\":0.85,\"y\":0.404683},{\"x\":0.85,\"y\":0.722406}]]", 1, true, "Hershey font redistribution license", "https://github.com/techninja/hersheytextjs/blob/262f4782cd412ee539eb43d4d9d9b92562d7590b/svg_fonts/HersheySans1.svg", 1, "262f4782cd412ee539eb43d4d9d9b92562d7590b+de-latin-v1", "https://github.com/techninja/hersheytextjs/blob/262f4782cd412ee539eb43d4d9d9b92562d7590b/svg_fonts/HersheySans1.svg", "m", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076578958L },
                    { 1563703640076579159L, "The Hershey Fonts were originally created by Dr. A. V. Hershey while working at the U. S. National Bureau of Standards. The format of the Font data in this distribution was originally created by James Hurt, Cognition, Inc., 900 Technology Park Drive, Billerica, MA 01821 (mit-eddie!ci-dandelion!hurt). SVG conversion by Windell H. Oskay. RealityLoop sampled/normalized paths and adapted the capital eszett reference.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "[[{\"x\":0.224603,\"y\":0.15},{\"x\":0.224603,\"y\":0.85}],[{\"x\":0.224603,\"y\":0.35},{\"x\":0.375397,\"y\":0.199206},{\"x\":0.475397,\"y\":0.15},{\"x\":0.624603,\"y\":0.15},{\"x\":0.72619,\"y\":0.199206},{\"x\":0.775397,\"y\":0.35},{\"x\":0.775397,\"y\":0.85}]]", 1, true, "Hershey font redistribution license", "https://github.com/techninja/hersheytextjs/blob/262f4782cd412ee539eb43d4d9d9b92562d7590b/svg_fonts/HersheySans1.svg", 1, "262f4782cd412ee539eb43d4d9d9b92562d7590b+de-latin-v1", "https://github.com/techninja/hersheytextjs/blob/262f4782cd412ee539eb43d4d9d9b92562d7590b/svg_fonts/HersheySans1.svg", "n", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076578959L },
                    { 1563703640076579160L, "The Hershey Fonts were originally created by Dr. A. V. Hershey while working at the U. S. National Bureau of Standards. The format of the Font data in this distribution was originally created by James Hurt, Cognition, Inc., 900 Technology Park Drive, Billerica, MA 01821 (mit-eddie!ci-dandelion!hurt). SVG conversion by Windell H. Oskay. RealityLoop sampled/normalized paths and adapted the capital eszett reference.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "[[{\"x\":0.424603,\"y\":0.15},{\"x\":0.326191,\"y\":0.199206},{\"x\":0.224603,\"y\":0.300794},{\"x\":0.175397,\"y\":0.45},{\"x\":0.175397,\"y\":0.55},{\"x\":0.224603,\"y\":0.7},{\"x\":0.326191,\"y\":0.8},{\"x\":0.424603,\"y\":0.85},{\"x\":0.575397,\"y\":0.85},{\"x\":0.675397,\"y\":0.8},{\"x\":0.775397,\"y\":0.7},{\"x\":0.824603,\"y\":0.55},{\"x\":0.824603,\"y\":0.45},{\"x\":0.775397,\"y\":0.300794},{\"x\":0.675397,\"y\":0.199206},{\"x\":0.575397,\"y\":0.15},{\"x\":0.424603,\"y\":0.15}]]", 1, true, "Hershey font redistribution license", "https://github.com/techninja/hersheytextjs/blob/262f4782cd412ee539eb43d4d9d9b92562d7590b/svg_fonts/HersheySans1.svg", 1, "262f4782cd412ee539eb43d4d9d9b92562d7590b+de-latin-v1", "https://github.com/techninja/hersheytextjs/blob/262f4782cd412ee539eb43d4d9d9b92562d7590b/svg_fonts/HersheySans1.svg", "o", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076578960L },
                    { 1563703640076579161L, "The Hershey Fonts were originally created by Dr. A. V. Hershey while working at the U. S. National Bureau of Standards. The format of the Font data in this distribution was originally created by James Hurt, Cognition, Inc., 900 Technology Park Drive, Billerica, MA 01821 (mit-eddie!ci-dandelion!hurt). SVG conversion by Windell H. Oskay. RealityLoop sampled/normalized paths and adapted the capital eszett reference.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "[[{\"x\":0.299849,\"y\":0.15},{\"x\":0.299849,\"y\":0.85}],[{\"x\":0.299849,\"y\":0.250605},{\"x\":0.367625,\"y\":0.182829},{\"x\":0.433283,\"y\":0.15},{\"x\":0.533888,\"y\":0.15},{\"x\":0.600605,\"y\":0.182829},{\"x\":0.667322,\"y\":0.250605},{\"x\":0.700151,\"y\":0.350151},{\"x\":0.700151,\"y\":0.416868},{\"x\":0.667322,\"y\":0.516944},{\"x\":0.600605,\"y\":0.583661},{\"x\":0.533888,\"y\":0.61702},{\"x\":0.433283,\"y\":0.61702},{\"x\":0.367625,\"y\":0.583661},{\"x\":0.299849,\"y\":0.516944}]]", 1, true, "Hershey font redistribution license", "https://github.com/techninja/hersheytextjs/blob/262f4782cd412ee539eb43d4d9d9b92562d7590b/svg_fonts/HersheySans1.svg", 1, "262f4782cd412ee539eb43d4d9d9b92562d7590b+de-latin-v1", "https://github.com/techninja/hersheytextjs/blob/262f4782cd412ee539eb43d4d9d9b92562d7590b/svg_fonts/HersheySans1.svg", "p", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076578961L },
                    { 1563703640076579162L, "The Hershey Fonts were originally created by Dr. A. V. Hershey while working at the U. S. National Bureau of Standards. The format of the Font data in this distribution was originally created by James Hurt, Cognition, Inc., 900 Technology Park Drive, Billerica, MA 01821 (mit-eddie!ci-dandelion!hurt). SVG conversion by Windell H. Oskay. RealityLoop sampled/normalized paths and adapted the capital eszett reference.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "[[{\"x\":0.700151,\"y\":0.15},{\"x\":0.700151,\"y\":0.85}],[{\"x\":0.700151,\"y\":0.250605},{\"x\":0.633434,\"y\":0.182829},{\"x\":0.566717,\"y\":0.15},{\"x\":0.466112,\"y\":0.15},{\"x\":0.400454,\"y\":0.182829},{\"x\":0.332678,\"y\":0.250605},{\"x\":0.299849,\"y\":0.350151},{\"x\":0.299849,\"y\":0.416868},{\"x\":0.332678,\"y\":0.516944},{\"x\":0.400454,\"y\":0.583661},{\"x\":0.466112,\"y\":0.61702},{\"x\":0.566717,\"y\":0.61702},{\"x\":0.633434,\"y\":0.583661},{\"x\":0.700151,\"y\":0.516944}]]", 1, true, "Hershey font redistribution license", "https://github.com/techninja/hersheytextjs/blob/262f4782cd412ee539eb43d4d9d9b92562d7590b/svg_fonts/HersheySans1.svg", 1, "262f4782cd412ee539eb43d4d9d9b92562d7590b+de-latin-v1", "https://github.com/techninja/hersheytextjs/blob/262f4782cd412ee539eb43d4d9d9b92562d7590b/svg_fonts/HersheySans1.svg", "q", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076578962L },
                    { 1563703640076579163L, "The Hershey Fonts were originally created by Dr. A. V. Hershey while working at the U. S. National Bureau of Standards. The format of the Font data in this distribution was originally created by James Hurt, Cognition, Inc., 900 Technology Park Drive, Billerica, MA 01821 (mit-eddie!ci-dandelion!hurt). SVG conversion by Windell H. Oskay. RealityLoop sampled/normalized paths and adapted the capital eszett reference.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "[[{\"x\":0.3,\"y\":0.15},{\"x\":0.3,\"y\":0.85}],[{\"x\":0.3,\"y\":0.45},{\"x\":0.350794,\"y\":0.300794},{\"x\":0.450794,\"y\":0.199206},{\"x\":0.550794,\"y\":0.15},{\"x\":0.7,\"y\":0.15}]]", 1, true, "Hershey font redistribution license", "https://github.com/techninja/hersheytextjs/blob/262f4782cd412ee539eb43d4d9d9b92562d7590b/svg_fonts/HersheySans1.svg", 1, "262f4782cd412ee539eb43d4d9d9b92562d7590b+de-latin-v1", "https://github.com/techninja/hersheytextjs/blob/262f4782cd412ee539eb43d4d9d9b92562d7590b/svg_fonts/HersheySans1.svg", "r", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076578963L },
                    { 1563703640076579164L, "The Hershey Fonts were originally created by Dr. A. V. Hershey while working at the U. S. National Bureau of Standards. The format of the Font data in this distribution was originally created by James Hurt, Cognition, Inc., 900 Technology Park Drive, Billerica, MA 01821 (mit-eddie!ci-dandelion!hurt). SVG conversion by Windell H. Oskay. RealityLoop sampled/normalized paths and adapted the capital eszett reference.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "[[{\"x\":0.775397,\"y\":0.300794},{\"x\":0.724603,\"y\":0.199206},{\"x\":0.575397,\"y\":0.15},{\"x\":0.424603,\"y\":0.15},{\"x\":0.27381,\"y\":0.199206},{\"x\":0.224603,\"y\":0.300794},{\"x\":0.27381,\"y\":0.399206},{\"x\":0.375397,\"y\":0.45},{\"x\":0.624603,\"y\":0.500794},{\"x\":0.724603,\"y\":0.55},{\"x\":0.775397,\"y\":0.65},{\"x\":0.775397,\"y\":0.7},{\"x\":0.724603,\"y\":0.8},{\"x\":0.575397,\"y\":0.85},{\"x\":0.424603,\"y\":0.85},{\"x\":0.27381,\"y\":0.8},{\"x\":0.224603,\"y\":0.7}]]", 1, true, "Hershey font redistribution license", "https://github.com/techninja/hersheytextjs/blob/262f4782cd412ee539eb43d4d9d9b92562d7590b/svg_fonts/HersheySans1.svg", 1, "262f4782cd412ee539eb43d4d9d9b92562d7590b+de-latin-v1", "https://github.com/techninja/hersheytextjs/blob/262f4782cd412ee539eb43d4d9d9b92562d7590b/svg_fonts/HersheySans1.svg", "s", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076578964L },
                    { 1563703640076579165L, "The Hershey Fonts were originally created by Dr. A. V. Hershey while working at the U. S. National Bureau of Standards. The format of the Font data in this distribution was originally created by James Hurt, Cognition, Inc., 900 Technology Park Drive, Billerica, MA 01821 (mit-eddie!ci-dandelion!hurt). SVG conversion by Windell H. Oskay. RealityLoop sampled/normalized paths and adapted the capital eszett reference.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "[[{\"x\":0.466163,\"y\":0.15},{\"x\":0.466163,\"y\":0.716767},{\"x\":0.5,\"y\":0.816692},{\"x\":0.565559,\"y\":0.85},{\"x\":0.633233,\"y\":0.85}],[{\"x\":0.366767,\"y\":0.383686},{\"x\":0.599396,\"y\":0.383686}]]", 1, true, "Hershey font redistribution license", "https://github.com/techninja/hersheytextjs/blob/262f4782cd412ee539eb43d4d9d9b92562d7590b/svg_fonts/HersheySans1.svg", 1, "262f4782cd412ee539eb43d4d9d9b92562d7590b+de-latin-v1", "https://github.com/techninja/hersheytextjs/blob/262f4782cd412ee539eb43d4d9d9b92562d7590b/svg_fonts/HersheySans1.svg", "t", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076578965L },
                    { 1563703640076579166L, "The Hershey Fonts were originally created by Dr. A. V. Hershey while working at the U. S. National Bureau of Standards. The format of the Font data in this distribution was originally created by James Hurt, Cognition, Inc., 900 Technology Park Drive, Billerica, MA 01821 (mit-eddie!ci-dandelion!hurt). SVG conversion by Windell H. Oskay. RealityLoop sampled/normalized paths and adapted the capital eszett reference.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "[[{\"x\":0.224603,\"y\":0.15},{\"x\":0.224603,\"y\":0.65},{\"x\":0.275397,\"y\":0.8},{\"x\":0.375397,\"y\":0.85},{\"x\":0.52619,\"y\":0.85},{\"x\":0.624603,\"y\":0.8},{\"x\":0.775397,\"y\":0.65}],[{\"x\":0.775397,\"y\":0.15},{\"x\":0.775397,\"y\":0.85}]]", 1, true, "Hershey font redistribution license", "https://github.com/techninja/hersheytextjs/blob/262f4782cd412ee539eb43d4d9d9b92562d7590b/svg_fonts/HersheySans1.svg", 1, "262f4782cd412ee539eb43d4d9d9b92562d7590b+de-latin-v1", "https://github.com/techninja/hersheytextjs/blob/262f4782cd412ee539eb43d4d9d9b92562d7590b/svg_fonts/HersheySans1.svg", "u", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076578966L },
                    { 1563703640076579167L, "The Hershey Fonts were originally created by Dr. A. V. Hershey while working at the U. S. National Bureau of Standards. The format of the Font data in this distribution was originally created by James Hurt, Cognition, Inc., 900 Technology Park Drive, Billerica, MA 01821 (mit-eddie!ci-dandelion!hurt). SVG conversion by Windell H. Oskay. RealityLoop sampled/normalized paths and adapted the capital eszett reference.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "[[{\"x\":0.2,\"y\":0.15},{\"x\":0.498413,\"y\":0.85}],[{\"x\":0.8,\"y\":0.15},{\"x\":0.498413,\"y\":0.85}]]", 1, true, "Hershey font redistribution license", "https://github.com/techninja/hersheytextjs/blob/262f4782cd412ee539eb43d4d9d9b92562d7590b/svg_fonts/HersheySans1.svg", 1, "262f4782cd412ee539eb43d4d9d9b92562d7590b+de-latin-v1", "https://github.com/techninja/hersheytextjs/blob/262f4782cd412ee539eb43d4d9d9b92562d7590b/svg_fonts/HersheySans1.svg", "v", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076578967L },
                    { 1563703640076579168L, "The Hershey Fonts were originally created by Dr. A. V. Hershey while working at the U. S. National Bureau of Standards. The format of the Font data in this distribution was originally created by James Hurt, Cognition, Inc., 900 Technology Park Drive, Billerica, MA 01821 (mit-eddie!ci-dandelion!hurt). SVG conversion by Windell H. Oskay. RealityLoop sampled/normalized paths and adapted the capital eszett reference.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "[[{\"x\":0.15,\"y\":0.19375},{\"x\":0.325,\"y\":0.80625}],[{\"x\":0.5,\"y\":0.19375},{\"x\":0.325,\"y\":0.80625}],[{\"x\":0.5,\"y\":0.19375},{\"x\":0.675,\"y\":0.80625}],[{\"x\":0.85,\"y\":0.19375},{\"x\":0.675,\"y\":0.80625}]]", 1, true, "Hershey font redistribution license", "https://github.com/techninja/hersheytextjs/blob/262f4782cd412ee539eb43d4d9d9b92562d7590b/svg_fonts/HersheySans1.svg", 1, "262f4782cd412ee539eb43d4d9d9b92562d7590b+de-latin-v1", "https://github.com/techninja/hersheytextjs/blob/262f4782cd412ee539eb43d4d9d9b92562d7590b/svg_fonts/HersheySans1.svg", "w", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076578968L },
                    { 1563703640076579169L, "The Hershey Fonts were originally created by Dr. A. V. Hershey while working at the U. S. National Bureau of Standards. The format of the Font data in this distribution was originally created by James Hurt, Cognition, Inc., 900 Technology Park Drive, Billerica, MA 01821 (mit-eddie!ci-dandelion!hurt). SVG conversion by Windell H. Oskay. RealityLoop sampled/normalized paths and adapted the capital eszett reference.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "[[{\"x\":0.224603,\"y\":0.15},{\"x\":0.775397,\"y\":0.85}],[{\"x\":0.775397,\"y\":0.15},{\"x\":0.224603,\"y\":0.85}]]", 1, true, "Hershey font redistribution license", "https://github.com/techninja/hersheytextjs/blob/262f4782cd412ee539eb43d4d9d9b92562d7590b/svg_fonts/HersheySans1.svg", 1, "262f4782cd412ee539eb43d4d9d9b92562d7590b+de-latin-v1", "https://github.com/techninja/hersheytextjs/blob/262f4782cd412ee539eb43d4d9d9b92562d7590b/svg_fonts/HersheySans1.svg", "x", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076578969L },
                    { 1563703640076579170L, "The Hershey Fonts were originally created by Dr. A. V. Hershey while working at the U. S. National Bureau of Standards. The format of the Font data in this distribution was originally created by James Hurt, Cognition, Inc., 900 Technology Park Drive, Billerica, MA 01821 (mit-eddie!ci-dandelion!hurt). SVG conversion by Windell H. Oskay. RealityLoop sampled/normalized paths and adapted the capital eszett reference.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "[[{\"x\":0.316793,\"y\":0.15},{\"x\":0.515885,\"y\":0.61702}],[{\"x\":0.717095,\"y\":0.15},{\"x\":0.515885,\"y\":0.61702},{\"x\":0.450227,\"y\":0.750454},{\"x\":0.382451,\"y\":0.817171},{\"x\":0.316793,\"y\":0.85},{\"x\":0.282905,\"y\":0.85}]]", 1, true, "Hershey font redistribution license", "https://github.com/techninja/hersheytextjs/blob/262f4782cd412ee539eb43d4d9d9b92562d7590b/svg_fonts/HersheySans1.svg", 1, "262f4782cd412ee539eb43d4d9d9b92562d7590b+de-latin-v1", "https://github.com/techninja/hersheytextjs/blob/262f4782cd412ee539eb43d4d9d9b92562d7590b/svg_fonts/HersheySans1.svg", "y", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076578970L },
                    { 1563703640076579171L, "The Hershey Fonts were originally created by Dr. A. V. Hershey while working at the U. S. National Bureau of Standards. The format of the Font data in this distribution was originally created by James Hurt, Cognition, Inc., 900 Technology Park Drive, Billerica, MA 01821 (mit-eddie!ci-dandelion!hurt). SVG conversion by Windell H. Oskay. RealityLoop sampled/normalized paths and adapted the capital eszett reference.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "[[{\"x\":0.775397,\"y\":0.15},{\"x\":0.224603,\"y\":0.85}],[{\"x\":0.224603,\"y\":0.15},{\"x\":0.775397,\"y\":0.15}],[{\"x\":0.224603,\"y\":0.85},{\"x\":0.775397,\"y\":0.85}]]", 1, true, "Hershey font redistribution license", "https://github.com/techninja/hersheytextjs/blob/262f4782cd412ee539eb43d4d9d9b92562d7590b/svg_fonts/HersheySans1.svg", 1, "262f4782cd412ee539eb43d4d9d9b92562d7590b+de-latin-v1", "https://github.com/techninja/hersheytextjs/blob/262f4782cd412ee539eb43d4d9d9b92562d7590b/svg_fonts/HersheySans1.svg", "z", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076578971L },
                    { 1563703640076579172L, "The Hershey Fonts were originally created by Dr. A. V. Hershey while working at the U. S. National Bureau of Standards. The format of the Font data in this distribution was originally created by James Hurt, Cognition, Inc., 900 Technology Park Drive, Billerica, MA 01821 (mit-eddie!ci-dandelion!hurt). SVG conversion by Windell H. Oskay. RealityLoop sampled/normalized paths and adapted the capital eszett reference.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "[[{\"x\":0.683838,\"y\":0.404545},{\"x\":0.683838,\"y\":0.85}],[{\"x\":0.683838,\"y\":0.500505},{\"x\":0.620202,\"y\":0.435859},{\"x\":0.556566,\"y\":0.404545},{\"x\":0.460606,\"y\":0.404545},{\"x\":0.39798,\"y\":0.435859},{\"x\":0.333333,\"y\":0.500505},{\"x\":0.30202,\"y\":0.595454},{\"x\":0.30202,\"y\":0.659091},{\"x\":0.333333,\"y\":0.754546},{\"x\":0.39798,\"y\":0.818182},{\"x\":0.460606,\"y\":0.85},{\"x\":0.556566,\"y\":0.85},{\"x\":0.620202,\"y\":0.818182},{\"x\":0.683838,\"y\":0.754546}],[{\"x\":0.415152,\"y\":0.15},{\"x\":0.383838,\"y\":0.181313},{\"x\":0.415152,\"y\":0.213636},{\"x\":0.447475,\"y\":0.181313},{\"x\":0.415152,\"y\":0.15}],[{\"x\":0.665657,\"y\":0.15},{\"x\":0.634343,\"y\":0.181313},{\"x\":0.665657,\"y\":0.213636},{\"x\":0.69798,\"y\":0.181313},{\"x\":0.665657,\"y\":0.15}]]", 1, true, "Hershey font redistribution license", "https://github.com/techninja/hersheytextjs/blob/262f4782cd412ee539eb43d4d9d9b92562d7590b/svg_fonts/HersheySans1.svg", 1, "262f4782cd412ee539eb43d4d9d9b92562d7590b+de-latin-v1", "https://github.com/techninja/hersheytextjs/blob/262f4782cd412ee539eb43d4d9d9b92562d7590b/svg_fonts/HersheySans1.svg", "ä", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076578972L },
                    { 1563703640076579173L, "The Hershey Fonts were originally created by Dr. A. V. Hershey while working at the U. S. National Bureau of Standards. The format of the Font data in this distribution was originally created by James Hurt, Cognition, Inc., 900 Technology Park Drive, Billerica, MA 01821 (mit-eddie!ci-dandelion!hurt). SVG conversion by Windell H. Oskay. RealityLoop sampled/normalized paths and adapted the capital eszett reference.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "[[{\"x\":0.45202,\"y\":0.404545},{\"x\":0.389394,\"y\":0.435859},{\"x\":0.324747,\"y\":0.500505},{\"x\":0.293434,\"y\":0.595454},{\"x\":0.293434,\"y\":0.659091},{\"x\":0.324747,\"y\":0.754546},{\"x\":0.389394,\"y\":0.818182},{\"x\":0.45202,\"y\":0.85},{\"x\":0.54798,\"y\":0.85},{\"x\":0.611616,\"y\":0.818182},{\"x\":0.675253,\"y\":0.754546},{\"x\":0.706566,\"y\":0.659091},{\"x\":0.706566,\"y\":0.595454},{\"x\":0.675253,\"y\":0.500505},{\"x\":0.611616,\"y\":0.435859},{\"x\":0.54798,\"y\":0.404545},{\"x\":0.45202,\"y\":0.404545}],[{\"x\":0.407576,\"y\":0.15},{\"x\":0.376263,\"y\":0.181313},{\"x\":0.407576,\"y\":0.213636},{\"x\":0.439899,\"y\":0.181313},{\"x\":0.407576,\"y\":0.15}],[{\"x\":0.656061,\"y\":0.15},{\"x\":0.624747,\"y\":0.181313},{\"x\":0.656061,\"y\":0.213636},{\"x\":0.688384,\"y\":0.181313},{\"x\":0.656061,\"y\":0.15}]]", 1, true, "Hershey font redistribution license", "https://github.com/techninja/hersheytextjs/blob/262f4782cd412ee539eb43d4d9d9b92562d7590b/svg_fonts/HersheySans1.svg", 1, "262f4782cd412ee539eb43d4d9d9b92562d7590b+de-latin-v1", "https://github.com/techninja/hersheytextjs/blob/262f4782cd412ee539eb43d4d9d9b92562d7590b/svg_fonts/HersheySans1.svg", "ö", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076578973L },
                    { 1563703640076579174L, "The Hershey Fonts were originally created by Dr. A. V. Hershey while working at the U. S. National Bureau of Standards. The format of the Font data in this distribution was originally created by James Hurt, Cognition, Inc., 900 Technology Park Drive, Billerica, MA 01821 (mit-eddie!ci-dandelion!hurt). SVG conversion by Windell H. Oskay. RealityLoop sampled/normalized paths and adapted the capital eszett reference.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "[[{\"x\":0.313636,\"y\":0.404545},{\"x\":0.313636,\"y\":0.722727},{\"x\":0.34596,\"y\":0.818182},{\"x\":0.409596,\"y\":0.85},{\"x\":0.505556,\"y\":0.85},{\"x\":0.568182,\"y\":0.818182},{\"x\":0.664141,\"y\":0.722727}],[{\"x\":0.664141,\"y\":0.404545},{\"x\":0.664141,\"y\":0.85}],[{\"x\":0.387374,\"y\":0.15},{\"x\":0.356061,\"y\":0.181313},{\"x\":0.387374,\"y\":0.213636},{\"x\":0.419697,\"y\":0.181313},{\"x\":0.387374,\"y\":0.15}],[{\"x\":0.65404,\"y\":0.15},{\"x\":0.622727,\"y\":0.181313},{\"x\":0.65404,\"y\":0.213636},{\"x\":0.686364,\"y\":0.181313},{\"x\":0.65404,\"y\":0.15}]]", 1, true, "Hershey font redistribution license", "https://github.com/techninja/hersheytextjs/blob/262f4782cd412ee539eb43d4d9d9b92562d7590b/svg_fonts/HersheySans1.svg", 1, "262f4782cd412ee539eb43d4d9d9b92562d7590b+de-latin-v1", "https://github.com/techninja/hersheytextjs/blob/262f4782cd412ee539eb43d4d9d9b92562d7590b/svg_fonts/HersheySans1.svg", "ü", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076578974L },
                    { 1563703640076579175L, "The Hershey Fonts were originally created by Dr. A. V. Hershey while working at the U. S. National Bureau of Standards. The format of the Font data in this distribution was originally created by James Hurt, Cognition, Inc., 900 Technology Park Drive, Billerica, MA 01821 (mit-eddie!ci-dandelion!hurt). SVG conversion by Windell H. Oskay. RealityLoop sampled/normalized paths and adapted the capital eszett reference.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "[[{\"x\":0.425134,\"y\":0.775165},{\"x\":0.559837,\"y\":0.775165},{\"x\":0.647768,\"y\":0.745699},{\"x\":0.677703,\"y\":0.716232},{\"x\":0.707636,\"y\":0.6573},{\"x\":0.707636,\"y\":0.569368},{\"x\":0.677703,\"y\":0.5095},{\"x\":0.647768,\"y\":0.480502},{\"x\":0.559837,\"y\":0.451503},{\"x\":0.573862,\"y\":0.443564},{\"x\":0.58693,\"y\":0.435067},{\"x\":0.599043,\"y\":0.426059},{\"x\":0.610204,\"y\":0.416587},{\"x\":0.620414,\"y\":0.406699},{\"x\":0.629677,\"y\":0.396443},{\"x\":0.637993,\"y\":0.385865},{\"x\":0.645366,\"y\":0.375013},{\"x\":0.651796,\"y\":0.363936},{\"x\":0.657288,\"y\":0.352679},{\"x\":0.661842,\"y\":0.341291},{\"x\":0.665462,\"y\":0.32982},{\"x\":0.668148,\"y\":0.318312},{\"x\":0.669904,\"y\":0.306815},{\"x\":0.670732,\"y\":0.295377},{\"x\":0.670633,\"y\":0.284045},{\"x\":0.66961,\"y\":0.272866},{\"x\":0.667665,\"y\":0.261889},{\"x\":0.664801,\"y\":0.25116},{\"x\":0.661019,\"y\":0.240727},{\"x\":0.656322,\"y\":0.230638},{\"x\":0.650712,\"y\":0.22094},{\"x\":0.644191,\"y\":0.21168},{\"x\":0.636761,\"y\":0.202906},{\"x\":0.628424,\"y\":0.194665},{\"x\":0.619183,\"y\":0.187005},{\"x\":0.609041,\"y\":0.179974},{\"x\":0.597998,\"y\":0.173618},{\"x\":0.586057,\"y\":0.167986},{\"x\":0.573221,\"y\":0.163125},{\"x\":0.559491,\"y\":0.159082},{\"x\":0.54487,\"y\":0.155904},{\"x\":0.540741,\"y\":0.155178},{\"x\":0.536517,\"y\":0.154433},{\"x\":0.532198,\"y\":0.153687},{\"x\":0.527781,\"y\":0.15296},{\"x\":0.523266,\"y\":0.152272},{\"x\":0.51865,\"y\":0.15164},{\"x\":0.513933,\"y\":0.151084},{\"x\":0.509113,\"y\":0.150623},{\"x\":0.504188,\"y\":0.150276},{\"x\":0.499157,\"y\":0.150062},{\"x\":0.494019,\"y\":0.15},{\"x\":0.488771,\"y\":0.150109},{\"x\":0.483413,\"y\":0.150408},{\"x\":0.477944,\"y\":0.150916},{\"x\":0.47236,\"y\":0.151652},{\"x\":0.466662,\"y\":0.152636},{\"x\":0.460848,\"y\":0.153885},{\"x\":0.454916,\"y\":0.15542},{\"x\":0.448864,\"y\":0.157258},{\"x\":0.442692,\"y\":0.15942},{\"x\":0.436398,\"y\":0.161924},{\"x\":0.42998,\"y\":0.164789},{\"x\":0.423436,\"y\":0.168035},{\"x\":0.416767,\"y\":0.171679},{\"x\":0.409969,\"y\":0.175742},{\"x\":0.403041,\"y\":0.180242},{\"x\":0.395983,\"y\":0.185199},{\"x\":0.388792,\"y\":0.190631},{\"x\":0.381467,\"y\":0.196557},{\"x\":0.374007,\"y\":0.202996},{\"x\":0.366409,\"y\":0.209968},{\"x\":0.358674,\"y\":0.217491},{\"x\":0.351164,\"y\":0.225265},{\"x\":0.344229,\"y\":0.232979},{\"x\":0.337849,\"y\":0.240637},{\"x\":0.331999,\"y\":0.248242},{\"x\":0.326659,\"y\":0.255799},{\"x\":0.321806,\"y\":0.263311},{\"x\":0.317417,\"y\":0.270782},{\"x\":0.313472,\"y\":0.278216},{\"x\":0.309948,\"y\":0.285616},{\"x\":0.306822,\"y\":0.292986},{\"x\":0.304073,\"y\":0.300331},{\"x\":0.301678,\"y\":0.307653},{\"x\":0.299616,\"y\":0.314957},{\"x\":0.297864,\"y\":0.322246},{\"x\":0.2964,\"y\":0.329524},{\"x\":0.295202,\"y\":0.336795},{\"x\":0.294248,\"y\":0.344063},{\"x\":0.293516,\"y\":0.351331},{\"x\":0.292984,\"y\":0.358604},{\"x\":0.292629,\"y\":0.365884},{\"x\":0.292429,\"y\":0.373176},{\"x\":0.292364,\"y\":0.380484},{\"x\":0.292409,\"y\":0.387811},{\"x\":0.292543,\"y\":0.395161},{\"x\":0.292744,\"y\":0.402538},{\"x\":0.29299,\"y\":0.409945},{\"x\":0.293259,\"y\":0.417387},{\"x\":0.293529,\"y\":0.424867},{\"x\":0.293777,\"y\":0.432389},{\"x\":0.293982,\"y\":0.439957},{\"x\":0.294121,\"y\":0.447574},{\"x\":0.294172,\"y\":0.455245},{\"x\":0.294172,\"y\":0.468308},{\"x\":0.294172,\"y\":0.482024},{\"x\":0.294172,\"y\":0.496325},{\"x\":0.294172,\"y\":0.511139},{\"x\":0.294172,\"y\":0.526397},{\"x\":0.294172,\"y\":0.542029},{\"x\":0.294172,\"y\":0.557964},{\"x\":0.294172,\"y\":0.574132},{\"x\":0.294172,\"y\":0.590464},{\"x\":0.294172,\"y\":0.606889},{\"x\":0.294172,\"y\":0.623337},{\"x\":0.294172,\"y\":0.639738},{\"x\":0.294172,\"y\":0.656022},{\"x\":0.294172,\"y\":0.67212},{\"x\":0.294172,\"y\":0.68796},{\"x\":0.294172,\"y\":0.703473},{\"x\":0.294172,\"y\":0.718589},{\"x\":0.294172,\"y\":0.733237},{\"x\":0.294172,\"y\":0.747348},{\"x\":0.294172,\"y\":0.760851},{\"x\":0.294172,\"y\":0.773677},{\"x\":0.294172,\"y\":0.785755},{\"x\":0.294172,\"y\":0.797016},{\"x\":0.294172,\"y\":0.807388},{\"x\":0.294172,\"y\":0.816803},{\"x\":0.294172,\"y\":0.82519},{\"x\":0.294172,\"y\":0.832479},{\"x\":0.294172,\"y\":0.8386},{\"x\":0.294172,\"y\":0.843482},{\"x\":0.294172,\"y\":0.847056},{\"x\":0.294172,\"y\":0.849252},{\"x\":0.294172,\"y\":0.85}]]", 1, true, "Hershey font redistribution license", "https://github.com/techninja/hersheytextjs/blob/262f4782cd412ee539eb43d4d9d9b92562d7590b/svg_fonts/HersheySans1.svg", 1, "262f4782cd412ee539eb43d4d9d9b92562d7590b+de-latin-v1", "https://github.com/techninja/hersheytextjs/blob/262f4782cd412ee539eb43d4d9d9b92562d7590b/svg_fonts/HersheySans1.svg", "ß", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076578975L }
                });

            migrationBuilder.InsertData(
                table: "vocabulary_sense_translations",
                columns: new[] { "id", "created_at", "locale", "meaning", "updated_at", "usage_note", "version", "vocabulary_sense_id" },
                values: new object[,]
                {
                    { 1563703640076580416L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Xin chào.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580116L },
                    { 1563703640076580417L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "Hello.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580116L },
                    { 1563703640076580418L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Chào buổi sáng.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580117L },
                    { 1563703640076580419L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "Good morning.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580117L },
                    { 1563703640076580420L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Xin chào; lời chào ban ngày.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580118L },
                    { 1563703640076580421L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "Good day; a daytime greeting.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580118L },
                    { 1563703640076580422L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Chào buổi tối.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580119L },
                    { 1563703640076580423L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "Good evening.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580119L },
                    { 1563703640076580424L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Tạm biệt; cách nói lịch sự.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580120L },
                    { 1563703640076580425L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "Goodbye, politely.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580120L },
                    { 1563703640076580426L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Chào tạm biệt; thân mật.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580121L },
                    { 1563703640076580427L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "Bye, informally.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580121L },
                    { 1563703640076580428L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Cảm ơn.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580122L },
                    { 1563703640076580429L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "Thank you.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580122L },
                    { 1563703640076580430L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Làm ơn; cũng dùng để đáp lời cảm ơn.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Tùy ngữ cảnh, bitte có thể là làm ơn, không có gì hoặc lời mời nhận một thứ.", 1L, 1563703640076580123L },
                    { 1563703640076580431L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "Please; also used to respond to thanks.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Depending on context, bitte can mean please, you are welcome, or here you are.", 1L, 1563703640076580123L },
                    { 1563703640076580432L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Xin lỗi; dùng để xin lỗi hoặc gọi sự chú ý.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580124L },
                    { 1563703640076580433L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "Excuse me; an apology or a way to get attention.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580124L },
                    { 1563703640076580434L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Vâng; có.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580125L },
                    { 1563703640076580435L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "Yes.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580125L },
                    { 1563703640076580436L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Không; lời đáp phủ định.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580126L },
                    { 1563703640076580437L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "No; a negative response.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580126L },
                    { 1563703640076580438L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Tôi.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580127L },
                    { 1563703640076580439L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "I.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580127L },
                    { 1563703640076580440L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Bạn; cách xưng hô thân mật với một người.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580128L },
                    { 1563703640076580441L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "You, addressing one person informally.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580128L },
                    { 1563703640076580442L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Anh ấy; ông ấy.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580129L },
                    { 1563703640076580443L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "He.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580129L },
                    { 1563703640076580444L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Cô ấy; hoặc họ, tùy cách chia động từ.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "sie ist nghĩa là cô ấy là, sie sind nghĩa là họ là. Phân biệt với Sie viết hoa dùng để xưng hô lịch sự.", 1L, 1563703640076580130L },
                    { 1563703640076580445L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "She; or they, depending on verb agreement.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "sie ist refers to she, while sie sind refers to they. Distinguish these from capitalized formal Sie.", 1L, 1563703640076580130L },
                    { 1563703640076580446L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Nó; đại từ ngôi ba số ít giống trung.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580131L },
                    { 1563703640076580447L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "It; the neuter third-person singular pronoun.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580131L },
                    { 1563703640076580448L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Chúng tôi; chúng ta.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580132L },
                    { 1563703640076580449L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "We.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580132L },
                    { 1563703640076580450L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Các bạn; cách xưng hô thân mật với nhiều người.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580133L },
                    { 1563703640076580451L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "You, addressing several people informally.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580133L },
                    { 1563703640076580452L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Ông; bà; anh; chị; cách xưng hô lịch sự.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Luôn viết hoa Sie khi xưng hô lịch sự. Động từ chia như ngôi ba số nhiều.", 1L, 1563703640076580134L },
                    { 1563703640076580453L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "You, in formal address to one or more people.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Capitalize Sie for formal address. The verb takes the third-person plural form.", 1L, 1563703640076580134L },
                    { 1563703640076580454L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Ai.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580135L },
                    { 1563703640076580455L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "Who.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580135L },
                    { 1563703640076580456L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Cái gì.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580136L },
                    { 1563703640076580457L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "What.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580136L },
                    { 1563703640076580458L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Ở đâu.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580137L },
                    { 1563703640076580459L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "Where.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580137L },
                    { 1563703640076580460L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Khi nào.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580138L },
                    { 1563703640076580461L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "When.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580138L },
                    { 1563703640076580462L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Như thế nào.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580139L },
                    { 1563703640076580463L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "How.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580139L },
                    { 1563703640076580464L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Số không.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580140L },
                    { 1563703640076580465L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "Zero.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580140L },
                    { 1563703640076580466L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Một.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580141L },
                    { 1563703640076580467L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "One.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580141L },
                    { 1563703640076580468L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Hai.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580142L },
                    { 1563703640076580469L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "Two.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580142L },
                    { 1563703640076580470L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Ba.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580143L },
                    { 1563703640076580471L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "Three.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580143L },
                    { 1563703640076580472L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Bốn.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580144L },
                    { 1563703640076580473L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "Four.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580144L },
                    { 1563703640076580474L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Năm.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580145L },
                    { 1563703640076580475L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "Five.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580145L },
                    { 1563703640076580476L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Sáu.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580146L },
                    { 1563703640076580477L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "Six.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580146L },
                    { 1563703640076580478L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Bảy.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580147L },
                    { 1563703640076580479L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "Seven.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580147L },
                    { 1563703640076580480L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Tám.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580148L },
                    { 1563703640076580481L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "Eight.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580148L },
                    { 1563703640076580482L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Chín.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580149L },
                    { 1563703640076580483L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "Nine.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580149L },
                    { 1563703640076580484L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Mười.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580150L },
                    { 1563703640076580485L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "Ten.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580150L },
                    { 1563703640076580486L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Mười một.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580151L },
                    { 1563703640076580487L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "Eleven.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580151L },
                    { 1563703640076580488L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Mười hai.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580152L },
                    { 1563703640076580489L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "Twelve.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580152L },
                    { 1563703640076580490L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Hai mươi.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580153L },
                    { 1563703640076580491L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "Twenty.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580153L },
                    { 1563703640076580492L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Một trăm.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580154L },
                    { 1563703640076580493L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "One hundred.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580154L },
                    { 1563703640076580494L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Thứ hai.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Mạo từ der Montag. Số nhiều Montage.", 1L, 1563703640076580155L },
                    { 1563703640076580495L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "Monday.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Article der Montag. Plural Montage.", 1L, 1563703640076580155L },
                    { 1563703640076580496L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Thứ ba.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Mạo từ der Dienstag. Số nhiều Dienstage.", 1L, 1563703640076580156L },
                    { 1563703640076580497L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "Tuesday.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Article der Dienstag. Plural Dienstage.", 1L, 1563703640076580156L },
                    { 1563703640076580498L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Thứ tư.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Mạo từ der Mittwoch. Số nhiều Mittwoche.", 1L, 1563703640076580157L },
                    { 1563703640076580499L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "Wednesday.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Article der Mittwoch. Plural Mittwoche.", 1L, 1563703640076580157L },
                    { 1563703640076580500L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Thứ năm.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Mạo từ der Donnerstag. Số nhiều Donnerstage.", 1L, 1563703640076580158L },
                    { 1563703640076580501L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "Thursday.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Article der Donnerstag. Plural Donnerstage.", 1L, 1563703640076580158L },
                    { 1563703640076580502L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Thứ sáu.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Mạo từ der Freitag. Số nhiều Freitage.", 1L, 1563703640076580159L },
                    { 1563703640076580503L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "Friday.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Article der Freitag. Plural Freitage.", 1L, 1563703640076580159L },
                    { 1563703640076580504L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Thứ bảy.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Mạo từ der Samstag. Số nhiều Samstage.", 1L, 1563703640076580160L },
                    { 1563703640076580505L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "Saturday.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Article der Samstag. Plural Samstage.", 1L, 1563703640076580160L },
                    { 1563703640076580506L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Chủ nhật.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Mạo từ der Sonntag. Số nhiều Sonntage.", 1L, 1563703640076580161L },
                    { 1563703640076580507L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "Sunday.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Article der Sonntag. Plural Sonntage.", 1L, 1563703640076580161L },
                    { 1563703640076580508L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Hôm nay.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580162L },
                    { 1563703640076580509L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "Today.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580162L },
                    { 1563703640076580510L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Ngày mai.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580163L },
                    { 1563703640076580511L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "Tomorrow.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580163L },
                    { 1563703640076580512L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Hôm qua.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580164L },
                    { 1563703640076580513L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "Yesterday.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580164L },
                    { 1563703640076580514L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Bây giờ.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580165L },
                    { 1563703640076580515L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "Now.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580165L },
                    { 1563703640076580516L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Luôn luôn.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580166L },
                    { 1563703640076580517L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "Always.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580166L },
                    { 1563703640076580518L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Thường xuyên.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580167L },
                    { 1563703640076580519L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "Often.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580167L },
                    { 1563703640076580520L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Thỉnh thoảng.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580168L },
                    { 1563703640076580521L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "Sometimes.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580168L },
                    { 1563703640076580522L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Không bao giờ.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580169L },
                    { 1563703640076580523L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "Never.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580169L },
                    { 1563703640076580524L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Ngày.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Mạo từ der Tag. Số nhiều Tage.", 1L, 1563703640076580170L },
                    { 1563703640076580525L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "A day.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Article der Tag. Plural Tage.", 1L, 1563703640076580170L },
                    { 1563703640076580526L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Tuần.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Mạo từ die Woche. Số nhiều Wochen.", 1L, 1563703640076580171L },
                    { 1563703640076580527L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "A week.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Article die Woche. Plural Wochen.", 1L, 1563703640076580171L },
                    { 1563703640076580528L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Tháng.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Mạo từ der Monat. Số nhiều Monate.", 1L, 1563703640076580172L },
                    { 1563703640076580529L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "A month.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Article der Monat. Plural Monate.", 1L, 1563703640076580172L },
                    { 1563703640076580530L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Năm.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Mạo từ das Jahr. Số nhiều Jahre.", 1L, 1563703640076580173L },
                    { 1563703640076580531L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "A year.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Article das Jahr. Plural Jahre.", 1L, 1563703640076580173L },
                    { 1563703640076580532L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Thời gian.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Mạo từ die Zeit. Số nhiều Zeiten.", 1L, 1563703640076580174L },
                    { 1563703640076580533L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "Time.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Article die Zeit. Plural Zeiten.", 1L, 1563703640076580174L },
                    { 1563703640076580534L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Một giờ; khoảng thời gian sáu mươi phút.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Mạo từ die Stunde. Số nhiều Stunden.", 1L, 1563703640076580175L },
                    { 1563703640076580535L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "An hour; a sixty-minute duration.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Article die Stunde. Plural Stunden.", 1L, 1563703640076580175L },
                    { 1563703640076580536L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Phút.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Mạo từ die Minute. Số nhiều Minuten.", 1L, 1563703640076580176L },
                    { 1563703640076580537L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "A minute.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Article die Minute. Plural Minuten.", 1L, 1563703640076580176L },
                    { 1563703640076580538L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Gia đình.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Mạo từ die Familie. Số nhiều Familien.", 1L, 1563703640076580177L },
                    { 1563703640076580539L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "A family.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Article die Familie. Plural Familien.", 1L, 1563703640076580177L },
                    { 1563703640076580540L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Mẹ.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Mạo từ die Mutter. Số nhiều Mütter.", 1L, 1563703640076580178L },
                    { 1563703640076580541L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "A mother.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Article die Mutter. Plural Mütter.", 1L, 1563703640076580178L },
                    { 1563703640076580542L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Bố; cha.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Mạo từ der Vater. Số nhiều Väter.", 1L, 1563703640076580179L },
                    { 1563703640076580543L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "A father.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Article der Vater. Plural Väter.", 1L, 1563703640076580179L },
                    { 1563703640076580544L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Anh hoặc em trai.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Mạo từ der Bruder. Số nhiều Brüder.", 1L, 1563703640076580180L },
                    { 1563703640076580545L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "A brother.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Article der Bruder. Plural Brüder.", 1L, 1563703640076580180L },
                    { 1563703640076580546L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Chị hoặc em gái.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Mạo từ die Schwester. Số nhiều Schwestern.", 1L, 1563703640076580181L },
                    { 1563703640076580547L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "A sister.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Article die Schwester. Plural Schwestern.", 1L, 1563703640076580181L },
                    { 1563703640076580548L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Trẻ em; người con.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Mạo từ das Kind. Số nhiều Kinder.", 1L, 1563703640076580182L },
                    { 1563703640076580549L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "A child.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Article das Kind. Plural Kinder.", 1L, 1563703640076580182L },
                    { 1563703640076580550L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Người đàn ông.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Mạo từ der Mann. Số nhiều Männer.", 1L, 1563703640076580183L },
                    { 1563703640076580551L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "A man.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Article der Mann. Plural Männer.", 1L, 1563703640076580183L },
                    { 1563703640076580552L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Người phụ nữ.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Mạo từ die Frau. Số nhiều Frauen.", 1L, 1563703640076580184L },
                    { 1563703640076580553L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "A woman.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Article die Frau. Plural Frauen.", 1L, 1563703640076580184L },
                    { 1563703640076580554L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Người bạn nam; có thể chỉ bạn trai tùy ngữ cảnh.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Mạo từ der Freund. Số nhiều Freunde.", 1L, 1563703640076580185L },
                    { 1563703640076580555L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "A male friend; sometimes a boyfriend, depending on context.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Article der Freund. Plural Freunde.", 1L, 1563703640076580185L },
                    { 1563703640076580556L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Người bạn nữ; có thể chỉ bạn gái tùy ngữ cảnh.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Mạo từ die Freundin. Số nhiều Freundinnen.", 1L, 1563703640076580186L },
                    { 1563703640076580557L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "A female friend; sometimes a girlfriend, depending on context.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Article die Freundin. Plural Freundinnen.", 1L, 1563703640076580186L },
                    { 1563703640076580558L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Tên.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Mạo từ der Name. Số nhiều Namen.", 1L, 1563703640076580187L },
                    { 1563703640076580559L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "A name.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Article der Name. Plural Namen.", 1L, 1563703640076580187L },
                    { 1563703640076580560L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Một người; cá nhân.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Mạo từ die Person. Số nhiều Personen.", 1L, 1563703640076580188L },
                    { 1563703640076580561L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "A person.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Article die Person. Plural Personen.", 1L, 1563703640076580188L },
                    { 1563703640076580562L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Giáo viên nam.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Mạo từ der Lehrer. Số nhiều Lehrer.", 1L, 1563703640076580189L },
                    { 1563703640076580563L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "A male teacher.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Article der Lehrer. Plural Lehrer.", 1L, 1563703640076580189L },
                    { 1563703640076580564L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Giáo viên nữ.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Mạo từ die Lehrerin. Số nhiều Lehrerinnen.", 1L, 1563703640076580190L },
                    { 1563703640076580565L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "A female teacher.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Article die Lehrerin. Plural Lehrerinnen.", 1L, 1563703640076580190L },
                    { 1563703640076580566L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Sinh viên nam.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Mạo từ der Student. Số nhiều Studenten.", 1L, 1563703640076580191L },
                    { 1563703640076580567L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "A male university student.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Article der Student. Plural Studenten.", 1L, 1563703640076580191L },
                    { 1563703640076580568L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Trường học.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Mạo từ die Schule. Số nhiều Schulen.", 1L, 1563703640076580192L },
                    { 1563703640076580569L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "A school.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Article die Schule. Plural Schulen.", 1L, 1563703640076580192L },
                    { 1563703640076580570L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Trường đại học.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Mạo từ die Universität. Số nhiều Universitäten.", 1L, 1563703640076580193L },
                    { 1563703640076580571L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "A university.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Article die Universität. Plural Universitäten.", 1L, 1563703640076580193L },
                    { 1563703640076580572L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Công việc; việc làm.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Mạo từ die Arbeit. Số nhiều Arbeiten.", 1L, 1563703640076580194L },
                    { 1563703640076580573L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "Work; a job.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Article die Arbeit. Plural Arbeiten.", 1L, 1563703640076580194L },
                    { 1563703640076580574L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Công ty.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Mạo từ die Firma. Số nhiều Firmen.", 1L, 1563703640076580195L },
                    { 1563703640076580575L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "A company.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Article die Firma. Plural Firmen.", 1L, 1563703640076580195L },
                    { 1563703640076580576L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Bác sĩ nam.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Mạo từ der Arzt. Số nhiều Ärzte.", 1L, 1563703640076580196L },
                    { 1563703640076580577L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "A male medical doctor.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Article der Arzt. Plural Ärzte.", 1L, 1563703640076580196L },
                    { 1563703640076580578L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Bệnh viện.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Mạo từ das Krankenhaus. Số nhiều Krankenhäuser.", 1L, 1563703640076580197L },
                    { 1563703640076580579L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "A hospital.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Article das Krankenhaus. Plural Krankenhäuser.", 1L, 1563703640076580197L },
                    { 1563703640076580580L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Hiệu thuốc.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Mạo từ die Apotheke. Số nhiều Apotheken.", 1L, 1563703640076580198L },
                    { 1563703640076580581L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "A pharmacy.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Article die Apotheke. Plural Apotheken.", 1L, 1563703640076580198L },
                    { 1563703640076580582L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Ngân hàng.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Mạo từ die Bank. Số nhiều Banken.", 1L, 1563703640076580199L },
                    { 1563703640076580583L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "A bank; a financial institution.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Article die Bank. Plural Banken.", 1L, 1563703640076580199L },
                    { 1563703640076580584L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Nhà ga đường sắt.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Mạo từ der Bahnhof. Số nhiều Bahnhöfe.", 1L, 1563703640076580200L },
                    { 1563703640076580585L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "A railway station.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Article der Bahnhof. Plural Bahnhöfe.", 1L, 1563703640076580200L },
                    { 1563703640076580586L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Sân bay.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Mạo từ der Flughafen. Số nhiều Flughäfen.", 1L, 1563703640076580201L },
                    { 1563703640076580587L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "An airport.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Article der Flughafen. Plural Flughäfen.", 1L, 1563703640076580201L },
                    { 1563703640076580588L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Thành phố.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Mạo từ die Stadt. Số nhiều Städte.", 1L, 1563703640076580202L },
                    { 1563703640076580589L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "A city.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Article die Stadt. Plural Städte.", 1L, 1563703640076580202L },
                    { 1563703640076580590L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Quốc gia.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Mạo từ das Land. Số nhiều Länder.", 1L, 1563703640076580203L },
                    { 1563703640076580591L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "A country.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Article das Land. Plural Länder.", 1L, 1563703640076580203L },
                    { 1563703640076580592L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Đường phố.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Mạo từ die Straße. Số nhiều Straßen.", 1L, 1563703640076580204L },
                    { 1563703640076580593L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "A street.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Article die Straße. Plural Straßen.", 1L, 1563703640076580204L },
                    { 1563703640076580594L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Ngôi nhà.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Mạo từ das Haus. Số nhiều Häuser.", 1L, 1563703640076580205L },
                    { 1563703640076580595L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "A house.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Article das Haus. Plural Häuser.", 1L, 1563703640076580205L },
                    { 1563703640076580596L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Căn hộ.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Mạo từ die Wohnung. Số nhiều Wohnungen.", 1L, 1563703640076580206L },
                    { 1563703640076580597L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "An apartment.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Article die Wohnung. Plural Wohnungen.", 1L, 1563703640076580206L },
                    { 1563703640076580598L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Phòng.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Mạo từ das Zimmer. Số nhiều Zimmer.", 1L, 1563703640076580207L },
                    { 1563703640076580599L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "A room.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Article das Zimmer. Plural Zimmer.", 1L, 1563703640076580207L },
                    { 1563703640076580600L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Nhà bếp.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Mạo từ die Küche. Số nhiều Küchen.", 1L, 1563703640076580208L },
                    { 1563703640076580601L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "A kitchen.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Article die Küche. Plural Küchen.", 1L, 1563703640076580208L },
                    { 1563703640076580602L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Phòng tắm.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Mạo từ das Bad. Số nhiều Bäder.", 1L, 1563703640076580209L },
                    { 1563703640076580603L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "A bathroom.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Article das Bad. Plural Bäder.", 1L, 1563703640076580209L },
                    { 1563703640076580604L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Khách sạn.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Mạo từ das Hotel. Số nhiều Hotels.", 1L, 1563703640076580210L },
                    { 1563703640076580605L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "A hotel.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Article das Hotel. Plural Hotels.", 1L, 1563703640076580210L },
                    { 1563703640076580606L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Nhà hàng.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Mạo từ das Restaurant. Số nhiều Restaurants.", 1L, 1563703640076580211L },
                    { 1563703640076580607L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "A restaurant.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Article das Restaurant. Plural Restaurants.", 1L, 1563703640076580211L },
                    { 1563703640076580608L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Siêu thị.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Mạo từ der Supermarkt. Số nhiều Supermärkte.", 1L, 1563703640076580212L },
                    { 1563703640076580609L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "A supermarket.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Article der Supermarkt. Plural Supermärkte.", 1L, 1563703640076580212L },
                    { 1563703640076580610L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Cửa hàng.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Mạo từ das Geschäft. Số nhiều Geschäfte.", 1L, 1563703640076580213L },
                    { 1563703640076580611L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "A shop.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Article das Geschäft. Plural Geschäfte.", 1L, 1563703640076580213L },
                    { 1563703640076580612L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Nước.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Mạo từ das Wasser. Trong nghĩa cơ bản này thường dùng như danh từ không đếm được.", 1L, 1563703640076580214L },
                    { 1563703640076580613L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "Water.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Article das Wasser. In this basic sense it is usually used as an uncountable noun.", 1L, 1563703640076580214L },
                    { 1563703640076580614L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Cà phê.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Mạo từ der Kaffee. Trong nghĩa cơ bản này thường dùng như danh từ không đếm được.", 1L, 1563703640076580215L },
                    { 1563703640076580615L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "Coffee.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Article der Kaffee. In this basic sense it is usually used as an uncountable noun.", 1L, 1563703640076580215L },
                    { 1563703640076580616L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Trà.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Mạo từ der Tee. Trong nghĩa cơ bản này thường dùng như danh từ không đếm được.", 1L, 1563703640076580216L },
                    { 1563703640076580617L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "Tea.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Article der Tee. In this basic sense it is usually used as an uncountable noun.", 1L, 1563703640076580216L },
                    { 1563703640076580618L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Sữa.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Mạo từ die Milch. Trong nghĩa cơ bản này thường dùng như danh từ không đếm được.", 1L, 1563703640076580217L },
                    { 1563703640076580619L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "Milk.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Article die Milch. In this basic sense it is usually used as an uncountable noun.", 1L, 1563703640076580217L },
                    { 1563703640076580620L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Bánh mì.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Mạo từ das Brot. Số nhiều Brote.", 1L, 1563703640076580218L },
                    { 1563703640076580621L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "Bread; a loaf of bread.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Article das Brot. Plural Brote.", 1L, 1563703640076580218L },
                    { 1563703640076580622L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Bánh mì ổ nhỏ.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Mạo từ das Brötchen. Số nhiều Brötchen.", 1L, 1563703640076580219L },
                    { 1563703640076580623L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "A bread roll.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Article das Brötchen. Plural Brötchen.", 1L, 1563703640076580219L },
                    { 1563703640076580624L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Trứng.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Mạo từ das Ei. Số nhiều Eier.", 1L, 1563703640076580220L },
                    { 1563703640076580625L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "An egg.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Article das Ei. Plural Eier.", 1L, 1563703640076580220L },
                    { 1563703640076580626L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Phô mai.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Mạo từ der Käse. Trong nghĩa cơ bản này thường dùng như danh từ không đếm được.", 1L, 1563703640076580221L },
                    { 1563703640076580627L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "Cheese.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Article der Käse. In this basic sense it is usually used as an uncountable noun.", 1L, 1563703640076580221L },
                    { 1563703640076580628L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Thịt.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Mạo từ das Fleisch. Trong nghĩa cơ bản này thường dùng như danh từ không đếm được.", 1L, 1563703640076580222L },
                    { 1563703640076580629L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "Meat.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Article das Fleisch. In this basic sense it is usually used as an uncountable noun.", 1L, 1563703640076580222L },
                    { 1563703640076580630L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Cá.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Mạo từ der Fisch. Số nhiều Fische.", 1L, 1563703640076580223L },
                    { 1563703640076580631L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "A fish; fish as food.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Article der Fisch. Plural Fische.", 1L, 1563703640076580223L },
                    { 1563703640076580632L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Quả táo.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Mạo từ der Apfel. Số nhiều Äpfel.", 1L, 1563703640076580224L },
                    { 1563703640076580633L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "An apple.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Article der Apfel. Plural Äpfel.", 1L, 1563703640076580224L },
                    { 1563703640076580634L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Quả chuối.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Mạo từ die Banane. Số nhiều Bananen.", 1L, 1563703640076580225L },
                    { 1563703640076580635L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "A banana.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Article die Banane. Plural Bananen.", 1L, 1563703640076580225L },
                    { 1563703640076580636L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Khoai tây.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Mạo từ die Kartoffel. Số nhiều Kartoffeln.", 1L, 1563703640076580226L },
                    { 1563703640076580637L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "A potato.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Article die Kartoffel. Plural Kartoffeln.", 1L, 1563703640076580226L },
                    { 1563703640076580638L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Gạo; cơm.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Mạo từ der Reis. Trong nghĩa cơ bản này thường dùng như danh từ không đếm được.", 1L, 1563703640076580227L },
                    { 1563703640076580639L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "Rice.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Article der Reis. In this basic sense it is usually used as an uncountable noun.", 1L, 1563703640076580227L },
                    { 1563703640076580640L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Súp; canh.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Mạo từ die Suppe. Số nhiều Suppen.", 1L, 1563703640076580228L },
                    { 1563703640076580641L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "Soup; a soup.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Article die Suppe. Plural Suppen.", 1L, 1563703640076580228L },
                    { 1563703640076580642L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Bữa sáng.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Mạo từ das Frühstück. Số nhiều Frühstücke.", 1L, 1563703640076580229L },
                    { 1563703640076580643L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "Breakfast.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Article das Frühstück. Plural Frühstücke.", 1L, 1563703640076580229L },
                    { 1563703640076580644L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Tiền.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Mạo từ das Geld. Trong nghĩa cơ bản này thường dùng như danh từ không đếm được.", 1L, 1563703640076580230L },
                    { 1563703640076580645L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "Money.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Article das Geld. In this basic sense it is usually used as an uncountable noun.", 1L, 1563703640076580230L },
                    { 1563703640076580646L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Giá tiền.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Mạo từ der Preis. Số nhiều Preise.", 1L, 1563703640076580231L },
                    { 1563703640076580647L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "A price.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Article der Preis. Plural Preise.", 1L, 1563703640076580231L },
                    { 1563703640076580648L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Vé phương tiện giao thông.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Mạo từ die Fahrkarte. Số nhiều Fahrkarten.", 1L, 1563703640076580232L },
                    { 1563703640076580649L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "A transport ticket.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Article die Fahrkarte. Plural Fahrkarten.", 1L, 1563703640076580232L },
                    { 1563703640076580650L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Ô tô.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Mạo từ das Auto. Số nhiều Autos.", 1L, 1563703640076580233L },
                    { 1563703640076580651L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "A car.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Article das Auto. Plural Autos.", 1L, 1563703640076580233L },
                    { 1563703640076580652L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Xe buýt.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Mạo từ der Bus. Số nhiều Busse.", 1L, 1563703640076580234L },
                    { 1563703640076580653L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "A bus.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Article der Bus. Plural Busse.", 1L, 1563703640076580234L },
                    { 1563703640076580654L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Tàu hỏa.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Mạo từ der Zug. Số nhiều Züge.", 1L, 1563703640076580235L },
                    { 1563703640076580655L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "A train.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Article der Zug. Plural Züge.", 1L, 1563703640076580235L },
                    { 1563703640076580656L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Xe đạp.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Mạo từ das Fahrrad. Số nhiều Fahrräder.", 1L, 1563703640076580236L },
                    { 1563703640076580657L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "A bicycle.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Article das Fahrrad. Plural Fahrräder.", 1L, 1563703640076580236L },
                    { 1563703640076580658L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Quyển sách.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Mạo từ das Buch. Số nhiều Bücher.", 1L, 1563703640076580237L },
                    { 1563703640076580659L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "A book.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Article das Buch. Plural Bücher.", 1L, 1563703640076580237L },
                    { 1563703640076580660L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Điện thoại.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Mạo từ das Telefon. Số nhiều Telefone.", 1L, 1563703640076580238L },
                    { 1563703640076580661L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "A telephone.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Article das Telefon. Plural Telefone.", 1L, 1563703640076580238L },
                    { 1563703640076580662L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Chìa khóa.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Mạo từ der Schlüssel. Số nhiều Schlüssel.", 1L, 1563703640076580239L },
                    { 1563703640076580663L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "A key.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Article der Schlüssel. Plural Schlüssel.", 1L, 1563703640076580239L },
                    { 1563703640076580664L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Túi xách.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Mạo từ die Tasche. Số nhiều Taschen.", 1L, 1563703640076580240L },
                    { 1563703640076580665L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "A bag.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Article die Tasche. Plural Taschen.", 1L, 1563703640076580240L },
                    { 1563703640076580666L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Cái bàn.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Mạo từ der Tisch. Số nhiều Tische.", 1L, 1563703640076580241L },
                    { 1563703640076580667L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "A table.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Article der Tisch. Plural Tische.", 1L, 1563703640076580241L },
                    { 1563703640076580668L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Cái ghế.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Mạo từ der Stuhl. Số nhiều Stühle.", 1L, 1563703640076580242L },
                    { 1563703640076580669L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "A chair.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Article der Stuhl. Plural Stühle.", 1L, 1563703640076580242L },
                    { 1563703640076580670L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Cửa ra vào.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Mạo từ die Tür. Số nhiều Türen.", 1L, 1563703640076580243L },
                    { 1563703640076580671L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "A door.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Article die Tür. Plural Türen.", 1L, 1563703640076580243L },
                    { 1563703640076580672L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Cửa sổ.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Mạo từ das Fenster. Số nhiều Fenster.", 1L, 1563703640076580244L },
                    { 1563703640076580673L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "A window.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Article das Fenster. Plural Fenster.", 1L, 1563703640076580244L },
                    { 1563703640076580674L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Là; ở trong một trạng thái.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580245L },
                    { 1563703640076580675L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "To be.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580245L },
                    { 1563703640076580676L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Có; sở hữu.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580246L },
                    { 1563703640076580677L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "To have.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580246L },
                    { 1563703640076580678L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Đi.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580247L },
                    { 1563703640076580679L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "To go; to walk.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580247L },
                    { 1563703640076580680L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Đến.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580248L },
                    { 1563703640076580681L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "To come.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580248L },
                    { 1563703640076580682L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Sống; cư trú ở đâu.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580249L },
                    { 1563703640076580683L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "To live; to reside somewhere.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580249L },
                    { 1563703640076580684L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Có tên là.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580250L },
                    { 1563703640076580685L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "To be called.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580250L },
                    { 1563703640076580686L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Nói; nói một ngôn ngữ.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580251L },
                    { 1563703640076580687L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "To speak.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580251L },
                    { 1563703640076580688L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Học.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580252L },
                    { 1563703640076580689L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "To learn.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580252L },
                    { 1563703640076580690L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Làm việc.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580253L },
                    { 1563703640076580691L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "To work.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580253L },
                    { 1563703640076580692L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Làm; tạo ra.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580254L },
                    { 1563703640076580693L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "To do; to make.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580254L },
                    { 1563703640076580694L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Mua.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580255L },
                    { 1563703640076580695L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "To buy.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580255L },
                    { 1563703640076580696L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Thanh toán.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580256L },
                    { 1563703640076580697L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "To pay.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580256L },
                    { 1563703640076580698L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Có giá là.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580257L },
                    { 1563703640076580699L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "To cost.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580257L },
                    { 1563703640076580700L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Cần.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580258L },
                    { 1563703640076580701L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "To need.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580258L },
                    { 1563703640076580702L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Tìm kiếm.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580259L },
                    { 1563703640076580703L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "To look for.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580259L },
                    { 1563703640076580704L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Tìm thấy.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580260L },
                    { 1563703640076580705L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "To find.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580260L },
                    { 1563703640076580706L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Ăn.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580261L },
                    { 1563703640076580707L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "To eat.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580261L },
                    { 1563703640076580708L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Uống.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580262L },
                    { 1563703640076580709L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "To drink.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580262L },
                    { 1563703640076580710L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Nấu ăn.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580263L },
                    { 1563703640076580711L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "To cook.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580263L },
                    { 1563703640076580712L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Đọc.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580264L },
                    { 1563703640076580713L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "To read.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580264L },
                    { 1563703640076580714L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Viết.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580265L },
                    { 1563703640076580715L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "To write.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580265L },
                    { 1563703640076580716L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Nghe.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580266L },
                    { 1563703640076580717L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "To hear; to listen.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580266L },
                    { 1563703640076580718L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Nhìn thấy; xem.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580267L },
                    { 1563703640076580719L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "To see; to watch.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580267L },
                    { 1563703640076580720L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Ngủ.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580268L },
                    { 1563703640076580721L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "To sleep.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580268L },
                    { 1563703640076580722L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Đi bằng phương tiện; lái xe.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580269L },
                    { 1563703640076580723L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "To travel by vehicle; to drive.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580269L },
                    { 1563703640076580724L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Lấy; nhận.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580270L },
                    { 1563703640076580725L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "To take.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580270L },
                    { 1563703640076580726L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Cho; đưa.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580271L },
                    { 1563703640076580727L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "To give.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580271L },
                    { 1563703640076580728L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Giúp đỡ.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "helfen đi với tân ngữ Dativ, như Ich helfe dir.", 1L, 1563703640076580272L },
                    { 1563703640076580729L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "To help.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "helfen takes a dative object, as in Ich helfe dir.", 1L, 1563703640076580272L },
                    { 1563703640076580730L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Hỏi.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580273L },
                    { 1563703640076580731L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "To ask.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580273L },
                    { 1563703640076580732L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Trả lời.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580274L },
                    { 1563703640076580733L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "To answer.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580274L },
                    { 1563703640076580734L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Hiểu.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580275L },
                    { 1563703640076580735L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "To understand.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580275L },
                    { 1563703640076580736L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Biết một sự việc hoặc thông tin.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Dùng wissen cho thông tin hoặc sự việc. kennen thường dùng khi quen biết người, địa điểm hoặc sự vật.", 1L, 1563703640076580276L },
                    { 1563703640076580737L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "To know a fact or piece of information.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Use wissen for facts or information. kennen commonly expresses familiarity with a person, place or thing.", 1L, 1563703640076580276L },
                    { 1563703640076580738L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Có thể; có khả năng.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580277L },
                    { 1563703640076580739L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "Can; to be able to.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580277L },
                    { 1563703640076580740L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Phải; cần phải.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580278L },
                    { 1563703640076580741L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "Must; to have to.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580278L },
                    { 1563703640076580742L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Muốn; có ý định.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580279L },
                    { 1563703640076580743L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "To want; to intend.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580279L },
                    { 1563703640076580744L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Được phép.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580280L },
                    { 1563703640076580745L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "May; to be allowed to.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580280L },
                    { 1563703640076580746L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Nên; phải theo lời yêu cầu hoặc chỉ dẫn.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580281L },
                    { 1563703640076580747L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "Should; to be supposed to.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580281L },
                    { 1563703640076580748L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Thích.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "mögen diễn đạt thích. möchte thường dùng để bày tỏ mong muốn lịch sự.", 1L, 1563703640076580282L },
                    { 1563703640076580749L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "To like.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "mögen expresses liking. möchte commonly expresses a polite wish.", 1L, 1563703640076580282L },
                    { 1563703640076580750L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Tốt.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580283L },
                    { 1563703640076580751L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "Good.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580283L },
                    { 1563703640076580752L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Tệ; xấu.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580284L },
                    { 1563703640076580753L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "Bad.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580284L },
                    { 1563703640076580754L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "To; lớn; cao.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580285L },
                    { 1563703640076580755L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "Big; large; tall.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580285L },
                    { 1563703640076580756L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Nhỏ.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580286L },
                    { 1563703640076580757L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "Small.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580286L },
                    { 1563703640076580758L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Mới.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580287L },
                    { 1563703640076580759L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "New.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580287L },
                    { 1563703640076580760L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Cũ; già; có tuổi.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580288L },
                    { 1563703640076580761L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "Old.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580288L },
                    { 1563703640076580762L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Đẹp.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580289L },
                    { 1563703640076580763L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "Beautiful; nice.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580289L },
                    { 1563703640076580764L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Đắt.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580290L },
                    { 1563703640076580765L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "Expensive.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580290L },
                    { 1563703640076580766L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Rẻ.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580291L },
                    { 1563703640076580767L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "Inexpensive; cheap.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580291L },
                    { 1563703640076580768L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Nóng.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580292L },
                    { 1563703640076580769L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "Hot.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580292L },
                    { 1563703640076580770L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Lạnh.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580293L },
                    { 1563703640076580771L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "Cold.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580293L },
                    { 1563703640076580772L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Ấm.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580294L },
                    { 1563703640076580773L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "Warm.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580294L },
                    { 1563703640076580774L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Nhanh.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580295L },
                    { 1563703640076580775L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "Fast.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580295L },
                    { 1563703640076580776L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Chậm.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580296L },
                    { 1563703640076580777L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "Slow.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580296L },
                    { 1563703640076580778L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Đúng; chính xác.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580297L },
                    { 1563703640076580779L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "Correct; right.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580297L },
                    { 1563703640076580780L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Sai.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580298L },
                    { 1563703640076580781L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "Incorrect; wrong.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580298L },
                    { 1563703640076580782L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Quan trọng.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580299L },
                    { 1563703640076580783L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "Important.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580299L },
                    { 1563703640076580784L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Mệt; buồn ngủ.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580300L },
                    { 1563703640076580785L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "Tired; sleepy.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580300L },
                    { 1563703640076580786L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Hạnh phúc.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580301L },
                    { 1563703640076580787L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "Happy.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580301L },
                    { 1563703640076580788L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Bị ốm; bệnh.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580302L },
                    { 1563703640076580789L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "Ill; sick.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580302L },
                    { 1563703640076580790L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Khỏe mạnh.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580303L },
                    { 1563703640076580791L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "Healthy.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580303L },
                    { 1563703640076580792L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Đang mở.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580304L },
                    { 1563703640076580793L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "Open.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580304L },
                    { 1563703640076580794L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Đang đóng.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580305L },
                    { 1563703640076580795L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "Closed.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580305L },
                    { 1563703640076580796L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Ở đây.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580306L },
                    { 1563703640076580797L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "Here.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580306L },
                    { 1563703640076580798L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Ở đó; ở kia.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580307L },
                    { 1563703640076580799L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "There.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580307L },
                    { 1563703640076580800L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Về bên trái.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580308L },
                    { 1563703640076580801L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "To the left; on the left.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580308L },
                    { 1563703640076580802L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Về bên phải.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580309L },
                    { 1563703640076580803L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "To the right; on the right.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580309L },
                    { 1563703640076580804L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Rất.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580310L },
                    { 1563703640076580805L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "Very.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580310L },
                    { 1563703640076580806L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Cũng.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580311L },
                    { 1563703640076580807L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "Also; too.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580311L },
                    { 1563703640076580808L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Chỉ; chỉ có.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580312L },
                    { 1563703640076580809L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "Only.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580312L },
                    { 1563703640076580810L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Và.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580313L },
                    { 1563703640076580811L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "And.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580313L },
                    { 1563703640076580812L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Hoặc; hay.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580314L },
                    { 1563703640076580813L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "Or.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580314L },
                    { 1563703640076580814L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "vi", "Nhưng.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580315L },
                    { 1563703640076580815L, new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "en", "But.", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, 1L, 1563703640076580315L }
                });

            migrationBuilder.InsertData(
                table: "writing_system_item_readings",
                columns: new[] { "id", "audio_url", "code", "created_at", "sort_order", "transcription", "transcription_system", "updated_at", "version", "writing_system_item_id" },
                values: new object[,]
                {
                    { 1563703640076579016L, null, "letter-name", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 0, "aː", "ipa", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076578916L },
                    { 1563703640076579017L, null, "letter-name", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 0, "beː", "ipa", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076578917L },
                    { 1563703640076579018L, null, "letter-name", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 0, "tseː", "ipa", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076578918L },
                    { 1563703640076579019L, null, "letter-name", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 0, "deː", "ipa", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076578919L },
                    { 1563703640076579020L, null, "letter-name", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 0, "eː", "ipa", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076578920L },
                    { 1563703640076579021L, null, "letter-name", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 0, "ɛf", "ipa", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076578921L },
                    { 1563703640076579022L, null, "letter-name", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 0, "ɡeː", "ipa", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076578922L },
                    { 1563703640076579023L, null, "letter-name", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 0, "haː", "ipa", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076578923L },
                    { 1563703640076579024L, null, "letter-name", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 0, "iː", "ipa", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076578924L },
                    { 1563703640076579025L, null, "letter-name", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 0, "jɔt", "ipa", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076578925L },
                    { 1563703640076579026L, null, "letter-name", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 0, "kaː", "ipa", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076578926L },
                    { 1563703640076579027L, null, "letter-name", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 0, "ɛl", "ipa", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076578927L },
                    { 1563703640076579028L, null, "letter-name", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 0, "ɛm", "ipa", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076578928L },
                    { 1563703640076579029L, null, "letter-name", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 0, "ɛn", "ipa", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076578929L },
                    { 1563703640076579030L, null, "letter-name", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 0, "oː", "ipa", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076578930L },
                    { 1563703640076579031L, null, "letter-name", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 0, "peː", "ipa", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076578931L },
                    { 1563703640076579032L, null, "letter-name", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 0, "kuː", "ipa", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076578932L },
                    { 1563703640076579033L, null, "letter-name", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 0, "ɛʁ", "ipa", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076578933L },
                    { 1563703640076579034L, null, "letter-name", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 0, "ɛs", "ipa", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076578934L },
                    { 1563703640076579035L, null, "letter-name", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 0, "teː", "ipa", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076578935L },
                    { 1563703640076579036L, null, "letter-name", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 0, "uː", "ipa", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076578936L },
                    { 1563703640076579037L, null, "letter-name", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 0, "faʊ", "ipa", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076578937L },
                    { 1563703640076579038L, null, "letter-name", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 0, "veː", "ipa", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076578938L },
                    { 1563703640076579039L, null, "letter-name", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 0, "ɪks", "ipa", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076578939L },
                    { 1563703640076579040L, null, "letter-name", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 0, "ˈʏpsilɔn", "ipa", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076578940L },
                    { 1563703640076579041L, null, "letter-name", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 0, "tsɛt", "ipa", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076578941L },
                    { 1563703640076579042L, null, "letter-name", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 0, "ɛː", "ipa", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076578942L },
                    { 1563703640076579043L, null, "letter-name", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 0, "øː", "ipa", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076578943L },
                    { 1563703640076579044L, null, "letter-name", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 0, "yː", "ipa", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076578944L },
                    { 1563703640076579045L, null, "letter-name", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 0, "ɛsˈtsɛt", "ipa", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076578945L },
                    { 1563703640076579046L, null, "letter-name", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 0, "aː", "ipa", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076578946L },
                    { 1563703640076579047L, null, "letter-name", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 0, "beː", "ipa", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076578947L },
                    { 1563703640076579048L, null, "letter-name", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 0, "tseː", "ipa", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076578948L },
                    { 1563703640076579049L, null, "letter-name", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 0, "deː", "ipa", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076578949L },
                    { 1563703640076579050L, null, "letter-name", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 0, "eː", "ipa", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076578950L },
                    { 1563703640076579051L, null, "letter-name", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 0, "ɛf", "ipa", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076578951L },
                    { 1563703640076579052L, null, "letter-name", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 0, "ɡeː", "ipa", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076578952L },
                    { 1563703640076579053L, null, "letter-name", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 0, "haː", "ipa", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076578953L },
                    { 1563703640076579054L, null, "letter-name", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 0, "iː", "ipa", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076578954L },
                    { 1563703640076579055L, null, "letter-name", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 0, "jɔt", "ipa", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076578955L },
                    { 1563703640076579056L, null, "letter-name", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 0, "kaː", "ipa", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076578956L },
                    { 1563703640076579057L, null, "letter-name", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 0, "ɛl", "ipa", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076578957L },
                    { 1563703640076579058L, null, "letter-name", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 0, "ɛm", "ipa", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076578958L },
                    { 1563703640076579059L, null, "letter-name", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 0, "ɛn", "ipa", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076578959L },
                    { 1563703640076579060L, null, "letter-name", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 0, "oː", "ipa", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076578960L },
                    { 1563703640076579061L, null, "letter-name", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 0, "peː", "ipa", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076578961L },
                    { 1563703640076579062L, null, "letter-name", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 0, "kuː", "ipa", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076578962L },
                    { 1563703640076579063L, null, "letter-name", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 0, "ɛʁ", "ipa", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076578963L },
                    { 1563703640076579064L, null, "letter-name", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 0, "ɛs", "ipa", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076578964L },
                    { 1563703640076579065L, null, "letter-name", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 0, "teː", "ipa", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076578965L },
                    { 1563703640076579066L, null, "letter-name", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 0, "uː", "ipa", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076578966L },
                    { 1563703640076579067L, null, "letter-name", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 0, "faʊ", "ipa", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076578967L },
                    { 1563703640076579068L, null, "letter-name", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 0, "veː", "ipa", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076578968L },
                    { 1563703640076579069L, null, "letter-name", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 0, "ɪks", "ipa", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076578969L },
                    { 1563703640076579070L, null, "letter-name", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 0, "ˈʏpsilɔn", "ipa", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076578970L },
                    { 1563703640076579071L, null, "letter-name", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 0, "tsɛt", "ipa", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076578971L },
                    { 1563703640076579072L, null, "letter-name", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 0, "ɛː", "ipa", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076578972L },
                    { 1563703640076579073L, null, "letter-name", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 0, "øː", "ipa", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076578973L },
                    { 1563703640076579074L, null, "letter-name", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 0, "yː", "ipa", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076578974L },
                    { 1563703640076579075L, null, "letter-name", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 0, "ɛsˈtsɛt", "ipa", new DateTimeOffset(new DateTime(2026, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 1L, 1563703640076578975L }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Catalog deletion cascades into private learning rows. Require explicit handling of live profiles first.
            migrationBuilder.Sql("""
                LOCK TABLE learning_profiles IN SHARE ROW EXCLUSIVE MODE;
                DO $$
                BEGIN
                    IF EXISTS (SELECT 1 FROM learning_profiles WHERE target_language = 'de') THEN
                        RAISE EXCEPTION 'German learning profiles exist. Preserve or explicitly migrate their data before rolling back German content.'
                            USING ERRCODE = 'P0001';
                    END IF;
                END $$;
                """);
            migrationBuilder.DeleteData(
                table: "grammar_point_translations",
                keyColumn: "id",
                keyValue: 1563703640076581116L);

            migrationBuilder.DeleteData(
                table: "grammar_point_translations",
                keyColumn: "id",
                keyValue: 1563703640076581117L);

            migrationBuilder.DeleteData(
                table: "grammar_point_translations",
                keyColumn: "id",
                keyValue: 1563703640076581118L);

            migrationBuilder.DeleteData(
                table: "grammar_point_translations",
                keyColumn: "id",
                keyValue: 1563703640076581119L);

            migrationBuilder.DeleteData(
                table: "grammar_point_translations",
                keyColumn: "id",
                keyValue: 1563703640076581120L);

            migrationBuilder.DeleteData(
                table: "grammar_point_translations",
                keyColumn: "id",
                keyValue: 1563703640076581121L);

            migrationBuilder.DeleteData(
                table: "grammar_point_translations",
                keyColumn: "id",
                keyValue: 1563703640076581122L);

            migrationBuilder.DeleteData(
                table: "grammar_point_translations",
                keyColumn: "id",
                keyValue: 1563703640076581123L);

            migrationBuilder.DeleteData(
                table: "grammar_point_translations",
                keyColumn: "id",
                keyValue: 1563703640076581124L);

            migrationBuilder.DeleteData(
                table: "grammar_point_translations",
                keyColumn: "id",
                keyValue: 1563703640076581125L);

            migrationBuilder.DeleteData(
                table: "grammar_point_translations",
                keyColumn: "id",
                keyValue: 1563703640076581126L);

            migrationBuilder.DeleteData(
                table: "grammar_point_translations",
                keyColumn: "id",
                keyValue: 1563703640076581127L);

            migrationBuilder.DeleteData(
                table: "grammar_point_translations",
                keyColumn: "id",
                keyValue: 1563703640076581128L);

            migrationBuilder.DeleteData(
                table: "grammar_point_translations",
                keyColumn: "id",
                keyValue: 1563703640076581129L);

            migrationBuilder.DeleteData(
                table: "grammar_point_translations",
                keyColumn: "id",
                keyValue: 1563703640076581130L);

            migrationBuilder.DeleteData(
                table: "grammar_point_translations",
                keyColumn: "id",
                keyValue: 1563703640076581131L);

            migrationBuilder.DeleteData(
                table: "grammar_point_translations",
                keyColumn: "id",
                keyValue: 1563703640076581132L);

            migrationBuilder.DeleteData(
                table: "grammar_point_translations",
                keyColumn: "id",
                keyValue: 1563703640076581133L);

            migrationBuilder.DeleteData(
                table: "grammar_point_translations",
                keyColumn: "id",
                keyValue: 1563703640076581134L);

            migrationBuilder.DeleteData(
                table: "grammar_point_translations",
                keyColumn: "id",
                keyValue: 1563703640076581135L);

            migrationBuilder.DeleteData(
                table: "grammar_point_translations",
                keyColumn: "id",
                keyValue: 1563703640076581136L);

            migrationBuilder.DeleteData(
                table: "grammar_point_translations",
                keyColumn: "id",
                keyValue: 1563703640076581137L);

            migrationBuilder.DeleteData(
                table: "grammar_point_translations",
                keyColumn: "id",
                keyValue: 1563703640076581138L);

            migrationBuilder.DeleteData(
                table: "grammar_point_translations",
                keyColumn: "id",
                keyValue: 1563703640076581139L);

            migrationBuilder.DeleteData(
                table: "grammar_point_translations",
                keyColumn: "id",
                keyValue: 1563703640076581140L);

            migrationBuilder.DeleteData(
                table: "grammar_point_translations",
                keyColumn: "id",
                keyValue: 1563703640076581141L);

            migrationBuilder.DeleteData(
                table: "grammar_point_translations",
                keyColumn: "id",
                keyValue: 1563703640076581142L);

            migrationBuilder.DeleteData(
                table: "grammar_point_translations",
                keyColumn: "id",
                keyValue: 1563703640076581143L);

            migrationBuilder.DeleteData(
                table: "grammar_point_translations",
                keyColumn: "id",
                keyValue: 1563703640076581144L);

            migrationBuilder.DeleteData(
                table: "grammar_point_translations",
                keyColumn: "id",
                keyValue: 1563703640076581145L);

            migrationBuilder.DeleteData(
                table: "grammar_point_translations",
                keyColumn: "id",
                keyValue: 1563703640076581146L);

            migrationBuilder.DeleteData(
                table: "grammar_point_translations",
                keyColumn: "id",
                keyValue: 1563703640076581147L);

            migrationBuilder.DeleteData(
                table: "grammar_point_translations",
                keyColumn: "id",
                keyValue: 1563703640076581148L);

            migrationBuilder.DeleteData(
                table: "grammar_point_translations",
                keyColumn: "id",
                keyValue: 1563703640076581149L);

            migrationBuilder.DeleteData(
                table: "grammar_point_translations",
                keyColumn: "id",
                keyValue: 1563703640076581150L);

            migrationBuilder.DeleteData(
                table: "grammar_point_translations",
                keyColumn: "id",
                keyValue: 1563703640076581151L);

            migrationBuilder.DeleteData(
                table: "grammar_point_translations",
                keyColumn: "id",
                keyValue: 1563703640076581152L);

            migrationBuilder.DeleteData(
                table: "grammar_point_translations",
                keyColumn: "id",
                keyValue: 1563703640076581153L);

            migrationBuilder.DeleteData(
                table: "grammar_point_translations",
                keyColumn: "id",
                keyValue: 1563703640076581154L);

            migrationBuilder.DeleteData(
                table: "grammar_point_translations",
                keyColumn: "id",
                keyValue: 1563703640076581155L);

            migrationBuilder.DeleteData(
                table: "grammar_point_translations",
                keyColumn: "id",
                keyValue: 1563703640076581156L);

            migrationBuilder.DeleteData(
                table: "grammar_point_translations",
                keyColumn: "id",
                keyValue: 1563703640076581157L);

            migrationBuilder.DeleteData(
                table: "grammar_point_translations",
                keyColumn: "id",
                keyValue: 1563703640076581158L);

            migrationBuilder.DeleteData(
                table: "grammar_point_translations",
                keyColumn: "id",
                keyValue: 1563703640076581159L);

            migrationBuilder.DeleteData(
                table: "grammar_point_translations",
                keyColumn: "id",
                keyValue: 1563703640076581160L);

            migrationBuilder.DeleteData(
                table: "grammar_point_translations",
                keyColumn: "id",
                keyValue: 1563703640076581161L);

            migrationBuilder.DeleteData(
                table: "grammar_point_translations",
                keyColumn: "id",
                keyValue: 1563703640076581162L);

            migrationBuilder.DeleteData(
                table: "grammar_point_translations",
                keyColumn: "id",
                keyValue: 1563703640076581163L);

            migrationBuilder.DeleteData(
                table: "grammar_point_translations",
                keyColumn: "id",
                keyValue: 1563703640076581164L);

            migrationBuilder.DeleteData(
                table: "grammar_point_translations",
                keyColumn: "id",
                keyValue: 1563703640076581165L);

            migrationBuilder.DeleteData(
                table: "grammar_point_translations",
                keyColumn: "id",
                keyValue: 1563703640076581166L);

            migrationBuilder.DeleteData(
                table: "grammar_point_translations",
                keyColumn: "id",
                keyValue: 1563703640076581167L);

            migrationBuilder.DeleteData(
                table: "grammar_point_translations",
                keyColumn: "id",
                keyValue: 1563703640076581168L);

            migrationBuilder.DeleteData(
                table: "grammar_point_translations",
                keyColumn: "id",
                keyValue: 1563703640076581169L);

            migrationBuilder.DeleteData(
                table: "grammar_point_translations",
                keyColumn: "id",
                keyValue: 1563703640076581170L);

            migrationBuilder.DeleteData(
                table: "grammar_point_translations",
                keyColumn: "id",
                keyValue: 1563703640076581171L);

            migrationBuilder.DeleteData(
                table: "grammar_point_translations",
                keyColumn: "id",
                keyValue: 1563703640076581172L);

            migrationBuilder.DeleteData(
                table: "grammar_point_translations",
                keyColumn: "id",
                keyValue: 1563703640076581173L);

            migrationBuilder.DeleteData(
                table: "grammar_point_translations",
                keyColumn: "id",
                keyValue: 1563703640076581174L);

            migrationBuilder.DeleteData(
                table: "grammar_point_translations",
                keyColumn: "id",
                keyValue: 1563703640076581175L);

            migrationBuilder.DeleteData(
                table: "grammar_point_translations",
                keyColumn: "id",
                keyValue: 1563703640076581176L);

            migrationBuilder.DeleteData(
                table: "grammar_point_translations",
                keyColumn: "id",
                keyValue: 1563703640076581177L);

            migrationBuilder.DeleteData(
                table: "grammar_point_translations",
                keyColumn: "id",
                keyValue: 1563703640076581178L);

            migrationBuilder.DeleteData(
                table: "grammar_point_translations",
                keyColumn: "id",
                keyValue: 1563703640076581179L);

            migrationBuilder.DeleteData(
                table: "grammar_point_translations",
                keyColumn: "id",
                keyValue: 1563703640076581180L);

            migrationBuilder.DeleteData(
                table: "grammar_point_translations",
                keyColumn: "id",
                keyValue: 1563703640076581181L);

            migrationBuilder.DeleteData(
                table: "grammar_point_translations",
                keyColumn: "id",
                keyValue: 1563703640076581182L);

            migrationBuilder.DeleteData(
                table: "grammar_point_translations",
                keyColumn: "id",
                keyValue: 1563703640076581183L);

            migrationBuilder.DeleteData(
                table: "grammar_point_translations",
                keyColumn: "id",
                keyValue: 1563703640076581184L);

            migrationBuilder.DeleteData(
                table: "grammar_point_translations",
                keyColumn: "id",
                keyValue: 1563703640076581185L);

            migrationBuilder.DeleteData(
                table: "grammar_point_translations",
                keyColumn: "id",
                keyValue: 1563703640076581186L);

            migrationBuilder.DeleteData(
                table: "grammar_point_translations",
                keyColumn: "id",
                keyValue: 1563703640076581187L);

            migrationBuilder.DeleteData(
                table: "grammar_point_translations",
                keyColumn: "id",
                keyValue: 1563703640076581188L);

            migrationBuilder.DeleteData(
                table: "grammar_point_translations",
                keyColumn: "id",
                keyValue: 1563703640076581189L);

            migrationBuilder.DeleteData(
                table: "grammar_point_translations",
                keyColumn: "id",
                keyValue: 1563703640076581190L);

            migrationBuilder.DeleteData(
                table: "grammar_point_translations",
                keyColumn: "id",
                keyValue: 1563703640076581191L);

            migrationBuilder.DeleteData(
                table: "grammar_point_translations",
                keyColumn: "id",
                keyValue: 1563703640076581192L);

            migrationBuilder.DeleteData(
                table: "grammar_point_translations",
                keyColumn: "id",
                keyValue: 1563703640076581193L);

            migrationBuilder.DeleteData(
                table: "grammar_point_translations",
                keyColumn: "id",
                keyValue: 1563703640076581194L);

            migrationBuilder.DeleteData(
                table: "grammar_point_translations",
                keyColumn: "id",
                keyValue: 1563703640076581195L);

            migrationBuilder.DeleteData(
                table: "grammar_point_translations",
                keyColumn: "id",
                keyValue: 1563703640076581196L);

            migrationBuilder.DeleteData(
                table: "grammar_point_translations",
                keyColumn: "id",
                keyValue: 1563703640076581197L);

            migrationBuilder.DeleteData(
                table: "grammar_point_translations",
                keyColumn: "id",
                keyValue: 1563703640076581198L);

            migrationBuilder.DeleteData(
                table: "grammar_point_translations",
                keyColumn: "id",
                keyValue: 1563703640076581199L);

            migrationBuilder.DeleteData(
                table: "grammar_point_translations",
                keyColumn: "id",
                keyValue: 1563703640076581200L);

            migrationBuilder.DeleteData(
                table: "grammar_point_translations",
                keyColumn: "id",
                keyValue: 1563703640076581201L);

            migrationBuilder.DeleteData(
                table: "grammar_point_translations",
                keyColumn: "id",
                keyValue: 1563703640076581202L);

            migrationBuilder.DeleteData(
                table: "grammar_point_translations",
                keyColumn: "id",
                keyValue: 1563703640076581203L);

            migrationBuilder.DeleteData(
                table: "grammar_point_translations",
                keyColumn: "id",
                keyValue: 1563703640076581204L);

            migrationBuilder.DeleteData(
                table: "grammar_point_translations",
                keyColumn: "id",
                keyValue: 1563703640076581205L);

            migrationBuilder.DeleteData(
                table: "grammar_point_translations",
                keyColumn: "id",
                keyValue: 1563703640076581206L);

            migrationBuilder.DeleteData(
                table: "grammar_point_translations",
                keyColumn: "id",
                keyValue: 1563703640076581207L);

            migrationBuilder.DeleteData(
                table: "grammar_point_translations",
                keyColumn: "id",
                keyValue: 1563703640076581208L);

            migrationBuilder.DeleteData(
                table: "grammar_point_translations",
                keyColumn: "id",
                keyValue: 1563703640076581209L);

            migrationBuilder.DeleteData(
                table: "grammar_point_translations",
                keyColumn: "id",
                keyValue: 1563703640076581210L);

            migrationBuilder.DeleteData(
                table: "grammar_point_translations",
                keyColumn: "id",
                keyValue: 1563703640076581211L);

            migrationBuilder.DeleteData(
                table: "grammar_point_translations",
                keyColumn: "id",
                keyValue: 1563703640076581212L);

            migrationBuilder.DeleteData(
                table: "grammar_point_translations",
                keyColumn: "id",
                keyValue: 1563703640076581213L);

            migrationBuilder.DeleteData(
                table: "grammar_point_translations",
                keyColumn: "id",
                keyValue: 1563703640076581214L);

            migrationBuilder.DeleteData(
                table: "grammar_point_translations",
                keyColumn: "id",
                keyValue: 1563703640076581215L);

            migrationBuilder.DeleteData(
                table: "handwriting_templates",
                keyColumn: "id",
                keyValue: 1563703640076579116L);

            migrationBuilder.DeleteData(
                table: "handwriting_templates",
                keyColumn: "id",
                keyValue: 1563703640076579117L);

            migrationBuilder.DeleteData(
                table: "handwriting_templates",
                keyColumn: "id",
                keyValue: 1563703640076579118L);

            migrationBuilder.DeleteData(
                table: "handwriting_templates",
                keyColumn: "id",
                keyValue: 1563703640076579119L);

            migrationBuilder.DeleteData(
                table: "handwriting_templates",
                keyColumn: "id",
                keyValue: 1563703640076579120L);

            migrationBuilder.DeleteData(
                table: "handwriting_templates",
                keyColumn: "id",
                keyValue: 1563703640076579121L);

            migrationBuilder.DeleteData(
                table: "handwriting_templates",
                keyColumn: "id",
                keyValue: 1563703640076579122L);

            migrationBuilder.DeleteData(
                table: "handwriting_templates",
                keyColumn: "id",
                keyValue: 1563703640076579123L);

            migrationBuilder.DeleteData(
                table: "handwriting_templates",
                keyColumn: "id",
                keyValue: 1563703640076579124L);

            migrationBuilder.DeleteData(
                table: "handwriting_templates",
                keyColumn: "id",
                keyValue: 1563703640076579125L);

            migrationBuilder.DeleteData(
                table: "handwriting_templates",
                keyColumn: "id",
                keyValue: 1563703640076579126L);

            migrationBuilder.DeleteData(
                table: "handwriting_templates",
                keyColumn: "id",
                keyValue: 1563703640076579127L);

            migrationBuilder.DeleteData(
                table: "handwriting_templates",
                keyColumn: "id",
                keyValue: 1563703640076579128L);

            migrationBuilder.DeleteData(
                table: "handwriting_templates",
                keyColumn: "id",
                keyValue: 1563703640076579129L);

            migrationBuilder.DeleteData(
                table: "handwriting_templates",
                keyColumn: "id",
                keyValue: 1563703640076579130L);

            migrationBuilder.DeleteData(
                table: "handwriting_templates",
                keyColumn: "id",
                keyValue: 1563703640076579131L);

            migrationBuilder.DeleteData(
                table: "handwriting_templates",
                keyColumn: "id",
                keyValue: 1563703640076579132L);

            migrationBuilder.DeleteData(
                table: "handwriting_templates",
                keyColumn: "id",
                keyValue: 1563703640076579133L);

            migrationBuilder.DeleteData(
                table: "handwriting_templates",
                keyColumn: "id",
                keyValue: 1563703640076579134L);

            migrationBuilder.DeleteData(
                table: "handwriting_templates",
                keyColumn: "id",
                keyValue: 1563703640076579135L);

            migrationBuilder.DeleteData(
                table: "handwriting_templates",
                keyColumn: "id",
                keyValue: 1563703640076579136L);

            migrationBuilder.DeleteData(
                table: "handwriting_templates",
                keyColumn: "id",
                keyValue: 1563703640076579137L);

            migrationBuilder.DeleteData(
                table: "handwriting_templates",
                keyColumn: "id",
                keyValue: 1563703640076579138L);

            migrationBuilder.DeleteData(
                table: "handwriting_templates",
                keyColumn: "id",
                keyValue: 1563703640076579139L);

            migrationBuilder.DeleteData(
                table: "handwriting_templates",
                keyColumn: "id",
                keyValue: 1563703640076579140L);

            migrationBuilder.DeleteData(
                table: "handwriting_templates",
                keyColumn: "id",
                keyValue: 1563703640076579141L);

            migrationBuilder.DeleteData(
                table: "handwriting_templates",
                keyColumn: "id",
                keyValue: 1563703640076579142L);

            migrationBuilder.DeleteData(
                table: "handwriting_templates",
                keyColumn: "id",
                keyValue: 1563703640076579143L);

            migrationBuilder.DeleteData(
                table: "handwriting_templates",
                keyColumn: "id",
                keyValue: 1563703640076579144L);

            migrationBuilder.DeleteData(
                table: "handwriting_templates",
                keyColumn: "id",
                keyValue: 1563703640076579145L);

            migrationBuilder.DeleteData(
                table: "handwriting_templates",
                keyColumn: "id",
                keyValue: 1563703640076579146L);

            migrationBuilder.DeleteData(
                table: "handwriting_templates",
                keyColumn: "id",
                keyValue: 1563703640076579147L);

            migrationBuilder.DeleteData(
                table: "handwriting_templates",
                keyColumn: "id",
                keyValue: 1563703640076579148L);

            migrationBuilder.DeleteData(
                table: "handwriting_templates",
                keyColumn: "id",
                keyValue: 1563703640076579149L);

            migrationBuilder.DeleteData(
                table: "handwriting_templates",
                keyColumn: "id",
                keyValue: 1563703640076579150L);

            migrationBuilder.DeleteData(
                table: "handwriting_templates",
                keyColumn: "id",
                keyValue: 1563703640076579151L);

            migrationBuilder.DeleteData(
                table: "handwriting_templates",
                keyColumn: "id",
                keyValue: 1563703640076579152L);

            migrationBuilder.DeleteData(
                table: "handwriting_templates",
                keyColumn: "id",
                keyValue: 1563703640076579153L);

            migrationBuilder.DeleteData(
                table: "handwriting_templates",
                keyColumn: "id",
                keyValue: 1563703640076579154L);

            migrationBuilder.DeleteData(
                table: "handwriting_templates",
                keyColumn: "id",
                keyValue: 1563703640076579155L);

            migrationBuilder.DeleteData(
                table: "handwriting_templates",
                keyColumn: "id",
                keyValue: 1563703640076579156L);

            migrationBuilder.DeleteData(
                table: "handwriting_templates",
                keyColumn: "id",
                keyValue: 1563703640076579157L);

            migrationBuilder.DeleteData(
                table: "handwriting_templates",
                keyColumn: "id",
                keyValue: 1563703640076579158L);

            migrationBuilder.DeleteData(
                table: "handwriting_templates",
                keyColumn: "id",
                keyValue: 1563703640076579159L);

            migrationBuilder.DeleteData(
                table: "handwriting_templates",
                keyColumn: "id",
                keyValue: 1563703640076579160L);

            migrationBuilder.DeleteData(
                table: "handwriting_templates",
                keyColumn: "id",
                keyValue: 1563703640076579161L);

            migrationBuilder.DeleteData(
                table: "handwriting_templates",
                keyColumn: "id",
                keyValue: 1563703640076579162L);

            migrationBuilder.DeleteData(
                table: "handwriting_templates",
                keyColumn: "id",
                keyValue: 1563703640076579163L);

            migrationBuilder.DeleteData(
                table: "handwriting_templates",
                keyColumn: "id",
                keyValue: 1563703640076579164L);

            migrationBuilder.DeleteData(
                table: "handwriting_templates",
                keyColumn: "id",
                keyValue: 1563703640076579165L);

            migrationBuilder.DeleteData(
                table: "handwriting_templates",
                keyColumn: "id",
                keyValue: 1563703640076579166L);

            migrationBuilder.DeleteData(
                table: "handwriting_templates",
                keyColumn: "id",
                keyValue: 1563703640076579167L);

            migrationBuilder.DeleteData(
                table: "handwriting_templates",
                keyColumn: "id",
                keyValue: 1563703640076579168L);

            migrationBuilder.DeleteData(
                table: "handwriting_templates",
                keyColumn: "id",
                keyValue: 1563703640076579169L);

            migrationBuilder.DeleteData(
                table: "handwriting_templates",
                keyColumn: "id",
                keyValue: 1563703640076579170L);

            migrationBuilder.DeleteData(
                table: "handwriting_templates",
                keyColumn: "id",
                keyValue: 1563703640076579171L);

            migrationBuilder.DeleteData(
                table: "handwriting_templates",
                keyColumn: "id",
                keyValue: 1563703640076579172L);

            migrationBuilder.DeleteData(
                table: "handwriting_templates",
                keyColumn: "id",
                keyValue: 1563703640076579173L);

            migrationBuilder.DeleteData(
                table: "handwriting_templates",
                keyColumn: "id",
                keyValue: 1563703640076579174L);

            migrationBuilder.DeleteData(
                table: "handwriting_templates",
                keyColumn: "id",
                keyValue: 1563703640076579175L);

            migrationBuilder.DeleteData(
                table: "language_translations",
                keyColumn: "id",
                keyValue: 1563703640076578817L);

            migrationBuilder.DeleteData(
                table: "language_translations",
                keyColumn: "id",
                keyValue: 1563703640076578818L);

            migrationBuilder.DeleteData(
                table: "pronunciation_examples",
                keyColumn: "id",
                keyValue: 1563703640076579416L);

            migrationBuilder.DeleteData(
                table: "pronunciation_examples",
                keyColumn: "id",
                keyValue: 1563703640076579417L);

            migrationBuilder.DeleteData(
                table: "pronunciation_examples",
                keyColumn: "id",
                keyValue: 1563703640076579418L);

            migrationBuilder.DeleteData(
                table: "pronunciation_examples",
                keyColumn: "id",
                keyValue: 1563703640076579419L);

            migrationBuilder.DeleteData(
                table: "pronunciation_examples",
                keyColumn: "id",
                keyValue: 1563703640076579420L);

            migrationBuilder.DeleteData(
                table: "pronunciation_examples",
                keyColumn: "id",
                keyValue: 1563703640076579421L);

            migrationBuilder.DeleteData(
                table: "pronunciation_examples",
                keyColumn: "id",
                keyValue: 1563703640076579422L);

            migrationBuilder.DeleteData(
                table: "pronunciation_examples",
                keyColumn: "id",
                keyValue: 1563703640076579423L);

            migrationBuilder.DeleteData(
                table: "pronunciation_examples",
                keyColumn: "id",
                keyValue: 1563703640076579424L);

            migrationBuilder.DeleteData(
                table: "pronunciation_examples",
                keyColumn: "id",
                keyValue: 1563703640076579425L);

            migrationBuilder.DeleteData(
                table: "pronunciation_examples",
                keyColumn: "id",
                keyValue: 1563703640076579426L);

            migrationBuilder.DeleteData(
                table: "pronunciation_examples",
                keyColumn: "id",
                keyValue: 1563703640076579427L);

            migrationBuilder.DeleteData(
                table: "pronunciation_examples",
                keyColumn: "id",
                keyValue: 1563703640076579428L);

            migrationBuilder.DeleteData(
                table: "pronunciation_examples",
                keyColumn: "id",
                keyValue: 1563703640076579429L);

            migrationBuilder.DeleteData(
                table: "pronunciation_examples",
                keyColumn: "id",
                keyValue: 1563703640076579430L);

            migrationBuilder.DeleteData(
                table: "pronunciation_examples",
                keyColumn: "id",
                keyValue: 1563703640076579431L);

            migrationBuilder.DeleteData(
                table: "pronunciation_examples",
                keyColumn: "id",
                keyValue: 1563703640076579432L);

            migrationBuilder.DeleteData(
                table: "pronunciation_examples",
                keyColumn: "id",
                keyValue: 1563703640076579433L);

            migrationBuilder.DeleteData(
                table: "pronunciation_examples",
                keyColumn: "id",
                keyValue: 1563703640076579434L);

            migrationBuilder.DeleteData(
                table: "pronunciation_examples",
                keyColumn: "id",
                keyValue: 1563703640076579435L);

            migrationBuilder.DeleteData(
                table: "pronunciation_examples",
                keyColumn: "id",
                keyValue: 1563703640076579436L);

            migrationBuilder.DeleteData(
                table: "pronunciation_examples",
                keyColumn: "id",
                keyValue: 1563703640076579437L);

            migrationBuilder.DeleteData(
                table: "pronunciation_examples",
                keyColumn: "id",
                keyValue: 1563703640076579438L);

            migrationBuilder.DeleteData(
                table: "pronunciation_examples",
                keyColumn: "id",
                keyValue: 1563703640076579439L);

            migrationBuilder.DeleteData(
                table: "pronunciation_examples",
                keyColumn: "id",
                keyValue: 1563703640076579440L);

            migrationBuilder.DeleteData(
                table: "pronunciation_examples",
                keyColumn: "id",
                keyValue: 1563703640076579441L);

            migrationBuilder.DeleteData(
                table: "pronunciation_examples",
                keyColumn: "id",
                keyValue: 1563703640076579442L);

            migrationBuilder.DeleteData(
                table: "pronunciation_examples",
                keyColumn: "id",
                keyValue: 1563703640076579443L);

            migrationBuilder.DeleteData(
                table: "pronunciation_examples",
                keyColumn: "id",
                keyValue: 1563703640076579444L);

            migrationBuilder.DeleteData(
                table: "pronunciation_examples",
                keyColumn: "id",
                keyValue: 1563703640076579445L);

            migrationBuilder.DeleteData(
                table: "pronunciation_examples",
                keyColumn: "id",
                keyValue: 1563703640076579446L);

            migrationBuilder.DeleteData(
                table: "pronunciation_examples",
                keyColumn: "id",
                keyValue: 1563703640076579447L);

            migrationBuilder.DeleteData(
                table: "pronunciation_examples",
                keyColumn: "id",
                keyValue: 1563703640076579448L);

            migrationBuilder.DeleteData(
                table: "pronunciation_examples",
                keyColumn: "id",
                keyValue: 1563703640076579449L);

            migrationBuilder.DeleteData(
                table: "pronunciation_examples",
                keyColumn: "id",
                keyValue: 1563703640076579450L);

            migrationBuilder.DeleteData(
                table: "pronunciation_examples",
                keyColumn: "id",
                keyValue: 1563703640076579451L);

            migrationBuilder.DeleteData(
                table: "pronunciation_examples",
                keyColumn: "id",
                keyValue: 1563703640076579452L);

            migrationBuilder.DeleteData(
                table: "pronunciation_examples",
                keyColumn: "id",
                keyValue: 1563703640076579453L);

            migrationBuilder.DeleteData(
                table: "pronunciation_examples",
                keyColumn: "id",
                keyValue: 1563703640076579454L);

            migrationBuilder.DeleteData(
                table: "pronunciation_examples",
                keyColumn: "id",
                keyValue: 1563703640076579455L);

            migrationBuilder.DeleteData(
                table: "pronunciation_examples",
                keyColumn: "id",
                keyValue: 1563703640076579456L);

            migrationBuilder.DeleteData(
                table: "pronunciation_examples",
                keyColumn: "id",
                keyValue: 1563703640076579457L);

            migrationBuilder.DeleteData(
                table: "pronunciation_examples",
                keyColumn: "id",
                keyValue: 1563703640076579458L);

            migrationBuilder.DeleteData(
                table: "pronunciation_examples",
                keyColumn: "id",
                keyValue: 1563703640076579459L);

            migrationBuilder.DeleteData(
                table: "pronunciation_examples",
                keyColumn: "id",
                keyValue: 1563703640076579460L);

            migrationBuilder.DeleteData(
                table: "pronunciation_examples",
                keyColumn: "id",
                keyValue: 1563703640076579461L);

            migrationBuilder.DeleteData(
                table: "pronunciation_examples",
                keyColumn: "id",
                keyValue: 1563703640076579462L);

            migrationBuilder.DeleteData(
                table: "pronunciation_examples",
                keyColumn: "id",
                keyValue: 1563703640076579463L);

            migrationBuilder.DeleteData(
                table: "pronunciation_examples",
                keyColumn: "id",
                keyValue: 1563703640076579464L);

            migrationBuilder.DeleteData(
                table: "pronunciation_examples",
                keyColumn: "id",
                keyValue: 1563703640076579465L);

            migrationBuilder.DeleteData(
                table: "pronunciation_examples",
                keyColumn: "id",
                keyValue: 1563703640076579466L);

            migrationBuilder.DeleteData(
                table: "pronunciation_examples",
                keyColumn: "id",
                keyValue: 1563703640076579467L);

            migrationBuilder.DeleteData(
                table: "pronunciation_examples",
                keyColumn: "id",
                keyValue: 1563703640076579468L);

            migrationBuilder.DeleteData(
                table: "pronunciation_examples",
                keyColumn: "id",
                keyValue: 1563703640076579469L);

            migrationBuilder.DeleteData(
                table: "pronunciation_examples",
                keyColumn: "id",
                keyValue: 1563703640076579470L);

            migrationBuilder.DeleteData(
                table: "pronunciation_examples",
                keyColumn: "id",
                keyValue: 1563703640076579471L);

            migrationBuilder.DeleteData(
                table: "pronunciation_examples",
                keyColumn: "id",
                keyValue: 1563703640076579472L);

            migrationBuilder.DeleteData(
                table: "pronunciation_examples",
                keyColumn: "id",
                keyValue: 1563703640076579473L);

            migrationBuilder.DeleteData(
                table: "pronunciation_examples",
                keyColumn: "id",
                keyValue: 1563703640076579474L);

            migrationBuilder.DeleteData(
                table: "pronunciation_examples",
                keyColumn: "id",
                keyValue: 1563703640076579475L);

            migrationBuilder.DeleteData(
                table: "pronunciation_rule_translations",
                keyColumn: "id",
                keyValue: 1563703640076579366L);

            migrationBuilder.DeleteData(
                table: "pronunciation_rule_translations",
                keyColumn: "id",
                keyValue: 1563703640076579367L);

            migrationBuilder.DeleteData(
                table: "pronunciation_rule_translations",
                keyColumn: "id",
                keyValue: 1563703640076579368L);

            migrationBuilder.DeleteData(
                table: "pronunciation_rule_translations",
                keyColumn: "id",
                keyValue: 1563703640076579369L);

            migrationBuilder.DeleteData(
                table: "pronunciation_rule_translations",
                keyColumn: "id",
                keyValue: 1563703640076579370L);

            migrationBuilder.DeleteData(
                table: "pronunciation_rule_translations",
                keyColumn: "id",
                keyValue: 1563703640076579371L);

            migrationBuilder.DeleteData(
                table: "pronunciation_rule_translations",
                keyColumn: "id",
                keyValue: 1563703640076579372L);

            migrationBuilder.DeleteData(
                table: "pronunciation_rule_translations",
                keyColumn: "id",
                keyValue: 1563703640076579373L);

            migrationBuilder.DeleteData(
                table: "pronunciation_rule_translations",
                keyColumn: "id",
                keyValue: 1563703640076579374L);

            migrationBuilder.DeleteData(
                table: "pronunciation_rule_translations",
                keyColumn: "id",
                keyValue: 1563703640076579375L);

            migrationBuilder.DeleteData(
                table: "pronunciation_rule_translations",
                keyColumn: "id",
                keyValue: 1563703640076579376L);

            migrationBuilder.DeleteData(
                table: "pronunciation_rule_translations",
                keyColumn: "id",
                keyValue: 1563703640076579377L);

            migrationBuilder.DeleteData(
                table: "pronunciation_rule_translations",
                keyColumn: "id",
                keyValue: 1563703640076579378L);

            migrationBuilder.DeleteData(
                table: "pronunciation_rule_translations",
                keyColumn: "id",
                keyValue: 1563703640076579379L);

            migrationBuilder.DeleteData(
                table: "pronunciation_rule_translations",
                keyColumn: "id",
                keyValue: 1563703640076579380L);

            migrationBuilder.DeleteData(
                table: "pronunciation_rule_translations",
                keyColumn: "id",
                keyValue: 1563703640076579381L);

            migrationBuilder.DeleteData(
                table: "pronunciation_rule_translations",
                keyColumn: "id",
                keyValue: 1563703640076579382L);

            migrationBuilder.DeleteData(
                table: "pronunciation_rule_translations",
                keyColumn: "id",
                keyValue: 1563703640076579383L);

            migrationBuilder.DeleteData(
                table: "pronunciation_rule_translations",
                keyColumn: "id",
                keyValue: 1563703640076579384L);

            migrationBuilder.DeleteData(
                table: "pronunciation_rule_translations",
                keyColumn: "id",
                keyValue: 1563703640076579385L);

            migrationBuilder.DeleteData(
                table: "pronunciation_rule_translations",
                keyColumn: "id",
                keyValue: 1563703640076579386L);

            migrationBuilder.DeleteData(
                table: "pronunciation_rule_translations",
                keyColumn: "id",
                keyValue: 1563703640076579387L);

            migrationBuilder.DeleteData(
                table: "pronunciation_rule_translations",
                keyColumn: "id",
                keyValue: 1563703640076579388L);

            migrationBuilder.DeleteData(
                table: "pronunciation_rule_translations",
                keyColumn: "id",
                keyValue: 1563703640076579389L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580416L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580417L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580418L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580419L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580420L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580421L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580422L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580423L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580424L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580425L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580426L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580427L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580428L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580429L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580430L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580431L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580432L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580433L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580434L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580435L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580436L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580437L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580438L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580439L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580440L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580441L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580442L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580443L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580444L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580445L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580446L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580447L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580448L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580449L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580450L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580451L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580452L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580453L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580454L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580455L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580456L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580457L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580458L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580459L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580460L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580461L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580462L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580463L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580464L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580465L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580466L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580467L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580468L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580469L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580470L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580471L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580472L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580473L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580474L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580475L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580476L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580477L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580478L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580479L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580480L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580481L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580482L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580483L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580484L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580485L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580486L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580487L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580488L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580489L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580490L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580491L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580492L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580493L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580494L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580495L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580496L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580497L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580498L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580499L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580500L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580501L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580502L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580503L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580504L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580505L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580506L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580507L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580508L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580509L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580510L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580511L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580512L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580513L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580514L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580515L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580516L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580517L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580518L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580519L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580520L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580521L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580522L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580523L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580524L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580525L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580526L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580527L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580528L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580529L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580530L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580531L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580532L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580533L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580534L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580535L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580536L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580537L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580538L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580539L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580540L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580541L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580542L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580543L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580544L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580545L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580546L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580547L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580548L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580549L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580550L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580551L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580552L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580553L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580554L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580555L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580556L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580557L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580558L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580559L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580560L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580561L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580562L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580563L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580564L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580565L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580566L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580567L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580568L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580569L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580570L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580571L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580572L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580573L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580574L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580575L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580576L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580577L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580578L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580579L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580580L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580581L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580582L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580583L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580584L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580585L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580586L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580587L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580588L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580589L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580590L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580591L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580592L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580593L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580594L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580595L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580596L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580597L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580598L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580599L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580600L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580601L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580602L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580603L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580604L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580605L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580606L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580607L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580608L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580609L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580610L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580611L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580612L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580613L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580614L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580615L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580616L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580617L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580618L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580619L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580620L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580621L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580622L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580623L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580624L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580625L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580626L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580627L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580628L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580629L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580630L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580631L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580632L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580633L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580634L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580635L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580636L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580637L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580638L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580639L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580640L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580641L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580642L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580643L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580644L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580645L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580646L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580647L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580648L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580649L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580650L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580651L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580652L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580653L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580654L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580655L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580656L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580657L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580658L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580659L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580660L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580661L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580662L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580663L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580664L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580665L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580666L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580667L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580668L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580669L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580670L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580671L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580672L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580673L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580674L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580675L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580676L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580677L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580678L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580679L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580680L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580681L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580682L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580683L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580684L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580685L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580686L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580687L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580688L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580689L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580690L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580691L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580692L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580693L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580694L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580695L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580696L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580697L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580698L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580699L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580700L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580701L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580702L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580703L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580704L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580705L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580706L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580707L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580708L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580709L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580710L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580711L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580712L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580713L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580714L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580715L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580716L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580717L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580718L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580719L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580720L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580721L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580722L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580723L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580724L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580725L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580726L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580727L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580728L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580729L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580730L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580731L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580732L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580733L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580734L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580735L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580736L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580737L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580738L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580739L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580740L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580741L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580742L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580743L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580744L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580745L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580746L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580747L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580748L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580749L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580750L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580751L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580752L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580753L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580754L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580755L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580756L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580757L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580758L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580759L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580760L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580761L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580762L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580763L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580764L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580765L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580766L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580767L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580768L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580769L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580770L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580771L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580772L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580773L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580774L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580775L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580776L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580777L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580778L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580779L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580780L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580781L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580782L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580783L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580784L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580785L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580786L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580787L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580788L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580789L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580790L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580791L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580792L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580793L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580794L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580795L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580796L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580797L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580798L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580799L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580800L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580801L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580802L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580803L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580804L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580805L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580806L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580807L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580808L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580809L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580810L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580811L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580812L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580813L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580814L);

            migrationBuilder.DeleteData(
                table: "vocabulary_sense_translations",
                keyColumn: "id",
                keyValue: 1563703640076580815L);

            migrationBuilder.DeleteData(
                table: "writing_system_item_readings",
                keyColumn: "id",
                keyValue: 1563703640076579016L);

            migrationBuilder.DeleteData(
                table: "writing_system_item_readings",
                keyColumn: "id",
                keyValue: 1563703640076579017L);

            migrationBuilder.DeleteData(
                table: "writing_system_item_readings",
                keyColumn: "id",
                keyValue: 1563703640076579018L);

            migrationBuilder.DeleteData(
                table: "writing_system_item_readings",
                keyColumn: "id",
                keyValue: 1563703640076579019L);

            migrationBuilder.DeleteData(
                table: "writing_system_item_readings",
                keyColumn: "id",
                keyValue: 1563703640076579020L);

            migrationBuilder.DeleteData(
                table: "writing_system_item_readings",
                keyColumn: "id",
                keyValue: 1563703640076579021L);

            migrationBuilder.DeleteData(
                table: "writing_system_item_readings",
                keyColumn: "id",
                keyValue: 1563703640076579022L);

            migrationBuilder.DeleteData(
                table: "writing_system_item_readings",
                keyColumn: "id",
                keyValue: 1563703640076579023L);

            migrationBuilder.DeleteData(
                table: "writing_system_item_readings",
                keyColumn: "id",
                keyValue: 1563703640076579024L);

            migrationBuilder.DeleteData(
                table: "writing_system_item_readings",
                keyColumn: "id",
                keyValue: 1563703640076579025L);

            migrationBuilder.DeleteData(
                table: "writing_system_item_readings",
                keyColumn: "id",
                keyValue: 1563703640076579026L);

            migrationBuilder.DeleteData(
                table: "writing_system_item_readings",
                keyColumn: "id",
                keyValue: 1563703640076579027L);

            migrationBuilder.DeleteData(
                table: "writing_system_item_readings",
                keyColumn: "id",
                keyValue: 1563703640076579028L);

            migrationBuilder.DeleteData(
                table: "writing_system_item_readings",
                keyColumn: "id",
                keyValue: 1563703640076579029L);

            migrationBuilder.DeleteData(
                table: "writing_system_item_readings",
                keyColumn: "id",
                keyValue: 1563703640076579030L);

            migrationBuilder.DeleteData(
                table: "writing_system_item_readings",
                keyColumn: "id",
                keyValue: 1563703640076579031L);

            migrationBuilder.DeleteData(
                table: "writing_system_item_readings",
                keyColumn: "id",
                keyValue: 1563703640076579032L);

            migrationBuilder.DeleteData(
                table: "writing_system_item_readings",
                keyColumn: "id",
                keyValue: 1563703640076579033L);

            migrationBuilder.DeleteData(
                table: "writing_system_item_readings",
                keyColumn: "id",
                keyValue: 1563703640076579034L);

            migrationBuilder.DeleteData(
                table: "writing_system_item_readings",
                keyColumn: "id",
                keyValue: 1563703640076579035L);

            migrationBuilder.DeleteData(
                table: "writing_system_item_readings",
                keyColumn: "id",
                keyValue: 1563703640076579036L);

            migrationBuilder.DeleteData(
                table: "writing_system_item_readings",
                keyColumn: "id",
                keyValue: 1563703640076579037L);

            migrationBuilder.DeleteData(
                table: "writing_system_item_readings",
                keyColumn: "id",
                keyValue: 1563703640076579038L);

            migrationBuilder.DeleteData(
                table: "writing_system_item_readings",
                keyColumn: "id",
                keyValue: 1563703640076579039L);

            migrationBuilder.DeleteData(
                table: "writing_system_item_readings",
                keyColumn: "id",
                keyValue: 1563703640076579040L);

            migrationBuilder.DeleteData(
                table: "writing_system_item_readings",
                keyColumn: "id",
                keyValue: 1563703640076579041L);

            migrationBuilder.DeleteData(
                table: "writing_system_item_readings",
                keyColumn: "id",
                keyValue: 1563703640076579042L);

            migrationBuilder.DeleteData(
                table: "writing_system_item_readings",
                keyColumn: "id",
                keyValue: 1563703640076579043L);

            migrationBuilder.DeleteData(
                table: "writing_system_item_readings",
                keyColumn: "id",
                keyValue: 1563703640076579044L);

            migrationBuilder.DeleteData(
                table: "writing_system_item_readings",
                keyColumn: "id",
                keyValue: 1563703640076579045L);

            migrationBuilder.DeleteData(
                table: "writing_system_item_readings",
                keyColumn: "id",
                keyValue: 1563703640076579046L);

            migrationBuilder.DeleteData(
                table: "writing_system_item_readings",
                keyColumn: "id",
                keyValue: 1563703640076579047L);

            migrationBuilder.DeleteData(
                table: "writing_system_item_readings",
                keyColumn: "id",
                keyValue: 1563703640076579048L);

            migrationBuilder.DeleteData(
                table: "writing_system_item_readings",
                keyColumn: "id",
                keyValue: 1563703640076579049L);

            migrationBuilder.DeleteData(
                table: "writing_system_item_readings",
                keyColumn: "id",
                keyValue: 1563703640076579050L);

            migrationBuilder.DeleteData(
                table: "writing_system_item_readings",
                keyColumn: "id",
                keyValue: 1563703640076579051L);

            migrationBuilder.DeleteData(
                table: "writing_system_item_readings",
                keyColumn: "id",
                keyValue: 1563703640076579052L);

            migrationBuilder.DeleteData(
                table: "writing_system_item_readings",
                keyColumn: "id",
                keyValue: 1563703640076579053L);

            migrationBuilder.DeleteData(
                table: "writing_system_item_readings",
                keyColumn: "id",
                keyValue: 1563703640076579054L);

            migrationBuilder.DeleteData(
                table: "writing_system_item_readings",
                keyColumn: "id",
                keyValue: 1563703640076579055L);

            migrationBuilder.DeleteData(
                table: "writing_system_item_readings",
                keyColumn: "id",
                keyValue: 1563703640076579056L);

            migrationBuilder.DeleteData(
                table: "writing_system_item_readings",
                keyColumn: "id",
                keyValue: 1563703640076579057L);

            migrationBuilder.DeleteData(
                table: "writing_system_item_readings",
                keyColumn: "id",
                keyValue: 1563703640076579058L);

            migrationBuilder.DeleteData(
                table: "writing_system_item_readings",
                keyColumn: "id",
                keyValue: 1563703640076579059L);

            migrationBuilder.DeleteData(
                table: "writing_system_item_readings",
                keyColumn: "id",
                keyValue: 1563703640076579060L);

            migrationBuilder.DeleteData(
                table: "writing_system_item_readings",
                keyColumn: "id",
                keyValue: 1563703640076579061L);

            migrationBuilder.DeleteData(
                table: "writing_system_item_readings",
                keyColumn: "id",
                keyValue: 1563703640076579062L);

            migrationBuilder.DeleteData(
                table: "writing_system_item_readings",
                keyColumn: "id",
                keyValue: 1563703640076579063L);

            migrationBuilder.DeleteData(
                table: "writing_system_item_readings",
                keyColumn: "id",
                keyValue: 1563703640076579064L);

            migrationBuilder.DeleteData(
                table: "writing_system_item_readings",
                keyColumn: "id",
                keyValue: 1563703640076579065L);

            migrationBuilder.DeleteData(
                table: "writing_system_item_readings",
                keyColumn: "id",
                keyValue: 1563703640076579066L);

            migrationBuilder.DeleteData(
                table: "writing_system_item_readings",
                keyColumn: "id",
                keyValue: 1563703640076579067L);

            migrationBuilder.DeleteData(
                table: "writing_system_item_readings",
                keyColumn: "id",
                keyValue: 1563703640076579068L);

            migrationBuilder.DeleteData(
                table: "writing_system_item_readings",
                keyColumn: "id",
                keyValue: 1563703640076579069L);

            migrationBuilder.DeleteData(
                table: "writing_system_item_readings",
                keyColumn: "id",
                keyValue: 1563703640076579070L);

            migrationBuilder.DeleteData(
                table: "writing_system_item_readings",
                keyColumn: "id",
                keyValue: 1563703640076579071L);

            migrationBuilder.DeleteData(
                table: "writing_system_item_readings",
                keyColumn: "id",
                keyValue: 1563703640076579072L);

            migrationBuilder.DeleteData(
                table: "writing_system_item_readings",
                keyColumn: "id",
                keyValue: 1563703640076579073L);

            migrationBuilder.DeleteData(
                table: "writing_system_item_readings",
                keyColumn: "id",
                keyValue: 1563703640076579074L);

            migrationBuilder.DeleteData(
                table: "writing_system_item_readings",
                keyColumn: "id",
                keyValue: 1563703640076579075L);

            migrationBuilder.DeleteData(
                table: "writing_system_translations",
                keyColumn: "id",
                keyValue: 1563703640076578836L);

            migrationBuilder.DeleteData(
                table: "writing_system_translations",
                keyColumn: "id",
                keyValue: 1563703640076578837L);

            migrationBuilder.DeleteData(
                table: "grammar_points",
                keyColumn: "id",
                keyValue: 1563703640076581016L);

            migrationBuilder.DeleteData(
                table: "grammar_points",
                keyColumn: "id",
                keyValue: 1563703640076581017L);

            migrationBuilder.DeleteData(
                table: "grammar_points",
                keyColumn: "id",
                keyValue: 1563703640076581018L);

            migrationBuilder.DeleteData(
                table: "grammar_points",
                keyColumn: "id",
                keyValue: 1563703640076581019L);

            migrationBuilder.DeleteData(
                table: "grammar_points",
                keyColumn: "id",
                keyValue: 1563703640076581020L);

            migrationBuilder.DeleteData(
                table: "grammar_points",
                keyColumn: "id",
                keyValue: 1563703640076581021L);

            migrationBuilder.DeleteData(
                table: "grammar_points",
                keyColumn: "id",
                keyValue: 1563703640076581022L);

            migrationBuilder.DeleteData(
                table: "grammar_points",
                keyColumn: "id",
                keyValue: 1563703640076581023L);

            migrationBuilder.DeleteData(
                table: "grammar_points",
                keyColumn: "id",
                keyValue: 1563703640076581024L);

            migrationBuilder.DeleteData(
                table: "grammar_points",
                keyColumn: "id",
                keyValue: 1563703640076581025L);

            migrationBuilder.DeleteData(
                table: "grammar_points",
                keyColumn: "id",
                keyValue: 1563703640076581026L);

            migrationBuilder.DeleteData(
                table: "grammar_points",
                keyColumn: "id",
                keyValue: 1563703640076581027L);

            migrationBuilder.DeleteData(
                table: "grammar_points",
                keyColumn: "id",
                keyValue: 1563703640076581028L);

            migrationBuilder.DeleteData(
                table: "grammar_points",
                keyColumn: "id",
                keyValue: 1563703640076581029L);

            migrationBuilder.DeleteData(
                table: "grammar_points",
                keyColumn: "id",
                keyValue: 1563703640076581030L);

            migrationBuilder.DeleteData(
                table: "grammar_points",
                keyColumn: "id",
                keyValue: 1563703640076581031L);

            migrationBuilder.DeleteData(
                table: "grammar_points",
                keyColumn: "id",
                keyValue: 1563703640076581032L);

            migrationBuilder.DeleteData(
                table: "grammar_points",
                keyColumn: "id",
                keyValue: 1563703640076581033L);

            migrationBuilder.DeleteData(
                table: "grammar_points",
                keyColumn: "id",
                keyValue: 1563703640076581034L);

            migrationBuilder.DeleteData(
                table: "grammar_points",
                keyColumn: "id",
                keyValue: 1563703640076581035L);

            migrationBuilder.DeleteData(
                table: "grammar_points",
                keyColumn: "id",
                keyValue: 1563703640076581036L);

            migrationBuilder.DeleteData(
                table: "grammar_points",
                keyColumn: "id",
                keyValue: 1563703640076581037L);

            migrationBuilder.DeleteData(
                table: "grammar_points",
                keyColumn: "id",
                keyValue: 1563703640076581038L);

            migrationBuilder.DeleteData(
                table: "grammar_points",
                keyColumn: "id",
                keyValue: 1563703640076581039L);

            migrationBuilder.DeleteData(
                table: "grammar_points",
                keyColumn: "id",
                keyValue: 1563703640076581040L);

            migrationBuilder.DeleteData(
                table: "grammar_points",
                keyColumn: "id",
                keyValue: 1563703640076581041L);

            migrationBuilder.DeleteData(
                table: "grammar_points",
                keyColumn: "id",
                keyValue: 1563703640076581042L);

            migrationBuilder.DeleteData(
                table: "grammar_points",
                keyColumn: "id",
                keyValue: 1563703640076581043L);

            migrationBuilder.DeleteData(
                table: "grammar_points",
                keyColumn: "id",
                keyValue: 1563703640076581044L);

            migrationBuilder.DeleteData(
                table: "grammar_points",
                keyColumn: "id",
                keyValue: 1563703640076581045L);

            migrationBuilder.DeleteData(
                table: "grammar_points",
                keyColumn: "id",
                keyValue: 1563703640076581046L);

            migrationBuilder.DeleteData(
                table: "grammar_points",
                keyColumn: "id",
                keyValue: 1563703640076581047L);

            migrationBuilder.DeleteData(
                table: "grammar_points",
                keyColumn: "id",
                keyValue: 1563703640076581048L);

            migrationBuilder.DeleteData(
                table: "grammar_points",
                keyColumn: "id",
                keyValue: 1563703640076581049L);

            migrationBuilder.DeleteData(
                table: "grammar_points",
                keyColumn: "id",
                keyValue: 1563703640076581050L);

            migrationBuilder.DeleteData(
                table: "grammar_points",
                keyColumn: "id",
                keyValue: 1563703640076581051L);

            migrationBuilder.DeleteData(
                table: "grammar_points",
                keyColumn: "id",
                keyValue: 1563703640076581052L);

            migrationBuilder.DeleteData(
                table: "grammar_points",
                keyColumn: "id",
                keyValue: 1563703640076581053L);

            migrationBuilder.DeleteData(
                table: "grammar_points",
                keyColumn: "id",
                keyValue: 1563703640076581054L);

            migrationBuilder.DeleteData(
                table: "grammar_points",
                keyColumn: "id",
                keyValue: 1563703640076581055L);

            migrationBuilder.DeleteData(
                table: "grammar_points",
                keyColumn: "id",
                keyValue: 1563703640076581056L);

            migrationBuilder.DeleteData(
                table: "grammar_points",
                keyColumn: "id",
                keyValue: 1563703640076581057L);

            migrationBuilder.DeleteData(
                table: "grammar_points",
                keyColumn: "id",
                keyValue: 1563703640076581058L);

            migrationBuilder.DeleteData(
                table: "grammar_points",
                keyColumn: "id",
                keyValue: 1563703640076581059L);

            migrationBuilder.DeleteData(
                table: "grammar_points",
                keyColumn: "id",
                keyValue: 1563703640076581060L);

            migrationBuilder.DeleteData(
                table: "grammar_points",
                keyColumn: "id",
                keyValue: 1563703640076581061L);

            migrationBuilder.DeleteData(
                table: "grammar_points",
                keyColumn: "id",
                keyValue: 1563703640076581062L);

            migrationBuilder.DeleteData(
                table: "grammar_points",
                keyColumn: "id",
                keyValue: 1563703640076581063L);

            migrationBuilder.DeleteData(
                table: "grammar_points",
                keyColumn: "id",
                keyValue: 1563703640076581064L);

            migrationBuilder.DeleteData(
                table: "grammar_points",
                keyColumn: "id",
                keyValue: 1563703640076581065L);

            migrationBuilder.DeleteData(
                table: "pronunciation_rules",
                keyColumn: "id",
                keyValue: 1563703640076579316L);

            migrationBuilder.DeleteData(
                table: "pronunciation_rules",
                keyColumn: "id",
                keyValue: 1563703640076579317L);

            migrationBuilder.DeleteData(
                table: "pronunciation_rules",
                keyColumn: "id",
                keyValue: 1563703640076579318L);

            migrationBuilder.DeleteData(
                table: "pronunciation_rules",
                keyColumn: "id",
                keyValue: 1563703640076579319L);

            migrationBuilder.DeleteData(
                table: "pronunciation_rules",
                keyColumn: "id",
                keyValue: 1563703640076579320L);

            migrationBuilder.DeleteData(
                table: "pronunciation_rules",
                keyColumn: "id",
                keyValue: 1563703640076579321L);

            migrationBuilder.DeleteData(
                table: "pronunciation_rules",
                keyColumn: "id",
                keyValue: 1563703640076579322L);

            migrationBuilder.DeleteData(
                table: "pronunciation_rules",
                keyColumn: "id",
                keyValue: 1563703640076579323L);

            migrationBuilder.DeleteData(
                table: "pronunciation_rules",
                keyColumn: "id",
                keyValue: 1563703640076579324L);

            migrationBuilder.DeleteData(
                table: "pronunciation_rules",
                keyColumn: "id",
                keyValue: 1563703640076579325L);

            migrationBuilder.DeleteData(
                table: "pronunciation_rules",
                keyColumn: "id",
                keyValue: 1563703640076579326L);

            migrationBuilder.DeleteData(
                table: "pronunciation_rules",
                keyColumn: "id",
                keyValue: 1563703640076579327L);

            migrationBuilder.DeleteData(
                table: "vocabulary_senses",
                keyColumn: "id",
                keyValue: 1563703640076580116L);

            migrationBuilder.DeleteData(
                table: "vocabulary_senses",
                keyColumn: "id",
                keyValue: 1563703640076580117L);

            migrationBuilder.DeleteData(
                table: "vocabulary_senses",
                keyColumn: "id",
                keyValue: 1563703640076580118L);

            migrationBuilder.DeleteData(
                table: "vocabulary_senses",
                keyColumn: "id",
                keyValue: 1563703640076580119L);

            migrationBuilder.DeleteData(
                table: "vocabulary_senses",
                keyColumn: "id",
                keyValue: 1563703640076580120L);

            migrationBuilder.DeleteData(
                table: "vocabulary_senses",
                keyColumn: "id",
                keyValue: 1563703640076580121L);

            migrationBuilder.DeleteData(
                table: "vocabulary_senses",
                keyColumn: "id",
                keyValue: 1563703640076580122L);

            migrationBuilder.DeleteData(
                table: "vocabulary_senses",
                keyColumn: "id",
                keyValue: 1563703640076580123L);

            migrationBuilder.DeleteData(
                table: "vocabulary_senses",
                keyColumn: "id",
                keyValue: 1563703640076580124L);

            migrationBuilder.DeleteData(
                table: "vocabulary_senses",
                keyColumn: "id",
                keyValue: 1563703640076580125L);

            migrationBuilder.DeleteData(
                table: "vocabulary_senses",
                keyColumn: "id",
                keyValue: 1563703640076580126L);

            migrationBuilder.DeleteData(
                table: "vocabulary_senses",
                keyColumn: "id",
                keyValue: 1563703640076580127L);

            migrationBuilder.DeleteData(
                table: "vocabulary_senses",
                keyColumn: "id",
                keyValue: 1563703640076580128L);

            migrationBuilder.DeleteData(
                table: "vocabulary_senses",
                keyColumn: "id",
                keyValue: 1563703640076580129L);

            migrationBuilder.DeleteData(
                table: "vocabulary_senses",
                keyColumn: "id",
                keyValue: 1563703640076580130L);

            migrationBuilder.DeleteData(
                table: "vocabulary_senses",
                keyColumn: "id",
                keyValue: 1563703640076580131L);

            migrationBuilder.DeleteData(
                table: "vocabulary_senses",
                keyColumn: "id",
                keyValue: 1563703640076580132L);

            migrationBuilder.DeleteData(
                table: "vocabulary_senses",
                keyColumn: "id",
                keyValue: 1563703640076580133L);

            migrationBuilder.DeleteData(
                table: "vocabulary_senses",
                keyColumn: "id",
                keyValue: 1563703640076580134L);

            migrationBuilder.DeleteData(
                table: "vocabulary_senses",
                keyColumn: "id",
                keyValue: 1563703640076580135L);

            migrationBuilder.DeleteData(
                table: "vocabulary_senses",
                keyColumn: "id",
                keyValue: 1563703640076580136L);

            migrationBuilder.DeleteData(
                table: "vocabulary_senses",
                keyColumn: "id",
                keyValue: 1563703640076580137L);

            migrationBuilder.DeleteData(
                table: "vocabulary_senses",
                keyColumn: "id",
                keyValue: 1563703640076580138L);

            migrationBuilder.DeleteData(
                table: "vocabulary_senses",
                keyColumn: "id",
                keyValue: 1563703640076580139L);

            migrationBuilder.DeleteData(
                table: "vocabulary_senses",
                keyColumn: "id",
                keyValue: 1563703640076580140L);

            migrationBuilder.DeleteData(
                table: "vocabulary_senses",
                keyColumn: "id",
                keyValue: 1563703640076580141L);

            migrationBuilder.DeleteData(
                table: "vocabulary_senses",
                keyColumn: "id",
                keyValue: 1563703640076580142L);

            migrationBuilder.DeleteData(
                table: "vocabulary_senses",
                keyColumn: "id",
                keyValue: 1563703640076580143L);

            migrationBuilder.DeleteData(
                table: "vocabulary_senses",
                keyColumn: "id",
                keyValue: 1563703640076580144L);

            migrationBuilder.DeleteData(
                table: "vocabulary_senses",
                keyColumn: "id",
                keyValue: 1563703640076580145L);

            migrationBuilder.DeleteData(
                table: "vocabulary_senses",
                keyColumn: "id",
                keyValue: 1563703640076580146L);

            migrationBuilder.DeleteData(
                table: "vocabulary_senses",
                keyColumn: "id",
                keyValue: 1563703640076580147L);

            migrationBuilder.DeleteData(
                table: "vocabulary_senses",
                keyColumn: "id",
                keyValue: 1563703640076580148L);

            migrationBuilder.DeleteData(
                table: "vocabulary_senses",
                keyColumn: "id",
                keyValue: 1563703640076580149L);

            migrationBuilder.DeleteData(
                table: "vocabulary_senses",
                keyColumn: "id",
                keyValue: 1563703640076580150L);

            migrationBuilder.DeleteData(
                table: "vocabulary_senses",
                keyColumn: "id",
                keyValue: 1563703640076580151L);

            migrationBuilder.DeleteData(
                table: "vocabulary_senses",
                keyColumn: "id",
                keyValue: 1563703640076580152L);

            migrationBuilder.DeleteData(
                table: "vocabulary_senses",
                keyColumn: "id",
                keyValue: 1563703640076580153L);

            migrationBuilder.DeleteData(
                table: "vocabulary_senses",
                keyColumn: "id",
                keyValue: 1563703640076580154L);

            migrationBuilder.DeleteData(
                table: "vocabulary_senses",
                keyColumn: "id",
                keyValue: 1563703640076580155L);

            migrationBuilder.DeleteData(
                table: "vocabulary_senses",
                keyColumn: "id",
                keyValue: 1563703640076580156L);

            migrationBuilder.DeleteData(
                table: "vocabulary_senses",
                keyColumn: "id",
                keyValue: 1563703640076580157L);

            migrationBuilder.DeleteData(
                table: "vocabulary_senses",
                keyColumn: "id",
                keyValue: 1563703640076580158L);

            migrationBuilder.DeleteData(
                table: "vocabulary_senses",
                keyColumn: "id",
                keyValue: 1563703640076580159L);

            migrationBuilder.DeleteData(
                table: "vocabulary_senses",
                keyColumn: "id",
                keyValue: 1563703640076580160L);

            migrationBuilder.DeleteData(
                table: "vocabulary_senses",
                keyColumn: "id",
                keyValue: 1563703640076580161L);

            migrationBuilder.DeleteData(
                table: "vocabulary_senses",
                keyColumn: "id",
                keyValue: 1563703640076580162L);

            migrationBuilder.DeleteData(
                table: "vocabulary_senses",
                keyColumn: "id",
                keyValue: 1563703640076580163L);

            migrationBuilder.DeleteData(
                table: "vocabulary_senses",
                keyColumn: "id",
                keyValue: 1563703640076580164L);

            migrationBuilder.DeleteData(
                table: "vocabulary_senses",
                keyColumn: "id",
                keyValue: 1563703640076580165L);

            migrationBuilder.DeleteData(
                table: "vocabulary_senses",
                keyColumn: "id",
                keyValue: 1563703640076580166L);

            migrationBuilder.DeleteData(
                table: "vocabulary_senses",
                keyColumn: "id",
                keyValue: 1563703640076580167L);

            migrationBuilder.DeleteData(
                table: "vocabulary_senses",
                keyColumn: "id",
                keyValue: 1563703640076580168L);

            migrationBuilder.DeleteData(
                table: "vocabulary_senses",
                keyColumn: "id",
                keyValue: 1563703640076580169L);

            migrationBuilder.DeleteData(
                table: "vocabulary_senses",
                keyColumn: "id",
                keyValue: 1563703640076580170L);

            migrationBuilder.DeleteData(
                table: "vocabulary_senses",
                keyColumn: "id",
                keyValue: 1563703640076580171L);

            migrationBuilder.DeleteData(
                table: "vocabulary_senses",
                keyColumn: "id",
                keyValue: 1563703640076580172L);

            migrationBuilder.DeleteData(
                table: "vocabulary_senses",
                keyColumn: "id",
                keyValue: 1563703640076580173L);

            migrationBuilder.DeleteData(
                table: "vocabulary_senses",
                keyColumn: "id",
                keyValue: 1563703640076580174L);

            migrationBuilder.DeleteData(
                table: "vocabulary_senses",
                keyColumn: "id",
                keyValue: 1563703640076580175L);

            migrationBuilder.DeleteData(
                table: "vocabulary_senses",
                keyColumn: "id",
                keyValue: 1563703640076580176L);

            migrationBuilder.DeleteData(
                table: "vocabulary_senses",
                keyColumn: "id",
                keyValue: 1563703640076580177L);

            migrationBuilder.DeleteData(
                table: "vocabulary_senses",
                keyColumn: "id",
                keyValue: 1563703640076580178L);

            migrationBuilder.DeleteData(
                table: "vocabulary_senses",
                keyColumn: "id",
                keyValue: 1563703640076580179L);

            migrationBuilder.DeleteData(
                table: "vocabulary_senses",
                keyColumn: "id",
                keyValue: 1563703640076580180L);

            migrationBuilder.DeleteData(
                table: "vocabulary_senses",
                keyColumn: "id",
                keyValue: 1563703640076580181L);

            migrationBuilder.DeleteData(
                table: "vocabulary_senses",
                keyColumn: "id",
                keyValue: 1563703640076580182L);

            migrationBuilder.DeleteData(
                table: "vocabulary_senses",
                keyColumn: "id",
                keyValue: 1563703640076580183L);

            migrationBuilder.DeleteData(
                table: "vocabulary_senses",
                keyColumn: "id",
                keyValue: 1563703640076580184L);

            migrationBuilder.DeleteData(
                table: "vocabulary_senses",
                keyColumn: "id",
                keyValue: 1563703640076580185L);

            migrationBuilder.DeleteData(
                table: "vocabulary_senses",
                keyColumn: "id",
                keyValue: 1563703640076580186L);

            migrationBuilder.DeleteData(
                table: "vocabulary_senses",
                keyColumn: "id",
                keyValue: 1563703640076580187L);

            migrationBuilder.DeleteData(
                table: "vocabulary_senses",
                keyColumn: "id",
                keyValue: 1563703640076580188L);

            migrationBuilder.DeleteData(
                table: "vocabulary_senses",
                keyColumn: "id",
                keyValue: 1563703640076580189L);

            migrationBuilder.DeleteData(
                table: "vocabulary_senses",
                keyColumn: "id",
                keyValue: 1563703640076580190L);

            migrationBuilder.DeleteData(
                table: "vocabulary_senses",
                keyColumn: "id",
                keyValue: 1563703640076580191L);

            migrationBuilder.DeleteData(
                table: "vocabulary_senses",
                keyColumn: "id",
                keyValue: 1563703640076580192L);

            migrationBuilder.DeleteData(
                table: "vocabulary_senses",
                keyColumn: "id",
                keyValue: 1563703640076580193L);

            migrationBuilder.DeleteData(
                table: "vocabulary_senses",
                keyColumn: "id",
                keyValue: 1563703640076580194L);

            migrationBuilder.DeleteData(
                table: "vocabulary_senses",
                keyColumn: "id",
                keyValue: 1563703640076580195L);

            migrationBuilder.DeleteData(
                table: "vocabulary_senses",
                keyColumn: "id",
                keyValue: 1563703640076580196L);

            migrationBuilder.DeleteData(
                table: "vocabulary_senses",
                keyColumn: "id",
                keyValue: 1563703640076580197L);

            migrationBuilder.DeleteData(
                table: "vocabulary_senses",
                keyColumn: "id",
                keyValue: 1563703640076580198L);

            migrationBuilder.DeleteData(
                table: "vocabulary_senses",
                keyColumn: "id",
                keyValue: 1563703640076580199L);

            migrationBuilder.DeleteData(
                table: "vocabulary_senses",
                keyColumn: "id",
                keyValue: 1563703640076580200L);

            migrationBuilder.DeleteData(
                table: "vocabulary_senses",
                keyColumn: "id",
                keyValue: 1563703640076580201L);

            migrationBuilder.DeleteData(
                table: "vocabulary_senses",
                keyColumn: "id",
                keyValue: 1563703640076580202L);

            migrationBuilder.DeleteData(
                table: "vocabulary_senses",
                keyColumn: "id",
                keyValue: 1563703640076580203L);

            migrationBuilder.DeleteData(
                table: "vocabulary_senses",
                keyColumn: "id",
                keyValue: 1563703640076580204L);

            migrationBuilder.DeleteData(
                table: "vocabulary_senses",
                keyColumn: "id",
                keyValue: 1563703640076580205L);

            migrationBuilder.DeleteData(
                table: "vocabulary_senses",
                keyColumn: "id",
                keyValue: 1563703640076580206L);

            migrationBuilder.DeleteData(
                table: "vocabulary_senses",
                keyColumn: "id",
                keyValue: 1563703640076580207L);

            migrationBuilder.DeleteData(
                table: "vocabulary_senses",
                keyColumn: "id",
                keyValue: 1563703640076580208L);

            migrationBuilder.DeleteData(
                table: "vocabulary_senses",
                keyColumn: "id",
                keyValue: 1563703640076580209L);

            migrationBuilder.DeleteData(
                table: "vocabulary_senses",
                keyColumn: "id",
                keyValue: 1563703640076580210L);

            migrationBuilder.DeleteData(
                table: "vocabulary_senses",
                keyColumn: "id",
                keyValue: 1563703640076580211L);

            migrationBuilder.DeleteData(
                table: "vocabulary_senses",
                keyColumn: "id",
                keyValue: 1563703640076580212L);

            migrationBuilder.DeleteData(
                table: "vocabulary_senses",
                keyColumn: "id",
                keyValue: 1563703640076580213L);

            migrationBuilder.DeleteData(
                table: "vocabulary_senses",
                keyColumn: "id",
                keyValue: 1563703640076580214L);

            migrationBuilder.DeleteData(
                table: "vocabulary_senses",
                keyColumn: "id",
                keyValue: 1563703640076580215L);

            migrationBuilder.DeleteData(
                table: "vocabulary_senses",
                keyColumn: "id",
                keyValue: 1563703640076580216L);

            migrationBuilder.DeleteData(
                table: "vocabulary_senses",
                keyColumn: "id",
                keyValue: 1563703640076580217L);

            migrationBuilder.DeleteData(
                table: "vocabulary_senses",
                keyColumn: "id",
                keyValue: 1563703640076580218L);

            migrationBuilder.DeleteData(
                table: "vocabulary_senses",
                keyColumn: "id",
                keyValue: 1563703640076580219L);

            migrationBuilder.DeleteData(
                table: "vocabulary_senses",
                keyColumn: "id",
                keyValue: 1563703640076580220L);

            migrationBuilder.DeleteData(
                table: "vocabulary_senses",
                keyColumn: "id",
                keyValue: 1563703640076580221L);

            migrationBuilder.DeleteData(
                table: "vocabulary_senses",
                keyColumn: "id",
                keyValue: 1563703640076580222L);

            migrationBuilder.DeleteData(
                table: "vocabulary_senses",
                keyColumn: "id",
                keyValue: 1563703640076580223L);

            migrationBuilder.DeleteData(
                table: "vocabulary_senses",
                keyColumn: "id",
                keyValue: 1563703640076580224L);

            migrationBuilder.DeleteData(
                table: "vocabulary_senses",
                keyColumn: "id",
                keyValue: 1563703640076580225L);

            migrationBuilder.DeleteData(
                table: "vocabulary_senses",
                keyColumn: "id",
                keyValue: 1563703640076580226L);

            migrationBuilder.DeleteData(
                table: "vocabulary_senses",
                keyColumn: "id",
                keyValue: 1563703640076580227L);

            migrationBuilder.DeleteData(
                table: "vocabulary_senses",
                keyColumn: "id",
                keyValue: 1563703640076580228L);

            migrationBuilder.DeleteData(
                table: "vocabulary_senses",
                keyColumn: "id",
                keyValue: 1563703640076580229L);

            migrationBuilder.DeleteData(
                table: "vocabulary_senses",
                keyColumn: "id",
                keyValue: 1563703640076580230L);

            migrationBuilder.DeleteData(
                table: "vocabulary_senses",
                keyColumn: "id",
                keyValue: 1563703640076580231L);

            migrationBuilder.DeleteData(
                table: "vocabulary_senses",
                keyColumn: "id",
                keyValue: 1563703640076580232L);

            migrationBuilder.DeleteData(
                table: "vocabulary_senses",
                keyColumn: "id",
                keyValue: 1563703640076580233L);

            migrationBuilder.DeleteData(
                table: "vocabulary_senses",
                keyColumn: "id",
                keyValue: 1563703640076580234L);

            migrationBuilder.DeleteData(
                table: "vocabulary_senses",
                keyColumn: "id",
                keyValue: 1563703640076580235L);

            migrationBuilder.DeleteData(
                table: "vocabulary_senses",
                keyColumn: "id",
                keyValue: 1563703640076580236L);

            migrationBuilder.DeleteData(
                table: "vocabulary_senses",
                keyColumn: "id",
                keyValue: 1563703640076580237L);

            migrationBuilder.DeleteData(
                table: "vocabulary_senses",
                keyColumn: "id",
                keyValue: 1563703640076580238L);

            migrationBuilder.DeleteData(
                table: "vocabulary_senses",
                keyColumn: "id",
                keyValue: 1563703640076580239L);

            migrationBuilder.DeleteData(
                table: "vocabulary_senses",
                keyColumn: "id",
                keyValue: 1563703640076580240L);

            migrationBuilder.DeleteData(
                table: "vocabulary_senses",
                keyColumn: "id",
                keyValue: 1563703640076580241L);

            migrationBuilder.DeleteData(
                table: "vocabulary_senses",
                keyColumn: "id",
                keyValue: 1563703640076580242L);

            migrationBuilder.DeleteData(
                table: "vocabulary_senses",
                keyColumn: "id",
                keyValue: 1563703640076580243L);

            migrationBuilder.DeleteData(
                table: "vocabulary_senses",
                keyColumn: "id",
                keyValue: 1563703640076580244L);

            migrationBuilder.DeleteData(
                table: "vocabulary_senses",
                keyColumn: "id",
                keyValue: 1563703640076580245L);

            migrationBuilder.DeleteData(
                table: "vocabulary_senses",
                keyColumn: "id",
                keyValue: 1563703640076580246L);

            migrationBuilder.DeleteData(
                table: "vocabulary_senses",
                keyColumn: "id",
                keyValue: 1563703640076580247L);

            migrationBuilder.DeleteData(
                table: "vocabulary_senses",
                keyColumn: "id",
                keyValue: 1563703640076580248L);

            migrationBuilder.DeleteData(
                table: "vocabulary_senses",
                keyColumn: "id",
                keyValue: 1563703640076580249L);

            migrationBuilder.DeleteData(
                table: "vocabulary_senses",
                keyColumn: "id",
                keyValue: 1563703640076580250L);

            migrationBuilder.DeleteData(
                table: "vocabulary_senses",
                keyColumn: "id",
                keyValue: 1563703640076580251L);

            migrationBuilder.DeleteData(
                table: "vocabulary_senses",
                keyColumn: "id",
                keyValue: 1563703640076580252L);

            migrationBuilder.DeleteData(
                table: "vocabulary_senses",
                keyColumn: "id",
                keyValue: 1563703640076580253L);

            migrationBuilder.DeleteData(
                table: "vocabulary_senses",
                keyColumn: "id",
                keyValue: 1563703640076580254L);

            migrationBuilder.DeleteData(
                table: "vocabulary_senses",
                keyColumn: "id",
                keyValue: 1563703640076580255L);

            migrationBuilder.DeleteData(
                table: "vocabulary_senses",
                keyColumn: "id",
                keyValue: 1563703640076580256L);

            migrationBuilder.DeleteData(
                table: "vocabulary_senses",
                keyColumn: "id",
                keyValue: 1563703640076580257L);

            migrationBuilder.DeleteData(
                table: "vocabulary_senses",
                keyColumn: "id",
                keyValue: 1563703640076580258L);

            migrationBuilder.DeleteData(
                table: "vocabulary_senses",
                keyColumn: "id",
                keyValue: 1563703640076580259L);

            migrationBuilder.DeleteData(
                table: "vocabulary_senses",
                keyColumn: "id",
                keyValue: 1563703640076580260L);

            migrationBuilder.DeleteData(
                table: "vocabulary_senses",
                keyColumn: "id",
                keyValue: 1563703640076580261L);

            migrationBuilder.DeleteData(
                table: "vocabulary_senses",
                keyColumn: "id",
                keyValue: 1563703640076580262L);

            migrationBuilder.DeleteData(
                table: "vocabulary_senses",
                keyColumn: "id",
                keyValue: 1563703640076580263L);

            migrationBuilder.DeleteData(
                table: "vocabulary_senses",
                keyColumn: "id",
                keyValue: 1563703640076580264L);

            migrationBuilder.DeleteData(
                table: "vocabulary_senses",
                keyColumn: "id",
                keyValue: 1563703640076580265L);

            migrationBuilder.DeleteData(
                table: "vocabulary_senses",
                keyColumn: "id",
                keyValue: 1563703640076580266L);

            migrationBuilder.DeleteData(
                table: "vocabulary_senses",
                keyColumn: "id",
                keyValue: 1563703640076580267L);

            migrationBuilder.DeleteData(
                table: "vocabulary_senses",
                keyColumn: "id",
                keyValue: 1563703640076580268L);

            migrationBuilder.DeleteData(
                table: "vocabulary_senses",
                keyColumn: "id",
                keyValue: 1563703640076580269L);

            migrationBuilder.DeleteData(
                table: "vocabulary_senses",
                keyColumn: "id",
                keyValue: 1563703640076580270L);

            migrationBuilder.DeleteData(
                table: "vocabulary_senses",
                keyColumn: "id",
                keyValue: 1563703640076580271L);

            migrationBuilder.DeleteData(
                table: "vocabulary_senses",
                keyColumn: "id",
                keyValue: 1563703640076580272L);

            migrationBuilder.DeleteData(
                table: "vocabulary_senses",
                keyColumn: "id",
                keyValue: 1563703640076580273L);

            migrationBuilder.DeleteData(
                table: "vocabulary_senses",
                keyColumn: "id",
                keyValue: 1563703640076580274L);

            migrationBuilder.DeleteData(
                table: "vocabulary_senses",
                keyColumn: "id",
                keyValue: 1563703640076580275L);

            migrationBuilder.DeleteData(
                table: "vocabulary_senses",
                keyColumn: "id",
                keyValue: 1563703640076580276L);

            migrationBuilder.DeleteData(
                table: "vocabulary_senses",
                keyColumn: "id",
                keyValue: 1563703640076580277L);

            migrationBuilder.DeleteData(
                table: "vocabulary_senses",
                keyColumn: "id",
                keyValue: 1563703640076580278L);

            migrationBuilder.DeleteData(
                table: "vocabulary_senses",
                keyColumn: "id",
                keyValue: 1563703640076580279L);

            migrationBuilder.DeleteData(
                table: "vocabulary_senses",
                keyColumn: "id",
                keyValue: 1563703640076580280L);

            migrationBuilder.DeleteData(
                table: "vocabulary_senses",
                keyColumn: "id",
                keyValue: 1563703640076580281L);

            migrationBuilder.DeleteData(
                table: "vocabulary_senses",
                keyColumn: "id",
                keyValue: 1563703640076580282L);

            migrationBuilder.DeleteData(
                table: "vocabulary_senses",
                keyColumn: "id",
                keyValue: 1563703640076580283L);

            migrationBuilder.DeleteData(
                table: "vocabulary_senses",
                keyColumn: "id",
                keyValue: 1563703640076580284L);

            migrationBuilder.DeleteData(
                table: "vocabulary_senses",
                keyColumn: "id",
                keyValue: 1563703640076580285L);

            migrationBuilder.DeleteData(
                table: "vocabulary_senses",
                keyColumn: "id",
                keyValue: 1563703640076580286L);

            migrationBuilder.DeleteData(
                table: "vocabulary_senses",
                keyColumn: "id",
                keyValue: 1563703640076580287L);

            migrationBuilder.DeleteData(
                table: "vocabulary_senses",
                keyColumn: "id",
                keyValue: 1563703640076580288L);

            migrationBuilder.DeleteData(
                table: "vocabulary_senses",
                keyColumn: "id",
                keyValue: 1563703640076580289L);

            migrationBuilder.DeleteData(
                table: "vocabulary_senses",
                keyColumn: "id",
                keyValue: 1563703640076580290L);

            migrationBuilder.DeleteData(
                table: "vocabulary_senses",
                keyColumn: "id",
                keyValue: 1563703640076580291L);

            migrationBuilder.DeleteData(
                table: "vocabulary_senses",
                keyColumn: "id",
                keyValue: 1563703640076580292L);

            migrationBuilder.DeleteData(
                table: "vocabulary_senses",
                keyColumn: "id",
                keyValue: 1563703640076580293L);

            migrationBuilder.DeleteData(
                table: "vocabulary_senses",
                keyColumn: "id",
                keyValue: 1563703640076580294L);

            migrationBuilder.DeleteData(
                table: "vocabulary_senses",
                keyColumn: "id",
                keyValue: 1563703640076580295L);

            migrationBuilder.DeleteData(
                table: "vocabulary_senses",
                keyColumn: "id",
                keyValue: 1563703640076580296L);

            migrationBuilder.DeleteData(
                table: "vocabulary_senses",
                keyColumn: "id",
                keyValue: 1563703640076580297L);

            migrationBuilder.DeleteData(
                table: "vocabulary_senses",
                keyColumn: "id",
                keyValue: 1563703640076580298L);

            migrationBuilder.DeleteData(
                table: "vocabulary_senses",
                keyColumn: "id",
                keyValue: 1563703640076580299L);

            migrationBuilder.DeleteData(
                table: "vocabulary_senses",
                keyColumn: "id",
                keyValue: 1563703640076580300L);

            migrationBuilder.DeleteData(
                table: "vocabulary_senses",
                keyColumn: "id",
                keyValue: 1563703640076580301L);

            migrationBuilder.DeleteData(
                table: "vocabulary_senses",
                keyColumn: "id",
                keyValue: 1563703640076580302L);

            migrationBuilder.DeleteData(
                table: "vocabulary_senses",
                keyColumn: "id",
                keyValue: 1563703640076580303L);

            migrationBuilder.DeleteData(
                table: "vocabulary_senses",
                keyColumn: "id",
                keyValue: 1563703640076580304L);

            migrationBuilder.DeleteData(
                table: "vocabulary_senses",
                keyColumn: "id",
                keyValue: 1563703640076580305L);

            migrationBuilder.DeleteData(
                table: "vocabulary_senses",
                keyColumn: "id",
                keyValue: 1563703640076580306L);

            migrationBuilder.DeleteData(
                table: "vocabulary_senses",
                keyColumn: "id",
                keyValue: 1563703640076580307L);

            migrationBuilder.DeleteData(
                table: "vocabulary_senses",
                keyColumn: "id",
                keyValue: 1563703640076580308L);

            migrationBuilder.DeleteData(
                table: "vocabulary_senses",
                keyColumn: "id",
                keyValue: 1563703640076580309L);

            migrationBuilder.DeleteData(
                table: "vocabulary_senses",
                keyColumn: "id",
                keyValue: 1563703640076580310L);

            migrationBuilder.DeleteData(
                table: "vocabulary_senses",
                keyColumn: "id",
                keyValue: 1563703640076580311L);

            migrationBuilder.DeleteData(
                table: "vocabulary_senses",
                keyColumn: "id",
                keyValue: 1563703640076580312L);

            migrationBuilder.DeleteData(
                table: "vocabulary_senses",
                keyColumn: "id",
                keyValue: 1563703640076580313L);

            migrationBuilder.DeleteData(
                table: "vocabulary_senses",
                keyColumn: "id",
                keyValue: 1563703640076580314L);

            migrationBuilder.DeleteData(
                table: "vocabulary_senses",
                keyColumn: "id",
                keyValue: 1563703640076580315L);

            migrationBuilder.DeleteData(
                table: "writing_system_items",
                keyColumn: "id",
                keyValue: 1563703640076578916L);

            migrationBuilder.DeleteData(
                table: "writing_system_items",
                keyColumn: "id",
                keyValue: 1563703640076578917L);

            migrationBuilder.DeleteData(
                table: "writing_system_items",
                keyColumn: "id",
                keyValue: 1563703640076578918L);

            migrationBuilder.DeleteData(
                table: "writing_system_items",
                keyColumn: "id",
                keyValue: 1563703640076578919L);

            migrationBuilder.DeleteData(
                table: "writing_system_items",
                keyColumn: "id",
                keyValue: 1563703640076578920L);

            migrationBuilder.DeleteData(
                table: "writing_system_items",
                keyColumn: "id",
                keyValue: 1563703640076578921L);

            migrationBuilder.DeleteData(
                table: "writing_system_items",
                keyColumn: "id",
                keyValue: 1563703640076578922L);

            migrationBuilder.DeleteData(
                table: "writing_system_items",
                keyColumn: "id",
                keyValue: 1563703640076578923L);

            migrationBuilder.DeleteData(
                table: "writing_system_items",
                keyColumn: "id",
                keyValue: 1563703640076578924L);

            migrationBuilder.DeleteData(
                table: "writing_system_items",
                keyColumn: "id",
                keyValue: 1563703640076578925L);

            migrationBuilder.DeleteData(
                table: "writing_system_items",
                keyColumn: "id",
                keyValue: 1563703640076578926L);

            migrationBuilder.DeleteData(
                table: "writing_system_items",
                keyColumn: "id",
                keyValue: 1563703640076578927L);

            migrationBuilder.DeleteData(
                table: "writing_system_items",
                keyColumn: "id",
                keyValue: 1563703640076578928L);

            migrationBuilder.DeleteData(
                table: "writing_system_items",
                keyColumn: "id",
                keyValue: 1563703640076578929L);

            migrationBuilder.DeleteData(
                table: "writing_system_items",
                keyColumn: "id",
                keyValue: 1563703640076578930L);

            migrationBuilder.DeleteData(
                table: "writing_system_items",
                keyColumn: "id",
                keyValue: 1563703640076578931L);

            migrationBuilder.DeleteData(
                table: "writing_system_items",
                keyColumn: "id",
                keyValue: 1563703640076578932L);

            migrationBuilder.DeleteData(
                table: "writing_system_items",
                keyColumn: "id",
                keyValue: 1563703640076578933L);

            migrationBuilder.DeleteData(
                table: "writing_system_items",
                keyColumn: "id",
                keyValue: 1563703640076578934L);

            migrationBuilder.DeleteData(
                table: "writing_system_items",
                keyColumn: "id",
                keyValue: 1563703640076578935L);

            migrationBuilder.DeleteData(
                table: "writing_system_items",
                keyColumn: "id",
                keyValue: 1563703640076578936L);

            migrationBuilder.DeleteData(
                table: "writing_system_items",
                keyColumn: "id",
                keyValue: 1563703640076578937L);

            migrationBuilder.DeleteData(
                table: "writing_system_items",
                keyColumn: "id",
                keyValue: 1563703640076578938L);

            migrationBuilder.DeleteData(
                table: "writing_system_items",
                keyColumn: "id",
                keyValue: 1563703640076578939L);

            migrationBuilder.DeleteData(
                table: "writing_system_items",
                keyColumn: "id",
                keyValue: 1563703640076578940L);

            migrationBuilder.DeleteData(
                table: "writing_system_items",
                keyColumn: "id",
                keyValue: 1563703640076578941L);

            migrationBuilder.DeleteData(
                table: "writing_system_items",
                keyColumn: "id",
                keyValue: 1563703640076578942L);

            migrationBuilder.DeleteData(
                table: "writing_system_items",
                keyColumn: "id",
                keyValue: 1563703640076578943L);

            migrationBuilder.DeleteData(
                table: "writing_system_items",
                keyColumn: "id",
                keyValue: 1563703640076578944L);

            migrationBuilder.DeleteData(
                table: "writing_system_items",
                keyColumn: "id",
                keyValue: 1563703640076578945L);

            migrationBuilder.DeleteData(
                table: "writing_system_items",
                keyColumn: "id",
                keyValue: 1563703640076578946L);

            migrationBuilder.DeleteData(
                table: "writing_system_items",
                keyColumn: "id",
                keyValue: 1563703640076578947L);

            migrationBuilder.DeleteData(
                table: "writing_system_items",
                keyColumn: "id",
                keyValue: 1563703640076578948L);

            migrationBuilder.DeleteData(
                table: "writing_system_items",
                keyColumn: "id",
                keyValue: 1563703640076578949L);

            migrationBuilder.DeleteData(
                table: "writing_system_items",
                keyColumn: "id",
                keyValue: 1563703640076578950L);

            migrationBuilder.DeleteData(
                table: "writing_system_items",
                keyColumn: "id",
                keyValue: 1563703640076578951L);

            migrationBuilder.DeleteData(
                table: "writing_system_items",
                keyColumn: "id",
                keyValue: 1563703640076578952L);

            migrationBuilder.DeleteData(
                table: "writing_system_items",
                keyColumn: "id",
                keyValue: 1563703640076578953L);

            migrationBuilder.DeleteData(
                table: "writing_system_items",
                keyColumn: "id",
                keyValue: 1563703640076578954L);

            migrationBuilder.DeleteData(
                table: "writing_system_items",
                keyColumn: "id",
                keyValue: 1563703640076578955L);

            migrationBuilder.DeleteData(
                table: "writing_system_items",
                keyColumn: "id",
                keyValue: 1563703640076578956L);

            migrationBuilder.DeleteData(
                table: "writing_system_items",
                keyColumn: "id",
                keyValue: 1563703640076578957L);

            migrationBuilder.DeleteData(
                table: "writing_system_items",
                keyColumn: "id",
                keyValue: 1563703640076578958L);

            migrationBuilder.DeleteData(
                table: "writing_system_items",
                keyColumn: "id",
                keyValue: 1563703640076578959L);

            migrationBuilder.DeleteData(
                table: "writing_system_items",
                keyColumn: "id",
                keyValue: 1563703640076578960L);

            migrationBuilder.DeleteData(
                table: "writing_system_items",
                keyColumn: "id",
                keyValue: 1563703640076578961L);

            migrationBuilder.DeleteData(
                table: "writing_system_items",
                keyColumn: "id",
                keyValue: 1563703640076578962L);

            migrationBuilder.DeleteData(
                table: "writing_system_items",
                keyColumn: "id",
                keyValue: 1563703640076578963L);

            migrationBuilder.DeleteData(
                table: "writing_system_items",
                keyColumn: "id",
                keyValue: 1563703640076578964L);

            migrationBuilder.DeleteData(
                table: "writing_system_items",
                keyColumn: "id",
                keyValue: 1563703640076578965L);

            migrationBuilder.DeleteData(
                table: "writing_system_items",
                keyColumn: "id",
                keyValue: 1563703640076578966L);

            migrationBuilder.DeleteData(
                table: "writing_system_items",
                keyColumn: "id",
                keyValue: 1563703640076578967L);

            migrationBuilder.DeleteData(
                table: "writing_system_items",
                keyColumn: "id",
                keyValue: 1563703640076578968L);

            migrationBuilder.DeleteData(
                table: "writing_system_items",
                keyColumn: "id",
                keyValue: 1563703640076578969L);

            migrationBuilder.DeleteData(
                table: "writing_system_items",
                keyColumn: "id",
                keyValue: 1563703640076578970L);

            migrationBuilder.DeleteData(
                table: "writing_system_items",
                keyColumn: "id",
                keyValue: 1563703640076578971L);

            migrationBuilder.DeleteData(
                table: "writing_system_items",
                keyColumn: "id",
                keyValue: 1563703640076578972L);

            migrationBuilder.DeleteData(
                table: "writing_system_items",
                keyColumn: "id",
                keyValue: 1563703640076578973L);

            migrationBuilder.DeleteData(
                table: "writing_system_items",
                keyColumn: "id",
                keyValue: 1563703640076578974L);

            migrationBuilder.DeleteData(
                table: "writing_system_items",
                keyColumn: "id",
                keyValue: 1563703640076578975L);

            migrationBuilder.DeleteData(
                table: "vocabulary_entries",
                keyColumn: "id",
                keyValue: 1563703640076579816L);

            migrationBuilder.DeleteData(
                table: "vocabulary_entries",
                keyColumn: "id",
                keyValue: 1563703640076579817L);

            migrationBuilder.DeleteData(
                table: "vocabulary_entries",
                keyColumn: "id",
                keyValue: 1563703640076579818L);

            migrationBuilder.DeleteData(
                table: "vocabulary_entries",
                keyColumn: "id",
                keyValue: 1563703640076579819L);

            migrationBuilder.DeleteData(
                table: "vocabulary_entries",
                keyColumn: "id",
                keyValue: 1563703640076579820L);

            migrationBuilder.DeleteData(
                table: "vocabulary_entries",
                keyColumn: "id",
                keyValue: 1563703640076579821L);

            migrationBuilder.DeleteData(
                table: "vocabulary_entries",
                keyColumn: "id",
                keyValue: 1563703640076579822L);

            migrationBuilder.DeleteData(
                table: "vocabulary_entries",
                keyColumn: "id",
                keyValue: 1563703640076579823L);

            migrationBuilder.DeleteData(
                table: "vocabulary_entries",
                keyColumn: "id",
                keyValue: 1563703640076579824L);

            migrationBuilder.DeleteData(
                table: "vocabulary_entries",
                keyColumn: "id",
                keyValue: 1563703640076579825L);

            migrationBuilder.DeleteData(
                table: "vocabulary_entries",
                keyColumn: "id",
                keyValue: 1563703640076579826L);

            migrationBuilder.DeleteData(
                table: "vocabulary_entries",
                keyColumn: "id",
                keyValue: 1563703640076579827L);

            migrationBuilder.DeleteData(
                table: "vocabulary_entries",
                keyColumn: "id",
                keyValue: 1563703640076579828L);

            migrationBuilder.DeleteData(
                table: "vocabulary_entries",
                keyColumn: "id",
                keyValue: 1563703640076579829L);

            migrationBuilder.DeleteData(
                table: "vocabulary_entries",
                keyColumn: "id",
                keyValue: 1563703640076579830L);

            migrationBuilder.DeleteData(
                table: "vocabulary_entries",
                keyColumn: "id",
                keyValue: 1563703640076579831L);

            migrationBuilder.DeleteData(
                table: "vocabulary_entries",
                keyColumn: "id",
                keyValue: 1563703640076579832L);

            migrationBuilder.DeleteData(
                table: "vocabulary_entries",
                keyColumn: "id",
                keyValue: 1563703640076579833L);

            migrationBuilder.DeleteData(
                table: "vocabulary_entries",
                keyColumn: "id",
                keyValue: 1563703640076579834L);

            migrationBuilder.DeleteData(
                table: "vocabulary_entries",
                keyColumn: "id",
                keyValue: 1563703640076579835L);

            migrationBuilder.DeleteData(
                table: "vocabulary_entries",
                keyColumn: "id",
                keyValue: 1563703640076579836L);

            migrationBuilder.DeleteData(
                table: "vocabulary_entries",
                keyColumn: "id",
                keyValue: 1563703640076579837L);

            migrationBuilder.DeleteData(
                table: "vocabulary_entries",
                keyColumn: "id",
                keyValue: 1563703640076579838L);

            migrationBuilder.DeleteData(
                table: "vocabulary_entries",
                keyColumn: "id",
                keyValue: 1563703640076579839L);

            migrationBuilder.DeleteData(
                table: "vocabulary_entries",
                keyColumn: "id",
                keyValue: 1563703640076579840L);

            migrationBuilder.DeleteData(
                table: "vocabulary_entries",
                keyColumn: "id",
                keyValue: 1563703640076579841L);

            migrationBuilder.DeleteData(
                table: "vocabulary_entries",
                keyColumn: "id",
                keyValue: 1563703640076579842L);

            migrationBuilder.DeleteData(
                table: "vocabulary_entries",
                keyColumn: "id",
                keyValue: 1563703640076579843L);

            migrationBuilder.DeleteData(
                table: "vocabulary_entries",
                keyColumn: "id",
                keyValue: 1563703640076579844L);

            migrationBuilder.DeleteData(
                table: "vocabulary_entries",
                keyColumn: "id",
                keyValue: 1563703640076579845L);

            migrationBuilder.DeleteData(
                table: "vocabulary_entries",
                keyColumn: "id",
                keyValue: 1563703640076579846L);

            migrationBuilder.DeleteData(
                table: "vocabulary_entries",
                keyColumn: "id",
                keyValue: 1563703640076579847L);

            migrationBuilder.DeleteData(
                table: "vocabulary_entries",
                keyColumn: "id",
                keyValue: 1563703640076579848L);

            migrationBuilder.DeleteData(
                table: "vocabulary_entries",
                keyColumn: "id",
                keyValue: 1563703640076579849L);

            migrationBuilder.DeleteData(
                table: "vocabulary_entries",
                keyColumn: "id",
                keyValue: 1563703640076579850L);

            migrationBuilder.DeleteData(
                table: "vocabulary_entries",
                keyColumn: "id",
                keyValue: 1563703640076579851L);

            migrationBuilder.DeleteData(
                table: "vocabulary_entries",
                keyColumn: "id",
                keyValue: 1563703640076579852L);

            migrationBuilder.DeleteData(
                table: "vocabulary_entries",
                keyColumn: "id",
                keyValue: 1563703640076579853L);

            migrationBuilder.DeleteData(
                table: "vocabulary_entries",
                keyColumn: "id",
                keyValue: 1563703640076579854L);

            migrationBuilder.DeleteData(
                table: "vocabulary_entries",
                keyColumn: "id",
                keyValue: 1563703640076579855L);

            migrationBuilder.DeleteData(
                table: "vocabulary_entries",
                keyColumn: "id",
                keyValue: 1563703640076579856L);

            migrationBuilder.DeleteData(
                table: "vocabulary_entries",
                keyColumn: "id",
                keyValue: 1563703640076579857L);

            migrationBuilder.DeleteData(
                table: "vocabulary_entries",
                keyColumn: "id",
                keyValue: 1563703640076579858L);

            migrationBuilder.DeleteData(
                table: "vocabulary_entries",
                keyColumn: "id",
                keyValue: 1563703640076579859L);

            migrationBuilder.DeleteData(
                table: "vocabulary_entries",
                keyColumn: "id",
                keyValue: 1563703640076579860L);

            migrationBuilder.DeleteData(
                table: "vocabulary_entries",
                keyColumn: "id",
                keyValue: 1563703640076579861L);

            migrationBuilder.DeleteData(
                table: "vocabulary_entries",
                keyColumn: "id",
                keyValue: 1563703640076579862L);

            migrationBuilder.DeleteData(
                table: "vocabulary_entries",
                keyColumn: "id",
                keyValue: 1563703640076579863L);

            migrationBuilder.DeleteData(
                table: "vocabulary_entries",
                keyColumn: "id",
                keyValue: 1563703640076579864L);

            migrationBuilder.DeleteData(
                table: "vocabulary_entries",
                keyColumn: "id",
                keyValue: 1563703640076579865L);

            migrationBuilder.DeleteData(
                table: "vocabulary_entries",
                keyColumn: "id",
                keyValue: 1563703640076579866L);

            migrationBuilder.DeleteData(
                table: "vocabulary_entries",
                keyColumn: "id",
                keyValue: 1563703640076579867L);

            migrationBuilder.DeleteData(
                table: "vocabulary_entries",
                keyColumn: "id",
                keyValue: 1563703640076579868L);

            migrationBuilder.DeleteData(
                table: "vocabulary_entries",
                keyColumn: "id",
                keyValue: 1563703640076579869L);

            migrationBuilder.DeleteData(
                table: "vocabulary_entries",
                keyColumn: "id",
                keyValue: 1563703640076579870L);

            migrationBuilder.DeleteData(
                table: "vocabulary_entries",
                keyColumn: "id",
                keyValue: 1563703640076579871L);

            migrationBuilder.DeleteData(
                table: "vocabulary_entries",
                keyColumn: "id",
                keyValue: 1563703640076579872L);

            migrationBuilder.DeleteData(
                table: "vocabulary_entries",
                keyColumn: "id",
                keyValue: 1563703640076579873L);

            migrationBuilder.DeleteData(
                table: "vocabulary_entries",
                keyColumn: "id",
                keyValue: 1563703640076579874L);

            migrationBuilder.DeleteData(
                table: "vocabulary_entries",
                keyColumn: "id",
                keyValue: 1563703640076579875L);

            migrationBuilder.DeleteData(
                table: "vocabulary_entries",
                keyColumn: "id",
                keyValue: 1563703640076579876L);

            migrationBuilder.DeleteData(
                table: "vocabulary_entries",
                keyColumn: "id",
                keyValue: 1563703640076579877L);

            migrationBuilder.DeleteData(
                table: "vocabulary_entries",
                keyColumn: "id",
                keyValue: 1563703640076579878L);

            migrationBuilder.DeleteData(
                table: "vocabulary_entries",
                keyColumn: "id",
                keyValue: 1563703640076579879L);

            migrationBuilder.DeleteData(
                table: "vocabulary_entries",
                keyColumn: "id",
                keyValue: 1563703640076579880L);

            migrationBuilder.DeleteData(
                table: "vocabulary_entries",
                keyColumn: "id",
                keyValue: 1563703640076579881L);

            migrationBuilder.DeleteData(
                table: "vocabulary_entries",
                keyColumn: "id",
                keyValue: 1563703640076579882L);

            migrationBuilder.DeleteData(
                table: "vocabulary_entries",
                keyColumn: "id",
                keyValue: 1563703640076579883L);

            migrationBuilder.DeleteData(
                table: "vocabulary_entries",
                keyColumn: "id",
                keyValue: 1563703640076579884L);

            migrationBuilder.DeleteData(
                table: "vocabulary_entries",
                keyColumn: "id",
                keyValue: 1563703640076579885L);

            migrationBuilder.DeleteData(
                table: "vocabulary_entries",
                keyColumn: "id",
                keyValue: 1563703640076579886L);

            migrationBuilder.DeleteData(
                table: "vocabulary_entries",
                keyColumn: "id",
                keyValue: 1563703640076579887L);

            migrationBuilder.DeleteData(
                table: "vocabulary_entries",
                keyColumn: "id",
                keyValue: 1563703640076579888L);

            migrationBuilder.DeleteData(
                table: "vocabulary_entries",
                keyColumn: "id",
                keyValue: 1563703640076579889L);

            migrationBuilder.DeleteData(
                table: "vocabulary_entries",
                keyColumn: "id",
                keyValue: 1563703640076579890L);

            migrationBuilder.DeleteData(
                table: "vocabulary_entries",
                keyColumn: "id",
                keyValue: 1563703640076579891L);

            migrationBuilder.DeleteData(
                table: "vocabulary_entries",
                keyColumn: "id",
                keyValue: 1563703640076579892L);

            migrationBuilder.DeleteData(
                table: "vocabulary_entries",
                keyColumn: "id",
                keyValue: 1563703640076579893L);

            migrationBuilder.DeleteData(
                table: "vocabulary_entries",
                keyColumn: "id",
                keyValue: 1563703640076579894L);

            migrationBuilder.DeleteData(
                table: "vocabulary_entries",
                keyColumn: "id",
                keyValue: 1563703640076579895L);

            migrationBuilder.DeleteData(
                table: "vocabulary_entries",
                keyColumn: "id",
                keyValue: 1563703640076579896L);

            migrationBuilder.DeleteData(
                table: "vocabulary_entries",
                keyColumn: "id",
                keyValue: 1563703640076579897L);

            migrationBuilder.DeleteData(
                table: "vocabulary_entries",
                keyColumn: "id",
                keyValue: 1563703640076579898L);

            migrationBuilder.DeleteData(
                table: "vocabulary_entries",
                keyColumn: "id",
                keyValue: 1563703640076579899L);

            migrationBuilder.DeleteData(
                table: "vocabulary_entries",
                keyColumn: "id",
                keyValue: 1563703640076579900L);

            migrationBuilder.DeleteData(
                table: "vocabulary_entries",
                keyColumn: "id",
                keyValue: 1563703640076579901L);

            migrationBuilder.DeleteData(
                table: "vocabulary_entries",
                keyColumn: "id",
                keyValue: 1563703640076579902L);

            migrationBuilder.DeleteData(
                table: "vocabulary_entries",
                keyColumn: "id",
                keyValue: 1563703640076579903L);

            migrationBuilder.DeleteData(
                table: "vocabulary_entries",
                keyColumn: "id",
                keyValue: 1563703640076579904L);

            migrationBuilder.DeleteData(
                table: "vocabulary_entries",
                keyColumn: "id",
                keyValue: 1563703640076579905L);

            migrationBuilder.DeleteData(
                table: "vocabulary_entries",
                keyColumn: "id",
                keyValue: 1563703640076579906L);

            migrationBuilder.DeleteData(
                table: "vocabulary_entries",
                keyColumn: "id",
                keyValue: 1563703640076579907L);

            migrationBuilder.DeleteData(
                table: "vocabulary_entries",
                keyColumn: "id",
                keyValue: 1563703640076579908L);

            migrationBuilder.DeleteData(
                table: "vocabulary_entries",
                keyColumn: "id",
                keyValue: 1563703640076579909L);

            migrationBuilder.DeleteData(
                table: "vocabulary_entries",
                keyColumn: "id",
                keyValue: 1563703640076579910L);

            migrationBuilder.DeleteData(
                table: "vocabulary_entries",
                keyColumn: "id",
                keyValue: 1563703640076579911L);

            migrationBuilder.DeleteData(
                table: "vocabulary_entries",
                keyColumn: "id",
                keyValue: 1563703640076579912L);

            migrationBuilder.DeleteData(
                table: "vocabulary_entries",
                keyColumn: "id",
                keyValue: 1563703640076579913L);

            migrationBuilder.DeleteData(
                table: "vocabulary_entries",
                keyColumn: "id",
                keyValue: 1563703640076579914L);

            migrationBuilder.DeleteData(
                table: "vocabulary_entries",
                keyColumn: "id",
                keyValue: 1563703640076579915L);

            migrationBuilder.DeleteData(
                table: "vocabulary_entries",
                keyColumn: "id",
                keyValue: 1563703640076579916L);

            migrationBuilder.DeleteData(
                table: "vocabulary_entries",
                keyColumn: "id",
                keyValue: 1563703640076579917L);

            migrationBuilder.DeleteData(
                table: "vocabulary_entries",
                keyColumn: "id",
                keyValue: 1563703640076579918L);

            migrationBuilder.DeleteData(
                table: "vocabulary_entries",
                keyColumn: "id",
                keyValue: 1563703640076579919L);

            migrationBuilder.DeleteData(
                table: "vocabulary_entries",
                keyColumn: "id",
                keyValue: 1563703640076579920L);

            migrationBuilder.DeleteData(
                table: "vocabulary_entries",
                keyColumn: "id",
                keyValue: 1563703640076579921L);

            migrationBuilder.DeleteData(
                table: "vocabulary_entries",
                keyColumn: "id",
                keyValue: 1563703640076579922L);

            migrationBuilder.DeleteData(
                table: "vocabulary_entries",
                keyColumn: "id",
                keyValue: 1563703640076579923L);

            migrationBuilder.DeleteData(
                table: "vocabulary_entries",
                keyColumn: "id",
                keyValue: 1563703640076579924L);

            migrationBuilder.DeleteData(
                table: "vocabulary_entries",
                keyColumn: "id",
                keyValue: 1563703640076579925L);

            migrationBuilder.DeleteData(
                table: "vocabulary_entries",
                keyColumn: "id",
                keyValue: 1563703640076579926L);

            migrationBuilder.DeleteData(
                table: "vocabulary_entries",
                keyColumn: "id",
                keyValue: 1563703640076579927L);

            migrationBuilder.DeleteData(
                table: "vocabulary_entries",
                keyColumn: "id",
                keyValue: 1563703640076579928L);

            migrationBuilder.DeleteData(
                table: "vocabulary_entries",
                keyColumn: "id",
                keyValue: 1563703640076579929L);

            migrationBuilder.DeleteData(
                table: "vocabulary_entries",
                keyColumn: "id",
                keyValue: 1563703640076579930L);

            migrationBuilder.DeleteData(
                table: "vocabulary_entries",
                keyColumn: "id",
                keyValue: 1563703640076579931L);

            migrationBuilder.DeleteData(
                table: "vocabulary_entries",
                keyColumn: "id",
                keyValue: 1563703640076579932L);

            migrationBuilder.DeleteData(
                table: "vocabulary_entries",
                keyColumn: "id",
                keyValue: 1563703640076579933L);

            migrationBuilder.DeleteData(
                table: "vocabulary_entries",
                keyColumn: "id",
                keyValue: 1563703640076579934L);

            migrationBuilder.DeleteData(
                table: "vocabulary_entries",
                keyColumn: "id",
                keyValue: 1563703640076579935L);

            migrationBuilder.DeleteData(
                table: "vocabulary_entries",
                keyColumn: "id",
                keyValue: 1563703640076579936L);

            migrationBuilder.DeleteData(
                table: "vocabulary_entries",
                keyColumn: "id",
                keyValue: 1563703640076579937L);

            migrationBuilder.DeleteData(
                table: "vocabulary_entries",
                keyColumn: "id",
                keyValue: 1563703640076579938L);

            migrationBuilder.DeleteData(
                table: "vocabulary_entries",
                keyColumn: "id",
                keyValue: 1563703640076579939L);

            migrationBuilder.DeleteData(
                table: "vocabulary_entries",
                keyColumn: "id",
                keyValue: 1563703640076579940L);

            migrationBuilder.DeleteData(
                table: "vocabulary_entries",
                keyColumn: "id",
                keyValue: 1563703640076579941L);

            migrationBuilder.DeleteData(
                table: "vocabulary_entries",
                keyColumn: "id",
                keyValue: 1563703640076579942L);

            migrationBuilder.DeleteData(
                table: "vocabulary_entries",
                keyColumn: "id",
                keyValue: 1563703640076579943L);

            migrationBuilder.DeleteData(
                table: "vocabulary_entries",
                keyColumn: "id",
                keyValue: 1563703640076579944L);

            migrationBuilder.DeleteData(
                table: "vocabulary_entries",
                keyColumn: "id",
                keyValue: 1563703640076579945L);

            migrationBuilder.DeleteData(
                table: "vocabulary_entries",
                keyColumn: "id",
                keyValue: 1563703640076579946L);

            migrationBuilder.DeleteData(
                table: "vocabulary_entries",
                keyColumn: "id",
                keyValue: 1563703640076579947L);

            migrationBuilder.DeleteData(
                table: "vocabulary_entries",
                keyColumn: "id",
                keyValue: 1563703640076579948L);

            migrationBuilder.DeleteData(
                table: "vocabulary_entries",
                keyColumn: "id",
                keyValue: 1563703640076579949L);

            migrationBuilder.DeleteData(
                table: "vocabulary_entries",
                keyColumn: "id",
                keyValue: 1563703640076579950L);

            migrationBuilder.DeleteData(
                table: "vocabulary_entries",
                keyColumn: "id",
                keyValue: 1563703640076579951L);

            migrationBuilder.DeleteData(
                table: "vocabulary_entries",
                keyColumn: "id",
                keyValue: 1563703640076579952L);

            migrationBuilder.DeleteData(
                table: "vocabulary_entries",
                keyColumn: "id",
                keyValue: 1563703640076579953L);

            migrationBuilder.DeleteData(
                table: "vocabulary_entries",
                keyColumn: "id",
                keyValue: 1563703640076579954L);

            migrationBuilder.DeleteData(
                table: "vocabulary_entries",
                keyColumn: "id",
                keyValue: 1563703640076579955L);

            migrationBuilder.DeleteData(
                table: "vocabulary_entries",
                keyColumn: "id",
                keyValue: 1563703640076579956L);

            migrationBuilder.DeleteData(
                table: "vocabulary_entries",
                keyColumn: "id",
                keyValue: 1563703640076579957L);

            migrationBuilder.DeleteData(
                table: "vocabulary_entries",
                keyColumn: "id",
                keyValue: 1563703640076579958L);

            migrationBuilder.DeleteData(
                table: "vocabulary_entries",
                keyColumn: "id",
                keyValue: 1563703640076579959L);

            migrationBuilder.DeleteData(
                table: "vocabulary_entries",
                keyColumn: "id",
                keyValue: 1563703640076579960L);

            migrationBuilder.DeleteData(
                table: "vocabulary_entries",
                keyColumn: "id",
                keyValue: 1563703640076579961L);

            migrationBuilder.DeleteData(
                table: "vocabulary_entries",
                keyColumn: "id",
                keyValue: 1563703640076579962L);

            migrationBuilder.DeleteData(
                table: "vocabulary_entries",
                keyColumn: "id",
                keyValue: 1563703640076579963L);

            migrationBuilder.DeleteData(
                table: "vocabulary_entries",
                keyColumn: "id",
                keyValue: 1563703640076579964L);

            migrationBuilder.DeleteData(
                table: "vocabulary_entries",
                keyColumn: "id",
                keyValue: 1563703640076579965L);

            migrationBuilder.DeleteData(
                table: "vocabulary_entries",
                keyColumn: "id",
                keyValue: 1563703640076579966L);

            migrationBuilder.DeleteData(
                table: "vocabulary_entries",
                keyColumn: "id",
                keyValue: 1563703640076579967L);

            migrationBuilder.DeleteData(
                table: "vocabulary_entries",
                keyColumn: "id",
                keyValue: 1563703640076579968L);

            migrationBuilder.DeleteData(
                table: "vocabulary_entries",
                keyColumn: "id",
                keyValue: 1563703640076579969L);

            migrationBuilder.DeleteData(
                table: "vocabulary_entries",
                keyColumn: "id",
                keyValue: 1563703640076579970L);

            migrationBuilder.DeleteData(
                table: "vocabulary_entries",
                keyColumn: "id",
                keyValue: 1563703640076579971L);

            migrationBuilder.DeleteData(
                table: "vocabulary_entries",
                keyColumn: "id",
                keyValue: 1563703640076579972L);

            migrationBuilder.DeleteData(
                table: "vocabulary_entries",
                keyColumn: "id",
                keyValue: 1563703640076579973L);

            migrationBuilder.DeleteData(
                table: "vocabulary_entries",
                keyColumn: "id",
                keyValue: 1563703640076579974L);

            migrationBuilder.DeleteData(
                table: "vocabulary_entries",
                keyColumn: "id",
                keyValue: 1563703640076579975L);

            migrationBuilder.DeleteData(
                table: "vocabulary_entries",
                keyColumn: "id",
                keyValue: 1563703640076579976L);

            migrationBuilder.DeleteData(
                table: "vocabulary_entries",
                keyColumn: "id",
                keyValue: 1563703640076579977L);

            migrationBuilder.DeleteData(
                table: "vocabulary_entries",
                keyColumn: "id",
                keyValue: 1563703640076579978L);

            migrationBuilder.DeleteData(
                table: "vocabulary_entries",
                keyColumn: "id",
                keyValue: 1563703640076579979L);

            migrationBuilder.DeleteData(
                table: "vocabulary_entries",
                keyColumn: "id",
                keyValue: 1563703640076579980L);

            migrationBuilder.DeleteData(
                table: "vocabulary_entries",
                keyColumn: "id",
                keyValue: 1563703640076579981L);

            migrationBuilder.DeleteData(
                table: "vocabulary_entries",
                keyColumn: "id",
                keyValue: 1563703640076579982L);

            migrationBuilder.DeleteData(
                table: "vocabulary_entries",
                keyColumn: "id",
                keyValue: 1563703640076579983L);

            migrationBuilder.DeleteData(
                table: "vocabulary_entries",
                keyColumn: "id",
                keyValue: 1563703640076579984L);

            migrationBuilder.DeleteData(
                table: "vocabulary_entries",
                keyColumn: "id",
                keyValue: 1563703640076579985L);

            migrationBuilder.DeleteData(
                table: "vocabulary_entries",
                keyColumn: "id",
                keyValue: 1563703640076579986L);

            migrationBuilder.DeleteData(
                table: "vocabulary_entries",
                keyColumn: "id",
                keyValue: 1563703640076579987L);

            migrationBuilder.DeleteData(
                table: "vocabulary_entries",
                keyColumn: "id",
                keyValue: 1563703640076579988L);

            migrationBuilder.DeleteData(
                table: "vocabulary_entries",
                keyColumn: "id",
                keyValue: 1563703640076579989L);

            migrationBuilder.DeleteData(
                table: "vocabulary_entries",
                keyColumn: "id",
                keyValue: 1563703640076579990L);

            migrationBuilder.DeleteData(
                table: "vocabulary_entries",
                keyColumn: "id",
                keyValue: 1563703640076579991L);

            migrationBuilder.DeleteData(
                table: "vocabulary_entries",
                keyColumn: "id",
                keyValue: 1563703640076579992L);

            migrationBuilder.DeleteData(
                table: "vocabulary_entries",
                keyColumn: "id",
                keyValue: 1563703640076579993L);

            migrationBuilder.DeleteData(
                table: "vocabulary_entries",
                keyColumn: "id",
                keyValue: 1563703640076579994L);

            migrationBuilder.DeleteData(
                table: "vocabulary_entries",
                keyColumn: "id",
                keyValue: 1563703640076579995L);

            migrationBuilder.DeleteData(
                table: "vocabulary_entries",
                keyColumn: "id",
                keyValue: 1563703640076579996L);

            migrationBuilder.DeleteData(
                table: "vocabulary_entries",
                keyColumn: "id",
                keyValue: 1563703640076579997L);

            migrationBuilder.DeleteData(
                table: "vocabulary_entries",
                keyColumn: "id",
                keyValue: 1563703640076579998L);

            migrationBuilder.DeleteData(
                table: "vocabulary_entries",
                keyColumn: "id",
                keyValue: 1563703640076579999L);

            migrationBuilder.DeleteData(
                table: "vocabulary_entries",
                keyColumn: "id",
                keyValue: 1563703640076580000L);

            migrationBuilder.DeleteData(
                table: "vocabulary_entries",
                keyColumn: "id",
                keyValue: 1563703640076580001L);

            migrationBuilder.DeleteData(
                table: "vocabulary_entries",
                keyColumn: "id",
                keyValue: 1563703640076580002L);

            migrationBuilder.DeleteData(
                table: "vocabulary_entries",
                keyColumn: "id",
                keyValue: 1563703640076580003L);

            migrationBuilder.DeleteData(
                table: "vocabulary_entries",
                keyColumn: "id",
                keyValue: 1563703640076580004L);

            migrationBuilder.DeleteData(
                table: "vocabulary_entries",
                keyColumn: "id",
                keyValue: 1563703640076580005L);

            migrationBuilder.DeleteData(
                table: "vocabulary_entries",
                keyColumn: "id",
                keyValue: 1563703640076580006L);

            migrationBuilder.DeleteData(
                table: "vocabulary_entries",
                keyColumn: "id",
                keyValue: 1563703640076580007L);

            migrationBuilder.DeleteData(
                table: "vocabulary_entries",
                keyColumn: "id",
                keyValue: 1563703640076580008L);

            migrationBuilder.DeleteData(
                table: "vocabulary_entries",
                keyColumn: "id",
                keyValue: 1563703640076580009L);

            migrationBuilder.DeleteData(
                table: "vocabulary_entries",
                keyColumn: "id",
                keyValue: 1563703640076580010L);

            migrationBuilder.DeleteData(
                table: "vocabulary_entries",
                keyColumn: "id",
                keyValue: 1563703640076580011L);

            migrationBuilder.DeleteData(
                table: "vocabulary_entries",
                keyColumn: "id",
                keyValue: 1563703640076580012L);

            migrationBuilder.DeleteData(
                table: "vocabulary_entries",
                keyColumn: "id",
                keyValue: 1563703640076580013L);

            migrationBuilder.DeleteData(
                table: "vocabulary_entries",
                keyColumn: "id",
                keyValue: 1563703640076580014L);

            migrationBuilder.DeleteData(
                table: "vocabulary_entries",
                keyColumn: "id",
                keyValue: 1563703640076580015L);

            migrationBuilder.DeleteData(
                table: "writing_systems",
                keyColumn: "id",
                keyValue: 1563703640076578826L);

            migrationBuilder.DeleteData(
                table: "languages",
                keyColumn: "id",
                keyValue: 1563703640076578816L);
        }
    }
}
