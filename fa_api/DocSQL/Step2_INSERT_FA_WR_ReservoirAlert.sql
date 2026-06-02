-- 插入測試資料到 FA_WR_ReservoirAlert
INSERT INTO [FA_WR_ReservoirAlert] 
    ([ReservoirName], [LowLevelPercentage], [MiddleLevelPercentage], [Longitude], [Latitude], [CreateUserNo])
VALUES 
    (N'石門水庫', 30.00, 50.00, 121.240000, 24.880000, 'SYSTEM'),
    (N'曾文水庫', 35.00, 55.00, 120.470000, 23.260000, 'SYSTEM'),
    (N'德基水庫', 28.00, 48.00, 121.120000, 24.220000, 'SYSTEM'),
    (N'鯉魚潭水庫', 32.00, 52.00, 120.860000, 24.300000, 'SYSTEM');