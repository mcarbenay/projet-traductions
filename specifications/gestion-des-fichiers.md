# Gestion des fichiers de traduction

## Fonctionnalités principales

- Importer et exporter des fichiers de ressources (formats courants : .resx, .json, .po, etc.)
- Visualiser la structure des fichiers et leur contenu
- Modifier les traductions directement dans l’interface
- Suivre l’état de complétion des traductions par langue
- Historique des modifications et gestion des versions

## Règles de gestion

- Chaque fichier de ressource est associé à un composant d’un projet
- Les modifications sont historisées et traçables
- Les fichiers peuvent être téléchargés individuellement ou en lot

## Entités de données

### Propriétés d’un fichier de ressources

Un fichier de ressources possède les propriétés suivantes :

1. **Identifiant (UUID)** : Identifiant unique universel du fichier.
2. **Nom** : Nom du fichier (ex : "messages.fr_FR.json").
3. **Chemin** : Chemin relatif ou absolu du fichier dans le projet.
4. **Projet** : Référence au projet auquel il appartient.
5. **Composant** : Référence au composant auquel il appartient (si applicable).

Chaque fichier contient un ensemble de ressources à traduire.

### Propriétés d’une ressource à traduire

Une ressource à traduire (entrée à traduire dans un fichier) possède les propriétés suivantes :

1. **Identifiant (UUID)** : Identifiant unique universel de la ressource.
2. **Clé** : Clé de la ressource (ex : "welcome.message").
3. **Valeur source** : Texte source (ex : en anglais ou langue par défaut).
4. **Description** : Texte descriptif ou contexte d’utilisation de la ressource (facultatif, ex : "Message affiché à l’accueil").
5. **Fichier** : Référence au fichier de ressources auquel elle appartient.

Chaque ressource à traduire pourra être traduite dans chacune des langues/variantes définies pour la solution.

### Propriétés d’une traduction de ressource

Une traduction de ressource représente la version traduite d’une ressource à traduire dans un besoin de traduction (langue/variante) spécifique. Elle possède les propriétés suivantes :

1. **Identifiant (UUID)** : Identifiant unique universel de la traduction.
2. **Ressource à traduire** : Référence à la ressource à traduire concernée.
3. **Besoin de traduction** : Référence au besoin de traduction (langue/variante) ciblé.
4. **Valeur traduite validée** : Texte traduit validé dans la langue/variante cible.
5. **Valeur traduite suggérée** : Texte proposé ou en cours de relecture/modification.
6. **Statut** : Indique l’état de la traduction (ex : à traduire, en cours, validée, suggestion en attente).
7. **Date de dernière modification**
8. **Utilisateur ayant modifié**

Pour chaque ressource à traduire et besoin de traduction, il peut donc exister une version validée et une version suggérée, facilitant l’itération et la relecture avant validation finale.
