import type { Transaction } from '../types/asset';

type Props = {
  transactions: Transaction[];
};

export const TransactionList = ({ transactions }: Props) => {
  if (transactions.length === 0) {
    return (
      <p className="py-8 text-center text-sm text-gray-500">Nenhuma compra registrada.</p>
    );
  }

  return (
    <div className="overflow-x-auto">
      <table className="w-full text-left text-sm text-gray-300">
        <thead className="text-xs uppercase bg-gray-800/50 text-gray-500">
          <tr>
            <th className="px-4 py-3">Data</th>
            <th className="px-4 py-3">Qtd</th>
            <th className="px-4 py-3 text-right">Preço Unitário</th>
            <th className="px-4 py-3 text-right">Total</th>
          </tr>
        </thead>
        <tbody className="divide-y divide-gray-800">
          {transactions.map((t) => (
            <tr key={t.id} className="hover:bg-gray-800/30 transition-colors">
              <td className="px-4 py-3">
                {new Date(t.createdAt).toLocaleDateString('pt-BR')}
              </td>
              <td className="px-4 py-3 font-mono">{t.quantity}</td>
              <td className="px-4 py-3 text-right font-mono text-emerald-500">
                R$ {t.priceAtTime.amount.toFixed(2)}
              </td>
              <td className="px-4 py-3 text-right font-mono font-bold">
                R$ {(t.quantity * t.priceAtTime.amount).toFixed(2)}
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
};
