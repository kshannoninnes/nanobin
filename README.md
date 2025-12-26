# nanobin

A minimalist, privacy-focused pastebin application with client-side encryption. Built with a modern tech stack featuring a .NET backend and React frontend. Re-written from [Nanobin-Legacy](https://github.com/kshannoninnes/nanobin-legacy/)

Try it out at https://nanobin.orthrus.dev

## Screenshots
<p>
  <a href="https://github.com/user-attachments/assets/9d3d7070-47eb-4165-a3c9-647b2bec8338">
    <img src="https://github.com/user-attachments/assets/9d3d7070-47eb-4165-a3c9-647b2bec8338" width="320" />
  </a>
  <a href="https://github.com/user-attachments/assets/e5351f80-cc27-47db-986c-62724624b9e4">
    <img src="https://github.com/user-attachments/assets/e5351f80-cc27-47db-986c-62724624b9e4" width="320" />
  </a>
  <a href="https://github.com/user-attachments/assets/0914d8fa-665d-4ce1-a709-011f1a6f1604">
    <img src="https://github.com/user-attachments/assets/0914d8fa-665d-4ce1-a709-011f1a6f1604" width="320" />
  </a>
</p>

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
- **Highlight.JS** - Automatic syntax highlighting

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
- Each paste stores an encryption key in the URL fragment (after the `#`), which is never sent to the server
- The server only stores encrypted blobs and has no way to decrypt it
