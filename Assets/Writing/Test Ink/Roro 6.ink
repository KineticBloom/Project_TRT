VAR roro_6_bartered = false

{roro_6_bartered: ->NoBarter | ->Start}

=== Start ===
Oh no, I can't find it! #NPC
* I would like to Barter. 
    ->Barter
* Is something troubling you?
    Huh? Oh I just misplaced my [Book of Hymns]. #NPC
    I need it tomorrow as I'm leading the service. #NPC
    Argh, I'd give anything to get another copy. #NPC
    ** How interesting...
    -> END
* Nevermind.
    -> END

=== Barter ===
NULL_LINE #Barter
-> END

=== NoBarter ===
How did the first hymn go again? #NPC
* Let's leave them be
-> END