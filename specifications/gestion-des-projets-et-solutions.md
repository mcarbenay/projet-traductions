# Project and Solution Management

## Main Features

- Create, edit, and delete solutions and projects
- Hierarchical organization: solution > project > component > resource file
- Global structure visualization
- Association of translation files to components

## Business Rules

- A project belongs to a single solution
- A component belongs to a single project
- A resource file belongs to a single component

## Data Entities

### Solution Properties

A solution has the following properties:

1. **ID (UUID)**: Universally unique identifier of the solution.
2. **Code**: Short unique code identifying the solution (e.g., "ERP", "MOBILE_APP").
3. **Name**: Unique name of the solution.
4. **Description**: Descriptive text of the solution.
5. **Presentation URL**: Link to a presentation or external documentation page.
6. **Owner**: User referenced as the owner of the solution.

### Project Properties

A project has the following properties:

1. **ID (UUID)**: Universally unique identifier of the project.
2. **Code**: Short unique code identifying the project (e.g., "BACKEND", "FRONTEND").
3. **Name**: Project name.
4. **Description**: Descriptive text of the project.
5. **Origin**: Source of the project content (e.g., GitHub, API, Azure DevOps, etc.).
6. **Origin URL**: Link to the project source (e.g., GitHub repository URL, API, etc.).
7. **External identifier (UUID)**: Reference to the external identifier associated with the project (see [external-connections-management.md](gestion-des-connections-externes.md)).

### Component Properties

A component has the following properties:

1. **ID (UUID)**: Universally unique identifier of the component.
2. **Name**: Component name.
3. **Code**: Short code or technical name of the component (e.g., folder name containing the translation files).

### Translation Need Properties

A translation need (e.g., a language or regional variant to be translated for a solution) has the following properties:

1. **ID (UUID)**: Universally unique identifier of the translation need.
2. **Code**: Normalized code (e.g., "fr_FR", "en", "fr_BE").
3. **Label**: Display label for the user (e.g., "French (France)", "English").
4. **Default**: Boolean indicating if this translation need is the main language of the solution.
5. **Solution**: Reference to the related solution.

Each solution has a list of translation needs, which determines the languages/variants to manage for all files in the solution. Only one translation need can be marked as default per solution.

## Solution, Project, and Component Management Interface

### Solution List View

- The user sees a list of their solutions (name, code, description, owner).
- Clicking a solution opens its detail page.

### Solution Detail

- Display of an interactive tree of the solution's projects and components (hierarchical navigation).
- Presentation of a summary table of the translation progress of all files in the solution:
  - One row per resource file
  - One column per desired language
  - Display of completion rate or a visual indicator (e.g., progress bar, color)
- Ability to filter this table by clicking a project or component in the tree:
  - The table then only shows files related to the selected item

### Navigation

- Return to the solution list possible at any time
- The URL reflects the selected item (solution, project, component) to allow sharing direct links

