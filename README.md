# Wisheo Backend

Wisheo es una plataforma social para gestionar listas de deseos y conectar con amigos. Este backend está construido con ASP.NET Core y arquitectura en capas, con soporte para autenticacion propia, SSO con Firebase, listas publicas, colaboracion y notificaciones push.

## Stack tecnologico

- **Framework:** .NET / ASP.NET Core
- **Base de datos:** PostgreSQL
- **ORM:** Entity Framework Core (Code First)
- **Autenticacion:** JWT + Firebase Admin SDK (Google/Apple SSO)
- **Real-time:** SignalR
- **Push notifications:** Firebase Cloud Messaging (FirebaseAdmin.Messaging)
- **Infraestructura:** Docker & Docker Compose

## Arquitectura

El proyecto sigue el patrón de arquitectura en capas:

```
Controllers  →  Services  →  Repositories  →  Database
```

- **Controllers** — endpoints REST y validación de entrada
- **Services** — lógica de negocio
- **Repositories** — abstracción de persistencia con Entity Framework
- **Models / DTOs** — entidades y objetos de transferencia

### Diagrama de clases

```mermaid
classDiagram
    class User {
        +Guid Id
        +string Name
        +string Surname
        +string Username
        +string Email
        +string? FirebaseUid
        +DateTime Birthday
    }
    class Wishlist {
        +Guid Id
        +string Title
        +string? Description
        +string? Emoji
        +bool IsPublic
        +Guid UserId
    }
    class WishItem {
        +Guid Id
        +string Name
        +string? Description
        +decimal? Price
        +string? ImageUrl
        +bool IsPurchased
    }
    class Post {
        +Guid Id
        +string Content
        +DateTime CreatedAt
        +Guid UserId
    }
    class Comment {
        +Guid Id
        +string Text
        +Guid PostId
        +Guid UserId
    }
    class Follow {
        +Guid FollowerId
        +Guid FollowedId
    }

    User "1" -- "*" Wishlist : owns
    Wishlist "1" -- "*" WishItem : contains
    User "1" -- "*" Post : writes
    Post "1" -- "*" Comment : has
    User "1" -- "*" Comment : writes
    User "1" -- "*" Follow : follows
```

### Flujo de autenticacion

```mermaid
sequenceDiagram
    participant App
    participant Backend
    participant Firebase

    Note over App,Backend: Credenciales propias
    App->>Backend: POST /api/users/login (username, password)
    Backend-->>App: JWT + Refresh Token

    Note over App,Backend: SSO (Google / Apple)
    App->>Firebase: signInWithPopup / authenticate
    Firebase-->>App: Firebase ID Token
    App->>Backend: POST /api/users/firebase-login (idToken)
    Backend->>Firebase: VerifyIdTokenAsync
    Firebase-->>Backend: Token válido
    Backend-->>App: JWT + Refresh Token
```

## Endpoints principales

### Autenticacion
| Método | Ruta | Descripción |
|--------|------|-------------|
| POST | `/api/users/register` | Registro con credenciales |
| POST | `/api/users/login` | Login con username y contraseña |
| POST | `/api/users/firebase-login` | Login / registro con Firebase SSO |
| POST | `/api/users/refresh` | Renovar access token |
| PATCH | `/api/users/me` | Actualizar perfil |
| POST | `/api/users/me/device-token` | Registrar token FCM del dispositivo |

### Wishlists
| Método | Ruta | Descripción |
|--------|------|-------------|
| GET | `/api/wishlists` | Obtener wishlists del usuario |
| POST | `/api/wishlists` | Crear wishlist |
| PUT | `/api/wishlists/{id}` | Editar wishlist |
| DELETE | `/api/wishlists/{id}` | Eliminar wishlist |
| POST | `/api/wishlists/{id}/share` | Generar / recuperar slug publico para compartir |
| POST | `/api/wishlists/{id}/items` | Añadir item |
| PUT | `/api/wishlists/items/{id}` | Editar item |
| DELETE | `/api/wishlists/items/{id}` | Eliminar item |
| PATCH | `/api/wishlists/items/{id}/toggle-purchased` | Marcar/desmarcar como reservado |
| GET | `/api/wishlists/{id}/collaborators` | Obtener colaboradores de una wishlist |
| POST | `/api/wishlists/{id}/collaborators` | Invitar colaborador por username |
| DELETE | `/api/wishlists/{id}/collaborators/{userId}` | Eliminar colaborador |

