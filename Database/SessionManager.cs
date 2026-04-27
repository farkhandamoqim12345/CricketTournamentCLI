namespace CricketTournamentCLI.Database
{
    // Logged-in user ka data store karne ke liye
    public static class SessionManager
    {
        public static int UserId { get; set; }
        public static string Username { get; set; } = "";
        public static string FullName { get; set; } = "";
        public static string Role { get; set; } = "";

        public static bool IsAdmin => Role == "admin";

        public static void Clear()
        {
            UserId = 0;
            Username = "";
            FullName = "";
            Role = "";
        }
    }
}