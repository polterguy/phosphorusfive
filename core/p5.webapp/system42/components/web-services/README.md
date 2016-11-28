Lambda Web Services
===============

A lambda web service is a web service which is capable of evaluating some arbitrary piece of lambda object, and returning the results of the 
evaluation back to the client.

Normally, a web service endpoint contains a piece of logic/function, and the client provides the data for the function to use as it evaluates. In a
_"lambda web service"_, the responsibilities are inversed. Meaning, the client supplies both the input data, and the logic/lambda/function the web service
endpoint should evaluate. This has several advantages. Among other things, it allows you to have a much more flexible web service, to such an extent,
that you at least in theory, only need _one_ web service endpoint for your server. Simply, because this single web service endpoint, can provide any
needs, all possible clients could possibly have.

This has, as far as I know, never been implemented before. Simply because of that allowing some arbitrary client, to execute some arbitrary piece of code,
on your server, simply creates too many security risks. A client could for instance delete your entire database, or inject some piece of malware function
into your server, etc. However, when you implement lambda web services with Hyperlambda, you can evaluate the specified lambda object, within the context
of a *[whitelist]* invocation. This allows you to create a sub-vocabulary of Active Events, which is a whitelist of Active Events the client is allowed
to execute on your server.

If you create your whitelist, in such a way that you exclusively allow safe Active Events to be invoked, then this is just as secure as any other web services.
Simply because, if a client tries to inject or execute dangerous events on your server, the execution will be rejected, and a security exception will be raised.
Below is an example of how to create a web service endpoint, which only allows for some basic keyword Active Events, in addition to *[select-data]* to be
invoked.

```
sys42.utilities.evaluate-web-service-invocation
  _whitelist
    set
    add
    insert-before
    insert-after
    eval-x
    return
    select-data
```

If you create a new CMS/lambda page in System42, and paste in the code above, and make sure your page's URL is _"/ws"_, you can invoke your web service 
with the following code, assuming your server is running on _"localhost"_ on port _"1176"_.

```
p5.net.http-post:"http://localhost:1176/ws"
  Content-Type:application/x-hyperlambda
  content
    select-data:x:/*/*/p5.page
    set:x:/@select-data/*/*(/lambda|/html)
    insert-before:x:
      src:x:/@select-data/*
```

The above invocation of our web service, will return something resembling the following code.

```
p5.net.http-post
  result:"http://localhost:1176/ws"
    status:OK
    Status-Description:OK
    Content-Type:application/x-hyperlambda; charset=utf-8
    Last-Modified:date:"2016-11-28T09:16:25.796"
    Server:Microsoft-IIS/10.0
    Transfer-Encoding:chunked
    X-SourceFiles:=?UTF-8?B?QzpccHJvamVjdHNccGhvc3Bob3J1c2ZpdmVcY29yZVxwNS53ZWJhcHBcd3M=?=
    Cache-Control:private
    Date:"Mon, 28 Nov 2016 07:16:25 GMT"
    Set-Cookie:ASP.NET_SessionId=v1pvzwh4grwxetcmtnqa15j1; path=/; HttpOnly
    X-AspNet-Version:4.0.30319
    X-Powered-By:ASP.NET
    content
      _success
        p5.page:/
          type:html
          name:Home
          template:/system42/apps/CMS/page-templates/default.hl
          role:
        p5.page:/app-loader
          name:App loader
          role:root
          type:lambda
        p5.page:/ws
          role:guest
          name:WS
          template:/system42/apps/CMS/page-templates/no-navbar-menu.hl
          type:lambda
```

Notice the *[content]* node above, containing the results of our invocation. The above is a very good example of why you'd need lambda web services in fact,
since it strips away both any *[html]* nodes, in addition to any *[lambda]* nodes, creating a much smaller response from the server, consuming less bandwidth.

In the above example, the client is not interested in the actual content of the *[p5.page]* objects, but only their meta information. Hence, returning the
actual content, would be a waste of bandwidth, both for the client, and the server.

