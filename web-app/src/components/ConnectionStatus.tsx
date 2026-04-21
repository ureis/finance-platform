import { useAssetStore } from '../store/useAssetStore';
import { Wifi, WifiOff, RefreshCw } from 'lucide-react';

export const ConnectionStatus = () => {
  const status = useAssetStore((state) => state.status);

  const config = {
    connected: { color: 'text-emerald-400', icon: <Wifi size={16} />, label: 'Online' },
    connecting: { color: 'text-amber-400', icon: <RefreshCw size={16} className="animate-spin" />, label: 'Reconectando' },
    disconnected: { color: 'text-red-400', icon: <WifiOff size={16} />, label: 'Offline' },
  };

  const current = config[status];

  return (
    <div className={`flex items-center gap-2 px-3 py-1 rounded-full bg-gray-800 border border-gray-700 ${current.color}`}>
      {current.icon}
      <span className="text-xs font-medium uppercase tracking-wider">{current.label}</span>
    </div>
  );
};