### Wishlists publicas (sin cuenta)
| Método | Ruta | Descripción |
|--------|------|-------------|
| GET | `/api/public/wishlists/{slug}` | Ver wishlist publica por slug |
| POST | `/api/public/wishlists/{slug}/items/{itemId}/reserve` | Reservar item como invitado |
| DELETE | `/api/public/wishlists/{slug}/items/{itemId}/reserve` | Cancelar reserva con token |

### Social
| Método | Ruta | Descripción |
|--------|------|-------------|
| POST | `/api/social/follow/{id}` | Seguir usuario |
| DELETE | `/api/social/unfollow/{id}` | Dejar de seguir |
| GET | `/api/social/followers` | Mis seguidores |
| GET | `/api/social/following` | A quién sigo |

### Feed y posts
| Método | Ruta | Descripción |
|--------|------|-------------|
| GET | `/api/feed` | Feed personalizado |
| POST | `/api/posts` | Crear post |
| POST | `/api/posts/{id}/comments` | Comentar en un post |

## Features implementadas (roadmap alta prioridad)

- **Feature 1 - Listas publicas compartibles:** slug publico, endpoint anonimo y reservas con token de cancelacion.
- **Feature 2 - Share sheet:** soporte en app cliente (sin cambios de API necesarios).
- **Feature 3 - Listas colaborativas:** modelo `WishlistCollaborator`, autorizacion por rol, endpoints de gestion de colaboradores.
- **Feature 4 - Push notifications:** modelo `DeviceToken`, endpoint de registro de token y servicio `NotificationService`.

### Eventos push actualmente conectados

- Reserva de item en lista publica -> notifica al owner.
- Nuevo follower -> notifica al usuario seguido.
- Nuevo comentario en post propio -> notifica al autor del post.
- Alta de colaborador en wishlist -> notifica al usuario invitado.

## Instalacion y ejecucion

### Requisitos
- .NET SDK
- PostgreSQL (o Docker)
- `dotnet-ef` tool

### 1. Levantar la base de datos

```bash
docker-compose up -d
```

### 2. Configurar secrets locales

```bash
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Port=5433;Database=wisheo_db;Username=postgres;Password=TU_PASSWORD"
dotnet user-secrets set "JwtSettings:Secret" "tu_clave_secreta"
dotnet user-secrets set "JwtSettings:Issuer" "wisheo-api"
dotnet user-secrets set "JwtSettings:Audience" "wisheo-app"
```

### 3. Configurar Firebase (SSO + FCM backend)

Descarga la clave de cuenta de servicio desde Firebase Console → Configuración del proyecto → Cuentas de servicio → Generar nueva clave privada, y guárdala como:

```
firebase-service-account.json  ← en la raíz del proyecto (excluido de git)
```

### 4. Aplicar migraciones

```bash
dotnet ef database update --connection "Host=localhost;Port=5433;Database=wisheo_db;Username=postgres;Password=TU_PASSWORD"
```

### 5. Ejecutar

```bash
dotnet run
```

La API estara disponible en `http://localhost:5000`.

## Seguridad

- Las contraseñas se almacenan hasheadas con BCrypt.
- El `firebase-service-account.json` está excluido de git via `.gitignore`.
- Los secrets de configuración se gestionan con `dotnet user-secrets` en local y variables de entorno en producción.
- Todos los endpoints protegidos requieren JWT via header `Authorization: Bearer <token>`.
