using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;
using fa_api.Dtos.Ncdr;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace fa_api.Services.Ncdr
{
    /// <summary>
    /// NCDR 民生示警公開資料平台 - 枯旱預警服務
    /// 使用 API 使用 CAP (Common Alerting Protocol) 標準格式
    /// 水利署 OpenAPI 枯旱警示系統
    /// </summary>
    public class NcdrDroughtService : INcdrDroughtService
    {
        private const string NCDR_DROUGHT_URL = "https://alerts.ncdr.nat.gov.tw/webapi/JSONAtomFeed.ashx?AlertType=2099";

        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<NcdrDroughtService> _logger;

        public NcdrDroughtService(IHttpClientFactory httpClientFactory, ILogger<NcdrDroughtService> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        /// <summary>
        /// 從 NCDR 警示訂閱頻道取得枯旱預警 CAP .cap 檔案內容
        /// 並解析 XML 成完整結構化資料（含 severity 與影響區域）
        /// </summary>
        /// <returns>最新一筆枯旱預警，若無警示則回傳 null</returns>
        public async Task<DroughtAlertDto> FetchDroughtAlertAsync()
        {
            try
            {
                var client = _httpClientFactory.CreateClient();

                // 1. 取得枯旱預警清單（AlertType=2099 已過濾只有枯旱預警）
                // API 回傳 JSON 格式
                _logger.LogInformation($"開始取得 NCDR 枯旱預警清單: {NCDR_DROUGHT_URL}");
                var feedRes = await client.GetStringAsync(NCDR_DROUGHT_URL);
                
                _logger.LogInformation($"成功取得資料，長度: {feedRes.Length} 字元");

                // 2. 解析 JSON
                JObject feedJson;
                try
                {
                    feedJson = JObject.Parse(feedRes);
                }
                catch (JsonException jsonEx)
                {
                    _logger.LogError(jsonEx, $"JSON 解析失敗。內容: {feedRes}");
                    throw new InvalidOperationException("無法解析 NCDR API 回傳的 JSON 內容", jsonEx);
                }

                var entries = feedJson["entry"] as JArray;

                if (entries == null || entries.Count == 0)
                {
                    _logger.LogInformation("目前無枯旱預警資料");
                    return null;
                }

                _logger.LogInformation($"找到 {entries.Count} 筆警示資料");

                // 3. 取得最新一筆（依 updated 排序）
                var latest = entries
                    .OrderByDescending(e => 
                    {
                        var updatedStr = e["updated"]?.ToString();
                        return DateTime.TryParse(updatedStr, out var date) ? date : DateTime.MinValue;
                    })
                    .FirstOrDefault();

                if (latest == null)
                {
                    _logger.LogWarning("無法找到有效的警示資料");
                    return null;
                }

                // 4. 取得 .cap XML 檔案連結
                var capUrl = latest["link"]?["@href"]?.ToString();

                if (string.IsNullOrEmpty(capUrl))
                {
                    _logger.LogWarning("無法取得 CAP 檔案連結");
                    return null;
                }

                _logger.LogInformation($"正在下載 CAP 檔案: {capUrl}");

                // 5. 下載並解析 CAP XML 檔案
                string capRes;
                try
                {
                    capRes = await client.GetStringAsync(capUrl);
                    _logger.LogInformation($"成功下載 CAP 檔案，長度: {capRes.Length} 字元");
                }
                catch (HttpRequestException httpEx)
                {
                    _logger.LogError(httpEx, $"下載 CAP 檔案失敗: {capUrl}");
                    throw new InvalidOperationException($"無法下載 CAP 檔案", httpEx);
                }

                // 6. 解析 CAP XML
                XDocument capXml;
                try
                {
                    capXml = XDocument.Parse(capRes);
                }
                catch (System.Xml.XmlException xmlEx)
                {
                    _logger.LogError(xmlEx, $"CAP XML 解析失敗。內容前 500 字元: {capRes.Substring(0, Math.Min(500, capRes.Length))}");
                    throw new InvalidOperationException("無法解析 CAP XML 內容", xmlEx);
                }

                // 7. 解析 CAP 內容（CAP 1.2 標準）
                var capNs = capXml.Root?.GetDefaultNamespace() ?? XNamespace.Get("urn:oasis:names:tc:emergency:cap:1.2");
                var alert = capXml.Root;

                if (alert == null)
                {
                    _logger.LogWarning("CAP XML 根元素為空");
                    return null;
                }

                var info = alert.Element(capNs + "info");
                var rawAreas = info?.Elements(capNs + "area");

                var areas = rawAreas?
                    .Select(a => new DroughtAreaDto
                    {
                        AreaDesc = a.Element(capNs + "areaDesc")?.Value
                    })
                    .ToList() ?? new List<DroughtAreaDto>();

                var droughtAlert = new DroughtAlertDto
                {
                    Identifier = alert.Element(capNs + "identifier")?.Value,
                    Sent = alert.Element(capNs + "sent")?.Value,
                    Status = alert.Element(capNs + "status")?.Value,
                    Info = new DroughtInfoDto
                    {
                        Severity = info?.Element(capNs + "severity")?.Value,
                        Headline = info?.Element(capNs + "headline")?.Value,
                        Description = info?.Element(capNs + "description")?.Value,
                        Effective = DateTime.Parse(info?.Element(capNs + "effective")?.Value),
                        Expires = DateTime.Parse(info?.Element(capNs + "expires")?.Value),
                        Area = areas
                    }
                };

                _logger.LogInformation($"✅ 成功解析枯旱預警: {droughtAlert.Identifier}");
                _logger.LogInformation($"   嚴重程度: {droughtAlert.Info?.Severity}");
                _logger.LogInformation($"   影響地區數量: {areas.Count}");
                
                return droughtAlert;
            }
            catch (HttpRequestException httpEx)
            {
                _logger.LogError(httpEx, "HTTP 請求失敗");
                throw new InvalidOperationException("無法連接到 NCDR API", httpEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "取得枯旱預警資料時發生錯誤");
                throw;
            }
        }
    }
}