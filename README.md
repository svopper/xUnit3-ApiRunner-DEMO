# xUnit3 API Runner Demo

This is a demo project that demonstrates how to trigger xUnit 3 tests using an API endpoint.

## Overview

This project showcases a web API application that can execute xUnit 3 tests programmatically. It combines an ASP.NET Core Web API with xUnit 3 test infrastructure, allowing tests to be run in multiple ways.

## Running Tests

This project supports multiple ways to run tests:

### 1. Using `dotnet test`

Run tests using the standard .NET test command:

```bash
dotnet test
```

### 2. Using Your IDE

Tests can be run directly from your IDE using built-in test runners:
- **Visual Studio**: Use the Test Explorer
- **Visual Studio Code**: Use the .NET Test Explorer extension
- **JetBrains Rider**: Use the built-in test runner

### 3. Using the `/run-tests` API Endpoint

Start the API application and trigger tests via HTTP (from the project root directory):

```bash
# Start the application
cd ApiTestRunner.Api
dotnet run

# In another terminal, trigger tests via the API
curl -X POST http://localhost:5000/run-tests
```

The endpoint returns a JSON response with test results.

## Docker Support

This project includes Docker support for containerized deployment.

### Building the Docker Image

```bash
docker buildx build -t apirunner -f ApiTestRunner.Api/Dockerfile .
```

### Running the Docker Container

```bash
docker run -p 8080:8080 apirunner
```

### Running Tests in Docker

Once the container is running, trigger tests using:

```bash
curl -X POST http://localhost:8080/run-tests
```

## Project Structure

- **ApiTestRunner.Api**: ASP.NET Core Web API application that hosts the test runner endpoint
- **ApiTestRunner.Tests**: xUnit 3 test project containing sample tests

## Requirements

- .NET 10.0 SDK
- Docker (optional, for containerized deployment)

## Purpose

This is a demonstration project for educational and testing purposes. It shows how to integrate xUnit 3 with a web API to enable remote test execution.
