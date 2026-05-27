using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using fa_api.Dtos.Yolo;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Runtime.InteropServices;

namespace fa_api.Services.Yolo
{
    /// <summary>
    /// YOLO 影像辨識服務實作，透過 Process.Start 呼叫本機 Python 腳本執行推論。
    /// </summary>
    public class YoloService : IYoloService
    {
        private readonly ILogger<YoloService> _logger;
        private readonly YoloSettings _settings;

        public YoloService(ILogger<YoloService> logger, IOptions<YoloSettings> settings)
        {
            _logger = logger;
            _settings = settings.Value;
        }

        /// <inheritdoc />
        public async Task<YoloResponseDto> DetectAsync(byte[] imageBytes)
        {
            // 1. 驗證設定完整性（Requirement 4.2）
            if (string.IsNullOrEmpty(_settings.PythonPath) || string.IsNullOrEmpty(_settings.ScriptPath))
            {
                throw new InvalidOperationException("YoloSettings 設定不完整，請確認 PythonPath 與 ScriptPath");
            }

            // 計時器（Requirement 5.3）
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            // 2. 建立暫存檔案路徑（Requirements 2.1, 2.5）
            var tempImagePath = Path.GetTempFileName() + ".jpg";

            try
            {
                // 3. 將影像位元組寫入暫存檔案
                await File.WriteAllBytesAsync(tempImagePath, imageBytes);

                // 4. 取得 Python 執行檔路徑
                var pythonExe = GetPythonExecutable();

                // 5. 記錄 Debug 資訊（Requirement 5.4）
                _logger.LogDebug("YOLO 呼叫 - PythonPath: {PythonPath}, ScriptPath: {ScriptPath}, TempImage: {TempImage}",
                    pythonExe, _settings.ScriptPath, tempImagePath);

                // 6. 建立 ProcessStartInfo（Requirement 2.1）
                var psi = new ProcessStartInfo
                {
                    FileName = pythonExe,
                    Arguments = $"\"{_settings.ScriptPath}\" \"{tempImagePath}\"",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                // 7. 逾時控制與 Process 執行（Requirements 2.2, 2.4）
                using (var cts = new CancellationTokenSource(TimeSpan.FromSeconds(_settings.TimeoutSeconds)))
                using (var process = new Process { StartInfo = psi })
                {
                    process.Start();

                    // .NET Core 3.1 不支援 WaitForExitAsync(CancellationToken)，
                    // 改用 Task.WhenAny 搭配 CancellationToken 觸發的 delay task
                    var waitTask = Task.Run(() => process.WaitForExit());
                    var tcs = new TaskCompletionSource<bool>();
                    using (cts.Token.Register(() => tcs.TrySetResult(true)))
                    {
                        var completed = await Task.WhenAny(waitTask, tcs.Task);

                        if (completed != waitTask || cts.IsCancellationRequested)
                        {
                            try { process.Kill(); } catch { }
                            _logger.LogError("YOLO 腳本執行逾時 ({TimeoutSeconds} 秒)", _settings.TimeoutSeconds);
                            throw new TimeoutException($"YOLO 腳本執行逾時 ({_settings.TimeoutSeconds} 秒)");
                        }
                    }

                    // 8. 讀取 stdout 和 stderr
                    var stdout = await process.StandardOutput.ReadToEndAsync();
                    var stderr = await process.StandardError.ReadToEndAsync();

                    // 9. 記錄輸出以便調試
                    if (!string.IsNullOrEmpty(stderr))
                    {
                        _logger.LogWarning("Python 腳本 stderr 輸出: {StdErr}", stderr);
                    }

                    // 10. 檢查執行結果
                    if (process.ExitCode != 0)
                    {
                        _logger.LogError("YOLO 腳本執行失敗 (ExitCode: {ExitCode}), stderr: {StdErr}", 
                            process.ExitCode, stderr);
                        throw new InvalidOperationException($"YOLO 腳本執行失敗 (ExitCode: {process.ExitCode})\n錯誤訊息: {stderr}");
                    }

                    // 11. 記錄原始輸出以便調試
                    _logger.LogDebug("Python 腳本 stdout 輸出: {StdOut}", stdout);

                    // 12. 解析 JSON 輸出
                    List<DetectionResultDto> detections;
                    try
                    {
                        detections = ParseDetections(stdout);
                    }
                    catch (JsonException jsonEx)
                    {
                        _logger.LogError(jsonEx, "解析 YOLO 輸出 JSON 失敗。原始輸出: {StdOut}", stdout);
                        throw new InvalidOperationException($"解析 YOLO 輸出失敗: {jsonEx.Message}\n原始輸出: {stdout}");
                    }

                    stopwatch.Stop();
                    var elapsedMs = (int)stopwatch.ElapsedMilliseconds;
                    
                    _logger.LogInformation("YOLO 辨識完成 - 偵測到 {Count} 個物件，執行時間 {ElapsedMs} ms", 
                        detections.Count, elapsedMs);
                    
                    return new YoloResponseDto
                    {
                        Detections = detections,
                        ElapsedMs = elapsedMs
                    };
                }
            }
            catch (Exception ex) when (!(ex is TimeoutException || ex is InvalidOperationException))
            {
                // 捕捉其他未預期的錯誤
                _logger.LogError(ex, "YOLO 辨識過程發生未預期的錯誤");
                throw;
            }
            finally
            {
                // 13. 無論成功或失敗，刪除暫存檔案（Requirement 2.5）
                DeleteTempFile(tempImagePath);
            }
        }

        /// <summary>
        /// 取得 Python 執行檔路徑（自動處理相對路徑與絕對路徑）
        /// </summary>
        private string GetPythonExecutable()
        {
            var pythonPath = _settings.PythonPath;

            // 如果是 "py"，直接返回（Windows Python Launcher）
            if (pythonPath.Equals("py", System.StringComparison.OrdinalIgnoreCase))
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    var pyPath = @"C:\Windows\py.exe";
                    if (File.Exists(pyPath))
                    {
                        _logger.LogInformation("使用 Python Launcher: {Path}", pyPath);
                        return pyPath;
                    }
                    
                    _logger.LogWarning("找不到 py.exe，嘗試直接使用 'py' 命令");
                    return "py";
                }
            }

