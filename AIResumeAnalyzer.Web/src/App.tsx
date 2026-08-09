import { Box, Typography } from "@mui/material";
import { Navigate, Route, Routes } from "react-router-dom";

import LoginPage from "./pages/auth/LoginPage";
import DashboardPage from "./pages/dashboard/DashboardPage";

import ProtectedRoute from "./routes/ProtectedRoute";
import AppLayout from "./components/layout/AppLayout";

interface PlaceholderPageProps {
  title: string;
}

function PlaceholderPage({
  title,
}: PlaceholderPageProps) {
  return (
    <Box
      sx={{
        p: 4,
      }}
    >
      <Typography
        variant="h4"
        sx={{
          fontWeight: 700,
          mb: 1,
        }}
      >
        {title}
      </Typography>

      <Typography color="text.secondary">
        AI Resume Analyzer
      </Typography>
    </Box>
  );
}

function App() {
  return (
    <Routes>
      {/* =====================================================
          Public Routes
          ===================================================== */}

      <Route
        path="/login"
        element={<LoginPage />}
      />

      <Route
        path="/register"
        element={
          <PlaceholderPage title="Register" />
        }
      />

      {/* =====================================================
          Protected Routes
          ===================================================== */}

      <Route element={<ProtectedRoute />}>
        <Route element={<AppLayout />}>

          {/* Dashboard */}

          <Route
            path="/dashboard"
            element={<DashboardPage />}
          />

          {/* Resume Management */}

          <Route
            path="/resumes"
            element={
              <PlaceholderPage
                title="Resume Management"
              />
            }
          />

          {/* ATS Analysis */}

          <Route
            path="/ats"
            element={
              <PlaceholderPage
                title="ATS Analysis"
              />
            }
          />

          {/* Cover Letter */}

          <Route
            path="/cover-letter"
            element={
              <PlaceholderPage
                title="Cover Letter"
              />
            }
          />

          {/* Interview */}

          <Route
            path="/interview"
            element={
              <PlaceholderPage
                title="Interview Questions"
              />
            }
          />

        </Route>
      </Route>

      {/* =====================================================
          Default Routes
          ===================================================== */}

      <Route
        path="/"
        element={
          <Navigate
            to="/login"
            replace
          />
        }
      />

      <Route
        path="*"
        element={
          <Navigate
            to="/login"
            replace
          />
        }
      />
    </Routes>
  );
}

export default App;