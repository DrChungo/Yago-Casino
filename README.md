# 🏗️ Chaos Casino — Plataforma Web de Entretenimiento y Microeconomía

[![React 19](https://img.shields.io/badge/Frontend-React%2019-blue?style=flat-square&logo=react)](https://react.dev/)
[![.NET 9](https://img.shields.io/badge/Backend-.NET%209%20%2F%20C%23-purple?style=flat-square&logo=dotnet)](https://dotnet.microsoft.com/)
[![PostgreSQL](https://img.shields.io/badge/Database-PostgreSQL%20%2F%20Supabase-blue?style=flat-square&logo=postgresql)](https://www.postgresql.org/)
[![Blazor WASM](https://img.shields.io/badge/Backoffice-Blazor%20WASM-purple?style=flat-square&logo=dotnet)](https://dotnet.microsoft.com/apps/aspnet/web-apps/blazor)

**Chaos Casino** es un ecosistema monorepo fullstack premium de juegos de azar multijugador online, integrado con una economía interactiva de mascotas virtuales (biomas), tienda, bar con efectos de estado y rankings globales. Dispone de un panel de administración (Backoffice) desarrollado en Blazor WebAssembly para gestionar toda la lógica interna y configuraciones del casino.

---

## 🚀 Funcionalidades Clave

### 👥 Ruleta Rusa Multijugador (Lobby en Tiempo Real)
* **Gestión de Lobbies**: Creación y unión de partidas mediante códigos de acceso únicos.
* **Roles y Privilegios**: Los administradores del lobby pueden expulsar (*kick*) jugadores y comenzar la partida de forma segura.
* **Limpieza Automática**: Control inteligente para eliminar lobbies del sistema si el creador abandona la sesión antes de comenzar el juego.
* **Multijugador Síncrono**: Polling eficiente de sincronización de estado de la partida.

### 🎰 Juegos de Casino Clásicos
* **Blackjack**: Partidas dinámicas con cálculo automatizado de cartas y crupier.
* **Slots (Tragaperras)**: Motor de slots personalizable con sistema de líneas de pago y carretes visuales.
* **European Roulette & CoinFlip**: Apuestas síncronas a números, colores o lanzamientos de moneda.
* **Higher or Lower**: Juego de probabilidad basado en predicción de cartas de baraja.

### 🐾 Ecosistema de Mascotas Virtuales & Biomas
* **Hábitats Interactivos (Biomas)**: 9 biomas únicos (Granja, Jurásico, Océano, Sabana, Jungla, etc.) donde los animales se mueven utilizando motores de físicas en Canvas 2D.
* **Microeconomía**: Compra, venta y evolución de mascotas mediante monedas virtuales obtenidas en los juegos del casino.
* **Apuestas de Mascotas**: Posibilidad de apostar tus animales en las partidas de casino.

### 🍻 Bar Interactiva y Consumibles
* Adquisición de bebidas con efectos de estado específicos para los usuarios (visualizados mediante componentes UI interactivos en el frontend).

### 📈 Rankings Globales
* Tablas de clasificación en tiempo real ordenadas por fortuna de usuario, valor de animales y estadísticas del casino.

### 🛠️ Backoffice Administrativo (Blazor WebAssembly)
* Portal independiente para la administración total de juegos, control de configuraciones de las máquinas tragaperras, tasas de ganancias, balance general y gestión de usuarios.

---

## 🛠️ Stack Tecnológico

| Componente | Tecnologías Utilizadas |
| :--- | :--- |
| **Frontend** | React 19, TypeScript, Vite 8, React Router DOM 7, Vanilla CSS, Canvas 2D |
| **Backend API** | C#, .NET 9, ASP.NET Core Web API, JWT Auth |
| **Base de Datos** | PostgreSQL (Alojado en Supabase), Entity Framework Core, SQL Server Database Project |
| **Backoffice** | Blazor WebAssembly, .NET 9 |

---

## 📁 Estructura del Proyecto

* `Chaos/`: Backend API en ASP.NET Core, servicios y lógica de negocio.
* `Chaos.Infraestructure/`: Capa de persistencia con Entity Framework Core y migraciones para PostgreSQL.
* `Chaos.Shared/`: Modelos y DTOs comunes compartidos entre backend, frontend y Blazor WASM.
* `ChaosFrontend/`: Frontend React + Vite + TypeScript.
* `BlazorCasinoApp/`: Portal de Backoffice/Administración Blazor WebAssembly.
* `CasinoDB/`: Proyecto de base de datos SQL para estructuración de esquemas y queries.

---

## ⚙️ Guía de Instalación y Ejecución Local

### Prerrequisitos
* [.NET SDK 9.0](https://dotnet.microsoft.com/download/dotnet/9.0)
* [Node.js](https://nodejs.org/) (Versión v18 o superior recomendada)
* [PostgreSQL](https://www.postgresql.org/) (o instancia activa de Supabase)

---

### 🟢 1. Configuración de Base de Datos y Backend

1. Accede al archivo `Chaos/appsettings.json` y configura tu cadena de conexión PostgreSQL (`ConnectionStrings:DefaultConnection`) y la configuración JWT.
2. Abre tu terminal en la raíz del proyecto y aplica las migraciones para generar la base de datos:
   ```bash
   dotnet ef database update --project Chaos.Infraestructure/ --startup-project Chaos/
   ```
3. Ejecuta la API:
   ```bash
   dotnet run --project Chaos/
   ```
   > Por defecto, la API estará disponible en `https://localhost:7101` y la documentación interactiva en `https://localhost:7101/swagger`.

---

### 🔴 2. Configuración del Frontend (React)

1. Dirígete al directorio de frontend:
   ```bash
   cd ChaosFrontend
   ```
2. Instala las dependencias necesarias:
   ```bash
   npm install
   ```
3. Crea un archivo `.env` en la raíz del directorio `ChaosFrontend/` con el siguiente contenido:
   ```env
   VITE_BASE_URL=https://localhost:7101
   ```
4. Ejecuta el servidor de desarrollo:
   ```bash
   npm run dev
   ```
   > El sitio frontend se abrirá en `http://localhost:5173`.

---

### 🟣 3. Configuración del Backoffice (Blazor WASM)

1. Dirígete a la carpeta del proyecto Blazor:
   ```bash
   cd BlazorCasinoApp/BlazorCasinoApp
   ```
2. Ejecuta la aplicación Blazor:
   ```bash
   dotnet run
   ```

---

## 🐳 Despliegue con Docker

El proyecto cuenta con un `dockerfile` listo para contenerizar la API de ASP.NET Core:

```bash
# Compilar la imagen Docker
docker build -t chaos-api .

# Ejecutar el contenedor
docker run -d -p 8080:8080 --name chaos-api-container chaos-api
```

---

## 📄 Licencia

Este proyecto está bajo la Licencia MIT - mira el archivo [LICENSE](file:///c:/Users/z00599aj/Desktop/Chaos-Api-practices_2026/Chaos-Api-practices_2026/LICENSE) para más detalles.
