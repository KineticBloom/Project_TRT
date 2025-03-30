// Global Variables. Please put these at the top.
// Flag variable. Can be any name you want. Default false


// Inventory Card Variable (Info Card or Item Card). Default false
// Name NEEDS to have "IC_" before the name. It'd be best if you keep this consistent with the item/info card name.
// Programmers will make sure this name matches the card in game. 
VAR IC_B4RN3Y_OBSESSION_REVEAL = false
VAR IC_B4RN3Y_MALWARE_MAIL_REVEAL = false
VAR IC_B4RN3Y_NAME_REVEAL = false
VAR IC_F1X_MAILORDER_REVEAL = false

->Intro

=== Intro ===
Hello! Do you know a fact about me? I only talk to people who know a fact about me. #NPC
* You're the mail bot?
    That's half a fact!  #NPC
    Oh? What was I missing?
    I am not defined by my work. You were missing the name half of me. My name is B4RN3Y the mailbot! Good to meet you! #NPC
    Good to meet you.
    ... #NPC
    Does that mean I can't talk to you?
    Yep! Can't talk to me! Since you didn't know a fact about me. Nice try though! Bye bye! #NPC
    ~ IC_B4RN3Y_NAME_REVEAL = true
    -> END
* {IC_B4RN3Y_NAME_REVEAL} You're B4RN3Y the mailbot.
    Absolutely true! How can I help you? #NPC
    ->Start
    
* {IC_B4RN3Y_OBSESSION_REVEAL} You're obsessed with sending mail.
    You know me so well! Which is strange because we have never met... How can I help you? #NPC
    ->Start

=== Start ===
* What is another fact about you?
    I love sending mail. Sometimes a little too much!  #NPC
    ~ IC_B4RN3Y_OBSESSION_REVEAL = true
    -> Start

* {IC_B4RN3Y_OBSESSION_REVEAL and IC_F1X_MAILORDER_REVEAL} F1X['s <i>unique</i> delivery?] said she had you send out a <i>unique</i> delivery. What was it? 
    I was given a strange executable that I sent out in a wave to every machine in a 3-lightyear radius. #NPC
    ~ IC_B4RN3Y_MALWARE_MAIL_REVEAL = true
    Ever since, that old facility up the road has been covered in blinking red lights day and night. I swear the blinking is speeding up, too. (might give secret mail-runner key or tell about a secret path to reach facility) #NPC
    -> Start
    
* That's all. // Exit dialogue
    -> END



