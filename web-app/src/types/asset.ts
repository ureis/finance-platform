export interface Asset {
  id: string;
  ticker: string;
  name: string;
  type: string;
  quantity: number;
  averagePrice: number;
  currentPrice: number;
}

export interface Transaction {
  id: string;
  assetId: string;
  quantity: number;
  priceAtTime: {
    amount: number;
    currency: string;
  };
  createdAt: string;
}

export interface BuyAssetRequest {
  ticker: string;
  name: string;
  type: 'CDB' | 'Stocks' | 'LCA' | 'LCI' | 'FixedIncome';
  quantity: number;
  price: number;
}