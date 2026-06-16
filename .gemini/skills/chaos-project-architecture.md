# 🏗️ Chaos Casino — Arquitectura Completa del Proyecto

> **⚠️ REGLA DE ACTUALIZACIÓN OBLIGATORIA**: Cada vez que se realice un cambio importante en la arquitectura del proyecto (nuevo controlador, nuevo modelo de base de datos, nueva página de frontend, nuevo servicio, cambio de estructura de carpetas, nuevo juego, nueva ruta, etc.), **DEBES actualizar este archivo Skill** para reflejar el cambio. El objetivo es que este archivo sea siempre la referencia más actualizada de la arquitectura del proyecto.

---

## 📋 Descripción General

**Chaos Casino** es una plataforma de casino online con juegos interactivos, sistema de animales coleccionables, rankings, lobbies multijugador, tienda, y un backoffice de administración. El proyecto sigue una arquitectura **fullstack monorepo** con:

- **Backend API** → C# / .NET 9 / ASP.NET Core Web API
- **Frontend Principal** → React 19 + TypeScript + Vite 8
- **Backoffice / Admin Panel** → Blazor WebAssembly (.NET 9)
- **Base de Datos** → PostgreSQL (Supabase) via Entity Framework Core + Npgsql
- **Definición de esquema SQL** → SQL Server Database Project (`CasinoDB`)

---

## 📁 Estructura del Monorepo

