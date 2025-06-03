->Intro

==Intro==
Encantado de conocerlo. ¿Quiere ver mi mercancía? #NPC
->Dialogue

==Dialogue==
*I would like to barter.
    Espléndido #NPC
    ->Barter
*Can I ask you something?
    Qué pasa!? #NPC
    ->Questioning
*Nevermind.
    Oh, está bien! #NPC
    ->END
    
==Barter==
NULL_LINE #Barter
->END

==Questioning==
*What is your favorite fruit?
    ¡Tomate!. #NPC
    **Isn't that a vegetable?
        Suficientemente cerca, licenciado... #NPC
        ->Questioning
*What do you do?
    Soy ingeniero de invernaderos. #NPC
    ->Questioning
*Let's talk business.
    naturalmente. #NPC
    ->Dialogue