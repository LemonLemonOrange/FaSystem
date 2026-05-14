import React, { useState, useMemo, useRef, useEffect, useCallback } from 'react';
import './WaterDashboard.css';
import ReservoirCard from './components/ReservoirCard';
import { ComposableMap, Geographies, Geography } from 'react-simple-maps';
import { useReservoirStation, useReservoirRealTimeInfo } from '../../libs/WraGov/reservoir';
import { Spin, Button, Popconfirm, message } from 'antd';

const GEO_URL = 'https://cdn.jsdelivr.net/npm/taiwan-atlas/counties-10t.json';

// 水庫實際經緯度
const reservoirMapData = [
  { name: '寶山水庫', coords: [121.04, 24.78] },
  { name: '寶山第二水庫', coords: [121.07, 24.74] },
  { name: '永和山水庫', coords: [120.96, 24.62] },
  { name: '石門水庫', coords: [121.24, 24.88] },
  { name: '翡翠水庫', coords: [121.60, 24.93] },
  { name: '鯉魚潭水庫', coords: [120.86, 24.30] },
  { name: '德基水庫', coords: [121.12, 24.22] },
  { name: '南化水庫', coords: [120.45, 23.12] },
  { name: '蘭潭水庫', coords: [120.53, 23.46] },
  { name: '仁義潭水庫', coords: [120.51, 23.44] },
  { name: '曾文水庫', coords: [120.47, 23.26] },
  { name: '烏山頭水庫', coords: [120.33, 23.12] },
];
// 卡片預設位移（未調整時的初始位置）
const DEFAULT_CARD_OFFSETS = {
  '寶山水庫': { dx: -214, dy: 168 },
  '寶山第二水庫': { dx: -464, dy: -89 },
  '翡翠水庫': { dx: 284, dy: 16 },
  '曾文水庫': { dx: -72, dy: 232 },
  '南化水庫': { dx: -287, dy: -239 },
  '蘭潭水庫': { dx: 379, dy: 252 },
  '烏山頭水庫': { dx: -266, dy: 205 },
  '德基水庫': { dx: 369, dy: 54 },
  '永和山水庫': { dx: -251, dy: 296 },
  '鯉魚潭水庫': { dx: 298, dy: 121 },
  '石門水庫': { dx: 37, dy: -410 },
  '仁義潭水庫': { dx: 506, dy: 62 },
};

// 標記點預設位移（未調整時的初始位置）
const DEFAULT_MARKER_OFFSETS = {
  '仁義潭水庫': { dx: 88, dy: 80 },
  '蘭潭水庫': { dx: 98, dy: -22 },
  '永和山水庫': { dx: -64, dy: 127 },
  '南化水庫': { dx: 37, dy: -97 },
  '曾文水庫': { dx: 32, dy: 31 },
  '烏山頭水庫': { dx: 57, dy: -33 },
  '寶山第二水庫': { dx: -44, dy: 47 },
  '石門水庫': { dx: -23, dy: 35 },
  '翡翠水庫': { dx: -112, dy: 68 },
};


// 精確複製 react-simple-maps 的 geoMercator 投影
// projectionConfig: { center: [121, 23.8], scale: 8000 }, width=600, height=700
// 最後從 SVG 座標空間 (600×700) 縮放到容器像素
const SVG_W = 600;
const SVG_H = 700;
const PROJ_CENTER_LNG = 121;
const PROJ_CENTER_LAT = 23.8;
const PROJ_SCALE = 8000;

function latlngToPixel(lng, lat, containerW, containerH) {
  const toRad = (d) => (d * Math.PI) / 180;
  // geoMercator: x 線性，y 使用 log(tan) 公式
  const x_svg =
    (lng - PROJ_CENTER_LNG) * PROJ_SCALE * (Math.PI / 180) + SVG_W / 2;
  const y_svg =
    -(
      Math.log(Math.tan(Math.PI / 4 + toRad(lat) / 2)) -
      Math.log(Math.tan(Math.PI / 4 + toRad(PROJ_CENTER_LAT) / 2))
    ) *
    PROJ_SCALE +
    SVG_H / 2;
  // 從 SVG 座標空間縮放到容器像素
  return {
    x: (x_svg / SVG_W) * containerW,
    y: (y_svg / SVG_H) * containerH,
  };
}


