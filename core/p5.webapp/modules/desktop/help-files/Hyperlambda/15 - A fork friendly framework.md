## A fork friendly framework

Linguistics are funny, especially when you start perceiving them from a poetic point of view. I like to think that the poetic and linguistic
ambiguity of P5 is one of its most interesting qualities. In this chapter, we will create a generic and reusable CRUD _"database"_ layer,
which stores its items in the internal _"p5.data"_ database. This allows us to accommodate for future change, without breaking the rules
of *YAGNI*.

**Definition**; YAGNI means *"You Ain't Gonna Need It"*, and implies you should never implement something you don't need immediately. This 
important design principle, often contradicts another important design principle of OOP - Which is that you should expect change. Hence, 
the art of balancing YAGNI, with the design principles necessary to accommodate for future change - Often seems to be an impossible task.

I often refer to this oxymoron's insanity, by stating how according to the rules of *"SOLID programming"* (Google it) - The perfect class has 
no public fields, methods, or properties, it cannot be instantiated, and is 100% *"encapsulated"* - Which of course translates into, that the 
perfect class is the one that *doesn't exist*.

### How Hyperlambda solves this

With Hyperlambda, balancing this line however, becomes significantly easier. Simply because, we always have *"Yet Another Layer Of Abstraction"* 
at our disposal. Which is another design principle, often referred to as *"YALOA"*. The reasons for this is, because in P5, polymorphism 
is *implicitly given*, for any lambda object - Due to that there are no semantic differences between any two lambda objects that can possibly 
exist. Hence, any lambda object, is at least in theory, substitutable by any other lambda object. This results in that it becomes impossible 
to violate the *"Liskov Substitution Principle"* in Hyperlambda (Google it). In Hyperlambda, the entire Liskov Substitution Principle 
hence becomes *obsolete*. The dilemma of balancing YAGNI with future potential change requirements is non-existent in Hyperlambda.
Resulting in that with Hyperlambda, you can easily accommodate for future change, _without_ breaking YAGNI.

In Hyperlambda, you can always inject another layer of lambda, in between any existing piece of lambda, and any other existing piece of
lambda evaluating the first. This allows you to modify an existing system, having *"anything intercept anything"*. This allows you to
change your entire system, without even removing a single lambda object, having your new logic, invoke the old logic, without any problems.
Which of course results in that you can easily fork the system, and completely change its behavior, without removing or changing a single 
line of code in it.

With that in our mind, let's create a 100% perfectly generic CRUD database layer, that permanently solves (in theory), all our future 
CRUD issues.

## Code

We will be creating 4 Active Events. These events will have the following names.

* __[examples.data.create]__
* __[examples.data.read]__
* __[examples.data.update]__
* __[examples.data.delete]__

By never using the P5 database directly, but always going through the above Active Events - We can entirely change the underlaying database 
implementation later, without breking any of our apps, as long as we are a little bit careful. This makes a lot of sense, since _"p5.data"_ scales
terribly, since it keeps all its data in memory.

We are going to create these Active Events in 4 different Hyperlambda files, and store our files in our *"/modules/hello-world/"* folder from
one of our first chapters. This ensures that our Active Events are re-created every time our web server process for some reasons is restarted. 
Below is the content of all of these 4 files listed. Then from our _"startup.hl"_ file, we will use **[micro.evaluate.file]** to evaluate these 
4 files.

**examples.data.create.hl**

```hyperlambda
/*
 * Our "create" wrapper for our CRUD operations.
 * Stores all arguments into our database, as the type 
 * specified by our main argument [_arg].
 */
create-event:examples.data.create

  // Adding all arguments, except [_arg].
  add:x:/../*/insert-data/*
    src:x:(/./--!/./--/_arg)/<-

  // Changing the name of [type] to reflect value of [_arg].
  set:x:/../*/insert-data/*?name
    src:x:/../*/_arg?value

  // Inserting data into database.
  insert-data
    type
```

**examples.data.read.hl**

```hyperlambda
/*
 * Our "read" wrapper for our CRUD operations.
 * Expects [start], [end] and a main argument.
 * The main argument [_arg] is the type you're interested in.
 */
create-event:examples.data.read

  // Making sure we provide default [start] and [end] values.
  if:x:/../*/start
    not
    insert-before:x:/../0
      src:"start:0"
  if:x:/../*/end
    not
    insert-before:x:/../0
      src:"end:10"

  // Actual select invocation.
  select-data:x:/*/*/{0}/[{1},{2}]
    :x:/../*/_arg?value
    :x:/../*/start?value
    :x:/../*/end?value

  /*
   * Deleting [start] and [end] arguments, 
   * to prevent them from being returned from event, since they
   * might have been injected as defaults, making our event return
   * them as a part of its "result".
   */
  set:x:/../*(/start|/end)
  return:x:/@select-data/*
```

