import React, { useState, useMemo, useRef, useEffect, useCallback } from "react";
import "./WaterDashboard.css";
import ReservoirCard from "./components/ReservoirCard";
import { ComposableMap, Geographies, Geography, Marker } from "react-simple-maps";
import { geoMercator } from "d3-geo";
import useReservoirDisplayList from "libs/hooks/api/wrSituation/WraGov/useReservoirDisplayList";
import useReservoirRealTimeInfo from "libs/hooks/api/wrSituation/WraGov/useReservoirRealTimeInfo";
import useReservoirStation from "libs/hooks/api/wrSituation/WraGov/useReservoirStation";
import { Spin, Button, Popconfirm, message } from "antd";

const GEO_URL = "/counties-10t.json";

// 卡片預設位移（可拖曳後儲存）
const DEFAULT_CARD_OFFSETS = {
  "寶山水庫": { dx: -525, dy: -152 },
  "寶山第二水庫": { dx: -575, dy: 10 },
  "翡翠水庫": { dx: 256, dy: 53 },
  "石門水庫": { dx: -263, dy: -103 },
  "曾文水庫": { dx: -296, dy: 65 },
  "蘭潭水庫": { dx: 412, dy: 199 },
  "烏山頭水庫": { dx: -321, dy: 121 },
  "德基水庫": { dx: 379, dy: 22 },
  "永和山水庫": { dx: -559, dy: 154 },
  "鯉魚潭水庫": { dx: -429, dy: 3 },
  "南化水庫": { dx: 319, dy: -128 },
  "仁義潭水庫": { dx: 224, dy: 258 },
};
// 與 react-simple-maps 的 geoMercator 參數一致
const PROJ_CENTER_LNG = 121;
const PROJ_CENTER_LAT = 23.8;
const PROJ_SCALE = 8000;

const readField = (obj, camelKey, pascalKey) => obj?.[camelKey] ?? obj?.[pascalKey];
const asArray = (value) => (Array.isArray(value) ? value : Array.isArray(value?.data) ? value.data : []);

function normalizeReservoir(item, realTimeInfos, stations) {
  const rawStationNo = readField(item, "stationNo", "StationNo");
  const name = readField(item, "reservoirName", "ReservoirName");
  const station = stations?.find((s) => {
    const stationNo = readField(s, "stationNo", "StationNo");
    const stationName = readField(s, "stationName", "StationName");
    return (rawStationNo && stationNo === rawStationNo) || stationName === name;
  });
  const stationNo = rawStationNo ?? readField(station, "stationNo", "StationNo");
  const longitude = readField(item, "longitude", "Longitude") ?? readField(station, "longitude", "Longitude");
  const latitude = readField(item, "latitude", "Latitude") ?? readField(station, "latitude", "Latitude");

  if (!name || longitude == null || latitude == null) {
    return null;
  }

  const realTime = stationNo
    ? realTimeInfos?.find((r) => readField(r, "stationNo", "StationNo") === stationNo)
    : realTimeInfos?.find((r) => readField(r, "stationName", "StationName") === name);
  const storage = readField(item, "storage", "Storage") ?? readField(station, "storage", "Storage");
  const effectiveStorage = readField(realTime, "effectiveStorage", "EffectiveStorage");
  const percentageOfStorage = readField(realTime, "percentageOfStorage", "PercentageOfStorage");
  const volume = effectiveStorage != null
    ? Math.round(Number(effectiveStorage)).toLocaleString()
    : storage != null
      ? Math.round(Number(storage)).toLocaleString()
      : "-";
  const percent = percentageOfStorage != null
    ? Number(Number(percentageOfStorage).toFixed(2))
    : 0;

  return {
    id: stationNo || name,
    stationNo,
    name,
    coords: [Number(longitude), Number(latitude)],
    volume,
    percent,
  };
}

