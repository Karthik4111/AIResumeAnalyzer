import { useState } from "react";
import {
  Alert,
  Box,
  Button,
  Card,
  CardContent,
  CircularProgress,
  Container,
  TextField,
  Typography,
} from "@mui/material";
import { Link, useNavigate } from "react-router-dom";

import { login as loginApi } from "../../services/authService";
import { useAuth } from "../../context/AuthContext";

function LoginPage() {
  const navigate = useNavigate();
  const { login } = useAuth();

  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");

  const [error, setError] = useState("");
  const [loading, setLoading] = useState(false);

  const handleSubmit = async (
    event: React.FormEvent<HTMLFormElement>
  ) => {
    event.preventDefault();

    setError("");

    if (!email.trim() || !password.trim()) {
  setError("Email and password are required.");
  return;
}

    try {
      setLoading(true);

      const response = await loginApi({
  email,
  password,
});

      login(
        response.token,
        response.firstName,
        response.lastName,
        response.email,
        response.role,
        response.expiresAt
        );

      navigate("/dashboard");
    } catch (error) {
      console.error(error);

      setError(
        "Login failed. Please check your username and password."
      );
    } finally {
      setLoading(false);
    }
  };

  return (
    <Container maxWidth="sm">
      <Box
        sx={{
          minHeight: "100vh",
          display: "flex",
          alignItems: "center",
          justifyContent: "center",
        }}
      >
        <Card
          sx={{
            width: "100%",
            maxWidth: 450,
            borderRadius: 3,
            boxShadow: 4,
          }}
        >
          <CardContent sx={{ p: 4 }}>
            <Box
              sx={{
                textAlign: "center",
                mb: 4,
              }}
            >
              <Typography
                variant="h4"
                sx={{
                  fontWeight: 700,
                  mb: 1,
                }}
              >
                AI Resume Analyzer
              </Typography>

              <Typography color="text.secondary">
                Sign in to continue
              </Typography>
            </Box>

            {error && (
              <Alert severity="error" sx={{ mb: 3 }}>
                {error}
              </Alert>
            )}

            <Box
              component="form"
              onSubmit={handleSubmit}
            >
           <TextField
  fullWidth
  label="Email"
  type="email"
  value={email}
  onChange={(event) =>
    setEmail(event.target.value)
  }
  margin="normal"
  autoComplete="email"
/>
              <TextField
                fullWidth
                label="Password"
                type="password"
                value={password}
                onChange={(event) =>
                  setPassword(event.target.value)
                }
                margin="normal"
                autoComplete="current-password"
              />

              <Button
                type="submit"
                fullWidth
                variant="contained"
                size="large"
                disabled={loading}
                sx={{
                  mt: 3,
                  py: 1.5,
                  fontWeight: 600,
                }}
              >
                {loading ? (
                  <CircularProgress size={24} color="inherit" />
                ) : (
                  "Login"
                )}
              </Button>

              <Box
                sx={{
                  textAlign: "center",
                  mt: 3,
                }}
              >
                <Typography
                  variant="body2"
                  color="text.secondary"
                >
                  Don't have an account?{" "}
                  <Link to="/register">
                    Register
                  </Link>
                </Typography>
              </Box>
            </Box>
          </CardContent>
        </Card>
      </Box>
    </Container>
  );
}

export default LoginPage;