# Nanobin

A hyper-minimalistic pastebin alternative built with .NET and Blazor.

## Features

### Current
- **Syntax Highlighting:** Syntax highlighting for code snippets.
- **Code Un-indentation:** Automatically removes common leading whitespace from your code snippets.
- **Timestamps:** Each paste is timestamped with its creation date and time.

### Planned
- Automatic end-to-end encryption
- Add expiration, both with a default value and a dropdown to change this value
- Add a language dropdown to change the syntax highlighting if it picks the wrong language automatically

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

The application will be available at `http://localhost:5000`.

### Running with Docker

1.  **Build the Docker image:**
    ```bash
    docker build -t nanobin .
    ```

2.  **Run the Docker container:**
    ```bash
    docker run -p 8080:8080 nanobin
    ```

The application will be available at `http://localhost:8080`.

## Contributing

Contributions are welcome, however this project is intended to be as minimalistic as is reasonable, so new features will need to be discussed first.
