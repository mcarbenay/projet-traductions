# File Management

## Main Features

- Upload, download, and versioning of translation files
- File format support (e.g., .resx, .json, .po, .xliff)
- File validation and integrity checks
- Association of files with projects, components, and languages
- Access control for file operations
- Files must NOT be stored on the server.
- When uploading a file, the backend must parse the file content to extract and deduce the "translation needs" (languages/variants present in the file).
- A common parser interface must be defined for all file formats. Each supported format (.resx, .json, .po, .xliff, etc.) must have its own implementation of this interface.
- The parser must extract all relevant translation keys, values, and language codes from the file.
- When a user wants to download a file, they must specify the desired format. Similarly to the parser interface for file formats, an interface must be defined for generating a file in a specific format. Each supported format must have its own implementation of this export interface.
- The API must provide an endpoint to list all supported file formats.
- The API must provide, for a given file, an endpoint to list all languages with at least one validated translation.

## Business Rules

- Only authorized users can upload or modify files
- File names and paths must be unique within a project/component
- File history and versioning must be maintained
- Deleted files are archived, not permanently removed
- No file content is persisted on the server after upload; only parsed data is stored in the database.
- If the file cannot be parsed or does not match the expected format, the upload must be rejected with a clear error message.
- The system must be easily extensible to support new file formats by implementing the parser interface and the export interface for each format.
