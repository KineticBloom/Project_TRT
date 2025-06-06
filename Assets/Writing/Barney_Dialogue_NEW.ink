
VAR barney_bartered = false
VAR IC_letter = false
VAR IC_name_tag = false
VAR IC_duct_tape = false
//Is there any way that we can keep track of if a trade happened? Like if the merchant has or hasn't received a certain item?

// Unique NPC Starting Logic
-> INTRO
==INTRO==
Hoot. Hello and welcome. The dead mail day shop is open. #NPC
-> Start

=== Start ===
*I would like to Barter. 
    {barney_bartered: -> NoBarter | -> Barter} // Only necessary for Unique NPCs
+Can I ask you something?
    What would you like to know? #NPC
    ->Asking

*Nevermind // Exit dialogue
    -> END
    
==Asking==
+Dead Mail Day?
        Correct! Undelivered mail is sold on Sundays. #NPC
        ->Dead_Mail
+What is your favorite fruit?
        The fruit in me! It lets me work around the clock. #NPC
    ->Asking
+What are you looking for?
    I am looking for any tools, something to replace my old name tag, and any news that may be lying around! #NPC 
    //If we can break all of these lines into their own textbox thatd be awesome possum :)
    ->Asking
+I want to talk about something else.
    Assuredly. #NPC
    ->Start
    
==Dead_Mail==
*How does mail go undelivered?
    Incorrect address. No return address. Things such as that. My job is to barter them off. #NPC
    ->Dead_Mail
*So you just stand here all day?
    Quite an overexaggeration! I stand here 8-4 sharp every Sunday. #NPC
    ->Dead_Mail
*Is that all you do?
    I-er what? No. I'm a mailbot. I deliver mail too. I just do this on Sundays. #NPC
    **Gotcha.
        What of you? What do you do for work, companion? #NPC
        ->Work
*I want to talk about something else.
    Assuredly. #NPC
    ->Start

==Work==
*I'm an archaeologist.
    Hm, you did seem familiar. Pri-v8 right? I read about you during my 30 minute mandated break a month ago. Sorry to hear about the budget cuts at the university. #NPC
    **Is that what they're calling it?
        Oh, the university? I of course meant VCSC, Virtual College of So- #NPC
        ***[I know where I worked.] I meant the budget cuts. 
            Is there an issue? Should I refer to it as something else? #NPC
            ****Money's just going in the wrong hands.
                I see. My regards to you and your students Professor V8. #NPC
                *****Thanks Barney. 
                ->Dead_Mail
    **Thanks, for the concern. I'll be fine.
    ->END
*I am, well... was... a teacher.
    Hm, you did seem familiar. Pri-v8 right? I read about you during my 30 minute mandated break a month ago. Sorry to hear about the budget cuts at the university. #NPC
    **Is that what they're calling it?
        Oh, the university? I of course meant VCSC, Virtual College of So- #NPC
        ***[I know where I worked.] I meant the budget cuts. 
            Is there an issue? Should I refer to it as something else? #NPC
            ****Money's just going in the wrong hands.
                I see. My regards to you and your students Professor V8. #NPC
                *****Thanks Barney. 
                ->Dead_Mail
*It's... complicated.
    I will not pry more then, back to business. #NPC
    ->Dead_Mail

=== Barter ===
Package identified. Calculating shipping costs... Zero. #NPC
NULL_LINE #Barter
-> END

==BarterWin==
The City Packet Service hope you enjoy your package #NPC
->END

==BarterFail
{shuffle:
- No deal ...And clean up these packing peanuts, please. #NPC
- That will not do. Please offer equal or greater value. #NPC
}
->END

=== NoBarter ===
We're out of stock. Please return next Sunday. #NPC
-> END