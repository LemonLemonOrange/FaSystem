import React, { useState } from 'react';
import './WaterDashboard.css';
import ReservoirDashboard from './ReservoirDashboard';
import WaterWarningDashboard from './WaterWarningDashboard';
import ReservoirFactoryMappingForm from './components/ReservoirFactoryMappingForm';

const WaterReservoirDashboard = () => {
  const [activeTab, setActiveTab] = useState('overview');

  return (
    <div className="water-dashboard-page" style={{ padding: '20px' }}>
      <div className={`dashboard-container ${activeTab === 'overview' ? 'overview-mode' : ''}`}>

        {/* Header Tabs */}
        <div className="dashboard-header-tabs">
          <div
            className={`header-tab ${activeTab === 'overview' ? 'active' : ''}`}
            onClick={() => setActiveTab('overview')}
          >
            總覽
          </div>
          <div
            className={`header-tab ${activeTab === 'reservoir' ? 'active' : ''}`}
            onClick={() => setActiveTab('reservoir')}
          >
            水庫蓄水圖
          </div>
          <div
            className={`header-tab ${activeTab === 'warning' ? 'active' : ''}`}
            onClick={() => setActiveTab('warning')}
          >
            水情燈號
          </div>
        </div>

        {activeTab === 'reservoir' && <ReservoirDashboard />}
        {activeTab === 'warning' && <WaterWarningDashboard />}

        {activeTab === 'overview' && (
          <div className="overview-scroll-area">
            <ReservoirFactoryMappingForm />
          </div>
        )}
      </div>

    </div>
  );
};

export default WaterReservoirDashboard;