VAR goro_SM_1_interrogated = false
VAR SAVE_sm_ec_2 = false
VAR IC_id_card = false

=== Start ===
Oh hello there! #NPC
Wanna know something interesting? #NPC
* Uh, sure...
    ->Question
* No.
    Have a good day!
    -> END

=== Question ===
Well, here's a fun fact about F1x. #NPC
Despite their looks, F1x is a very professional merchant. #NPC
They never go without a suit and tie. #NPC
They've been looking for a [Brand New Tie] recently! #NPC
~SAVE_sm_ec_2 = true
* Good to know
    Heh, you know it. #NPC
    -> END