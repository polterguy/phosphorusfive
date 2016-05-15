ICU Actions
========

Plugin directory for "ICU actions". Contains all the different actions that a click on an 
[icu.board] "key" can trigger.

An "action" consists of two items, a description or friendly name [_action-name] and
an [_action] which is a lambda object, which is the action occurring when key is clicked.
[_action] is given [_image] - ID to [icu.image], [_board] - ID to [icu.board] action belongs
to, and [_x] and [_y], which defines the position for the key on the board to which the 
action is to be associated with.



