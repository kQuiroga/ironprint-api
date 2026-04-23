# IronPrint API — Backend .NET 10

## Dev Workflow

- Ejecutar todo: `docker compose up -d` (levanta PostgreSQL + API)
- Rebuildar API: `docker compose up -d --build api`
- Docs interactivos: `http://localhost:8080/scalar/v1` (solo Development)
- Tests: `dotnet test` desde la raíz del repo

## Modelo de Dominio

```
User
  +-- Routine (name, weeksDuration, isActive)
        +-- RoutineDay (dayOfWeek, name?, muscleGroups[])
              +-- RoutineExercise (exerciseId, order, targetSets, targetReps)

Exercise (name, muscleGroup, notes?)  ← catálogo personal del usuario

WorkoutSession (date, routineDayId?)
  +-- ExerciseLog (exerciseId, order)
        +-- SetLog (setNumber, weight{value,unit}, reps, completed)

DayLog (date, status: Completed|NotCompleted)
```

## Endpoints

| Método | Ruta | Descripción |
|--------|------|-------------|
| GET | /health | Health check |
| POST | /auth/register | Registro, devuelve JWT |
| POST | /auth/login | Login, devuelve JWT |
| POST | /auth/refresh | Renovar access token |
| POST | /auth/revoke | Revocar refresh token |
| GET | /exercises | Catálogo del usuario |
| POST | /exercises | Crear ejercicio |
| GET | /exercises/{id} | Detalle |
| PUT | /exercises/{id} | Actualizar |
| DELETE | /exercises/{id} | Eliminar |
| GET | /routines | Rutinas del usuario |
| POST | /routines | Crear rutina |
| GET | /routines/{id} | Detalle con días y ejercicios |
| PUT | /routines/{id} | Actualizar |
| DELETE | /routines/{id} | Eliminar |
| GET | /routines/active | Rutina activa |
| POST | /routines/{id}/activate | Activar rutina |
| PUT | /routines/{id}/deactivate | Desactivar rutina |
| GET | /workout-sessions/calendar | Calendario en un rango de fechas |
| GET | /workout-sessions/{date} | Sesión por fecha |
| POST | /workout-sessions | Crear sesión |
| POST | /workout-sessions/{id}/sets | Registrar una serie |
| PUT | /day-logs/{date} | Crear o actualizar day log |
| DELETE | /day-logs/{date} | Eliminar day log |

## Arquitectura Hexagonal

```
Api → Application → Domain ← Infrastructure
```

- **Api** (`IronPrint.Api`): Minimal APIs, endpoints, middleware, DI config. Thin — delega todo a MediatR.
- **Application** (`IronPrint.Application`): Commands, Queries, Handlers (MediatR CQRS), DTOs, Behaviors. Depende solo de Domain.
- **Domain** (`IronPrint.Domain`): Entidades (private ctor + Create/Reconstitute), Value Objects, enums, interfaces de repositorio (ports). Sin dependencias externas.
- **Infrastructure** (`IronPrint.Infrastructure`): Repositorios (Dapper), Identity, JWT, DbUp migrations. Implementa interfaces de Domain.
- **Tests** (`IronPrint.Tests`): xUnit + FluentAssertions + NSubstitute

## Stack y Patrones

- **ORM**: Dapper + PostgreSQL (no EF Core)
- **CQRS**: MediatR — Commands y Queries separados
- **Auth**: ASP.NET Identity + JWT (access 15min, refresh 30d con rotation)
- **Logging**: Serilog (console + daily rolling file)
- **Validación**: FluentValidation (MediatR pipeline behavior)
- **Docs**: Scalar / OpenAPI (solo en Development)
- **Patrones**: Repository, Result Pattern, Value Objects, private constructors + factory methods

## Migraciones (DbUp)

- Se ejecutan automáticamente al arrancar la API
- Deben ser idempotentes: `IF NOT EXISTS`, `CREATE OR REPLACE`
- Scripts en `src/IronPrint.Infrastructure/Migrations/Scripts/`

## Secrets

- Local: `appsettings.Development.json` (gitignoreado — copiar de `.example`)
- Docker: `.env` (gitignoreado — copiar de `.env.example`)
- Producción: Azure Key Vault → env vars

## Convenciones

- Código y commits en inglés, conventional commit format
- Rutas específicas ANTES que rutas con parámetros (e.g. `/routines/active` antes de `/routines/{id:guid}`)

## Gotchas

- Para columnas `INTEGER[]` de PostgreSQL usar `IntArrayTypeHandler` (registrado en `DapperConfig.cs`)
- Rate limiting: `/auth/login` (5/min), `/auth/register` (10/min)

---

# Code Review Rules

## General
- No hardcoded secrets or connection strings — use appsettings.json / environment variables
- No commented-out code
- No unused imports or variables

## C# / .NET
- Use `sealed` on classes that are not meant to be inherited
- Prefer `record` for DTOs and flat data structures
- Use `Result<T>` / `Result` pattern — never throw exceptions for business logic
- Async methods must use `CancellationToken` where applicable
- Repository methods return domain entities, not raw DB rows

## Architecture (Hexagonal)
- Domain layer has zero infrastructure dependencies
- Application layer only references Domain — never Infrastructure
- Infrastructure implements interfaces defined in Application/Domain
- Endpoints are thin — delegate everything to MediatR commands/queries

## Database
- Use Dapper for all queries — no EF Core
- No N+1 queries — use JOIN + in-memory grouping
- Migrations managed by DbUp — SQL scripts only, never run DDL from code

## Security
- All endpoints except /auth/* and /health must require authentication
- Never log passwords, tokens, or sensitive user data
- Rate limit auth endpoints

## Naming
- Commands: `VerbNounCommand` (e.g. `CreateRoutineCommand`)
- Queries: `GetNounQuery` (e.g. `GetRoutineByIdQuery`)
- Handlers: `VerbNounCommandHandler` / `GetNounQueryHandler`
- Repositories: `INounRepository` / `NounRepository`
