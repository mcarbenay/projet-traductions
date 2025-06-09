# Projet de gestion des traductions

Tu vas rédiger les spécifications fonctionnelles et techniques pour un projet de gestion des fichiers de traductions. Puis tu vas l'implémenter.

## Spécifications fonctionnelles

Les spécifications vont dans le dosser "specifications", elles doivent être rédigées en anglais (!IMPORTANT) et mis dans des fichiers markdown.
Créé une structure simple :
- un fichier pour les informations générales du projet (index.md)
- un fichier par grand domaine fonctionnel (par exemple : "gestion-des-fichiers.md", "gestion-des-utilisateurs.md", etc.)

## Implémentation

L'implémentation se fera dans le dossier "src". 
Tu sépareras le backend et le frontend dans des sous-dossiers "backend" et "frontend".
Le backend sera en .net 8.0 
Le frontend en Vue.js 3.0. tu utiliseras le framework Vuetify pour l'interface utilisateur. Tu pourras utiliser Pinia, VueRouter et UseVue.

## demande de modification

Une fois que l'implémentation aura été faite, il faudra que tu conserve, dans un dossier "modifications", un fichier avec toutes les modifications que tu auras apportées.
Ce fichier devra être au format markdown et contiendra :
- la description de tous les éléments à modifier
- les fonctionnalités à ajouter ou modifier
- les fichiers ou parties de code à modifier
- les tests à ajouter ou modifier
Ce fichier devra être mis à jour à chaque fois que tu feras une modification dans les spécifications et servira à savoir ce qu'il faut modifier dans le code.
Tu nommeras ce fichier "todo.md"
.
Lorsque je te demanderai d'appliquer les modifications, tu devras :
- lire le fichier de modifications "todo.md"
- effectuer les modifications dans l'implémentation
- une fois les modifications effectuées, compiler le projet et vérifier que les tests passent
- renommer le fichier "todo.md" en "readyforreview.md"

pour savoir si l'implémentation a été déjà faite, tu pourras vérifier si le dossier "src" contient déjà des sous-dossiers "backend" et "frontend" et que ceux-ci contiennent des fichiers. 
Si c'est le cas, tu n'auras pas à refaire l'implémentation, mais seulement à appliquer les modifications.