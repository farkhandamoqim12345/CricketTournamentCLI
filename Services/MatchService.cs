using System.Data;
using CricketTournamentCLI.Database;

namespace CricketTournamentCLI.Services
{
    public static class MatchService
    {
        public static void ManageMatches(DbHelper db)
        {
            bool back = false;
            while (!back)
            {
                Console.Clear();
                Console.WriteLine("\n📋 MATCH MANAGEMENT");
                Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━");
                Console.WriteLine("1. View All Matches");
                Console.WriteLine("2. Schedule New Match");
                Console.WriteLine("3. Update Match Result");
                Console.WriteLine("4. View Schedule");
                Console.WriteLine("5. Back to Main Menu");
                Console.Write("\nChoice: ");

                string choice = Console.ReadLine();
                switch (choice)
                {
                    case "1": ViewAllMatches(db); break;
                    case "2": ScheduleMatch(db); break;
                    case "3": UpdateMatchResult(db); break;
                    case "4": ViewSchedule(db); break;
                    case "5": back = true; break;
                    default: Console.WriteLine("Invalid option!"); Console.ReadKey(); break;
                }
            }
        }

        public static void ViewAllMatches(DbHelper db)
        {
            Console.Clear();
            Console.WriteLine("\n📋 ALL MATCHES");
            Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━");

            DataTable dt = db.GetData(@"
                SELECT m.MatchId, t1.TeamName as Team1, t2.TeamName as Team2,
                       CONVERT(NVARCHAR, m.MatchDate, 103) as MatchDate, m.Venue, m.Status,
                       ISNULL(m.Team1Score, '—') as Score1, ISNULL(m.Team2Score, '—') as Score2
                FROM Matches m
                JOIN Teams t1 ON m.Team1Id = t1.TeamId
                JOIN Teams t2 ON m.Team2Id = t2.TeamId
                ORDER BY m.MatchDate DESC");

            if (dt.Rows.Count == 0)
            {
                Console.WriteLine("No matches found!");
            }
            else
            {
                Console.WriteLine($"{"ID",-5} {"Team 1",-18} {"Team 2",-18} {"Date",-12} {"Status",-12} {"Venue",-15}");
                Console.WriteLine(new string('-', 85));
                foreach (DataRow row in dt.Rows)
                {
                    Console.WriteLine($"{row["MatchId"],-5} {row["Team1"],-18} {row["Team2"],-18} {row["MatchDate"],-12} {row["Status"],-12} {row["Venue"],-15}");
                }
            }

            Console.WriteLine($"\nTotal Matches: {dt.Rows.Count}");
            Console.WriteLine("\nPress any key...");
            Console.ReadKey();
        }

        static void ScheduleMatch(DbHelper db)
        {
            Console.Clear();
            Console.WriteLine("\n📅 SCHEDULE MATCH");
            Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━");

            DataTable teams = db.GetData("SELECT TeamId, TeamName FROM Teams");
            if (teams.Rows.Count < 2)
            {
                Console.WriteLine("❌ At least 2 teams required to schedule a match!");
                Console.ReadKey();
                return;
            }

            Console.WriteLine("\nTeams:");
            foreach (DataRow row in teams.Rows)
                Console.WriteLine($"  {row["TeamId"]}. {row["TeamName"]}");

            Console.Write("\nTeam 1 ID: ");
            int t1 = int.Parse(Console.ReadLine());
            Console.Write("Team 2 ID: ");
            int t2 = int.Parse(Console.ReadLine());

            if (t1 == t2)
            {
                Console.WriteLine("❌ Same team cannot play against itself!");
                Console.ReadKey();
                return;
            }

            Console.Write("Date (yyyy-mm-dd): ");
            string date = Console.ReadLine();
            Console.Write("Venue: ");
            string venue = Console.ReadLine();

            db.ExecuteQuery($@"
                INSERT INTO Matches (Team1Id, Team2Id, MatchDate, Venue, Status)
                VALUES ({t1}, {t2}, '{date}', '{venue}', 'Scheduled')");

            // Ensure standings exist for both teams
            db.ExecuteQuery($"IF NOT EXISTS (SELECT 1 FROM Standings WHERE TeamId={t1}) INSERT INTO Standings (TeamId) VALUES ({t1})");
            db.ExecuteQuery($"IF NOT EXISTS (SELECT 1 FROM Standings WHERE TeamId={t2}) INSERT INTO Standings (TeamId) VALUES ({t2})");

            Console.WriteLine("✅ Match scheduled successfully!");
            Console.ReadKey();
        }

        public static void UpdateMatchResult(DbHelper db)
        {
            ViewAllMatches(db);
            Console.Write("\nEnter Match ID to update result: ");
            if (int.TryParse(Console.ReadLine(), out int id))
            {
                DataTable match = db.GetData($@"
                    SELECT m.MatchId, t1.TeamName as Team1, t2.TeamName as Team2, 
                           t1.TeamId as Team1Id, t2.TeamId as Team2Id, m.Status
                    FROM Matches m
                    JOIN Teams t1 ON m.Team1Id = t1.TeamId
                    JOIN Teams t2 ON m.Team2Id = t2.TeamId
                    WHERE m.MatchId = {id}");

                if (match.Rows.Count > 0)
                {
                    DataRow row = match.Rows[0];
                    string status = row["Status"].ToString();

                    if (status == "Completed")
                    {
                        Console.WriteLine("⚠️ This match is already completed!");
                        Console.ReadKey();
                        return;
                    }

                    Console.Clear();
                    Console.WriteLine($"\n🎯 UPDATE RESULT - Match ID: {id}");
                    Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
                    Console.WriteLine($"\n{row["Team1"]} vs {row["Team2"]}");

                    Console.Write($"\nScore of {row["Team1"]} (e.g., 145/6): ");
                    string s1 = Console.ReadLine();
                    Console.Write($"Score of {row["Team2"]} (e.g., 138/8): ");
                    string s2 = Console.ReadLine();
                    Console.Write($"\nWinner (1={row["Team1"]}, 2={row["Team2"]}): ");
                    int winner = int.Parse(Console.ReadLine());
                    int winnerId = winner == 1 ? Convert.ToInt32(row["Team1Id"]) : Convert.ToInt32(row["Team2Id"]);
                    int loserId = winner == 1 ? Convert.ToInt32(row["Team2Id"]) : Convert.ToInt32(row["Team1Id"]);

                    // Update match result
                    db.ExecuteQuery($@"
                        UPDATE Matches SET 
                            Team1Score='{s1}', Team2Score='{s2}', 
                            WinnerId={winnerId}, Status='Completed'
                        WHERE MatchId={id}");

                    // Update standings (winner)
                    db.ExecuteQuery($@"UPDATE Standings SET 
                        Played = Played + 1, 
                        Won = Won + 1, 
                        Points = Points + 2 
                        WHERE TeamId = {winnerId}");

                    // Update standings (loser)
                    db.ExecuteQuery($@"UPDATE Standings SET 
                        Played = Played + 1, 
                        Lost = Lost + 1 
                        WHERE TeamId = {loserId}");

                    Console.WriteLine("\n✅ Match result updated successfully!");
                }
                else
                {
                    Console.WriteLine("❌ Match not found!");
                }
            }
            Console.ReadKey();
        }

        public static void ViewSchedule(DbHelper db)
        {
            Console.Clear();
            Console.WriteLine("\n📅 MATCH SCHEDULE");
            Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━");

            DataTable dt = db.GetData(@"
                SELECT CONVERT(NVARCHAR, m.MatchDate, 103) as Date, 
                       DATENAME(WEEKDAY, m.MatchDate) as Day,
                       t1.TeamName as Team1, t2.TeamName as Team2,
                       m.Venue, m.Status
                FROM Matches m
                JOIN Teams t1 ON m.Team1Id = t1.TeamId
                JOIN Teams t2 ON m.Team2Id = t2.TeamId
                ORDER BY m.MatchDate");

            if (dt.Rows.Count == 0)
            {
                Console.WriteLine("No matches scheduled!");
            }
            else
            {
                Console.WriteLine($"{"Date",-12} {"Day",-10} {"Team 1",-18} {"Team 2",-18} {"Venue",-15} {"Status",-10}");
                Console.WriteLine(new string('-', 90));
                foreach (DataRow row in dt.Rows)
                {
                    Console.WriteLine($"{row["Date"],-12} {row["Day"],-10} {row["Team1"],-18} {row["Team2"],-18} {row["Venue"],-15} {row["Status"],-10}");
                }
            }

            Console.WriteLine($"\nTotal Matches: {dt.Rows.Count}");
            Console.WriteLine("\nPress any key...");
            Console.ReadKey();
        }
    }
}