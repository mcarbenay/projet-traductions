# External Integrations

## Objective

Describe the supported external integrations (e.g., AzureDevOps, GitHub, custom APIs) and how the platform interacts with them for synchronization, automation, or data retrieval.

## Supported Integrations

- AzureDevOps (work items, pipelines, repositories)
- GitHub (repositories, webhooks)
- Custom APIs (translation providers, CI/CD, etc.)

## Integration Features

- Synchronization of translation needs and statuses
- Automated import/export of resource files
- Webhook/event handling for triggering platform actions
- API key and credential management for secure access
- Error handling and retry logic for external calls

## Security & Configuration

- All credentials and API keys are stored securely and never exposed in logs or error messages
- Integration configuration is managed via environment variables or secure vaults

## Business Rules

- Only users with appropriate rights can configure or trigger integrations
- All integration actions are logged and auditable

## Technical Notes

- Integrations are implemented as services or background jobs
- All external calls are traced and monitored via OpenTelemetry
- Failures are reported using the platform's error handling conventions
