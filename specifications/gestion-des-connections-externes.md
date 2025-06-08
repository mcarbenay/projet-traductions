# Gestion des connexions et identifiants externes

## Objectif

Permettre de gérer les connexions et identifiants provenant de systèmes externes (AzureDevOps, GitHub, API, etc.) pour les projets, composants ou autres entités de la solution.

## Fonctionnalités principales

- Définir et enregistrer différents types de connexions externes (ex : AzureDevOps, GitHub, API personnalisée).
- Associer un ou plusieurs identifiants externes à un projet, composant ou autre entité.
- Préciser la source et le type d’identifiant (ex : repositoryId, pipelineId, projectKey, etc.).
- Permettre la consultation, la modification et la suppression de ces identifiants depuis l’interface.
- Historiser les modifications des identifiants externes.

## Structure d’un identifiant externe

- **Identifiant (UUID)** : Identifiant unique universel de l’identifiant externe (permettant la référence depuis un projet).
- **Type de source** : (ex : AzureDevOps, GitHub, API)
- **Nom du champ** : (ex : repositoryId, pipelineId, projectKey)
- **Valeur** : (ex : 123456, "repo-xyz", "my-project-key")
- **Date d’ajout / modification**
- **Utilisateur ayant effectué la modification**

## Règles de gestion

- Un projet ne peut référencer qu’un seul identifiant externe (une seule source). Toutefois, un identifiant externe peut servir à plusieurs projet.
- Les identifiants externes doivent être uniques pour une même source et un même type de champ au sein des projets.
- Les droits d’accès à la gestion des identifiants externes sont alignés sur ceux du projet concerné.
