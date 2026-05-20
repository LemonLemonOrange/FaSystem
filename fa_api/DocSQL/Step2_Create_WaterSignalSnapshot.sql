IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE name = 'FA_WR_SignalSnapshot' AND type = 'U')
BEGIN
    CREATE TABLE FA_WR_SignalSnapshot (
        Id                INT           IDENTITY(1,1) PRIMARY KEY,
        BatchId           NVARCHAR(36)  NOT NULL,
        AreaName          NVARCHAR(100) NOT NULL,
        SignalLevel       NVARCHAR(10)  NOT NULL,
        SupplyStatus      INT           NOT NULL,
        StatusDescription NVARCHAR(50)  NULL,
        RecordTime        DATETIME      NOT NULL DEFAULT GETDATE(),

        CONSTRAINT CK_FA_WR_SignalSnapshot_SignalLevel
            CHECK (SignalLevel IN (N'綠燈', N'黃燈', N'橙燈', N'紅燈'))
    );

    CREATE INDEX IX_FA_WR_SignalSnapshot_BatchId    ON FA_WR_SignalSnapshot(BatchId);
    CREATE INDEX IX_FA_WR_SignalSnapshot_RecordTime ON FA_WR_SignalSnapshot(RecordTime);
    CREATE INDEX IX_FA_WR_SignalSnapshot_AreaName   ON FA_WR_SignalSnapshot(AreaName);
END
