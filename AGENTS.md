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
- All endpoints except /auth/* and /health must require authentication (/health is public for Azure Container Apps health probes)
- Never log passwords, tokens, or sensitive user data
- Rate limit auth endpoints

## Naming
- Commands: `VerbNounCommand` (e.g. `CreateRoutineCommand`)
- Queries: `GetNounQuery` (e.g. `GetRoutineByIdQuery`)
- Handlers: `VerbNounCommandHandler` / `GetNounQueryHandler`
- Repositories: `INounRepository` / `NounRepository`
