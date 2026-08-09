import api from "./api";
import type {
  LoginRequest,
  LoginResponse,
  RegisterRequest,
} from "../types/auth";

export const login = async (
  request: LoginRequest
): Promise<LoginResponse> => {
  const response = await api.post<LoginResponse>(
    "/Auth/login",
    request
  );

  return response.data;
};

export const register = async (
  request: RegisterRequest
): Promise<void> => {
  await api.post("/Auth/register", request);
};