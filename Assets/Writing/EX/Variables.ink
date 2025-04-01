// Global Variables. Please put these at the top.
// Flag variable. Can be any name you want. Default false
VAR Solar_for_IC_4 = false
// Inventory Card Variable (Info Card or Item Card). Default false
// Name NEEDS to have "IC_" before the name. It'd be best if you keep this consistent with the item/info card name.
// Programmers will make sure this name matches the card in game. 
VAR IC_item_1 = false
VAR IC_item_2 = false
VAR IC_item_3 = false
VAR IC_item_4 = false
VAR IC_item_5 = false
VAR IC_item_6 = false
VAR IC_info_card_1 = false
VAR IC_info_card_2 = false
VAR IC_key_card = false
VAR IC_facility_usb = false

->Intro

=== Intro ===
Welcome to the best decision you'll make all day. #NPC
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