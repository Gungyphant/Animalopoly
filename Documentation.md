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

Monopoly
https://github.com/intrepidcoder/monopoly
https://github.com/b2developer/MonopolyNEAT
https://github.com/zhongyi-tong/monopoly	

### Objectives

Starting:

1. Players should be allowed to enter any one-char name. If their name is invalid, they should be prompted to enter a new name

UI:

2. Players' names should be shown
3. The board should be shown
4. It should be possible to distinguish between players with the same name
5. The name, set, and owner of an animal should be apparant at a glance
6. There should be a UI element for an animal showing at least the following information: the animal's name, the animal's level, how much a player has to pay if they land there, how much the animal costs to buy/upgrade, the set the animal is in, and the animal's owner
7. There should be an option to use a CLI-based UI
8. There should be an option to use a GUI which exists in a separate window, with the console being used only for text output and input
9. The rolling dice should be visible, as should their final result and the number of spaces the player should move
10. Text should be easily readable, with sufficient contrast against the background
11. Text should allow formatting to draw attention to certain parts, and to make its purpose clear
12. On the live-refreshing GUI, player pieces should visibly take 'steps' as they move

Movement:

13. The number of spaces moved should be pseudorandom with an expected distribution matching that of rolling 2 6-sided dice
14. If the dice show the same face, the player should be awarded a random card
15. Upon landing on an unowned space, players should be given the choice whether or not to buy, with the UI element showing the animal's information being displayed
16. Upon landing on a space they own, players should be given the choice whether or not to upgrade, with the UI element shown
17. Upon landing on a space owned by another player, players should be informed they have to pay the owner, with the UI element shown
18. Upon passing start, players should be awarded £500
19. Upon landing on start, players should be awarded £1000 and not awarded the £500 for passing it
20. Upon landing on the 'Miss a go' square, players should be informed their next turn will be skipped; this should also be clear when it gets to their skipped turn

Commands:

21. Players should be able to run commands to take certain actions
22. There should be a command to see an explanation of what commands exist, and how to use them
23. There should be the ability to save and load on demand via a command
24. There should be a command to alter a player's money, both for testing and to allow players to customise their game experience; this should be considered a 'cheat', and confirmation should be required before the first cheat can be run
25. There should be the ability to view information about players on demand
26. There should be the ability to view the card for an animal on demand
27. There should be the ability for players to trade animals and/or money on demand

AI:

28. There should be the option to have AI 'players', so that the game can be played with fewer than 4 players
29. There should be varying strengths of AI available
30. One AI should always buy/upgrade the animals, when it is given the opportunity
31. One AI should always buy/upgrade the animals if doing so would not put it at risk of bankruptcy before its next turn
32. One AI should determine if buying/upgrading the animal is a good investment, and then only do it if it would not be put in danger of bankruptcy buy doing so
33. One AI should predict the possible future states of the game and maximise the worst-case probability of it winning

Saving:

34. There should be the ability to save and load
35. Saving should write every important piece of data to disk, such that the program can be completely relaunched and the save file can be loaded to resume the game with no noticable differences
36. There should be data cleaning to prevent invalid names - e.g. ones containing slashes - from being used

Ending:

37. When players go 'into debt' (have negative money), they should be informed and have one turn to get out of debt or else be eliminated
38. When players are eliminated, their animals should have their owner cleared, but should not return to the base level, thus making them more valuable
39. When there is only one player left in, the game should end and they should be declared the winner. If all remaining players are eliminated on the same turn, the winner should be decided by which player had the smallest 'debts' (had the most money)
40. After the game ends, a 'game review' graph should be generated to let players see how much many they had throughout the game

Graph:

41. The graph should show how much money each player had on each turn
42. The players should be easily distinguishable on the graph
43. There should be a command to generate a graph at any time at any resolution
44. The text in the graph should be readable regardless of the resolution
45. The markings for the axes should not overlap eachother
46. It should be apparant how much money players start with, and at what point they are in danger of bankruptcy
47. The axes should automatically scale to fit the data

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

They will store every player's data, as well as the name of the current game, the turn count, and whether or not cheats have been enabled.

#### Class Diagram:

