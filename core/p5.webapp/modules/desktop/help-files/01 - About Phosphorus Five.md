## About Phosphorus Five

<img style="margin-left:1rem; float:right;max-width: 20%;" src="modules/hyper-ide/media/logo.svg" />

Phosphorus Five is a web operating system, and a web application development framework, with some pretty
unique USPs. Among other things, it's extremely dynamic in nature, allowing for orchestrating your application
building blocks together, almost the same way you would assemble LEGO bricks into creations.
Phosphorus Five is a highly modularized, dynamic, and flexible web operating system, created as a thin layer
on top of your existing web server and underlaying operating system.

Phosphorus Five is built around three axioms, which you can read three MSDN Magazine articles about at the following URLs.

* [Active Events](https://msdn.microsoft.com/en-us/magazine/mt795187), providing a functional alternative to OOP, where polymorphism and encapsulation is being performed at the _"function level"_ instead of at the class level
* [Hyperlambda](https://msdn.microsoft.com/en-us/magazine/mt809119), which is a natural consequence of Active Events
* [Managed Ajax](https://msdn.microsoft.com/en-us/magazine/mt826343), providing an automatic bridge between your server side Hyperlambda and your client side JavaScript

The above three USPs provides an extremely flexible application abstraction layer, on top of the .Net Framework/CLR
or C#, allowing for you to create web apps much faster than what you'd achieve using conventional techniques.
Phosphorus Five is particularly well suited for _"enterprise types of web apps"_, managing data, using web forms
and similar types of techniques - And probably not that well suited for apps that are intended to be massively
scalable, such as social media platforms, required to responding to millions of web requests every day.

Phosphorus Five contains a whole range of apps, which we refer to as the _"GaiaSoul Suite"_, that allows you
to rapidly create your web apps, one way or another. Some examples of such apps are given below.

* Hyper IDE - A web based Integrated Development Environment
* Hypereval - A _"snippet database"_, allowing for you to easily create snippets of Hyperlambda, stored in your MySQL database
* Camphora Five - A CRUD app generator, allowing you to create CRUD apps, literally in seconds

In addition Phosphorus Five also contains a whole range of _"enterprise types of apps"_, such as Sephia Five and
Sulphur Five, which is a webmail software system with cryptography support, and a secure file sharing system.
Phosphorus Five also contain an integrated _"Bazar"_, which kind of becomes your own personal _"App Store"_ for
distributing your own apps. Phosphorus Five's goal, is to provide all the tools for you that you need to create
your own _"private and personal Silicon Valley"_, such that you can take control over your online life, and
not be dependent upon using any other _"cloud"_ vendors, resulting in that you loose control over your own data.

### Philosophy

Phosphorus Five is about privacy, freedom of expression, and the freedom to create, without having to conform
to the terms of others. Hence our slogan, which goes like this _"In the beginning there was Hyperlambda"_. If you
don't understand where I picked up that slogan, feel free to pick up any Bible. The idea is _"One God each"_.
And having a close and personal relationship to God, without any intermediaries controlling the terms, and/or
your access. Gutenberg was my philosophical inspiration while developing Phosphorus Five. Gutenberg's idea
was to commoditize access to _"the word"_ (AKA; God) - While my idea is to allow you to use _"the word"_ to
communicate with your computer, without a cast of _"gurus"_ acting as intermediaries on your behalf.
Which probably explains why I have been permanently banned from Facebook ... ;)

### About the help system

These help files are actually _"alive"_, which means they contain snippets of code, illustrating some concept,
which will be evaluate on the server when you choose to evaluate the associated snippet. This gives you hopefully
a nice and interactive way to learn Phosphorus Five, including your tactile learning abilities, creating a
_"hands on"_ type of learning curve. Below is an example.

```hyperlambda-snippet
/*
 * Using speech synthesis from Micro to utter a sentence.
 *
 * Hint; Click the button in the bottom right corner of
 * this "snippet" to evaluate this piece of Hyperlambda.
 */
micro.speech.speak:Hello there, my name is Phosphorus Five

/*
 * Displaying an information bubble window.
 */
micro.windows.info:Hello World!
```

If you have Hypereval installed, you can also instantiate Hypereval as a plugin, and immediately start
playing around with and modifying these code examples.

### License

Phosphorus Five is distributed under the terms of the GPL license version 3, but [proprietary options exists](/bazar?app=license)
if you want to create _"closed source"_ or _"proprietary software"_.
