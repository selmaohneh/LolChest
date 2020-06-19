# LolChest
## Introduction
My friends and me have a piggy bank to save up some money for a vacation, a concert or just some beer. :-) To get some money into it we play League of Legends. We defined a penalty depending on how well (or bad) each of us performed during a game. 

The formula for that penalty is `Deaths / (Kills + Assists + 1) * GameDurationInMinutes * x * y`. If we won the game `x` is 1, if we lost `x` is 1.5 (just to get the penalty a little more juicy). With the factor `y` we can tune our overall costs each month to somehow get the desired input into the piggy bank.

Since I am way too lazy to calculate the penalties for all my friends manually after each game, I programmed **LolChest**. **LolChest** periodically polls the statistics of the games we played together from the Riot Games API and calculates the penalties for each participant. The data is stored in a table for further use, i.e. I made a little HTTP endpoint where we can see the recent results and summed up penalties of each month.

This piggy bank was kind of always the first project I solved when dipping into a new technology. This time, by going serverless with Azure Functions, it might have been the last time :-)

## Thoughts / Learnings
The serverless approach was totally new for me, so there were many eye-openers for me.
* Azure Functions Apps support Dependency Injection! This was very important since there is a rate limit when polling the Riot Games API. I could achieve that by registering the `IRiotClient` as singleton inside the service builder. In additon this will allow easy testing since all Azure Functions "boilerplate stuff" can simply be mocked away.
* My first approach was chaining all functions via queue triggers. The queues are very flexible: You can configure how many items get dequeued per batch, how often the queue should be polled, how often each item should be retried if a functions failed with it, and how long the function should wait before retrying an item.
* The first approach worked fine but had some high table transaction counts. Each player was polled for it's last 100 games. One function checked whether the game id is already known by polling the table. Of course there were some common games between the players (since that's what **LolChest** is all about :-D) so this function was fired multiple times with redundant data - this sums up pretty fast. After some research I found Azure Durable Functions. This allowed me to implement the current approach utilizing the fan in/fan out pattern to i.e. collect all game ids of each player and distinct and filter them before providing them to the next function of the cahin.
* All Azure resources I am using are currently free of charge since I am not reaching the free quota. Hope that stays that way as soon as my friends start polling the result all the time. :-P

## Getting Started / Contributing
* Simply pull the repository and get coding! :-) Pull Requests are always welcome!
* Feel free to create any kind of issue.
* You have to create the Azure resources for **LolChest** yourself - but I bet there is an import/export for that. I definitely have to do some research on that topic. That would make the on-boaring way more easy...
* [Kaffee? :-)](https://www.buymeacoffee.com/SaMAsU1N6)
