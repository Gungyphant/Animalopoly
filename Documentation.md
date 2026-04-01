# **Animalopoly**



## <u>**Overview**</u>

A Monopoly-style board game themed around running a zoo, with 4 players, where the players roll dice to move around the board and purchase animals.

## <u>**Analysis**</u>

### Background

Create a board game based on Monopoly; players roll 2 dice to move around a 26-tile board. Upon landing on an property they may purchase it if it is unowned, spend money to upgrade it if they own it, or be forced to pay the owner some money. The last player to have money wins.

If both dice the player rolls show the same number, they will draw a card from a deck, which will have a consequence, e.g. losing money, and a scenario that explains why it happened.

When players pass start, they should be awarded £500, and when they land on start, £1000.

When 

### Alternative solutions

https://github.com/intrepidcoder/monopoly
https://github.com/b2developer/MonopolyNEAT
https://github.com/zhongyi-tong/monopoly	

### Objectives

UI:

- Players' names should be shown
- The board should be shown
- It should be possible to distinguish between players with the same name
- The name, set, and owner of an animal should be apparant at a glance
- There should be a UI element for an animal showing at least the following information: the animal's name, the animal's level, how much a player has to pay if they land there, how much the animal costs to buy/upgrade, the set the animal is in, and the animal's owner
- There should be an option to use a CLI-based UI
- There should be an option to use a GUI which exists in a separate window, with the console being used only for text output and input
- The rolling dice should be visible, as should their final result and the number of spaces the player should move
- Text should be easily readable, with sufficient contrast against the background
- Text should allow formatting to draw attention to certain parts, and to make its purpose clear
- On the live-refreshing GUI, player pieces should visibly take 'steps' as they move

Movement:

- The number of spaces moved should be pseudorandom with an expected distribution matching that of rolling 2 6-sided dice
- If the dice show the same face, the player should be awarded a random card
- Upon landing on an unowned space, players should be given the choice whether or not to buy, with the UI element showing the animal's information being displayed
- Upon landing on a space they own, players should be given the choice whether or not to upgrade, with the UI element shown
- Upon landing on a space owned by another player, players should be informed they have to pay the owner, with the UI element shown
- Upon passing start, players should be awarded £500
- Upon landing on start, players should be awarded £1000 and not awarded the £500 for passing it
- Upon landing on the 'Miss a go' square, players should be informed their next turn will be skipped; this should also be clear when it gets to their skipped turn

Commands:

- Players should be able to run commands to take certain actions
- There should be a command to see an explanation of what commands exist, and how to use them
- There should be the ability to save and load on demand via a command
- There should be a command to alter a player's money, both for testing and to allow players to customise their game experience; this should be considered a 'cheat', and confirmation should be required before the first cheat can be run
- There should be the ability to view information about players on demand
- There should be the ability to view the card for an animal on demand
- There should be the ability for players to trade animals and/or money on demand

AI:

- There should be the option to have AI 'players', so that the game can be played with less than 4 players
- There should be varying strengths of AI available
- One AI should always buy/upgrade the animals, when it is given the opportunity
- One AI should always buy/upgrade the animals if doing so would not put it at risk of bankruptcy before its next turn
- One AI should determine if buying/upgrading the animal is a good investment, and then only do it if it would not be put in danger of bankruptcy buy doing so
- One AI should predict the possible future states of the game and maximise the worst-case probability of it winning

Saving:

- There should be the ability to save and load
- Saving should write every important piece of data to disk, such that the program can be completely relaunched and the save file can be loaded to resume the game with no noticable differences
- There should be data cleaning to prevent invalid names - e.g. ones containing slashes - from being used

Ending:

- When players go 'into debt' (have negative money), they should be informed and have one turn to get out of debt or else be eliminated
- When players are eliminated, their animals should have their owner cleared, but should not return to the base level, thus making them more valuable
- When there is only one player left in, the game should end and they should be declared the winner. If all remaining players are eliminated on the same turn, the winner should be decided by which player had the smallest 'debts' (had the most money)
- After the game ends, a 'game review' graph should be generated to let players see how much many they had throughout the game

## <u>**Design**</u>

### Interface design:

#### CLI-based UI:

![CLI UI](./Documentation_images/CLI_UI.png)

#### GUI:

![GUI UI](./Documentation_images/GUI_UI.png)

#### Animal info card:

![Card](./Documentation_images/Card.png)

#### Saving:

Save files will be stored as [MessagePack binary files](https://msgpack.org/index.html "MessagePack home page")

They will store every player's data, as well as the name of the game, the turn count, and whether or not cheats have been enabled.

#### Class Diagram:

![Class diagram](./Documentation_images/Class_diagram.png)

## <u>**Technical Solution**</u>



## <u>**Testing**</u>



## <u>**Evaluation**</u>

