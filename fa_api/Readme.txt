# 核心套件
Install-Package Microsoft.EntityFrameworkCore -Version 3.1.32

# 資料庫提供者（依需求選擇其一）
Install-Package Microsoft.EntityFrameworkCore.SqlServer -Version 3.1.32  # SQL Server
Install-Package Microsoft.EntityFrameworkCore.Tools -Version 3.1.32
Install-Package Microsoft.EntityFrameworkCore.Design -Version 3.1.32

# 產生 Entity Model 類別與 DbContext 類別
-OutputDir Models：產生的 Entity Model 類別（.cs 檔）輸出到 Models/ 資料夾。
-ContextDir Data：產生的 DbContext 類別輸出到 Data/ 資料夾（與 Model 分開放）。
-Context FaDbContext：指定產生的 DbContext 類別名稱為 FaDbContext，否則預設會用資料庫名稱命名。
-Force：強制覆蓋，若目標檔案已存在則直接蓋掉，不會詢問確認。
-DataAnnotations：用 Data Annotations（如 [Key]、[Required]、[MaxLength]）來描述欄位規則，而非全部寫在 OnModelCreating() 的 Fluent API 裡。
-Tables：只針對這兩張資料表產生程式碼，不加此參數則會產生資料庫內所有資料表。
Scaffold-DbContext "Name=ConnectionStrings:DefaultConnection" Microsoft.EntityFrameworkCore.SqlServer -OutputDir Models -ContextDir Data -Context FaDbContext -Force -DataAnnotations -Tables FA_WR_DroughtAlert,FA_WR_ReservoirAlert

# 水利署 api
https://fhy.wra.gov.tw/WraApi