const ReservoirDashboard = () => {
  const [hovered, setHovered] = useState(null);
  const [layoutMode, setLayoutMode] = useState("auto");
  const containerRef = useRef(null);
  const [containerSize, setContainerSize] = useState({ w: 900, h: 675 });

  const [dragOffsets, setDragOffsets] = useState(() => {
    try {
      const saved = JSON.parse(localStorage.getItem("reservoir-card-offsets"));
      return saved ?? DEFAULT_CARD_OFFSETS;
    } catch {
      return DEFAULT_CARD_OFFSETS;
    }
  });

  const draggingRef = useRef(null);

  const { data: displayList, isLoading: isDisplayListLoading } = useReservoirDisplayList();
  const { data: realTimeInfos, isLoading: isRealTimeLoading } = useReservoirRealTimeInfo();
  const { data: stations, isLoading: isStationLoading } = useReservoirStation();
  const mapWidth = Math.max(1, containerSize.w);
  const mapHeight = Math.max(1, containerSize.h);
  const mapProjection = useMemo(
    () =>
      geoMercator()
        .center([PROJ_CENTER_LNG, PROJ_CENTER_LAT])
        .scale(PROJ_SCALE)
        .translate([mapWidth / 2, mapHeight / 2]),
    [mapWidth, mapHeight]
  );

  useEffect(() => {
    if (!containerRef.current) return;
    const ro = new ResizeObserver((entries) => {
      for (const e of entries) {
        setContainerSize({ w: e.contentRect.width, h: e.contentRect.height });
      }
    });
    ro.observe(containerRef.current);
    return () => ro.disconnect();
  }, []);

  const handleCardMouseDown = useCallback((e, name) => {
    e.preventDefault();
    const cur = dragOffsets[name] ?? { dx: 0, dy: 0 };
    draggingRef.current = {
      name,
      startX: e.clientX,
      startY: e.clientY,
      baseDx: cur.dx,
      baseDy: cur.dy,
    };

    const onMove = (ev) => {
      const d = draggingRef.current;
      if (!d) return;
      setDragOffsets((prev) => ({
        ...prev,
        [d.name]: {
          dx: d.baseDx + ev.clientX - d.startX,
          dy: d.baseDy + ev.clientY - d.startY,
        },
      }));
    };

    const onUp = () => {
      draggingRef.current = null;
      window.removeEventListener("mousemove", onMove);
      window.removeEventListener("mouseup", onUp);
      setDragOffsets((prev) => {
        localStorage.setItem("reservoir-card-offsets", JSON.stringify(prev));
        return prev;
      });
    };

    window.addEventListener("mousemove", onMove);
    window.addEventListener("mouseup", onUp);
  }, [dragOffsets]);

  const handleReset = useCallback(() => {
    localStorage.removeItem("reservoir-card-offsets");
    setDragOffsets(DEFAULT_CARD_OFFSETS);
  }, []);

  const handleExport = useCallback(() => {
    const cardLines = Object.entries(dragOffsets)
      .map(([name, { dx, dy }]) => `  '${name}': { dx: ${Math.round(dx)}, dy: ${Math.round(dy)} },`)
      .join("\n");

    const text = `const DEFAULT_CARD_OFFSETS = {\n${cardLines}\n};`;
    navigator.clipboard.writeText(text).then(() => {
      message.success("已複製卡片位置到剪貼簿");
    });
  }, [dragOffsets]);

  const reservoirs = useMemo(() => {
    const list = asArray(displayList);
    if (list.length === 0) {
      return [];
    }

    return list.map((item) => normalizeReservoir(item, realTimeInfos, stations)).filter(Boolean);
  }, [displayList, realTimeInfos, stations]);

  const reservoirById = useMemo(
    () => Object.fromEntries(reservoirs.map((res) => [res.id, res])),
    [reservoirs]
  );

  const cardPositions = useMemo(() => {
    return reservoirs.map((res) => {
      const [anchorX, anchorY] = mapProjection(res.coords);
      return {
        id: res.id,
        name: res.name,
        stationNo: res.stationNo,
        coords: res.coords,
        x: anchorX,
        y: anchorY,
        anchorX,
        anchorY,
      };
    });
  }, [reservoirs, mapProjection]);

  const updateTime = useMemo(() => {
    if (!realTimeInfos || realTimeInfos.length === 0) return "";

    const timeStr = realTimeInfos[0].time;
    let date = new Date(timeStr);

    if (isNaN(date.getTime())) {
      const parts = timeStr.split(" ");
      const dateParts = parts[0].split("/");
      if (dateParts.length === 3) {
        date = new Date(`${dateParts[2]}-${dateParts[0].padStart(2, "0")}-${dateParts[1].padStart(2, "0")}T${parts[1]}`);
      }
    }

    if (isNaN(date.getTime())) return "";

    const twYear = date.getFullYear() - 1911;
    const month = String(date.getMonth() + 1).padStart(2, "0");
    const day = String(date.getDate()).padStart(2, "0");
    const hours = String(date.getHours()).padStart(2, "0");
    return `${twYear}-${month}-${day} ${hours}時`;
  }, [realTimeInfos]);

  if (isDisplayListLoading || isRealTimeLoading || isStationLoading) {
    return (
      <div style={{ display: "flex", justifyContent: "center", alignItems: "center", height: "100%" }}>
        <Spin size="large" tip="資料載入中..." />
      </div>
    );
  }

  const isMobileLayout = containerSize.w > 0 && containerSize.w < 768;
  const isGridLayout = layoutMode === "grid" || (layoutMode === "auto" && isMobileLayout);

  if (isGridLayout) {
    return (
      <div ref={containerRef} className="mobile-grid-view" style={{ position: "absolute", inset: 0 }}>
        <div style={{ position: "absolute", top: 8, right: 8, zIndex: 50 }}>
          <Button size="small" onClick={() => setLayoutMode("map")}>
            使用地圖檢視
          </Button>
        </div>
        <div style={{ textAlign: "center", marginBottom: 20, color: "#0abcce", fontSize: "1.2rem", fontWeight: "bold" }}>
          水庫蓄水圖 {updateTime ? `(${updateTime})` : ""}
        </div>
        <div className="reservoir-grid">
          {reservoirs.map((res) => (
            <ReservoirCard
              key={res.name}
              name={res.name}
              storage={res.volume}
              pct={res.percent}
              isHovered={res.name === hovered}
              onMouseEnter={() => setHovered(res.name)}
              onMouseLeave={() => setHovered(null)}
            />
          ))}
        </div>
      </div>
    );
  }

  return (
    <div ref={containerRef} style={{ position: "absolute", inset: 0 }}>
      <div className="dashboard-title">
        水庫蓄水圖 {updateTime ? `(${updateTime})` : ""}
      </div>

      <div style={{ position: "absolute", top: 8, right: 8, zIndex: 50, display: "flex", gap: 6 }}>
        <Button size="small" onClick={() => setLayoutMode("grid")}>
          使用網格檢視
        </Button>
        <Button size="small" onClick={handleExport}>
          匯出位置
        </Button>
        <Popconfirm
          title="確定要重置目前位置嗎？"
          onConfirm={handleReset}
          okText="重置"
          cancelText="取消"
        >
          <Button size="small" danger>
            重置位置
          </Button>
        </Popconfirm>
      </div>

      <ComposableMap
        projection={mapProjection}
        width={mapWidth}
        height={mapHeight}
        style={{ width: "100%", height: "100%", position: "absolute", top: 0, left: 0 }}
      >
        <Geographies geography={GEO_URL} parseNodeName="counties">
          {({ geographies }) =>
            geographies.map((geo) => (
              <Geography
                key={geo.rsmKey}
                geography={geo}
                fill="#c8e6f5"
                stroke="#FFFFFF"
                strokeWidth={0.8}
                style={{
                  default: { outline: "none" },
                  hover: { fill: "#b0d6ea", outline: "none" },
                  pressed: { outline: "none" },
                }}
              />
            ))}
        </Geographies>

        {reservoirs.map((res) => (
          <Marker key={`map-marker-${res.id}`} coordinates={res.coords}>
            <circle
              r={res.name === hovered ? 9 : 6}
              fill={res.name === hovered ? "#ffc107" : "#F5A623"}
              stroke="#fff"
              strokeWidth={2}
              style={{ transition: "r 0.2s, fill 0.2s" }}
            />
          </Marker>
        ))}
      </ComposableMap>

      <svg
        width={mapWidth}
        height={mapHeight}
        viewBox={`0 0 ${mapWidth} ${mapHeight}`}
        preserveAspectRatio="none"
        style={{ position: "absolute", inset: 0, width: "100%", height: "100%", zIndex: 6, pointerEvents: "none" }}
      >
        {cardPositions.map((pos) => {
          const cOff = dragOffsets[pos.name] ?? { dx: 0, dy: 0 };
          const markerX = pos.anchorX;
          const markerY = pos.anchorY;
          const cardX = pos.x + cOff.dx;
          const cardY = pos.y + cOff.dy;

          return (
            <g key={pos.name}>
              <line
                x1={markerX}
                y1={markerY}
                x2={cardX}
                y2={cardY}
                stroke="#081c1fff"
                strokeWidth={pos.name === hovered ? 2.5 : 1.2}
                strokeDasharray={pos.name === hovered ? "0" : "4 3"}
                opacity={pos.name === hovered ? 1 : 0.55}
                style={{ transition: "all 0.3s" }}
              />
            </g>
          );
        })}
      </svg>

      {cardPositions.map((pos) => {
        const res = reservoirById[pos.id];
        if (!res) return null;

        const off = dragOffsets[pos.name] ?? { dx: 0, dy: 0 };
        const finalX = pos.x + off.dx;
        const finalY = pos.y + off.dy;
        const isDragging = draggingRef.current?.name === pos.name;

        return (
          <div
            key={`card-${pos.name}`}
            onMouseDown={(e) => handleCardMouseDown(e, pos.name)}
            style={{
              position: "absolute",
              left: finalX,
              top: finalY,
              zIndex: pos.name === hovered ? 30 : 10,
              cursor: isDragging ? "grabbing" : "grab",
              userSelect: "none",
            }}
          >
            <ReservoirCard
              name={res.name}
              storage={res.volume}
              pct={res.percent}
              isHovered={res.name === hovered}
              onMouseEnter={() => setHovered(res.name)}
              onMouseLeave={() => setHovered(null)}
            />
          </div>
        );
      })}

      <div
        style={{
          position: "absolute",
          bottom: 15,
          width: "100%",
          textAlign: "center",
          color: "#fff",
          fontSize: "0.85rem",
          zIndex: 10,
          textShadow: "0 1px 3px rgba(0,0,0,0.8)",
          pointerEvents: "none",
        }}
      >
        <div>即時蓄水量單位：萬立方公尺</div>
        <div>本圖表未經人工判讀檢驗，僅供參考。</div>
      </div>
    </div>
  );
};

export default ReservoirDashboard;
