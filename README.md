# Digital Wallet - Microservices Architecture

A modern digital wallet system built with .NET 8 and microservices architecture, demonstrating clean architecture principles, CQRS pattern, and domain-driven design.

## 🎯 Overview

This project is a reference implementation of a digital wallet platform that handles wallet management and financial transactions. It demonstrates the architecture and patterns; it is not the production system, and it still lacks authentication, idempotency and other features a production wallet needs (see the Roadmap).

## ✨ Key Features

- **Microservices Architecture**: Independent, scalable services
- **Clean Architecture**: Clear separation of concerns across layers
- **CQRS Pattern**: Command and Query segregation using MediatR
- **Domain-Driven Design**: Rich domain models with business logic
- **Event-Driven**: Domain events for decoupled communication
- **Logging & Monitoring**: Serilog with Seq integration
- **API Documentation**: Interactive Swagger/OpenAPI documentation
- **Validation**: FluentValidation for input validation
- **Entity Framework Core**: Modern ORM with migrations
- **Docker Support**: Containerized infrastructure services

## 🏗️ Architecture

### Services

#### 1. Wallet Service
**Port**: 5002 (HTTP) / 7002 (HTTPS)

Manages user wallets with the following capabilities:
- Create new wallets with unique codes
- Deposit money
- Withdraw money
- Check balance and wallet details
- Currency support (IRR)

**Key Components**:
- Domain: Wallet entity, Money value object, WalletCode
- Application: CQRS commands/queries with validation
- Infrastructure: EF Core with SQL Server
- API: RESTful endpoints with global exception handling

#### 2. Transaction Service
**Port**: 5003 (HTTP) / 7003 (HTTPS)

Records and tracks all financial transactions:
- Record transactions (Deposit, Withdrawal, Transfer, Refund, Fee)
- Query by transaction ID, wallet, or user
- Filter by type, status, and date range
- Transaction statistics and analytics
- Unique reference code generation
- Health check endpoints

**Key Components**:
- Domain: Transaction entity, TransactionType enum, domain events
- Application: Complete CQRS implementation
- Infrastructure: EF Core with database seeding
- API: RESTful endpoints with comprehensive error handling

### Architecture Layers

Each service follows Clean Architecture principles:

```
┌─────────────────────────────────────────┐
│          API Layer                       │
│  Controllers, Middleware, Filters        │
├─────────────────────────────────────────┤
│       Application Layer                  │
│  Commands, Queries, Handlers, DTOs       │
│  Validators, Behaviors, Interfaces       │
├─────────────────────────────────────────┤
│         Domain Layer                     │
│  Entities, Value Objects, Events         │
│  Domain Exceptions, Business Rules       │
├─────────────────────────────────────────┤
│      Infrastructure Layer                │
│  DbContext, Repositories, Migrations     │
│  External Services, Event Publishers     │
└─────────────────────────────────────────┘
```

## 🛠️ Technology Stack

### Backend
- **.NET 8**: Latest LTS version
- **C# 12**: Modern C# features
- **ASP.NET Core**: Web API framework
- **Entity Framework Core 9.0**: ORM and database migrations
- **MediatR 13.1**: CQRS implementation
- **FluentValidation 11.10**: Input validation
- **Serilog**: Structured logging

### Database
- **SQL Server**: Primary database for both services
  - `DigitalWalletDb` - Wallet Service
  - `DigitalWallet_Transaction` - Transaction Service

### Infrastructure
- **Docker & Docker Compose**: Container orchestration
- **RabbitMQ**: Message broker (planned event bus)
- **MongoDB**: NoSQL database (configured)
- **Redis**: Caching (configured)
- **Seq**: Log aggregation and visualization

### Development Tools
- **Swagger/OpenAPI**: API documentation

