import React from 'react';
import { Card, Typography } from 'antd';

const { Title } = Typography;

const Users = () => {
  return (
    <div style={{ padding: 24 }}>
      <div style={{ display: 'flex', justifyContent: 'space-between', marginBottom: 16 }}>
        <Title level={2}>帳號權限管理</Title>
      </div>
      <Card>
        <p>這裡是帳號權限頁面，目前尚未接上 API。</p>
        <p>未來將提供新增使用者、角色指派與權限控制功能。</p>
      </Card>
    </div>
  );
};

export default Users;
