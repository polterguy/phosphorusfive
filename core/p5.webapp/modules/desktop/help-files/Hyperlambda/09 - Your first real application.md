## Your first real application

Congratulations, you should now know all the most difficult concepts you need to learn, in order to create
rich and complex Ajax apps with Hyperlambda. We shall therefor celebrate by creating a _"fully fledged app"_.
In this chapter, we will therefor create a contacts application, allowing us to keep track of our friends,
their emails, and phone numbers. Before we can dive into our application though, there are some concepts
we'll need to cover first.

* Loading and saving of files
* Parts of the folder structure of Phosphorus Five
* The **[apply]** Active Event
* Widget lambda events

### Loading and saving files

In P5, there are two important events for loading and saving files. These are **[load-file]** and **[save-file]**. Before we start using 
them though, we'll need to have a look at the folder structure of P5.

The most important folder for this chapter is called *"users"*. This folder will contain one folder for each user you have in your P5 installation. 
By default, P5 has only one user, which is your *"root"* user - So typically as you start out with P5, there will only be one folder here. This 
folder will be the *"root"* folder. You can browse this folder using your folder explorer in Hyper IDE.

Inside of your user's folder, the *"root"* folder that is, you can find a *"documents"* folder. This is the equivalent of *"Your documents"* 
in windows, or *"Home"* in Linux. This is important to understand for this chapter, since we will store our *"database"* within this folder. 
Our file will be named *"adr.hl"*. The extension *".hl"* implies *"[H]yper - [L]ambda"*. Your *"documents"* folder, contains two folders. 
One *"private"* folder, and another *"public"* folder. We will be using the *"private"* folder, to store a file, containing the data for our 
CRUD application. Files inside of this folder, cannot in general, be accessed by anyone, except the user whom these files belongs to. 
Hence, they are your user's *"private files"*.

To reference files inside of your user's home folder, you can prepend your path with a tilde `~/`. If you start out a filename with a tilde, 
this will automatically refer to a file in the currently logged in user's home folder. If you want to create a file inside of your private documents folder, 
you can use something resembling the following code.

```hyperlambda
save-file:~/documents/private/foo.txt
  src:Hello filesystem!
```

If you execute the above code in Hypereval, it will create a simple text file for you, inside of your private documents folder. To understand 
why, realize that the tilde `~` will be substituted with *"/users/username"*, before the file is saved. The *"username"* parts of the path, 
depends upon which user is logged in, invoking the **[save-file]** event. Since the root folder for *everything* related to files in P5, 
is the main root web application folder, this means your file can be found within *"/phosphorusfive/core/p5.webapp/users/root/documents/private/"* - 
Where ever that is locally within your system. Notice, this assumes you're using the source code version of Phosphorus Five. If you have 
deployed your system into a Linux production environment, typically the file will end up somewhere inside 
of _"/var/www/html/users/root/documents/private/"_ somewhere.

### The [apply] event

The **[apply]** Active event, is kind of like the *"Superman version"* of **[add]**. It allows you to take a data source, and combine it 
with a template, to create an entire hierarchy of nodes, and append into some destination, applying your template for each **[src]** result 
node set. We will be using this Active Event, to dynamically create an HTML table, from the content of your data file, containing the *"database"* 
for your contacts.

It might help to visualize the **[apply]** event as *"braiding"* together a data source and a template, to produce a combined results. If you 
find this concept difficult to understand, then don't despair, since there's a video at the end of this chapter, explaining it in details.

### Widget lambda events

As vaguely touched upon in one of our previous chapters, a widget can associate Active Events with itself. These are *"local"* events, 
that only exists, for as long as the widget itself exists. We will create one widget lambda event like this, to *"databind"* our HTML table.
We will create this event with the name of **[examples.databind-addresses]**. This event can be invoked just like a normal Active Event. 
If we wanted to, we could also pass in arguments, and return arguments from it, just as if it was a normal event. We do not need to neither 
pass in, nor return any arguments from it though. We simply need to *"databind"* our HTML table element within it - Which we will do, 
by loading our database file, and create an HTML table widget, for each **[item]** in our file.

### The code for our application

With these concepts, at least partially covered, let's move on to the code, and show you the entire listing for an Address book web app. 
Modify our _"Hello World"_ app's _"launch.hl"_ file from one of the first chapters, and exchange its code with the code listed below.
Or, simply click the _"flash"_ button at the bottom of our source listing, to evaluate it inline. Notice, this is
a fairly long listing, since it's arguably a _"complete web app"_. We will walk through the code later in this
chapter, hopefully making you able to understand the ghist of it.

