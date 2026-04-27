using CricketTournamentCLI.Database;
using CricketTournamentCLI.Services;

namespace CricketTournamentCLI
{
    class Program
    {
        private static DbHelper? db;
        private static bool isLoggedIn = false;
        private static string currentUserRole = "";
        private static string currentUserName = "";

        static void Main(string[] args)
        {
            Console.Title = "Cricket Tournament Manager";
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            // ─── Docker ya LocalDB? ──────────────────────────────────────
            // Agar USE_DOCKER=true environment variable hai toh Docker SQL Server use karo
            string useDocker = Environment.GetEnvironmentVariable("USE_DOCKER") ?? "false";
            string dbServer  = Environment.GetEnvironmentVariable("DB_SERVER")  ?? "sqlserver";
            string dbPass    = Environment.GetEnvironmentVariable("DB_PASSWORD") ?? "Cricket@123";

            if (useDocker.ToLower() == "true")
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write("[INFO] ");
                Console.ResetColor();
                Console.WriteLine($"Docker mode: connecting to SQL Server at '{dbServer}'...");
                db = new DbHelper(dbServer, dbPass);
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write("[INFO] ");
                Console.ResetColor();
                Console.WriteLine("LocalDB mode: connecting to (localdb)\\MSSQLLocalDB...");
                db = new DbHelper();
            }
            // ─────────────────────────────────────────────────────────────

            // Connection test karo
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("[INFO] ");
            Console.ResetColor();
            Console.WriteLine("Checking database connection...");

            // Docker mein SQL Server start hone mein time lagta hai - retry karo
            int retries = 5;
            bool connected = false;
            while (retries > 0 && !connected)
            {
                connected = db.TestConnection();
                if (!connected)
                {
                    retries--;
                    if (retries > 0)
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.Write("[WAIT] ");
                        Console.ResetColor();
                        Console.WriteLine($"Retrying connection... ({retries} attempts left)");
                        Thread.Sleep(3000); // 3 second wait
                    }
                }
            }

            if (!connected)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("[ERROR] ");
                Console.ResetColor();
                Console.WriteLine("Database connection failed!");
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("[HELP]  ");
                Console.ResetColor();
                if (useDocker.ToLower() == "true")
                    Console.WriteLine("Docker mode: Make sure 'docker-compose up' is running.");
                else
                    Console.WriteLine("LocalDB mode: Make sure SQL Server LocalDB is installed.");
                Console.WriteLine("\nPress any key to exit...");
                Console.ReadKey();
                return;
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("[OK]    ");
            Console.ResetColor();
            Console.WriteLine("Database connected successfully!");
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();