**examples.data.update.hl**

```hyperlambda
/*
 * Our "update" wrapper for our CRUD operations.
 * Expects [id] argument being the ID of object to update.
 * Also expects a main argument [_arg], being its type.
 * All other arguments besides [id] and [_arg], will become properties.
 */
create-event:examples.data.update

  // Adding all arguments, except [id] and [_arg].
  add:x:/../*/update-data/*/*
    src:x:(/./--!/./--(/id|/_arg))/<-

  // Setting [type] to [_arg]'s value.
  set:x:/../*/update-data/*/src/*/type?name
    src:x:/../*/_arg?value

  // Actual update.
  update-data:x:@"/*/*/{0}/""=:guid:{1}"""
    :x:/../*/_arg?value
    :x:/../*/id?value
    src
      type
```

**examples.data.delete.hl**

```hyperlambda
/*
 * Our "delete" wrapper for our CRUD operations.
 * Expects main argument [_arg] being ID of what object to delete.
 *
 * Doesn't care about the "type" of object, but will simply delete
 * the object with the given [_arg] ID, regardless of its type.
 */
create-event:examples.data.delete
  delete-data:x:@"/*/*/""=:guid:{0}"""
    :x:/../*/_arg?value
```

Make sure you create the 4 files from above, and that you explicitly execute them - Which you can do with the following 
code in the Hypereval.

```hyperlambda
.files
  /modules/hello-world/examples.data.create.hl
  /modules/hello-world/examples.data.read.hl
  /modules/hello-world/examples.data.update.hl
  /modules/hello-world/examples.data.delete.hl
micro.evaluate.file:x:/@.files/*?name
```

Then copy the above code into your _"Hello World"_ application's _"startup.hl"_ file. This ensures that these event are
re-created, if for some reasons your web server should restart.

### Using our database layer

At this point, you can start creating, reading, updating and deleting items from your database. Try out the following code for instance.

```hyperlambda
examples.data.create:examples.my-data
  name:Thomas Hansen
  email:thomas@gaiasoul.com
```

Which of course inserts a a lambda object into your database. Then try to read your newly inserted object, using the following code.

```hyperlambda
examples.data.read:examples.my-data
```

Notice, providing a **[start]** and **[end]** argument to your **[examples.data.read]** invocation is optional. If these arguments 
are not supplied, they will default to *"0"* and *"10"* respectively. You can also update an existing object, with the following Hyperlambda.

```hyperlambda
examples.data.update:examples.my-data

  // Make sure you use the ID matching results from our previous "read".
  id:baeffb4c-ed3b-48eb-9032-1bcb2f60b7a5

  email:foo@bar.com
  name:John Doe
  phone:90909090
```

Notice, in our above code, we actually add a **[phone]** property to our object, which didn't previously exist. Notice also that you 
need to change the above **[id]** argument's value, to match the ID from the results of your above read operation. When you have 
executed the above code, you can try to use a read invocation again, to see how your object actually changed.

```hyperlambda
examples.data.read:examples.my-data
```

### I can do that with OOP!

The observant reader, might at this point argue that what we did in our above database layer, is nothing new, and that it can easily 
be done with traditional OOP. Implementing something like the above, by wrapping these four operations into some class, creating four 
public methods, would be trivial. However, this would highly likely result in a lot more code. Our above datalayer consists of 34 lines 
of code, plus some comments. A traditional OOP solution, would highly likely result in much more code.

Besides, what you might forget by repeating the claims of the above header, is that we actually *changed* our lambda object's *structure*, 
during our update. Resulting in, that if you wish, you can have a heterogeneous *"table"* structure, allowing for you to easily extend 
your types in the future - *Without* breaking neither your code, nor your database structure. In C# for instance, such a change, would 
often require a recompilation of your *"model"*, breaking any existing clients consuming your model out there. For the record, I realise 
that there are ways to circumvent this in C#, at which point we're back to the delicate problem of trying to balance YAGNI 
with *"future change requirements"*. Meaning, to apply such a *"model"* in for instance C#, often would result in huge amounts of 
additional boiler plate code, which you might never need, such that you could accommodate for a future change, that *might never happen*. 
Besides, creating such a design in for instance C#, tends to end up with having to re-create the Node structure, effectively ending up 
with an *"incomplete implementation of Hyperlambda"*.

