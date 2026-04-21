import { api } from './api';
import type { BuyAssetRequest } from '../types/asset';

export const buyAsset = async (data: {
  ticker: string;
  name: string;
  type: string;
  quantity: number;
  price: number;
}) => {
  // A chamada passa pelo Gateway Go (porta 8080)
  // O interceptor do axios injeta o Token automaticamente
  const response = await api.post('/wallet/assets/buy', data);
  return response.data;
};

export const walletService = {
    buyAsset: async (data: BuyAssetRequest) => {
      // A rota /wallet/v1/... é interceptada pelo Gateway Go
      const response = await api.post('/wallet/assets/buy', data);
      return response.data;
    },
    
    getAssets: async () => {
      const response = await api.get('/wallet/assets');
      return response.data;
    },

    getTransactions: async (assetId: string) => {
      const response = await api.get(`/wallet/assets/${assetId}/transactions`);
      return response.data;
    },
  };