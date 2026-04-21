import { create } from 'zustand';
import { persist } from 'zustand/middleware';
import type { Asset } from '../types/asset';

type ConnectionStatus = 'connected' | 'connecting' | 'disconnected';

interface AssetStore {
  assets: Asset[];
  status: ConnectionStatus;
  setAssets: (assets: Asset[]) => void;
  setStatus: (status: ConnectionStatus) => void;
  updatePrice: (ticker: string, newPrice: number) => void;
}

type PersistedSlice = Pick<AssetStore, 'assets'>;

export const useAssetStore = create<AssetStore>()(
  persist(
    (set) => ({
      assets: [],
      status: 'disconnected',
      setAssets: (assets) =>
        set({
          assets,
        }),
      setStatus: (status) => set({ status }),
      updatePrice: (ticker, newPrice) =>
        set((state) => {
          const next = Number(newPrice);
          if (!Number.isFinite(next)) {
            return state;
          }

          return {
            assets: state.assets.map((asset) => {
              if (asset.ticker !== ticker) {
                return asset;
              }
              return {
                ...asset,
                currentPrice: next,
              };
            }),
          };
        }),
    }),
    {
      name: 'finance-app-storage',
      version: 2,
      partialize: (state): PersistedSlice => ({ assets: state.assets }),
      migrate: (persisted): PersistedSlice => {
        const p = persisted as { assets?: Asset[] };
        const list = p.assets ?? [];
        return {
          assets: list,
        };
      },
    }
  )
);