In addition, if you'd like to create support for multiple different database implementations, you'd need to create an abstract base class, 
or an interface. If you'd like to use your database layer in multiple different components, you'd also have to create some sort 
of *"abstract factory method"*, to create your object implementing this interface. Which would result in that the number of lines of 
code, would literally explode - And the complexity of the code, and its API, would significantly increase. Besides, the knowledge required 
for creating such an architecture, would be far more demanding, than what we did above.

If you'd like to create a *"polymorphistic override"* of the above 4 Active Events, which you could substitute with your original Active 
Events - Simply creating 4 new Active Events, taking similar arguments, would be sufficient. Polymorphism is a *non-existent problem* in 
Hyperlambda.

Imagine for instance, that you'd like to start storing *some* of your CRUD objects into a more scalable database for instance. Creating 
4 similar Active Events, resembling the above *"API"* (taking the same sets of argument), would simply require 30-50 new additional lines 
of code. After you had done such a thing, you could easily go through your app's code, and change the original invocations to our P5 
database invocations, to your *"whatever 'overrides'"*. You could even automate this process, by using the meta features of P5, to 
recursively traverse all Hyperlambda files within some folder, and easily change all invocations to **[examples.data.read]**, to 
e.g. **[examples.whatever.read]**. The latter could even be incorporated in your app, making your app automatically change its 
existing invocations *"whatever-1"* to *"whatever-2"*.

Another example includes the use-case of instead of storing your objects directly into the P5 database, rather transmit them as an HTTP 
web service request, to another server, or for that matter, storing your objects in your own file system locally. Since lambda objects 
are implicitly serializable, and easily converted into Hyperlambda - This would require no more than a handful of changes to your 
original code.

As always, the gift of Hyperlambda, isn't necessarily in *"what you see"*, it's rather in *"what you cannot see"*. Our above CRUD database 
layer, is much more generic and agile, than whatever you could build in traditional OOP languages, without violating the laws of YAGNI. 
Simply because the above solution is built, accommodating for change, without having to pay the price of added complexity, resulting 
in violating the rules of YAGNI.

In fact, let's illustrate this fact, with a simple change to our read Active Event. Change your **examples.data.read.hl** file to 
contain the following code. Don't worry if you don't understand everything in this file, it'll be thoroughly explained in later chapters.

```hyperlambda
/*
 * Our "read" wrapper for our CRUD operations.
 * Expects [start], [end] and a main [_arg] argument.
 * The main argument [_arg] is the type you're interested in.
 * All other arguments are "filter criteria", allowing you
 * to filter the results, according to its properties.
 */
create-event:examples.data.read

  /*
   * Used as a "signal node", to signal the beginning of event's
   * main lambda object, such that we can more easily get to the arguments
   * supplied by the caller.
   */
  .signal

  // Making sure we provide default [start] and [end] values.
  if:x:/../*/start
    not
    insert-before:x:/../0
      src:"start:0"
  if:x:/../*/end
    not
    insert-before:x:/../0
      src:"end:10"
  select-data:x:/*/*/{0}
    :x:/../*/_arg?value

  // Further filtering away nodes not matching specified criteria.
  for-each:x:/@.signal/--(!/start!/end!/_arg)
    set:x:/@select-data/*!/@select-data/*/*/{0}/.
      :x:/@_dp/#?name
    set:x:@"/@select-data/*/*(/{0}!/""=:regex:/{1}/i"")/."
      :x:/@_dp/#?name
      :x:/@_dp/#?value

  // Forward evaluating return, before deleting [start] and [end].
  eval-x:x:/../*/return/*
  set:x:/../*(/start|/end)

  // Returning results to caller.
  return:x:/@select-data/*/[{0},{1}]
    :x:/../*/start?value
    :x:/../*/end?value
```

The above modification, all of a sudden allows our read Active Event to be further parametrized, allowing filtering according to any property. 
Make sure you update your Active Event, by executing the following code first.

```hyperlambda
micro.evaluate.file:/modules/hello-world/examples.data.read.hl
```

Then create a couple of items with the following code

```hyperlambda
examples.data.create:examples.my-data
  name:Harry Foo Bar
  email:harry@foobar.com
examples.data.create:examples.my-data
  name:Jane Doe
  email:jane@doe.com
```

Then read some items, just like you did earlier.

```hyperlambda
examples.data.read:examples.my-data
```

As you can see, nothing has changed, hence we have not in any ways broken an existing code. However, try another read operation 
with the following code, to try out our *"new feature"*.

```hyperlambda
examples.data.read:examples.my-data
  name:doe
```

Our above implementation of our read Active Event, can all of a sudden be parametrized. If it is parametrized, it will filter 
away all resulting items, not having a property matching our parameter. Try the following for instance.

```hyperlambda
examples.data.read:examples.my-data
  name:d[a-z]{1,}e
```

