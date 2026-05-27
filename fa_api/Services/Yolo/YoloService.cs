using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using fa_api.Dtos.Yolo;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;

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

                // 4. 記錄 Debug 資訊（Requirement 5.4）
                _logger.LogDebug("YOLO 呼叫 - PythonPath: {PythonPath}, ScriptPath: {ScriptPath}, TempImage: {TempImage}",
                    _settings.PythonPath, _settings.ScriptPath, tempImagePath);

                // 5. 建立 ProcessStartInfo（Requirement 2.1）
                var psi = new ProcessStartInfo
                {
                    FileName = _settings.PythonPath,
                    Arguments = $"\"{_settings.ScriptPath}\" \"{tempImagePath}\"",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                // 6. 逾時控制與 Process 執行（Requirements 2.2, 2.4）
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
                            throw new TimeoutException("YOLO 腳本執行逾時");
                        }
                    }

                    if (process.ExitCode != 0)
                    {
                        var stderr = await process.StandardError.ReadToEndAsync();
                        throw new InvalidOperationException($"YOLO 腳本執行失敗：{stderr}");
                    }

                    // Task 3.4: 讀取 stdout、解析 JSON、過濾 confidence、記錄 LogInformation（Requirements 2.3, 3.1, 3.3, 5.3）
                    var stdout = await process.StandardOutput.ReadToEndAsync();
                    var detections = ParseDetections(stdout);
                    stopwatch.Stop();
                    var elapsedMs = (int)stopwatch.ElapsedMilliseconds;
                    _logger.LogInformation("YOLO 辨識完成 - 偵測到 {Count} 個物件，執行時間 {ElapsedMs} ms", detections.Count, elapsedMs);
                    return new YoloResponseDto
                    {
                        Detections = detections,
                        ElapsedMs = elapsedMs
                    };
                }
            }
            finally
            {
                // 6. 無論成功或失敗，刪除暫存檔案（Requirement 2.5）
                DeleteTempFile(tempImagePath);
            }
        }

        /// <summary>
        /// 將 Python 腳本輸出的 JSON 字串解析為 DetectionResultDto 清單。
        /// confidence 不在 [0.0, 1.0] 範圍內的項目將被過濾（Requirement 3.3）。
        /// </summary>
        private List<DetectionResultDto> ParseDetections(string json)
        {
            var result = new List<DetectionResultDto>();
            var array = JArray.Parse(json);
            foreach (var item in array)
            {
                var confidence = item["confidence"].Value<decimal>();
                if (confidence < 0.0m || confidence > 1.0m)
                    continue;
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
                    File.Delete(path);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "刪除暫存影像檔案失敗：{Path}", path);
            }
        }
    }
}
