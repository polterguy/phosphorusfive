Parsing and creating HTML in Phosphorus Five
===============

This project contains the necessary Active Events to handle HTML. You can parse HTML, and create a lambda object out of it, which you modify,
and later create HTML of again, if you wish. This makes it very easy to traverse and transform HTML, both HTML snippets, and entire HTML pages.

Below is an example of how to create a p5.lambda structure from HTML, change some of its properties, and create HTML again of the changed lambda.

```
_data:@"<div class=""some-css-class"">Howdy world! This is <strong>strong</strong> text</div>"
html2lambda:x:/-?value
set:x:/-/*/*/@class?value
  src:ANOTHER-css-class
lambda2html:x:/-2/*
```

The above p5.lambda, will produce the following result. Notice how the "class" attribute of our div HTML element was changed in our final result.

```
_data:@"<div class=""some-css-class"">Howdy world! This is <strong>strong</strong> text</div>"
html2lambda
  div
    @class:ANOTHER-css-class
    #text:"Howdy world! This is "
    strong
      #text:strong
    #text:" text"
set:x:/-/*/*/@class?value
  src:ANOTHER-css-class
lambda2html:@"<div class=""ANOTHER-css-class"">Howdy world! This is <strong>strong</strong> text</div>"
```

When you transform HTML to lambda, using the *[html2lambda]* Active Event, then the p5.lambda created, will contain the *[#text]* node for every
text fragment inside an element. Every attribute from your HTML, will be created as a child node, having a name, starting with "@". All children HTML
elements of another HTML element, will be created as children nodes, of the parent element's node, with the name of the node being the name of the element.
This structure allows P5 to perfectly re-create the original HTML from the p5.lambda structure it creates from your HTML.

## Example usage

Some of its uses, is to among other things, retrieve HTML documents from for instance an HTTP get request, over the web, for then to retrieve only all
hyperlinks from the document fetched. Imagine this.

```
_result
p5.net.http-get:"http://reddit.com"
html2lambda:x:/-/*/result/*/content?value
add:x:/../*/_result
  src:x:/./-/**/a/*/@href?value
set:x:/../*/_result/*(!/~http)
set:x:/../*!/../*/_result
```

The above p5.lambda, will retrieve the HTML document at reddit.com, transform it to p5.lambda, extract all hyperlinks, and return them as *[_result]*. 
To clarify the result value, it will remove all nodes from itself after evaluation, except the *[_result]* node. The end result being you get a list of 
all hyperlinks from reddit.com

## Supporting Active Events

In addition to the two HTML transformation Active Events above, there are also some supporting Active Events in this library.

* [html-encode], encodes a piece of HTML, replacing "<" with "&lt;" etc
* [html-decode], does the opposite of the above
* [url-encode], URL encodes a piece of string with %xx notation
* [url-decode], does the opposite of the above


