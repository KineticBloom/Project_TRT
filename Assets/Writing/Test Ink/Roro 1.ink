VAR roro_1_bartered = false

{roro_1_bartered: ->NoBarter | ->Start}

=== Start ===
Hello there. #NPC
* I would like to Barter.
    Splendid! #NPC
    ->Barter
* What do you like? 
    Not much 'sides the news! #NPC
    ** How interesting...
    -> END
* Nevermind.
    -> END

=== Barter ===
NULL_LINE #Barter
-> END

=== NoBarter ===
Sorry, I have nothing new to give you. #NPC
-> END