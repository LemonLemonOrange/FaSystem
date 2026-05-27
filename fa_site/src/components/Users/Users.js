import React from "react";
import { Card, Typography } from "antd";

const { Title } = Typography;

const Users = () => {
  return (
    <div style={{ padding: 24 }}>
      <div style={{ display: "flex", justifyContent: "space-between", marginBottom: 16 }}>
        <Title level={2}>�b���v���޲z</Title>
      </div>
      <Card>
        <p>�o�̬O�b���v�������A�ثe�|�����W API�C</p>
        <p>���ӱN���ѷs�W�ϥΪ̡B��������P�v������\��C</p>
      </Card>
    </div>
  );
};

export default Users;
