# Avant-propos

Ce repo contient le code source et les ressources du projet Unity du jeu **ChromAdventure**.
Il s'agit d'un jeu d'aventure en 3D de type *Infinite Runner* cree dans le cadre du cours de Programmation de Jeux Vidéo dispensé par Gaming Campus.

Le jeu met en scène un personnage principal qui court à travers des environnements colorés et variés, collectant des objets et évitant des obstacles pour atteindre le score le plus élevé possible.

Le jeu est cree sur la base du Game Design Document (GDD) suivant : [ChromAdventure_Game-Design-Document.pdf](Documentation/ChromAdventure_Game-Design-Document.pdf).

Les personnes ayant participé à la création de ce GDD sont :

- Alex Gonzalez 
- Lucas Mortier 
- Lucas Tesseron
- Mina Ineflas 
- Nathan Letessier
- Rudy Luna 
- Samuel Zerbib 
- Sébastien Frayssinet
- Thomas Menant 
- Vincent Distribué

Les comptes rendus des réunions de creation du GDD sont disponibles ici [ChromAdventure_Game-Design-Document_Comptes-rendu-reu.pdf](Documentation/ChromAdventure_Game-Design-Document_Comptes-rendu-reu.pdf).

Le developpement de ce protoype du jeu a demarre mi decembre 2025 et s'acheve debut fevrier 2026.Il est realise par la personne suivante :

