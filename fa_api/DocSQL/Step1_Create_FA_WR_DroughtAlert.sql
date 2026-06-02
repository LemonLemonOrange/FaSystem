-- ============================================================
-- Step1_Create_FA_WR_DroughtAlert.sql
-- 建立 FA_WR_DroughtAlert 資料表（乾旱警示紀錄）
-- ============================================================
USE WMS_DB;
GO

-- ========== 刪除舊的資料表（如果需要重建）==========
-- IF EXISTS (SELECT 1 FROM sys.objects WHERE name = 'FA_WR_DroughtAlert' AND type = 'U')
-- BEGIN
--     DROP TABLE FA_WR_DroughtAlert;
--     PRINT 'FA_WR_DroughtAlert 資料表已刪除';
-- END
-- GO

-- ========== 建立 FA_WR_DroughtAlert 資料表 ==========
CREATE TABLE [dbo].[FA_WR_DroughtAlert] (
    [Id] [bigint] IDENTITY(1,1) NOT NULL,
    [AreaName] [nvarchar](20) NOT NULL, -- 區域名稱
    [Severity] [varchar](10) NULL, -- 警示嚴重程度
    [CreateTime] [datetime2](7) NOT NULL
        CONSTRAINT [DF_FA_WR_DroughtAlert_CreateTime] DEFAULT (SYSDATETIME()),
    [CreateUserNo] [varchar](20) NOT NULL,
    [UpdateTime] [datetime2](7) NULL,
    [UpdateUserNo] [varchar](20) NULL,

    CONSTRAINT [PK_FA_WR_DroughtAlert] PRIMARY KEY CLUSTERED ([Id] ASC),

    CONSTRAINT [UQ_FA_WR_DroughtAlert_AreaName] UNIQUE ([AreaName]),

    CONSTRAINT [CK_FA_WR_DroughtAlert_Severity]
        CHECK ([Severity] IN ('Minor', 'Moderate', 'Severe', 'Extreme'))
);
GO

-- ========== 顯示資料表結構 ==========
EXEC sp_help 'FA_WR_DroughtAlert';
GO
