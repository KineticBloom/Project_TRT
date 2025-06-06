VAR roro_3_bartered = false

{roro_3_bartered: ->NoBarter | ->Start}

=== Start ===
Hello there. #NPC
* I would like to Barter. 
    Great! #NPC
    ->Barter
* What do you like? 
    I love farming! #NPC
    ** How interesting...
    -> END
* Nevermind.
    -> END

=== Barter ===
NULL_LINE #Barter
-> END

=== NoBarter ===
As much as I'd like more things to farm with, I've go nothing else for you. #NPC
-> END