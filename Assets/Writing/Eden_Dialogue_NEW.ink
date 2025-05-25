VAR eden_bartered = false
VAR IC_new_robes = false
VAR IC_cornucopia = false
VAR IC_religious_relic = false

// Unique NPC Starting Logic
-> Start

// There is a max of 4 choices per choice option
// If you need more feel free to add a "more" option that diverts to more optuins
=== Start ===
Greetings, wanderer. How may we of the Order be of assistance to you? #NPC
*I would like to Barter. 
    {eden_bartered: -> NoBarter | -> Barter} // Only necessary for Unique NPCs
*Can I ask you something?
    What is peaking your curiosity? #NPC
    ->Asking
*Nevermind.
    ->END

==Asking==
*What do you do?
    I pray for the safety and longevity of every soul. #NPC
    ->Souls
*What is your favorite fruit?
    I've always enjoyed kiwis, they are small, fuzzy, and sweet. #NPC
    ->Asking
*What are you looking for?
    The Church requires a symbol of abundance, a relic that we had lost, and something to replace these old, dirty robes. #NPC
    ->Asking

==Souls==
*Soul?
    Yes, the 'fruit of the soul,' as we call it here. The organic matter that powers our cores. #NPC
    ->Souls
*Do you usually just wait here?
    Not usually. I often run errands or listen to the people of the district. I am here uring a rare rest period. #NPC
    **Rare?
    ... It must be done. Are you on rest too? What is work like for you? #NPC
    ->Work
*I want to talk about something else.
    So it shall be. #NPC
    ->Start
    
==Work==
*I'm an archaeologist.
    An archaeologist? I assume you have heard of the disbandment of the archaeology program at VCSC?
    **I am all too familiar.
        Had you worked there? My sincerest condolences for your program. We will be praying for you all. #NPC
        ***We appreciate it.
        ->Souls
*I was a teacher.
    Was? Oh my... you worked at VCSC then? I've heard about the unfair closing of several programs. #NPC
    **You know?
        Well yes, several residents have returned after their program was cut. They have asked for our prayers since then. #NPC
        ***That's... terrible.
            You are affected by them too, you are in our prayers as well professor. #NPC
            ****Thank you Eden. 
            ->Souls
*It's... complicated.
    I pray that times get better for you then. #NPC
    ->Souls


=== Barter ===
Expell all greed. #NPC
NULL_LINE #Barter // This line is necessary as barter starts the instant you move to the line with the tag
-> END

==BarterWin==
{shuffle:
- May fresh fruit be in your tidings. #NPC
- Please do not hesitate to reach out to the Order again. May the Harvest bless you. #NPC
- Patience is a virtue. Enjoy the spoils of this trade. #NPC
}
-> END

==BarterFail==
I will pray for your bartering skills to improve. #NPC
-> END

=== NoBarter ===
The Church can offer no more. #NPC
-> END

