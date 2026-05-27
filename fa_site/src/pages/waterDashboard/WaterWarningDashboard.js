import { useMemo } from "react";
import TaiwanMap from "./components/TaiwanMap";
import "./WaterWarningDashboard.css";
import { useDroughtAlert } from "libs/hooks/api/wrSituation/Ncdr";

/** severity → { colorClass, status } 對應表 */
const SEVERITY_MAP = {
  Minor: { colorClass: "green", status: "水情提醒" },
  Moderate: { colorClass: "yellow", status: "減壓供水" },
  Severe: { colorClass: "orange", status: "減量供水" },
  Extreme: { colorClass: "red", status: "分區供水或定點供水" },
};

const WaterWarningDashboard = () => {
  const { data: alert, isLoading } = useDroughtAlert();
  console.log("alert", alert);

  const publishDate = useMemo(() => {
    if (!alert?.info?.effective) return "---";
    const date = new Date(alert.info.effective);
    if (isNaN(date.getTime())) return alert.info.effective;
    const twYear = date.getFullYear() - 1911;
    const month = date.getMonth() + 1;
    const day = date.getDate();
    return `${twYear}年${month}月${day}日`;
  }, [alert]);

  const warnings = useMemo(() => {
    if (!alert?.info) return [];

    const { severity, area } = alert.info;
    const severityInfo = SEVERITY_MAP[severity] || SEVERITY_MAP.Minor;

    // 每個受影響區域都有相同的 severity 狀態
    return (area || []).map(a => ({
      region: a.areaDesc, // 例如「新竹縣」、「台中市」
      ...severityInfo,
    }));
  }, [alert]);

  if (isLoading) {
    return (
      <div style={{ display: "flex", justifyContent: "center", alignItems: "center", minHeight: "600px" }}>
        <h2 style={{ color: "#0abcce" }}>資料載入中...</h2>
      </div>
    );
  }

  return (
    <div className="warning-dashboard-wrapper">
      {/* 左側地圖區 */}
      <div className="warning-map-section">
        <TaiwanMap warnings={warnings} />
      </div>

      {/* 右側資訊欄 */}
      <div className="warning-info-section">
        <div className="legend-panel">
          <div className="legend-item">
            <span className="dot green"></span>
            <span className="legend-text">水情提醒</span>
          </div>
          <div className="legend-item">
            <span className="dot yellow"></span>
            <span className="legend-text">減壓供水</span>
          </div>
          <div className="legend-item">
            <span className="dot orange"></span>
            <span className="legend-text">減量供水</span>
          </div>
          <div className="legend-item">
            <span className="dot red"></span>
            <span className="legend-text">分區供水或定點供水</span>
          </div>
        </div>

        <div className="date-panel">
          <div className="date-label">最新水情燈號公告日期</div>
          <div className="date-value">{publishDate}</div>
        </div>

        <div className="status-list-panel">
          {warnings.length === 0 ? (
            <div style={{ color: "#555", textAlign: "center", padding: "20px 0" }}>目前無水情警戒</div>
          ) : (
            warnings.map((w, idx) => (
              <div key={idx} className="status-item">
                <span className="region-name">{w.region}</span>
                <span className={`dot ${w.colorClass}`}></span>
                <span className="status-text">{w.status}</span>
              </div>
            ))
          )}
        </div>

        <div className="download-panel">
          {alert?.info ? (
            <a
              href={"https://www.wra.gov.tw/EarlyWarning.aspx?n=18804&sms=0"}
              target="_blank"
              rel="noreferrer"
              className="download-btn"
            >
              旱災警報檔下載
            </a>
          ) : (
            <button className="download-btn" disabled>旱災警報檔下載</button>
          )}
        </div>
      </div>
    </div>
  );
};

export default WaterWarningDashboard;