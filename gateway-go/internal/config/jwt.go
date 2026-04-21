package config

import "os"

// JWTSecret chave H256 compartilhada entre Login e AuthMiddleware.
// Em produção defina JWT_SECRET no ambiente.
func JWTSecret() []byte {
	s := os.Getenv("JWT_SECRET")
	if s == "" {
		return []byte("sua-chave-secreta-muito-forte")
	}
	return []byte(s)
}