            ShowWelcomeScreen();
        }

        // ══════════════════════════════════════════════════════════════════
        //  WELCOME SCREEN
        // ══════════════════════════════════════════════════════════════════
        static void ShowWelcomeScreen()
        {
            bool exit = false;

            while (!exit && !isLoggedIn)
            {
                Console.Clear();

                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine("+" + new string('-', 70) + "+");

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("|" + new string(' ', 70) + "|");
                string title = ">> CRICKET TOURNAMENT MANAGEMENT SYSTEM <<";
                Console.WriteLine("|" + Centre(title, 70) + "|");
                Console.WriteLine("|" + new string(' ', 70) + "|");

                Console.ForegroundColor = ConsoleColor.Cyan;
                string subtitle = "The Ultimate Cricket Manager";
                Console.WriteLine("|" + Centre(subtitle, 70) + "|");
                Console.WriteLine("|" + new string(' ', 70) + "|");

                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine("+" + new string('-', 70) + "+");

                Console.ForegroundColor = ConsoleColor.Yellow;
                string welcome = "WELCOME TO THE CRICKET ARENA";
                Console.WriteLine("|" + Centre(welcome, 70) + "|");
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine("+" + new string('-', 70) + "+");
                Console.ResetColor();

                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("|" + new string(' ', 70) + "|");
                Console.WriteLine("|" + new string(' ', 15) + "+----------------------------------------+" + new string(' ', 15) + "|");
                Console.WriteLine("|" + new string(' ', 15) + "|                                        |" + new string(' ', 15) + "|");

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("|" + new string(' ', 15) + "|     [1] ADMIN LOGIN                    |" + new string(' ', 15) + "|");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("|" + new string(' ', 15) + "|     [2] CONTINUE AS GUEST              |" + new string(' ', 15) + "|");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("|" + new string(' ', 15) + "|     [0] EXIT                           |" + new string(' ', 15) + "|");

                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("|" + new string(' ', 15) + "|                                        |" + new string(' ', 15) + "|");
                Console.WriteLine("|" + new string(' ', 15) + "+----------------------------------------+" + new string(' ', 15) + "|");
                Console.WriteLine("|" + new string(' ', 70) + "|");

                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine("+" + new string('-', 70) + "+");
                Console.ResetColor();

                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine("\n" + new string(' ', 22) + "Developed with Passion for Cricket");
                Console.ResetColor();

                Console.Write("\n" + new string(' ', 28) + "Enter your choice: ");
                string? option = Console.ReadLine();

                switch (option)
                {
                    case "1":
                        ShowAdminLogin();
                        break;
                    case "2":
                        currentUserRole = "viewer";
                        currentUserName = "Guest";
                        isLoggedIn = true;
                        RunUserDashboard();
                        break;
                    case "0":
                        exit = true;
                        Console.Clear();
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("\n\n" + new string(' ', 22) + "Thanks for using Cricket Tournament Manager!");
                        Console.WriteLine(new string(' ', 28) + "See you next time! 🏏");
                        Console.ResetColor();
                        break;
                    default:
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("\n[ERROR] Invalid option! Press any key...");
                        Console.ResetColor();
                        Console.ReadKey();
                        break;
                }
            }
        }

        // ══════════════════════════════════════════════════════════════════
        //  ADMIN LOGIN
        // ══════════════════════════════════════════════════════════════════
        static void ShowAdminLogin()
        {
            Console.Clear();

            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("+" + new string('-', 50) + "+");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("|" + new string(' ', 50) + "|");
            string title = ">> ADMIN LOGIN PANEL <<";
            Console.WriteLine("|" + Centre(title, 50) + "|");
            Console.WriteLine("|" + new string(' ', 50) + "|");
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("+" + new string('-', 50) + "+");
            Console.ResetColor();

            Console.WriteLine();
            Console.Write("  Username: ");
            string username = Console.ReadLine() ?? "";

            Console.Write("  Password: ");
            // Password hidden dikhao
            string password = "";
            ConsoleKeyInfo key;
            while ((key = Console.ReadKey(true)).Key != ConsoleKey.Enter)
            {
                if (key.Key == ConsoleKey.Backspace && password.Length > 0)
                {
                    password = password[..^1];
                    Console.Write("\b \b");
                }
                else if (key.Key != ConsoleKey.Backspace)
                {
                    password += key.KeyChar;
                    Console.Write("*");
                }
            }
            Console.WriteLine();

            if (db == null) return;

            var (success, role, fullName, userId) = db.CheckLogin(username, password);

            if (success)
            {
                SessionManager.UserId   = userId;
                SessionManager.Username = username;
                SessionManager.FullName = fullName;
                SessionManager.Role     = role;

                currentUserRole = role;
                currentUserName = fullName;
                isLoggedIn      = true;

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"\n  ✅ Login successful! Welcome, {fullName}!");
                Console.ResetColor();
                Thread.Sleep(1000);
                RunAdminDashboard();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\n  ❌ Invalid credentials! Press any key...");
                Console.ResetColor();
                Console.ReadKey();
                isLoggedIn = false;
            }
        }

        // ══════════════════════════════════════════════════════════════════
        //  ADMIN DASHBOARD
        // ══════════════════════════════════════════════════════════════════
        static void RunAdminDashboard()
        {
            bool running = true;

            while (running && db != null)
            {
                Console.Clear();

                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine("+" + new string('-', 70) + "+");
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("|" + new string(' ', 70) + "|");
                string header = ">> ADMIN DASHBOARD <<";
                Console.WriteLine("|" + Centre(header, 70) + "|");
                Console.ForegroundColor = ConsoleColor.Cyan;
                string user = "Welcome, " + currentUserName + " (Admin)";
                Console.WriteLine("|" + Centre(user, 70) + "|");
                Console.WriteLine("|" + new string(' ', 70) + "|");
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine("+" + new string('-', 70) + "+");
                Console.ResetColor();

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine(@"
|" + new string(' ', 70) + @"|
|   +------------------------------------------------------------------+   |
|   |                         MAIN MENU                               |   |
|   +------------------------------------------------------------------+   |
|   |                                                                  |   |
|   |     1. MANAGE TEAMS              6. UPDATE MATCH RESULT         |   |
|   |     2. MANAGE PLAYERS            7. VIEW PLAYER STATS           |   |
|   |     3. MANAGE MATCHES            8. MANAGE USERS                |   |
|   |     4. VIEW SCHEDULE             9. TOURNAMENT SUMMARY          |   |
|   |     5. VIEW STANDINGS            0. LOGOUT / EXIT               |   |
|   |                                                                  |   |
|   +------------------------------------------------------------------+   |");
                Console.ResetColor();

                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine("+" + new string('-', 70) + "+");
                Console.ResetColor();

                Console.ForegroundColor = ConsoleColor.White;
                Console.Write("\n" + new string(' ', 28) + "Enter your choice: ");
                Console.ResetColor();
                string? choice = Console.ReadLine();

                switch (choice)
                {
                    case "1": TeamService.ManageTeams(db);            break;
                    case "2": PlayerService.ManagePlayers(db);        break;
                    case "3": MatchService.ManageMatches(db);         break;
                    case "4": MatchService.ViewSchedule(db);          break;
                    case "5": StandingsService.ViewStandings(db);     break;
                    case "6": MatchService.UpdateMatchResult(db);     break;
                    case "7": PlayerService.ViewPlayerStats(db);      break;
                    case "8": UserService.ManageUsers(db);            break;
                    case "9": TournamentSummaryService.ShowSummary(db); break;
                    case "0":
                        Logout();
                        running = false;
                        ShowWelcomeScreen();
                        return;
                    default:
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("\n[ERROR] Invalid option! Press any key...");
                        Console.ResetColor();
                        Console.ReadKey();
                        break;
                }
            }
        }

        // ══════════════════════════════════════════════════════════════════
        //  GUEST / VIEWER DASHBOARD
        // ══════════════════════════════════════════════════════════════════
        static void RunUserDashboard()
        {
            bool running = true;

            while (running && db != null)
            {
                Console.Clear();

                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine("+" + new string('-', 70) + "+");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("|" + new string(' ', 70) + "|");
                string header = ">> VIEWER DASHBOARD <<";
                Console.WriteLine("|" + Centre(header, 70) + "|");
                Console.ForegroundColor = ConsoleColor.Cyan;
                string user = "Welcome, " + currentUserName + " (Read Only)";
                Console.WriteLine("|" + Centre(user, 70) + "|");
                Console.WriteLine("|" + new string(' ', 70) + "|");
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine("+" + new string('-', 70) + "+");
                Console.ResetColor();

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine(@"
|" + new string(' ', 70) + @"|
|   +------------------------------------------------------------------+   |
|   |                         VIEWER MENU                             |   |
|   +------------------------------------------------------------------+   |
|   |                                                                  |   |
|   |     1. VIEW ALL TEAMS         4. VIEW STANDINGS                  |   |
|   |     2. VIEW ALL PLAYERS       5. VIEW PLAYER STATS               |   |
|   |     3. VIEW SCHEDULE          6. TOURNAMENT SUMMARY              |   |
|   |                               7. SEARCH PLAYER                  |   |
|   |                               8. SWITCH TO ADMIN / EXIT         |   |
|   |                                                                  |   |
|   +------------------------------------------------------------------+   |");
                Console.ResetColor();

                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine("+" + new string('-', 70) + "+");
                Console.ResetColor();

                Console.ForegroundColor = ConsoleColor.White;
                Console.Write("\n" + new string(' ', 28) + "Enter your choice: ");
                Console.ResetColor();
                string? choice = Console.ReadLine();

                switch (choice)
                {
                    case "1": TeamService.ViewAllTeams(db);              break;
                    case "2": PlayerService.ViewAllPlayers(db);          break;
                    case "3": MatchService.ViewSchedule(db);             break;
                    case "4": StandingsService.ViewStandings(db);        break;
                    case "5": PlayerService.ViewPlayerStats(db);         break;
                    case "6": TournamentSummaryService.ShowSummary(db);  break;
                    case "7": PlayerService.SearchPlayer(db);            break;
                    case "8":
                        Logout();
                        running = false;
                        ShowWelcomeScreen();
                        return;
                    default:
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("\n[ERROR] Invalid option! Press any key...");
                        Console.ResetColor();
                        Console.ReadKey();
                        break;
                }
            }
        }

        // ══════════════════════════════════════════════════════════════════
        //  HELPERS
        // ══════════════════════════════════════════════════════════════════

        // Text ko center karne ka helper
        static string Centre(string text, int width)
        {
            int total = width - text.Length;
            int left  = total / 2;
            int right = total - left;
            return new string(' ', left) + text + new string(' ', right);
        }

        static void Logout()
        {
            SessionManager.Clear();
            isLoggedIn      = false;
            currentUserRole = "";
            currentUserName = "";
        }
    }
}
