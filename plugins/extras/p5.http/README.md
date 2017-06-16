HTTP REST from Hyperlambda
===============

This folder contains the Active Events necessary to invoke HTTP REST WebServices. It contains full support for all four basic HTTP verbs.

* [GET]
* [POST]
* [PUT]
* [DELETE]

To retrieve a document, using HTTP GET for instance, you could do something like the following.

```
p5.http.get:"http://google.com"
```

The above will return something similar to this.

```
p5.http.get
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
p5.http.get:x:/-/*?value
```

You will have one *[result]* node returned, for each URL you supply to it. You can also supply any HTTP headers you wish, as illustrated below.

```
p5.http.get:"https://httpbin.org/get"
  Foo-Bar:Some data goes here
src:x:/../**/content?value.string
```

There are 4 basic Active Events in this project.

* [p5.http.get] - HTTP GET - Returns document
* [p5.http.post] - HTTP POST - Posts data
* [p5.http.put] - HTTP PUT - Puts data
* [p5.http.delete] - HTTP DELETE - Deletes data

In addition to the above Active Events, there are 3 additional public events.

* [p5.http.get-file] - Retrieves a document, and saves it to a specified file, without loading it into memory
* [p5.http.post-file] - Posts a file, without loading it into memory
* [p5.http.put-file] - Puts a file, without loading it into memory

## POST and PUT

The POST and PUT events, automatically recognize Hyperlambda, and allows you to transmit a lambda node structure, 
without first creating text from it. To transmit a piece of code to another server, you could use something like the 
following for instance.

```
p5.http.post:"https://httpbin.org/post"
  content
    _data
      no1:Thomas
      no2:John
src:x:/../**/content?value.string
```

If you wish to POST or PUT simple content, you can do such a thing with something resembling the following.

```
p5.http.post:"https://httpbin.org/post"
  content:"foo bar"
src:x:/../**/content?value.string
```

## POST'ing and PUT'ing files

If you have a big file you wish to POST or PUT, you can achieve it using the following syntax.

```
p5.http.post-file:"https://httpbin.org/post"
  filename:/application-startup.hl
src:x:/../**/content?value.string
```

Exchange the above invocation to *[p5.http.put-file]* if you wish to use PUT the file instead.

The above _"put-file"_ and _"post-file"_ invocations, will not read the files into memory, before they're transmitted to your REST endpoint. But rather,
copy the stream directly from disc to the request stream. This allows you to transfer huge files, without exhausting your server's resources.

## GETting files

If you instead want to retrieve a document using HTTP GET, and save it directly to disc, without loading it into memory, you can use *[p5.http.get-file]*.
Consider the following code.

```
p5.http.get-file:"https://google.com"
  filename:~/documents/private/foo-google.txt
```

The above code will download google.com's index document, and save it to your private documents folder.

## Ninja tricks (Hyperlambda Web Services)

Due to the extreme dynamic nature of Hyperlambda, you can easily transmit Hyperlambda, over for instance an HTTP POST request, to have it evaluated on 
another server, and then return it to caller as Hyperlambda. Consider creating the following CMS/lambda page, that reads the body of your request, 
and evaluates it as Hyperlambda, for then to return the result to caller. To create such a page, you could do something like the following.

```
// Retrieves the HTTP POST request body.
p5.web.request.get-body

// Evaluates the request body as Hyperlambda.
eval:x:/@p5.web.request.get-body

// Converts the return value from the evaluated Hyperlambda to Hyperlambda code.
lambda2hyper:x:/@eval/*

// Making sure we return it with the correct Content-Type, such that
// invoker of Web Service can more easily recognize it as Hyperlambda.
p5.web.header.set
  Content-Type:application/x-hyperlambda

// Returning the Hyperlambda to caller.
p5.web.echo:x:/@lambda2hyper?value
```

Make sure you set the page's _"Role"_ to _"guest"_ in its _"Settings"_, and that you set its URL to _"/invisible-my-service"_. By starting your page's URL
with _"/invisible-"_, you make sure it doesn't show up in the navbar or menu.

Then evaluate the following code, assuming your web server is listening on port 8080.

```
p5.http.post:"http://localhost:8080/invisible-my-service"
  Content-Type:application/x-hyperlambda
  content
    _data
      no1:Thomas
      no2:John
    for-each:x:/-/*?value
      eval-x:x:/+/*/*/*
      add:x:/../*/return
        src
          p5.web.widgets.create-literal
            parent:content
            position:0
            element:h3
            innerValue:x:/@_dp?value
    return
eval:x:/../**/content
```

After evaluating the above HTTP POST request, the return value should look something like the following.

```
p5.http.post
  result:"http://localhost:1176/invisible-my-service"
    status:OK
    Status-Description:OK
    Content-Type:application/x-hyperlambda; charset=utf-8

    /* ... more HTTP headers ... */

    content
      p5.web.widgets.create-literal
        parent:content
        position:0
        element:h3
        innerValue:Thomas
      p5.web.widgets.create-literal
        parent:content
        position:0
        element:h3
        innerValue:John
eval
```

To understand the beauty of the above construct, realize that what we actually did, was to transmit Hyperlambda to another server, for then to have
that server evaluate the Hyperlambda, allowing the caller's code to decide what to return after evaluation. In theory, this makes it possible for you
to create _one single Web Service endpoint_, for every single Web Service needs you can possibly have.

### Warning!!

The above construct, allows anyone to evaluate any piece of Hyperlambda on your server, which of course is an extremely dangerous security risk, effectively
opening up your server entirely for any arbitrary piece of code, anyone wants to execute on it.

If you combine the above construct, with the PGP cryptography and cryptographically signed features from the [p5.mime](/plugins/extras/p5.mime/) project,
you can require that the client invoking your web service, is trusted, by only allowing requests from a list of pre-declared trustees, and requiring them
to cryptographically sign their MIME messages in a PGP mime/multipart. Still, you would need to be 100% confident in that the client's private PGP key has
not somehow been compromised, and that you can trust the client transmitting the Hyperlambda.

There are ways to further refine this, and increase the security, by requiring the client to only supply a sub-set of Active Events, through using 
e.g. the *[eval-whitelist]* Active Event when evaluating the incoming and returned lambda. Please see the *[eval-whitelist]* Active Event for details
about this. You can find the *[eval-whitelist]* event in the [p5.lambda](/plugins/p5.lambda) project.

