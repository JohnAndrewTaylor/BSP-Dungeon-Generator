# Dungeon Room Generator Using Binary Space Partitioning

Through my Game A.I. course at Georgia Tech, I was exposed to a lot of interesting approaches to procedural content generation. Our homework project involved creating old-school Mario levels in a Java game engine, but I decided that I wanted to apply my knowledge in a more common gaming industry product, such as Unity with C# scripts. Initially, I was inclined to use cellular automata, which we had discussed in class for the purpose of creating terrain and biomes, but I decided that such a technique would lead to cave-like structures, which did not fit the theme of a retro dungeon. After a bit of research, I determined that Binary Space Partitioning would lead to a very fast algorithm that could create rectangular dungeon rooms.

The algorithm continuously splits a given game space into two randomly-sized children. This action stops once an area cannot be divided in half without violating the minimum size requirements of a dungeon room. Any area that has no children - and is therefore a leaf - can then be considered a valid dungeon room. The algorithm then fills each tilemap cell appropriately using the provided tiles (the artwork I used is referenced below). This algorithm has shown to work extremely fast and has not displayed any bugs so far. The algorithm is currently missing the code which creates hallways to connect the rooms.

All the artwork used was created by Buch and is available at OpenGameArt under a public domain license (https://opengameart.org/content/a-blocky-dungeon).

### To do:
* Improve decoration placement logic (such as a checkerboard restriction)
* Add hallway create
* Improve efficiency and remove redundancies
* Increase documentation
* Add example output images to repository
