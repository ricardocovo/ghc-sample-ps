I want to create the specs related to "Manage Players", this will include:

* Creating all the data model, entitines, etc. needed
* Creating a Mock data provider  to be able to do proper testing

We will need basically 3 Page:

1. Manage Players
    * look at ./docs/wireframes/ManagePlayers.png
    * it will display the list of players with their age
    * when you click into a player you go to the "Edit Player" plage
    * There will be an "Add Player"  button at the top as in the wireframe that will go to the create player [age].

1. Create Player
    * look at ./docs/wireframes/CreatePlayer.png
    * This page allows to create a new player. It does not let you pick teams yet.
    * After the player is created, you get directed to the "Edit Player" page.

1. Edit Player
    * look at ./docs/wireframes/EditPlayer*.png
    * For now, focus only on the player info.
    * For the "Teams" and "Stats" page, simply add a label "FUTURE DEV"
    * When a user updates a player, it should be redirected to the Manage Players page.