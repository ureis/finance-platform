import { useEffect, useRef } from 'react';
import { useAssetStore } from '../store/useAssetStore';

export const usePriceWebSocket = () => {
  const updatePrice = useAssetStore((s) => s.updatePrice)
  const setStatus = useAssetStore((s) => s.setStatus)
  const socketRef = useRef<WebSocket | null>(null);
  const reconnectTimeoutRef = useRef<number | undefined>(undefined);
  const unmountingRef = useRef(false)
  const connectingRef = useRef(false)

  useEffect(() => {
    unmountingRef.current = false

    const connect = () => {
      if (unmountingRef.current) return
      if (connectingRef.current) return
      if (socketRef.current && (socketRef.current.readyState === WebSocket.OPEN || socketRef.current.readyState === WebSocket.CONNECTING)) {
        return
      }

      connectingRef.current = true
      setStatus('connecting');
      const socket = new WebSocket('ws://localhost:8080/ws/prices');

      socket.onopen = () => {
        console.log('WS Conectado');
        connectingRef.current = false
        setStatus('connected');
      };

      socket.onmessage = (event) => {
        const data = JSON.parse(event.data);
        updatePrice(data.ticker, data.newPrice);
      };

      socket.onclose = () => {
        connectingRef.current = false
        setStatus('disconnected');
        // Tenta reconectar após 5 segundos
        if (!unmountingRef.current) {
          reconnectTimeoutRef.current = window.setTimeout(connect, 5000);
        }
      };

      socket.onerror = (err) => {
        // Em dev (StrictMode), o efeito pode desmontar rapidamente e fechar o socket
        if (!unmountingRef.current) console.error('WS Erro:', err);
        // Evita fechar um socket que ainda está CONNECTING (gera warning no browser)
        if (!unmountingRef.current && socket.readyState === WebSocket.OPEN) {
          socket.close();
        }
      };

      socketRef.current = socket;
    };

    connect();
    return () => {
      unmountingRef.current = true
      if (reconnectTimeoutRef.current) clearTimeout(reconnectTimeoutRef.current);
      const sock = socketRef.current
      // Não feche durante CONNECTING: isso dispara "closed before the connection is established" no console
      if (sock && sock.readyState === WebSocket.OPEN) sock.close()
    };
  }, [setStatus, updatePrice]);
};