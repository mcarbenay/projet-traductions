# Schéma des entités principales

> Convention : tous les noms d’entités, propriétés, relations, tables, colonnes, endpoints d’API, méthodes et objets métiers sont en anglais, conformément aux choix techniques du projet.

Ce diagramme présente la structure des entités principales du domaine et leurs relations.

```mermaid
classDiagram
    class Solution {
        id
        code
        name
        description
        presentationUrl
        owner
    }
    class Project {
        id
        code
        name
        description
        origin
        originUrl
        externalIdentifierId
    }
    class Component {
        id
        name
        code
    }
    class TranslationNeed {
        id
        code
        label
        isDefault
    }
    class ResourceFile {
        id
        name
        path
        project
        component
    }
    class TranslatableResource {
        id
        key
        sourceValue
        description
        resourceFile
    }
    class ResourceTranslation {
        id
        translatableResource
        translationNeed
        validatedValue
        suggestedValue
        status
        lastModifiedDate
        modifiedBy
    }
    class ExternalIdentifier {
        id
        sourceType
        fieldName
        value
        addedOrModifiedDate
        modifiedBy
    }
    class User {
        id
        name
        email
    }
    Solution "1" o-- "*" Project : contains
    Project "1" o-- "*" Component : contains
    Solution "1" o-- "*" TranslationNeed : translationNeeds
    Project "1" o-- "0..1" ExternalIdentifier : reference
    Solution "1" o-- "1" User : owner
    Component "1" o-- "*" ResourceFile : contains
    ResourceFile "1" o-- "*" TranslatableResource : contains
    TranslatableResource "1" o-- "*" ResourceTranslation : translations
    TranslationNeed "1" o-- "*" ResourceTranslation : target
```