            // 如果是絕對路徑且檔案存在，直接使用
            if (Path.IsPathRooted(pythonPath) && File.Exists(pythonPath))
            {
                return pythonPath;
            }

            // 如果只寫 "python"，在 Windows 上需要找到完整路徑
            if (pythonPath.Equals("python", System.StringComparison.OrdinalIgnoreCase))
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    // 優先嘗試 py.exe
                    var pyPath = @"C:\Windows\py.exe";
                    if (File.Exists(pyPath))
                    {
                        _logger.LogInformation("自動切換到 Python Launcher: {Path}", pyPath);
                        return pyPath;
                    }

                    // 嘗試常見的 Python 安裝位置
                    var userName = System.Environment.UserName;
                    var commonPaths = new[]
                    {
                        @"C:\Python312\python.exe",
                        @"C:\Python311\python.exe",
                        @"C:\Python310\python.exe",
                        Path.Combine(@"C:\Users", userName, @"AppData\Local\Programs\Python\Python312\python.exe"),
                        Path.Combine(@"C:\Users", userName, @"AppData\Local\Programs\Python\Python311\python.exe"),
                        Path.Combine(@"C:\Users", userName, @"AppData\Local\Programs\Python\Python310\python.exe")
                    };

                    foreach (var path in commonPaths)
                    {
                        if (File.Exists(path))
                        {
                            _logger.LogInformation("自動偵測到 Python 路徑: {Path}", path);
                            return path;
                        }
                    }

                    var searchedPaths = string.Join(", ", commonPaths);
                    _logger.LogError("找不到 Python 執行檔。搜尋的路徑: {SearchedPaths}", searchedPaths);
                    throw new FileNotFoundException(
                        $"找不到 Python 執行檔，請在 appsettings.json 的 YoloSettings.PythonPath 中設定完整路徑。\n" +
                        $"建議使用 'py' 或執行 'where py' 命令來找到 Python Launcher 路徑。\n" +
                        $"已搜尋: {searchedPaths}");
                }
            }

            return pythonPath;
        }

        /// <summary>
        /// 將 Python 腳本輸出的 JSON 字串解析為 DetectionResultDto 清單。
        /// confidence 不在 [0.0, 1.0] 範圍內的項目將被過濾（Requirement 3.3）。
        /// </summary>
        private List<DetectionResultDto> ParseDetections(string json)
        {
            var result = new List<DetectionResultDto>();
            
            // 檢查空白或無效的 JSON
            if (string.IsNullOrWhiteSpace(json))
            {
                _logger.LogWarning("Python 腳本返回空白輸出");
                return result;
            }

            var array = JArray.Parse(json);
            
            foreach (var item in array)
            {
                try
                {
                    var confidence = item["confidence"].Value<decimal>();
                    
                    // 過濾不合理的 confidence 值
                    if (confidence < 0.0m || confidence > 1.0m)
                    {
                        _logger.LogWarning("跳過不合理的 confidence 值: {Confidence}", confidence);
                        continue;
                    }
                    
                    result.Add(new DetectionResultDto
                    {
                        Label = item["label"].Value<string>(),
                        Confidence = confidence,
                        X = item["x"].Value<int>(),
                        Y = item["y"].Value<int>(),
                        Width = item["width"].Value<int>(),
                        Height = item["height"].Value<int>()
                    });
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "解析單一偵測結果失敗，跳過此項目: {Item}", item.ToString());
                    continue;
                }
            }
            
            return result;
        }

        /// <summary>
        /// 刪除指定路徑的暫存影像檔案。刪除失敗時記錄警告，不拋出例外。
        /// </summary>
        private void DeleteTempFile(string path)
        {
            try
            {
                if (File.Exists(path))
                {
                    File.Delete(path);
                    _logger.LogDebug("已刪除暫存檔案: {Path}", path);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "刪除暫存影像檔案失敗：{Path}", path);
            }
        }
    }
}
