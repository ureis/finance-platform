package middleware

import (
	"errors"
	"fmt"
	"net/http"
	"strings"

	"github.com/gin-gonic/gin"
	"github.com/golang-jwt/jwt/v5"
	"github.com/ureis/finance-gateway/internal/config"
)

func AuthMiddleware() gin.HandlerFunc {
	secretKey := config.JWTSecret()

	return func(c *gin.Context) {
		authHeader := c.GetHeader("Authorization")
		if authHeader == "" {
			c.AbortWithStatusJSON(http.StatusUnauthorized, gin.H{"error": "Token não fornecido"})
			return
		}

		tokenString := strings.TrimSpace(strings.TrimPrefix(authHeader, "Bearer"))
		if tokenString == "" {
			c.AbortWithStatusJSON(http.StatusUnauthorized, gin.H{"error": "Token não fornecido"})
			return
		}

		token, err := jwt.Parse(tokenString, func(token *jwt.Token) (interface{}, error) {
			if _, ok := token.Method.(*jwt.SigningMethodHMAC); !ok {
				return nil, fmt.Errorf("método de assinatura inesperado: %v", token.Header["alg"])
			}
			return secretKey, nil
		})

		if err != nil || token == nil || !token.Valid {
			detail := ""
			if err != nil {
				detail = err.Error()
			}
			c.AbortWithStatusJSON(http.StatusUnauthorized, gin.H{
				"error":   "Token inválido",
				"details": detail,
			})
			return
		}

		claims, ok := token.Claims.(jwt.MapClaims)
		if !ok {
			c.AbortWithStatusJSON(http.StatusUnauthorized, gin.H{"error": "Claims inválidas"})
			return
		}

		sub, err := subFromClaims(claims)
		if err != nil {
			c.AbortWithStatusJSON(http.StatusUnauthorized, gin.H{"error": err.Error()})
			return
		}

		c.Request.Header.Set("X-User-ID", sub)
		c.Next()
	}
}

func subFromClaims(claims jwt.MapClaims) (string, error) {
	raw, ok := claims["sub"]
	if !ok || raw == nil {
		return "", errors.New("token sem claim sub")
	}
	switch v := raw.(type) {
	case string:
		if strings.TrimSpace(v) == "" {
			return "", errors.New("sub vazio")
		}
		return v, nil
	case float64:
		return fmt.Sprintf("%.0f", v), nil
	default:
		return fmt.Sprint(v), nil
	}
}