Which will result in anything having a name, starting with *"d"*, ending with *"e"*, and having any other characters repeated 
at least once in between there be returned. Notice, if regular expressions are Greek to you, or the above lambda expression 
is completely impossible to understand - Hopefully, it will become more clear for you, as we proceed further into the book.

### Change is the only constant

We have *changed the signature of our event*. If we were to do such a thing in e.g. C# or C++, we'd be forced to recompile, 
and redistribute every single component, where we were using our old Active Events. This would imply having to contact all 
consumers of our Active Events, having them use our new and improved version, recompile their own modules, and redistribute 
this to everyone. If we didn't do this, we would break existing systems. There are ways around this in traditional OOP, 
but it requires literally an explosion of boiler plate code, yet again violating the laws of YAGNI.

However, since *any* lambda object, is a perfect substitute for any other possible lambda object - With Hyperlambda, we have 
not broken as much as a single application out there in existence using our component. And as consumers of our little component 
feels the urge to start using our new and improved version, they can slowly migrate their existing apps, over to start using 
our new version, and add up filters as they see fit.

In fact, even code written for the new version, would still perfectly function with the old version of our component. Although, 
it would result in an erronous result, since the invoker would expect a filtered result, while the old implementation would 
simply ignore the filtering criteria.

### Warning

At this point, the observant computer programmer, might point to the semantics of my second read Active Event, and probably 
claim it's utterly insane to create such an Active Event - Which of course actually was my intention. The read implementation 
above, would highly likely result in that your CPU and/or RAM would literally _"melt"_, if you started adding more than 
some ~5.000-10.000 complex CRUD items into your database, and start creating complex regular expression filtered read invocations.

However, this is a mute point, since the above solution gets you extremely rapidly up in regards to development of your components - 
Which means you can start focusing on your domain problems ASAP. As you later need to implement some super scalable database solution, 
such as MongoDB for instance - This can easily be done, by changing the entire implementation of your CRUD events, passing in the 
parameters given to some hard core, super scalable, MongoDB wrapper, allowing your database to go from ~5.000-10.000 records, to 
a _"gazillion"_ records, without breaking as much as a single line of code.

So instead of having to spend six months up front, creating some super scalable data layer, and generic parametrization design, 
for having generic methods, capable of taking *"whatever"*, and turn it into *"something else"* - You get to start out with your 
domain problem, from day one. Leaving the scalability issues to the end of the project. Which have drastic consequences for your 
ability to rapidly start creating value for your customer. While also never having to implement something you might never need. 
Some apps, might be perfectly fine with having a maximum of ~5.000 objects in their database, and hence would probably never 
need your super scalable database design, you wasted 6 months implementing, because you *believed* you were going to need it!

### You're not developing an "app"

Another beautiful idea, hopefully intuitively understood from our above example, is the fact that inevitably, as your solutions 
becomes more and more popular - Other app developers will realise they can consume your components, to add value to their own apps. 
At which point you are no longer an *"app developer"*, but in fact a *"platform vendor"*, allowing you to have others add value 
on top of your solutions. And even if others don't implicitly understand that fact, then you and your colleagues will understand it
easily, as you start out your *next* app - Since at that point, it is highly likely that you can in fact reuse 90% of everything
you did in your *first* app!

Being a platform developer is almost one of the implicit side-effects of using P5, because no app can be perceived as being isolated 
from any other app, running on the same system. The reasons for this, is because while in other programming languages, creating 
reusable software, is a tedious and difficult task - In P5, it is almost implicitly given, that whatever you create becomes reusable. 
This is due to that all lambda objects are created as equals. The Liskov Substitution Principle, has effectively been rendered *obsolete*. 
In P5, almost everything you can in theory create, is in practice reusable.

In fact, every single _"app"_ that you can install through [the Bazar](/bazar), gives you more Active Events you can use, since
none of them are _"apps"_, but rather built as reusable components and modules, allowing you to reuse them in your own projects.
If you need to send a PGP encrypted email for instance, you can easily do this using the API of [Sephia Five](/bazar?app=sephia-five).

Imagine if every single time you created a project, you could reuse all of its code, in all of your future projects. What would this 
do to your productivity? Would this give you a competetive edge?

With P5, this is the inevitable result, since you are creating *"modules"* and *"components"*, for then to *"orchestrate"* these 
modules together. P5 is all about eliminating borders, and reducing complexity, while increasing reusability and improving Time2Market.

### So what this have to do with the header of this article?

Well, the paradox is, that you can arguably _"fork"_ Phosphorus Five, without actually forking it. Since you can completely
change its behaviour, by _"injecting"_ your own logic, as a _"YALOA"_, resulting in that it becomes _"fork friendly"_.
