Trying to build a chess engine from scratch by myself.

WIP, working on the search algorithms and a new and faster move-generator using bitboards. This version only searches to depth 3 to make it demoable on most pcs (You can change this in the GetNextMove method in the engine)

To run the current the engine in [Arena](http://www.playwitharena.de/), first cd into Hattin/ and run `dotnet publish`. Then remove all the other loaded engines (press F11 in Arena to see them). After that, install the engine under the "Engines-> Install New Engine" by selecting the pubilshed .exe in bin/Release/net(your verison)/Hattin. Click ok on the prompts and thats it. Now you can either play a game against it or make it play against itself by clicking "Demo".

To just run it in the console, run `dotnet run` instead. When it boots up, you have to issue the commands `uci` and `isready`. Now you can make it search the current position with `go`. To give it a sequence of moves from the starting position use `position startpos [moves ...a list of moves]` where moves are given in a FromsquareTosquare format (e2e4 for example), followed by `go`. To give it a specific position, run `position fen <fenstring> [moves ...a list of moves]`.
