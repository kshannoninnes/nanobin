# ---------- Frontend ----------
FROM node:20-alpine AS frontend
WORKDIR /src/frontend

COPY frontend/package*.json ./
RUN npm ci

COPY frontend/ ./
RUN npm run build


# ---------- Backend ----------
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS backend
WORKDIR /src

COPY backend/Nanobin.API/Nanobin.API.csproj ./backend/Nanobin.API/
RUN dotnet restore ./backend/Nanobin.API/Nanobin.API.csproj

COPY backend/ ./backend/

RUN rm -rf ./backend/Nanobin.API/wwwroot || true
RUN mkdir -p ./backend/Nanobin.API/wwwroot
COPY --from=frontend /src/frontend/dist/ ./backend/Nanobin.API/wwwroot/

RUN dotnet publish ./backend/Nanobin.API/Nanobin.API.csproj -c Release -o /app/publish


# ---------- Start ----------
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS start
WORKDIR /app

COPY --from=backend /app/publish ./

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "Nanobin.API.dll"]