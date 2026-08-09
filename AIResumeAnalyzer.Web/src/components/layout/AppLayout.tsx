import {
  Box,
} from "@mui/material";

import {
  Outlet,
} from "react-router-dom";

import Sidebar from "./Sidebar";
import TopBar from "./TopBar";

function AppLayout() {
  return (
    <Box
      sx={{
        minHeight: "100vh",
        backgroundColor: "background.default",
      }}
    >
      <Sidebar />

      <TopBar />

      <Box
        component="main"
        sx={{
          ml: "250px",
          pt: "72px",
          minHeight: "100vh",
        }}
      >
        <Outlet />
      </Box>
    </Box>
  );
}

export default AppLayout;