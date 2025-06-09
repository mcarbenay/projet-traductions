# Implementation Documentation

## Project Structure

- **Solution Name:** UseTheOps.PolyglotInitiative
- **Backend Path:** src/backend/UseTheOps.PolyglotInitiative
- **Test Project:** src/backend/UseTheOps.PolyglotInitiative.Tests
- **Key Folders:**
  - Controllers
  - Models
  - Data
  - Services
  - Middleware
  - docs

## NuGet Configuration
- The `nuget.config` file is present in `src/backend` and is used to ignore unauthorized sources.

## Key Technical Choices
- .NET 8.0, C#, RESTful API
- PostgreSQL with Entity Framework Core
- JWT authentication (user & API key)
- OpenTelemetry for logging/tracing
- All user-facing/log strings in .resx files
- TestContainers for integration tests
- All public methods/classes documented for Swagger

## Implementation Steps
1. Solution and project initialization
2. Entity and DbContext modeling
3. Authentication and authorization
4. Business services
5. API controllers
6. Background services
7. Internationalization
8. Tests
9. CI/CD compliance

## Notes
- All code follows the namespace `UseTheOps.PolyglotInitiative` and sub-namespaces.
- Middleware and non-testable code is marked as `ExcludedFromCodeCoverage`.
- All error handling and logging is centralized and uses OpenTelemetry.
- All API endpoints are documented and secured.

---

This file will be updated as the implementation progresses.
