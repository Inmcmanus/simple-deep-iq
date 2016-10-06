# simple-deep-iq
A simple implementation of Deep IQ, a basic "opponent" to test MtG decks against. Based on modified tables from the original, found here http://www.wooberg.net/deep-iq.html

The program reads in the rollable tables of any size and number (By default I've just done the 6 tables in the original) and a token table (which tokens roll on when they are created for keywords). The program is otherwise pretty straightforward. It just rolls a new effect from the table each turn - managing itself and advancing to new tables - and tells the player if they need to make any choices, such as "Sacrifice your best creature".

It's not intended to be a very challenging or clever opponent, simply one that can provide a little pressure. The player makes any choices that DeepIQ needs to make, choosing the worst option for themselves. In my experience, DeepIQ is a good matchup for limited decks.
