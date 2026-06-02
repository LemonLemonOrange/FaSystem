-- ============================================================
-- Step1_Create_FA_WR_ReservoirAlert.sql
-- 建立 FA_WR_ReservoirAlert 資料表（水庫警示紀錄）
-- ============================================================
USE WMS_DB;
GO

-- ========== 刪除舊的資料表（如果需要重建）==========
-- IF EXISTS (SELECT 1 FROM sys.objects WHERE name = 'FA_WR_ReservoirAlert' AND type = 'U')
-- BEGIN
--     DROP TABLE FA_WR_ReservoirAlert;
--     PRINT 'FA_WR_ReservoirAlert 資料表已刪除';
-- END
-- GO

-- ========== 建立 [FA_WR_ReservoirAlert] 資料表 ==========
CREATE TABLE [dbo].[FA_WR_ReservoirAlert] (
    [Id] [bigint] IDENTITY(1,1) NOT NULL,
    [ReservoirName] [nvarchar](20) NOT NULL, -- 水庫名稱
    [LowLevelPercentage] [decimal](5,2) NULL, -- 低水位百分比(%)
    [MiddleLevelPercentage] [decimal](5,2) NULL, -- 中水位差異百分比(%)
    [Longitude] [decimal](10,6) NULL,  -- 經度 (WGS84)
    [Latitude] [decimal](10,6) NULL,   -- 緯度 (WGS84)
    [CreateTime] [datetime2](7) NOT NULL
        CONSTRAINT [DF_FA_WR_ReservoirAlert_CreateTime] DEFAULT (SYSDATETIME()),
    [CreateUserNo] [varchar](20) NOT NULL,
    [UpdateTime] [datetime2](7) NULL,
    [UpdateUserNo] [varchar](20) NULL,

    CONSTRAINT [PK_FA_WR_ReservoirAlert] PRIMARY KEY CLUSTERED ([Id] ASC),

    CONSTRAINT [UQ_FA_WR_ReservoirAlert_ReservoirName] UNIQUE ([ReservoirName]),

    CONSTRAINT [CK_FA_WR_ReservoirAlert_LowLevelPercentage]
        CHECK ([LowLevelPercentage] IS NULL OR ([LowLevelPercentage] >= 0 AND [LowLevelPercentage] <= 100)),

    CONSTRAINT [CK_FA_WR_ReservoirAlert_MiddleLevelPercentage]
        CHECK ([MiddleLevelPercentage] IS NULL OR ([MiddleLevelPercentage] >= 0 AND [MiddleLevelPercentage] <= 100))
);
GO

-- ========== 顯示資料表結構 ==========
EXEC sp_help 'FA_WR_ReservoirAlert';
GO
