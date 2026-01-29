

# Avant-propos

Ce repo contient le code source et les ressources du projet Unity du jeu **ChromAdventure**.
Il s'agit d'un jeu d'aventure en 3D de type *Infinite Runner* cree dans le cadre du cours de Programmation de Jeux Vidéo dispensé par Gaming Campus.

Le jeu met en scène un personnage qui court à travers des environnements colorés et variés, collectant des objets et évitant des obstacles pour atteindre le score le plus élevé possible.

Le cahier des charges du projet est disponible ici : [GamingCampus-CDC-dev-jv.pdf](Documentation/GamingCampus-CDC-dev-jv.pdf).

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


Les assets du projets notament graphiques et sonores proviennent de ressources libres de droits :


- Musique - [licence](https://pixabay.com/service/license-summary/)
    - Musique du menu principal : ["Ambient game"](https://pixabay.com/sound-effects/musical-ambient-game-67014/) de JeltsinSH
    - Musique de fond du jeu : ["Game Roblox Gaming Background Music"](https://pixabay.com/music/bloopers-game-roblox-gaming-background-music-347589/) de Tunetank

- Effets sonores - [licence](https://pixabay.com/service/license-summary/)
    - [265 - fiezewarthog](https://pixabay.com/sound-effects/film-special-effects-265-41177/)
    - [Button Click - freesoundeffects](https://pixabay.com/sound-effects/film-special-effects-button-click-289742/)
    - [land2 - deleted_user_10023915](https://pixabay.com/sound-effects/film-special-effects-land2-43790/)
    - [Magazine Slide - MootMcnoodles](https://pixabay.com/sound-effects/film-special-effects-magazine-slide-100131/)
    - [Drop Coin - Crunchpix Studio](https://pixabay.com/sound-effects/film-special-effects-drop-coin-384921/)
    - [Thump - Macif](https://pixabay.com/sound-effects/household-thump-105302/)
    - [HealPop - shyguy014](https://pixabay.com/sound-effects/film-special-effects-healpop-46004/)

- Model 3D du personnage
    - [licence CC0 1.0 Universal (CC0 1.0) Public Domain Dedication](https://creativecommons.org/publicdomain/zero/1.0/)
    - [Kenney - Animated Characters 2](https://kenney.nl/assets/animated-characters-2)

- Splash screen du jeu

    - Genere par DALL·E 3 (https://chat.openai.com/)
    - [Usage commercial et non-commercial autorisé](https://openai.com/policies/terms-of-use/)




# Architecture generale du projet


## Environnement de developpement

Le projet a ete realise avec le moteur de jeu **Unity (version 6000.2.8f1)** et utilise le langage de programmation C#. Il a ete developpe sous Linux, en utilisant l'IDE **Visual Studio Code**. A noter que le projet cible deux plateformes de build : Linux et WebGL. Il prends donc en consideration les contraintes et les optimisations necessaires pour ces plateformes (WebGL en particulier).


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



## Procedure de Tests

Les tests unitaires et les tests d'integration n'ont pas ete mis en place dans ce projet en raison de contraintes de temps et de ressources. Cependant, des tests manuels ont ete effectues pour verifier le bon fonctionnement des principales fonctionnalites du jeu.

Tout au long du developpement:
- en utilisant la console de l'editeur Unity avec [`de nombreux Debug`](https://github.com/search?q=repo%3Akzlucas%2Finfinite-runner%20Debug&type=code) : `Debug.Log`, `Debug.DrawRay`, `Debug.Break`, `OnDrawGizmos`...
- en utilsant l'editeur Unity pour simuler differentes situations de jeu et verifier les comportements attendus.
- en jouant au jeu regulierement pour identifier et corriger les bugs et les problemes de gameplay.
- en faisant des builds du jeu pour tester les performances et la compatibilite sur differentes plateformes (Linux et WebGL).

![debug-1.png](Documentation/debug-1.png)

Le protoype a ete envoyes au groupe de travail sur Discord pour demander des retours et identifier d'eventuels bugs ou problemes de gameplay. Quelques bugs ont ete remontes et corriges avant la version finale du protoype.


#### Description de la procedure de test manuelle utilisee :

1. Demarrer le jeu
2. Verifier l'affichage du menu principal
3. Cliquer sur Start
4. Verifier le chargement de la scene de jeu
5. Verifier le systeme de generation procedurale du monde
6. Jouer au jeu
  - Verifier le deplacement du personnage
  - Verifier le saut et la glissade
  - Verifier la collecte des cristaux
  - Verifier les collisions avec les obstacles
  - Verifier la perte de points de vie
  - Verifier le systeme de rewind
  - Verifier l'affichage du score et des cristaux collectes
7. Perdre la partie
  - Verifier l'affichage de l'ecran de fin de partie
  - Verifier le calcul du score final
8. Redemarrer une nouvelle partie
  - Verifier le rechargement de la scene de jeu
  - Recommencer les tests de jeu (1 a 6)
9. Verifier les parametres audio
  - Muter et demuter le son
  - Verifier le volume sonore
10. Verifier la mise en pause du jeu
  - Verifier l'ouverture/fermeture du menu pause en appuyant sur Echap
  - Verifier l'ouverture/fermeture du menu pause en appuyant sur le bouton Pause
  - Reprendre le jeu
  - Verifier la reprise du jeu
11. Arreter le jeu
15. Verifier si le fichier local de sauvegarde a bien ete ecrit
12. Demarrer le jeu
15. Verifier la persistance des statistiques du joueur (meilleur score, cristaux collectes) apres redemarrage du jeu
13. Verifier la persistance des parametres audio dans le menu
14. Arreter le jeu
15. Verifier les erreur et warnings dans la console de l'editeur Unity et corriger si necessaire


#### Description de la procedure de test tu Tutorial utilisee :

1. Supprimer les PlayerPrefs dans l'editeur Unity
2. Supprimer le fichier de sauvegarde local (savefile.json) s'il existe (methode Editor `SaveService::DeleteSave`)
3. Demarrer le jeu
4. Verifier l'affichage du menu principal
5. Cliquer sur Start
6. Verifier le chargement de la scene de jeu
7. Jouer et verifier la completion du tutorial pas a pas :
  - Verifier l'affichage du message "Comment se Deplacer horizontalement"
  - Verifier l'affichage du message "Comment Sautez"
  - Verifier l'affichage du message "Comment Glissez"
  - Verifier l'affichage du message "Collectez des Cristaux"
  - Verifier l'affichage du message "Tutoriel complete"
8. Arreter le jeu
9. Demarrer le jeu
10. Verifier que le tutorial ne se lance pas a nouveau

#### Procedure de test des mondes

A noter que pour chaque nouveau segment de monde ajoutes au cours du developpement, une procedure de test manuelle a ete realisee pour verifier que le segment s'integre correctement dans le systeme de generation procedurale du monde mais aussi qu'il ne provoque pas de problemes de collisions ou de rebondissements indésirables, que le joueur peut bien le franchir, que les obstacles sont bien placés, etc.

## Suivi des features

Tableau de suivi des principales features developpes au cours du projet :
[Google Sheet Document](https://docs.google.com/spreadsheets/d/1VynXDeEw_dpZwPe93qpfKKuDYcDk6X5uEqaUhXpRSZ0/edit?gid=0#gid=0)


## Commentaire de code et formatage

Les methodes et les classes sont commentees a l'aide de commentaires XML pour faciliter la comprehension du code et la generation de documentation automatique.

A noter que les commentaires XML sont utilises principalement pour documenter les classes et les methodes publiques, tandis que les commentaires en ligne (// ou /* */) sont utilises pour expliquer des sections de code plus complexes ou des logiques specifiques.

Le code source du projet suit en partie les conventions de nommage et de formatage standard de C#. Les classes, methodes et variables sont nommees de maniere descriptive pour faciliter la comprehension du code.

Ici les conventions de nommage et de formatage utilisees dans ce projet qui ont ete respectees :

- ✅ PascalCase (UpperCamelCase) : Classes, méthodes, propriétés, espaces de noms (namespaces), interfaces (IInterface)
- ✅ camelCase (LowerCamelCase) : Variables locales, paramètres de méthode
- ❌ Champ privé (private fields) : _camelCase (underscore + camelCase).
- ❌ Constantes : PascalCase ou UPPER_CASE
- ✅ Interfaces : Commencent par une majuscule 'I'.
- ✅ Booléens : Préfixer par Is, Can, Has. 
- ⚠️ Accolades : Utiliser le style Allman (accolades ouvrant et fermant sur une nouvelle ligne). --> Sur ce point, j'ai assez souvent omis les accolades lorsqu'il n'y avait qu'une seule instruction dans un bloc conditionnel ou de boucle. Pour le reste le style Allman a ete respecté.
- ✅ Indentation : 4 espaces (ne pas utiliser de tabulations).
- ✅ var keyword : Utiliser var lorsque le type est évident à droite de l'assignation, sinon préciser le type.
- ⚠️ Nommage des fichiers : Faire correspondre le nom de la classe au nom du fichier (ex: Class1.cs). --> Sur ce point, j'ai parfois plusieurs classes dans un meme fichier lorsque ces classes sont petites et fortement liées entre elles.
- ✅ Commentaires : Utiliser // pour les commentaires sur une seule ligne. 

## Input System

La classe [`InputHandlersManager`](Assets/Scripts/Inputs/InputHandlersManager.cs) est responsable de la gestion des entrees utilisateur. Elle utilise le systeme d'Input de Unity pour detecter les actions de l'utilisateur et declencher les evenements appropries.

Les composants du projet peuvent utiliser cette classe pour mapper un input a une fonction en utilisant la methode `RegisterInputHandler`. Chaque handler est associe a une action specifique et peut definir des callbacks pour les evenements de pression (`OnInput`), de relachement (`OnRelease`) et de maintien d'un input (`OnHold`).

*[Assets/Scripts/Player/PlayerController.cs](Assets/Scripts/Player/PlayerController.cs)*
```csharp 
    InputHandlersManager.Instance.Register(
        label: "Jump", 
        actionRef: jumpActionRef, 
        OnTrigger: OnJumpInputPressed
    );
```


## Singleton

Le pattern de conception [`Singleton<T>`](Assets/Scripts/Singleton.cs) a ete utilise dans plusieurs classes du projet pour garantir qu'une seule instance de ces classes existe a tout moment pendant l'execution du jeu.


## Interfaces

Quelques Interfaces ont ete utilisees pour definir des contrats entre les differentes classes du projet. Cela permet de decoupler les composants et de faciliter la maintenance du code.

Elles sont disponibles dans le chemin `Assets/Scripts/Interfaces/`.

## Data

Deux services de sauvegarde de donnees ont ete implementes dans le projet :

- [PlayerPrefService](Assets/Scripts/DataServices/PlayerPrefService.cs) : Permet de sauvegarder et de charger des donnees locales en utilisant PlayerPrefs de Unity. Utilise pour sauvegarder les parametres du jeu (audio mute notamment).

- [SaveService](Assets/Scripts/DataServices/SaveService.cs) : Permet de sauvegarder des donnees plus complexes en utilisant la serialisation JSON. Utilise pour sauvegarder les statistiques du joueur (meilleur score, cristaux collectés, etc.).



## Audio Manager

La gestion de l'audio dans le jeu est realisee a l'aide de la classe [`AudioManager`](Assets/Scripts/SceneCore/AudioManager.cs). Cette classe est responsable de la lecture des effets sonores et de la musique de fond dans le jeu.

*eg*
```csharp 
AudioManager.Instance.PlaySound("crash");
```

L'AudioManager utilise un dictionnaire pour stocker les clips audio et permet de jouer des sons en utilisant leur nom (`string`). Il prend en charge la lecture de sons uniques ainsi que la lecture en boucle pour la musique de fond.


## Tutorial

Un [`TutorialManager`](Assets/Scripts/Tutorials/TutorialManager.cs) a ete implemente pour guider les nouveaux joueurs a travers les mecanismes de base du jeu. Il affiche des messages contextuels a l'ecran pour expliquer les controles et les objectifs du jeu.

## UI Toolkit

Toutes les interfaces utilisateur du jeu sont construites en utilisant le systeme UI Toolkit de Unity.

Tous les `GameObject`s qui contiennent le composant [`UnityEngine.UIElementsModule.UIDocument`](https://docs.unity3d.com/2021.3/Documentation/ScriptReference/UIElements.UIDocument.html) herite de la classe [`UIController`](Assets/Scripts/UI/BaseClasses/UiController.cs) qui fournit des methodes de base pour gerer l'affichage et la mise a jour des elements UI. 

Toutes les fonctionnalites UI specifiques sont implementees dans des classes derivees de `UIController`, telles que :

- [`UiSplashScreen`](Assets/Scripts/UI/UiSplashScreen.cs)
- [`UiPauseMenu`](Assets/Scripts/UI/UiPauseMenu.cs)
- [`UiEndGame`](Assets/Scripts/UI/UiEndGame.cs)
- [`UiHud`](Assets/Scripts/UI/UiHud.cs)
- ...


## Description des Unity tags utilises dans le jeu

- `World Segment` : Utilise pour identifier les segments de monde generes proceduralement.
- `Composite Square Collider` : Utilise pour identifier les colliders composites utiliser par le systeme de fusion des colliders.
- `Slot` : Identifie les emplacements disponible pour les obstacles lors de la generation du monde.
- `Crystal` : Identifie les cristaux a collecter dans le jeu.


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

Les donnees sur le Monde sont stockees dans des Scriptable Objects ([`SO_BiomeData`](Assets/Scripts/WorldGeneration/SO_BiomeData.cs)), les donnees sur les Segments sont stockes dans des classes [`WorldSegment`](Assets/Scripts/WorldGeneration/WorldSegment.cs) ce qui permet de faciliter la configuration et la modification des segments de monde sans avoir a modifier le code.



## Rewind system

Lorsque le joueur percute un obstacle, il perd un point de vie et est renvoyé a une position precedente grace au systeme de rewind. Ce systeme permet de repositionner le joueur a un point de controle anterieur, lui permettant ainsi de continuer sa progression sans repartir du debut.

La classe [`PlayerHistory`](Assets/Scripts/Player/PlayerHistory.cs) est responsable du stockage et de la restauration de l'historique des positions du joueur. Elle enregistre periodiquement la position et la vitesse du joueur dans une liste de records (`PlayerHistoryRecord`). Lorsqu'un rewind est necessaire, elle restaure la derniere position en douceur en deplacant le joueur vers cette position en utilisant une interpolation.

Les `PlayerHistoryRecord` (*Checkpoint*) sont enregistres seulement si le joueur a avance d'une certaine distance depuis le dernier enregistrement, afin d'optimiser l'utilisation de la memoire, mais aussi ils doivent etre des positions qui permettent au joueur de repartir sans risquer d'etre en collision avec un obstacle dans les prochaines frames. C'est le role de la methode `PlayerHistory::IsSafeZoneToRespawn`.

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
