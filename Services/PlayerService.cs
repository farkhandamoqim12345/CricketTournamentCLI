using System.Data;
using CricketTournamentCLI.Database;

namespace CricketTournamentCLI.Services
{
    public static class PlayerService
    {
        public static void ManagePlayers(DbHelper db)
        {
            bool back = false;
            while (!back)
            {
                Console.Clear();
                Console.WriteLine("\n👤 PLAYER MANAGEMENT");
                Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━");
                Console.WriteLine("1. View All Players");
                Console.WriteLine("2. Add New Player");
                Console.WriteLine("3. Delete Player");
                Console.WriteLine("4. Update Player Stats");
                Console.WriteLine("5. View Player Stats");
                Console.WriteLine("6. Search Player by Name");   // ← NEW
                Console.WriteLine("7. Back to Main Menu");
                Console.Write("\nChoice: ");

                string choice = Console.ReadLine() ?? "";

                switch (choice)
                {
                    case "1": ViewAllPlayers(db);    break;
                    case "2": AddPlayer(db);         break;
                    case "3": DeletePlayer(db);      break;
                    case "4": UpdatePlayerStats(db); break;
                    case "5": ViewPlayerStats(db);   break;
                    case "6": SearchPlayer(db);      break;   // ← NEW
                    case "7": back = true;           break;
                    default:
                        Console.WriteLine("Invalid option!");
                        Console.ReadKey();
                        break;
                }
            }
        }

