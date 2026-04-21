package rabbitmq

import (
	"encoding/json"
	"log"

	amqp "github.com/rabbitmq/amqp091-go"
	"github.com/ureis/finance-gateway/internal/websocket"
)

type PriceEvent struct {
	Ticker   string  `json:"ticker"`
	NewPrice float64 `json:"newPrice"`
}

func StartPriceConsumer(hub *websocket.Hub) {
	conn, err := amqp.Dial("amqp://guest:guest@localhost:5672/")
	if err != nil {
		log.Fatalf("Falha ao conectar no RabbitMQ: %v", err)
	}

	ch, err := conn.Channel()
	if err != nil {
		log.Fatalf("Falha ao abrir canal: %v", err)
	}

	// Nota: MassTransit usa o nome do tipo como Exchange.
	// Ajuste o nome conforme o que aparece no seu Management do RabbitMQ.
	q, err := ch.QueueDeclare(
		"gateway-websocket-queue", // nome da fila
		false, false, false, false, nil,
	)

	// Bind da fila com a exchange de cotações
	err = ch.QueueBind(q.Name, "", "Quotes.Contracts:AssetPriceUpdatedEvent", false, nil)

	msgs, err := ch.Consume(q.Name, "", true, false, false, false, nil)

	go func() {
		for d := range msgs {
			var event PriceEvent
			if err := json.Unmarshal(d.Body, &event); err == nil {
				// Envia para o Hub fazer o broadcast para os WebSockets
				hub.BroadcastPrice(event)
			}
		}
	}()
}
