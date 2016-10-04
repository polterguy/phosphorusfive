System42's CMS page loader
========

This folder contains the Hyperlambda scripts, necessary to actually _load_ your files, according to whatever logic you wish for
to occur during this process. See the [page-editor](/core/p5.webapp/system42/apps/cms/page-editor/) folder for more details
about how this work.

Notice, most of these templates expects a template that contains at the very least a literal with the innerValue of {header}, ID
of "header" - And a container widget with the innerValue of {content} and the ID of "content". What happens to these, are dependent
upon what the "specialized page loader" does with your page, during loading.

