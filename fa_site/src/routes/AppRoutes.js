import React from "react";
import { Routes, Route, Navigate } from "react-router-dom";

import Users from "../components/Users/Users";
import WaterDashboard from "../pages/waterDashboard/WaterDashboard";
import ImgReadPage from "../pages/ImgRead/ImgReadPage";

const AppRoutes = () => (
  <Routes>
    <Route path="/" element={<Navigate to="/water-resource/dashboard" replace />} />
    <Route path="/admin/users" element={<Users />} />
    <Route path="/water-resource/dashboard" element={<WaterDashboard />} />
    <Route path="/img-read" element={<ImgReadPage />} />
    <Route path="*" element={<Navigate to="/water-resource/dashboard" replace />} />
  </Routes>
);

export default AppRoutes;
