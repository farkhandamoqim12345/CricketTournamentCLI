using System.Data;
using CricketTournamentCLI.Database;

namespace CricketTournamentCLI.Services
{
    /// <summary>
    /// Tournament ka complete summary ek jagah dikhata hai
    /// </summary>
    public static class TournamentSummaryService
    {
        public static void ShowSummary(DbHelper db)
        {
            Console.Clear();

            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("+" + new string('=', 70) + "+");
            Console.ForegroundColor = ConsoleColor.Green;
            string title = "🏆  TOURNAMENT SUMMARY  🏆";
            Console.WriteLine("|" + Centre(title, 70) + "|");
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("+" + new string('=', 70) + "+");
            Console.ResetColor();

            // ── Overview counts ─────────────────────────────────────────
            DataTable overview = db.GetData(@"
                SELECT 
                    (SELECT COUNT(*) FROM Teams)   AS TotalTeams,
                    (SELECT COUNT(*) FROM Players) AS TotalPlayers,
                    (SELECT COUNT(*) FROM Matches) AS TotalMatches,
                    (SELECT COUNT(*) FROM Matches WHERE Status = 'Completed') AS CompletedMatches,
                    (SELECT COUNT(*) FROM Matches WHERE Status = 'Scheduled') AS UpcomingMatches
            ");

            if (overview.Rows.Count > 0)
            {
                DataRow r = overview.Rows[0];
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("\n  📊 OVERVIEW");
                Console.ResetColor();
                Console.WriteLine($"  {"Teams Registered:",-28} {r["TotalTeams"]}");
                Console.WriteLine($"  {"Total Players:",-28} {r["TotalPlayers"]}");
                Console.WriteLine($"  {"Total Matches:",-28} {r["TotalMatches"]}");
                Console.WriteLine($"  {"Completed Matches:",-28} {r["CompletedMatches"]}");
                Console.WriteLine($"  {"Upcoming Matches:",-28} {r["UpcomingMatches"]}");
            }

            Console.WriteLine("\n" + new string('-', 72));

            // ── Points Table ────────────────────────────────────────────
            DataTable standings = db.GetData(@"
                SELECT ROW_NUMBER() OVER (ORDER BY s.Points DESC, s.Won DESC) AS Pos,
                       t.TeamName, s.Played, s.Won, s.Lost, s.Points
                FROM Standings s
                JOIN Teams t ON s.TeamId = t.TeamId
                ORDER BY s.Points DESC, s.Won DESC
            ");

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\n  🥇 POINTS TABLE");
            Console.ResetColor();

            if (standings.Rows.Count == 0)
            {
                Console.WriteLine("  No standings data yet.");
            }
            else
            {
                Console.WriteLine($"  {"Pos",-5} {"Team",-24} {"P",-5} {"W",-5} {"L",-5} {"Pts",-5}");
                Console.WriteLine("  " + new string('-', 48));
                foreach (DataRow row in standings.Rows)
                {
                    // Leader ko highlight karo
                    if (Convert.ToInt32(row["Pos"]) == 1)
                        Console.ForegroundColor = ConsoleColor.Green;

                    Console.WriteLine($"  {row["Pos"],-5} {row["TeamName"],-24} {row["Played"],-5} {row["Won"],-5} {row["Lost"],-5} {row["Points"],-5}");
                    Console.ResetColor();
                }

                // Champion banner
                DataRow leader = standings.Rows[0];
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"\n  🏆 Current Leader: {leader["TeamName"]} ({leader["Points"]} pts)");
                Console.ResetColor();
            }

            Console.WriteLine("\n" + new string('-', 72));

            // ── Top Batsman ─────────────────────────────────────────────
            DataTable topBat = db.GetData(@"
                SELECT TOP 3 p.PlayerName, t.TeamName, ps.Runs, ps.HighScore, ps.Matches
                FROM PlayerStats ps
                JOIN Players p ON ps.PlayerId = p.PlayerId
                JOIN Teams   t ON p.TeamId    = t.TeamId
                WHERE ps.Matches > 0
                ORDER BY ps.Runs DESC
            ");

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\n  🏏 TOP BATSMEN (by Runs)");
            Console.ResetColor();

            if (topBat.Rows.Count == 0)
            {
                Console.WriteLine("  No batting stats available yet.");
            }
            else
            {
                Console.WriteLine($"  {"Player",-22} {"Team",-20} {"Runs",-8} {"High",-8} {"Matches",-8}");
                Console.WriteLine("  " + new string('-', 60));
                int rank = 1;
                foreach (DataRow row in topBat.Rows)
                {
                    string medal = rank == 1 ? "🥇" : rank == 2 ? "🥈" : "🥉";
                    Console.WriteLine($"  {medal} {row["PlayerName"],-20} {row["TeamName"],-20} {row["Runs"],-8} {row["HighScore"],-8} {row["Matches"],-8}");
                    rank++;
                }
            }

            Console.WriteLine("\n" + new string('-', 72));

            // ── Top Bowler ──────────────────────────────────────────────
            DataTable topBowl = db.GetData(@"
                SELECT TOP 3 p.PlayerName, t.TeamName, ps.Wickets, ps.BestBowling, ps.Matches
                FROM PlayerStats ps
                JOIN Players p ON ps.PlayerId = p.PlayerId
                JOIN Teams   t ON p.TeamId    = t.TeamId
                WHERE ps.Wickets > 0
                ORDER BY ps.Wickets DESC
            ");

            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("\n  🎯 TOP BOWLERS (by Wickets)");
            Console.ResetColor();

            if (topBowl.Rows.Count == 0)
            {
                Console.WriteLine("  No bowling stats available yet.");
            }
            else
            {
                Console.WriteLine($"  {"Player",-22} {"Team",-20} {"Wkts",-8} {"Best",-10} {"Matches",-8}");
                Console.WriteLine("  " + new string('-', 65));
                int rank = 1;
                foreach (DataRow row in topBowl.Rows)
                {
                    string medal = rank == 1 ? "🥇" : rank == 2 ? "🥈" : "🥉";
                    Console.WriteLine($"  {medal} {row["PlayerName"],-20} {row["TeamName"],-20} {row["Wickets"],-8} {row["BestBowling"],-10} {row["Matches"],-8}");
                    rank++;
                }
            }

            Console.WriteLine("\n" + new string('-', 72));

            // ── Recent Results ──────────────────────────────────────────
            DataTable recent = db.GetData(@"
                SELECT TOP 3 
                    t1.TeamName AS Team1, m.Team1Score,
                    t2.TeamName AS Team2, m.Team2Score,
                    tw.TeamName AS Winner,
                    CONVERT(NVARCHAR, m.MatchDate, 103) AS MatchDate
                FROM Matches m
                JOIN Teams t1 ON m.Team1Id  = t1.TeamId
                JOIN Teams t2 ON m.Team2Id  = t2.TeamId
                LEFT JOIN Teams tw ON m.WinnerId = tw.TeamId
                WHERE m.Status = 'Completed'
                ORDER BY m.MatchDate DESC
            ");

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\n  📋 RECENT RESULTS");
            Console.ResetColor();

            if (recent.Rows.Count == 0)
            {
                Console.WriteLine("  No completed matches yet.");
            }
            else
            {
                foreach (DataRow row in recent.Rows)
                {
                    Console.WriteLine($"  {row["MatchDate"]}  {row["Team1"]} {row["Team1Score"]} vs {row["Team2"]} {row["Team2Score"]}");
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"           Winner: {row["Winner"]} ✅");
                    Console.ResetColor();
                    Console.WriteLine();
                }
            }

            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("+" + new string('=', 70) + "+");
            Console.ResetColor();

            Console.WriteLine("\nPress any key to go back...");
            Console.ReadKey();
        }

        static string Centre(string text, int width)
        {
            int total = width - text.Length;
            int left  = total / 2;
            int right = total - left;
            return new string(' ', left) + text + new string(' ', right);
        }
    }
}
