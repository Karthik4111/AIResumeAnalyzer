import {
  createContext,
  useContext,
  useEffect,
  useState,
  type ReactNode,
} from "react";

interface AuthContextType {
  token: string | null;
  firstName: string | null;
  lastName: string | null;
  email: string | null;
  role: string | null;
  expiresAt: string | null;
  isAuthenticated: boolean;

  login: (
    token: string,
    firstName: string,
    lastName: string,
    email: string,
    role: string,
    expiresAt: string
  ) => void;

  logout: () => void;
}

const AuthContext = createContext<AuthContextType | undefined>(
  undefined
);

export function AuthProvider({
  children,
}: {
  children: ReactNode;
}) {
  const [token, setToken] = useState<string | null>(
    localStorage.getItem("token")
  );

  const [firstName, setFirstName] = useState<string | null>(
    localStorage.getItem("firstName")
  );

  const [lastName, setLastName] = useState<string | null>(
    localStorage.getItem("lastName")
  );

  const [email, setEmail] = useState<string | null>(
    localStorage.getItem("email")
  );

  const [role, setRole] = useState<string | null>(
    localStorage.getItem("role")
  );

  const [expiresAt, setExpiresAt] = useState<string | null>(
    localStorage.getItem("expiresAt")
  );

  useEffect(() => {
    if (token) {
      localStorage.setItem("token", token);
    } else {
      localStorage.removeItem("token");
    }

    if (firstName) {
      localStorage.setItem("firstName", firstName);
    } else {
      localStorage.removeItem("firstName");
    }

    if (lastName) {
      localStorage.setItem("lastName", lastName);
    } else {
      localStorage.removeItem("lastName");
    }

    if (email) {
      localStorage.setItem("email", email);
    } else {
      localStorage.removeItem("email");
    }

    if (role) {
      localStorage.setItem("role", role);
    } else {
      localStorage.removeItem("role");
    }

    if (expiresAt) {
      localStorage.setItem("expiresAt", expiresAt);
    } else {
      localStorage.removeItem("expiresAt");
    }
  }, [
    token,
    firstName,
    lastName,
    email,
    role,
    expiresAt,
  ]);

  const login = (
    newToken: string,
    newFirstName: string,
    newLastName: string,
    newEmail: string,
    newRole: string,
    newExpiresAt: string
  ) => {
    setToken(newToken);
    setFirstName(newFirstName);
    setLastName(newLastName);
    setEmail(newEmail);
    setRole(newRole);
    setExpiresAt(newExpiresAt);
  };

  const logout = () => {
    setToken(null);
    setFirstName(null);
    setLastName(null);
    setEmail(null);
    setRole(null);
    setExpiresAt(null);
  };

  return (
    <AuthContext.Provider
      value={{
        token,
        firstName,
        lastName,
        email,
        role,
        expiresAt,
        isAuthenticated: !!token,
        login,
        logout,
      }}
    >
      {children}
    </AuthContext.Provider>
  );
}

export function useAuth() {
  const context = useContext(AuthContext);

  if (!context) {
    throw new Error(
      "useAuth must be used inside AuthProvider"
    );
  }

  return context;
}