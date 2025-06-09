# Technical Choices

## Backend

- **Technology**: .NET 8.0
- **Language**: C#
- **Architecture**: RESTful API
- **Structure**: The backend will be organized using a simple and pragmatic approach:
  - Controllers: handle HTTP requests and responses, and call business services.
  - Services: contain business logic and interact directly with the Entity Framework DbContext (no unnecessary abstractions or interfaces unless justified).
  - Models: data classes (entities, DTOs, etc.).
- **Background service**: The backend will include, in addition to the APIs, a background service for maintenance tasks and long-running jobs (e.g., sending emails).
- **API/Background communication**: Information exchange between the APIs and the background service will be performed using Channels (producer/consumer pattern).
- **Logging**: Logging and tracing will be based on OpenTelemetry for distributed tracing, metrics, and log collection.
- **Documentation**: All public methods, properties, and classes must be documented with XML comments. These comments must be made available to the OpenAPI module to provide detailed operation descriptions in the generated API documentation.
- **Authentication**: JWT or OAuth2 (to be specified as needed)
- **Database**: PostgreSQL (main data storage)
- **Migration management**: Entity Framework Core
- **Tests**: MSTest
- **API documentation**: Mandatory OpenAPI (Swagger) declaration for all endpoints. All public methods, properties, and classes must include XML comments. These XML comments must be made available to the OpenAPI module to provide detailed documentation for all API operations, parameters, and models.

## Frontend

- **Framework**: Vue.js 3.0
- **UI**: Vuetify
- **State management**: Pinia
- **Routing**: Vue Router
- **Utilities**: UseVue
- **Language**: TypeScript
- **API calls management**: Axios
- **Tests**: Vitest or Jest
- **Accessibility**: The user interface must be fully usable with the keyboard. All features must be accessible via clear tab navigation and keyboard shortcuts. Focus indicators must be visible, and all interactive elements must be reachable and operable without a mouse.

## Code Organization

- Strict separation between backend (src/backend) and frontend (src/frontend)
- Use of clear and consistent naming conventions
- Code and API documentation (Swagger for the backend)

## Naming Convention

- All property names, tables, database columns, API endpoints, methods, business objects, etc. must be written in English (e.g., user_id, translationRequest, getProjectList, etc.).
- This convention applies to both backend and frontend, as well as technical documentation and database schemas.

## CI/CD

- Continuous integration and deployment provided via **GitHub Actions**
- Pipelines for:
  - Backend build and tests (dotnet)
  - Frontend build and tests (npm, Vitest/Jest)
  - Static code analysis
  - Docker image publication
  - Automated deployment (if needed)

## Deployment

- Containerization with Docker
- Multi-service orchestration and configuration with Docker Compose

## Security

- Rights management on both backend and frontend
- User input validation
- Logging of access and sensitive actions

## Internationalization

- Native i18n support on the frontend
- Centralized management of resource files

## Configuration

- All configuration parameters, such as database connection information, must be provided via environment variables.
- This applies to both backend and frontend (if needed for API URLs, etc.).
- No sensitive information should be hardcoded in the source code or configuration files committed to version control.

## Recommended Tools

- Visual Studio Code
- Postman for API testing
- Git for versioning

## Error Handling

- All API endpoints must return errors using the "problem details" format as defined in [RFC 7807](https://www.rfc-editor.org/rfc/rfc7807).
- Error responses must include at least the following fields: `type`, `title`, `status`, `detail`, and `instance`.
- API endpoints must handle the `Accept-Language` HTTP header to personalize error messages in the user's language when possible. The default language is `en-US` if the header is not provided or not supported.
- Error messages must never expose implementation-specific details such as SQL queries, URLs, connection strings, credentials, or third-party API/nuget usage details. Instead, errors should clearly indicate the component and the type of problem encountered, in a user-understandable way. 
  - Example: An authentication error with an external API should be reported as "Credentials for OtherApi was incorrect" rather than exposing the URL or credentials.
  - Example: A database error should be reported as "Update failed on theDataConcept" rather than exposing the SQL query or database structure.
- Log messages, on the other hand, must capture and retain all technical details of the error (such as stack traces, SQL queries, URLs, connection strings, and third-party API/nuget usage details) to facilitate debugging and root cause analysis. These details must never be sent to the end user but must be available in the logs for developers and operators.
