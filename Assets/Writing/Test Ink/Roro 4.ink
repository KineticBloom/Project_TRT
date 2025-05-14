VAR roro_4_bartered = false

{roro_4_bartered: ->NoBarter | ->Start}

=== Start ===
Hello there. #NPC
* I would like to Barter. 
    ->Barter
* What do you like? 
    Lots of religious items. #NPC
    I won't accept less than 2! #NPC
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