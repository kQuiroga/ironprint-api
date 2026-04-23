# IronPrint API

Backend for IronPrint — a personal workout tracking app. Built with .NET 10 Minimal APIs and a Hexagonal Architecture.

> **Monorepo sibling:** [`ironprint-web`](https://github.com/kQuiroga/ironprint-web) (Next.js 16 frontend)

## Tech Stack

- **Runtime:** .NET 10 (ASP.NET Core Minimal APIs)
- **Architecture:** Hexagonal (Ports & Adapters) + CQRS via MediatR
- **Database:** PostgreSQL 17 with Dapper (no ORM)
- **Migrations:** DbUp (idempotent SQL scripts, auto-run on startup)
- **Auth:** JWT Bearer + refresh tokens (stored in DB)
- **Validation:** FluentValidation (MediatR pipeline behavior)
- **Docs:** Scalar / OpenAPI (available in Development only)
- **Logging:** Serilog (console + daily rolling file)
- **Testing:** xUnit, FluentAssertions, NSubstitute

## Features

- JWT authentication with refresh token rotation
- Rate limiting on auth endpoints (5 req/min login, 10 req/min register)
- Exercise catalog — full CRUD per user
- Routine management — days, exercises, activation
- Workout session logging — per-exercise set tracking (reps + weight)
- Day log — mark days as completed or not completed
- Calendar endpoint — returns planned days, sessions, and day logs in one query
- Health check endpoint for Azure Container Apps

## Getting Started

### Prerequisites

- Docker (recommended) — runs PostgreSQL + API together
- Or: .NET 10 SDK + a running PostgreSQL 17 instance

### Running with Docker (recommended)

```bash
# From the monorepo root (ironprint/)
docker compose up -d
```

API is available at `http://localhost:8080`.  
Interactive docs: `http://localhost:8080/scalar/v1`

To rebuild after code changes:

```bash
docker compose up -d --build api
```

### Running locally (without Docker)

```bash
cd src/IronPrint.Api
dotnet run
```

Requires `appsettings.Development.json` (see [Environment Variables](#environment-variables) below).

### Running tests

```bash
dotnet test
```

## Environment Variables

### Docker (`.env` at monorepo root)

```env
POSTGRES_DB=ironprint
POSTGRES_USER=ironprint
POSTGRES_PASSWORD=ironprint_dev
JWT_SECRET_KEY=your-secret-key-min-32-chars
```

### Local development (`appsettings.Development.json`)

```json
{
  "ConnectionStrings": {
    "Database": "Host=localhost;Port=5432;Database=ironprint;Username=ironprint;Password=ironprint_dev"
  },
  "Jwt": {
    "SecretKey": "your-secret-key-min-32-chars"
  },
  "Cors": {
    "AllowedOrigins": ["http://localhost:3000"]
  }
}
```

## Project Structure

```
src/
├── IronPrint.Api            # Minimal API endpoints, DI wiring, middleware
├── IronPrint.Application    # CQRS commands, queries, handlers, validators, DTOs
├── IronPrint.Domain         # Entities, value objects, repository interfaces (no external deps)
└── IronPrint.Infrastructure # Dapper repositories, JWT service, migrations

tests/
└── IronPrint.Tests          # Unit tests (xUnit + FluentAssertions + NSubstitute)
```

### Layer dependencies

```
Api → Application → Domain ← Infrastructure
```

Domain has no external dependencies. Infrastructure implements the repository interfaces (Ports) defined in Domain.

## API Endpoints

| Method | Path | Description |
|--------|------|-------------|
| GET | `/health` | Health check |
| POST | `/auth/register` | Register a new user |
| POST | `/auth/login` | Login — returns access + refresh token |
| POST | `/auth/refresh` | Refresh access token |
| POST | `/auth/revoke` | Revoke refresh token |
| GET | `/exercises` | List user's exercises |
| POST | `/exercises` | Create exercise |
| GET | `/exercises/{id}` | Get exercise by ID |
| PUT | `/exercises/{id}` | Update exercise |
| DELETE | `/exercises/{id}` | Delete exercise |
| GET | `/routines` | List user's routines |
| POST | `/routines` | Create routine |
| GET | `/routines/{id}` | Get routine with days and exercises |
| PUT | `/routines/{id}` | Update routine |
| DELETE | `/routines/{id}` | Delete routine |
| GET | `/routines/active` | Get the active routine |
| POST | `/routines/{id}/activate` | Activate a routine |
| PUT | `/routines/{id}/deactivate` | Deactivate a routine |
| GET | `/workout-sessions/calendar` | Calendar data for a date range |
| GET | `/workout-sessions/{date}` | Get session by date |
| POST | `/workout-sessions` | Create workout session |
| POST | `/workout-sessions/{id}/sets` | Log a set |
| PUT | `/day-logs/{date}` | Create or update a day log |
| DELETE | `/day-logs/{date}` | Delete a day log |

## Deployment

- **Platform:** Azure Container Apps
- **Database:** Azure Database for PostgreSQL Flexible Server
- **Secrets:** Azure Key Vault → environment variables
- **CI/CD:** GitHub Actions
