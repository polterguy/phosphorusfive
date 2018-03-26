## About Phosphorus Five

<img style="margin-left:1rem; float:right;max-width: 20%;" src="/modules/hyper-ide/media/logo.svg" />

Phosphorus Five is a web operating system, and a web application development framework, with some pretty
unique USPs. Among other things, it's extremely dynamic in nature, allowing for orchestrating your application
building blocks together, almost the same way you would assemble LEGO bricks into creations.
Phosphorus Five is built around three axioms, which you can read three MSDN Magazine articles about at the
following URLs. Notice, these articles are fairly technical in nature, and intended for software developers.

* [Active Events](https://msdn.microsoft.com/en-us/magazine/mt795187), providing a functional alternative to OOP, where polymorphism and encapsulation is being performed at the _"function level"_ instead of at the class level
* [Hyperlambda](https://msdn.microsoft.com/en-us/magazine/mt809119), which is a natural consequence of Active Events
* [Managed Ajax](https://msdn.microsoft.com/en-us/magazine/mt826343), providing an automatic bridge between your server side Hyperlambda and your client side JavaScript

The above three USPs provides an extremely flexible application abstraction layer, on top of the .Net Framework/CLR
or C#, allowing for you to create web apps much faster than what you'd achieve using conventional techniques.
Phosphorus Five is particularly well suited for _"enterprise types of web apps"_, managing data, using web forms
and similar types of techniques - And probably not that well suited for apps that are intended to be massively
scalable, such as social media platforms, required to respond to millions of web requests every day.

Phosphorus Five contains a whole range of apps, which we refer to as the _"GaiaSoul Suite"_, that allows you
to rapidly create your web apps, one way or another. Some examples of such apps are given below.

* Hyper IDE - A web based Integrated Development Environment
* Magic Menu - A global navigation system, with support for speech recognition and speech synthesis, allowing you to use _"natural language"_ to control your computer
* Camphora Five - A CRUD app generator, allowing you to create CRUD apps, literally in seconds, without requiring you to know any programming

### Philosophy

Phosphorus Five is about privacy, freedom of expression, and the freedom to create - Without having to conform
to the terms of others. Hence our slogan, which goes like this _"In the beginning there was Hyperlambda"_. If you
don't understand where I picked up that slogan, feel free to pick up any Bible. The idea is to allow for a close and
personal relationship to the act of creation, without any intermediaries controlling the terms, and/or your access.
Gutenberg was my philosophical inspiration while developing Phosphorus Five. Gutenberg's idea was to commoditize
access to _"the word"_ (AKA; God) - While my idea is to allow you to use _"the word"_ to communicate with your
computer, and have it do what you want it to do, without a cast of _"gurus"_ acting as intermediaries on your
behalf, or some shady people spying on your data and privacy in the process.

### About the help system

These help files are actually _"alive"_, which means they contain snippets of code, illustrating some concept,
which will be evaluated on the server when you choose to evaluate the associated snippet. This gives you hopefully
a nice and interactive way to learn Phosphorus Five, including your tactile learning abilities, creating a
_"hands on"_ type of learning curve. Below is an example. Notice, this example only works in browsers that have
speech synthesis. Hint; Google Chrome does ...

```hyperlambda-snippet
/*
 * Using speech synthesis from Micro to utter a sentence.
 *
 * Hint; Click the button in the bottom right corner of
 * this "snippet" to evaluate this piece of Hyperlambda.
 */
micro.speech.speak:Hello, my name is Phosphorus Five
  voice:Karen

/*
 * Displaying an information bubble window.
 */
micro.windows.info:Hello World!
```

You can also copy and paste example snippets, such as the above, into Hypereval - At which point you can modify them,
and play around with them, to include the tactile parts of your brain while learning. If you want to learn
Hyperlambda, I have written a _"book"_ about it, which you can find by clicking the _"home"_ button in the help
files, and choosing the _"Hyperlambda"_ sub section.
