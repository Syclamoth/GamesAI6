GamesAI6
========

Notices
-------
None as of 08 Aug 2012.


Sheep_working branch
-------
For Sheep's Finite State Machine code. The sheep has 3 states:
+ Sheep_roaming (idle state): Sheep follows the sheperd (if it can see him) with its normal speed. If it can't see the sheperd, it follow the nearest sheep.
+ Sheep_run: when it sees a wolf, it will run away from the wolf towards the sheperd or any nearest sheep. It runs faster than normal. Panic level will continuously increase
+ Sheep_gonenuts: It's in its panic state. It will run wildly or just stay at the same spot. Panic level at max. If it doesn't see the wolf anymore, panic level will decrease and return to its idle state (roaming)

panicLevel variable: the panic level of a sheep, range from 0 - 10
courageLevel variable: each sheep has different courageLevel, range from 0 - 10. The higher it is, the lower chance the sheep will change to its panic state.

The code: I don't understand much about the code used in Machine.cs, FuzzyTransition.cs, Brain.cs, etc, so I took a liberty to change it to something easier to understand for me:
+ Sheep.cs: contains info regarding sheep behaviour (i.e. sheep finite state machine Machine<Sheep> and its characteristics.
+ Machine.cs: Finite State Machine in generic format i.e. Machine<T>. T is the owner of the Finite State Machine, in this case is sheep - Machine<Sheep>
+ State.cs: the state in generic format i.e. State<T>. It contains only 3 abstract classes. I don't understand the rest. For what ?


How to configure git (using bash)
---------------------------------

1. Navigate to the directory holding the project directory. i.e. Assets/../..
2. Make sure the project directory is not called 'GamesAI6'. If so rename it either using a file browser or the `mv` command.
3. Clone the repo using the command `git clone https://github.com/Syclamoth/GamesAI6.git`
4. Copy the contents of the 'GamesAI6' folder to the project directory using the command: `cp -R GamesAI6/ project_directory/` obviously replacing `project_directory withe name of the Unity project directory.
5. The repository is now set up, commit files using `git add FILENAME1 FILENAME2` followed by `git commit -m "Commit description"`. To add all changed and new files, use `git add .`
6. Push changes to github using `git push origin BRANCHNAME` all stable code should be pushed to the `master` branch.
7. Before you start a programming session, call `git pull` first. This ensures that the codebase you are working with is the latest version.

Managing branches
-----------------

One of the best thing about git is the ability for a programmer to create a new branch and edit that, without the danger of ruining perfectly stable code for everyone else. To learn more about branching, visit: http://learn.github.com/p/branching.html

Please make sure when writing new code you do it in a new branch :)

Links
-----

* [To-do list](https://github.com/Syclamoth/GamesAI6/blob/master/TODO.md)