const ReservoirDashboard = () => {
  const [hovered, setHovered] = useState(null);
  const [layoutMode, setLayoutMode] = useState('auto');
  const containerRef = useRef(null);
  const [containerSize, setContainerSize] = useState({ w: 900, h: 675 });
  // 卡片 drag state（localStorage 沒有時 fallback 到 DEFAULT_CARD_OFFSETS）
  const [dragOffsets, setDragOffsets] = useState(() => {
    try {
      const saved = JSON.parse(localStorage.getItem('reservoir-card-offsets'));
      return saved ?? DEFAULT_CARD_OFFSETS;
    } catch { return DEFAULT_CARD_OFFSETS; }
  });
  const draggingRef = useRef(null);
  // 標記點 drag state（localStorage 沒有時 fallback 到 DEFAULT_MARKER_OFFSETS）
  const [markerOffsets, setMarkerOffsets] = useState(() => {
    try {
      const saved = JSON.parse(localStorage.getItem('reservoir-marker-offsets'));
      return saved ?? DEFAULT_MARKER_OFFSETS;
    } catch { return DEFAULT_MARKER_OFFSETS; }
  });
  const markerDraggingRef = useRef(null);

  const { data: stations, isLoading: isStationsLoading } = useReservoirStation();
  const { data: realTimeInfos, isLoading: isRealTimeLoading } = useReservoirRealTimeInfo();

  // 監聽容器尺寸變化
  useEffect(() => {
    if (!containerRef.current) return;
    const ro = new ResizeObserver(entries => {
      for (const e of entries) {
        setContainerSize({ w: e.contentRect.width, h: e.contentRect.height });
      }
    });
    ro.observe(containerRef.current);
    return () => ro.disconnect();
  }, []);

  const handleCardMouseDown = useCallback((e, name, baseX, baseY) => {
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
      setDragOffsets(prev => ({
        ...prev,
        [d.name]: {
          dx: d.baseDx + ev.clientX - d.startX,
          dy: d.baseDy + ev.clientY - d.startY,
        },
      }));
    };

    const onUp = () => {
      draggingRef.current = null;
      window.removeEventListener('mousemove', onMove);
      window.removeEventListener('mouseup', onUp);
      // 持久化卡片位置
      setDragOffsets(prev => {
        localStorage.setItem('reservoir-card-offsets', JSON.stringify(prev));
        return prev;
      });
    };

    window.addEventListener('mousemove', onMove);
    window.addEventListener('mouseup', onUp);
  }, [dragOffsets]);

  const handleMarkerMouseDown = useCallback((e, name) => {
    e.preventDefault();
    e.stopPropagation();
    const cur = markerOffsets[name] ?? { dx: 0, dy: 0 };
    markerDraggingRef.current = {
      name,
      startX: e.clientX,
      startY: e.clientY,
      baseDx: cur.dx,
      baseDy: cur.dy,
    };

    const onMove = (ev) => {
      const d = markerDraggingRef.current;
      if (!d) return;
      setMarkerOffsets(prev => ({
        ...prev,
        [d.name]: {
          dx: d.baseDx + ev.clientX - d.startX,
          dy: d.baseDy + ev.clientY - d.startY,
        },
      }));
    };

    const onUp = () => {
      markerDraggingRef.current = null;
      window.removeEventListener('mousemove', onMove);
      window.removeEventListener('mouseup', onUp);
      // 持久化標記點位置
      setMarkerOffsets(prev => {
        localStorage.setItem('reservoir-marker-offsets', JSON.stringify(prev));
        return prev;
      });
    };

    window.addEventListener('mousemove', onMove);
    window.addEventListener('mouseup', onUp);
  }, [markerOffsets]);

  const handleReset = useCallback(() => {
    localStorage.removeItem('reservoir-card-offsets');
    localStorage.removeItem('reservoir-marker-offsets');
    setDragOffsets(DEFAULT_CARD_OFFSETS);
    setMarkerOffsets(DEFAULT_MARKER_OFFSETS);
  }, []);

  const handleExport = useCallback(() => {
    const cardLines = Object.entries(dragOffsets)
      .map(([name, { dx, dy }]) => `  '${name}': { dx: ${Math.round(dx)}, dy: ${Math.round(dy)} },`)
      .join('\n');
    const markerLines = Object.entries(markerOffsets)
      .map(([name, { dx, dy }]) => `  '${name}': { dx: ${Math.round(dx)}, dy: ${Math.round(dy)} },`)
      .join('\n');

    const text = `const DEFAULT_CARD_OFFSETS = {\n${cardLines}\n};\n\nconst DEFAULT_MARKER_OFFSETS = {\n${markerLines}\n};`;
    navigator.clipboard.writeText(text).then(() => {
      message.success('已複製卡片與標記點位置到剪貼簿');
    });
  }, [dragOffsets, markerOffsets]);

  const reservoirs = useMemo(() => {
    if (!stations || !realTimeInfos) return reservoirMapData.map(d => ({ ...d, volume: '-', percent: 0 }));
    return reservoirMapData.map(mapData => {
      const station = stations.find(s => s.StationName === mapData.name);
      if (!station) return { ...mapData, volume: '-', percent: 0 };
      const realTime = realTimeInfos.find(r => r.StationNo === station.StationNo);
      const volume = realTime?.EffectiveStorage != null ? Math.round(realTime.EffectiveStorage).toLocaleString() : '-';
      const percent = realTime?.PercentageOfStorage != null ? Number(realTime.PercentageOfStorage.toFixed(2)) : 0;
      return { ...mapData, volume, percent };
    });
  }, [stations, realTimeInfos]);

  // 計算卡片位置，並解決重疊
  const cardPositions = useMemo(() => {
    const { w, h } = containerSize;
    const initial = reservoirs.map(res => {
      const anchor = latlngToPixel(res.coords[0], res.coords[1], w, h);
      return {
        name: res.name,
        x: anchor.x,
        y: anchor.y,
        anchorX: anchor.x,
        anchorY: anchor.y,
      };
    });
    return initial;
  }, [reservoirs, containerSize]);

  const updateTime = useMemo(() => {
    if (!realTimeInfos || realTimeInfos.length === 0) return '';
    const date = new Date(realTimeInfos[0].Time);
    if (isNaN(date.getTime())) return '';
    const twYear = date.getFullYear() - 1911;
    const month = String(date.getMonth() + 1).padStart(2, '0');
    const day = String(date.getDate()).padStart(2, '0');
    const hours = String(date.getHours()).padStart(2, '0');
    return `${twYear}-${month}-${day} ${hours}時`;
  }, [realTimeInfos]);

  if (isStationsLoading || isRealTimeLoading) {
    return (
      <div style={{ display: 'flex', justifyContent: 'center', alignItems: 'center', height: '100%' }}>
        <Spin size="large" tip="資料載入中..." />
      </div>
    );
  }

  const isMobileLayout = containerSize.w > 0 && containerSize.w < 768;
  const isGridLayout = layoutMode === 'grid' || (layoutMode === 'auto' && isMobileLayout);

  if (isGridLayout) {
    return (
      <div ref={containerRef} className="mobile-grid-view" style={{ position: 'absolute', inset: 0 }}>
        <div style={{ position: 'absolute', top: 8, right: 8, zIndex: 50 }}>
          <Button size="small" onClick={() => setLayoutMode('map')}>
            使用地圖排版
          </Button>
        </div>
        <div style={{ textAlign: 'center', marginBottom: 20, color: '#0abcce', fontSize: '1.2rem', fontWeight: 'bold' }}>
          水庫蓄水情形 {updateTime ? `(${updateTime})` : ''}
        </div>
        <div className="reservoir-grid">
          {reservoirs.map(res => (
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
    <div ref={containerRef} style={{ position: 'absolute', inset: 0 }}>
      <div className="dashboard-title">
        水庫蓄水情形 {updateTime ? `(${updateTime})` : ''}
      </div>

      {/* 工具列 */}
      <div style={{ position: 'absolute', top: 8, right: 8, zIndex: 50, display: 'flex', gap: 6 }}>
        <Button size="small" onClick={() => setLayoutMode('grid')}>
          使用網格排版
        </Button>
        <Button size="small" onClick={handleExport}>
          匯出位置
        </Button>
        <Popconfirm
          title="確定要重置所有位置？"
          onConfirm={handleReset}
          okText="重置"
          cancelText="取消"
        >
          <Button size="small" danger>
            重置位置
          </Button>
        </Popconfirm>
      </div>

      {/* 地圖 */}
      <ComposableMap
        projection="geoMercator"
        projectionConfig={{ center: [121, 23.8], scale: 8000 }}
        width={600}
        height={700}
        style={{ width: '100%', height: '100%', position: 'absolute', top: 0, left: 0 }}
      >
        <Geographies geography={GEO_URL} parseNodeName="counties">
          {({ geographies }) =>
            geographies.map(geo => (
              <Geography
                key={geo.rsmKey}
                geography={geo}
                fill="#c8e6f5"
                stroke="#FFFFFF"
                strokeWidth={0.8}
                style={{
                  default: { outline: 'none' },
                  hover: { fill: '#b0d6ea', outline: 'none' },
                  pressed: { outline: 'none' },
                }}
              />
            ))
          }
        </Geographies>

        {/* 水庫標記點已移至 SVG overlay，支援拖曳 */}
      </ComposableMap>

      {/* 標記點 + 連接線 SVG overlay（只負責繪圖，無事件） */}
      <svg
        style={{ position: 'absolute', inset: 0, width: '100%', height: '100%', zIndex: 6, pointerEvents: 'none' }}
      >
        {cardPositions.map(pos => {
          const mOff = markerOffsets[pos.name] ?? { dx: 0, dy: 0 };
          const cOff = dragOffsets[pos.name] ?? { dx: 0, dy: 0 };
          const markerX = pos.anchorX + mOff.dx;
          const markerY = pos.anchorY + mOff.dy;
          const cardX = pos.x + cOff.dx;
          const cardY = pos.y + cOff.dy;
          const isDraggingMarker = markerDraggingRef.current?.name === pos.name;
          return (
            <g key={pos.name}>
              <line
                x1={markerX}
                y1={markerY}
                x2={cardX}
                y2={cardY}
                stroke="#081c1fff"
                strokeWidth={pos.name === hovered ? 2.5 : 1.2}
                strokeDasharray={pos.name === hovered ? '0' : '4 3'}
                opacity={pos.name === hovered ? 1 : 0.55}
                style={{ transition: isDraggingMarker ? 'none' : 'all 0.3s' }}
              />
              <circle
                cx={markerX}
                cy={markerY}
                r={pos.name === hovered ? 9 : 6}
                fill={pos.name === hovered ? '#ffc107' : '#F5A623'}
                stroke="#fff"
                strokeWidth={2}
                style={{ transition: isDraggingMarker ? 'none' : 'r 0.2s, fill 0.2s' }}
              />
            </g>
          );
        })}
      </svg>

      {/* 標記點透明 div 層：負責 hover / drag 事件 + antd Tooltip */}
      {cardPositions.map(pos => {
        const mOff = markerOffsets[pos.name] ?? { dx: 0, dy: 0 };
        const markerX = pos.anchorX + mOff.dx;
        const markerY = pos.anchorY + mOff.dy;
        const isDraggingMarker = markerDraggingRef.current?.name === pos.name;
        const SIZE = pos.name === hovered ? 18 : 14; // 直徑，比 circle 稍大好點擊
        return (
          // <Tooltip
          //   key={`marker-tooltip-${pos.name}`}
          //   title={`${pos.name} 標記點｜X: ${Math.round(markerX)}, Y: ${Math.round(markerY)}`}
          //   placement="right"
          // >
          <div
            onMouseEnter={() => setHovered(pos.name)}
            onMouseLeave={() => setHovered(null)}
            onMouseDown={(e) => handleMarkerMouseDown(e, pos.name)}
            style={{
              position: 'absolute',
              left: markerX - SIZE / 2,
              top: markerY - SIZE / 2,
              width: SIZE,
              height: SIZE,
              borderRadius: '50%',
              cursor: isDraggingMarker ? 'grabbing' : 'grab',
              zIndex: 8,
            }}
          />
          // </Tooltip>
        );
      })}

      {/* 資訊卡：使用 force-separated 像素座標 + 拖曳位移 */}
      {cardPositions.map(pos => {
        const res = reservoirs.find(r => r.name === pos.name);
        if (!res) return null;
        const off = dragOffsets[pos.name] ?? { dx: 0, dy: 0 };
        const finalX = pos.x + off.dx;
        const finalY = pos.y + off.dy;
        const isDragging = draggingRef.current?.name === pos.name;
        return (
          // <Tooltip
          //   key={`card-tooltip-${pos.name}`}
          //   title={`${pos.name} 卡片｜X: ${Math.round(finalX)}, Y: ${Math.round(finalY)}`}
          //   placement="top"
          // >
          <div
            onMouseDown={(e) => handleCardMouseDown(e, pos.name, pos.x, pos.y)}
            style={{
              position: 'absolute',
              left: finalX,
              top: finalY,
              zIndex: pos.name === hovered ? 30 : 10,
              cursor: isDragging ? 'grabbing' : 'grab',
              userSelect: 'none',
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
          // </Tooltip>
        );
      })}

      {/* 備註 */}
      <div style={{ position: 'absolute', bottom: 15, width: '100%', textAlign: 'center', color: '#fff', fontSize: '0.85rem', zIndex: 10, textShadow: '0 1px 3px rgba(0,0,0,0.8)', pointerEvents: 'none' }}>
        <div>※蓄水量單位：萬立方公尺</div>
        <div>※本表未經人工完整檢驗，僅供參考</div>
      </div>
    </div>
  );
};

export default ReservoirDashboard;

