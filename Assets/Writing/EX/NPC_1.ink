->Intro

=== Intro ===
I want [key_card] and will give you [item_1] #NPC
->Start

=== Start ===
* I would like to Barter. 
    -> Barter
* Nevermind. // Exit dialogue
    -> END
    
=== Barter ===
Sure, why not. #NPC
NULL_LINE #Barter // This line is necessary as barter starts the instant you move to the line with the tag
-> END

// Required Knots for NPCs
=== BarterWin === 
Well done #NPC
-> END

=== BarterLose ===
Get lost #NPC
-> END