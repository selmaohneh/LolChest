# LolChest

A serverless piggy bank for League of Legends.

## Introduction
My friends and me have a piggy bank to save up some money for a vacation, a concert or just some beer. :-) To get some money into it we play League of Legends. We defined a penalty depending on how well (or bad) each of us performed during a game. 

The formula for that penalty is `Deaths / (Kills + Assists + 1) * GameDurationInMinutes * x * y`. If we won the game `x` is 1, if we lost `x` is 1.5 (just to get the penalty a little more juicy). With the factor `y` we can tune our overall costs each month to somehow get the desired input into the piggy bank.

Since I am way too lazy to calculate the penalties for all my friends manually after each game, I programmed **LolChest**. **LolChest** periodically polls the statistics of the games we played together from the Riot Games API and calculates the penalties for each participant. The daily and monthy results will be pushed to each player via mail and the data is stored in a table for further use.