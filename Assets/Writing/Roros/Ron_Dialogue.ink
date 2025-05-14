->Intro

==Intro==
“Hi :]” #NPC
->Dialogue

==Dialogue==
*I would like to barter.
    Great! #NPC
    ->Barter
*Can I ask you something?
    What!? #NPC
    ->Questioning
*Nevermind.
    Ok! #NPC
    ->END
    
==Barter==
NULL_LINE #Barter
->END

==Questioning==
*What is your favorite fruit?
    Banana :]. #NPC
    ->Questioning
*What do you do?
    Work :]. #NPC
    ->Questioning
*Let's talk business.
    Cool. #NPC
    ->Dialogue