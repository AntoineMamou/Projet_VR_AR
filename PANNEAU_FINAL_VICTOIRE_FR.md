# Panneau final de victoire

## Vue d'ensemble

Dans l'etat actuel du projet, le panneau final apparait quand le joueur atteint le `GoalTrigger` dans la scene `Assets/Scenes/Level01_Prototype.unity`.

La chaine de fonctionnement est la suivante :

1. `PlayerTrigger` represente le volume de detection du joueur.
2. `PlayerTriggerFollower` maintient ce volume au niveau de la camera active (`Camera.main`).
3. `GoalTrigger` detecte l'entree du `PlayerTrigger` via `LevelGoalTrigger`.
4. `LevelGoalTrigger` appelle `LevelRunStats.Instance.CompleteLevel()`.
5. `LevelRunStats` envoie l'evenement `LevelCompleted`.
6. `VictoryPanelController` ecoute cet evenement, met a jour les textes, calcule les etoiles, place le canvas devant la camera et affiche le panneau.

## Comment le panneau est declenche

### 1. Detection du joueur

- Objet joueur utilise pour la detection : `XR Origin (XR Rig) -> PlayerTrigger`
- Script implique : `Assets/Scripts/PlayerTriggerFollower.cs`
- Tag requis : `Player`

`PlayerTrigger` n'est pas le panneau et n'est pas la camera elle-meme. C'est un collider dedie a la logique de jeu. Le script `PlayerTriggerFollower` le fait suivre la camera active pour que le systeme XR/VR puisse declencher correctement les zones de trigger.

### 2. Detection de l'arrivee

- Objet d'arrivee : `GoalTrigger`
- Script implique : `Assets/Scripts/LevelGoalTrigger.cs`

Conditions actuelles :

- `GoalTrigger` doit avoir un collider en mode `Is Trigger`
- `LevelGoalTrigger` doit etre actif
- Le tag requis doit etre `Player`
- Le rigidbody du `GoalTrigger` ne doit pas etre dynamique avec gravite

Quand `PlayerTrigger` entre dans `GoalTrigger`, `LevelGoalTrigger` complete le niveau.

### 3. Collecte des statistiques

- Script implique : `Assets/Scripts/LevelRunStats.cs`

`LevelRunStats` stocke :

- le temps ecoule
- le nombre de cubes utilises
- le nombre de buzzers/collectes touches

Au moment de la completion, il envoie un `LevelResultData` au panneau final.

## Comment le panneau est affiche

### 1. Canvas et panneau

- Canvas : `XR Origin (XR Rig) -> Main Camera -> VictoryCanvas`
- Panneau : `VictoryCanvas -> VictoryPanel`
- Script implique : `Assets/Scripts/VictoryPanelController.cs`

Le `VictoryCanvas` est un canvas en `World Space`, parent de la camera principale. Au moment de la fin de niveau, `VictoryPanelController` :

1. formate les textes du panneau
2. calcule le nombre d'etoiles
3. repositionne le canvas devant la camera
4. rend le panneau visible
5. met `Time.timeScale = 0`

### 2. Textes affiches

Le panneau affiche maintenant les libelles complets :

- `TEMPS : mm:ss`
- `CUBES UTILISES : x`
- `BUZZERS TOUCHES : y`

Ces formats sont portes par `VictoryPanelController` et aussi serialises dans la scene.

### 3. Logique actuelle des etoiles

La logique actuelle est la suivante :

- `0` etoile si aucun buzzer/collecte n'a ete touche
- `1` etoile si au moins un buzzer/collecte a ete touche
- `2` etoiles si le nombre de buzzers touches atteint le seuil requis
- `3` etoiles si le seuil de buzzers est atteint et que les contraintes de temps et de cubes utilises sont respectees

Parametres actuels dans `VictoryPanelController` :

- seuil 3 etoiles : `45s` et `3` cubes max
- seuil 2 etoiles : `90s` et `6` cubes max
- buzzers requis : `3`

Ordre d'affichage actuel des etoiles : gauche -> centre -> droite.

## Migration vers une autre scene

## Objectif

Reproduire exactement le meme comportement dans une autre scene, sans oublier les references manuelles dans l'Inspector.

### Etape 1. Ajouter les scripts necessaires

Verifier que la nouvelle scene utilise ces scripts :

- `Assets/Scripts/LevelRunStats.cs`
- `Assets/Scripts/LevelGoalTrigger.cs`
- `Assets/Scripts/VictoryPanelController.cs`
- `Assets/Scripts/PlayerTriggerFollower.cs`
- `Assets/Scripts/BuzzerTrigger.cs` si vous voulez conserver le systeme de collectes/buzzers