![Class diagram](./Documentation_images/Class_diagram.png)

#### Graphs:

![Normal graph](./Documentation_images/Graph.png)
![Large range graph](./Documentation_images/Large_range_graph.png)
![Custom res graph](./Documentation_images/Custom_res_graph.png)

#### Algorithms:

The main gameplay loop, used for each player each turn

![Flowchart](./Documentation_images/Flowchart.png)

## <u>**Technical Solution**</u>



## <u>**Testing**</u>

| Test ID | Test description | Test data | Expected results | Objectives tested | Evidence | Notes |
| :-----: | :--------------- | :-------- | :--------------- | :---------------- | :------- | :---- |
| 1 | Check that invalid names are not allowed | <ul><li>Either select or do not select GUI mode</li><li>Enter a name longer than 1 char</li><li>Press enter without entering a name (Console.ReadLine returns "")</li><li>Press Ctrl+Z then enter (Console.ReadLine returns returns null)</li><li>Enter a valid name</li><li>Repeat this for all players</li></ul> | For each invalid name, it re-requests that the player enters a name. When the valid name is entered, the player's name is set to it. | 01 | ![Evidence](./Documentation_images/Testing/4.png) |  |
| 2 | Check that the board renders correctly in CLI mode | <ul><li>Do not select GUI mode</li><li>Input any valid names for the 4 players, e.g. a, b, c, d</li></ul> | Board appears similar to the one shown in the design for the CLI UI. | 02, 03, 04, 05, 07, 10, 11 | ![Evidence](./Documentation_images/Testing/2.png) |  |
| 3 | Check that the board renders correctly in GUI mode | <ul><li>Select GUI mode</li><li>Input any valid names for the 4 players, e.g. a, b, c, d</li></ul> | Board appears similar to the one shown in the design for the GUI. | 02, 03, 04, 05, 08, 10 | ![Evidence](./Documentation_images/Testing/1.png) |  |
| 4 | Check that rolling and purchasing animals works correctly in GUI mode | <ul><li>Select GUI mode</li><li>Input any valid names for the 4 players, e.g. a, b, c, d</li><li>Press enter to roll</li><li>Purchase the animal landed on</li></ul> | When enter is pressed, the dice should appear and visibly roll. If the numbers match, the player should be awarded a random card. The player should then be seen taking steps until they reach the animal, at which point they will be shown the animal card, which should look similar to the one in the design section, and asked if they want to purchase it. When they purchase the animal, the colour of its name should update to match the player. | 06, 09, 11, 12, 14, 15 | [![Evidence](https://markdown-videos-api.jorgenkh.no/url?url=https%3A%2F%2Fwww.youtube.com%2Fwatch%3Fv=fVPUJFb42SI)](https://youtu.be/fVPUJFb42SI |  |
| 5 | Check that rolling and purchasing animals works correctly in CLI mode | <ul><li>Do not select GUI mode</li><li>Input any valid names for the 4 players, e.g. a, b, c, d</li><li>Press enter to roll</li><li>Purchase the animal landed on</li><li>Press enter for the second player to roll</li></ul> | When enter is pressed, the face values on the dice should appear and visibly change. If the numbers match, the player should be awarded a card. The player should then move the correct number of steps to the animal, at which point they will be shown the animal card, which should look similar to the one in the design section, and asked if they want to purchase it. When the next player rolls and the board is displayed again, the colour of the purchased animal's name should have updated to match the player who purchased it | 06, 09, 14, 15 | [![Evidence](https://markdown-videos-api.jorgenkh.no/url?url=https%3A%2F%2Fwww.youtube.com%2Fwatch%3Fv=5OzeTL0dfUA)](https://youtu.be/5OzeTL0dfUA |  |
| 6 | Check that upgrading animals works correctly | <ul><li>Either select or do not select GUI mode</li><li>Input any valid names for the 4 players, e.g. a, b, c, d</li><li>Play until a player lands on their own animal</li><li>Choose to upgrade</li><li>Run `!info animal [the id of the animal upgraded]`</li></ul> | When the player lands on their own animal, they should be shown the animal card and asked if they want to upgrade. When they do so, the money should be taken from them. When `!info animal [the id of the animal upgraded]` is run, it should show that the level has increased. | 16 | [![Evidence](https://markdown-videos-api.jorgenkh.no/url?url=https%3A%2F%2Fwww.youtube.com%2Fwatch%3Fv=DTC8UXzBRrY)](https://youtu.be/DTC8UXzBRrY | Upgrade occurs at 1:26. |
| 7 | Check that paying other players works correctly | <ul><li>Either select or do not select GUI mode</li><li>Input any valid names for the 4 players, e.g. a, b, c, d</li><li>Play until a player lands on another's animal</li></ul> | When the player lands on another's animal, they should be shown the animal card and informed they have to pay. The correct amount of money should be taken from them and given to the animal's owner. | 17 | [![Evidence](https://markdown-videos-api.jorgenkh.no/url?url=https%3A%2F%2Fwww.youtube.com%2Fwatch%3Fv=G8kC9APMmt0)](https://youtu.be/G8kC9APMmt0 |  |
| 8 | Check that money is correctly awarded when passing Start | <ul><li>Either select or do not select GUI mode</li><li>Input any valid names for the 4 players, e.g. a, b, c, d</li><li>Play until a player passes Start</li><li>Run `!info player [the id of the player who passed Start] m`</li></ul> | When the player passes but does not land on start, they should be awarded £500 before taking the actions required when landing on their destination square. The output of `!info` should show this has occurred. | 18 | [![Evidence](https://markdown-videos-api.jorgenkh.no/url?url=https%3A%2F%2Fwww.youtube.com%2Fwatch%3Fv=xatV8rZpkDc)](https://youtu.be/xatV8rZpkDc | Start crossed at 1:00. |
| 9 | Check that money is correctly awarded when landing on Start | <ul><li>Either select or do not select GUI mode</li><li>Input any valid names for the 4 players, e.g. a, b, c, d</li><li>Play until a player lands on Start</li><li>Run `!info player [the id of the player who passed Start] m`</li></ul> | When the player lands on start, they should be awarded £1000. The output of `!info` should show this. | 19 | [![Evidence](https://markdown-videos-api.jorgenkh.no/url?url=https%3A%2F%2Fwww.youtube.com%2Fwatch%3Fv=dKRkkPiYMLs)](https://youtu.be/dKRkkPiYMLs | Start landed on at 1:37. |
| 10 | Check that Miss A Go works correctly | <ul><li>Either select or do not select GUI mode</li><li>Input any valid names for the 4 players, e.g. a, b, c, d</li><li>Play until a player lands on Miss A Go</li><li>Run `!info player [the id of the player who landed on Miss A Go] s`</li><li>Continue play until it would be that player's turn again</li><li>Run `!info player [the id of the player who landed on Miss A Go] s`</li><li>Continue play until the player's next turn</li></ul> | When the player lands on Miss A Go, they should be informed they will miss their next turn. The first `!info` will confirm that this has been updated. When it reaches their turn again, they will instead be told it has been skipped. The second `!info` will show that the skip has been cleared, and when it is the player's turn again, they will take their turn as normal. | 20 | [![Evidence](https://markdown-videos-api.jorgenkh.no/url?url=https%3A%2F%2Fwww.youtube.com%2Fwatch%3Fv=yLMibRqf9W0?si=v-ppevhS4eWU76eH)](https://youtu.be/yLMibRqf9W0?si=v-ppevhS4eWU76eH | Miss a turn landed on at 1:29. |
| 11 | Check that commands work correctly | <ul><li>Either select or do not select GUI mode</li><li>Input any valid names for the 4 players, e.g. a, b, c, d</li><li>Run the following commands:</li><li>`!help`</li><li>`!cheats on`</li><li>`!money set 1 10000`</li><li>`!info player 1`</li><li>`!info animal 3`</li><li>Roll the dice and purchase the animal landed on</li><li>`!trade 1 3 -100 [the id of the animal landed on] `</li><li>`!info player 3 p m`</li><li>`!save "Commands test"`</li><li>`!save "Invalid/name:"`</li><li>`!graph "Commands test" 5000 1000`</li><li>Open the graph</li><li>Restart the program</li><li>`!load "Commands test"`</li><li>`!info player 3 p m`</li></ul> | When `!help` is run, the list of commands and how to use them should be shown. When `!info player 1` is run, some useful information about player 1 should be shown. When `!info animal 3` is run, animal 3's card should be shown. When `!info player 3 p m` is run, it should show that player 3 has lost £100 and gained the animal player 1 had landed on. When `!save "Invalid/name:"` is run, the filepath should be updated and the user should be informed what it was renamed to. When `!graph "Commands test" 5000 1000` is run, a graph similar to the one in Design should be generated and saved in the same folder as the save file, with a resolution of 5000px x 1000px. When `!info player 3 p m` is run after the restart and load, it should output the same information as before the load. | 21, 22, 23, 24, 25, 26, 27, 34, 35, 36, 43 | [![Evidence](https://markdown-videos-api.jorgenkh.no/url?url=https%3A%2F%2Fwww.youtube.com%2Fwatch%3Fv=iDa68MTayl4)](https://youtu.be/iDa68MTayl4 | The testing ended prematurely as `!load` caused a crash, as loading has not yet been successfully implemented, meaning objective 34 failed. The program did, however, successfully write the data. During the testing, a parameter was unintentionally left out from the `!money` command, which shows how the program handled the error and informed the user. |
| 12 | Check that AIs work correctly | <ul><li>Either select or do not select GUI mode</li><li>Input any valid names for the 4 players, e.g. a, b, c, d</li><li>Run the following commands:</li><li>`!help ai`</li><li>`!ai 1 1`</li><li>`!ai 2 2`</li><li>`!ai 3 3`</li><li>`!ai 4 4`</li><li>Press enter to roll</li></ul> | When `!help ai` is run, the info for `!ai` should be displayed, which will show what number corresponds to each AI level. These correspond with the AI described in objectives 30, 31, 32, and 33 respectively. When enter is pressed, the AIs should begin playing automatically, behaving according to their AI level. | 28, 29, 30, 31, 32, 33 | [![Evidence](https://markdown-videos-api.jorgenkh.no/url?url=https%3A%2F%2Fwww.youtube.com%2Fwatch%3Fv=b-QKNn5UvBg)](https://youtu.be/b-QKNn5UvBg | Player 4 did not behave correctly, as its AI has not been implemented, meaning objective 33 was failed. The graph is visible at 1:56. |
| 13 | Check that the ending of the game works correctly | <ul><li>Either select or do not select GUI mode</li><li>Input any valid names for the 4 players, e.g. a, b, c, d</li><li>Play until a player goes 'into debt'</li><li>Play until a player goes bankrupt</li><li>To reduce testing time, run `!anims off`</li><li>Run `!info player [the id of the player that has gone bankrupt] p m`</li><li>Play until the game ends</li><li>Open the generated graph</li></ul> | When the player goes into debt, they should be informed. When the player goes bankrupt, the colours of the animals previously owned by them should revert to white, to show they are now unowned. When the `!info player` is run, it should show the bankrupted player no longer has any animals and that their money is negative. The generated graph should be similar to the one shown in design. | 37, 38, 39, 40, 41, 42, 44, 45, 46, 47 | [![Evidence](https://markdown-videos-api.jorgenkh.no/url?url=https%3A%2F%2Fwww.youtube.com%2Fwatch%3Fv=yC8H98k5a4Q)](https://youtu.be/yC8H98k5a4Q | The command used to reduce testing time disables all 'animations', e.g. pieces visually moving and dice visually rolling. Player is informed they are 'in debt' at 1:31. Player goes bankrupt at 1:52. The game ends at 3:38. Note that, even after b is eliminated, c has another turn; this is to ensure all players have had the same number of turns - if c had gone bankrupt on that final turn, whichever of the two had the most money (least 'debt') when they were bankrupt would win. Graph shown at 3:59. |

Objective 13 cannot easily be tested through regular gameplay, so instead Testing.Thirteen() can be called, returning True if the observed results for rolling 10 million times are a sufficient approximation of the expected results; in testing, it has always returned true, although it is theoretically possible, due to the random nature of Roll, that it would return false

## <u>**Evaluation**</u>



## <u>**Appendix**</u>


