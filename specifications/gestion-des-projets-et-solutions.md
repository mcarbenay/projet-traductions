# Gestion des projets et des solutions

## Fonctionnalités principales

- Création, modification et suppression de solutions et de projets
- Organisation hiérarchique : solution > projet > composant > fichier de ressource
- Visualisation de la structure globale
- Association des fichiers de traduction aux composants

## Règles de gestion

- Un projet appartient à une seule solution
- Un composant appartient à un seul projet
- Un fichier de ressource appartient à un seul composant

## Entités de données

### Propriétés d’une solution

Une solution possède les propriétés suivantes :

1. **Identifiant (UUID)** : Identifiant unique universel de la solution.
2. **Code** : Code court unique permettant d’identifier la solution (ex : "ERP", "MOBILE_APP").
3. **Nom** : Nom unique de la solution.
4. **Description** : Texte descriptif de la solution.
5. **URL de présentation** : Lien vers une page de présentation ou documentation externe.
6. **Propriétaire** : Utilisateur référencé comme propriétaire de la solution.

### Propriétés d’un projet

Un projet possède les propriétés suivantes :

1. **Identifiant (UUID)** : Identifiant unique universel du projet.
2. **Code** : Code court unique permettant d’identifier le projet (ex : "BACKEND", "FRONTEND").
3. **Nom** : Nom du projet.
4. **Description** : Texte descriptif du projet.
5. **Origine** : Source du contenu du projet (ex : GitHub, API, Azure DevOps, etc.).
6. **URL d’origine** : Lien vers la source du projet (ex : URL du dépôt GitHub, de l’API, etc.).
7. **Identifiant externe (UUID)** : Référence à l’identifiant externe associé au projet (voir [gestion-des-connections-externes.md](gestion-des-connections-externes.md)).

### Propriétés d’un composant

Un composant possède les propriétés suivantes :

1. **Identifiant (UUID)** : Identifiant unique universel du composant.
2. **Nom** : Nom du composant.
3. **Code** : Code court ou nom technique du composant (ex : nom du dossier contenant les fichiers de traductions).

### Propriétés d’un besoin de traduction

Un besoin de traduction (par exemple : une langue ou une variante régionale à traduire pour une solution) possède les propriétés suivantes :

1. **Identifiant (UUID)** : Identifiant unique universel du besoin de traduction.
2. **Code** : Code normalisé (ex : "fr_FR", "en", "fr_BE").
3. **Libellé** : Libellé affiché à l’utilisateur (ex : "Français (France)", "Anglais").
4. **Défaut** : Booléen indiquant si ce besoin de traduction est la langue principale de la solution.
5. **Solution** : Référence à la solution concernée.

Chaque solution possède une liste de besoins de traduction, qui détermine les langues/variantes à gérer pour tous les fichiers de la solution. Un seul besoin de traduction peut être marqué comme défaut par solution.

## Interface de gestion des solutions, projets et composants

### Vue liste des solutions

- L’utilisateur visualise une liste de ses solutions (nom, code, description, propriétaire).
- Un clic sur une solution ouvre la page de détail correspondante.

### Détail d’une solution

- Affichage d’une arborescence interactive des projets et composants de la solution (navigation hiérarchique).
- Présentation d’un tableau de synthèse de la progression de traduction de tous les fichiers de la solution :
  - Une ligne par fichier de ressource
  - Une colonne par langue souhaitée
  - Affichage du taux de complétion ou d’un indicateur visuel (ex : barre de progression, couleur)
- Possibilité de filtrer ce tableau en cliquant sur un projet ou un composant dans l’arborescence :
  - Le tableau n’affiche alors que les fichiers liés à l’élément sélectionné

### Navigation

- Retour à la liste des solutions possible à tout moment
- L’URL reflète l’élément sélectionné (solution, projet, composant) pour permettre le partage de liens directs

