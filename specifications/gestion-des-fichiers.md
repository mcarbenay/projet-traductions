# Translation File Management

## Main Features

- Import and export resource files (common formats: .resx, .json, .po, etc.)
- View the structure and content of files
- Edit translations directly in the interface
- Track translation completion status by language
- Modification history and version management

## Business Rules

- Each resource file is associated with a project component
- Modifications are historized and traceable
- Files can be downloaded individually or in bulk

## Data Entities

### Resource File Properties

A resource file has the following properties:

1. **ID (UUID)**: Universally unique identifier of the file.
2. **Name**: File name (e.g., "messages.fr_FR.json").
3. **Path**: Relative or absolute path of the file in the project.
4. **Project**: Reference to the project it belongs to.
5. **Component**: Reference to the component it belongs to (if applicable).

Each file contains a set of translatable resources.

### Translatable Resource Properties

A translatable resource (an entry to be translated in a file) has the following properties:

1. **ID (UUID)**: Universally unique identifier of the resource.
2. **Key**: Resource key (e.g., "welcome.message").
3. **Source value**: Source text (e.g., in English or default language).
4. **Description**: Descriptive text or usage context of the resource (optional, e.g., "Message displayed on the home page").
5. **File**: Reference to the resource file it belongs to.

Each translatable resource can be translated into each of the languages/variants defined for the solution.

### Resource Translation Properties

A resource translation represents the translated version of a translatable resource in a specific translation need (language/variant). It has the following properties:

1. **ID (UUID)**: Universally unique identifier of the translation.
2. **Translatable resource**: Reference to the concerned translatable resource.
3. **Translation need**: Reference to the targeted translation need (language/variant).
4. **Validated value**: Validated translated text in the target language/variant.
5. **Suggested value**: Proposed or under-review/modified text.
6. **Status**: Indicates the translation status (e.g., to be translated, in progress, validated, suggestion pending).
7. **Last modified date**
8. **Modified by user**

For each translatable resource and translation need, there may be a validated version and a suggested version, facilitating iteration and review before final validation.
