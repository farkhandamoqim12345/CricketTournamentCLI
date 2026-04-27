using System.Data;
using Microsoft.Data.SqlClient;

namespace CricketTournamentCLI.Database
{
    public class DbHelper
    {
        private string _connectionString;

        // ── LocalDB ke liye (default - bina Docker) ───────────────────────
        public DbHelper()
        {
            _connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=CricketDB;Integrated Security=True;TrustServerCertificate=True;";
        }

        // ── Docker SQL Server ke liye ─────────────────────────────────────
        // server = "sqlserver" (docker-compose service name)
        // password = "Cricket@123"
        public DbHelper(string server, string password)
        {
            _connectionString = $"Server={server},1433;Database=CricketDB;User Id=sa;Password={password};TrustServerCertificate=True;Encrypt=False;";
        }

        // ── Simple INSERT / UPDATE / DELETE (purani style - bina params) ──
        public void ExecuteQuery(string query)
        {
            using SqlConnection conn = new SqlConnection(_connectionString);
            using SqlCommand cmd     = new SqlCommand(query, conn);
            conn.Open();
            cmd.ExecuteNonQuery();
        }

        // ── Parameterized INSERT / UPDATE / DELETE (SQL Injection safe) ───
        // Example:
        //   db.ExecuteQueryWithParams(
        //     "INSERT INTO Teams (TeamName, Coach) VALUES (@name, @coach)",
        //     new Dictionary<string,object> { ["@name"]="Pak XI", ["@coach"]="Waqar" }
        //   );
        public void ExecuteQueryWithParams(string query, Dictionary<string, object> parameters)
        {
            using SqlConnection conn = new SqlConnection(_connectionString);
            using SqlCommand cmd     = new SqlCommand(query, conn);

            foreach (var p in parameters)
                cmd.Parameters.AddWithValue(p.Key, p.Value ?? DBNull.Value);

            conn.Open();
            cmd.ExecuteNonQuery();
        }

        // ── SELECT queries ke liye ────────────────────────────────────────
        public DataTable GetData(string query)
        {
            DataTable dt = new DataTable();
            using SqlConnection conn  = new SqlConnection(_connectionString);
            using SqlDataAdapter da   = new SqlDataAdapter(query, conn);
            conn.Open();
            da.Fill(dt);
            return dt;
        }

        // ── Parameterized SELECT (future use ke liye) ─────────────────────
        public DataTable GetDataWithParams(string query, Dictionary<string, object> parameters)
        {
            DataTable dt = new DataTable();
            using SqlConnection conn = new SqlConnection(_connectionString);
            using SqlCommand cmd     = new SqlCommand(query, conn);

            foreach (var p in parameters)
                cmd.Parameters.AddWithValue(p.Key, p.Value ?? DBNull.Value);

            using SqlDataAdapter da = new SqlDataAdapter(cmd);
            conn.Open();
            da.Fill(dt);
            return dt;
        }

        // ── Login check karne ke liye (already parameterized - safe) ─────
        public (bool success, string role, string fullName, int userId) CheckLogin(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                return (false, "", "", 0);

            using SqlConnection conn = new SqlConnection(_connectionString);
            conn.Open();

            string query = @"SELECT UserId, Role, FullName 
                             FROM Users 
                             WHERE Username=@u AND Password=@p";

            using SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@u", username);
            cmd.Parameters.AddWithValue("@p", password);

            using SqlDataReader r = cmd.ExecuteReader();
            if (r.Read())
            {
                return (true,
                        r["Role"]?.ToString()     ?? "user",
                        r["FullName"]?.ToString() ?? "",
                        Convert.ToInt32(r["UserId"]));
            }

            return (false, "", "", 0);
        }

        // ── Database connection test ───────────────────────────────────────
        public bool TestConnection()
        {
            try
            {
                using SqlConnection conn = new SqlConnection(_connectionString);
                conn.Open();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  Connection error: {ex.Message}");
                return false;
            }
        }
    }
}
