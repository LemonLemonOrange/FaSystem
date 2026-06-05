import React from "react";
import PropTypes from "prop-types";
import "./ReservoirCard.css";

const sanitizeId = (value) => String(value).replace(/[^\w-]/g, "");

const formatStorage = (value) => {
  if (value === null || value === undefined || value === "") {
    return "-";
  }

  const numericValue = Number(String(value).replace(/,/g, ""));
  if (Number.isFinite(numericValue)) {
    return numericValue.toLocaleString("zh-TW");
  }

  return String(value);
};

const clamp01 = (value) => Math.min(Math.max(value, 0), 1);

const getStorageFillRatio = (storage) => {
  const numericStorage = Number(String(storage).replace(/,/g, ""));
  if (!Number.isFinite(numericStorage)) {
    return null;
  }

  return clamp01(numericStorage / 45000);
};

const ReservoirCard = ({ name, storage, pct, isHovered = false, isDragging = false, onMouseEnter, onMouseLeave }) => {
  const hasPct = Number.isFinite(pct);
  const safePct = hasPct ? Math.min(Math.max(pct, 0), 100) : 0;
  const id = sanitizeId(name) || "reservoir";
  const formattedStorage = formatStorage(storage);
  const storageFillRatio = getStorageFillRatio(storage);
  const fillRatio = hasPct ? safePct / 100 : storageFillRatio != null ? storageFillRatio : 0;
  const waterLevelY = -22 + (1 - fillRatio) * 82;

  return (
    <div className="reservoir-card-wrapper" onMouseEnter={onMouseEnter} onMouseLeave={onMouseLeave} aria-label={`${name} 水庫資訊圖示`}>
      <div className="reservoir-title">{name}</div>
      <div className={`reservoir-card ${isHovered ? "hovered" : ""} ${isDragging ? "dragging" : ""}`}>


        <div className="reservoir-gauge">
          <svg className="reservoir-gauge-svg" viewBox="0 0 100 100" role="img" aria-label={`${name} 水庫蓄水圖`}>
            <defs>
              <clipPath id={`reservoir-clip-${id}`}>
                <circle cx="50" cy="50" r="41" />
              </clipPath>
              <linearGradient id={`reservoir-ring-${id}`} x1="12" y1="8" x2="88" y2="92" gradientUnits="userSpaceOnUse">
                <stop offset="0%" stopColor="#b9f9ff" />
                <stop offset="30%" stopColor="#63dbff" />
                <stop offset="66%" stopColor="#1a8bd7" />
                <stop offset="100%" stopColor="#084f92" />
              </linearGradient>
              <linearGradient id={`water-shine-${id}`} x1="0" y1="0" x2="0" y2="100" gradientUnits="userSpaceOnUse">
                <stop offset="0%" stopColor="#f6feff" />
                <stop offset="18%" stopColor="#b6f0ff" />
                <stop offset="52%" stopColor="#2ab5eb" />
                <stop offset="100%" stopColor="#05599e" />
              </linearGradient>
              <linearGradient id={`water-depth-${id}`} x1="0" y1="0" x2="0" y2="100" gradientUnits="userSpaceOnUse">
                <stop offset="0%" stopColor="#125f96" />
                <stop offset="62%" stopColor="#0b3d6a" />
                <stop offset="100%" stopColor="#051d35" />
              </linearGradient>
            </defs>

            <circle className="reservoir-ring" cx="50" cy="50" r="43" stroke={`url(#reservoir-ring-${id})`} />
            <circle className="reservoir-inner-bg" cx="50" cy="50" r="40" fill={`url(#water-depth-${id})`} />

            <g clipPath={`url(#reservoir-clip-${id})`}>
              <rect className="reservoir-dark-water" x="8" y="8" width="84" height="84" fill={`url(#water-depth-${id})`} />
              <g className="water-level" transform={`translate(0 ${waterLevelY})`}>
                <g className="water-level-bob">
                  <g className="wave-track wave-track-back">
                    <path
                      className="water-wave water-wave-back"
                      d="M-20 29 C-10 25 0 25 10 29 C20 33 30 33 40 29 C50 25 60 25 70 29 C80 33 90 33 100 29 L100 112 L-20 112 Z"
                      fill={`url(#water-shine-${id})`}
                    />
                    <path
                      className="water-wave water-wave-back"
                      d="M-20 29 C-10 25 0 25 10 29 C20 33 30 33 40 29 C50 25 60 25 70 29 C80 33 90 33 100 29 L100 112 L-20 112 Z"
                      fill={`url(#water-shine-${id})`}
                      transform="translate(120 0)"
                    />
                  </g>
                  <g className="wave-track wave-track-front">
                    <path
                      className="water-wave water-wave-front"
                      d="M-20 30 C-10 26 0 26 10 30 C20 34 30 34 40 30 C50 26 60 26 70 30 C80 34 90 34 100 30 L100 112 L-20 112 Z"
                      fill="#0f84d9"
                    />
                    <path
                      className="water-wave water-wave-front"
                      d="M-20 30 C-10 26 0 26 10 30 C20 34 30 34 40 30 C50 26 60 26 70 30 C80 34 90 34 100 30 L100 112 L-20 112 Z"
                      fill="#0f84d9"
                      transform="translate(120 0)"
                    />
                    <path
                      className="water-highlight"
                      d="M-18 28 C-8 24 2 24 12 28 C22 32 32 32 42 28 C52 24 62 24 72 28 C82 32 92 32 102 28"
                    />
                    <path
                      className="water-highlight"
                      d="M-18 28 C-8 24 2 24 12 28 C22 32 32 32 42 28 C52 24 62 24 72 28 C82 32 92 32 102 28"
                      transform="translate(120 0)"
                    />
                  </g>
                  <circle className="reservoir-reflection" cx="41" cy="33" r="18" />
                </g>
              </g>
            </g>
          </svg>

          <div className="reservoir-gauge-content">
            <div className="storage-label">蓄水量</div>
            <div className="storage-amount">{formattedStorage}</div>
            <div className="pct-divider" aria-hidden="true" />
            <div className="storage-unit">萬立方公尺</div>
            <div className="pct-badge">{safePct.toFixed(2)}%</div>
          </div>
        </div>
      </div>
    </div>
  );
};

export default ReservoirCard;

ReservoirCard.propTypes = {
  name: PropTypes.string.isRequired,
  storage: PropTypes.oneOfType([PropTypes.string, PropTypes.number]).isRequired,
  pct: PropTypes.number.isRequired,
  isHovered: PropTypes.bool,
  isDragging: PropTypes.bool,
  onMouseEnter: PropTypes.func,
  onMouseLeave: PropTypes.func,
};

