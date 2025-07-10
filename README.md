# Nanobin

A hyper-minimalistic pastebin alternative built with .NET and Blazor.

## Features

### Current
- Syntax highlighting for code snippets.
- Automatically removes common leading whitespace from your code snippets.
- Uses SQLite for efficient, file-based storage.

### Planned
- Automatic end-to-end encryption
- Automatic expiration, both with a default value and a dropdown to change this value
- Language selection to change the syntax highlighting if it picks the wrong language automatically

## Getting Started

### Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Docker](https://www.docker.com/get-started) (Optional, for running in a container)

### Running Locally

1.  **Clone the repository:**
    ```bash
    git clone https://github.com/kshannoninnes/Nanobin.git
    cd Nanobin
    ```

2.  **Run the application:**
    ```bash
    dotnet run
    ```

The application will be available at `http://localhost:5148`.

### Running with Docker

1. **Build the Docker image:**
   ```bash
   docker build -t nanobin .
   ```

2. **Run the Docker container:**
   ```bash
   docker run -v /database/path/on/host:/data -p 8080:8080 nanobin
   ```

   The application will be available at `http://localhost:8080`.

## Troubleshooting

### Permission Issues

If you encounter permission problems when running the Docker container:

1. **Volume Ownership**: Ensure the mounted volume has appropriate permissions:
   ```bash
   # On the host system, ensure the directory is accessible
   sudo chown -R <user>:<group> /database/path/on/host
   ```

2. **User Configuration**: You may need to run the container with a specific user ID:
   ```bash
   docker run -u <uid>:<gid> -v /database/path/on/host:/data -p 8080:8080 nanobin
   ```

## Contributing

Contributions are welcome, however this project is intended to be as minimalistic as is reasonable, so new features will need to be discussed first.
