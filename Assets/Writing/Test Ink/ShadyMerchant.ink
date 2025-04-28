VAR shady_bartered = false

Welcome to the best choice you’ll make all day. #NPC
* I would like to Barter. 
    {shady_bartered: ->NoBarter | ->Barter}
* Nevermind.
    -> END

=== Barter ===
Didn’t get this cheaply, ain’t gonna sell it cheaply. #NPC
NULL_LINE #Barter
-> END

=== NoBarter ===
Sorry, all out. #NPC
-> END