# User Management

## Main Features

- User authentication
- Role management (administrator, product owner, translator, reader)
- Rights assignment by project or component
- User action tracking (audit)
- API key management for integration with external tools and CI/CD systems

## Business Rules

- Only administrators can manage users and rights
- Product owners can manage projects and components they own
- Translators can edit translation files they have access to
- Readers have read-only access
- API keys can be generated, revoked, and associated with specific users or integration scopes
- API keys are required for external tool and CI/CD system integration
