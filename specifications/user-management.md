# User Management

## Main Features

- User authentication
- Role management (administrator, product owner, translator, reader)
- Rights assignment by project or component
- User action tracking (audit)
- API key management for integration with external tools and CI/CD systems

## Business Rules

- Only administrators can manage users and rights
- Only administrators can create solutions
- Administrators or product owners can create projects, components, and files
- Administrators or product owners can add required languages for a solution
- Administrators, product owners, or translators can upload, download, and update translation content
- Readers have read-only access
- API keys can be generated, revoked, and associated with specific users or integration scopes
- API keys are required for external tool and CI/CD system integration
- An API key is considered as having product owner rights

## Data Entities

### User Entity

A user has the following properties:
- Unique identifier (id)
- Name
- Email
- Password (securely hashed and stored)
- Platform access level: administrator or non-administrator
- Validation status: pending or confirmed

### User-Solution Access Mapping Entity

A join table (UserSolutionAccess) links users to solutions and defines their access level for each solution:
- User id
- Solution id
- Access level: product owner, translator, or reader

### External API Key Entity

An external API key has the following properties:
- Unique identifier (id)
- Solution id (the solution this key is associated with)
- Key value (the API key itself, securely stored)

## User Creation Process

1. The administrator creates the user by providing the name, email, general access level (administrator or not), and the solutions the user will have access to (with the access level for each solution). The user is created with status "pending" and a random password.
2. An email is sent to the user with a link to complete their account setup. This link contains the random password and user ID, both encoded for identification.
3. The user clicks the link and is taken to an "account finalization" page, where they can change their name and choose a password. Upon submitting this form, the account is finalized and the user's status becomes "confirmed".
