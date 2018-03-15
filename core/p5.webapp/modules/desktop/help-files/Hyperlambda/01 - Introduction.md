## Introduction

Welcome to the _"book"_ about Hyperlambda. In this interactive _"book"_, we will go through Hyperlambda examples, 
allowing you to instantly play with them, from within your IDE. This _"book"_ is created in an interactive way, 
allowing you to instantly play with the examples, by interacting with the documentation itself. I refer to 
this as _"literate documentation"_, implying that the documentation itself, understands, and can execute, the 
same type of computer code, it is intended to document.

This book is not _"read only"_. In fact, you can edit this page if you want to, by scrolling to the bottom of 
this page, and clicking the _"pencil"_ button in the bottom right corner of the page. This will open up this 
file in Hyper IDE, allowing for you to edit it, and add your own personal notes, as you proceed through the book. 
When you are done editing some parts of the book, you can click the _"reload"_ button, to reload the page, into 
the _"reader"_. This _"book"_ will also sometimes contain inline YouTube videos and images.

### Goals of the book

By reading this book, you will learn everything you need to know to create your first Hyperlambda application - From the first 
line of code, to deployment in a production environment. My goal is to bring you up to this knowledge in Hyperlambda, 
in roughly 2 days. That way, the book becomes very practical, and hands on, progressing rapidly, and you can feel that 
you have done something constructive fast. We will start out by creating smaller examples, and proceed to more rich
examples, providing the foundation for you to solve your own problems.

The second goal of the book, is to give you all the knowledge necessary to modify, create, and maintain Hyper IDE itself,
from which you are probably reading this book. This allows you to customise Hyper IDE, by creating plugins to it, and
in other ways extending it, to fit your own needs. So even if you never intend to create as much as a single application
in Hyperlambda, you will still benefit from reading this book, since it allows you to customise Hyper IDE according to
your needs.

The book will also contains lots of information about other relevant subjects, such as HTML, CSS, JavaScript and HTTP.

### A guide to the guide

The book's convention is created carefully to allow you to understand what is being described. First of all, 
any property or attribute from Hyperlambda is referenced like **[foo]**, where *"foo"* assumes we are talking about
a node's name. This convention is used whenever a node is referenced inline in the text. Emphasized and important points, 
are written like *this*. Inline code is written like `this`, and multiple lines of code is written like below.
Often you can also directly evaluate the code examples, by clicking the yellow _"flash"_ button embedded inside of the
code examples, such as the following illustrates.

```hyperlambda-snippet
create-widget:my-widget
  parent:hyper-ide-help-content
  element:button
  innerValue:A piece of Hyperlambda code!
  onclick
    set-widget-property:my-widget
      innerValue:I was clicked!
```

Notice, when you have evaluated the above code example, you can find its result at the bottom of your page. Click the yellow
_"flash"_ button, and scroll to the bottom of your page to see the result.

### Playing with the code examples

If you want to copy and paste these code examples into your own code, you will have to edit the **[parent]** argument, 
since it is referencing a widget, which only exists as long as the help files are open - Otherwise, you might
get _"weird exceptions"_ when evaluating the code examples in your own code.

Besides from that, most code examples are _"self contained"_, meaning you can simply copy them from
the documentation, and paste them into for instance the Hypereval module, to play around with them. 
If you have the _"Hypereval"_ plugin enabled, you can click it at the top of this page (the toolbar button 
that resembles a _"flash"_). Then copy and paste the above code into Hypereval, and click
the _"Evaluate"_ button in Hypereval (which is the button that resembles a _"flash"_). You can install 
Hypereval [here](/bazar?app=hypereval).

### External articles and references

If you are interested in the technical implementation details of Hyperlambda, its architecture, and how it was built - Then
you can read some of the following articles.

**Warning**, they're quite technical, and not necessary for you to get started with Hyperlambda.

1. [MSDN Magazine, Active Events, one design pattern instead of a dozen](https://msdn.microsoft.com/en-us/magazine/mt795187)
2. [MSDN Magazine, make C# more dynamic with Hyperlambda](https://msdn.microsoft.com/en-us/magazine/mt809119)
3. [MSDN Magazine, could managed AJAX put your web apps in the fast lane?](https://msdn.microsoft.com/en-us/magazine/mt826343)
