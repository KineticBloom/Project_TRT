VAR roro_c_bartered = false

{roro_c_bartered: ->NoBarter | ->Start}

=== Start ===
Hello, would you like to participate in community service? #NPC
* Sure
    -> Barter
* Community Service?
    -> Service
* Nah, I'm good.
    Have a great day! #NPC
    -> END

=== Service ===
Yeah, community service! #NPC
You've probably noticed how there's a lot of litter around the city #NPC
So I've decided to set up this little program to incentivize people to pick up litter #NPC
If you give me 4 items, I'll give you something nice! #NPC
How about it?
* Sure
    -> Barter
* Maybe another time.
    Have a great day! #NPC
    -> END

=== Barter ===
Great! Hand me 4 items and I'll give you something nice.
NULL_LINE #Barter
-> END

=== NoBarter ===
Thank you for your service! #NPC
I don't have anything else today, come back tomorrow!
-> END