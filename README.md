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


🚀 Quick Start (Docker - Recommended)
Use this method to run the entire environment (App + SQL Server) without installing anything locally.

1. Prerequisites
Docker Desktop installed and running.

2. Run the App
Navigate to the project root in your terminal and run:

Bash
docker-compose up --build


Note: The first run might take a few minutes to download the SQL Server image.

3. Attach to Interface
Once the containers are up, open a new terminal and run:

Bash
docker attach cricket_app

🖥️ Local Setup (Normal .NET Run)
1. Prerequisites
.NET 8 SDK

SQL Server Express or LocalDB

2. Database Setup
Open SQL Server Object Explorer in Visual Studio.

Right-click on (localdb)\MSSQLLocalDB → New Query.

Paste the content of init.sql and press Execute (F5).

3. Execution
Bash
dotnet run

✨ New Features

🔍 Search Player: Find players by name (Player Management → Option 6).
🏆 Tournament Summary: New dashboard for stats (Admin → Option 9).
🔒 Password Masking: Admin login now hides characters with *.
🔁 Auto-Retry: App will retry DB connection 5 times if SQL is slow to start.
🔐 CredentialsRoleUsernamePasswordAdminadminadmin123GuestNo login required—🛠️
 Useful Commands
TaskCommandStop Dockerdocker-compose downReset Everythingdocker-compose down -vView Logsdocker-compose logs -fFix Emojis (Win)chcp 65001




