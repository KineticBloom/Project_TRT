VAR mailbot_bartered = false

Hello and welcome. It’s Dead Mail day. How can the City Packet Service serve you today? #NPC
* I would like to Barter. 
    {mailbot_bartered: ->NoBarter | ->Barter}
* Nevermind
    -> END

=== Barter ===
Many parties want this lot. Convince me, please. #NPC
NULL_LINE #Barter
-> END

=== NoBarter ===
You have already convinced me. #NPC
-> END