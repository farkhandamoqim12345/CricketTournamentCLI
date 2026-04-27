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

> Yeh tab use karo jab Docker available nahi ho

### Step 1: Prerequisites
- Visual Studio 2022 ya VS Code
- .NET 8 SDK
- SQL Server Express ya LocalDB (Visual Studio ke saath aata hai)

### Step 2: Database Setup
1. Visual Studio mein **SQL Server Object Explorer** open karo
2. `(localdb)\MSSQLLocalDB` pe right-click → **New Query**
3. `init.sql` ka content paste karo aur **Execute** (F5) karo
4. Check karo ke `CricketDB` database ban gaya

### Step 3: Run the App
```bash
dotnet run
```

---

## 🐳 Option B — Docker (Sir ke liye)

> Poora environment Docker mein chalega — SQL Server bhi, App bhi

### Step 1: Prerequisites
- Docker Desktop install hona chahiye (Windows/Mac/Linux)
- `docker-compose` available hona chahiye

### Step 2: Files Copy Karo
Yeh files project root mein rakho:
- `Dockerfile`
- `docker-compose.yml`
- `init.sql` (already existing)

### Step 3: Run
```bash
# Terminal mein project folder mein jao, phir:
docker-compose up --build
```

Pehli baar thoda time lagega (SQL Server image download hogi ~500MB).

### Step 4: App Use Karo
```bash
# Dusre terminal mein:
docker attach cricket_app
```

### Useful Commands
```bash
# Stop karna
docker-compose down

# Sab kuch reset (database bhi)
docker-compose down -v

# Logs dekhna
docker-compose logs -f

# SQL Server se directly connect karna (optional)
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
