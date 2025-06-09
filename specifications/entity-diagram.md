# Entity Diagram

## Main Entities

- Solution
- Project
- Component
- ResourceFile
- TranslatableResource
- ResourceTranslation
- TranslationNeed
- User
- ExternalIdentifier
- API Key

## Relationships

- A Solution contains multiple Projects
- A Project contains multiple Components
- A Component contains multiple ResourceFiles
- A ResourceFile contains multiple TranslatableResources
- A TranslatableResource can have multiple ResourceTranslations
- A TranslationNeed is linked to a Project/Component/ResourceFile
- A User can have different roles per Solution/Project
- An ExternalIdentifier can be linked to multiple Projects
- An API Key is associated with a User

## Diagram

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