```
Chaos-Api-practices_2026/
│
├── Chaos.sln                          # Solución principal de Visual Studio
├── dockerfile                         # Docker: compilar + ejecutar la API
│
├── Chaos/                             # 🔵 Backend — ASP.NET Core Web API
│   ├── Chaos.Api.csproj
│   ├── Program.cs                     # Punto de entrada, DI, middleware, pipeline
│   ├── appsettings.json               # Configuración (DB, JWT)
│   ├── Controllers/                   # Controladores de API REST
│   │   ├── Config/                    # Controladores de configuración/backoffice
│   │   └── *.cs
│   ├── Service/                       # Lógica de negocio
│   │   ├── Config/                    # Servicios de configuración CRUD (backoffice)
│   │   └── *.cs
│   ├── Interface/                     # Interfaces de servicios (DI)
│   │   ├── Config/                    # Interfaces para Config services
│   │   └── I*.cs
│   ├── Middleware/                     # TokenBlackListMiddleware (revocación JWT + IsActive)
│   ├── Models/                        # Modelos de dominio API (no EF) — legacy + in-memory
│   ├── RequestEntity/                 # DTOs de entrada (requests)
│   │   └── Config/                    # DTOs de entrada para backoffice
│   ├── ResponseEntity/                # DTOs de salida (responses)
│   │   ├── Config/                    # DTOs de salida para backoffice
│   │   └── RussianRoulette/           # Responses específicos de Ruleta Rusa
│   ├── Enums/                         # Enums del dominio (juegos, cartas, animales, etc.)
│   ├── Provider/                      # CasinoProvider (acceso a contexto de negocio)
│   └── Utils/                         # Utilidades (password hash, lobby codes, draw money)
│
├── Chaos.Infraestructure/             # 🟢 Capa de Infraestructura — EF Core
│   ├── Chaos.Infraestructure.csproj
│   ├── Models/                        # Entidades EF Core (mapean tablas de BD)
│   │   ├── CasinoDBContext.cs         # DbContext principal (auto-generado por EF Power Tools)
│   │   ├── CasinoDBContext.Custom.cs  # Extensiones custom del DbContext
│   │   ├── User.cs, Animal.cs, Game.cs, GameSession.cs
│   │   ├── RussianRouletteLobby.cs, RussianRoulettePlayer.cs, etc.
│   │   ├── BlackjackSession.cs, CoinFlipSession.cs, SlotSession.cs, etc.
│   │   └── *Config.cs (configuraciones de juegos)
│   ├── Migrations/                    # Migraciones de EF Core
│   ├── Data/                          # CasinoDbInitializer (seed / inicialización)
│   ├── Extensions/                    # ServiceCollectionExtensions, ApplicationBuilderExtensions
│   └── efpt.config.json               # Configuración de EF Power Tools (reverse engineering)
│
├── Chaos.Shared/                      # 🟡 Proyecto Compartido
│   ├── Chaos.Shared.csproj
│   ├── Dto/                           # DTOs compartidos
│   ├── RequestEntity/                 # Entities de request compartidas
│   └── ResponseEntity/               # Entities de response compartidas
│
├── ChaosFrontend/                     # 🔴 Frontend Principal — React + Vite
│   ├── package.json                   # React 19, react-router-dom 7, Vite 8
│   ├── vite.config.ts
│   ├── .env                           # VITE_BASE_URL (URL de la API)
│   ├── vercel.json                    # Config deploy Vercel
│   └── src/
│       ├── main.tsx                   # Punto de entrada React
│       ├── App.tsx                    # Layout principal (Outlet, navegación)
│       ├── App.css                    # Estilos globales del App
│       ├── index.css                  # Estilos base
│       ├── router.tsx                 # Definición de rutas (createBrowserRouter)
│       ├── Pages/                     # Páginas principales
│       │   ├── IntroPage.tsx          # Página de inicio
│       │   ├── LoginPage.tsx          # Login
│       │   ├── RegisterPage.tsx       # Registro
│       │   ├── LobbyPage.tsx          # Lobby principal (selector de juegos)
│       │   ├── BarLanding.tsx         # Barra/Bar interactivo
│       │   ├── AnimalShop.tsx         # Tienda de animales
│       │   ├── YagoMachine.tsx        # Tragaperras
│       │   ├── MusicRoom.tsx          # Sala de música
│       │   ├── RankingPage.tsx        # Rankings/clasificaciones
│       │   ├── games/                 # Páginas de juegos
│       │   │   ├── BlackJack.tsx
│       │   │   ├── CoinFlip.tsx
│       │   │   ├── EuropeanRoulette.tsx
│       │   │   ├── HigherOrLower.tsx
│       │   │   └── RussianRoulette.tsx
│       │   ├── Biomas/                # Páginas de biomas (hábitats de animales)
│       │   │   ├── FarmPage.tsx
│       │   │   ├── OceanPage.tsx
│       │   │   ├── DesertPage.tsx
│       │   │   ├── JurassicPage.tsx
│       │   │   ├── UrbanPage.tsx
│       │   │   ├── SwampPage.tsx
│       │   │   ├── SavannaPage.tsx
│       │   │   ├── JunglePage.tsx
│       │   │   └── LegendaryPage.tsx
│       │   ├── Components/            # Componentes reutilizables de páginas
│       │   │   ├── LobbyRouletteRussian.tsx  # Lobby de la Ruleta Rusa
│       │   │   ├── CreateOrJoinRussianRoulette.tsx
│       │   │   ├── JoinToLobby.tsx
│       │   │   ├── BackButton.tsx
│       │   │   ├── ActiveDrinkEffect.tsx
│       │   │   ├── BiomeSelector.tsx
│       │   │   ├── BetsTable.tsx
│       │   │   ├── LoadingHamster.tsx
│       │   │   ├── MachineConfig.tsx
│       │   │   ├── MusicButton.tsx
│       │   │   ├── ErrorPopup.tsx
│       │   │   ├── PodiumColumn.tsx
│       │   │   ├── EditorButton.tsx
│       │   │   ├── AnimalToolTip.tsx
│       │   │   └── useSlotMachine.ts  # Hook de lógica de la tragaperras
│       │   └── error/                 # Páginas de error (NotFound, etc.)
│       ├── components/                # Componentes compartidos
│       │   ├── Auth/                  # ProtectedRoute (guard de rutas)
│       │   ├── Lobby/                 # Componentes de lobby
│       │   ├── UI/                    # Componentes UI genéricos
│       │   ├── AnimalRoulette/        # Ruleta de animales
│       │   ├── AnimalToBetInGame.tsx   # Componente para apostar animales
│       │   └── AnimalsUser.tsx        # Muestra los animales del usuario
│       ├── hooks/                     # Custom hooks
│       │   ├── useAudio.ts            # Reproductor de audio
│       │   ├── useAuth.ts             # Hook de autenticación
│       │   └── useDraggable.tsx       # Hook para drag & drop
│       ├── services/                  # Servicios frontend
│       │   ├── animalImageService.ts  # Obtener imágenes de animales
│       │   └── rankingService.ts      # Servicio de rankings
│       ├── AnimalMovement/            # Sistema de movimiento de animales en biomas
│       │   ├── AnimalEditor.tsx       # Editor de configuración de animales (dev)
│       │   ├── animalConfigs.json     # Configuración de movimiento por animal
│       │   ├── AnimalCollisions.ts    # Colisiones genéricas
│       │   └── *Collisions.ts         # Colisiones por bioma (Farm, Desert, etc.)
│       ├── styles/                    # Archivos CSS por componente/página
│       └── assets/                    # Imágenes, SVGs, fuentes, audio
│
├── BlazorCasinoApp/                   # 🟣 Backoffice — Blazor WebAssembly
│   └── BlazorCasinoApp/
│       ├── Chaos.BlazorCasinoApp.csproj
│       ├── Program.cs                 # Punto de entrada Blazor WASM
│       ├── App.razor                  # Componente raíz
│       ├── Pages/                     # Páginas Blazor (RussianRoulette/, etc.)
│       ├── Service/                   # Servicios HTTP del backoffice
│       ├── IApiService/               # Interfaces de servicios
│       ├── Auth/                      # Autenticación del backoffice
│       ├── Layout/                    # NavMenu, MainLayout
│       └── wwwroot/                   # Assets estáticos del backoffice
│
├── CasinoDB/                          # 🔷 SQL Server Database Project
│   ├── CasinoDB.sqlproj
│   └── dbo/Tables/                    # Definiciones SQL de tablas
│
└── docs/                              # Documentación (docfx)
```

