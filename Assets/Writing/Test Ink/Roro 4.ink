VAR roro_4_bartered = false

{roro_4_bartered: ->NoBarter | ->Start}

=== Start ===
Hello there. #NPC
* I would like to Barter.
    Great! #NPC
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
I don't need anything else. I am satisfied with our trade. #NPC
-> END