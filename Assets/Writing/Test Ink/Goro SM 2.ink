VAR goro_sm_2_interrogated = false
VAR goro_sm_2_FQ = false
VAR SAVE_sm_ec_3 = false
VAR IC_wooden_shark_tooth = false

{
    - goro_sm_2_FQ: -> FetchQuestDone
    - goro_sm_2_interrogated: -> Interrogated
    - else: ->Start
}

=== Start ===
Heh heh heh. #NPC
* What's up?
    ->Accost
* I'll ignore the crazy Roro.
    -> END

=== Accost ===
Nothing... Special... Heh heh heh. #NPC
* Are you sure? It sounds VERY special.
    -> NotFree
* Maybe a little <i>persuasion</i> will loosen your lips.
    Whoa, whoa! No need for violence #NPC
    -> NotFree

===NotFree===
~goro_sm_2_interrogated = true
How about a deal, heh? #NPC
My secret for a hmm... [Wooden Shark Tooth]. #NPC
How about it? #NPC
* Well, I guess I can find that
    -> END
* {IC_wooden_shark_tooth} Oh, I have one
    Well, well, well. Ever the prepared. #NPC
    -> Interrogated

===Interrogated===
Hand it over. #NPC
* {IC_wooden_shark_tooth} Here ya go
    ~IC_wooden_shark_tooth = false
    Heh, pleasue doin buisness #NPC
    ** The info?
        ~SAVE_sm_ec_3 = true
        Tch #NPC
        Of course, we had a deal. #NPC
        -> FetchQuestDone
* It?
    The [Wooden Shark Tooth]. #NPC
    We have an open deal if you wish to close it. #NPC
    ** Ohh, right... I'll get back to you
        ->END
* I need more time.
    ->END

=== FetchQuestDone ===
~~goro_sm_2_FQ = true
I found out that F1x is in love with Eden! #NPC
Crazy right? #NPC
They'd never admit it openly, but if they had some sort of confession #NPC
Like a <b> note </b> inviting them to a <b> date </b>, a [Date Note] one could say, #NPC
then they'd probably give anything for it. #NPC
* How interesting...
    Interesting indeed #NPC
    Heh heh heh heh heh #NPC
    -> END