---

## 🔵 Backend — API REST (.NET 9)

### Stack Tecnológico
- **Framework**: ASP.NET Core 9 Web API
- **ORM**: Entity Framework Core + Npgsql (PostgreSQL)
- **Autenticación**: JWT Bearer (`Microsoft.IdentityModel.Tokens`)
- **Documentación API**: Swagger + Scalar API Reference
- **Contenedor**: Docker (multi-stage build con `dotnet/sdk:9.0` + `dotnet/aspnet:9.0`)

### Cómo ejecutar
```bash
cd Chaos
dotnet run
```
- La API redirige `/` a `/swagger`
- Puerto configurado en `Properties/launchSettings.json`

### Compilar solo la API (sin CasinoDB.sqlproj)
```bash
dotnet build Chaos\Chaos.Api.csproj
```
> ⚠️ `dotnet build` a nivel de solución falla por el `CasinoDB.sqlproj` que requiere Visual Studio.

### Patrón arquitectónico
Sigue el patrón **Controller → Service → DbContext (EF Core)**:
1. **Controller** recibe la petición HTTP y delega al servicio
2. **Service** contiene la lógica de negocio, accede al `CasinoDBContext`
3. **Interface** define el contrato (inyección de dependencias)
4. **RequestEntity / ResponseEntity** son los DTOs de entrada/salida

### Controladores principales
| Controlador | Ruta Base | Función |
|---|---|---|
| `AuthController` | `/api/Auth` | Login, registro, logout |
| `UserController` | `/api/User` | CRUD usuarios, wallet |
| `CasinoGamesController` | `/api/CasinoGames` | Blackjack, CoinFlip, Slots, Roulette, HigherLower |
| `LobbyController` | `/api/Lobby` | CRUD lobbies, join/leave/kick |
| `RussianRouletteControllerr` | `/api/RussianRouletteControllerr` | Start game, play round, status, history |
| `AnimalController` | `/api/Animal` | CRUD animales, comprar, vender |
| `BarController` | `/api/Bar` | Efectos de bebidas |
| `RankingController` | `/api/Ranking` | Rankings de usuarios y animales |
| `HealthController` | `/api/Health` | Health check |

