# Hakime Project

Hakime is a groundbreaking project aimed at interconnecting health services in Ethiopia. This repository contains the backend API built with .NET 7 and MongoDB for robust data management.

## Prerequisites

Before you begin, ensure you have the following installed:

- [.NET 7](https://dotnet.microsoft.com/download/dotnet/7.0)
- [MongoDB](https://www.mongodb.com/try/download/community)

## Setup

1. Clone the repository:

    ```bash
    git clone https://github.com/HabeshaCare/BackendServer.git
    ```

2. Navigate to the project directory:

    ```bash
    cd BackendServer
    ```

3. (Optional) Update the MongoDB connection string in `appsettings.json`:

    ```json
    
    "DB": {
            "ConnectionURL": "mongodb://localhost:27017",
            "DBName": "Your preferred database"
          }
    ```

## Running the Application

1. Build and run the application:

    ```bash
    dotnet run
    ```

2. Access the API at `http://localhost:5072`.

## Mobile Application

Explore the interconnected health experience further with the Hakime Mobile App. Find the mobile app repository [here](https://github.com/michael-099/Hakime).
## Detailed documentation
For detailed documentation, visit the [Hakime Project GitHub Pages](https://habeshacare.github.io/BackendServer/).

Feel free to contribute and enhance the health services ecosystem in Ethiopia with Hakime!
