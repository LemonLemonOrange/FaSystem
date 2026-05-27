import React from "react";
import { BrowserRouter, useNavigate, useLocation } from "react-router-dom";
import { Layout, Menu, Typography } from "antd";
import { ScanOutlined, SettingOutlined } from "@ant-design/icons";
import { QueryClient, QueryClientProvider } from "react-query";

import AppRoutes from "./routes/AppRoutes";

const { Header, Content, Sider } = Layout;
const { Title } = Typography;

const queryClient = new QueryClient({
  defaultOptions: {
    queries: {
      refetchOnWindowFocus: false,
      retry: 1,
    },
  },
});

const AppContent = () => {
  const navigate = useNavigate();
  const location = useLocation();

  const handleMenuClick = ({ key }) => {
    navigate(key);
  };

  return (
    <Layout style={{ minHeight: "100vh" }}>
      <Header style={{ background: "#002140", padding: 0 }}>
        <Title level={3} style={{ color: "white", margin: "14px 20px" }}>Code WMS</Title>
      </Header>
      <Layout>
        <Sider width={200} theme="light">
          <Menu
            mode="inline"
            selectedKeys={[location.pathname]}
            defaultOpenKeys={["group_resource", "group_system"]}
            onClick={handleMenuClick}
            style={{ height: "100%", borderRight: 0 }}
            items={[
              {
                key: "group_resource",
                icon: <SettingOutlined />,
                label: "水資源管理",
                children: [{ key: "/water-resource/dashboard", label: "水庫管理" }],
              },
              {
                key: "group_system",
                icon: <SettingOutlined />,
                label: "系統管理",
                children: [{ key: "/admin/users", label: "帳號權限" }],
              },
              { key: "/img-read", icon: <ScanOutlined />, label: "影像辨識" },
            ]}
          />
        </Sider>
        <Layout style={{ padding: "24px" }}>
          <Content style={{ background: "#fff", padding: 24, margin: 0, minHeight: 280 }}>
            <AppRoutes />
          </Content>
        </Layout>
      </Layout>
    </Layout>
  );
};

function App() {
  return (
    <QueryClientProvider client={queryClient}>
      <BrowserRouter>
        <AppContent />
      </BrowserRouter>
    </QueryClientProvider>
  );
}

export default App;
