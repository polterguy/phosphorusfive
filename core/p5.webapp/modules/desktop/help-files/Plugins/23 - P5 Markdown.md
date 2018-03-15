
## p5.markdown - Markdown parsing

This folder contains the Active Events necessary to parse Markdown snippets. There is one Active Event in this
project, that allows you to parse an Markdown snippet, and create an HTML snippet out of it. Its name is
**[markdown2html]**. Below is an example of using it.

```hyperlambda
_md:@"## Hello, World

This is **bold** text. And this is *italics*.

This is another paragraph!"
markdown2html:x:/-?value
```

### URL resolving

Optionally, you can supply a **[root-url]** to this Active Event, which will resolve all _relative_ URLs to the
specified URL, adding your specified root-url as the root-url.