### Controladores de Config (Backoffice)
| Controlador | Función |
|---|---|
| `AuthBackOfficeController` | Autenticación del admin |
| `BlackjackConfigController` | Config de Blackjack |
| `CoinGameController` | Config de CoinFlip |
| `HigherLowerGameController` | Config de Higher/Lower |
| `RouletteGameConfigController` | Config de Ruleta Europea |
| `RussianRouletteController` | Config de Ruleta Rusa |
| `SlotsConfigController` | Config de Slots |
| `SlotSymbolController` | Símbolos de Slots |
| `SlotPayoutRuleController` | Reglas de pago Slots |
| `AnimalValueConfigController` | Config de valores de animales |
| `DevConfigController` | Config de desarrollo |

### Middleware
- `TokenBlackListMiddleware`: Intercepta cada request autenticada para verificar:
  1. Token no esté revocado (blacklist en memoria)
  2. Usuario tenga `IsActive = true` en BD

### Inyección de Dependencias
Todos los servicios se registran como `Scoped` en `Program.cs`, excepto:
- `Random` → `Singleton`
- `DeckService` → `Singleton`
- `TokenBlackListService` → `Singleton` (blacklist en memoria)

---

## 🟢 Capa de Infraestructura (EF Core)

### Entidades principales de Base de Datos
| Entidad | Tabla | Descripción |
|---|---|---|
| `User` | Users | Usuarios (name, email, password, wallet, isActive, isAdmin) |
| `Animal` | Animals | Animales coleccionables (type, name, biome, stats, ownerId) |
| `AnimalShop` | AnimalShops | Tienda de animales |
| `AnimalValueConfig` | AnimalValueConfigs | Configuración de valores de animales |
| `Game` | Games | Catálogo de juegos disponibles |
| `GameSession` | GameSessions | Sesiones de juego genéricas |
| `Casino` | Casinos | Entidad de casino |
| **Blackjack** | | |
| `BlackjackSession` | BlackjackSessions | Sesiones de blackjack |
| `BlackjackHand` | BlackjackHands | Manos de blackjack |
| `BlackjackCard` | BlackjackCards | Cartas en una mano |
| `BlackjackAction` | BlackjackActions | Acciones del jugador |
| `BlackjackGameConfig` | BlackjackGameConfigs | Config de blackjack |
| **CoinFlip** | | |
| `CoinFlipSession` | CoinFlipSessions | Sesiones de CoinFlip |
| `CoinFlipGameConfig` | CoinFlipGameConfigs | Config de CoinFlip |
| **HigherLower** | | |
| `HigherLowerSession` | HigherLowerSessions | Sesiones de Higher/Lower |
| `HigherLowerRound` | HigherLowerRounds | Rondas |
| `HigherLowerGameConfig` | HigherLowerGameConfigs | Config |
| **Ruleta Europea** | | |
| `RouletteSession` | RouletteSessions | Sesiones de ruleta |
| `RouletteGameConfig` | RouletteGameConfigs | Config |
| `RouletteBetType` | RouletteBetTypes | Tipos de apuesta |
| **Ruleta Rusa** | | |
| `RussianRouletteLobby` | RussianRouletteLobbies | Lobbies (status: Waiting/InProgress/Finished) |
| `RussianRoulettePlayer` | RussianRoulettePlayers | Jugadores en un lobby |
| `RussianRouletteRound` | RussianRouletteRounds | Rondas jugadas |
| `RussianRouletteGameConfig` | RussianRouletteGameConfigs | Config |
| **Slots** | | |
| `SlotSession` | SlotSessions | Sesiones de slots |
| `SlotGameConfig` | SlotGameConfigs | Config |
| `SlotSymbol` | SlotSymbols | Símbolos |
| `SlotPayoutRule` | SlotPayoutRules | Reglas de pago |
| **Tienda** | | |
| `ShopAnimalListing` | ShopAnimalListings | Listados de la tienda |
| `ShopTransaction` | ShopTransactions | Transacciones |
| **Bar** | | |
| `ActiveDrinkEffect` | ActiveDrinkEffects | Efectos activos de bebida |

### Migraciones
```bash
# Desde la raíz del proyecto:
cd Chaos.Infraestructure
dotnet ef migrations add NombreMigracion --startup-project ../Chaos/Chaos.Api.csproj
dotnet ef database update --startup-project ../Chaos/Chaos.Api.csproj
```

