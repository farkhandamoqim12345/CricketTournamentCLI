-- Database banao
CREATE DATABASE CricketDB;
GO

-- Database use karo
USE CricketDB;
GO

-- Users table
CREATE TABLE Users (
    UserId INT IDENTITY(1,1) PRIMARY KEY,
    Username NVARCHAR(50) NOT NULL UNIQUE,
    Password NVARCHAR(100) NOT NULL,
    FullName NVARCHAR(100),
    Role NVARCHAR(20) DEFAULT 'user'
);
GO

-- Teams table
CREATE TABLE Teams (
    TeamId INT IDENTITY(1,1) PRIMARY KEY,
    TeamName NVARCHAR(100) NOT NULL,
    Coach NVARCHAR(100)
);
GO

-- Players table
CREATE TABLE Players (
    PlayerId INT IDENTITY(1,1) PRIMARY KEY,
    PlayerName NVARCHAR(100) NOT NULL,
    Role NVARCHAR(50),
    BattingStyle NVARCHAR(50),
    BowlingStyle NVARCHAR(50),
    TeamId INT,
    IsCaptain BIT DEFAULT 0,
    IsWicketKeeper BIT DEFAULT 0
);
GO

-- Matches table
CREATE TABLE Matches (
    MatchId INT IDENTITY(1,1) PRIMARY KEY,
    Team1Id INT,
    Team2Id INT,
    MatchDate DATETIME,
    Venue NVARCHAR(100),
    Status NVARCHAR(20) DEFAULT 'Scheduled',
    Team1Score NVARCHAR(50),
    Team2Score NVARCHAR(50),
    WinnerId INT
);
GO

-- Standings table
CREATE TABLE Standings (
    StandingId INT IDENTITY(1,1) PRIMARY KEY,
    TeamId INT,
    Played INT DEFAULT 0,
    Won INT DEFAULT 0,
    Lost INT DEFAULT 0,
    Points INT DEFAULT 0
);
GO

-- PlayerStats table
CREATE TABLE PlayerStats (
    StatsId INT IDENTITY(1,1) PRIMARY KEY,
    PlayerId INT,
    Matches INT DEFAULT 0,
    Runs INT DEFAULT 0,
    Wickets INT DEFAULT 0,
    Catches INT DEFAULT 0,
    HighScore INT DEFAULT 0,
    BestBowling NVARCHAR(20)
);
GO

-- Admin user
INSERT INTO Users (Username, Password, FullName, Role)
VALUES ('admin', 'admin123', 'Administrator', 'admin');
GO

-- Normal user
INSERT INTO Users (Username, Password, FullName, Role)
VALUES ('user1', 'user123', 'Guest User', 'user');
GO

-- Sample teams
INSERT INTO Teams (TeamName, Coach) VALUES ('Pakistan XI', 'Waqar Younis');
INSERT INTO Teams (TeamName, Coach) VALUES ('India XI', 'Rahul Dravid');
GO

-- Check karo
SELECT '✅ DATABASE READY!' AS Status;
SELECT COUNT(*) AS TotalUsers FROM Users;
SELECT COUNT(*) AS TotalTeams FROM Teams;
GO