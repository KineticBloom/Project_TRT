VAR Sig_for_CI = false
VAR tutorial_bartered = false
VAR tutorial_talked = false
VAR IC_scrap_paper = false
VAR IC_autograph = false
VAR already_stopped = false

{
    - tutorial_bartered: -> Post_Barter
    - tutorial_talked: -> Paper_Waiting
    - else:  -> First
}

==First==
E-excuse me, a-a-are you… Pri-V8? Like THE archaeologist Pri-V8??? :plead: #NPC
*Why are you talking like that?
    I’m just… really nervous because ur sooooooo famous >_< #NPC 
    ->Mainline
*Who is asking?
    I’m just a really really REALLY big fan of ur work Mr.V8 ^_^!! #NPC
    ->Mainline 
*Sure am.
    Omg my friends r gunna FREAK!!!! #NPC
    ->Mainline

==Mainline==
*What’re you looking for anyways?
    C-could I have an autograph plz :point_right: :point_left: #NPC
    **Only if you stop talking like that.
        Yippie!!!!!!!!! ahem... I-i mean... Yes sir! :salute: #NPC
        ->Awkward
    **Fine.
        Yippie!!!!!!!! #NPC
        ->Awkward
    **Sure, what the hell.
        Yippie!!!!!!!! #NPC
        ->Awkward
        
==Awkward==
~tutorial_talked = true
Erm >_<... This is sooooo awkward... I don't have anything to sign x( Can you find some paper for me plz? #NPC
*{IC_scrap_paper} I've got paper already.
    ~IC_scrap_paper = false
    ~IC_autograph = true
    ->Barter
*Sure can for a fan.
    Ur even awesomer IRL XoX! #NPC
    ->END
*If it'll get you to leave.
    U won't hear from me again. Promise! #NPC
    ->END
*Fine.
    YAHOOO!!!!!!!! #NPC
    ->END
    
==Paper_Waiting==
Got papuh? x3 #NPC
*{IC_scrap_paper} Got it right here.
    ~IC_scrap_paper = false
    ~IC_autograph = true
    -> Barter
*Not yet.
    U should find some :3 #NPC
    ->END

==Barter==
Radical! Let's trade! #NPC
    **Trade?
        Oh! That’s just how things work here. Barter for everything. So... I'll give you something! #NPC
        ***Everybody does that?
            Just about! Much nicer and personable this way :grinning: #NPC
            ****Right...
            Anyways! Barter? Yeah? Barter? Yeah? :3 #NPC
NULL_LINE #Barter
->END

==Post_Barter== // Thanks kinda breaks
*Thanks.
    Whatchya doin here anyway? :3c #NPC
    **I’m investigating a lead for an artifact here. Heard anything?
        Woag… that’s so cool… I’ve heard some people talk about it :O. U should talk with some people here. They might know. #NPC
        ***Anything else to know?
            We R a bartering society! Everything gets traded here, like what we just did! Even if it’s just info, it’s got a price. #NPC
            ****Where's a good place to start?
                Well you can talk to other roros like me. Our deals are pretty cheap. Or talk to some of our renowned merchants. Get to know who they are, what they want. #NPC
                *****Thanks for letting me know. See you around.
                    No thank YOU! Everyone is gonna be so jealous :mischevious: #NPC
                    ->END
                ***** Alright, get lost kid.
                    Erm, ok :frown: Thanks for the autograph again Mr. V8. #NPC
                    ->END

==Barter_Fail==
C'mon! I just want an autograph :[ #NPC
->END

==Wait_Up==
{
    -tutorial_talked: ->Wait_Up_3
    -already_stopped: ->Wait_Up_2
}
No way... Are you...?! #NPC
P-please come over here! I just have to talk to you! #NPC
~already_stopped = true
->END

==Wait_Up_2==
W-wait! #NPC
Please, just a brief moment of your time! #NPC
->END

==Wait_Up_3==
Where are you going? #NPC
* Uh... Looking for paper.
    You won't find any over there silly #NPC
    ** Haha... Yeah...
        ->END
* Nowhere...
    I hope you find some paper soon! #NPC
    ** Me too...
        ->END