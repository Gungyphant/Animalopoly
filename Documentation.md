# **Animalopoly**



## <u>**Overview**</u>

A Monopoly-style board game themed around running a zoo, with 4 players, where the players roll dice to move around the board and purchase animals.

## <u>**Analysis**</u>



## <u>**Design**</u>

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
- The name of an animal should be apparant at a glance
- The set an animal is in should be apparant at a glance
- It should be apparant who owns an animal at a glance
- There should be a UI element for an animal showing at least the following information: the animal's name, the animal's level, how much a player has to pay if they land there, how much the animal costs to buy/upgrade, ***TODO***
- There should be an option to use a CLI-based UI
- There should be an option to use a GUI which exists in a separate window, with the console being used only for text output and input
- The rolling dice should be visible, as should their final result and the number of spaces the player should move
- Text should be easily readable
- Text should allow formatting to draw attention to certain parts, and to make its purpose clear
- On the live-refreshing GUI, player pieces should visibly take 'steps' as they move

Movement:

- The number of spaces moved should be pseudorandom with an expected distribution matching that of rolling 2 6-sided dice
- Upon landing on an unowned space, players should be 

## <u>**Technical Solution**</u>



## <u>**Testing**</u>



## <u>**Evaluation**</u>

