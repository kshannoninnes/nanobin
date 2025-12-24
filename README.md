# Nanobin

A minimalist, privacy-focused pastebin application with client-side encryption. Built with a modern tech stack featuring a .NET backend and React frontend. Try the demo at https://nanobin.orthrus.dev

## Features

- **Client-side encryption** - Content is encrypted in the browser before being sent to the server
- **Fast and lightweight** - Minimal dependencies and optimized performance
- **1-Click Docker Compose Deploy** - Easy deployment with Docker and Docker Compose
- **SQLite database** - Simple, file-based storage. No complicated dbms setup required.
- **Clean UI** - Simple minimalistic design
- **Secure** - Not affected by recent react vulnerabilities, due to not using react-server-dom packages. All react usage is client-side only.

## Tech Stack

### Backend
- **.NET 9.0** - Modern ASP.NET Core Web API
- **SQLite** - Lightweight, embedded database
- **Swagger/OpenAPI** - API testing in dev

### Frontend
- **React 19.2** - Modern UI library
- **TypeScript 5.9** - Type-safe JavaScript
- **Vite** - Fast build tool and dev server
- **React Router** - Client-side routing
- **Zod** - Runtime type validation
- **Web Crypto API** - Client-side encryption

## Getting Started

### Prerequisites
- [Docker](https://www.docker.com/) and Docker Compose (recommended)
- OR [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0) and [Node.js 20+](https://nodejs.org/)

### Running with Docker (Recommended)

1. Clone the repository:

```bash
git clone https://github.com/kshannoninnes/nanobin
cd nanobin
```

2. Build and run with Docker Compose:

```bash
docker-compose up -d
```

3. Access the application at http://localhost:36001

The SQLite database will be persisted in a Docker volume named `nanobin_sqlite`.

### Running Locally for Development

#### Backend

```bash
cd backend/Nanobin.API
dotnet restore
dotnet run
```

The API will be available at http://localhost:5000 (or as configured in launchSettings.json). API documentation is available via Swagger UI when running in development mode at `/swagger`.

#### Frontend

```bash
cd frontend
npm install
npm run dev
```

The frontend dev server will be available at http://localhost:5173

## Security

- All paste content is encrypted client-side using AES-GCM before being sent to the server
- The encryption key is never transmitted to the server
- The server only stores encrypted blobs and has no way to decrypt it
- Each paste has a unique encryption key included in the URL fragment (after the '#' symbol in the URL) which is limited to the client only