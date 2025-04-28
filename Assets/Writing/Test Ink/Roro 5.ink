VAR roro_5_bartered = false

{roro_5_bartered: ->NoBarter | ->Start}

=== Start ===
Hello there. #NPC
* I would like to Barter. 
    ->Barter
* Nevermind.
    -> END

=== Barter ===
NULL_LINE #Barter
-> END

=== NoBarter ===
Sorry, I don't have anything else. #NPC
-> END