        // ── View All Players ──────────────────────────────────────────────
        public static void ViewAllPlayers(DbHelper db)
        {
            Console.Clear();
            Console.WriteLine("\n📋 ALL PLAYERS");
            Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━");

            DataTable dt = db.GetData(@"
                SELECT p.PlayerId, p.PlayerName, p.Role, t.TeamName,
                       CASE WHEN p.IsCaptain=1 THEN 'Yes' ELSE '' END AS Captain
                FROM Players p
                LEFT JOIN Teams t ON p.TeamId = t.TeamId
                ORDER BY t.TeamName, p.PlayerName");

            if (dt.Rows.Count == 0)
            {
                Console.WriteLine("No players found!");
            }
            else
            {
                Console.WriteLine($"{"ID",-5} {"Player Name",-22} {"Role",-14} {"Team",-18} {"Captain",-8}");
                Console.WriteLine(new string('-', 70));
                foreach (DataRow row in dt.Rows)
                {
                    Console.WriteLine($"{row["PlayerId"],-5} {row["PlayerName"],-22} {row["Role"],-14} {row["TeamName"],-18} {row["Captain"],-8}");
                }
            }

            Console.WriteLine($"\nTotal Players: {dt.Rows.Count}");
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }

        // ── Add Player ────────────────────────────────────────────────────
        static void AddPlayer(DbHelper db)
        {
            Console.Clear();
            Console.WriteLine("\n➕ ADD NEW PLAYER");
            Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━");

            DataTable teams = db.GetData("SELECT TeamId, TeamName FROM Teams ORDER BY TeamName");
            if (teams.Rows.Count == 0)
            {
                Console.WriteLine("❌ No teams found! Add a team first.");
                Console.ReadKey();
                return;
            }

            Console.WriteLine("\nAvailable Teams:");
            foreach (DataRow row in teams.Rows)
                Console.WriteLine($"  {row["TeamId"]}. {row["TeamName"]}");

            Console.Write("\nSelect Team ID: ");
            if (!int.TryParse(Console.ReadLine(), out int teamId))
            {
                Console.WriteLine("❌ Invalid team ID!");
                Console.ReadKey();
                return;
            }

            Console.Write("Player Name: ");
            string name = Console.ReadLine() ?? "";

            if (string.IsNullOrWhiteSpace(name))
            {
                Console.WriteLine("❌ Player name cannot be empty!");
                Console.ReadKey();
                return;
            }

            Console.Write("Role (Batsman/Bowler/All-Rounder/Wicket-Keeper): ");
            string role = Console.ReadLine() ?? "Batsman";

            Console.Write("Batting Style (Right Hand/Left Hand): ");
            string battingStyle = Console.ReadLine() ?? "Right Hand";

            Console.Write("Bowling Style (e.g., Right Arm Fast / Left Arm Spin / N/A): ");
            string bowlingStyle = Console.ReadLine() ?? "N/A";

            Console.Write("Is Captain? (y/n): ");
            int isCaptain = Console.ReadLine()?.ToLower() == "y" ? 1 : 0;

            Console.Write("Is Wicket Keeper? (y/n): ");
            int isWK = Console.ReadLine()?.ToLower() == "y" ? 1 : 0;

            // Parameterized query use karo - SQL injection safe
            db.ExecuteQueryWithParams(
                @"INSERT INTO Players (PlayerName, Role, BattingStyle, BowlingStyle, TeamId, IsCaptain, IsWicketKeeper)
                  VALUES (@name, @role, @bat, @bowl, @teamId, @cap, @wk)",
                new Dictionary<string, object>
                {
                    ["@name"]   = name,
                    ["@role"]   = role,
                    ["@bat"]    = battingStyle,
                    ["@bowl"]   = bowlingStyle,
                    ["@teamId"] = teamId,
                    ["@cap"]    = isCaptain,
                    ["@wk"]     = isWK
                });

            // Player stats ka empty record bhi create karo
            DataTable newPlayer = db.GetData($"SELECT PlayerId FROM Players WHERE PlayerName='{name}' AND TeamId={teamId}");
            if (newPlayer.Rows.Count > 0)
            {
                int newId = Convert.ToInt32(newPlayer.Rows[0]["PlayerId"]);
                db.ExecuteQuery($"INSERT INTO PlayerStats (PlayerId) VALUES ({newId})");
            }

            Console.WriteLine($"✅ Player '{name}' added successfully!");
            Console.ReadKey();
        }

        // ── Delete Player ─────────────────────────────────────────────────
        static void DeletePlayer(DbHelper db)
        {
            ViewAllPlayers(db);
            Console.Write("\nEnter Player ID to delete: ");
            if (int.TryParse(Console.ReadLine(), out int id))
            {
                Console.Write($"Delete player {id}? (y/n): ");
                if (Console.ReadLine()?.ToLower() == "y")
                {
                    db.ExecuteQuery($"DELETE FROM PlayerStats WHERE PlayerId = {id}");
                    db.ExecuteQuery($"DELETE FROM Players WHERE PlayerId = {id}");
                    Console.WriteLine("✅ Player deleted!");
                }
            }
            Console.ReadKey();
        }

        // ── Update Player Stats ───────────────────────────────────────────
        static void UpdatePlayerStats(DbHelper db)
        {
            ViewAllPlayers(db);
            Console.Write("\nEnter Player ID: ");
            if (!int.TryParse(Console.ReadLine(), out int id)) { Console.ReadKey(); return; }

            Console.Write("Matches played: ");
            if (!int.TryParse(Console.ReadLine(), out int matches)) matches = 0;
            Console.Write("Total Runs: ");
            if (!int.TryParse(Console.ReadLine(), out int runs)) runs = 0;
            Console.Write("Wickets: ");
            if (!int.TryParse(Console.ReadLine(), out int wickets)) wickets = 0;
            Console.Write("Catches: ");
            if (!int.TryParse(Console.ReadLine(), out int catches)) catches = 0;
            Console.Write("Highest Score: ");
            if (!int.TryParse(Console.ReadLine(), out int highScore)) highScore = 0;
            Console.Write("Best Bowling (e.g., 5/20 or N/A): ");
            string bestBowling = Console.ReadLine() ?? "N/A";

            DataTable check = db.GetData($"SELECT COUNT(*) FROM PlayerStats WHERE PlayerId = {id}");
            if (Convert.ToInt32(check.Rows[0][0]) > 0)
            {
                db.ExecuteQueryWithParams(
                    @"UPDATE PlayerStats SET 
                        Matches=@m, Runs=@r, Wickets=@w, 
                        Catches=@c, HighScore=@hs, BestBowling=@bb
                      WHERE PlayerId=@id",
                    new Dictionary<string, object>
                    {
                        ["@m"]  = matches,  ["@r"]  = runs,
                        ["@w"]  = wickets,  ["@c"]  = catches,
                        ["@hs"] = highScore,["@bb"] = bestBowling,
                        ["@id"] = id
                    });
            }
            else
            {
                db.ExecuteQueryWithParams(
                    @"INSERT INTO PlayerStats (PlayerId, Matches, Runs, Wickets, Catches, HighScore, BestBowling)
                      VALUES (@id, @m, @r, @w, @c, @hs, @bb)",
                    new Dictionary<string, object>
                    {
                        ["@id"] = id,  ["@m"]  = matches, ["@r"]  = runs,
                        ["@w"]  = wickets, ["@c"] = catches,
                        ["@hs"] = highScore, ["@bb"] = bestBowling
                    });
            }

            Console.WriteLine("✅ Stats updated successfully!");
            Console.ReadKey();
        }

        // ── View Player Stats ─────────────────────────────────────────────
        public static void ViewPlayerStats(DbHelper db)
        {
            ViewAllPlayers(db);
            Console.Write("\nEnter Player ID: ");
            if (int.TryParse(Console.ReadLine(), out int id))
            {
                ShowPlayerDetail(db, id);
            }
            Console.WriteLine("\nPress any key...");
            Console.ReadKey();
        }

        // ── 🔍 Search Player by Name (NEW FEATURE) ───────────────────────
        public static void SearchPlayer(DbHelper db)
        {
            Console.Clear();
            Console.WriteLine("\n🔍 SEARCH PLAYER");
            Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━");
            Console.Write("Enter player name (partial ok): ");
            string keyword = Console.ReadLine() ?? "";

            if (string.IsNullOrWhiteSpace(keyword))
            {
                Console.WriteLine("❌ Please enter a search term!");
                Console.ReadKey();
                return;
            }

            // LIKE query - partial name se bhi search ho
            DataTable dt = db.GetData($@"
                SELECT p.PlayerId, p.PlayerName, p.Role, t.TeamName,
                       p.BattingStyle, p.BowlingStyle,
                       CASE WHEN p.IsCaptain=1 THEN 'Yes' ELSE 'No' END AS Captain
                FROM Players p
                LEFT JOIN Teams t ON p.TeamId = t.TeamId
                WHERE p.PlayerName LIKE '%{keyword}%'
                ORDER BY p.PlayerName");

            Console.Clear();
            Console.WriteLine($"\n🔍 SEARCH RESULTS FOR: \"{keyword}\"");
            Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");

            if (dt.Rows.Count == 0)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"\n  No players found matching '{keyword}'.");
                Console.ResetColor();
            }
            else
            {
                Console.WriteLine($"  Found {dt.Rows.Count} player(s):\n");

                foreach (DataRow row in dt.Rows)
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine($"  ┌─ ID: {row["PlayerId"]} ─ {row["PlayerName"]}");
                    Console.ResetColor();
                    Console.WriteLine($"  │  Role:          {row["Role"]}");
                    Console.WriteLine($"  │  Team:          {row["TeamName"]}");
                    Console.WriteLine($"  │  Batting Style: {row["BattingStyle"]}");
                    Console.WriteLine($"  │  Bowling Style: {row["BowlingStyle"]}");
                    Console.WriteLine($"  └─ Captain:       {row["Captain"]}");
                    Console.WriteLine();
                }

                // Ask if they want to see stats for any player
                Console.Write("  View detailed stats for a player? Enter Player ID (or 0 to skip): ");
                if (int.TryParse(Console.ReadLine(), out int chosenId) && chosenId != 0)
                {
                    ShowPlayerDetail(db, chosenId);
                }
            }

