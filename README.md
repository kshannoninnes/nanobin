# Nanobin

A minimalist, privacy-focused pastebin application with client-side encryption. Built with a modern tech stack featuring a .NET backend and React frontend. Re-written from [Nanobin-Legacy](https://github.com/kshannoninnes/nanobin-legacy/)

Try the demo at https://nanobin.orthrus.dev

## Features

- **Client-side encryption** - Content is encrypted in the browser before being sent to the server
- **Fast and lightweight** - Minimal dependencies and optimized performance
- **1-Click Docker Compose Deploy** - Easy deployment with Docker and Docker Compose
- **SQLite database** - File-based storage. No complicated DBMS setup required.
- **Clean UI** - Simple minimalistic design
- **Secure by design** – Uses client-side–only React (no React Server Components), so server-side React vulnerabilities do not apply.

## Tech Stack

### Backend
- **.NET 9** – ASP.NET Core Web API
- **SQLite** – Lightweight, embedded database
- **Swagger / OpenAPI** – API documentation in development

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
- Each paste has a unique encryption key stored in the URL fragment (after the `#`), which is never sent to the server
- The server only stores encrypted blobs and has no way to decrypt it
