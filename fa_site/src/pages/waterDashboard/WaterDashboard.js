import React, { useState } from 'react';
import './WaterDashboard.css';
import ReservoirDashboard from './ReservoirDashboard';
import WaterWarningDashboard from './WaterWarningDashboard';

const WaterReservoirDashboard = () => {
  const [activeTab, setActiveTab] = useState('reservoir');

  return (
    <div style={{ padding: '20px' }}>
      <div className="dashboard-container">

        {/* Header Tabs */}
        <div className="dashboard-header-tabs">
          <div 
            className={`header-tab ${activeTab === 'reservoir' ? 'active' : ''}`}
            onClick={() => setActiveTab('reservoir')}
          >
            水庫蓄水情形圖
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
        
      </div>
    </div>
  );
};

export default WaterReservoirDashboard;
