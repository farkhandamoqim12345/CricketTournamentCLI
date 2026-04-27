-- CricketDB Database Setup
CREATE DATABASE CricketDB;
GO

USE CricketDB;
GO

-- Users Table
CREATE TABLE Users (
    UserId   INT IDENTITY(1,1) PRIMARY KEY,
    Username NVARCHAR(50) NOT NULL UNIQUE,
    Password NVARCHAR(100) NOT NULL,
    FullName NVARCHAR(100),
    Role NVARCHAR(20) DEFAULT 'user'
);

-- Teams Table
CREATE TABLE Teams (
    TeamId    INT IDENTITY(1,1) PRIMARY KEY,
    TeamName  NVARCHAR(100) NOT NULL,
    Coach     NVARCHAR(100)
);

-- Players Table
CREATE TABLE Players (
    PlayerId     INT IDENTITY(1,1) PRIMARY KEY,
    PlayerName   NVARCHAR(100) NOT NULL,
    Role         NVARCHAR(50),
    BattingStyle NVARCHAR(50),
    BowlingStyle NVARCHAR(50),
    TeamId       INT FOREIGN KEY REFERENCES Teams(TeamId),
    IsCaptain    BIT DEFAULT 0,
    IsWicketKeeper BIT DEFAULT 0
);

-- Matches Table
CREATE TABLE Matches (
    MatchId    INT IDENTITY(1,1) PRIMARY KEY,
    Team1Id    INT FOREIGN KEY REFERENCES Teams(TeamId),
    Team2Id    INT FOREIGN KEY REFERENCES Teams(TeamId),
    MatchDate  DATETIME,
    Venue      NVARCHAR(100),
    Status     NVARCHAR(20) DEFAULT 'Scheduled',
    Team1Score NVARCHAR(50),
    Team2Score NVARCHAR(50),
    WinnerId   INT FOREIGN KEY REFERENCES Teams(TeamId)
);

-- Standings Table
CREATE TABLE Standings (
    StandingId INT IDENTITY(1,1) PRIMARY KEY,
    TeamId     INT FOREIGN KEY REFERENCES Teams(TeamId),
    Played     INT DEFAULT 0,
    Won        INT DEFAULT 0,
    Lost       INT DEFAULT 0,
    Points     INT DEFAULT 0
);

-- PlayerStats Table
CREATE TABLE PlayerStats (
    StatsId     INT IDENTITY(1,1) PRIMARY KEY,
    PlayerId    INT FOREIGN KEY REFERENCES Players(PlayerId),
    Matches     INT DEFAULT 0,
    Runs        INT DEFAULT 0,
    Wickets     INT DEFAULT 0,
    Catches     INT DEFAULT 0,
    HighScore   INT DEFAULT 0,
    BestBowling NVARCHAR(20)
);

-- Default Users
INSERT INTO Users (Username, Password, FullName, Role)
VALUES ('admin', 'admin123', 'Administrator', 'admin');

INSERT INTO Users (Username, Password, FullName, Role)
VALUES ('user1', 'user123', 'Guest User', 'user');

-- Sample Team
INSERT INTO Teams (TeamName, Coach) VALUES ('Pakistan XI', 'Waqar Younis');
INSERT INTO Teams (TeamName, Coach) VALUES ('India XI', 'Rahul Dravid');

-- Sample Players
INSERT INTO Players (PlayerName, Role, BattingStyle, BowlingStyle, TeamId) 
VALUES ('Babar Azam', 'Batsman', 'Right Hand', 'N/A', 1);
INSERT INTO Players (PlayerName, Role, BattingStyle, BowlingStyle, TeamId) 
VALUES ('Shaheen Afridi', 'Bowler', 'Left Hand', 'Left Arm Fast', 1);