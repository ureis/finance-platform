import axios, { AxiosHeaders } from 'axios';

export const api = axios.create({
  baseURL: '/api'
});

api.interceptors.request.use((config) => {
  const token = localStorage.getItem('token');
  if (token) {
    // Axios v1 pode expor headers como AxiosHeaders; normalize e aplique o Bearer sempre.
    const headers = AxiosHeaders.from(config.headers);
    headers.set('Authorization', `Bearer ${token}`);
    config.headers = headers;
  }
  return config;
});