Notice, if you tried to execute an Active Event which is not in the *[_whitelist]* definition of your web service _"page"_, you would raise an exception
on the server's endpoint. Try to evaluate the following code for instance, to see this in action.

```
p5.net.http-post:"http://localhost:1176/ws"
  Content-Type:application/x-hyperlambda
  content
    delete-data:x:/*/*
```

The above invocation will be rejected by your web service, simply since its *[_whitelist]* definition does not allow it to execute the *[delete-data]* event.
The return value from the server in such cases would resemble the following.

```
p5.net.http-post
  result:"http://localhost:1176/ws"
    status:InternalServerError
    Status-Description:Internal Server Error
    Content-Type:application/x-hyperlambda; charset=utf-8

    /* ... Rest of HTTP headers ... */

    content
      _error
        message:Caller tried to invoke illegal Active Event [delete-data] according to [whitelist] definition
        type:p5.exp.exceptions.LambdaSecurityException
        stack-trace:@".lambda
  delete-data:x:/*/*<<====================== [ERROR!!]"
```

If a web service invocation returns *[_success]* as the root node beneath *[content]*, it was successfully evaluated. If it returns *[_error]*, it raised
an exception, and the details of the exception, can be found inside of the *[_error]* node. If an exception is raised, it will also return _"InternalServerError"_
as its HTTP *[status]*, instead of _"OK"_, which it returns if invocation is successful.

## Ninja tricks

The above *[_whitelist]*, puts much trust in that your p5.data database exclusively contains things you'd like to share with the entire world. If you wish,
you could instead of whitelisting *[delete-data]*, whitelist a custom Active Event, which restricts the types of objects to select from the database, to whatever 
you would like to share with the entire world.

Below is an example of creating an Active Event, that exclusively allows the user to select *[p5.page]* objects, in addition to *[sys42.app-settings]*.

```
create-event:sys42.samples.select-data
  switch:x:/../*/_arg?value
    case:p5.page
      select-data:x:/*/*/p5.page
      insert-before:x:/../0
        src:x:/@select-data/*
    case:sys42.app-settings
      select-data:x:/*/*/sys42.app-settings
      insert-before:x:/../0
        src:x:/@select-data/*
    default
      throw:Sorry, I can't allow you to do that ...
```

If you evaluate the above Hyperlambda in your Executor, and then create another web service, and give it the URL of _"ws2"_, with the following code.

```
sys42.utilities.evaluate-web-service-invocation
  _whitelist
    set
    add
    insert-before
    insert-after
    eval-x
    return
    sys42.samples.select-data
      select-data
      switch
      case
      default
```

Then you would end up with a safer version of our original web service, that only allows the user to select either *[p5.page]* objects, and/or *[sys42.app-settings]*
objects form the database.

Notice, the above *[_whitelist]* definition, allows the *[select-data]* event to be raised, in addition to some other events, but exclusively from inside 
your *[sys42.samples.select-data]* Active Event. This has to be done, since the *[whitelist]* that your web service is being evaluated from within, changes your 
ApplicationContext, and will be passed into also Active Events. Meaning, unless we explicitly allow for *[select-data]* to be allowed inside of *[sys42.samples.select-data]*, 
then as we reach our *[select-data]* inside our *[sys42.samples.select-data]* invocation, then this will raise an exception.

To invoke the above web service, you could use something resembling the following.

```
p5.net.http-post:"http://localhost:1176/ws2"
  Content-Type:application/x-hyperlambda
  content
    sys42.samples.select-data:p5.page
    set:x:/@sys42.samples.select-data/*/*(/lambda|/html)
    insert-before:x:
      src:x:/@sys42.samples.select-data/*
```

The latter web service example would probably be highly more safe than the example we started out with, since it restricts the objects clients can select from the database,
to only two different types of objects. Notice,if you completely trust a specific Active Event, you can whitelist its entire body, with the following code instead. Replace
the lambda content of your _"ws2"_ page with the following.

```
sys42.utilities.evaluate-web-service-invocation
  _whitelist
    set
    add
    insert-before
    insert-after
    eval-x
    return
    sys42.samples.select-data
      *
```

