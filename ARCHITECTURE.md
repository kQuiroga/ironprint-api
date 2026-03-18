# IronPrint - Architecture Decision Records

## 1. Descripcion del proyecto

Aplicacion web de seguimiento de entrenamientos. Permite a los usuarios:

- Visualizar un calendario con los dias entrenados y no entrenados segun su plan.
- Hacer click en un dia para ver el detalle: ejercicios realizados, peso, repeticiones y series.
- Crear rutinas semanales definiendo ejercicios por dia de la semana.
- Repetir rutinas durante las semanas que el usuario desee.
- Crear ejercicios libremente (catalogo personal por usuario).
- Ver estadisticas y progresion personal (peso, volumen, etc.) a lo largo del tiempo.

## 2. Decisiones de arquitectura

### 2.1 Repositorios

| Repositorio | Contenido |
|---|---|
| `ironprint-api` | Backend .NET 10 (ASP.NET Core + Dapper + PostgreSQL) |
| `ironprint-web` | Frontend Next.js 15 |

Dos repositorios separados para mantener ciclos de despliegue, CI/CD y tooling independientes. Una futura app movil sera un tercer repositorio consumiendo la misma API.

### 2.2 Backend

- **Runtime**: .NET 10
- **Framework**: ASP.NET Core Minimal APIs
- **Arquitectura**: Hexagonal (Ports & Adapters) pragmatica
- **ORM**: Dapper (control total sobre SQL, mejor rendimiento en lecturas)
- **Migraciones de esquema**: DbUp (scripts SQL versionados, se ejecutan al arrancar la API)
- **Patron CQRS ligero**: MediatR para separar Commands (escritura) y Queries (lectura)
- **Autenticacion**: ASP.NET Identity + JWT (access token 15 min) + Refresh tokens (30 dias, con token rotation)
- **Logging**: Serilog — consola en dev, consola + archivo rotativo en prod
- **Patrones principales**:
  - Repository (abstraccion de acceso a datos)
  - CQRS ligero (Commands/Queries con MediatR)
  - Value Objects (Weight)
  - Result Pattern (evitar excepciones para flujo de control)

#### Estructura de proyectos backend

```
ironprint-api/
  src/
    IronPrint.Api/              -> Puerto de entrada (Minimal APIs, endpoints)
    IronPrint.Application/      -> Casos de uso (Commands, Queries, Handlers, Behaviors)
    IronPrint.Domain/           -> Entidades, Value Objects, interfaces de puertos
    IronPrint.Infrastructure/   -> Adaptadores de salida (Dapper, JWT, Identity, migraciones)
  tests/
    IronPrint.Tests/            -> Tests unitarios (xUnit + FluentAssertions)
```

### 2.3 Frontend

- **Framework**: Next.js 15 (App Router)
- **Lenguaje**: TypeScript
- **Estilos**: Tailwind CSS
- **Estado del servidor**: TanStack Query (cache, revalidacion, estados de carga)
- **Graficas**: Recharts (estadisticas y progresion)
- **Fechas**: date-fns
- **Formularios**: React Hook Form + Zod (validacion)

#### Estructura del frontend

```
ironprint-web/
  src/
    app/                    -> App Router (rutas y layouts)
      (auth)/               -> Rutas protegidas
        calendar/           -> Vista calendario
        routines/           -> CRUD rutinas
        workout/[id]/       -> Detalle de entrenamiento
        stats/              -> Estadisticas y progresion
      login/                -> Autenticacion
    components/
      ui/                   -> Componentes base (botones, inputs, modales)
      calendar/             -> Componentes del calendario
      routines/             -> Componentes de rutinas
      stats/                -> Graficas de progresion
    hooks/                  -> Custom hooks (useAuth, useRoutines, etc.)
    services/               -> Clientes HTTP para la API .NET
    types/                  -> Tipos TypeScript
```

### 2.4 Base de datos

- **Motor**: PostgreSQL 17
- **Servicio**: Azure Database for PostgreSQL (Flexible Server) en produccion
- **Desarrollo local**: PostgreSQL en contenedor Docker (via Docker Compose)

### 2.5 Despliegue

- **Backend**: Azure Container Apps (Docker, escala a cero)
- **Frontend**: Vercel (a confirmar) o Azure Container Apps
- **Base de datos**: Azure Database for PostgreSQL Flexible Server
- **CI/CD**: GitHub Actions (un pipeline por repositorio)

### 2.6 Autenticacion

- ASP.NET Identity para gestion de usuarios
- JWT access token (15 min) para autenticacion stateless
- Refresh token (30 dias) almacenado en DB con hash SHA-256 — con token rotation
- Endpoints: `/auth/register`, `/auth/login`, `/auth/refresh`, `/auth/revoke`

## 3. Modelo de dominio

```
User
  +-- Routine (name, weeks_duration)
        +-- RoutineDay (day_of_week)
              +-- RoutineExercise (exercise, order, target_sets, target_reps)

Exercise (name, muscle_group, notes)  <- catalogo personal del usuario

WorkoutSession (date, routine_day_ref)
  +-- ExerciseLog (exercise, order)
        +-- SetLog (set_number, weight, reps, completed)
```

### Explicacion de las entidades

- **User**: Usuario de la aplicacion.
- **Routine**: Rutina de entrenamiento semanal. Tiene un nombre y una duracion en semanas.
- **RoutineDay**: Dia concreto de la semana dentro de una rutina (e.g. Lunes, Miercoles).
- **RoutineExercise**: Ejercicio planificado dentro de un dia de rutina, con series y repeticiones objetivo.
- **Exercise**: Ejercicio creado por el usuario (catalogo personal). Nombre, grupo muscular, notas.
- **WorkoutSession**: Sesion de entrenamiento real en una fecha concreta.
- **ExerciseLog**: Registro de un ejercicio realizado dentro de una sesion.
- **SetLog**: Registro de una serie individual (numero de serie, peso, repeticiones, completada o no).

## 4. Consideraciones futuras

- App movil (tercer repositorio consumiendo la misma API REST)
- Posible uso de gRPC para la app movil si se necesita mejor rendimiento
- Notificaciones push (recordatorios de entrenamiento)
- Exportacion de datos (CSV, PDF)
