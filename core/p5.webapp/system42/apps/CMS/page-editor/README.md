System42's CMS page editor(s).
========

This folder contains the Hyperlambda, necessary to edit your CMS pages. By default, System42's CMS comes with two page types
pre-installed; HTML and _"lambda"_ pages. The latter allow you to evaluate any piece of Hyperlambda as your page is loaded.

If you wish to create your own custom page type, you'll need to at the very least create three files. A _"new-page-templates/"_
file, matching the *[type]* declaration of your *[p5.page]* objects, which serves as the default new file template.
A _"/specialized-editors/"_ file, also matching your *[type]*, which becomes the CMS editor, loaded as users are editing their
pages. The third and most important file you'll need to create, is a _"/page-loader/"_ file, which should be put 
into [page-loader](../page-loader/) folder, and is responsible for loading the page, from its associated *[p5.page]* database object.

If you do the above, you can create your own, page types object, allowing you to run any custom logic, both upon editing and loading
of your page objects.
