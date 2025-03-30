// Global Variables. Please put these at the top.
// Flag variable. Can be any name you want. Default false
VAR seen_empty_coffers = false

// Inventory Card Variable (Info Card or Item Card). Default false
// Name NEEDS to have "IC_" before the name. It'd be best if you keep this consistent with the item/info card name.
// Programmers will make sure this name matches the card in game. 
VAR IC_EDEN_EMPTY_COFFERS_REVEAL = false
VAR IC_EDEN_LOVE_LETTER_REVEAL = false
VAR IC_EDEN_NAME_REVEAL = false
VAR IC_love_letter = false

->Intro

=== Intro ===
Hello. #NPC
->Start

=== Start ===
* Who are you?
    I am Eden the fruit priestess. #NPC
    ~ IC_EDEN_NAME_REVEAL = true
    -> Start

* {IC_love_letter} I found this love letter. It is from you. Who is it to?
    I am in love with F1X. If you want, you can give my love letter to the mailbot if you GO THROUGH  THE GATE ON THE RIGHT ([eventually this should be a specific clue on how to find mailbot]).  #NPC
    ~ IC_EDEN_LOVE_LETTER_REVEAL = true
    -> Start

* COMMAND: Update SEEN_EMPTY_COFFERS to TRUE (after seeing mailbot)
    ~ seen_empty_coffers = true
    -> Start
    
* {seen_empty_coffers} The church coffers are empty. Why?
    I followed the instructions of the star fruit and sent all of the church’s money to F1X. #NPC
    ~ IC_EDEN_EMPTY_COFFERS_REVEAL = true
    -> Start
    
* That's all. // Exit dialogue
    -> END



