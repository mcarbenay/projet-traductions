# Choix techniques

## Backend

- **Technologie** : .NET 8.0
- **Langage** : C#
- **Architecture** : API RESTful
- **Authentification** : JWT ou OAuth2 (à préciser selon besoins)
- **Base de données** : PostgreSQL (stockage principal des données)
- **Gestion des migrations** : Entity Framework Core
- **Tests** : MSTest
- **Documentation API** : Déclaration OpenAPI (Swagger) obligatoire pour l'ensemble des endpoints

## Frontend

- **Framework** : Vue.js 3.0
- **UI** : Vuetify
- **Gestion d’état** : Pinia
- **Routing** : Vue Router
- **Utilitaires** : UseVue
- **Langage** : TypeScript
- **Gestion des appels API** : Axios
- **Tests** : Vitest ou Jest

## Organisation du code

- Séparation stricte entre backend (src/backend) et frontend (src/frontend)
- Utilisation de conventions de nommage claires et homogènes
- Documentation du code et des API (Swagger pour le backend)

## CI/CD

- Intégration continue et déploiement continu assurés via **GitHub Actions**
- Pipelines pour :
  - Build et tests backend (dotnet)
  - Build et tests frontend (npm, Vitest/Jest)
  - Analyse statique du code
  - Publication des images Docker
  - Déploiement automatisé (si besoin)

## Déploiement

- Conteneurisation avec Docker
- Orchestration et configuration multi-services avec Docker Compose

## Sécurité

- Gestion des droits côté backend et frontend
- Validation des entrées utilisateur
- Journalisation des accès et actions sensibles

## Internationalisation

- Prise en charge native de l’i18n côté frontend
- Gestion centralisée des fichiers de ressources

## Outils recommandés

- Visual Studio Code
- Postman pour le test des API
- Git pour le versioning

## Convention de nommage

- Tous les noms de propriétés, tables, colonnes de base de données, points d’API, méthodes, objets métiers, etc. doivent être rédigés en anglais (ex : user_id, translationRequest, getProjectList, etc.).
- Cette convention s’applique aussi bien au backend qu’au frontend, ainsi qu’à la documentation technique et aux schémas de base de données.
