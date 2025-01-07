Trying to build a chess engine from scratch by myself.

WIP, ~~currently finishing up the move generator.~~ ~~Next up is implementing the UCI protocol so it can be used with a GUI~~(Only the bare minimum for now). After that im starting to make the evaluator.

To run the current the engine(it only plays random moves yet) in [Arena](http://www.playwitharena.de/), first cd into Hattin/ and run `dotnet publish`. Then remove all the other loaded engines (press F11 in Arena to see them). After that, install the engine under the "Engines-> Install New Engine" by selecting the pubilshed .exe in bin/Release/net(your verison)/Hattin. Click ok on the promts and thats it. Now you can either play a game against it or make it play against itself by clicking "Demo".
