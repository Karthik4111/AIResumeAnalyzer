import {
  Box,
  Card,
  CardContent,
  Grid,
  Typography,
} from "@mui/material";

import { useAuth } from "../../context/AuthContext";

function DashboardPage() {
  const {
    firstName,
    lastName,
    email,
    role,
  } = useAuth();

  return (
    <Box sx={{ p: 4 }}>
      <Typography
        variant="h4"
        sx={{
          fontWeight: 700,
          mb: 1,
        }}
      >
        Welcome, {firstName} {lastName} 👋
      </Typography>

      <Typography
        color="text.secondary"
        sx={{ mb: 4 }}
      >
        Welcome to your AI Resume Analyzer dashboard.
      </Typography>

      <Grid container spacing={3}>
        <Grid size={{ xs: 12, md: 4 }}>
          <Card>
            <CardContent>
              <Typography
                variant="body2"
                color="text.secondary"
              >
                Email
              </Typography>

              <Typography
                variant="h6"
                sx={{ mt: 1, fontWeight: 600 }}
              >
                {email}
              </Typography>
            </CardContent>
          </Card>
        </Grid>

        <Grid size={{ xs: 12, md: 4 }}>
          <Card>
            <CardContent>
              <Typography
                variant="body2"
                color="text.secondary"
              >
                Role
              </Typography>

              <Typography
                variant="h6"
                sx={{ mt: 1, fontWeight: 600 }}
              >
                {role}
              </Typography>
            </CardContent>
          </Card>
        </Grid>

        <Grid size={{ xs: 12, md: 4 }}>
          <Card>
            <CardContent>
              <Typography
                variant="body2"
                color="text.secondary"
              >
                Resume Analysis
              </Typography>

              <Typography
                variant="h6"
                sx={{ mt: 1, fontWeight: 600 }}
              >
                Ready
              </Typography>
            </CardContent>
          </Card>
        </Grid>
      </Grid>
    </Box>
  );
}

export default DashboardPage;