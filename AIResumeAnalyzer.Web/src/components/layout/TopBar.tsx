import {
  AppBar,
  Avatar,
  Box,
  Button,
  Toolbar,
  Typography,
} from "@mui/material";

import LogoutIcon from "@mui/icons-material/Logout";

import { useNavigate } from "react-router-dom";
import { useAuth } from "../../context/AuthContext";

function TopBar() {
  const navigate = useNavigate();

  const {
    firstName,
    lastName,
    role,
    logout,
  } = useAuth();

  const handleLogout = () => {
    logout();
    navigate("/login");
  };

  const initials = `${firstName?.charAt(0) ?? ""}${lastName?.charAt(0) ?? ""}`;

  return (
    <AppBar
      position="fixed"
      elevation={0}
      sx={{
        width: "calc(100% - 250px)",
        ml: "250px",
        backgroundColor: "background.paper",
        color: "text.primary",
        borderBottom: "1px solid",
        borderColor: "divider",
      }}
    >
      <Toolbar
        sx={{
          minHeight: "72px !important",
          justifyContent: "flex-end",
          gap: 2,
        }}
      >
        <Avatar
          sx={{
            width: 40,
            height: 40,
            bgcolor: "primary.main",
          }}
        >
          {initials}
        </Avatar>

        <Box sx={{ mr: 1 }}>
          <Typography
            variant="body1"
            sx={{
              fontWeight: 600,
              lineHeight: 1.2,
            }}
          >
            {firstName} {lastName}
          </Typography>

          <Typography
            variant="caption"
            color="text.secondary"
          >
            {role}
          </Typography>
        </Box>

        <Button
          variant="outlined"
          color="inherit"
          startIcon={<LogoutIcon />}
          onClick={handleLogout}
          sx={{
            textTransform: "none",
            borderRadius: 2,
          }}
        >
          Logout
        </Button>
      </Toolbar>
    </AppBar>
  );
}

export default TopBar;