```hyperlambda-snippet
/*
 * Includes Micro's main CSS files.
 */
micro.css.include

/*
 * Creating our main application wrapper widget.
 * This widget contains at the very least our "add contact" button, 
 * in addition to also possible an HTML table, dynamically created, 
 * according to the content of our "database file".
 */
create-widget
  class:container
  oninit

    /*
     * Making sure we initially databind our address HTML table, 
     * by invoking our "widget lambda event", that is declared 
     * further down on page.
     */
    examples.databind-addresses

  widgets
    div
      class:row
      widgets
        div
          class:col-100
          widgets

            /*
             * This becomes the wrapper widget for our HTML table, 
             * containing our "list of contacts".
             */
            container:table_wrapper

            /*
             * This becomes our "create new contact button".
             */
            literal:add_btn
              element:button
              innerValue:+
              style:"width:200px;"
              onclick

                /*
                 * Using a "wizard window" to retrieve name, 
                 * email and phone from user.
                 */
                create-widgets
                  micro.widgets.modal:my-modal-widget
                    widgets
                      micro.widgets.wizard-form:my-input-form
                        text
                          info:Name
                          .data-field:name
                        text
                          info:Email
                          .data-field:email
                        text
                          info:Phone
                          .data-field:phone
                        button
                          innerValue:OK
                          onclick

                            /*
                             * Temporary variable containing the content of our "database".
                             */
                            .content

                            /*
                             * Retrieving data supplied by user.
                             */
                            micro.form.serialize:my-input-form

                            /*
                             * Loading database file, if it exists, and appending 
                             * its old values into our [save-file] invocation.
                             */
                            file-exists:~/documents/private/adr.hl
                            if:x:/@file-exists/*?value

                              // File exists, appending content into [_content] below.
                              load-file:~/documents/private/adr.hl
                              add:x:/../*/.content
                                src:x:/@load-file/*/*

                            /*
                             * Then appending the values supplied by user for new record.
                             * Notice, we make sure we HTML encode these values, to prevent an adversary
                             * from injecting malicious HTML code into our app.
                             */
                            for-each:x:/@micro.form.serialize/*
                              set:x:/@_dp/#?value
                                p5.html.html-encode:x:/@_dp/#?value
                            add:x:/+/*/*
                              src:x:/@micro.form.serialize/*
                            add:x:/@.content
                              src
                                item

                            /*
                             * Converting our [.content] node's content to Hyperlambda (string),
                             * and saving it to disc.
                             * Notice, to prevent this file from growing indefinitely, we make sure we only
                             * save a maximum of 10 items to it.
                             */
                            while:x:/@.content/*?count
                              >:int:10
                              set:x:/@.content/0
                            lambda2hyper:x:/@.content/*
                            save-file:~/documents/private/adr.hl
                              src:x:/@lambda2hyper?value

                            /*
                             * Making sure we databind our HTML table again, to make sure 
                             * our newly added record is shown.
                             */
                            examples.databind-addresses

                            /*
                             * Deleting modal window.
                             */
                            delete-widget:my-modal-widget

          /*
           * This is our "widget lambda events".
           */
          events

            /*
             * The "databind" table event.
             *
             * This Active Event simply loads our "database file", and creates 
             * our HTML table accordingly.
             *
             * The first line in our event, becomes its name, meaning we can 
             * invoke it by simply adding a node with a name matching this 
             * Active Event's name.
             */
            examples.databind-addresses

              /*
               * This invocation clears our HTML table wrapper widget for 
               * any previous content.
               */
              clear-widget:table_wrapper

              /*
               * Here we check if our "database file" exists.
               */
              file-exists:~/documents/private/adr.hl
              if:x:/@file-exists/*?value

                /*
                 * There exists a database.
                 * Loading it, and creating our HTML table accordingly.
                 */
                load-file:~/documents/private/adr.hl

                /*
                 * Notice, here we use [apply], to dynamically append to our 
                 * [create-widget] invocation's children [widgets].
                 */
                apply:x:/..if/*/create-widget/*/widgets/*/container/*/widgets
                  src:x:/@load-file/*/*
                  template
                    container
                      element:tr
                      widgets
                        literal
                          element:td
                          {innerValue}:x:/*/name?value
                        literal
                          element:td
                          {innerValue}:x:/*/email?value
                        literal
                          element:td
                          {innerValue}:x:/*/phone?value

                /*
                 * Now our "tbody" HTML widget below should contain on "tr" widget 
                 * for each row from our "database file".
                 * Each "tr" widget again, should contain one "td" widget for the name,
                 * email and phone from our contact.
                 */
                create-widget:contacts_table
                  parent:table_wrapper
                  element:table
                  widgets
                    literal
                      element:thead
                      innerValue:<tr><th>Name</th><th>Email</th><th>Phone</th></tr>
                    container
                      element:tbody
                      widgets

/*
 * In case you evaluate this from the Dox, we simply
 * make sure we create a bubble window, informing you
 * that you'll have to scroll to the bottom of your page,
 * to actually see the result of this Hyperlambda.
 */
micro.windows.info:Your 'app' can be found at the bottom of your page!
```

