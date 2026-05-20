-- ============================================================
-- Step1_Create_WaterSignal.sql
-- 建立 FA_WR_SignalLevel 資料表（水情燈號紀錄）
-- ============================================================
USE WMS_DB;
GO

-- ========== 刪除舊的資料表（如果需要重建）==========
-- IF EXISTS (SELECT 1 FROM sys.objects WHERE name = 'FA_WR_SignalLevel' AND type = 'U')
-- BEGIN
--     DROP TABLE FA_WR_SignalLevel;
--     PRINT 'FA_WR_SignalLevel 資料表已刪除';
-- END
-- GO

-- ========== 建立 FA_WR_SignalLevel 資料表 ==========
IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE name = 'FA_WR_SignalLevel' AND type = 'U')
BEGIN
    CREATE TABLE FA_WR_SignalLevel (
        
        Id                    INT             IDENTITY(1,1) PRIMARY KEY,  -- 主鍵，自動遞增
        
        -- ========== 水情燈號 ==========
        SignalLevel           NVARCHAR(10)    NOT NULL,                   -- 水情燈號：綠燈、黃燈、橙燈、紅燈
        SeverityCode          NVARCHAR(20)    NULL,                       -- 嚴重程度代碼：Minor, Moderate, Severe, Extreme
        
        -- ========== 地區資訊 ==========
        AreaDesc              NVARCHAR(100)   NULL,                       -- 受影響地區描述（例如：新竹縣、台中市）
        County                NVARCHAR(50)    NULL,                       -- 縣市
        
        -- ========== 警示資訊 ==========
        AlertIdentifier       NVARCHAR(100)   NULL,                       -- 警示識別碼（例如：WRA_Drought_20260427193326）
        Headline              NVARCHAR(500)   NULL,                       -- 警示標題
        Description           NVARCHAR(MAX)   NULL,                       -- 詳細說明
        
        -- ========== 時間欄位 ==========
        EffectiveTime         DATETIME        NULL,                       -- 警示生效時間
        ExpiresTime           DATETIME        NULL,                       -- 警示過期時間
        RecordTime            DATETIME        NOT NULL DEFAULT GETDATE(), -- 記錄建立時間
        UpdateTime            DATETIME        NULL,                       -- 最後更新時間
        
        -- ========== 資料來源 ==========
        DataSource            NVARCHAR(50)    NULL,                       -- 資料來源（例如：NCDR、WRA）
        
        -- ========== 基本欄位 ==========
        IsActive              BIT             NOT NULL DEFAULT 1,         -- 是否啟用
        Remarks               NVARCHAR(500)   NULL,                       -- 備註
        
        -- ========== 約束 ==========
        CONSTRAINT CK_FA_WR_SignalLevel_SignalLevel
            CHECK (SignalLevel IN (N'綠燈', N'黃燈', N'橙燈', N'紅燈', N'藍燈'))
    );
    
    -- ========== 建立索引 ==========
    CREATE INDEX IX_FA_WR_SignalLevel_SignalLevel ON FA_WR_SignalLevel(SignalLevel);
    CREATE INDEX IX_FA_WR_SignalLevel_County ON FA_WR_SignalLevel(County);
    CREATE INDEX IX_FA_WR_SignalLevel_RecordTime ON FA_WR_SignalLevel(RecordTime);
    CREATE INDEX IX_FA_WR_SignalLevel_EffectiveTime ON FA_WR_SignalLevel(EffectiveTime);
    CREATE INDEX IX_FA_WR_SignalLevel_AlertIdentifier ON FA_WR_SignalLevel(AlertIdentifier);
    
    PRINT '✅ FA_WR_SignalLevel 資料表已建立（完整版 - 水情燈號記錄）';
END
ELSE
BEGIN
    PRINT '⚠️ FA_WR_SignalLevel 資料表已存在';
END
GO

-- ========== 顯示資料表結構 ==========
EXEC sp_help 'FA_WR_SignalLevel';
GO
