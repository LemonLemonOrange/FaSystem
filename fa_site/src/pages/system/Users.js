import React from "react";
import { Card, Typography } from "antd";

const { Title } = Typography;

const Users = () => {
  return (
    <div style={{ padding: 24 }}>
      <div style={{ display: "flex", justifyContent: "space-between", marginBottom: 16 }}>
        <Title level={2}>帳號權限管理</Title>
      </div>
      <Card>
        <p>這裡是帳號與權限管理頁，尚未串接 API。</p>
        <p>後續將提供新增使用者、角色分派與權限控管功能。</p>
      </Card>
    </div>
  );
};

export default Users;
