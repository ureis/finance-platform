package middleware

import (
	"github.com/gin-gonic/gin"
	"github.com/google/uuid"
)

// TracingMiddleware garante X-Correlation-ID em toda a cadeia (resposta, request ao proxy e contexto Gin).
func TracingMiddleware() gin.HandlerFunc {
	return func(c *gin.Context) {
		correlationID := c.GetHeader("X-Correlation-ID")
		if correlationID == "" {
			correlationID = uuid.New().String()
		}

		c.Set("CorrelationID", correlationID)
		c.Header("X-Correlation-ID", correlationID)
		// Repasse ao reverse proxy (.NET)
		c.Request.Header.Set("X-Correlation-ID", correlationID)

		c.Next()
	}
}
