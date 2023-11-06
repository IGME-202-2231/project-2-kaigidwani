# Project _PERCH_

[Markdown Cheatsheet](https://github.com/adam-p/markdown-here/wiki/Markdown-Here-Cheatsheet)


### Student Info

-   Name: _Kai Gidwani_
-   Section: _02_

## Simulation Design

My simulation is going to have flocks of birds flying and landing on perches in trees. Occasionally big birds will want to take the perch of a smaller bird and force them to move. Players can also click to shake a tree and get birds to move off of it.

### Controls

-   Click on a tree to shake it and get the birds on it to move.

## _Small bird_

Small birds flock together and will try to land on a perch in a tree if they aren't escaping from being shaken out of their perch, or forced out by a big bird.

### _Seeking Spot_

**Objective:** _Find a spot in a tree to land in._

#### Steering Behaviors

- Avoid - Big bird
- Seek - Nearest unoccupied perch
- Flock - Follow group
- Obstacles - Big birds, the ground
- Seperation - Other small birds
   
#### State Transistions

- When a big bird pushes it out of its perch
- When a player's shake pushes it out of its perch
- When it leaves voluntarily with other birds nearby
   
### _Resting_

**Objective:** _Rest in a perch. Leave if forced out or if wanting to follow nearby birds. Leave after set time._

#### Steering Behaviors

- Avoid - Big bird
- Rest - Prefer not to move
- Flock - Follow group
- Obstacles - Big birds
- Seperation - Other small birds
   
#### State Transistions

- When it lands in a perch

## _Big bird_

Big birds don't flock with any other birds. They will try to land on a perch and force a small bird out of that perch if necessary. They cannot be forced out by other big birds.

### _Seeking Spot_

**Objective:** _Find a spot in a tree to land in. Force a small bird out if necessary._

#### Steering Behaviors

- Seek - ANY nearest perch
- Obstacles - The ground
- Seperation - Other big birds
   
#### State Transistions

- When a player's shake pushes it out of its perch
- When it leaves voluntarily with other birds nearby
   
### _Resting_

**Objective:** _Rest in a perch. Leave if forced out or after set time._

#### Steering Behaviors

- Rest - Prefer not to move
- Obstacles - Big birds
- Seperation - Big birds
   
#### State Transistions

- When it lands in a perch

## Sources

-   _List all project sources here –models, textures, sound clips, assets, etc._
-   _If an asset is from the Unity store, include a link to the page and the author’s name_

## Make it Your Own

- _List out what you added to your game to make it different for you_
- _If you will add more agents or states make sure to list here and add it to the documention above_
- _If you will add your own assets make sure to list it here and add it to the Sources section
- I plan on making all the art assets myself.

## Known Issues

_List any errors, lack of error checking, or specific information that I need to know to run your program_

### Requirements not completed

_If you did not complete a project requirement, notate that here_

