## Chapter 1, An Ajax primer

This chapter will introduce you to the concept of Ajax, and more importantly _"Managed Ajax"_. Managed Ajax is at the
core of Phosphorus Five, and what allows for you to create rich _"single page web apps"_, while only focusing on one
language - **Hyperlambda**.

**Notice**, if you don't understand this chapter's content, don't worry and just keep on reading. Everything will
become clear before you have finished the chapter.

### Defining Ajax

Ajax is a technology for incrementally applying changes to an HTML page. It is an acronym, although
commonly written _"Ajax"_. It means _“Asynchronous JavaScript and XML”_, and its name is actually
highly misleading, since it rarely have anything to do with XML. Today, most implementations will use JSON
instead of XML, so a more correct name would be _"Ajaj"_.

Ajax allows us to change parts of the view in our web pages, by fetching data from a server, and use this
data to modify parts of what the user is seeing on his page. Ajax is built around a simple idea, which is the
ability to use the *"XHR object"*, or the *"XML HTTP Request object"*. The XHR
object allows you to asynchronously retrieve data from a server. This combined with the ability to dynamically
modify the DOM, or *"Document Object Model"*, allows for the experience of *"interactive web pages"*. DOM is
what your HTML is creating when it is being evaluated by your browser.

Creating an Ajax web app is extremely difficult, since it assumes knowledge of HTTP, JavaScript, JSON, CSS,
HTML, DOM, some server side programming language, often several different libraries, written in different
programming languages, etc, etc, etc. Managed Ajax in Phosphorus Five only require you to learn one
thing; **Hyperlambda**.

### Managed Ajax, a significantly simplified solution

Managed Ajax implies that the use of JavaScript for the most parts are _optional_. With P5, you do
not need to use JavaScript at all to create rich interactive Ajax web pages. You _can_ use JavaScript if you want to,
and using JavaScript is surprisingly easy in combination with P5. However, if you do not want to use JavaScript,
you can still create highly rich and interactive web apps. The idea with Managed Ajax, is to encapsulate and
_"hide"_ JavaScript, to create a better code-model, that automatically takes care of the client to server side
Ajax mapping and HTML/DOM rendering. Below is an example of some Hyperlambda, that creates a server side Ajax
request, and shows an _"information window"_, with the current date and time _from the server_.

```hyperlambda-snippet
/*
 * Gets the server's date and time
 */
p5.types.date.now

/*
 * Creates an "information bubble" window on
 * the client.
 */
micro.windows.info:The server claims it's {0}
  :x:/@p5.types.date.now?value
```

No JavaScript was written to have the above example work. It relies upon the _"Managed Ajax"_ parts
of Phosphorus Five to invoke server-side Hyperlambda, and create a Managed Ajax widget on the client
displaying the result. This makes P5 a perfect beginner's Ajax framework, significantly simplifying the process
of creating rich Ajax applications. While also, due to its extensibility - A perfect framework for the
seasoned architect and software developer. In a traditional Ajax framework, creating something such as the above,
would often require 50-100 lines of code, in a multitude of different technologies and programming
languages, to simply have your client create an Ajax request towards the server, and displaying the date and
time of day.

**Definition**, [Managed Ajax](https://msdn.microsoft.com/en-us/magazine/mt826343), is the ability to create
Ajax applications, that automatically creates Ajax requests, while hiding or _"encapsulating"_ its internal
implementation.

### Managed Ajax, a better code model

Managed Ajax also implies that it becomes much harder to create security holes, since all the code
is evaluated on the server - And the HTML and JavaScript parts, are automatically created for you on the client side.
In addition, a code model where you only have to think about one concept, makes your code more easy to maintain -
Contrary to a pure JavaScript solution, where you often have to apply change _"all over the place"_, to change parts
of your app. This makes it easier to create secure apps, while retaining a better architecture, with less dependencies,
resulting in solutions that are more easily maintained.

Managed Ajax allows you to modify any *"widget"* on your page, from any event raised by any other *"widget"*. This
facilitates for a significantly simpler development model for applying changes to your page. When creating a P5
web app, you will in fact _"feel"_ as if you are developing a Desktop Windows app, due to the above mentioned features.
In fact, creating a highly rich Ajax app in P5, is probably _easier_ than creating something similar in WinForms,
Xamarin forms, Qt, etc.

Managed Ajax also _significantly reduces the bandwidth requirements_. Compare the total page load bandwidth usage
of this application towards for instance Twitter.com or GMail.com - And you'll see that there is at least
_"one order of magnitude"_ less bandwidth consumption in Phosphorus Five, compared to literally anything out there.
This makes your pages in general more responsive and faster, and makes it possible for you to create much richer
web apps, than a traditional Ajax framework allows you to do.

### The flip side

The disadvantage of using Managed Ajax, is that the state for your pages needs to be retained on the server side.
This consumes _significantly more memory_ per user/request than a traditional Ajax solution. This
results in that Managed Ajax does not scale as well as a traditional solution.
Due to this problem, Phosphorus Five is very well suited for creating _"enterprise"_ types of apps, with few
simultaneous users, requiring a rich user interface - And _not_ so well suited for creating websites that
requires scaling to hundreds of thousands, and sometimes millions of consecutive users.

I often tend to define Phosphorus Five and Hyperlambda as a _"fifth generation programming language"_, implying
it has the advantages of _"fourth generation programming languages"_, but also the disadvantages. Making it
arguably the _"FoxPro for web development"_. If you're creating web apps for your company, or enterprise apps,
requiring hundreds to some few thousand of simultaneous users, you should probably consider Managed Ajax.
If you're creating websites for the general public, Managed Ajax should probably not be your first choice.

### Summing up

To illustrate the difference, realise that if you wanted to implement our above date/time example in C#, you would
have to spend probably decades of training, to learn the following constructs.

* C#
* OOP
* Design Patterns
* HTTP
* HTML
* DOM
* XML
* CSS
* JavaScript
* JSON
* XHR
* Security constructs such as SSL, TLS and public key cryptography
* ++ dozens of frameworks and libraries, probably written in F#, VB.NET and Boo ...

And your code would easily end up being somewhere between 50 to 500 lines log. With Managed Ajax the above list
is reduced to ...

* __Hyperlambda__ - Which you can learn in a couple of hours ...

... and the code is **3 lines**!

Still, you can easily dive into all of the above, while using Managed Ajax and Hyperlambda to tie together all of
your C#/HTTP/HTML/JavaScript/etc constructs. The learning curve for creating Ajax apps is reduced from a decade to
a couple of hours.

https://phosphorusfive.files.wordpress.com/2018/03/26zca4.jpg