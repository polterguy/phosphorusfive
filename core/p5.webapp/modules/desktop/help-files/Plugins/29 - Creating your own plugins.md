## Creating your own plugin in C#

To create a plugin for Phosphorus Five, allowing you to invoke C#/CLR code from Hyperlambda, is very easy - Simply create an Active Event,
and make sure you load up your assembly and register it as an Active Event handler. See the video below for a demonstration of how you can
create your own plugins. The video is a little bit dated, and the API has changed slightly - But it gets the point through fairly well.

https://www.youtube.com/watch?v=sUeRdmzRwbs

To consume your own plugin in Phosphorus Five, you'll need to make sure you put your assembly (your DLL) into the _"bin"_ folder of your Phosphorus
Five installation, and add a reference to your plugin into the `phosphorus/assemblies` part of your web.config file. This will automatically load
up your plugin, as your web server starts, and register it as an Active Event handler. See an example below of how to modify your web.config file
to load up your plugin.

```xml
<assemblies>

    <!-- ... rest of file ... -->

    <add assembly="your-assembly" />

    <!-- ... rest of the file goes here ... -->
```

The above will allow Phosphorus Five to automatically load up your plugin, as your web server starts. To create a plugin, you'll need to reference
the _"p5.core"_ project, and create a method which is marked as an `ActiveEvent`. Normally, you'd declare your method as static, at which case your
Active Event will automatically be available from Hyperlambda as you start your web server. You can see an example of how to create an Active Event
in C# that is named **[foo.bar]** below.

```clike
using p5.core;

namespace YourNamespace
{
    public class YourClass
    {
        [ActiveEvent (Name = "foo.bar")]
        public static void foo_bar (ApplicationContext context, ActiveEventArgs e)
        {
            /* ... do stuff with e.Args here ...  */
            e.Args.Value = 42;
        }
    }
}
```

You can invoke your Active Event from Hyperlambda, using something like the following.

```hyperlambda
/*
 * This will invoke the C# method from above.
 */
foo.bar
```

After you evaluate the above Hyperlambda, assuming you created the above C# Active Event, your lambda structure will resemble the following.

```hyperlambda
foo.bar:int:42
```

From within your C# code, you have complete access to the entire Node structure (lambda/graph object) through `e.Args`, which is actually a tree structure -
And you can reference and/or return new nodes, using the API of the `Node` class. To for instance reference two arguments, and return the product of these back
to the caller, you could do something resembling the following.

```clike
using p5.core;

namespace YourNamespace
{
    public class YourClass
    {
        [ActiveEvent (Name = "fancy.add")]
        public static void fancy_add (ApplicationContext context, ActiveEventArgs e)
        {
            int no1 = (int)e.Args ["no1"].Value;
            int no2 = (int)e.Args ["no2"].Value;

            /*
             * This adds a new node, which will be returned to caller,
             * having the name of "result", and the value of no1 + no2.
             */
            e.Args.Add ("result", no1 + no2)
        }
    }
}
```

The above of course, expects the caller to supply two integer arguments; **[no1]** and **[no2]** - And will return the results of adding those two integers 
together as **[result]**. Invoking it as follows from Hyperlambda.

```hyperlambda
fancy.add
  no1:int:2
  no2:int:2
```

Will result in the following after invocation.

```hyperlambda
fancy.add
  no1:int:2
  no2:int2
  result:int:4
```

If you wish to use expressions to reference values passed into your Active Events, you'll need to reference the _"p5.exp"_ project, and use the `p5.exp` namespace -
At which point you'll get access to several extension methods on the Node class, allowing you to reference expression values from your arguments. Below is an example.

```clike
using p5.exp;
using p5.core;

namespace YourNamespace
{
    public class YourClass
    {
        [ActiveEvent (Name = "fancy.add2")]
        public static void fancy_add2 (ApplicationContext context, ActiveEventArgs e)
        {
            /*
             * The GetExChildValue method will follow any expressions given as values of the arguments.
             */
            int no1 = e.Args.GetExChildValue<int> ("no1", context);
            int no2 = e.Args.GetExChildValue<int> ("no2", context);

            /*
             * Returning the results of the expressions back to caller.
             */
            e.Args.Add ("result", no1 + no2)
        }
    }
}
```

If you create an Active Event in C# resembling the above, you can invoke it using the following Hyperlambda.

```hyperlambda
_no1:int:2
_no2:int:2

/*
 * Notice, we are now fetching the argument values to our event invocation,
 * as the results of our expressions.
 */
fancy.add2
  no1:x:/@_no1?value
  no2:x:/@_no2?value
```

The above Hyperlambda, assuming you created the C# event, will still return `4` as the **[result]** after invocation. Notice, you can
still pass in static values to the above **[fancy.add2]** event. The `GetExChildValue` method will even automatically convert its input
to an integer type, if you pass in a string for instance. This makes it much more _"convenient"_ to work with types in P5, since for the
most parts, they're irrelevant from Hyperlambda.

The p5.exp project also contains other helper methods, to handle expressions, and do other things from your Active Events. If
you for instance want to support expressions leading to a node collection, you might want to have a look at the `XUtil.Iterate` method.
You can also of course use the existing plugins in Phosphorus Five as example code to understand how this works.

### Non-static Active Events

You can also create instance Active Events. However, if you do, you'll need to explicitly create an instance of your class wrapping your Active
Event, and register your object as an _"instance listener"_. The _"samples"_ folder in the source code for Phosphorus Five, has some code that
demonstrates how to do this.

