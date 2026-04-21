import { useEffect, useState } from 'react';
import { X, History, Info } from 'lucide-react';
import type { Asset, Transaction } from '../types/asset';
import { walletService } from '../services/walletService';
import { TransactionList } from './TransactionList';

type Props = {
  asset: Asset | null;
  isOpen: boolean;
  onClose: () => void;
};

export const AssetDetailModal = ({ asset, isOpen, onClose }: Props) => {
  const [transactions, setTransactions] = useState<Transaction[]>([]);
  const [loading, setLoading] = useState(false);
  const [loadError, setLoadError] = useState<string | null>(null);

  useEffect(() => {
    if (!isOpen || !asset) {
      return;
    }

    // eslint-disable-next-line react-hooks/set-state-in-effect
    setLoadError(null);
    setLoading(true);
    walletService
      .getTransactions(asset.id)
      .then((data) => {
        setTransactions(Array.isArray(data) ? data : []);
      })
      .catch(() => {
        setLoadError('Não foi possível carregar o histórico.');
        setTransactions([]);
      })
      .finally(() => {
        setLoading(false);
      });
  }, [isOpen, asset]);

  if (!isOpen || !asset) {
    return null;
  }

  return (
    <div
      className="fixed inset-0 bg-black/80 backdrop-blur-md flex items-center justify-center z-[60] p-4"
      role="dialog"
      aria-modal="true"
      aria-labelledby="asset-detail-title"
    >
      <div className="bg-gray-900 border border-gray-800 w-full max-w-2xl rounded-2xl shadow-2xl">
        <div className="p-6 border-b border-gray-800 flex justify-between items-center">
          <div className="flex items-center gap-3">
            <div className="bg-blue-500/10 p-2 rounded-lg text-blue-400">
              <Info size={24} aria-hidden />
            </div>
            <div>
              <h2 id="asset-detail-title" className="text-xl font-bold text-white">
                {asset.ticker}
              </h2>
              <p className="text-sm text-gray-500">{asset.name}</p>
            </div>
          </div>
          <button
            type="button"
            onClick={onClose}
            className="text-gray-500 hover:text-white transition-colors"
            aria-label="Fechar"
          >
            <X size={24} />
          </button>
        </div>

        <div className="p-6">
          <h3 className="text-gray-400 text-xs uppercase font-bold mb-4 flex items-center gap-2">
            <History size={14} aria-hidden /> Histórico de Compras
          </h3>

          {loading ? (
            <div className="h-40 flex items-center justify-center text-gray-500">
              Carregando histórico...
            </div>
          ) : loadError ? (
            <div className="h-40 flex items-center justify-center text-sm text-red-400">{loadError}</div>
          ) : (
            <TransactionList transactions={transactions} />
          )}
        </div>
      </div>
    </div>
  );
};
