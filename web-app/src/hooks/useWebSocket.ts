import { useEffect } from 'react';
import { useAssetStore } from '../store/useAssetStore';

export const useWebSocket = () => {
  const updatePrice = useAssetStore((state) => state.updatePrice);

  useEffect(() => {
    const socket = new WebSocket('ws://localhost:8080/ws/prices');

    socket.onmessage = (event) => {
      const data = JSON.parse(event.data);
      // data = { ticker: "PETR4", newPrice: 35.50 }
      updatePrice(data.ticker, data.newPrice);
    };

    return () => socket.close();
  }, [updatePrice]);
};

export const usePriceWebSocket = () => {
  const updatePrice = useAssetStore((state) => state.updatePrice);

  useEffect(() => {
    // Endereço do nosso Gateway Go
    const socket = new WebSocket('ws://localhost:8080/ws/prices');

    socket.onmessage = (event) => {
      try {
        const data = JSON.parse(event.data);
        // O Gateway Go envia: { "ticker": "PETR4", "newPrice": 34.50 }
        updatePrice(data.ticker, data.newPrice);
      } catch (err) {
        console.error("Erro ao processar dados do WebSocket", err);
      }
    };

    socket.onerror = (error) => console.error("WebSocket Error:", error);

    return () => socket.close(); // Cleanup ao destruir o componente
  }, [updatePrice]);
};