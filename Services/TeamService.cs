using System.Data;
using CricketTournamentCLI.Database;

namespace CricketTournamentCLI.Services
{
    public static class TeamService
    {
        public static void ManageTeams(DbHelper db)
        {
            bool back = false;
            while (!back)
            {
                Console.Clear();
                Console.WriteLine("\n🏏 TEAM MANAGEMENT");
                Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━");
                Console.WriteLine("1. View All Teams");
                Console.WriteLine("2. Add New Team");
                Console.WriteLine("3. Delete Team");
                Console.WriteLine("4. Back to Main Menu");
                Console.Write("\nChoice: ");

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        ViewAllTeams(db);
                        break;
                    case "2":
                        AddTeam(db);
                        break;
                    case "3":
                        DeleteTeam(db);
                        break;
                    case "4":
                        back = true;
                        break;
                    default:
                        Console.WriteLine("Invalid option! Press any key...");
                        Console.ReadKey();
                        break;
                }
            }
        }

        public static void ViewAllTeams(DbHelper db)
        {
            Console.Clear();
            Console.WriteLine("\n📋 ALL TEAMS");
            Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━");

            DataTable dt = db.GetData("SELECT TeamId, TeamName, Coach FROM Teams ORDER BY TeamName");

            if (dt.Rows.Count == 0)
            {
                Console.WriteLine("No teams found!");
            }
            else
            {
                Console.WriteLine($"{"ID",-5} {"Team Name",-25} {"Coach",-20}");
                Console.WriteLine(new string('-', 50));
                foreach (DataRow row in dt.Rows)
                {
                    Console.WriteLine($"{row["TeamId"],-5} {row["TeamName"],-25} {row["Coach"],-20}");
                }
            }

            Console.WriteLine($"\nTotal Teams: {dt.Rows.Count}");
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }

        static void AddTeam(DbHelper db)
        {
            Console.Clear();
            Console.WriteLine("\n➕ ADD NEW TEAM");
            Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━");

            Console.Write("Team Name: ");
            string teamName = Console.ReadLine();
            Console.Write("Coach Name: ");
            string coach = Console.ReadLine();

            if (string.IsNullOrEmpty(teamName))
            {
                Console.WriteLine("❌ Team name required!");
                Console.ReadKey();
                return;
            }

            db.ExecuteQuery($"INSERT INTO Teams (TeamName, Coach) VALUES ('{teamName}', '{coach}')");
            Console.WriteLine($"✅ Team '{teamName}' added!");
            Console.ReadKey();
        }

        static void DeleteTeam(DbHelper db)
        {
            ViewAllTeams(db);
            Console.Write("\nEnter Team ID to delete: ");
            if (int.TryParse(Console.ReadLine(), out int id))
            {
                Console.Write($"Delete team {id}? (y/n): ");
                if (Console.ReadLine()?.ToLower() == "y")
                {
                    db.ExecuteQuery($"DELETE FROM Standings WHERE TeamId = {id}");
                    db.ExecuteQuery($"DELETE FROM Players WHERE TeamId = {id}");
                    db.ExecuteQuery($"DELETE FROM Matches WHERE Team1Id = {id} OR Team2Id = {id}");
                    db.ExecuteQuery($"DELETE FROM Teams WHERE TeamId = {id}");
                    Console.WriteLine("✅ Team deleted!");
                }
            }
            Console.ReadKey();
        }
    }
}