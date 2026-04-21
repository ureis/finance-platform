import React, { useState } from 'react';
import { X, PlusCircle, Loader2 } from 'lucide-react';
import type { BuyAssetRequest } from '../types/asset';
import { walletService } from '../services/walletService';

interface Props {
  isOpen: boolean;
  onClose: () => void;
  onSuccess: () => void;
}

export const AddAssetModal: React.FC<Props> = ({ isOpen, onClose, onSuccess }) => {
  const [loading, setLoading] = useState(false);
  const [formData, setFormData] = useState<BuyAssetRequest>({
    ticker: '', name: '', type: 'Stocks', quantity: 0, price: 0
  });

  if (!isOpen) return null;

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setLoading(true);
    try {
      await walletService.buyAsset(formData);
      onSuccess();
      onClose();
    } catch {
      alert("Erro ao cadastrar ativo. Verifique os dados.");
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="fixed inset-0 bg-black/60 backdrop-blur-sm flex items-center justify-center z-50 p-4">
      <div className="bg-gray-900 border border-gray-800 w-full max-w-md rounded-2xl shadow-2xl overflow-hidden">
        <div className="p-6 border-b border-gray-800 flex justify-between items-center">
          <h2 className="text-xl font-bold text-white flex items-center gap-2">
            <PlusCircle className="text-blue-500" /> Novo Ativo
          </h2>
          <button onClick={onClose} className="text-gray-400 hover:text-white transition-colors">
            <X size={20} />
          </button>
        </div>

        <form onSubmit={handleSubmit} className="p-6 space-y-4">
          <div className="grid grid-cols-2 gap-4">
            <div className="space-y-1">
              <label className="text-xs text-gray-400 uppercase font-semibold">Ticker</label>
              <input required maxLength={10} className="w-full bg-gray-800 border border-gray-700 rounded-lg p-2 text-white focus:ring-2 focus:ring-blue-500 outline-none" 
                     placeholder="Ex: PETR4" value={formData.ticker} onChange={e => setFormData({...formData, ticker: e.target.value.toUpperCase()})} />
            </div>
            <div className="space-y-1">
              <label className="text-xs text-gray-400 uppercase font-semibold">Tipo</label>
              <select className="w-full bg-gray-800 border border-gray-700 rounded-lg p-2 text-white outline-none"
                      value={formData.type} onChange={(e) => setFormData({ ...formData, type: e.target.value as BuyAssetRequest['type'] })}>
                <option value="Stocks">Ações</option>
                <option value="CDB">CDB</option>
                <option value="LCA">LCA</option>
                <option value="LCI">LCI</option>
                <option value="FixedIncome">Renda Fixa</option>
              </select>
            </div>
          </div>

          <div className="space-y-1">
            <label className="text-xs text-gray-400 uppercase font-semibold">Nome do Ativo</label>
            <input required className="w-full bg-gray-800 border border-gray-700 rounded-lg p-2 text-white outline-none" 
                   placeholder="Ex: Petrobras S.A." value={formData.name} onChange={e => setFormData({...formData, name: e.target.value})} />
          </div>

          <div className="grid grid-cols-2 gap-4">
            <div className="space-y-1">
              <label className="text-xs text-gray-400 uppercase font-semibold">Quantidade</label>
              <input required type="number" step="0.01" className="w-full bg-gray-800 border border-gray-700 rounded-lg p-2 text-white outline-none" 
                     value={formData.quantity} onChange={e => setFormData({...formData, quantity: Number(e.target.value)})} />
            </div>
            <div className="space-y-1">
              <label className="text-xs text-gray-400 uppercase font-semibold">Preço Unitário</label>
              <input required type="number" step="0.01" className="w-full bg-gray-800 border border-gray-700 rounded-lg p-2 text-white outline-none" 
                     value={formData.price} onChange={e => setFormData({...formData, price: Number(e.target.value)})} />
            </div>
          </div>

          <button type="submit" disabled={loading} className="w-full bg-blue-600 hover:bg-blue-700 disabled:bg-gray-700 text-white font-bold py-3 rounded-xl transition-all flex items-center justify-center gap-2 mt-4">
            {loading ? <Loader2 className="animate-spin" /> : "Confirmar Compra"}
          </button>
        </form>
      </div>
    </div>
  );
};