            Console.WriteLine("\nPress any key...");
            Console.ReadKey();
        }

        // ── Helper: Show one player's full detail ─────────────────────────
        static void ShowPlayerDetail(DbHelper db, int id)
        {
            DataTable player = db.GetData($"SELECT PlayerName FROM Players WHERE PlayerId={id}");
            if (player.Rows.Count == 0)
            {
                Console.WriteLine("❌ Player not found!");
                return;
            }

            string name = player.Rows[0]["PlayerName"].ToString() ?? "Unknown";

            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"\n📊 PLAYER STATISTICS - {name}");
            Console.ResetColor();
            Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");

            DataTable stats = db.GetData($"SELECT * FROM PlayerStats WHERE PlayerId={id}");
            if (stats.Rows.Count > 0)
            {
                DataRow row = stats.Rows[0];
                int matches  = Convert.ToInt32(row["Matches"]);
                int runs     = Convert.ToInt32(row["Runs"]);
                int wickets  = Convert.ToInt32(row["Wickets"]);

                Console.WriteLine($"  Matches Played : {matches}");
                Console.WriteLine($"  Total Runs     : {runs}");
                Console.WriteLine($"  Wickets        : {wickets}");
                Console.WriteLine($"  Catches        : {row["Catches"]}");
                Console.WriteLine($"  Highest Score  : {row["HighScore"]}");
                Console.WriteLine($"  Best Bowling   : {row["BestBowling"]}");

                if (matches > 0)
                {
                    double battingAvg = (double)runs / matches;
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine($"\n  Batting Average: {battingAvg:F2}");
                    Console.ResetColor();
                }
                if (wickets > 0)
                {
                    double bowlingAvg = (double)runs / wickets;
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    Console.WriteLine($"  Bowling Average: {bowlingAvg:F2}");
                    Console.ResetColor();
                }
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("  No stats recorded yet for this player.");
                Console.ResetColor();
            }
        }
    }
}
