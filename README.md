# 🏏 Cricket Tournament CLI — Setup Guide

---

## 📁 Project Structure

```
CricketTournamentCLI/
├── Program.cs                    ← Main entry point (UPDATED)
├── Database/
│   ├── DbHelper.cs               ← DB connection helper (UPDATED)
│   └── SessionManager.cs         ← Login session
├── Services/
│   ├── TeamService.cs
│   ├── PlayerService.cs          ← (UPDATED - Search Player added)
│   ├── MatchService.cs
│   ├── StandingsService.cs
│   ├── UserService.cs
│   └── TournamentSummaryService.cs  ← (NEW FILE)
├── init.sql                      ← Database setup SQL
├── Dockerfile                    ← (NEW)
└── docker-compose.yml            ← (NEW)
```

---

## 🖥️ Option A — LocalDB (Normal .NET Run)



### Step 1: Prerequisites
- Visual Studio 2022 or VS Code
- .NET 8 SDK
- SQL Server Express or LocalDB (Visual Studio )

### Step 2: Database Setup
1. Visual Studio  **SQL Server Object Explorer** open 
2. `(localdb)\MSSQLLocalDB` right-click → **New Query**
3. `init.sql`  paste and **Execute** (F5) 
4. Check 

### Step 3: Run the App
```bash
dotnet run
```

---

## 🐳 Option B — Docker 



### Step 1: Prerequisites
- Docker Desktop install  (Windows/Mac/Linux)
- `docker-compose` available 

### Step 2: Files Copy Karo
- `Dockerfile`
- `docker-compose.yml`
- `init.sql` (already existing)

### Step 3: Run
```bash
:
docker-compose up --build


### Step 4: App Use 
```bash

docker attach cricket_app
```

### Useful Commands
```bash

docker-compose down

docker-compose down -v


docker-compose logs -f

# Server: localhost,1433
# User: sa
# Password: Cricket@123
```

---

## ✨ New Features Added

| Feature | Where |
|---|---|
| 🔍 Search Player by Name | Player Management → Option 6 |
| 🏆 Tournament Summary | Admin/Viewer Dashboard → Option 9/6 |
| 🔁 Docker auto-retry | App retries DB connection 5 times |
| 🔒 Parameterized Queries | SQL injection protection |
| ⭐ Password masking | Admin login shows `*` instead of password |

---

## 🔐 Default Login

| Role | Username | Password |
|---|---|---|
| Admin | `admin` | `admin123` |
| Guest | (no login needed) | — |

---

## ❗ Troubleshooting

**Docker: App connects before SQL Server is ready**
> Normal hai! App 5 baar retry karega. Wait karo.

**LocalDB: Connection failed**
> Visual Studio mein SQL Server LocalDB Tools install hona chahiye. Check karo: `sqllocaldb info`

**Unicode/Emoji nahi dikh rahe**
> Run karo: `chcp 65001` (Windows CMD mein)
