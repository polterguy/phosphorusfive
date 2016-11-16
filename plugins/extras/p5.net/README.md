HTTP REST from Hyperlambda
===============

This folder contains the Active Events necessary to invoke HTTP REST WebServices. It contains full support for all four basic HTTP verbs.

* [GET]
* [POST]
* [PUT]
* [DELETE]

To retrieve a document, using HTTP GET for instance, you could do something like the following.

```
p5.net.http-get:"http://google.com"
```

The above will return something similar to this.

```
p5.net.http-get
  result:"http://google.com/"
    status:OK
    Status-Description:OK
    Content-Type:text/html; charset=ISO-8859-1
    Last-Modified:date:"2016-11-16T13:44:57.581"

    /* ... the rest of the HTTP headers returned from google.com ... */

    content:@"... document from google.com ..."
```

The actual document, will be returned as *[content]*, inside the above *[result]* node.

If you wish, you can retrieve multiple documents at the same time, by supplying an expression to your invocation. Example is given below.

```
_urls
  url1:"http://google.com"
  url2:"http://digg.com"
p5.net.http-get:x:/-/*?value
```

You will have one *[result]* node returned, for each URL you supply to it. You can also supply any HTTP headers you wish, as illustrated below.

```
p5.net.http-get:"https://httpbin.org/get"
  Foo-Bar:Some data goes here
set:x:?value
  src:x:/../**/content?value.string
```

There are 4 basic Active Events in this project.

* [p5.net.http-get] - HTTP GET - Returns document
* [p5.net.http-post] - HTTP POST - Posts data
* [p5.net.http-put] - HTTP PUT - Puts data
* [p5.net.http-delete] - HTTP DELETE - Deletes data

In addition to the above Active Events, there are 3 additional public events.

* [p5.net.http-get-file] - Retrieves a document, and saves it to a specified file, without loading it into memory
* [p5.net.http-post-file] - Posts a file, without loading it into memory
* [p5.net.http-put-file] - Puts a file, without loading it into memory

And if you are using C#, you can find 3 additional _"protected"_ events.

* [.p5.net.http-get-native]
* [.p5.net.http-post-native]
* [.p5.net.http-put-native]

The three latter events, are exclusively for C# invocations, and beyond the scope of this documentation. The POST and PUT events, automatically recognize
Hyperlambda, and alloas you to transmit a lambda node structure, without first creating text from it. To transmit a piece of code to another server, you could
use something like the following for instance.

```
p5.net.http-post:"https://httpbin.org/post"
  content
    _data
      no1:Thomas
      no2:John
```

## Ninja tricks (Hyperlambda Web Services)

Due to the extreme dynamic nature of Hyperlambda, you can easily transmit Hyperlambda, over for instance an HTTP POST request, to have it evaluated on 
another server, and then return it to caller as Hyperlambda. Consider creating the following CMS/lambda page, that reads the body of your request, 
and evaluates it as Hyperlambda, for the to return the result to caller. To create such a page, you could do something like the following.

```
// Retrieves the HTTP POST request body.
get-request-body

// Evaluates the request body as Hyperlambda.
eval:x:/@get-request-body

// Converts the return value from the evaluated Hyperlambda to Hyperlambda code.
lambda2hyper:x:/@eval/*

// Making sure we return it with the correct Content-Type, such that
// invoker of Web Service can more easily recognize it as Hyperlambda.
set-http-header
  Content-Type:application/x-hyperlambda

// Returning the Hyperlambda to caller.
echo:x:/@lambda2hyper?value
```

Make sure you set the page's _"Role"_ to _"guest"_ in its _"Settings"_, and that you set its URL to _"/invisible-my-service"_. By starting your page's URL
with _"/invisible-"_, you make sure it doesn't show up in the navbar or menu.

Then evaluate the following code, assuming your web server is listening on port 1176.

```
p5.net.http-post:"http://localhost:1176/invisible-my-service"
  Content-Type:application/x-hyperlambda
  content
    _data
      no1:Thomas
      no2:John
    for-each:x:/-/*?value
      eval-x:x:/+/*/*/*
      add:x:/../*/return
        src
          create-literal-widget
            parent:content
            position:0
            element:h3
            innerValue:x:/@_dp?value
    return
eval:x:/../**/content
```

After evaluating the above HTTP POST request,the return value should look something like the following.

```
p5.net.http-post
  result:"http://localhost:1176/invisible-my-service"
    status:OK
    Status-Description:OK
    Content-Type:application/x-hyperlambda; charset=utf-8
    Last-Modified:date:"2016-11-16T14:32:50.047"
    Server:Microsoft-IIS/10.0
    Transfer-Encoding:chunked
    X-SourceFiles:=?UTF-8?B?QzpccHJvamVjdHNccGhvc3Bob3J1c2ZpdmVcY29yZVxwNS53ZWJhcHBcaW52aXNpYmxlLW15LXNlcnZpY2U=?=
    Cache-Control:private
    Date:"Wed, 16 Nov 2016 12:32:50 GMT"
    Set-Cookie:ASP.NET_SessionId=r35jzc0j4iepthtravit4pxv; path=/; HttpOnly
    X-AspNet-Version:4.0.30319
    X-Powered-By:ASP.NET
    content
      create-literal-widget
        parent:content
        position:0
        element:h3
        innerValue:Thomas
      create-literal-widget
        parent:content
        position:0
        element:h3
        innerValue:John
eval
```

To understand the beauty of the above construct, realize that what we actually did, was to transmit Hyperlambda to another server, for then to have
that server evaluate the Hyperlambda, allowing the caller's code to decide what to return after evaluation. In theory, this makes it possible for you
to create _one single Web Service endpoint_, for every single Web Service needs you can possibly have.

Warning!
The above construct, allows anyone to evaluate any piece of Hyperlambda on your server, which of course is an extremely dangerous security risk, effectively
opening up your server entirely for any arbitrary piece of code, anyone wants to execute on it.

If you combine the above construct, with the PGP cryptography and cryptographically signed features from the [p5.mime](/plugins/extras/p5.mime/) project,
you can require that the client invoking your web service, is trusted, by only allowing requests from a list of pre-declared trustees, and requiring them
to cryptographically sign their MIME messages in a PGP mime/multipart. Still, you would need to be 100% confident in that the client's private PGP key has
not somehow been compromised, and that you can trust the client transmitting the Hyperlambda.

There are ways to further refine this, by whitewashing the Hyperlambda, only allowing a subset of keywords/events to be evaluated, and so on. But this
is beyond the scope of this documentation. For server you trust though, the above Web Service logic, is really quite spectacular, and something I think
Hyperlambda is the only programming language on the planet that can easily achieve.

