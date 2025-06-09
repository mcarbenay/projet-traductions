# TODO - Backend DTO Refactor

## Description
Refactor the backend API controllers for Solution, Project, and Component to only accept flat DTOs (no navigation properties or collections) for POST/PUT endpoints. Ensure that only allowed fields are accepted in the payloads.

## Features to Add/Modify
- Create DTOs for Solution, Project, and Component (Create/Update variants).
- Refactor `SolutionsController`, `ProjectsController`, and `ComponentsController` to use these DTOs for POST/PUT.
- Map DTOs to entities in the controller.
- Reject navigation properties/relations in input payloads.
- Update OpenAPI documentation if needed.

### [AJOUT 2025-06-09] API Keys & Auth
- Refactor du modèle `ApiKey` : suppression de `UserId`, ajout de `Scope`, liaison uniquement à `SolutionId`.
- Création des DTOs plats `ApiKeyCreateDto` et `ApiKeyDto`.
- Refactor du contrôleur `ApiKeysController` pour n'utiliser que ces DTOs et ne plus exposer le modèle EF.
- Ajout d'un endpoint d'authentification par clé API dans `AuthController` (`login/apikey`) générant un JWT avec rôle `ProductOwner` sur la solution liée.
- Migration EF Core régénérée à partir de zéro pour refléter le nouveau modèle.

## Files/Code to Modify
- `src/backend/UseTheOps.PolyglotInitiative/Controllers/SolutionsController.cs`
- `src/backend/UseTheOps.PolyglotInitiative/Controllers/ProjectsController.cs`
- `src/backend/UseTheOps.PolyglotInitiative/Controllers/ComponentsController.cs`
- `src/backend/UseTheOps.PolyglotInitiative/Models/Dtos/SolutionCreateDto.cs`
- `src/backend/UseTheOps.PolyglotInitiative/Models/Dtos/SolutionUpdateDto.cs`
- `src/backend/UseTheOps.PolyglotInitiative/Models/Dtos/ProjectCreateDto.cs`
- `src/backend/UseTheOps.PolyglotInitiative/Models/Dtos/ProjectUpdateDto.cs`
- `src/backend/UseTheOps.PolyglotInitiative/Models/Dtos/ComponentCreateDto.cs`
- `src/backend/UseTheOps.PolyglotInitiative/Models/Dtos/ComponentUpdateDto.cs`
- `src/backend/UseTheOps.PolyglotInitiative/Models/ApiKey.cs`
- `src/backend/UseTheOps.PolyglotInitiative/Models/Dtos/ApiKeyCreateDto.cs`
- `src/backend/UseTheOps.PolyglotInitiative/Models/Dtos/ApiKeyDto.cs`
- `src/backend/UseTheOps.PolyglotInitiative/Controllers/ApiKeysController.cs`
- `src/backend/UseTheOps.PolyglotInitiative/Controllers/AuthController.cs`
- `src/backend/UseTheOps.PolyglotInitiative/Services/ApiKeyService.cs`
- `src/backend/UseTheOps.PolyglotInitiative/Data/PolyglotInitiativeDbContext.cs`
- `src/backend/UseTheOps.PolyglotInitiative/Migrations/` (migration EF Core initiale)

## Tests to Add/Modify
- Update or add tests to ensure that navigation properties/relations are rejected in POST/PUT payloads (when tests are re-enabled).
- Ajouter des tests pour l'authentification par clé API et la gestion des droits associés.

---
_Last updated: 2025-06-09_