### Base de Datos
- **Proveedor**: PostgreSQL vía Supabase
- **Conexión**: Definida en `Chaos/appsettings.json` → `ConnectionStrings:DefaultConnection`
- **Driver**: Npgsql

---

## 🔴 Frontend Principal — React + Vite

### Stack Tecnológico
- **React 19** con TypeScript 6
- **Vite 8** (bundler + dev server)
- **react-router-dom 7** (enrutamiento con `createBrowserRouter`)
- **CSS Vanilla** (un archivo `.css` por componente/página)
- **No usa** Tailwind, Redux, ni ningún state manager externo

### Cómo ejecutar
```bash
cd ChaosFrontend
npm install
npm run dev
```

### Variables de entorno
Archivo `.env` en `ChaosFrontend/`:
```
VITE_BASE_URL=https://localhost:5174   # URL base de la API
```

### Comunicación con la API
- Se usa `fetch()` nativo de JavaScript (NO axios)
- El token JWT se guarda en `localStorage` con la clave `token_casino`
- Se envía en cada petición como header: `authorization: Bearer ${token}`
- Se decodifica manualmente con `atob(token.split('.')[1])` para obtener claims

### Rutas principales
| Ruta | Componente | Auth |
|---|---|---|
| `/` | `IntroPage` | No |
| `/login` | `LoginPage` | No |
| `/register` | `RegisterPage` | No |
| `/lobby` | `LobbyPage` | ✅ `ProtectedRoute` |
| `/bar` | `BarLanding` | ✅ |
| `/blackjack` | `BlackJack` | ✅ |
| `/coinflip` | `CoinFlip` | ✅ |
| `/european-roulette` | `EuropeanRoulette` | ✅ |
| `/higherorlower` | `HigherOrLower` | ✅ |
| `/yagomachine` | `YagoMachine` | ✅ |
| `/shop` | `AnimalShop` | ✅ |
| `/ranking` | `RankingPage` | ✅ |
| `/music-room` | `MusicRoom` | No |
| `/create-or-join-russian-roulette` | `CreateOrJoinRussianRoulette` | No |
| `/lobby-russian-roulette` | `LobbyRouletteRussian` | No |
| `/join-russian-roulette` | `JoinToLobby` | No |
| `/russian-roulette/game/:lobbyId` | `RussianRoulette` | No |
| `/farm`, `/ocean`, `/desert`, etc. | Biome Pages | ✅ |
| `/dev/animal-editor` | `AnimalEditor` | ✅ |
| `*` | `NotFound` | No |

### Patrones del Frontend
1. **Estilos**: Un archivo CSS en `src/styles/` por cada componente/página (ej: `BlackJack.css`)
2. **Componentes**: En `src/Pages/Components/` para componentes de página, `src/components/` para componentes compartidos
3. **Hooks**: Custom hooks en `src/hooks/` (`useAuth`, `useAudio`, `useDraggable`)
4. **Servicios**: En `src/services/` para lógica de datos
5. **Estado**: `useState` / `useRef` local (no hay state manager global)
6. **Polling**: Se usa `setInterval` en `useEffect` para datos en tiempo real (ej: lobby)
7. **Cleanup**: `useEffect` cleanup + `beforeunload`/`pagehide` para limpiar recursos al salir

---

## 🟣 Backoffice — Blazor WebAssembly

### Stack
- Blazor WebAssembly (.NET 9)
- Consume la misma API que el frontend React
- Permite administrar configuraciones de juegos vía CRUD

### Ubicación
```
BlazorCasinoApp/BlazorCasinoApp/
```

### Estructura
- `Pages/` → Páginas Razor para gestión (RussianRoulette, etc.)
- `Service/` → Servicios HTTP que consumen la API
- `IApiService/` → Interfaces de servicios
- `Auth/` → Autenticación del backoffice

---

## 🐳 Docker

El `dockerfile` en la raíz compila y publica solo la API:
```dockerfile
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build-env
# Restaura → publica → ejecuta con aspnet:9.0
ENTRYPOINT ["dotnet", "Chaos.Api.dll"]
```

---

## 🎮 Flujo del Sistema de Lobbies (Ruleta Rusa)

