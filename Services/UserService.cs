using System.Data;
using CricketTournamentCLI.Database;

namespace CricketTournamentCLI.Services
{
    public static class UserService
    {
        public static void ManageUsers(DbHelper db)
        {
            bool back = false;
            while (!back)
            {
                Console.Clear();
                Console.WriteLine("\n👥 USER MANAGEMENT");
                Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━");
                Console.WriteLine("1. View All Users");
                Console.WriteLine("2. Add New User");
                Console.WriteLine("3. Delete User");
                Console.WriteLine("4. Back to Main Menu");
                Console.Write("\nChoice: ");

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1": ViewAllUsers(db); break;
                    case "2": AddUser(db); break;
                    case "3": DeleteUser(db); break;
                    case "4": back = true; break;
                    default: Console.WriteLine("Invalid option!"); Console.ReadKey(); break;
                }
            }
        }

        static void ViewAllUsers(DbHelper db)
        {
            Console.Clear();
            Console.WriteLine("\n📋 ALL USERS");
            Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━");

            DataTable dt = db.GetData("SELECT UserId, Username, FullName, Role FROM Users");

            if (dt.Rows.Count == 0)
            {
                Console.WriteLine("No users found!");
            }
            else
            {
                Console.WriteLine($"{"ID",-5} {"Username",-15} {"Full Name",-22} {"Role",-10}");
                Console.WriteLine(new string('-', 55));
                foreach (DataRow row in dt.Rows)
                {
                    Console.WriteLine($"{row["UserId"],-5} {row["Username"],-15} {row["FullName"],-22} {row["Role"],-10}");
                }
            }

            Console.WriteLine($"\nTotal Users: {dt.Rows.Count}");
            Console.WriteLine("\nPress any key...");
            Console.ReadKey();
        }

        static void AddUser(DbHelper db)
        {
            Console.Clear();
            Console.WriteLine("\n➕ ADD NEW USER");
            Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━");

            Console.Write("Username: ");
            string username = Console.ReadLine();
            Console.Write("Password: ");
            string password = Console.ReadLine();
            Console.Write("Full Name: ");
            string fullName = Console.ReadLine();
            Console.Write("Role (admin/user): ");
            string role = Console.ReadLine();

            // Check if username already exists
            DataTable check = db.GetData($"SELECT COUNT(*) FROM Users WHERE Username = '{username}'");
            if (Convert.ToInt32(check.Rows[0][0]) > 0)
            {
                Console.WriteLine("❌ Username already exists!");
                Console.ReadKey();
                return;
            }

            db.ExecuteQuery($@"
                INSERT INTO Users (Username, Password, FullName, Role)
                VALUES ('{username}', '{password}', '{fullName}', '{role}')");

            Console.WriteLine($"✅ User '{username}' added successfully!");
            Console.ReadKey();
        }

        static void DeleteUser(DbHelper db)
        {
            ViewAllUsers(db);
            Console.Write("\nEnter User ID to delete: ");
            if (int.TryParse(Console.ReadLine(), out int id))
            {
                // Check if trying to delete admin
                DataTable check = db.GetData($"SELECT Role FROM Users WHERE UserId = {id}");
                if (check.Rows.Count > 0 && check.Rows[0]["Role"].ToString() == "admin")
                {
                    // Check if this is the only admin
                    DataTable adminCount = db.GetData("SELECT COUNT(*) FROM Users WHERE Role = 'admin'");
                    if (Convert.ToInt32(adminCount.Rows[0][0]) <= 1)
                    {
                        Console.WriteLine("❌ Cannot delete the only admin user!");
                        Console.ReadKey();
                        return;
                    }
                }

                Console.Write($"Delete user {id}? (y/n): ");
                if (Console.ReadLine()?.ToLower() == "y")
                {
                    db.ExecuteQuery($"DELETE FROM Users WHERE UserId = {id}");
                    Console.WriteLine("✅ User deleted successfully!");
                }
            }
            Console.ReadKey();
        }
    }
}