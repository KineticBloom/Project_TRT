VAR fp_bartered = false

Greetings, wanderer. How may we of the Order be of assistance to you? #NPC
* I would like to Barter. 
    {fp_bartered: ->NoBarter | ->Barter}
* Nevermind.
    -> END

=== Barter ===
Very well, you must purge all envy. We, the Order, believe in charity once proven worthy. #NPC
NULL_LINE #Barter
-> END

=== NoBarter ===
Apologies, we have already reached an agreement. #NPC
-> END