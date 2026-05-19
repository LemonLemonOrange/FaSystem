import React from 'react';
import { Card, Typography } from 'antd';

const { Title } = Typography;

const Users = () => {
  return (
    <div style={{ padding: 24 }}>
      <div style={{ display: 'flex', justifyContent: 'space-between', marginBottom: 16 }}>
        <Title level={2}>å¸³è?æ¬Šé?ç®¡ç?</Title>
      </div>
      <Card>
        <p>?™è£¡?¯å¸³?Ÿæ??é??¢ï??®å?å°šæœª?¥ä? API??/p>
        <p>?ªä?å°‡æ?ä¾›æ–°å¢žä½¿?¨è€…ã€è??²æ?æ´¾è?æ¬Šé??§åˆ¶?Ÿèƒ½??/p>
      </Card>
    </div>
  );
};

export default Users;
