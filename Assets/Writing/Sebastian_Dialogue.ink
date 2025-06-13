VAR goro_mb_interrogated = false
VAR SAVE_mailbot_ec_3 = false
VAR IC_bottle_of_wine = false

->Intro

==Intro==
Greeting and salutations feline! What commodities are in your possesion? Anything robust in your repetoire? #NPC
->Dialogue

==Dialogue==
* {goro_mb_interrogated} Could you remind me again?
    -> Info
* {!goro_mb_interrogated} What interests you?
    -> Quest
+Can I ask you something?
    I have the time. #NPC
    ->Questioning
+Nevermind.
    Heh, window shoppers... #NPC
    ->END
    
==Barter==
NULL_LINE #Barter
->END

==Quest==
Why only the finest [Bottle of Wine] would interest me. #NPC
You wouldn't happen to have some upon your person, would you? I happen to know a lot about this city and it's residents within. #NPC
Oh just the other day I heard that B4rn3y... Oops, can't just say that to any outsider. I could be convinced, however. #NPC
* {IC_bottle_of_wine} I happen to have a bottle, here
    ~IC_bottle_of_wine = false
    My, how generous! For this, my good sir, I'll provide you with some information. #NPC
    -> Info
* Unfortunately, I do not.
    Why that's a shame. #NPC
    -> Dialogue

==Info==
~goro_mb_interrogated = true
~SAVE_mailbot_ec_3 = true
Poor B4n3y lost his nametag the other day in a tragic mail accident. It was so horrible, so much so that I'll spare you the details. #NPC
Ever since then, he's been looking for a [Laminated Name Tag]! I do hope he can feel whole once again. #NPC
* Ah... I hope so to...
    -> END

==Questioning==
*What?
    Perplexed? Confounded? Mystified by my verbose language? Many are… many are. Hahaha! #NPC
    ->Questioning
*What is your favorite fruit?
    The finest yield in all of the land, grapes! #NPC
    ->Questioning
*What do you do?
    You must be new in town, since we are not acquaintances. My name is Sebastian Telfar-Verdi, the most illustrious sommelier of the region. #NPC
    **So you just drink?
        To reduce my occupation to such layman's terms, especially considering your intentional obtuseness, is something I’ve come to expect. Many cannot comprehend that which is required by a sommelier of my reputation. Perhaps one day you may tour local vineyards, and cultivate a stronger understanding of my profession. #NPC
        ->Questioning
*Let's talk business.
    Finally. #NPC
    ->Dialogue