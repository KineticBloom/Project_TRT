VAR roro_5_bartered = false

{roro_5_bartered: ->NoBarter | ->Start}

=== Start ===
Ugh, how dare they. #NPC
* I would like to Barter.
    ->Barter
* Is something troubling you?
    Hmm? Oh it's nothing really. #NPC
    It's just that people have been littering our [Church Pamphlets] everywhere! #NPC
    It's frustrating seeing them on the floor like that! #NPC
    Unfortunately, I'm too occupied standing here tending to the pews to pick them up. #NPC
    I'd give anything to retrieve those pamphlets. #NPC
    ** That sounds rough
        -> END
* Nevermind.
    -> END

=== Barter ===
NULL_LINE #Barter
-> END

=== NoBarter ===
Thanks for your help! #NPC
* No problem
    -> END