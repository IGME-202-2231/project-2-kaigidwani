# Project _Terrible Aquarium_

[Markdown Cheatsheet](https://github.com/adam-p/markdown-here/wiki/Markdown-Here-Cheatsheet)

### Student Info

-   Name: _Kai Gidwani_
-   Section: _02_

## Simulation Design

My simulation is going to have a school of small fish flocking together and swordfish that are trying to hunt them. Players can click to spawn food for the small fish that will attract them to that point.

### Controls

-   Click on a spot to spawn fish food for the small fish to eat.

## _Small fish_

Small fish flock together and will try to stay together and avoid swordfish. They will also be attracted to fish food and try to eat it.

### _Schooling_

**Objective:** _School with all the other fish to avoid getting singled out by the swordfish._

#### Steering Behaviors

- Flock (Align & Cohesion)- Follow group
- Wander - Around the screen
- Avoid Obstacles - Swordfish
- Avoid Boundaries
- Seperation - Other small fish
   
#### State Transistions

- Gets hunted by a swordfish
- Notices a food piece nearby
   
### _Hunting fish food_

**Objective:** _Go after fish food, even if it means leaving the school._

#### Steering Behaviors

- Seek - fish food
- Avoid Obstacles - Swordfish
- Avoid Boundaries
- Seperation - Other small fish
   
#### State Transistions

- Fish food no longer nearby (eaten by them or another fish)

## _Swordfish_

Swordfish don't school with small fish. They will occasionally try to shoot through the school and catch a small fish. They are NOT attracted to fish food.

### _Stalking_

**Objective:** _Stalking around the school, waiting on cooldown before they hunt._

#### Steering Behaviors

- Seperation - Other swordfish, small fish
- Wander - Around the screen
- Avoid Boundaries
   
#### State Transistions

- After the cooldown is over, they will try to hunt
   
### _Hunting_

**Objective:** _Chase and try to catch a small fish._

#### Steering Behaviors

- Seek - A chosen small fish
- Seperation - Other swordfish
   
#### State Transistions

- If it hits a small fish, return to stalking immediatley
- If it doesn't hit any fish, return to stalking

## Sources

-   I made every art asset in this game myself using Piskel.

## Make it Your Own

- I made all the art assets myself.

## Known Issues

_List any errors, lack of error checking, or specific information that I need to know to run your program_
- There is an occasional outOfBounds error.

### Requirements not completed

_If you did not complete a project requirement, notate that here_

