->Intro

==Intro==
Greeting and salutations feline! What commodities are in your possesion? Anything robust in your repetoire? #NPC
->Dialogue

==Dialogue==
*I would like to barter.
    Magnificent! #NPC
    ->Barter
*Can I ask you something?
    I have the time. #NPC
    ->Questioning
*Nevermind.
    Heh, window shoppers... #NPC
    ->END
    
==Barter==
NULL_LINE #Barter
->END

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