- Lucas Tesseron ([@kzlucas](https://github.com/kzlucas))

Le projet a ete realise avec le moteur de jeu **Unity (version 6000.2.8f1)** et utilise le langage de programmation C#.

Les assets du projets notament graphiques et sonores proviennent de ressources libres de droits :


- Musique - [licence](https://pixabay.com/service/license-summary/)
    - Musique du menu principal : ["Ambient game"](https://pixabay.com/sound-effects/musical-ambient-game-67014/) de JeltsinSH
    - Musique de fond du jeu : ["Game Roblox Gaming Background Music"](https://pixabay.com/music/bloopers-game-roblox-gaming-background-music-347589/) de Tunetank

- Effets sonores - [licence](https://pixabay.com/service/license-summary/)
    - [265 - fiezewarthog](https://pixabay.com/sound-effects/film-special-effects-265-41177/)
    - [Button Click - freesoundeffects](https://pixabay.com/sound-effects/film-special-effects-button-click-289742/)
    - [land2 - deleted_user_10023915](https://pixabay.com/sound-effects/film-special-effects-land2-43790/)
    - [Magazine Slide - MootMcnoodles](https://pixabay.com/sound-effects/film-special-effects-magazine-slide-100131/)
    - [Retro coin 4 - Driken5482](https://pixabay.com/sound-effects/film-special-effects-retro-coin-4-236671/)
    - [Thump - Macif](https://pixabay.com/sound-effects/household-thump-105302/)

- Model 3D du personnage
    - [licence CC0 1.0 Universal (CC0 1.0) Public Domain Dedication](https://creativecommons.org/publicdomain/zero/1.0/)
    - [Kenney - Animated Characters 2](https://kenney.nl/assets/animated-characters-2)

- Splash screen du jeu

    - Genere par DALL·E 3 (https://chat.openai.com/)
    - [Usage commercial et non-commercial autorisé](https://openai.com/policies/terms-of-use/)


## Structure du Repo

- `Documentation/` : Contient la documentation du projet, y compris le GDD et d'autres ressources pertinentes.

- `Assets/` : Contient tous les assets du projet Unity, y compris les scripts, les modèles 3D, les textures, les sons, etc.

    - `Assets/Animations/` : Contient les animations utilisées pour les personnages et les objets du jeu.
    - `Assets/Audio/` : Contient les fichiers audio utilisés dans le jeu.
    - `Assets/Graphics/` : Contient les éléments graphiques tels que les sprites et les UI.
    - `Assets/Materials/` : Contient les matériaux utilisés pour les modèles 3D.
    - `Assets/Models/` : Contient les modèles 3D utilisés dans le jeu.
    - `Assets/Prefabs/` : Contient les prefabs utilisés pour instancier des objets dans les scènes.
    - `Assets/Resources/` : Contient les ressources chargées dynamiquement pendant l'exécution du jeu.
    - `Assets/Scenes/` : Contient les différentes scènes du jeu.
    - `Assets/Scripts/` : Contient tous les scripts C# utilisés dans le projet.
    - `Assets/Settings/` : Contient les fichiers de configuration et les paramètres du projet Unity.
    - `Assets/Shaders/` : Contient les shaders utilisés pour les effets visuels dans le jeu.
    - `Assets/Terrains/` : Contient les terrains utilisés pour créer les environnements du jeu.
    - `Assets/UI Toolkit/` : Contient les éléments de l'interface utilisateur du jeu.


# Documentation technique sur l'architecture du code


## Initialisation de la scene

L'initialisation de la scene se fait grace aux scripts [`SceneLoader`](Assets/Scripts/SceneCore/SceneLoader.cs) et [`SceneInitializer`](Assets/Scripts/SceneCore/SceneInitializer.cs) attaché à un GameObject [Game Core](Assets/Prefabs/Game%20Core.prefab) present dans chaque scene du jeu.

Le "SceneLoader" est responsable du chargement des scenes et de la gestion des transitions entre celle ci. Deux evenements sont declenchés par ce script : "OnSceneLoaded" et "OnSceneExit".

Le "SceneInitializer" ecoute l'evenement "OnSceneLoaded" et initialise les differents elements de la scene en fonction de celle ci.

![Schema Scene Init](Documentation/schema-scene-init.png)



## Choix du moteur physique

Le choix du moteur physique pour ce projet s'est porté sur l'utilisation du moteur physique 3D de Unity  notament pour permettre de gerer les sauts du personnage sur des plateformes 3D. Le gameplay se deroule sur les 3 axes (X, Y et Z) et le personnage peut sauter et atterrir sur des plateformes de differentes hauteurs.

## Gestion des colliders

Un systeme de gestion des colliders a ete mis en place pour resoudre un probleme de rebondissements sur les bords des colliders lorsque ceux ci sont disposes cote a cote. Par nature, les elemnent du monde sont une suite de segments de monde ([`WorldSegment`](Assets/Scripts/WorldGeneration/WorldSegment.cs)) qui sont assemblés les uns aux autres pour former le monde infini. Lorsque deux segments de monde sont assembles, leurs colliders respectifs sont egalement mis cote a cote et cela provoque des problemes de rebondissements pour le joueur lorsqu'il passe d'un segment a un autre. Le Rigidbody du joueur rebondit legerement lorsqu'il touche la jonction entre deux colliders, ce qui peut perturbe le gameplay.

Pour resoudre ce probleme, un systeme de `Composite Square Colliders` a ete implemente. Ce systeme permet de combiner plusieurs colliders en un seul collider plus grand, eliminant ainsi les jonctions entre les colliders individuels et evitant les rebondissements indésirables.

La classe [`SquareCollidersMerger`](Assets/Scripts/Helpers/SquareCollidersMerger.cs) est responsable de la fusion des colliders carrés. Elle prend en entree une liste de colliders individuels et les combine en un seul collider composite. Ce collider composite est ensuite utilise pour gerer les collisions avec le joueur, assurant ainsi une experience de jeu fluide et sans rebondissements.

A noter que cette classe prend en charge uniquement les colliders de forme carrée, ce qui est suffisant pour les besoins de ce projet.

### Sans la fusion des colliders
![colliders-management-2.png](Documentation/colliders-management-2.png)

### Avec la fusion des colliders
![colliders-management-1.png](Documentation/colliders-management-1.png)



## Generation du monde

La generation du monde est realisee de maniere procedurale a l'aide de segments de monde reutilisables. Chaque segment de monde est un prefab qui contient des elements de decor, des obstacles et des objets a collecter.

Les prefabs sont stockes dans le dossier `Assets/Prefabs` et sont charges dynamiquement pendant l'execution du jeu.

La classe [`WorldGenerationManager`](Assets/Scripts/WorldGeneration/WorldGenerationManager.cs) est responsable de la generation du monde. Elle instancie les segments de monde a mesure que le joueur avance (`WorldGenerationManager::GenerationRoutine`). Elle supprime egalement les segments de monde qui sont hors de la vue du joueur, et qui ne sont plus necessaires, afin d'optimiser les performances du jeu (`WorldGenerationManager::ClearSegmentsBehindPlayer`). Si l'utilisateur percute un obstacle, il est envoyé a une position precedente grace au systeme de rewind (voir ci-dessous). Cette classe prend cela en compte dans sa logique de conservation des segments de monde.

Elle permet egalement de regenerer le monde lorsque le joueur a collecté suffisamment de cristaux pour atteindre un nouveau palier.

Les donnees sur le Monde et les Segments de Monde sont stockees dans des Scriptable Objects ([`SO_BiomeData`](Assets/Scripts/WorldGeneration/SO_BiomeData.cs), [`WorldSegment`](Assets/Scripts/WorldGeneration/WorldSegment.cs)), ce qui permet de faciliter la configuration et la modification des segments de monde sans avoir a modifier le code.

## Rewind system

Lorsque le joueur percute un obstacle, il perd un point de vie et est renvoyé a une position precedente grace au systeme de rewind. Ce systeme permet de repositionner le joueur a un point de controle anterieur, lui permettant ainsi de continuer sa progression sans repartir du debut.

La classe [`PlayerHistory`](Assets/Scripts/Player/PlayerHistory.cs) est responsable du stockage et de la restauration de l'historique des positions du joueur. Elle enregistre periodiquement la position et la vitesse du joueur dans une liste de records (`PlayerHistoryRecord`). Lorsqu'un rewind est necessaire, elle restaure la derniere position en douceur en deplacant le joueur vers cette position en utilisant une interpolation.

Les `PlayerHistoryRecord` sont enregistres seulement si le joueur a avance d'une certaine distance depuis le dernier enregistrement, afin d'optimiser l'utilisation de la memoire, mais aussi ils doivent etre des positions qui permettent au joueur de repartir sans risquer d'etre en collision avec un obstacle dans les prochaines frames. C'est le role de la methode `PlayerHistory::IsSafeZoneToRespawn`.

Cette methode va verifier que le joueur n'est pas entrain de sauter, de glisser ou de changer de voie mais aussi que la position actuelle du joueur est suffisamment eloignee des obstacles presents dans un espace defini autour de cette position. Si des obstacles sont detectes dans cet espace, la position n'est pas consideree comme une zone sure pour le respawn, et le record n'est pas enregistre. Elle verifie egalement que le sol continue sous les pieds du joueur pour eviter de respawner au bord d'un precipice.

### Les Rays envoyés pour detecter les obstacles autour de la position du joueur
Ici la position semble sur pour un respawn car aucun obstacle n'est detecté dans la zone definie.
![safe-zone-ray.png](Documentation/safe-zone-ray.png)



## Player Component

### Controller

Le joueur est representé par la classe [`PlayerController`](Assets/Scripts/Player/PlayerController.cs) qui gere les differentes actions du joueur telles que le deplacement, le saut, la glissade et la collecte d'objets.

Ce controlleur utilise un systeme d'etats ([**State Machine**](Assets/Scripts/StatesMachine/StateMachine.cs)) pour gerer les differentes actions du joueur. Chaque etat est represente par une classe derivee de la classe abstraite [`PlayerState`](Assets/Scripts/Player/States/PlayerState.cs). Les etats actuels sont :

- [`MoveState`](Assets/Scripts/Player/States/MoveState.cs) : Gere le deplacement lateral du joueur.
- [`CrashState`](Assets/Scripts/Player/States/CrashState.cs) : Gere l'etat de collision du joueur avec un obstacle.
- [`JumpState`](Assets/Scripts/Player/States/JumpState.cs) : Gere le saut du joueur.
- [`SlideState`](Assets/Scripts/Player/States/SlideState.cs) : Gere la glissade du joueur.
- [`IdleState`](Assets/Scripts/Player/States/IdleState.cs) : Gere l'etat d'attente du joueur.
- [`LandState`](Assets/Scripts/Player/States/LandState.cs) : Gere l'atterrissage du joueur apres un saut.
- [`RewindState`](Assets/Scripts/Player/States/RewindState.cs) : Gere le rewind du joueur apres une collision.


### Collision Handler

La classe [`PlayerCollisionHandling`](Assets/Scripts/Player/PlayerCollisionHandling.cs) est responsable de la gestion des collisions du joueur avec les obstacles et les objets a collecter. Elle detecte les collisions et declenche les actions appropriees en fonction du type d'objet touche.

A noter que le Player comporte 4 differents colliders pour detecter les collisions sur differentes parties du corps du joueur (corps, cote gauche, cote droit, avant). 

*[Assets/Scripts/Player/PlayerCollider.cs](Assets/Scripts/Player/PlayerCollider.cs)*
```csharp 
public enum ColliderPosition
{
    Body,
    Left,
    Right,
    Front,
}
```
Cela permet de differencier la collision du corps principal du joueur (Body) et les collisions sur les cotes (Left, Right) ou l'avant (Front) du joueur. Une collision avec un obstacle detectee par le collider avant (Front)  declenche le crash du joueur, tandis qu'une collision detectee par les collider Body peut vouloir simplement dire que le joueur a heurté le collider du sol lorsqu'il a atterri apres un saut.



### PlayerHealth



## Input System
## Architecture generale du projet
### Data et stats
### Audio Manager
### UI Toolkit
### Commentaire de code et formatage
## Description des tags

- World Segment
- Composite Square Collider
- Slot
- Crystal

