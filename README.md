# 🏏 Cricket Tournament CLI — Setup Guide

## 📁 Project Structure
CricketTournamentCLI/
├── Program.cs ← Main entry point (UPDATED)
├── Database/
│ ├── DbHelper.cs ← DB connection helper (UPDATED)
│ └── SessionManager.cs ← Login session
├── Services/
│ ├── TeamService.cs
│ ├── PlayerService.cs ← (UPDATED - Search Player added)
│ ├── MatchService.cs
│ ├── StandingsService.cs
│ ├── UserService.cs
│ └── TournamentSummaryService.cs ← (NEW FILE)
├── init.sql ← Database setup SQL
├── Dockerfile ← (NEW)
└── docker-compose.yml ← (NEW)

text

---

## 🖥️ Option A — LocalDB (Normal .NET Run)

> Use this when Docker is not available

### Step 1: Prerequisites
- Visual Studio 2022 or VS Code
- .NET 8 SDK
- SQL Server Express or LocalDB (comes with Visual Studio)

### Step 2: Database Setup
1. Open **SQL Server Object Explorer** in Visual Studio
2. Right-click on `(localdb)\MSSQLLocalDB` → **New Query**
3. Paste content of `init.sql` and press **Execute** (F5)
4. Verify that `CricketDB` database has been created

### Step 3: Run the App
```bash
dotnet run
🐳 Option B — Docker (Recommended)
Complete environment runs inside Docker — both SQL Server and the App

Step 1: Prerequisites
Docker Desktop installed (Windows/Mac/Linux)

docker-compose available

Step 2: Copy Files
Place these files in your project root:

Dockerfile

docker-compose.yml

init.sql (already exists)

Step 3: Run
bash
# Navigate to project folder in terminal, then:
docker-compose up --build
First time will take a while (SQL Server image download ~500MB).

Step 4: Use the App
bash
# In another terminal:
docker attach cricket_app
Useful Commands
Command	Description
docker-compose down	Stop everything
docker-compose down -v	Reset everything (including database)
docker-compose logs -f	View logs
Connect directly to SQL Server	Server: localhost,1433
User: sa
Password: Cricket@123
✨ New Features Added
Feature	Location
🔍 Search Player by Name	Player Management → Option 6
🏆 Tournament Summary	Admin/Viewer Dashboard → Option 9/6
🔁 Docker auto-retry	App retries DB connection 5 times
🔒 Parameterized Queries	SQL injection protection
⭐ Password masking	Admin login shows * instead of password
🔐 Default Login
Role	Username	Password
Admin	admin	admin123
Guest	(no login needed)	—
❗ Troubleshooting
Issue	Solution
Docker: App connects before SQL Server is ready	Normal! App will retry 5 times. Just wait.
LocalDB: Connection failed	Ensure SQL Server LocalDB Tools are installed in Visual Studio. Check with: sqllocaldb info
Unicode/Emoji not showing	Run: chcp 65001 (in Windows CMD)
🚀 Quick Start (One-liner for Docker)
bash
docker-compose up --build
text

---

## Ab karna yeh hai:

1. **README.md file open karo** apne project folder mein
2. **Saara content select karo** (Ctrl+A)
3. **Delete karo**
4. **Upar wala English version copy karo** (Ctrl+C)
5. **Paste karo** (Ctrl+V)
6. **Save karo** (Ctrl+S)

Phir commit aur push karo:

```bash
git add README.md
git commit -m "Updated README to English"
git push origin main
