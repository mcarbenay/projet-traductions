# External Connections and Identifiers Management

## Objective

Enable the management of connections and identifiers from external systems (AzureDevOps, GitHub, API, etc.) for projects or other solution entities.

## Main Features

- Define and register different types of external connections (e.g., AzureDevOps, GitHub, custom API)
- Associate one or more external identifiers with a project or other entity
- Specify the source and type of identifier (e.g., repositoryId, pipelineId, projectKey, etc.)
- Allow viewing, editing, and deleting these identifiers from the interface
- Keep a history of external identifier modifications

## External Identifier Structure

- **ID (UUID)**: Universally unique identifier of the external identifier (for referencing from a project)
- **Source type**: (e.g., AzureDevOps, GitHub, API)
- **Field name**: (e.g., repositoryId, pipelineId, projectKey)
- **Value**: (e.g., 123456, "repo-xyz", "my-project-key")
- **Date added/modified**
- **User who modified**

## Business Rules

- A project can reference only one external identifier (one source) at a time. However, an external identifier can be used by several projects.
- External identifiers must be unique for the same source and field type within projects.
- Access rights to manage external identifiers are aligned with those of the related project.
