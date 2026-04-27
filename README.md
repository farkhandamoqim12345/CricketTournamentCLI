# 🏏 Cricket Tournament CLI

A robust Command Line Interface (CLI) application built with **.NET 8** and **SQL Server** to manage cricket tournaments, teams, players, and matches efficiently. Now fully containerized with **Docker**.

---

## 📁 Project Structure

```text
CricketTournamentCLI/
├── Program.cs                 # Main entry point (Menu Logic)
├── Database/
│   ├── DbHelper.cs            # DB connection & Retry Logic
│   └── SessionManager.cs      # Login session management
├── Services/
│   ├── TeamService.cs         # Team CRUD operations
│   ├── PlayerService.cs       # Player Management & Search
│   ├── MatchService.cs        # Match scheduling & results
│   ├── StandingsService.cs    # Points table logic
│   ├── UserService.cs         # Authentication logic
│   └── TournamentSummaryService.cs # Stats & Summary Reports
├── init.sql                   # Database Schema & Seed Data
├── Dockerfile                 # App Containerization
└── docker-compose.yml         # Multi-container orchestration
🚀 Quick Start (Docker - Recommended)Use this method to run the entire environment (App + SQL Server) without installing anything locally.1. PrerequisitesDocker Desktop installed and running.2. Run the AppNavigate to the project root in your terminal and run:Bashdocker-compose up --build
Note: The first run might take a few minutes to download the SQL Server image (~500MB).3. Attach to InterfaceOnce the containers are up, open a new terminal and run:Bashdocker attach cricket_app
🖥️ Local Setup (Normal .NET Run)1. Prerequisites.NET 8 SDKSQL Server Express or LocalDB2. Database SetupOpen SQL Server Object Explorer in Visual Studio.Right-click on (localdb)\MSSQLLocalDB → New Query.Paste the content of init.sql and press Execute (F5).Verify CricketDB is created.3. ExecutionBashdotnet run
✨ Key Features🔍 Advanced Search: Find players instantly by name.🏆 Tournament Summary: Comprehensive dashboard for match stats.🔒 Secure Auth: Parameterized SQL queries and password masking (***).🔁 Resilient Design: Auto-retry logic for database connectivity.🐳 Dockerized: Seamless deployment with managed SQL environment.🔐 CredentialsRoleUsernamePasswordAdminadminadmin123GuestNo login required—🛠️ Useful Docker CommandsCommandDescriptiondocker-compose downStop servicesdocker-compose down -vReset everything (Deletes DB data)docker-compose logs -fView real-time logs❗ TroubleshootingUnicode Issues: If emojis/symbols don't show in Windows CMD, run chcp 65001.Connection Errors: If using Docker, the app might retry while SQL Server starts. This is normal; just wait for the "Retry 5/5" to complete.
