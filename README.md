# Reqres Clean Architecture Sample

This solution demonstrates a Clean Architecture approach for consuming the [Reqres](https://reqres.in/) public API using .NET 9 and C# 13.0. It is organized into distinct layers for maintainability, testability, and separation of concerns.

## Solution Structure

- **Reqres.Client**  
  Entry point and configuration. Handles application startup, dependency injection, and configuration loading.

- **Reqres.Application**  
  Contains business logic and service interfaces. Defines contracts for use cases and orchestrates application workflows.

- **Reqres.Infrastructure**  
  Implements external API communication, configuration, and data mapping. Contains adapters for the Reqres API and related models.

## Key Features

- **Clean Architecture**: Clear separation between application, infrastructure, and client layers.
- **Strongly-Typed Configuration**: Uses the Options Pattern for API settings.
- **HttpClient Usage**: Centralized, resilient HTTP communication with error handling and logging.
- **Model Mapping**: Maps external API DTOs to internal application models.
- **Extensible**: Easily add new endpoints or swap infrastructure implementations.

## Getting Started

### Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- Visual Studio 2022 (or later)

### Configuration

Edit `Reqres.Client/appsettings.json` to set the Reqres API base URL and cache duration:
