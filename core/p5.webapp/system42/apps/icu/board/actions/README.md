ICU Actions
========

Plugin directory for "actions". Contains all the different action plugins that a click
on an [icu.board] "key" can trigger.

An "action" consists of two items, a description or friendly name [_action-name] and
an [_action-creator] which is a lambda object, responsible for creating the action,
given [_image] - ID to [icu.image], [_board] - ID to [icu.board] we're creating action
for and [_x] and [_y], which defines the position for the key on the board to which
the action is to be associated with.


