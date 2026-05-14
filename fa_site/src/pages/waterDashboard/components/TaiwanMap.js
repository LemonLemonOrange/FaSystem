import React, { useMemo } from "react";
import { ComposableMap, Geographies, Geography, Marker } from "react-simple-maps";

const GEO_URL = "https://cdn.jsdelivr.net/npm/taiwan-atlas/counties-10t.json";

const COLOR_MAP = {
  green: { default: "#4CAF50", hover: "#388E3C" },
  yellow: { default: "#FFEB3B", hover: "#FBC02D" },
  orange: { default: "#FF9800", hover: "#F57C00" },
  red: { default: "#F44336", hover: "#D32F2F" },
};

// 縣市名稱 → 標記點的大約經緯度（用於 Marker）
const COUNTY_COORDINATES = {
  "新竹縣": [121.15, 24.7],
  "新竹市": [120.97, 24.8],
  "苗栗縣": [120.9, 24.55],
  "桃園市": [121.2, 24.95],
  "臺南市": [120.25, 23.1], "台南市": [120.25, 23.1],
  "高雄市": [120.4, 22.8],
  "嘉義市": [120.45, 23.48], "嘉義縣": [120.5, 23.45],
  "臺北市": [121.55, 25.05], "台北市": [121.55, 25.05],
  "新北市": [121.4, 25.0],
  "基隆市": [121.7, 25.1],
  "宜蘭縣": [121.75, 24.7],
  "花蓮縣": [121.5, 23.8],
  "臺東縣": [121.0, 22.8], "台東縣": [121.0, 22.8],
  "屏東縣": [120.6, 22.3],
  "臺中市": [120.7, 24.2], "台中市": [120.7, 24.2],
  "彰化縣": [120.5, 24.0],
  "南投縣": [120.9, 23.9],
  "雲林縣": [120.5, 23.7],
  "澎湖縣": [119.6, 23.6],
  "金門縣": [118.3, 24.4],
  "連江縣": [119.9, 26.1],
};

// 「地區」名稱 → 對應縣市列表（用來處理 CAP areaDesc 可能是地區名而非縣市名的情況）
const REGION_TO_COUNTIES = {
  "新竹地區": ["新竹縣", "新竹市"],
  "台中地區": ["臺中市", "台中市"],
  "苗栗地區": ["苗栗縣"],
  "桃園地區": ["桃園市"],
  "台南地區": ["臺南市", "台南市"],
  "高雄地區": ["高雄市"],
  "嘉義地區": ["嘉義市", "嘉義縣"],
  "北部地區": ["臺北市", "台北市", "新北市", "基隆市"],
  "宜蘭地區": ["宜蘭縣"],
  "花蓮地區": ["花蓮縣"],
  "台東地區": ["臺東縣", "台東縣"],
  "屏東地區": ["屏東縣"],
  "澎湖地區": ["澎湖縣"],
};

// 將 region 名稱展開成縣市陣列，若是縣市名稱則原樣回傳
const expandToCounties = (region) => REGION_TO_COUNTIES[region] ?? [region];

// 取地區的代表座標（若是縣市名則直接查，若是地區名取第一個縣市）
const getRepresentativeCoords = (region) => {
  if (COUNTY_COORDINATES[region]) return COUNTY_COORDINATES[region];
  const counties = REGION_TO_COUNTIES[region];
  if (counties) {
    for (const c of counties) {
      if (COUNTY_COORDINATES[c]) return COUNTY_COORDINATES[c];
    }
  }
  return null;
};

export default function TaiwanMap({ warnings = [] }) {
  const { highlightedAreas, markers } = useMemo(() => {
    const areas = {};
    const marks = [];

    warnings.forEach(w => {
      const colorInfo = COLOR_MAP[w.colorClass] || COLOR_MAP.green;

      // 展開地區名 → 縣市名，確保 TopoJSON feature 能被正確上色
      expandToCounties(w.region).forEach(county => {
        areas[county] = colorInfo;
      });

      // Marker 放在地區的代表座標
      const coords = getRepresentativeCoords(w.region);
      if (coords) {
        marks.push({
          name: w.region,
          coordinates: coords,
          color: colorInfo.default
        });
      }
    });

    console.log('areas', areas);
    console.log('markers', marks);
    return { highlightedAreas: areas, markers: marks };
  }, [warnings]);


  return (
    <div style={{ width: '100%', height: '100%', display: 'flex', justifyContent: 'center', alignItems: 'center' }}>
      <ComposableMap
        projection="geoMercator"
        projectionConfig={{ center: [121, 23.8], scale: 8000 }}
        width={600}
        height={700}
        style={{ width: "100%", height: "100%" }}
      >
        <Geographies geography={GEO_URL} parseNodeName="counties">
          {({ geographies }) =>
            geographies.map((geo) => {
              const highlightInfo = highlightedAreas[geo.properties.COUNTYNAME];
              const isHighlighted = !!highlightInfo;
              const defaultColor = isHighlighted ? highlightInfo.default : "#E0E0E0";
              const hoverColor = isHighlighted ? highlightInfo.hover : "#BDBDBD";

              return (
                <Geography
                  key={geo.rsmKey}
                  geography={geo}
                  fill={defaultColor}
                  stroke="#FFFFFF"
                  strokeWidth={0.8}
                  style={{
                    default: { outline: "none", transition: "all 250ms" },
                    hover: { fill: hoverColor, outline: "none", transition: "all 250ms" },
                    pressed: { outline: "none" },
                  }}
                />
              );
            })
          }
        </Geographies>

        {markers.map(({ name, coordinates, color }) => (
          <Marker key={name} coordinates={coordinates}>
            <circle r={6} fill={color} stroke="#fff" strokeWidth={2} />
            <text
              textAnchor="middle"
              y={-12}
              style={{
                fontFamily: "Noto Sans TC, sans-serif",
                fill: "#333",
                fontSize: "14px",
                fontWeight: "bold",
                textShadow: "1px 1px 0 #fff, -1px -1px 0 #fff, 1px -1px 0 #fff, -1px 1px 0 #fff"
              }}
            >
              {name}
            </text>
          </Marker>
        ))}
      </ComposableMap>
    </div>
  );
}
