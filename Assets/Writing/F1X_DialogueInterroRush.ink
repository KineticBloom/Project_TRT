// Global Variables. Please put these at the top.
// Flag variable. Can be any name you want. Default false
VAR late_in_loop = false

// Inventory Card Variable (Info Card or Item Card). Default false
// Name NEEDS to have "IC_" before the name. It'd be best if you keep this consistent with the item/info card name.
// Programmers will make sure this name matches the card in game. 
VAR IC_EDEN_EMPTY_COFFERS_REVEAL = false
VAR IC_F1X_NAME_REVEAL = false
VAR IC_F1X_DEBT_REVEAL = false
VAR IC_F1X_MAILORDER_REVEAL = false

->Intro

=== Intro ===
Hello. #NPC
->Start

=== Start ===
* Who are you?
    I am F1X the merchant. #NPC
    ~ IC_F1X_NAME_REVEAL = true
    -> Start

* {late_in_loop} Time has passed. You are acting guilty. Tell me the truth.
    I used to be a criminal.. and I was in significant debt until recently. #NPC
    ~ IC_F1X_DEBT_REVEAL = true
    -> Start

* {IC_F1X_DEBT_REVEAL and IC_EDEN_EMPTY_COFFERS_REVEAL} You were in debt. Eden sent you all of the church's money. What did you do to earn such payment?
    I received a large sum of money, a flash drive, and instructions to make a mail order.I convinced the mailbot to complete a... <i>unique kind</i> of delivery. #NPC
    ~ IC_F1X_MAILORDER_REVEAL = true
    -> Start
    
* That's all. // Exit dialogue
    ~ late_in_loop = true
    -> END



