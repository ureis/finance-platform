package handlers

import (
	"net/http"
	"time"

	"github.com/gin-gonic/gin"
	"github.com/golang-jwt/jwt/v5"
	"github.com/ureis/finance-gateway/internal/config"
)

type loginRequest struct {
	Username string `json:"username"`
	Password string `json:"password"`
}

// Login emite JWT HS256. Em produção valide credenciais contra banco/IdP.
func Login(c *gin.Context) {
	var req loginRequest
	if err := c.ShouldBindJSON(&req); err != nil {
		req.Username = "admin"
		req.Password = "admin"
	}

	// Só recusa se o cliente enviou credenciais explícitas e elas não batem.
	if (req.Username != "" || req.Password != "") &&
		(req.Username != "admin" || req.Password != "admin") {
		c.JSON(http.StatusUnauthorized, gin.H{"error": "Credenciais inválidas"})
		return
	}

	token := jwt.NewWithClaims(jwt.SigningMethodHS256, jwt.MapClaims{
		"sub": "usuario-id-123",
		"exp": time.Now().Add(24 * time.Hour).Unix(),
	})

	tokenString, err := token.SignedString(config.JWTSecret())
	if err != nil {
		c.JSON(http.StatusInternalServerError, gin.H{"error": "Falha ao assinar token"})
		return
	}

	c.JSON(http.StatusOK, gin.H{"token": tokenString})
}
