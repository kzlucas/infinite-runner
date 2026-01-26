# Avant-propos

Ce repo contient le code source et les ressources du projet Unity du jeu "ChromAdventure".
Il s'agit d'un jeu d'aventure en 3D de type "Infinite Runner" cree dans le cadre du cours de Programmation de Jeux Vidéo dispensé par Gaming Campus.

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

Le projet a ete realise avec le moteur de jeu Unity (version 6000.2.8f1) et utilise le langage de programmation C#.

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

L'initialisation de la scene se fait grace aux scripts "SceneLoader" et "SceneInitializer" attaché à un GameObject "Game Core" present dans chaque scene du jeu.

Le "SceneLoader" est responsable du chargement des scenes et de la gestion des transitions entre celle ci. Deux evenements sont declenchés par ce script : "OnSceneLoaded" et "OnSceneExit".

Le "SceneInitializer" ecoute l'evenement "OnSceneLoaded" et initialise les differents elements de la scene en fonction de celle ci.

![Alt text for the image](Documentation/schema-scene-init.png)

