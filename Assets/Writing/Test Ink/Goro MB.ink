VAR goro_mb_interrogated = false
VAR SAVE_mailbot_ec_3 = false
VAR IC_id_card = false

=== Start ===
Oh hello there! #NPC
May I help you? #NPC
* Help me how?
    ->Question
* No.
    Have a good day!
    -> END

=== Question ===
Well, here's a fun fact about B4rn3y. #NPC
B4n3y lost his nametag the other day in a tragic mail accident. #NPC
He's been looking for a [Laminated Name Tag] since! #NPC
~SAVE_mailbot_ec_3 = true
* Good to know
    Happy to help! #NPC
    -> END