import { Box, Typography } from "@mui/material";
import { Navigate, Route, Routes } from "react-router-dom";

function PlaceholderPage({ title }: { title: string }) {
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
      <Typography variant="h4" fontWeight={700}>
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
      <Route
        path="/login"
        element={<PlaceholderPage title="Login" />}
      />

      <Route
        path="/register"
        element={<PlaceholderPage title="Register" />}
      />

      <Route
        path="/dashboard"
        element={<PlaceholderPage title="Dashboard" />}
      />

      <Route
        path="/resumes"
        element={<PlaceholderPage title="Resume Management" />}
      />

      <Route
        path="/ats"
        element={<PlaceholderPage title="ATS Analysis" />}
      />

      <Route
        path="/cover-letter"
        element={<PlaceholderPage title="Cover Letter" />}
      />

      <Route
        path="/interview"
        element={<PlaceholderPage title="Interview Questions" />}
      />

      <Route
        path="/"
        element={<Navigate to="/login" replace />}
      />

      <Route
        path="*"
        element={<Navigate to="/login" replace />}
      />
    </Routes>
  );
}

export default App;