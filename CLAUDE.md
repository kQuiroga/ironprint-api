# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Proposito de este repositorio

Este es el repositorio de **documentacion de arquitectura** de IronPrint, un workout tracker. No contiene codigo fuente. El codigo vive en repos separados:

- `ironprint-api` — Backend .NET 10
- `ironprint-web` — Frontend Next.js 15

Ver `ARCHITECTURE.md` para decisiones de arquitectura detalladas.

## Stack tecnico

**Backend** (.NET 10): Arquitectura Hexagonal con capas Api/Application/Domain/Infrastructure. Usa Dapper + PostgreSQL, MediatR para CQRS, ASP.NET Identity + JWT. Patrones: Repository, Result Pattern, Value Objects.

**Frontend** (Next.js 15): App Router, TypeScript, Tailwind CSS, TanStack Query, React Hook Form + Zod.

## Infraestructura local

- PostgreSQL 17 en Docker (contenedor `ip-db`, puerto 5432)
- API en Docker (contenedor `ip-api`, puerto 8080)
- Documentacion interactiva en `http://localhost:8080/scalar/v1` (solo en modo Development)
- Migraciones gestionadas con DbUp — se ejecutan automaticamente al arrancar la API

## Modelo de dominio

```
User -> Routine -> RoutineDay -> RoutineExercise
User -> Exercise (catalogo personal)
WorkoutSession -> ExerciseLog -> SetLog
```

## Endpoints disponibles

| Metodo | Ruta | Descripcion |
|--------|------|-------------|
| POST | /auth/register | Registro de nuevo usuario, devuelve JWT |
| POST | /auth/login | Login, devuelve JWT |
| GET | /exercises | Catalogo de ejercicios del usuario |
| POST | /exercises | Crear ejercicio |
| GET | /exercises/{id} | Detalle de ejercicio |
| PUT | /exercises/{id} | Actualizar ejercicio |
| DELETE | /exercises/{id} | Eliminar ejercicio |
| GET | /routines | Rutinas del usuario |
| POST | /routines | Crear rutina |
| GET | /routines/{id} | Detalle de rutina con dias y ejercicios |
| PUT | /routines/{id} | Actualizar rutina |
| DELETE | /routines/{id} | Eliminar rutina |
| GET | /workout-sessions/calendar | Fechas con sesion en un rango |
| GET | /workout-sessions/{date} | Detalle de sesion por fecha |
| POST | /workout-sessions | Crear sesion de entrenamiento |
| POST | /workout-sessions/{id}/sets | Registrar una serie (set) |

## Convenciones de idioma

- Responder siempre en espanol.
- Codigo, commits y variables en ingles.
- Comentarios y documentacion en espanol.

## Gestion de secrets

- Desarrollo local (`dotnet run`): `appsettings.Development.json`
- Desarrollo con Docker: archivo `.env` (en `.gitignore`, nunca se commitea)
- Produccion (Azure Container Apps): variables de entorno inyectadas desde Azure Key Vault