1. **Crear lobby**: `POST /api/Lobby/create` → crea lobby con status `Waiting` + primer jugador como master
2. **Unirse**: `POST /api/Lobby/join` con `lobbyCode` → añade jugador
3. **Polling**: Frontend hace polling cada 3s a `GET /api/Lobby/select/{id}` para actualizar estado
4. **Kick**: `DELETE /api/Lobby/Kick/Player` → solo el master puede kickear
5. **Leave**: `POST /api/Lobby/leave?lobbyId=...` → si el master sale, se elimina todo el lobby
6. **Eliminar lobby**: `DELETE /api/Lobby/Remove`
7. **Iniciar juego**: `POST /api/RussianRouletteControllerr/Start/{lobbyId}` → cambia status a `InProgress`
8. **Jugar rondas**: `POST /api/RussianRouletteControllerr/PlayRound/{lobbyId}`
9. **Estado**: `GET /api/RussianRouletteControllerr/Status/{lobbyId}`

### Cleanup automático
- Si el **master** sale del lobby (navega fuera, cierra pestaña, refresca), el frontend llama a `/api/Lobby/leave` automáticamente
- El backend detecta que el master es quien sale y **elimina todo el lobby** + jugadores

---

## 🔐 Autenticación

### JWT
- **Clave**: Configurada en `appsettings.json` → `Jwt:Key`
- **Issuer / Audience**: `JwtTestDemo`
- **Claims**: `ClaimTypes.NameIdentifier` (userId), `ClaimTypes.Name`, `ClaimTypes.Role`
- **Middleware**: `TokenBlackListMiddleware` verifica token no revocado y usuario activo

### Frontend
- Token almacenado en `localStorage` como `token_casino`
- `ProtectedRoute` component verifica existencia del token antes de renderizar rutas protegidas
- Hook `useAuth` encapsula la lógica de autenticación

---

## 📦 Dependencias clave

### Backend (NuGet)
- `Microsoft.EntityFrameworkCore` + `Npgsql.EntityFrameworkCore.PostgreSQL`
- `Microsoft.AspNetCore.Authentication.JwtBearer`
- `Swashbuckle.AspNetCore` (Swagger)
- `Scalar.AspNetCore` (API docs)

### Frontend (npm)
- `react` 19, `react-dom` 19
- `react-router-dom` 7
- `vite` 8 + `@vitejs/plugin-react` 6
- `typescript` 6

---

## ⚡ Comandos de desarrollo rápido

| Acción | Comando | Directorio |
|---|---|---|
| **Ejecutar API** | `dotnet run` | `Chaos/` |
| **Compilar API** | `dotnet build Chaos\Chaos.Api.csproj` | Raíz |
| **Ejecutar Frontend** | `npm run dev` | `ChaosFrontend/` |
| **Build Frontend** | `npm run build` | `ChaosFrontend/` |
| **Nueva migración EF** | `dotnet ef migrations add <Name> --startup-project ../Chaos/Chaos.Api.csproj` | `Chaos.Infraestructure/` |
| **Aplicar migraciones** | `dotnet ef database update --startup-project ../Chaos/Chaos.Api.csproj` | `Chaos.Infraestructure/` |
| **Docker build** | `docker build -t chaos-api .` | Raíz |

---

## 📝 Convenciones del proyecto

1. **Servicios**: Siempre registrados como `Scoped` en `Program.cs` (excepto Singletons mencionados)
2. **Controladores**: Heredan de `ControllerBase`, no de `Controller`
3. **Nombres**: Los controladores acaban en `Controller` (nota: `RussianRouletteControllerr` tiene doble `r`)
4. **DTOs**: Request en `RequestEntity/`, Response en `ResponseEntity/`
5. **Config Controllers/Services**: Para las operaciones CRUD del backoffice, van en subcarpeta `Config/`
6. **CSS**: Un archivo por componente/página en `src/styles/`
7. **Base de datos**: Todas las columnas de fecha se guardan como `string` (no como `DateTime`)
8. **Frontend**: No se usa axios, se usa `fetch()` nativo
9. **Estado**: No hay state manager global, se usa `useState` local
