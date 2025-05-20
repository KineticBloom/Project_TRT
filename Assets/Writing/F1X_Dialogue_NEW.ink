
VAR fix_bartered = false
VAR fix_distrust = false
VAR IC_note = false
VAR IC_tie = false
VAR IC_clock = false
//Is there any way that we can keep track of if a trade happened? Like if the merchant has or hasn't received a certain item?

// Unique NPC Starting Logic
->Start

=== Start ===
Welcome to the best decision you'll make all day. #NPC
*I would like to Barter. 
    {fix_bartered: -> NoBarter | -> Barter} // Only necessary for Unique NPCs
*Can I ask you something?
    Sure, might cost ya extra though. #NPC
    ->Asking
*Nevermind // Exit dialogue
    -> END
    
==Asking==
*What do you do for work?
    What we're doin' right now. Barterin' with the locals. #NPC
    ->Work
*What is your favorite fruit?
    Mangos... Always interested in that fruit inside ya' too. #NPC
        **I like mangoes too.
            Good choice. #NPC
            ->Asking
        **What did you say?
            I'm not sure what you're referin' to. #NPC
            ->Asking
*{fix_distrust == false} What are you looking for?
    I've been needin' a new tie, and... you ain't gonna tell no one nothing right? #NPC
    ->fixandeden
    //If we can break all of these lines into their own textbox thatd be awesome possum :)
    ->Asking
* I want to talk about something else.
    Assuredly. #NPC
    ->Start
    
==fixandeden==
*What're you talking about?
    Yes or no cat? #NPC
    **I can't promise that.
        Then I ain't tellin' ya. #NPC
        ~fix_distrust = true
        ->Start
    **I won't tell anyone.
        ->SoGay
*I guess not?
    Be straight with me. #NPC
    **I won't then.
        ->SoGay
*I won't.
    To the point, I like that. #NPC
    ->SoGay

==SoGay==
If you find anything that could help me get to know Eden. I'd pay back handsomely. #NPC
    *Eden?
        About yay short, works at the church, antlers... #NPC
        **I'll keep an eye out.
            'Preciate it. #NPC
            ->Start
==Work==
Howz about you? #NPC
*Just an archaeologist nowadays.
    Just? What happened to ya? #NPC
    **[I used to teach.] Program got cut by the school.
        Tch, classic. VCSC I assume? #NPC
        ***You haven't read about it?
            I've just heard about who's workin' up top there. Can't say I'm surprised. #NPC
            ->Start
        
=== Barter ===
Didn't get this cheaply, ain't gonna sell it cheaply. #NPC
NULL_LINE #Barter
-> END

==BarterWin==
{shuffle:
- Don't let curiosity kill ya'! #NPC
- I guess cats do always land on their feet, eventually... #NPC
- You got some nice parts on ya', let me know if any are for sale next time. #NPC
}
-> END

==BarterFail
{shuffle:
- I didn't scare you off did I? Hehehe. #NPC
- Were ya' distracted? I can't say I'd blame you. #NPC
- Window shoppers... Won't find what I've got anywhere else! #NPC
}
-> END


==NoBarter==
Why dontcha come back another time? I'm all out. #NPC
->END