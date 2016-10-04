System42's CMS page editor(s)
========

This folder contains the Hyperlambda scripts, necessary to edit your CMS pages. By default, System42's CMS comes with two pages
pre-installed; HTML and "lambda" pages. The latter allow you to evaluate any piece of Hyperlambda as your page is loaded.

If you wish to create your own custom "page type", you'll need to at the very least create three files. A "new-page-template"
file, matching the *[type]* declaration of your *[p5.page]* objects, which serves as the "default new file" template.
A "specialized-editors" file, also matching your *[type]*, which becomes the CMS editor, loaded as users are editing their
pages.

The third and most important file you'll need to create, is a "page-loader" file, which should be put 
into [page-loader](/core/p5.webapp/system42/apps/cms/page-loader/) folder, and is responsible for "loading" the page, from its
associated *[p5.page]* database object.
