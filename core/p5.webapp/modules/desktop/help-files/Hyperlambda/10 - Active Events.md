## Active Events

**Notice**, this is a fairly theoretical chapter, intended for system developers that already know another
programming language from before. Feel free to skip this chapter if you find it boring, or don't understand
some of its theory.

In this chapter, we will start discussing the heart of P5; *Active Events*. So far, we have used this name
many times during the book, but we have never formally defined it, or explained it in any ways. After having
read this chapter, you will hopefully have a 10.000 feet astronaut's overview of the answer to the question;
*"Why Active Events?"* In the next chapter, we will discuss the technical details of Active Events. For
the record, I once wrote an MSDN article about Active Events, which you can find below.

* [MSDN Magazine Article about Active Events](https://msdn.microsoft.com/en-us/magazine/mt795187).

### Definition

Active Events are first and foremost a design pattern. It is however a design pattern, that largely replaces
most other design patterns in this world. Simply because it solves the same problems as dozens of other
design patterns do combined. Active Events also happens to solve these problems very beautifully, making
sure you are able to create better cohesion in your software, while retaining a larger degree of encapsulation,
and end up with increased means to apply polymorphism. Hence, it also largely becomes a replacement to OOP.

An Active Event is an alternative way of invoking functionality. It is, in such a way, a substitute to what
traditional programming refers to as *"functions"* and *"methods"*. Among its defining traits, is the fact
that every Active Event, can accept the same set of arguments. In fact, they all have the same input
signatures, which can be condensed down to *"a bunch of nodes"*. They also all return the exact same stuff,
which again is a _"bunch of nodes"_. A _"bunch of nodes"_ is again what we refer to as a _"lambda object"_,
which is really just a different way of stating a _"bunch of nodes"_.

https://phosphorusfive.files.wordpress.com/2018/01/22hpw9.jpg

### Terms

**Definition 0**; *"OOP"* is an acronym, and it means *"Object Oriented Programming"*. Its main design,
is based upon the assumption, that things in software, can be *"classified"*, and *"types"* can be created,
that encapsulates everything related to some part of your application.

**Definition 1**; *"Polymorphism"* is the art of substituting one piece of functionality with another piece
of functionality. Preferably such that any old functionality, is not broken as a consequence of applying
the new functionality. Polymorphism is about having old code, invoke new functionality, without breaking.

**Definition 2**; *"Encapsulation"* is the art of being able to hide the internals of your components. This
is crucial to be able to create large scale software systems, without forcing the software engineer to
understand every single detail of the system as a whole - It is arguably a prerequisite for creating systems,
where more than one developer is contributing to the system.

**Definition 3**; *"Cohesion"* is the art of making a piece of logic become *"self sufficient"*, and not
dependent upon some other prerequisite. For instance, if I say; *"New years eve is booring. But the
fireworks was nice this year"* - Then the last sentence cannot be isolatet, without loosing a crucial parts
of its context. If you drop the first sentence, the reader will not know if you're talking about new years
eve, or 4th of July. Hence, the last sentence, is arguably having *"bad cohesion"*. (oversimplified)

### The importance of good software design

All of the above definitions, are concepts you would want to increasingly take advantage of, as you create
more and more complex systems. A software system, lacking any of the above, is often referred to as being
_"badly designed"_. A software architect, and developer, should strive to _"increase cohesion"_,
_"apply encapsulation"_, and _"facilitate for polymorphism"_. If he or she does not do this, the result
often becomes _"spaghettic code"_.

Things in your software should preferably be *"substitutable with other things"* (polymorphism),
*"hide their internals from the rest of the system"* (encapsulation) and *"create value by themselves
without dependencies and pre-requisites"* (cohesion). If they do, you have created reusable software,
easily maintained, and understood by others - And the system is said to have a good architecture and design.
In traditional programming, a software system with good architecture, is often said to be _"SOLID"_.
Software guys (and gals) loves acronyms.

> Great software architecture is like great poetry, explaining it is redundant

### Active Events' architecture

Active Events, just so happens to drastically increase all of the above parameters. Or at the very least,
make it much easier for you, to accomplish the above. Which often comes as a surprise to software developers,
coming from traditional programming languages - Since they are often taught that the above concepts,
can only be achieved using OOP. Hyperlambda *have no OOP mechanisms*. Hence, arguably, it *refactors* our
best practices, and instigates the creation of an entirely new axiom in software architecture, entirely based
upon _"functional programming"_ - While paradoxically not loosing anything, since you can easily combine
Active Events and Hyperlambda with OOP, and *traditional* programming. You could for instance create an
Active Event in C#, where you use as many of the traditional OOP mechanisms as you see fit. To illustrate
this point, realize that Hyperlambda and P5 is entirely built in C#, _with_ OOP mechanisms.

One example of how OOP breaks down cohesion, is how OOP for instance, during the creation of objects,
forces you to decorate these objects. Already at this point, you have lost cohesion. Another example is
how OOP often requires you to pass in an object to a method, which requires you to instantiate this input
object, at which point you've also lost cohesion. A third example, is how a method in an OOP language has
a signature, which means you've already lost the ability to substitute your method invocation, with most
other methods in your system, and you have lost a lot of your ability to *"apply polymorphism"*.

By completely dropping the concept of OOP, we have very effectively *eliminated* an entire dimension of
potential problems related to encapsulation, polymorphism, and cohesion.

The perfect object, according to the rules of encapsulation, has no public methods, properties, or fields.
In addition, it can be substituted with any other object in your system, facilitating for 100% perfect
polymorphism. This just so happens to largely define lambda objects.

https://phosphorusfive.files.wordpress.com/2018/01/22hqeh.jpg

### How Active Events solves these problems

Any Active Event can accept any input arguments, intended for any other Active Event. This implies that you
can, at least in theory, substitute any Active Event, with any other Active Event. In addition, the
invocation to one Active Event, is a simple string, which can be changed as you see fit. Hence, Active
Events are said to facilitate for *100% polymorphism*, where anything can be substituted with anything -
At least in theory.

Since there are no types in Hyperlambda, we have eliminated a whole dimension of cohesion problems.
When you have no types, there is no *"external magical state"*, which increase dependency between your
code, and some other piece of code. Without types, it is meaningless to talk about initializing, or
decorating your objects, *obviously* - Hence, good cohesion becomes an *implicit side-effect* of Active
Events. Breaking the rules of cohesion, is almost impossible with Active Events.

Since no lambda objects contains any public methods, properties or fields, beyond the ones they all contain,
necessary to traverse and change the object - There are no semantic differences between two different lambda
objects. Arguably hence, the lambda object becomes perfectly encapsulated - And exchanging any lambda object
with any other, is as easy as editing a string in a text editor.

### The LSP problem

In classic OOP, an example of something that is extremely unintuitive, is the (L)iskov (S)ubstitution
(P)rinciple, in regards to for instance squares and reactangles. In OOP, you are according to LSP never
supposed to inherit a *"Square class"* from a *"Rectangle class"*. Neiter can you do the opposite. The
reasoning for that, is beyond the scope of this book, but can easily be found with a Google search. This
is beacuse in OOP a rectangle is not a subset of a square, neither is the opposite true. This creates
unintuitive structures for you in your code, where your code doesn't match the mental model you
*"intuitive feel"* is the right model for creating your class hierarchies.

The LSP problem often in practice, goes so deep in fact, that any software system which has a
*"good architecture"*, often ends up not being able to utilise inheritance at all. This results in that
polymorphism becomes impossible, without breaking the rules of SOLID code. A symptom of this problem,
can be seen by how all great software architects will repeat to you; *"Prefer composition, don't use
inheritance"*. So basically, the entire axiom that OOP was built around, which is classes inheriting from
other classes, becomes more or less by the definition of *"good software architecture"*, impossible to use,
and arguably obsolete, without risking ending up with *"spaghetti code"*, violating LSP.

In Hyperlambda, the Liskov Substitution Principle makes absolutely no sense, and the entire problem's axiom,
becomes the equivalent of asking the question; *"Is your car married?"*. In Hyperlambda, a vehicle can be
a cat, a soccer field, or a piece of metal - _Without_ breaking LSP. And you can easily marry any lambda
object with any other lambda object. This makes it possible for you, to apply whatever *"object model you
feel for applying"*, to your code, making the LSP problem completely *obsolete*.

### Active Events and C#

If you're an experienced developer, you might be wondering about how to create an Active Event in C#.
Below is a video where I do just that, for afterwards to use my Active Event from Hyperlambda. The video is
roughly 3 minutes long, and describes everything you need to know to create your own C# Active Event.

https://www.youtube.com/watch?v=_5N9bMjjZ0Y

### Wrapping up

In this chapter, we looked at Active Events, in relationship to OOP and software architecture - And hopefully,
we created a common understanding of the advantages of using Active Events, instead of traditional OOP.
A more thorough explanation can be found below.

[MSDN Magazine, Active Events, one design pattern instead of a dozen](https://msdn.microsoft.com/en-us/magazine/mt795187)

