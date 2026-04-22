# Skill Registry — ironprint-api

Generated: 2026-03-31

## Convention Files

| File | Purpose |
|------|---------|
| `AGENTS.md` | Code review rules: C#/.NET standards, hexagonal architecture, security, naming conventions |
| `CLAUDE.md` | Project conventions, dev workflow, architecture overview, gotchas |

## User Skills

| Skill | Trigger |
|-------|---------|
| `go-testing` | Writing Go tests, using teatest, adding test coverage |
| `skill-creator` | Creating new skills, adding agent instructions, documenting patterns for AI |
| `branch-pr` | Creating a pull request, opening a PR, preparing changes for review |
| `issue-creation` | Creating a GitHub issue, reporting a bug, requesting a feature |
| `judgment-day` | "judgment day", "review adversarial", "dual review", "doble review", "juzgar" |

## Compact Rules

### From AGENTS.md

```
- No hardcoded secrets — use appsettings.json / env vars
- No commented-out code, no unused imports
- Use `sealed` on non-inheritable classes
- Prefer `record` for DTOs
- Use Result<T>/Result pattern — never throw for business logic
- Async methods must accept CancellationToken
- Repository methods return domain entities, not raw DB rows
- Domain: zero infrastructure dependencies
- Application: only references Domain — never Infrastructure
- Infrastructure: implements interfaces from Application/Domain
- Endpoints: thin — delegate everything to MediatR
- Dapper only — no EF Core
- No N+1 queries — JOIN + in-memory grouping
- Migrations via DbUp SQL scripts only
- All endpoints except /auth/* require authentication
- Never log passwords, tokens, or sensitive data
- Rate limit auth endpoints
- Commands: VerbNounCommand | Queries: GetNounQuery
- Handlers: VerbNounCommandHandler / GetNounQueryHandler
- Repositories: INounRepository / NounRepository
```

### From CLAUDE.md (gotchas)

```
- INTEGER[] columns → use IntArrayTypeHandler (registered in DapperConfig.cs)
- Register specific routes BEFORE parameterized routes (e.g. /routines/active before /routines/{id:guid})
- Migrations must be idempotent: IF NOT EXISTS, CREATE OR REPLACE
- Strict TDD Mode: enabled
```
