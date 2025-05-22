VAR roro_2_bartered = false

{roro_2_bartered: ->NoBarter | ->Start}

=== Start ===
Hello there. #NPC
* I would like to Barter. 
    ->Barter
* What do you need? 
    I could use some new tools. #NPC
    ** How interesting...
    -> END
* Nevermind.
    -> END

=== Barter ===
NULL_LINE #Barter
-> END

=== NoBarter ===
Sorry, I don't have anything else. #NPC
-> END