## An introduction to Hyperlambda

Welcome to the _"The Guide"_ about Hyperlambda. This _"book"_ is created as an interactive course, allowing you to
instantly play with the examples, by interacting with the documentation itself. I refer to this as
_"literate documentation"_, implying that the documentation understands, illustrates, evaluates, and creates
Hyperlambda itself. Below is an example.

```hyperlambda-snippet
/*
 * Creates a modal widget with a button.
 */
create-widgets
  micro.widgets.modal
    widgets

      p
        innerValue:Click anywhere outside of the modal window to close it.

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

### A guide to the guide

The book's convention is carefully created to illustrate what is being described. First of all,
any property or attribute from Hyperlambda is referenced like **[foo]**, where *"foo"* assumes we are talking about
a node's name. This convention is used whenever a node or an _"argument"_ is referenced inline in the text.
Emphasized and important points, are written like *this*, and sometimes like **this**. Inline code is written like
`this`, and multiple lines of codes is written like the above example illustrates. Often you can also directly
evaluate the code examples, by clicking the _"flash"_ button embedded inside of the code examples.
See the above Hyperlambda snippet for an example.

### Hyperlambda syntax

Hyperlambda is at its core actually a simple file format, kind of like YAML or JSON. It has a relational
name/value/children structure, illustrated in the example below.

```hyperlambda
/*
 * Multiline comment.
 */
some-node:value of node
  child-of-above-node

// Single line comment
another-node

/*
 * A more "complex" string literal value, surrounded
 * by double quotes.
 */
complex-value:"This value needs double quotes, due to the colon (:) in its value."

/*
 * This node is referencing an Active Event, which
 * you can see by its color.
 */
create-widget

  /*
   * These are arguments to the above Active Event invocation.
   */
  foo1:argument-one
  foo2:argument-two
```

The above example has 7 _"nodes"_ in total, where 3 of the nodes are children of some other node. If you add
two consecutive spaces (SP) on a new line, you open up the children node hierarchy of the node above it, and you
can add children to the above node. For a more detailed explanation about Hyperlambda's syntax, please
refer to the _"Plugins"_ documentation, and more specifically its _"p5.hyperlambda"_ section.

### External articles and references

If you are interested in the technical implementation details of Hyperlambda, its architecture, and how it was built,
_and you are a seasoned system developer_ - Then you can read the following articles, preferably in order of appearance.

**Warning**, they're quite technical in nature, and not necessary for you to get started with Hyperlambda.

1. [MSDN Magazine, Active Events, one design pattern instead of a dozen](https://msdn.microsoft.com/en-us/magazine/mt795187)
2. [MSDN Magazine, make C# more dynamic with Hyperlambda](https://msdn.microsoft.com/en-us/magazine/mt809119)
3. [MSDN Magazine, could managed AJAX put your web apps in the fast lane?](https://msdn.microsoft.com/en-us/magazine/mt826343)

This book can probably be read through in a couple of hours, following the examples, giving you a good understanding
of Hyperlambda, in roughly 2 hours.