### [apply] in details

As previously mentioned, the **[apply]** Active Event, allows you to braid together a data **[src]**, with a **[template]**, and put the 
results into some destination. The destination is expected to be the value of the main **[apply]** node, and must be an expression leading 
to one or more nodes. The destination must return a node somehow, usually implying it should be type declared as a `?node` type of expression. 
Which you may remember from a previous chapter, was the default type declaration of an expression.

Its **[src]** argument, is expected to be an expression, also leading to a node-set somehow. The way to envision the **[src]** argument, 
is that **[apply]** iterates over its **[src]** result set, and uses the currently iterated node, as the idenity node, for creating a lambda 
object, based upon the **[template]**, where expressions inside of your **[template]** can reference the currently iterated **[src]** node.
The **[template]** hence, is being created once for each result from the **[src]** expression, having each iteration, use the currently 
iterated **[src]** node, as its identity node.

### Databound expressions in [apply]

You can see one of the data bound expressions in your code editor above where it says `{innerValue}:x:/*/name?value`. Such a databound 
expression, is declared by making sure its name starts with a `{`and ending with a `}`. These two parts of your node's name are removed 
though, before the node is created, and appended into its destination. They are simply there to inform **[apply]** that this is a 
databound node. Hence, to declare a databound node, simply wrap its name inside of curly braces, and add up an expression being its relative 
path, to whatever you wish to databind the node's value towards.

The expression in the value of a data bound node, is local for the currently iterated **[src]** result. This means that the *"idenity"* 
node for the first iteration, is in fact not the **[{innerValue}]** node itself, but rather the first item from our **[load-file]** invocation.

### Semantics of [load-file] when loading Hyperlambda files

At this point, it might be useful to realize that our invocation to **[load-file]**, will in fact automatically convert Hyperlambda files 
into a lambda object, unless you explicitly tell it not to. So our **[load-file]**, after invocation, will in fact not yield plain text, 
but in fact an entire lambda hierarchy. It will resemble the following example after evaluation.

```hyperlambda
/* ... rest of code ... */

load-file
  /users/root/documents/private/adr.hl
    item
      name:Thomas Hansen
      email:thomas@gaiasoul.com
      phone:98765432
    item
      name:John Doe
      email:john@doe.com
      phone:99887766

/* ... rest of code ... */
```

So what our **[src]** argument to **[apply]** is actually pointing to, are the **[item]** nodes returned from **[load-file]**. It probably 
helps to lock this mental picture into your mind, and realize that the **[src]** expression to **[apply]** hence, is actually iterating 
over every single **[item]** from your _"database file"_.

This means that when we write `:x:/*/name?value` inside of a databound node, like we do in our first data bound node, this expression is 
for the first iteration of our **[src]** argument, relative to the first **[item]** from the results of our **[load-file]** invocation above. 
For the second iteration, it is relative to the second **[item]**, and so on. So what is added to our **[widget]** hierarchy, becomes 
something similar to the following.

```hyperlambda
container
  element:tr
  widgets
    literal
      element:td
      innerValue:Thomas Hansen
    literal
      element:td
      innerValue:thomas@gaiasoul.com
    literal
      element:td
      innerValue:98765432
container
  element:tr
  widgets
    literal
      element:td
      innerValue:John Doe
    literal
      element:td
      innerValue:john@doe.com
    literal
      element:td
      innerValue:99887766
```

