package main

import (
	"log"
	"os"

	"github.com/gin-gonic/gin"
	"github.com/ureis/finance-gateway/internal/handlers"
	"github.com/ureis/finance-gateway/internal/middleware"
	"github.com/ureis/finance-gateway/internal/proxy"
	"github.com/ureis/finance-gateway/internal/rabbitmq"
	"github.com/ureis/finance-gateway/internal/websocket"
)

func main() {
	// Produção: evite debug mode por padrão
	if os.Getenv("GIN_MODE") == "" {
		gin.SetMode(gin.ReleaseMode)
	}

	r := gin.New()
	r.Use(gin.Logger(), gin.Recovery())

	// Não confie em todos os proxies; em dev, apenas loopback.
	// Em produção, configure via GIN_TRUSTED_PROXIES (ex: "10.0.0.0/8,192.168.0.0/16").
	if err := r.SetTrustedProxies(parseTrustedProxies(os.Getenv("GIN_TRUSTED_PROXIES"))); err != nil {
		log.Fatalf("Falha ao configurar trusted proxies: %v", err)
	}

	hub := websocket.NewHub()

	// Roda o Hub em uma goroutine
	go hub.Run()

	// Inicia o consumidor do RabbitMQ passando o hub
	rabbitmq.StartPriceConsumer(hub)

	r.Use(middleware.TracingMiddleware())

	// Rota WebSocket
	r.GET("/ws/prices", handlers.ServeWS(hub))

	r.POST("/api/login", handlers.Login)

	// Rotas protegidas: JWT validado no Gateway; X-User-ID repassado às APIs .NET
	authorized := r.Group("/api")
	authorized.Use(middleware.AuthMiddleware())
	{
		// Mapeia: /api/wallet/* -> /v1/* na Wallet API
		authorized.Any("/wallet/*path", proxy.ReverseProxyRewritePath("http://localhost:5212", "/api/wallet", "/v1"))
	}

	log.Println("Gateway rodando na porta 8080...")
	r.Run(":8080")
}

func parseTrustedProxies(value string) []string {
	if value == "" {
		return []string{"127.0.0.1", "::1"}
	}

	var proxies []string
	start := 0
	for i := 0; i <= len(value); i++ {
		if i == len(value) || value[i] == ',' {
			part := value[start:i]
			start = i + 1

			trimmed := ""
			for j := 0; j < len(part); j++ {
				if part[j] != ' ' && part[j] != '\t' && part[j] != '\n' && part[j] != '\r' {
					trimmed = part[j:]
					break
				}
			}
			// trim direita
			for len(trimmed) > 0 {
				last := trimmed[len(trimmed)-1]
				if last == ' ' || last == '\t' || last == '\n' || last == '\r' {
					trimmed = trimmed[:len(trimmed)-1]
					continue
				}
				break
			}

			if trimmed != "" {
				proxies = append(proxies, trimmed)
			}
		}
	}
	return proxies
}
