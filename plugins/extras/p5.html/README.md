Parsing and creating HTML in Phosphorus Five
===============

This project contains the necessary Active Events to handle HTML. You can parse HTML, and create a lambda object out of it, which you modify,
and later create HTML of again if you wish. This makes it very easy to traverse and transform HTML, both HTML snippets, and entire HTML pages.
Below is an example of how to create a lambda structure from HTML, change some of its properties, and create HTML again of the changed lambda.

```
_data:@"<div class=""some-css-class"">Howdy world! This is <strong>strong</strong> text</div>"
html2lambda:x:/-?value
set:x:/-/*/*/\@class?value
  src:ANOTHER-css-class
lambda2html:x:/-2/*
```

The above lambda, will produce the following result. Notice how the class attribute of our div HTML element was changed in our final result.
Also notice that attributes are rendered with an alpha character (@), which implies you need to escape their names when referencing them in 
lambda expressions.

```
_data:@"<div class=""some-css-class"">Howdy world! This is <strong>strong</strong> text</div>"
html2lambda
  div
    @class:ANOTHER-css-class
    #text:"Howdy world! This is "
    strong
      #text:strong
    #text:" text"
set:x:/-/*/*/\@class?value
  src:ANOTHER-css-class
lambda2html:@"<div class=""ANOTHER-css-class"">Howdy world! This is <strong>strong</strong> text</div>"
```

When you transform HTML to lambda, using the **[html2lambda]** Active Event, then the lambda created, will contain a **[#text]** node for every
text fragment inside an element. Every attribute from your HTML, will be created as a child node, having a name, starting with _"@"_. All children HTML
elements of another HTML element, will be created as children nodes, of the parent element's node, with the name of the node being the name of the element.
This structure allows P5 to perfectly re-create the original HTML from the lambda structure it creates from your HTML.

## Example usage

Some of its uses, is to among other things, retrieve HTML documents from for instance an HTTP get request, over the web, for then to retrieve only all
hyperlinks from the document fetched. Imagine this.

```
.result
p5.http.get:"http://reddit.com"
html2lambda:x:/-/*/result/*/content?value
add:x:/../*/.result
  src:x:/./-/**/a/*/\@href?value
set:x:/../*/.result/*(!/=~http)
set:x:/../*!/../*/.result
```

The above lambda, will retrieve the HTML document at reddit.com, transform it to lambda object, extract all hyperlinks, and return them as
a collection in **[.result]**. To clarify the result value, it will remove all nodes from itself after evaluation, except the **[.result]** node itself.
The end result being you get a list of all hyperlinks from reddit.com

## Supporting Active Events

In addition to the two HTML transformation Active Events above, there are also some supporting Active Events in this library.

* __[p5.html.html-encode]__, encodes a piece of HTML, replacing "<" with "&lt;" etc
* __[p5.html.html-decode]__, does the opposite of the above
* __[p5.html.url-encode]__, URL encodes a piece of string with %xx notation
* __[p5.html.url-decode]__, does the opposite of the above
