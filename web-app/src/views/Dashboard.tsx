import { useCallback, useEffect, useMemo, useState } from 'react'
import { AddAssetModal } from '../components/AddAssetModal'
import { AssetDetailModal } from '../components/AssetDetailModal'
import { ensureAuthToken } from '../services/authService'
import { walletService } from '../services/walletService'
import { AssetCard } from '../components/AssetCard'
import { ConnectionStatus } from '../components/ConnectionStatus'
import { usePriceWebSocket } from '../hooks/usePriceWebSocket'
import { useAssetStore } from '../store/useAssetStore'
import type { Asset } from '../types/asset'

export const Dashboard = () => {
  const assets = useAssetStore((s) => s.assets)
  const setAssets = useAssetStore((s) => s.setAssets)
  const [loading, setLoading] = useState(true)
  const [loadError, setLoadError] = useState<string | null>(null)
  const [isModalOpen, setIsModalOpen] = useState(false)
  const [selectedAsset, setSelectedAsset] = useState<Asset | null>(null)

  // Ativa o real-time (Gateway Go -> WS)
  usePriceWebSocket()

  const loadData = useCallback(async () => {
    try {
      setLoadError(null)
      setLoading(true)
      await ensureAuthToken()
      const data = await walletService.getAssets()
      setAssets(data)
    } catch (err) {
      console.error('Falha ao buscar ativos', err)
      setLoadError(
        'Falha ao autenticar ou buscar ativos. Confirme Gateway (8080), Wallet API e login em /api/login.'
      )
    } finally {
      setLoading(false)
    }
  }, [setAssets])

  useEffect(() => {
    // eslint-disable-next-line react-hooks/set-state-in-effect
    loadData()
  }, [loadData])

  const hasAssets = useMemo(() => assets.length > 0, [assets.length])

  return (
    <div className="min-h-screen bg-gray-950 text-gray-100 p-6">
      <div className="max-w-7xl mx-auto">
        <header className="flex justify-between items-center mb-10">
          <div>
            <h1 className="text-3xl font-extrabold bg-clip-text text-transparent bg-gradient-to-r from-blue-400 to-emerald-400">
              Minha Carteira
            </h1>
          </div>
          
          <div className="flex items-center gap-4">
            <ConnectionStatus />
            <button 
              onClick={() => setIsModalOpen(true)}
              className="bg-blue-600 hover:bg-blue-700 px-4 py-2 rounded-lg font-bold transition-all shadow-lg shadow-blue-900/20 active:scale-95"
            >
              + Novo Ativo
            </button>
          </div>
        </header>

        {loading ? (
          <div className="rounded-xl border border-gray-800 bg-gray-900/30 p-6 text-sm text-gray-300">
            Carregando ativos...
          </div>
        ) : loadError ? (
          <div className="rounded-xl border border-red-900/40 bg-red-950/20 p-6 text-sm text-red-200">
            {loadError}
          </div>
        ) : !hasAssets ? (
          <div className="rounded-xl border border-gray-800 bg-gray-900/30 p-6 text-sm text-gray-300">
            Nenhum ativo cadastrado ainda.
            <div className="mt-2 text-xs text-gray-400">
              Crie um ativo pelo Swagger (Wallet API) via <span className="font-mono">POST /v1/assets/buy</span> e recarregue.
            </div>
          </div>
        ) : (
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-6">
            {assets.map((asset) => (
              <AssetCard
                key={asset.id}
                asset={asset}
                onViewDetails={(a) => setSelectedAsset(a)}
              />
            ))}
          </div>
        )}
        <AssetDetailModal
          asset={selectedAsset}
          isOpen={!!selectedAsset}
          onClose={() => setSelectedAsset(null)}
        />

        <AddAssetModal 
          isOpen={isModalOpen} 
          onClose={() => setIsModalOpen(false)} 
          onSuccess={loadData} 
        />
      </div>
    </div>
  );
};