### Etape 2. Ajouter un `LevelRunStats`

Creer un GameObject vide dans la scene, par exemple `LevelRunStats`, puis ajouter le composant `LevelRunStats`.

Sans cet objet, le panneau de fin ne recevra aucun evenement de completion.

### Etape 3. Ajouter ou recopier `PlayerTrigger`

Creer un objet `PlayerTrigger` avec :

- un tag `Player`
- un collider pour la detection
- un `Rigidbody` kinematic
- le script `PlayerTriggerFollower`

Configuration recommandee :

- `Use Gravity = false`
- `Is Kinematic = true`
- `_target` peut rester vide, le script utilisera `Camera.main`
- conserver un offset vertical raisonnable dans `PlayerTriggerFollower`

### Etape 4. Ajouter ou recopier `GoalTrigger`

Creer un objet `GoalTrigger` avec :

- un collider en `Is Trigger`
- le script `LevelGoalTrigger`

Verifier :

- `LevelGoalTrigger` actif
- `_requiredTag = Player`
- si un `Rigidbody` est present, il ne doit pas tomber

### Etape 5. Ajouter ou recopier le panneau

Le plus simple est de recopier depuis `Level01_Prototype` :

- `VictoryCanvas`
- `VictoryPanel`
- les trois etoiles
- les textes
- le bouton `Restart`

Le `VictoryCanvas` doit etre place sous la camera active de la scene.

### Etape 6. Rebrancher `VictoryPanelController`

Dans le composant `VictoryPanelController`, verifier les references suivantes :

- `_panelRoot`
- `_timeValueText`
- `_cubesValueText`
- `_buzzersValueText`
- `_stars`
- `_filledStarSprite`
- `_emptyStarSprite`

Verifier aussi les parametres :

- `_distanceFromCamera`
- `_threeStarTimeLimit`
- `_twoStarTimeLimit`
- `_threeStarCubeLimit`
- `_twoStarCubeLimit`
- `_requiredBuzzersTouched`

### Etape 7. Rebrancher le bouton Restart

Le bouton `Restart` doit appeler :

- `VictoryPanelController.RestartLevel()`

Sans cela, le bouton apparaitra mais ne rechargera pas la scene.

### Etape 8. Rebrancher les collectes optionnelles

Si vous voulez conserver la logique des buzzers/collectes :

- ajouter `BuzzerTrigger` sur les objets a collecter/toucher
- verifier `_requiredTag = Player`

Si vous voulez compter les cubes utilises :

- conserver l'appel a `LevelRunStats.Instance.RegisterCubeUsed()` dans votre logique de spawn

## Check-list de verification

Avant de considerer la migration comme terminee, verifier :

- `PlayerTrigger` suit bien la camera
- `GoalTrigger` se declenche bien avec le tag `Player`
- `LevelRunStats` existe dans la scene
- le panneau s'affiche devant la camera
- les textes affichent les libelles complets
- les etoiles s'allument dans le bon ordre
- le bouton `Restart` recharge bien la scene

## Pieges frequents

### 1. Le panneau ne s'affiche pas

Ca arrive souvent si :

- `LevelRunStats` est absent
- `LevelGoalTrigger` est desactive
- `PlayerTrigger` n'a pas le bon tag
- `GoalTrigger` n'est pas en trigger

### 2. Le panneau s'affiche mais le texte semble casse

Ca arrive si :

- les references TMP ne pointent pas vers les bons objets
- le script remplace le texte complet par une simple valeur
- les formats de texte ne sont pas renseignes

### 3. Le panneau s'affiche mais les etoiles sont fausses

Ca arrive si :

- la logique de score ne correspond pas au game design voulu
- le compteur `BuzzersTouched` n'est jamais incremente
- les etoiles sont branchees dans le mauvais ordre

### 4. Le joueur "atteint le goal" mais rien ne se passe

Ca arrive si :

- seule la camera bouge mais pas le volume logique du joueur
- `PlayerTriggerFollower` ne suit pas la bonne camera
- `GoalTrigger` est mal place
- `GoalTrigger` tombe a cause d'un rigidbody mal configure

## Remarque importante sur le design actuel

Dans la scene actuelle, le compteur d'etoiles est base sur `BuzzersTouched`, c'est-a-dire les objets de type collecte/buzzer touches par le joueur, et non sur un systeme de "grab" XR pur.

Si vous voulez, dans une autre scene, que les etoiles dependent d'une vraie action de prise en main (`grab`), il faudra ajouter une statistique dediee et la brancher dans `LevelRunStats` et `VictoryPanelController`.
