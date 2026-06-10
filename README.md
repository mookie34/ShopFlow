# ShopFlow

Event-driven **microservices** system built with **.NET 9**, demonstrating reliable inter-service communication, resilience, and eventual consistency in a distributed architecture.

The domain is a simplified e-commerce flow: a customer places an order, the system verifies stock, persists the order, and asynchronously updates inventory and notifies the customer.

---

## Architecture

Three independent services, each with its **own database** and **Clean Architecture** layering (Domain / Application / Infrastructure / API). This enforces true deploy and data isolation — no shared schema between services.

```
                       POST /api/orders
                              │
                              ▼
                   ┌──────────────────────┐
                   │     Orders.API       │
                   │  (owns Orders DB)    │
                   └──────────┬───────────┘
                              │
              [SYNC] HTTP + Polly (Retry + Circuit Breaker)
                              │
                              ▼
                   ┌──────────────────────┐
                   │   Inventory.API      │  ← stock check
                   │ (owns Inventory DB)  │
                   └──────────────────────┘

   Order persisted → OrderCreatedEvent written to Outbox table
                              │
              OutboxPublisherJob (Hangfire) publishes to broker
                              │
                       ┌──────┴───────┐  RabbitMQ (MassTransit)
                       ▼              ▼
            ┌────────────────┐  ┌────────────────────┐
            │ Inventory.API  │  │ Notification.API   │
            │ reduces stock  │  │ sends notification │
            │ (idempotent)   │  │   (idempotent)     │
            └────────────────┘  └────────────────────┘
```

### Services

| Service | Responsibility | Database | HTTP Port |
|---|---|---|---|
| **Orders** | Receives orders, validates stock synchronously, persists order, publishes `OrderCreatedEvent` | Orders DB | 5132 |
| **Inventory** | Owns stock; serves synchronous stock checks and consumes `OrderCreatedEvent` to reduce stock | Inventory DB | 5156 |
| **Notification** | Consumes `OrderCreatedEvent` to notify the customer | Notification DB | 5099 |

`Shared.Events` is a small shared contract project holding the integration event (`OrderCreatedEvent`).

---

## Key patterns implemented

- **Synchronous + asynchronous communication in a single use case** — synchronous HTTP for the stock check (where the answer is needed immediately), asynchronous events for stock reduction and notification (where eventual consistency is acceptable).
- **Resilience with Polly** — the Orders → Inventory call is wrapped with **Retry** and **Circuit Breaker** policies (Circuit Breaker outer, Retry inner). The API surfaces `503` when the circuit is open (`BrokenCircuitException`).
- **Outbox Pattern** — the order and its `OrderCreatedEvent` are written in the same transaction to an outbox table; a background `OutboxPublisherJob` (Hangfire) publishes pending messages to the broker, solving the dual-write problem (the order and the event can never go out of sync).
- **Idempotent consumers** — each consuming service tracks processed `EventId`s in a `ProcessedEvents` table, so redelivered messages produce no duplicate side effects.
- **CQRS** with MediatR and FluentValidation in the Orders service.

---

## Tech stack

- .NET 9, ASP.NET Core Web API
- Entity Framework Core 9 (Code First, migrations)
- MassTransit 8 + RabbitMQ (message broker)
- Polly (`Microsoft.Extensions.Http.Polly`) for resilience
- Hangfire (Outbox publisher background job)
- MediatR + FluentValidation
- SQL Server
- Docker Compose (SQL Server + RabbitMQ)

---

## Running locally

### Prerequisites
- .NET 9 SDK
- Docker

### 1. Start infrastructure (SQL Server + RabbitMQ)

```bash
docker compose up -d
```

RabbitMQ management UI: http://localhost:15672 (user: `guest` / pass: `guest`)

### 2. Apply database migrations

Each service has its own database. From each service's Infrastructure project:

```bash
dotnet ef database update --project Orders.Infrastructure --startup-project Orders.API
dotnet ef database update --project Inventory.Infrastructure --startup-project Inventory.API
dotnet ef database update --project Notification.Infrastructure --startup-project Notification.API
```

### 3. Run the services

In separate terminals:

```bash
dotnet run --project Orders.API
dotnet run --project Inventory.API
dotnet run --project Notification.API
```

Swagger is available per service (e.g. http://localhost:5132/swagger for Orders).

### 4. Try the flow

```bash
curl -X POST http://localhost:5132/api/orders \
  -H "Content-Type: application/json" \
  -d '{
        "customerId": "00000000-0000-0000-0000-000000000001",
        "productId":  "00000000-0000-0000-0000-000000000010",
        "quantity": 2
      }'
```

Expected behaviour:
1. Orders calls Inventory synchronously to confirm stock.
2. The order is persisted and `OrderCreatedEvent` is written to the outbox.
3. The outbox job publishes the event to RabbitMQ.
4. Inventory reduces stock and Notification processes the customer notification — both idempotently.

---

## Scope and roadmap

This project focuses on the **communication, resilience, and consistency concerns** of microservices. The following are intentionally out of scope for this iteration and are candidate next steps:

- API Gateway (single external entry point) and Service Discovery
- Kubernetes manifests for orchestration
- Automated test suite (unit + integration)
- CI/CD pipeline

---

## Author

**Santiago Mazo Padierna** — Backend Software Engineer
[github.com/mookie34](https://github.com/mookie34)
