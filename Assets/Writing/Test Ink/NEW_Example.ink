// Global Variables. Please put these at the top. Note that these will reset to default value each loop.
VAR npc_name_bartered = false // Variable to check if you've bartered with this NPC. Default false
VAR flag_variable = false // Flag variable. Can be any name you want. Default false
// The following variables are special and need their prefix to work
VAR IC_item = false // Item Card Variable. Name is ID of item card. Default false
VAR NUM_counter = 0 // Counter variable. Basically just a number. Default 0.
// Saved Variables. Variables saved between loops, but reset on new game. 
// Any of the above variables can be a saved variable as long as they are prefixed with "SAVE_"
VAR SAVE_flag = false
VAR SAVE_NUM_count2 = 0

// Roro Starting Logic
// Start the story if they haven't been bartered with
// {npc_name_bartered: -> Start | -> NoBarter}

// Unique NPC Starting Logic
-> Start

// There is a max of 4 choices per choice option
// If you need more feel free to add a "more" option that diverts to more optuins
=== Start ===
This is the starting dialogue #NPC
* I would like to Barter. 
    {npc_name_bartered: -> NoBarter | -> Barter} // Only necessary for Unique NPCs
* Hello
    Hello #NPC
    -> END
* I have more questions
    Cool, shoot. #NPC
    ** How do you exist?
        I dunno. How do you exist? #NPC
        -> END
    ** Why do you exist?
        I dunno. #NPC
        -> END
* Nevermind // Exit dialogue
    -> END

// Required Knots for NPCs
// Recommended knot just to keep track of where bartering starts
// Only use #Barter in NPC scripts. Required to have tag in NPC scripts.
=== Barter ===
Witty line NPC says before barter #NPC
NULL_LINE #Barter // This line is necessary as barter starts the instant you move to the line with the tag
-> END

=== NoBarter ===
Sorry, I don't have anything else. #NPC
-> END

