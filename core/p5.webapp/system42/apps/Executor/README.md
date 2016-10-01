The Hyperlisp and p5.lambda Executor
========

The Executor allows you to evaluate Hyperlisp and p5.lambda directly, from within your browser. It consists of two
syntax highlighted Hyperlisp editors; One taking your "input Hyperlisp", and the other displaying its "output", after
having been evaluated, showing the result of your evaluated code.

To try it out, start System42, and paste the following code into the input editor, and click the "Evaluate" button.

```
create-literal-widget
  element:h3
  parent:content
  class:col-xs-12
  innerValue:Click me!
  onclick
    set-widget-property:x:/../*/_event?value
      innerValue:Hello World!
```

To run the Executor, you need to be logged in as "root".



