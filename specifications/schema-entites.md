# Main Entity Diagram

> Convention: All entity, property, relation, table, column, API endpoint, method, and business object names are in English, as per the project's technical choices.

This diagram shows the structure of the main domain entities and their relationships.

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
