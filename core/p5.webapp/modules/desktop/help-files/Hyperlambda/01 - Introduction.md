## Introduction

Welcome to the _"book"_ about Hyperlambda. In this interactive _"book"_, we will go through Hyperlambda examples,
allowing you to instantly play with them, from within your Phosphorus Five installation. This _"book"_ is created
in an interactive way, allowing you to instantly play with the examples, by interacting with the documentation
itself. I refer to this as _"literate documentation"_, implying that the documentation itself, understands, and
can execute, the same type of computer code, it is intended to document.

**Notice**, you might benefit from reading this guide from either Hyper IDE or Hypereval, to be able to instantly
play around with the examples, as you read through this guide - Or to create your own comments, by
editing the files in Hyper IDE.

### Goals of the book

By reading this book, you will learn everything you need to know to create your first Hyperlambda application - From the first 
line of code, to deployment in a production environment. My goal is to bring you up to this knowledge in Hyperlambda, 
in roughly 2 days. That way, the book becomes very practical, and hands on, progressing rapidly, and you can feel that 
you have done something constructive fast. We will start out by creating smaller examples, and proceed to more rich
examples, providing the foundation for you to solve your own problems.

### A guide to the guide

The book's convention is created carefully to allow you to understand what is being described. First of all, 
any property or attribute from Hyperlambda is referenced like **[foo]**, where *"foo"* assumes we are talking about
a node's name. This convention is used whenever a node is referenced inline in the text. Emphasized and important points, 
are written like *this*. Inline code is written like `this`, and multiple lines of code is written like below.
Often you can also directly evaluate the code examples, by clicking the _"flash"_ button embedded inside of the
code examples, such as the following illustrates.

```hyperlambda-snippet
/*
 * Creates a modal widget with a button.
 *
 * NOTICE, click anywhere outside of the modal widget to close it.
 */
create-widgets
  micro.widgets.modal
    widgets

      /*
       * Our button.
       */
      button:my-button
        element:button
        innerValue:A piece of Hyperlambda code!
        onclick

          /*
           * Changes the text of our button.
           */
          set-widget-property:my-button
            innerValue:I was clicked!
```

The **[micro.widgets.modal]** above, is an extension widget from Micro, which will be created as we invoke **[create-widgets]** (plural form).

### External articles and references

If you are interested in the technical implementation details of Hyperlambda, its architecture, and how it was built,
and you are a seasoned system developer - Then you can read the following articles, preferably in order of appearance.

**Warning**, they're quite technical, and not necessary for you to get started with Hyperlambda.

1. [MSDN Magazine, Active Events, one design pattern instead of a dozen](https://msdn.microsoft.com/en-us/magazine/mt795187)
2. [MSDN Magazine, make C# more dynamic with Hyperlambda](https://msdn.microsoft.com/en-us/magazine/mt809119)
3. [MSDN Magazine, could managed AJAX put your web apps in the fast lane?](https://msdn.microsoft.com/en-us/magazine/mt826343)
