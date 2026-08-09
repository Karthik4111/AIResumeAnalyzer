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

import {
  Link,
  useNavigate,
} from "react-router-dom";

import {
  register as registerApi,
} from "../../services/authService";

function RegisterPage() {
  const navigate = useNavigate();

  const [firstName, setFirstName] = useState("");
  const [lastName, setLastName] = useState("");
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");

  const [error, setError] = useState("");
  const [success, setSuccess] = useState("");
  const [loading, setLoading] = useState(false);

  const handleSubmit = async (
    event: React.FormEvent<HTMLFormElement>
  ) => {
    event.preventDefault();

    setError("");
    setSuccess("");

    if (
      !firstName.trim() ||
      !lastName.trim() ||
      !email.trim() ||
      !password.trim()
    ) {
      setError(
        "Please fill in all fields."
      );

      return;
    }

    try {
      setLoading(true);

      await registerApi({
        firstName,
        lastName,
        email,
        password,
      });

      setSuccess(
        "Registration successful. Redirecting to login..."
      );

      setTimeout(() => {
        navigate("/login");
      }, 1500);
    } catch (error: any) {
      console.error(error);

      const message =
        error?.response?.data?.Message ||
        error?.response?.data?.message ||
        "Registration failed. Please check the entered details.";

      setError(message);
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
            maxWidth: 500,
            borderRadius: 3,
            boxShadow: 4,
          }}
        >
          <CardContent sx={{ p: 4 }}>
            {/* Header */}

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
                Create Account
              </Typography>

              <Typography color="text.secondary">
                Create your AI Resume Analyzer account
              </Typography>
            </Box>

            {/* Error */}

            {error && (
              <Alert
                severity="error"
                sx={{ mb: 3 }}
              >
                {error}
              </Alert>
            )}

            {/* Success */}

            {success && (
              <Alert
                severity="success"
                sx={{ mb: 3 }}
              >
                {success}
              </Alert>
            )}

            {/* Form */}

            <Box
              component="form"
              onSubmit={handleSubmit}
            >
              <TextField
                fullWidth
                label="First Name"
                value={firstName}
                onChange={(event) =>
                  setFirstName(event.target.value)
                }
                margin="normal"
                autoComplete="given-name"
              />

              <TextField
                fullWidth
                label="Last Name"
                value={lastName}
                onChange={(event) =>
                  setLastName(event.target.value)
                }
                margin="normal"
                autoComplete="family-name"
              />

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
                autoComplete="new-password"
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
                  <CircularProgress
                    size={24}
                    color="inherit"
                  />
                ) : (
                  "Create Account"
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
                  Already have an account?{" "}
                  <Link to="/login">
                    Login
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

export default RegisterPage;