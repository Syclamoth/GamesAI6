Kieren, please add zchaoz to the repo ASAP so he can start coding.

How to configure git (using bash)
---------------------------------

1. Navigate to the directory holding the project directory. i.e. Assets/../..
2. Make sure the project directory is not called 'GamesAI6'. If so rename it either using a file browser or the `mv` command.
3. Clone the repo using the command `git clone https://github.com/Syclamoth/GamesAI6.git`
4. Copy the contents of the 'GamesAI6' folder to the project directory using the command: `cp -R GamesAI6/ project_directory/` obviously replacing `project_directory withe name of the Unity project directory.
5. The repository is now set up, commit files using `git add FILENAME1 FILENAME2` followed by `git commit -m "Commit description"`. To add all changed and new files, use `git add .`
6. Push changes to github using `git push origin BRANCHNAME` all stable code should be pushed to the `master` branch.

Managing branches
-----------------

One of the best thing about git is the ability for a programmer to create a new branch and edit that, without the danger of ruining perfectly stable code for everyone else. To learn more about branching, visit: http://learn.github.com/p/branching.html

Please make sure when writing new code you do it in a new branch :)
