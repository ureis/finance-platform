package proxy

import (
	"net/http/httputil"
	"net/url"
	"strings"

	"github.com/gin-gonic/gin"
)

func ReverseProxy(target string) gin.HandlerFunc {
	url, _ := url.Parse(target)
	proxy := httputil.NewSingleHostReverseProxy(url)

	return func(c *gin.Context) {
		// Aqui o Go "finge" ser o cliente e repassa a chamada para o .NET
		proxy.ServeHTTP(c.Writer, c.Request)
	}
}

func ReverseProxyRewritePath(target string, fromPrefix string, toPrefix string) gin.HandlerFunc {
	url, _ := url.Parse(target)
	proxy := httputil.NewSingleHostReverseProxy(url)

	return func(c *gin.Context) {
		if fromPrefix != "" && strings.HasPrefix(c.Request.URL.Path, fromPrefix) {
			rest := strings.TrimPrefix(c.Request.URL.Path, fromPrefix)
			if rest == "" {
				rest = "/"
			}
			if !strings.HasPrefix(rest, "/") {
				rest = "/" + rest
			}
			c.Request.URL.Path = strings.TrimRight(toPrefix, "/") + rest
		}

		proxy.ServeHTTP(c.Writer, c.Request)
	}
}