If you find the **[apply]** event to be confusing, you can watch this video, which unfortunately is a little bit old though.

https://www.youtube.com/watch?v=KcaRyjj2w58

### [lambda2hyper], converting lambda to Hyperlambda

The above **[lambda2hyper]** Active Event, which we use in our code, simply converts a piece of lambda to a string, resembling 
its Hyperlambda version. There also exists a **[hyper2lambda]** event, which does the opposite. These events are useful for transforming 
lambda objects to Hyperlambda, and vice versa - Such as when we want to save a lambda object to disc, or use it as a string for some reason. 
Both of these two events should be relatively self explaining.

### Our "wizard" window

Our **[micro.widgets.wizard]** invocation is a new construct. Its purpose is simply to provide an easy wrapper for asking the user for some 
new input, or edit some existing data. It allows to declare which input you want to ask the user from, and transforms from e.g. **[text]** to
an _"input"_ element/widget automatically. If you click the *"+"* button in your application, you can clearly see the relationship between 
the **[micro.widgets.wizard]** node's children, and the 3 input textboxes, asking the user for a *"name"*, *"email"* and *"phone"*.

Our wizard widget is documented in the Micro section of our documentation.

The **[micro.form.serialize]** event, does exactly what you think it does. After invocation, it will look something like the following.

```hyperlambda
micro.form.serialize
  name:Thomas Hansen
  email:thomas@gaiasoul.com
  phone:98765432
```

Also our serialize event is documented in our Micro section.

After we have retrieved the values from our wizard window, we first check if there already exists a *"database file"*. This is necessary, 
in order to make sure we actually *add* records to our data. If it does, we load this file, and add the contents of it into a 
temporary **[.content]** node. Notice, we do this before we add the values from our newly created record into this file, to ensure the 
last record supplied, physically becomes the last record in our file. This preserves the order of our records, according to the order 
they were supplied by the user. Then finally, before we convert this **[.content]** node to Hyperlambda, and save it to disc, we add 
the values from our *"wizard"* window.

Our little **[while]** trick, simply ensures we only save our 10 most recent records.

The last thing we do in our **[onclick]** Ajax event, is to make sure we invoke **[examples.databind-addresses]**, which is our widget 
lambda event, that is responsible for databinding our HTML table all over again.

### Making your app secure

Our invocation to **[p5.html.html-encode]** HTML _"encodes"_ your string. This is necessary to prevent an adversary
from being able to inject malicious HTML that is rendered on the clients somehow. Such malicious HTML could be for
instance including some malicious piece of JavaScript, that compromises the client somehow.
To avoid this, we can invoke **[p5.html.html-encode]**, which will transform all occurrencies of angle brackets,
and other potentially threatening characters, into their safe counterparts.

### Misc. constructs

The **[clear-widget]** Active Event simply empties a named widget for all of its children widgets. There also exists
a **[delete-widget]** cousine of this event, which instead of emptying the widget, will entirely delete a named
widget, including its children. However, since we need the actual widget to stay around, yet still need to empty
it - We use **[clear-widget]** instead.

The **[while]**, **[for-each]** and **[if]** Active Events will be covered in a later chapter. The **[file-exists]**
event simply checks to see if a file exists on disc, and if so, returns true. Refer to the documentation for _"p5.io"_
in the _"Plugins"_ parts of the documentation to understand how it works.

In our last **[create-widget]** invocation, at the bottom of our code, there is an example of providing HTML as the **[innerValue]**
of a widget. We are using this to create the header of our table HTML element. This will simply create _"static DOM"_
for us, and not create widgets which we can refere to ourselves, and is sometimes a more efficient way of creating
content/markup, than creating _"everything"_ as widgets. After all, a widget does come with some overhead.

The **[micro.widgets.wizard-form]** widget and the **[micro.form.serialize]** event is documented in the
_"Micro"_ section of these documentation files.

In our application, we are also sometimes declaring the values for our nodes with double quotes surrounding the value.
This is necessary only if your value contains complex characters, such as a (:) character, a carriage return, etc.
If you have created string literals in C#, this may seem intuitive - And in fact these types of string values in
Hyperlambda are identical to how you would compose a string literal in C#.

### Wrapping up

A nice tricks to play around with longer examples, such as these, is to create a _"snippet"_ in Hypereval, and
make sure you save your snippet as a _"page"_ type. This allows you to keep all your examples in the same place,
and later refer to them, as the need arises.

