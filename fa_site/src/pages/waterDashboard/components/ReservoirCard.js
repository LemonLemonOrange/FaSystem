import React from 'react';
import { Progress, Tag } from 'antd';
import './ReservoirCard.css';

const getStatus = (pct) => {
  if (pct >= 100) return { label: '滿庫', color: 'cyan' };
  if (pct >= 95) return { label: '高蓄水', color: 'blue' };
  if (pct >= 50) return { label: '正常', color: 'green' };
  if (pct >= 20) return { label: '偏低', color: 'orange' };
  return { label: '警戒', color: 'red' };
};

const getStrokeColor = (pct) => {
  if (pct >= 95) return '#13c2c2';
  if (pct >= 50) return '#52c41a';
  if (pct >= 20) return '#faad14';
  return '#ff4d4f';
};

const ReservoirCard = ({ name, storage, pct, isHovered, onMouseEnter, onMouseLeave }) => {
  const status = getStatus(pct);
  const isFull = pct >= 100;

  return (
    <div
      className={`reservoir-card ${isFull ? 'full' : ''} ${isHovered ? 'hovered' : ''}`}
      onMouseEnter={onMouseEnter}
      onMouseLeave={onMouseLeave}
    >
      <div className="card-header">
        <div className="reservoir-name">{name}</div>
        <Tag color={status.color} style={{ margin: 0, fontSize: '0.7rem' }}>
          {status.label}
        </Tag>
      </div>

      <Progress
        percent={Math.min(pct, 100)}
        size="small"
        strokeColor={getStrokeColor(pct)}
        trailColor="rgba(255,255,255,0.15)"
        showInfo={false}
        style={{ margin: '4px 0' }}
      />

      <div className="stats-row">
        <div>
          <div className="storage-amount">{storage}</div>
          <div className="storage-unit">萬立方公尺</div>
        </div>
        <div className="pct-badge">{pct.toFixed(2)}%</div>
      </div>

      <svg className="wave-bg" viewBox="0 0 300 50" preserveAspectRatio="none">
        <path d="M0,25 C50,5 100,45 150,25 S250,5 300,25 L300,50 L0,50Z" fill="var(--cyan)" />
      </svg>
    </div>
  );
};

export default ReservoirCard;
