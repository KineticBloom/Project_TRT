->Intro

==Intro==
“Nice to meet you! Care to see my wares?” #NPC
->Dialogue

==Dialogue==
*I would like to barter.
    Splendid! #NPC
    ->Barter
*Can I ask you something?
    What's up!? #NPC
    ->Questioning
*Nevermind.
    Oh, alright! #NPC
    ->END
    
==Barter==
NULL_LINE #Barter
->END

==Questioning==
*What is your favorite fruit?
    Watermelon. It's very refreshing on a hot day. #NPC
    ->Questioning
*What do you do?
    I’m a social event manager. #NPC
    ->Questioning
*Let's talk business.
    Naturally. #NPC
    ->Dialogue