IF OBJECT_ID(N'dbo.MatchPlayers', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.MatchPlayers
    (
        MatchPlayerId INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        MatchId INT NOT NULL,
        PlayerId INT NOT NULL,
        SeatNumber INT NOT NULL,
        FinishRank INT NOT NULL,
        Score INT NOT NULL,
        CardsPlayed INT NOT NULL,
        CardsDrawn INT NOT NULL,
        TurnCount INT NOT NULL,
        PlayerType NVARCHAR(40) NOT NULL,
        CreatedAtUtc DATETIMEOFFSET NOT NULL CONSTRAINT DF_MatchPlayers_CreatedAtUtc DEFAULT SYSUTCDATETIME()
    );
END
GO

IF OBJECT_ID(N'dbo.Matches', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Matches
    (
        MatchId INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        PlayedAtUtc DATETIMEOFFSET NOT NULL,
        WinnerPlayerId INT NULL,
        ScoringSystem NVARCHAR(40) NOT NULL,
        PlayerCount INT NOT NULL,
        Notes NVARCHAR(200) NULL
    );
END
GO

IF OBJECT_ID(N'dbo.Players', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Players
    (
        PlayerId INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        DisplayName NVARCHAR(80) NOT NULL,
        CreatedAtUtc DATETIMEOFFSET NOT NULL CONSTRAINT DF_Players_CreatedAtUtc DEFAULT SYSUTCDATETIME()
    );
END
GO

IF OBJECT_ID(N'dbo.Settings', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Settings
    (
        SettingKey NVARCHAR(100) NOT NULL PRIMARY KEY,
        SettingValue NVARCHAR(400) NOT NULL,
        UpdatedAtUtc DATETIMEOFFSET NOT NULL CONSTRAINT DF_Settings_UpdatedAtUtc DEFAULT SYSUTCDATETIME()
    );
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UX_Players_DisplayName' AND object_id = OBJECT_ID(N'dbo.Players'))
BEGIN
    CREATE UNIQUE INDEX UX_Players_DisplayName ON dbo.Players(DisplayName);
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_MatchPlayers_Matches')
BEGIN
    ALTER TABLE dbo.MatchPlayers
        ADD CONSTRAINT FK_MatchPlayers_Matches FOREIGN KEY (MatchId) REFERENCES dbo.Matches(MatchId);
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_MatchPlayers_Players')
BEGIN
    ALTER TABLE dbo.MatchPlayers
        ADD CONSTRAINT FK_MatchPlayers_Players FOREIGN KEY (PlayerId) REFERENCES dbo.Players(PlayerId);
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_Matches_WinnerPlayer')
BEGIN
    ALTER TABLE dbo.Matches
        ADD CONSTRAINT FK_Matches_WinnerPlayer FOREIGN KEY (WinnerPlayerId) REFERENCES dbo.Players(PlayerId);
END
GO
