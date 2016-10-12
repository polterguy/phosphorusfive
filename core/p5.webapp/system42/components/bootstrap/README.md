Bootstrap CSS
===============

This directory contains the Bootstrap CSS framework, together with some supporting Active Events.
For documentation, check out [Bootstrap dox](http://getbootstrap.com/css/)

Some components and apps in System42 are dependent upon this module to work correctly. Check the documentation
for your specific components, before attempting to remove it.

To include Bootstrap CSS on your page, simply invoke *[sys42.bootstrap.include-css]* to include the CSS. If you use some of
the JavaScript features from Bootstrap (such as using modal windows, etc) - Then you must also invoke *[sys42.bootstrap.include-javascript]*
to include the JavaScript from Bootstrap. (hint; The menu uses JavaScript for instance. Also the Modal Windows from Bootstrap)


