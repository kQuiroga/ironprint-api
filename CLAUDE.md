@../CLAUDE.md
@AGENTS.md

# IronPrint API — Backend .NET 10

## Dev Workflow

- Ejecutar: `docker compose up -d --build api` (desde la raiz `ironprint/`)
- Docs interactivos: `http://localhost:8080/scalar/v1` (solo Development)
- PostgreSQL 17 en Docker (contenedor `ip-db`, puerto 5432)
- Tests: `dotnet test` desde la raiz del API

## Migraciones (DbUp)

- Se ejecutan automaticamente al arrancar la API
- Deben ser idempotentes: `IF NOT EXISTS`, `CREATE OR REPLACE`
- Scripts en `Infrastructure/Migrations/Scripts/`

## Arquitectura Hexagonal

```
Api → Application → Domain ← Infrastructure
```

- **Api** (`IronPrint.Api`): Minimal APIs, endpoints, middleware, DI config. Thin — delega todo a MediatR.
- **Application** (`IronPrint.Application`): Commands, Queries, Handlers (MediatR CQRS), DTOs, Behaviors. Depende solo de Domain.
- **Domain** (`IronPrint.Domain`): Entidades (private ctor + Create/Reconstitute), Value Objects (Weight), enums (MuscleGroup, DayLogStatus, WeightUnit), interfaces de repositorio (ports). Sin dependencias externas.
- **Infrastructure** (`IronPrint.Infrastructure`): Repositorios (Dapper), Identity, JWT, DbUp migrations. Implementa interfaces de Domain.
- **Tests** (`IronPrint.Tests`): xUnit + FluentAssertions

## Stack y Patrones

- **ORM**: Dapper + PostgreSQL (no EF Core)
- **CQRS**: MediatR — Commands y Queries separados
- **Auth**: ASP.NET Identity + JWT (access 15min, refresh 30d con rotation)
- **Logging**: Serilog
- **Patrones**: Repository, Result Pattern, Value Objects, private constructors + factory methods

## Gotchas / Notas Importantes

- Para columnas `INTEGER[]` de PostgreSQL, usar `IntArrayTypeHandler` (registrado en `DapperConfig.cs`)
- Endpoints: registrar rutas especificas ANTES de rutas con parametros (e.g. `/routines/active` antes de `/routines/{id:guid}`)
- Rate limiting: `/auth/login` (5/min), `/auth/register` (10/min)
- Secrets: `appsettings.Development.json` (local) o `.env` (Docker), Azure Key Vault (prod)
