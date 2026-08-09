import { Box, Typography } from "@mui/material";
import { Navigate, Route, Routes } from "react-router-dom";

import LoginPage from "./pages/auth/LoginPage";
import ProtectedRoute from "./routes/ProtectedRoute";
import DashboardPage from "./pages/dashboard/DashboardPage";

interface PlaceholderPageProps {
  title: string;
}

function PlaceholderPage({ title }: PlaceholderPageProps) {
  return (
    <Box
      sx={{
        minHeight: "100vh",
        display: "flex",
        alignItems: "center",
        justifyContent: "center",
        flexDirection: "column",
        gap: 1,
      }}
    >
      <Typography
        variant="h4"
        sx={{ fontWeight: 700 }}
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

      {/* Public Routes */}

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

      {/* Protected Routes */}

      <Route element={<ProtectedRoute />}>

        <Route
          path="/dashboard"
          element={<DashboardPage />}
        />

        <Route
          path="/resumes"
          element={
            <PlaceholderPage title="Resume Management" />
          }
        />

        <Route
          path="/ats"
          element={
            <PlaceholderPage title="ATS Analysis" />
          }
        />

        <Route
          path="/cover-letter"
          element={
            <PlaceholderPage title="Cover Letter" />
          }
        />

        <Route
          path="/interview"
          element={
            <PlaceholderPage title="Interview Questions" />
          }
        />

      </Route>

      {/* Default */}

      <Route
        path="/"
        element={
          <Navigate to="/login" replace />
        }
      />

      <Route
        path="*"
        element={
          <Navigate to="/login" replace />
        }
      />

    </Routes>
  );
}

export default App;