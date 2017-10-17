
Creating PDF files with Phosphorus Five
===============

This folder contains the Active Events necessary to create PDF files from HTML. There is one Active Event in this project,
that allows you to create a PDF document from an HTML snippet. Its name is *[html2pdf]*. Below is an example of using it.

```
_html:@"<h2>Hello, World</h2>
<p>This is <strong>bold</strong> text. And this is <em>italics</em>.</p>
<p>This is another paragraph!</p>"
html2pdf:~/temp/my-pdf-files.pdf
  src:x:/@_html?value
```

## CSS support

Optionally, you can supply one or more **[css-file]** arguments to this Active Event, which will use the CSS files when creating the PDF.

