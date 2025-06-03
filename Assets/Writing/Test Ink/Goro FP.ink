VAR goro_fp_interrogated = false
VAR goro_fp_FQ = false
VAR SAVE_fp_ec_3 = false
VAR IC_banana = false

{
    - goro_fp_FQ: -> FetchQuestDone
    - goro_fp_interrogated: -> Interrogated
    - else: ->Start
}

=== Start ===
I can't belive that Eden... #NPC
* What about Eden?
    ->Accost
* Nevermind.
    -> END

=== Accost ===
H-huh! N-nothing... #NPC
I don't know anything! #NPC
* Are you sure? It sounds like you do.
    Well...
    -> NotFree
* Maybe a little <i>persuasion</i> will help you remember
    Whoa, whoa! Calm down #NPC
    -> NotFree

===NotFree===
~goro_fp_interrogated = true
I can't just tell you! #NPC
It's real secret stuff. #NPC
* C'mon get to the point!
    Ok, ok! #NPC
    -> Want
* What do you want?
    -> Want

===Want===
If you could get me an [Banana Bunch] then perhaps I can help you. #NPC
I won't settle for anything else! #NPC
* Well, I guess I'll find that
    -> END
* {IC_banana} Oh, I have one
    Really?! #NPC
    -> Interrogated

===Interrogated===
Did you get an [Banana Bunch]? #NPC
* {IC_banana} Here ya go
    ~SAVE_fp_ec_3 = true
    ~IC_banana = false
    Oh great! #NPC
    -> FetchQuestDone
* I'll come back
    ->END

=== FetchQuestDone ===
~goro_fp_FQ=true
What I know is that Eden is in love with F1x. #NPC
Any item of theirs is priceless! #NPC
An [Apple Ring] would do the trick! #NPC
Now leave me alone! #NPC
-> END