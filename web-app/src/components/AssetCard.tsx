import type { Asset } from '../types/asset'
import { Minus, History } from 'lucide-react';

type Props = {
  asset: Asset;
  onViewDetails: (asset: Asset) => void;
};

export const AssetCard = ({ asset, onViewDetails }: Props) => {
  const currentPrice = asset.currentPrice ?? 0
  const averagePrice = asset.averagePrice ?? 0

  const priceColor = 'text-white';

  return (
    <div className="bg-gray-800 p-5 rounded-xl border border-gray-700 hover:border-blue-500 group transition-all duration-300">
      <div className="flex justify-between items-start mb-2">
        <div>
          <h3 className="text-xl font-bold tracking-tight">{asset.ticker}</h3>
          <p className="text-gray-400 text-xs uppercase">{asset.name}</p>
        </div>
        <div className="p-2 rounded-full bg-gray-700">
          <Minus size={16} className="text-gray-400" />
        </div>
      </div>

      <div className="mt-4">
        <div className="flex justify-between items-end">
          <span className="text-2xl font-mono font-bold text-white">
            R$ {currentPrice.toFixed(2)}
          </span>
          <div className="text-right">
             <p className="text-gray-500 text-[10px]">Preço Médio</p>
             <p className="text-sm font-mono text-gray-300">R$ {averagePrice.toFixed(2)}</p>
          </div>
        </div>
      </div>

      <div className="space-y-3">
        <div className="flex justify-between items-baseline">
          <span className="text-gray-400 text-sm">Preço Atual</span>
          <span className={`text-2xl font-mono font-bold transition-colors duration-500 ${priceColor}`}>
            R$ {currentPrice.toFixed(2)}
          </span>
        </div>
        
        <div className="pt-3 border-t border-gray-700 flex justify-between text-xs">
          <div className="text-gray-400">
            Preço Médio: <span className="text-gray-200">R$ {averagePrice.toFixed(2)}</span>
          </div>
          <div className="text-gray-400">
            Qtd: <span className="text-gray-200">{asset.quantity}</span>
          </div>
        </div>
      </div>

      <button
        type="button"
        onClick={() => onViewDetails(asset)}
        className="mt-4 w-full flex items-center justify-center gap-2 py-2 text-xs font-bold text-gray-400 hover:text-white bg-gray-900/50 hover:bg-gray-700 rounded-lg border border-gray-700 transition-all opacity-0 group-hover:opacity-100 focus:opacity-100 focus:outline-none focus:ring-2 focus:ring-blue-500/50"
      >
        <History size={14} aria-hidden /> Ver Detalhes
      </button>
    </div>
  );
};