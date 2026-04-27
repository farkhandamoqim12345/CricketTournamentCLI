using System.Data;
using CricketTournamentCLI.Database;

namespace CricketTournamentCLI.Services
{
    public static class StandingsService
    {
        public static void ViewStandings(DbHelper db)
        {
            Console.Clear();
            Console.WriteLine("\n📊 POINTS TABLE (STANDINGS)");
            Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");

            DataTable dt = db.GetData(@"
                SELECT ROW_NUMBER() OVER (ORDER BY s.Points DESC, s.Won DESC) as Pos,
                       t.TeamName, s.Played, s.Won, s.Lost, s.Points
                FROM Standings s
                JOIN Teams t ON s.TeamId = t.TeamId
                ORDER BY s.Points DESC, s.Won DESC");

            if (dt.Rows.Count == 0)
            {
                Console.WriteLine("\nNo standings data available!");
                Console.WriteLine("Schedule some matches first to generate standings.");
            }
            else
            {
                Console.WriteLine($"\n{"Pos",-5} {"Team",-22} {"Played",-8} {"Won",-6} {"Lost",-6} {"Points",-8}");
                Console.WriteLine(new string('-', 60));
                foreach (DataRow row in dt.Rows)
                {
                    Console.WriteLine($"{row["Pos"],-5} {row["TeamName"],-22} {row["Played"],-8} {row["Won"],-6} {row["Lost"],-6} {row["Points"],-8}");
                }

                // Show champion if any team has points
                if (dt.Rows.Count > 0)
                {
                    DataRow top = dt.Rows[0];
                    Console.WriteLine($"\n🏆 Current Leader: {top["TeamName"]} with {top["Points"]} points!");
                }
            }

            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }
    }
}