using System;
using System.Collections.Generic;
using FLAGGED;

class Program
{
    static CoreGameLogic game = new CoreGameLogic();
    static int score = 0;

    static void Main()
    {
        Console.WriteLine("=== FLAGGED! Core Logic Test Harness ===");

        int day = 5; // random 6-of-19 mix from the full pool; re-run to vary
        // To force specific trap articles instead of a random day:
        // game.Debug_LoadArticles("ALERT LEVEL 5", "98 TRILLION", "police generals");
        Console.WriteLine($"\n--- Starting Shift: Day {day} ---");
        game.StartShift(day);

        while (true)
        {
            Article current = game.GetCurrentArticle();
            if (current == null)
            {
                Console.WriteLine("\n[SHIFT OVER] No more articles in the queue.");
                break;
            }

            PrintArticle(current);
            Console.WriteLine("Actions: [1] Accept  [2] Reject  [3] Database  [Q] Quit");
            Console.Write("> ");
            string input = (Console.ReadLine() ?? "").Trim();

            if (input.Equals("q", StringComparison.OrdinalIgnoreCase)) break;

            if (input == "3")
            {
                DatabaseLoop(current);
                continue; // searching never consumes the article
            }

            if (input != "1" && input != "2")
            {
                Console.WriteLine("(!) Press 1, 2, 3, or Q.");
                continue;
            }

            var result = game.SubmitJudgment(input == "1" ? Judgment.Accept : Judgment.Reject);
            score += result.PointsDelta;

            Console.WriteLine($"\nRESULT:     {(result.IsCorrect ? "CORRECT!" : "WRONG!")}");
            Console.WriteLine($"POINTS:     {(result.PointsDelta > 0 ? "+" : "")}{result.PointsDelta}  (total: {score})");
            Console.WriteLine($"ARTICLE WAS: {(result.WasFake ? "FAKE" : "REAL")}");
            Console.WriteLine($"FEEDBACK:   {result.Message}");

            Console.WriteLine("\n--- REVEAL (post-judgment only) ---");
            foreach (var row in result.Reveal)
            {
                Console.WriteLine($"  {row.Field}:");
                Console.WriteLine($"     Article : {row.ArticleSays}");
                Console.WriteLine($"     Official: {row.DatabaseSays}{(row.Mismatch ? "   <-- MISMATCH" : "")}");
            }

            if (result.ShiftEnded)
            {
                Console.WriteLine("\n[SHIFT OVER] Queue is empty.");
                break;
            }
        }

        Console.WriteLine($"\nFinal test score: {score}");
        Console.WriteLine("Test complete. Press Enter to exit.");
        Console.ReadLine();
    }

    // Shows ONLY UI-safe properties: Headline, Author, Domain, Body, and each
    // field's Name + ArticleValue. Never print OfficialValue or IsFake here —
    // those are post-judgment-only. This is the same contract Robby's UI follows.
    static void PrintArticle(Article a)
    {
        Console.WriteLine("\n========================================");
        Console.WriteLine($"HEADLINE: {a.Headline}");
        Console.WriteLine($"AUTHOR:   {a.Author}");
        Console.WriteLine($"DOMAIN:   {a.Domain}");
        Console.WriteLine($"BODY:     {a.Body}");
        Console.WriteLine("-- Claims to investigate --");
        for (int i = 0; i < a.Fields.Count; i++)
            Console.WriteLine($"  [{i + 1}] {a.Fields[i].Name}: {a.Fields[i].ArticleValue}");
        Console.WriteLine("========================================");
    }

    static void DatabaseLoop(Article current)
    {
        while (true)
        {
            Console.WriteLine("\n--- DATABASE ---");
            Console.WriteLine("search term | 'i <n>' investigate field n | 'b' [author|domain|statement] | 'x' close");
            Console.Write("db> ");
            string input = (Console.ReadLine() ?? "").Trim();
            if (input.Length == 0) continue;
            if (input.Equals("x", StringComparison.OrdinalIgnoreCase)) return;

            // Investigate a numbered field of the current article
            if (input.StartsWith("i ", StringComparison.OrdinalIgnoreCase))
            {
                if (int.TryParse(input.Substring(2).Trim(), out int n) &&
                    n >= 1 && n <= current.Fields.Count)
                {
                    PrintSearchResult(game.InvestigateField(n - 1));
                }
                else
                {
                    Console.WriteLine($"(!) Field number must be 1-{current.Fields.Count}.");
                }
                continue;
            }

            // Browse the registry — exact 'b' or 'b <category>'
            // ('b' alone = everything; searches like "Binaliw" still work normally)
            if (input.Equals("b", StringComparison.OrdinalIgnoreCase) ||
                input.StartsWith("b ", StringComparison.OrdinalIgnoreCase))
            {
                string rest = input.Length > 2 ? input.Substring(2).Trim() : "";
                string cat = null;
                if (rest.StartsWith("a", StringComparison.OrdinalIgnoreCase)) cat = "Author";
                else if (rest.StartsWith("d", StringComparison.OrdinalIgnoreCase)) cat = "Domain";
                else if (rest.StartsWith("s", StringComparison.OrdinalIgnoreCase) ||
                         rest.StartsWith("o", StringComparison.OrdinalIgnoreCase)) cat = "Official Statement";
                else if (rest.Length > 0)
                {
                    Console.WriteLine("(!) Categories: author, domain, statement.");
                    continue;
                }
                PrintRecords(game.BrowseDatabase(cat), $"BROWSE {(cat ?? "ALL")}");
                continue;
            }

            PrintSearchResult(game.SearchDatabase(input));
        }
    }

    static void PrintSearchResult(DatabaseSearchResult r)
    {
        Console.WriteLine($"\n{r.Message}");
        PrintRecords(r.Records, r.Query);
    }

    static void PrintRecords(List<DatabaseRecord> records, string label)
    {
        if (records.Count == 0) return;
        Console.WriteLine($"--- Records for '{label}' ---");
        foreach (var rec in records)
        {
            ConsoleColor old = Console.ForegroundColor;
            if (rec.Credibility.Equals("High", StringComparison.OrdinalIgnoreCase)) Console.ForegroundColor = ConsoleColor.Green;
            else if (rec.Credibility.Equals("Low", StringComparison.OrdinalIgnoreCase)) Console.ForegroundColor = ConsoleColor.Red;
            else Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"  [{rec.Category}] {rec.Name} — Credibility: {rec.Credibility}");
            Console.ForegroundColor = old;
            Console.WriteLine($"      {rec.Notes}");
        }
    }
}