## 🚀 Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Docker Desktop](https://www.docker.com/products/docker-desktop)
- [SQL Server](https://www.microsoft.com/sql-server) (or use Docker)

### Installation

1. **Clone the repository**
```bash
git clone https://github.com/Hoseinhayati/Wallet-API.git
cd Wallet-API
```

2. **Start infrastructure services**
```bash
docker-compose up -d
```

This starts:
- MongoDB (27017)
- Redis (6379)
- RabbitMQ (5672, Management UI: 15672)
- Seq (5341)

3. **Update database connection strings**

Edit `appsettings.json` in each service:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost,1433;Database=YourDbName;User Id=sa;Password=YourPassword;..."
  }
}
```

4. **Build the solution**
```bash
dotnet build
```

5. **Apply database migrations**

For Wallet Service:
```bash
cd src/Services/Wallet/DigitalWallet.Wallet.Infrastructure
dotnet ef database update --startup-project ../DigitalWallet.Wallet.API
```

For Transaction Service:
```bash
cd src/Services/Transaction/DigitalWallet.Transaction.Infrastructure
dotnet ef database update --startup-project ../DigitalWallet.Transaction.API
```

6. **Run the services**

Open separate terminals for each service:

**Wallet Service:**
```bash
cd src/Services/Wallet/DigitalWallet.Wallet.API
dotnet run
```

**Transaction Service:**
```bash
cd src/Services/Transaction/DigitalWallet.Transaction.API
dotnet run
```

### Access Points

- **Wallet API**: https://localhost:7002 | http://localhost:5002
  - Swagger: https://localhost:7002/swagger

- **Transaction API**: https://localhost:7003 | http://localhost:5003
  - Swagger: https://localhost:7003/swagger

- **RabbitMQ Management**: http://localhost:15672 (guest/guest)
- **Seq Logs**: http://localhost:5341

## 📖 API Documentation

### Wallet Service Endpoints

```
POST   /api/wallets              - Create a new wallet
GET    /api/wallets/{id}         - Get wallet details
POST   /api/wallets/{id}/deposit - Deposit money
POST   /api/wallets/{id}/withdraw- Withdraw money
```

**Example: Create Wallet**
```json
POST /api/wallets
{
  "userId": "user123",
  "currency": "IRR",
  "initialBalance": 0
}
```

### Transaction Service Endpoints

```
POST   /api/transactions                    - Record a transaction
GET    /api/transactions/{id}               - Get transaction by ID
GET    /api/transactions/wallet/{walletId}  - Get wallet transactions
GET    /api/transactions/user/{userId}      - Get user transactions
GET    /api/transactions/wallet/{walletId}/stats - Get statistics
GET    /api/health                          - Health check
GET    /api/health/database                 - Database health
```

**Example: Record Deposit**
```json
POST /api/transactions
{
  "walletId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "userId": "user123",
  "type": "Deposit",
  "amount": 100000,
  "currency": "IRR",
  "description": "Initial deposit",
  "balanceAfterTransaction": 100000
}
```

**Example: Query Transactions**
```
GET /api/transactions/wallet/{walletId}?type=Deposit&take=10&skip=0
```

## 🧪 Testing

The project includes unit tests for domain logic and application layer:

```bash
# Run all tests
dotnet test

# Run specific test project
dotnet test tests/Services/Wallet/DigitalWallet.Wallet.Domain.UnitTests
```

**Test Coverage**:
- Domain entities validation
- Value objects behavior
- Command handlers
- Business rule enforcement

## 📁 Project Structure

```
DigitalWallet/
├── src/
│   └── Services/
│       ├── Wallet/
│       │   ├── DigitalWallet.Wallet.API/
│       │   ├── DigitalWallet.Wallet.Application/
│       │   ├── DigitalWallet.Wallet.Domain/
│       │   └── DigitalWallet.Wallet.Infrastructure/
│       └── Transaction/
│           ├── DigitalWallet.Transaction.API/
│           ├── DigitalWallet.Transaction.Application/
│           ├── DigitalWallet.Transaction.Domain/
│           └── DigitalWallet.Transaction.Infrastructure/
├── tests/
│   ├── UnitTests/
│   └── Services/
│       └── Wallet/
│           ├── DigitalWallet.Wallet.Application.UnitTests/
│           └── DigitalWallet.Wallet.Domain.UnitTests/
├── docker-compose.yml
└── DigitalWallet.sln
```

## 🎨 Design Patterns Used

- **CQRS**: Separation of commands and queries
- **Repository Pattern**: Data access abstraction
- **Unit of Work**: Transaction management
- **Domain Events**: Decoupled domain logic
- **Strategy Pattern**: Multiple implementations
- **Factory Pattern**: Object creation
- **Mediator Pattern**: Request handling pipeline
- **Specification Pattern**: Business rule encapsulation

## 🔒 Security Considerations

- Input validation on all endpoints
- Global exception handling
- SQL injection prevention via EF Core
- CORS configuration
- HTTPS redirection
- Sensitive data protection in logs

**Note**: For production use:
- Implement authentication/authorization
- Use Azure Key Vault or similar for secrets
- Enable rate limiting
- Add API gateway
- Implement distributed tracing

## 🚧 Roadmap

- [ ] Add authentication service (JWT/OAuth)
- [ ] Implement event bus with RabbitMQ
- [ ] Add API Gateway (Ocelot/YARP)
- [ ] Implement distributed caching with Redis
- [ ] Add integration tests
- [ ] Implement distributed tracing (OpenTelemetry)
- [ ] Add circuit breaker pattern (Polly)
- [ ] Kubernetes deployment manifests
- [ ] Add notification service
- [ ] Implement idempotency for transactions

## 📝 License

This project is licensed under the MIT License - see the LICENSE file for details.

## 👤 Author

**Hosein Hayati**

This project demonstrates modern .NET development practices and microservices architecture patterns suitable for enterprise applications.

## 🙏 Acknowledgments

- Clean Architecture by Robert C. Martin
- Domain-Driven Design by Eric Evans
- Microservices patterns by Chris Richardson
- .NET Microservices: Architecture for Containerized .NET Applications (Microsoft)

---

**Note**: This is a demonstration project showcasing architectural patterns and best practices. For production use, additional security, monitoring, and reliability features should be implemented.
