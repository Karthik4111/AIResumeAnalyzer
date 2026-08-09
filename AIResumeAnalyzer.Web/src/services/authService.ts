import api from "./api";

import type {
  LoginRequest,
  LoginResponse,
  RegisterRequest,
} from "../types/auth";

export async function login(
  request: LoginRequest
): Promise<LoginResponse> {
  const response = await api.post<LoginResponse>(
    "/Auth/login",
    request
  );

  return response.data;
}

export async function register(
  request: RegisterRequest
): Promise<void> {
  await api.post(
    "/Auth/register",
    request
  );
}