import { api } from './api';

const TOKEN_KEY = 'token';

/**
 * Garante JWT no localStorage. O Gateway exige Bearer em /api/wallet/*;
 * /api/login é público e não usa o token.
 */
export async function ensureAuthToken(): Promise<void> {
  if (localStorage.getItem(TOKEN_KEY)) {
    return;
  }

  const { data } = await api.post<{ token: string }>('/login', {});
  if (!data?.token) {
    throw new Error('Resposta de login sem token');
  }
  localStorage.setItem(TOKEN_KEY, data.token);
}

export function clearAuthToken(): void {
  localStorage.removeItem(TOKEN_KEY);
}
