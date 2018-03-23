## Hello World

**Notice**, if you have followed the _"Hello World"_ tutorial previously, you will already have this app in
your _"/modules/"_ folder, at which point you can simply open up that folder, and browse its files instead.
This chapter also assumes you are reading it from within Hyper IDE. If you don't, then please open Hyper IDE,
and return to this chapter from the help system inside of Hyper IDE - The latter can be done by opening up
help from inside of Hyper IDE, clicking the documentation's _"home"_ button, and find this chapter again.

In this chapter we are going to create our first real Hyperlambda application called *"Hello World"*. By
walking through this application, and explaining what it does, you will be armed with the knowledge required
to create your own Ajax web apps. First make sure you select the _"/modules/"_ folder in the file explorer
to your left. Then click the _"star"_ toolbar button, which will open up a wizard form for you, allowing you
to create a new application. Name your app _"hello-world"_, and make sure you select the _"hello-world"_
template project (*type* that is), and click _"Create"_. Below is a screenshot of how this should look like.

https://phosphorusfive.files.wordpress.com/2018/03/hell-world-template-screenshot.png

This will create a new project for you, and automatically open up its _"launch.hl"_ file. If you open up another
tab window, by clicking [here](/hello-world), you can test your application immediately, assuming you named
your app exactly _"hello-world"_. If you try to click the button in your app, it will change its value.

### Your app's file structure

There are 5 files that most Phosphorus Five apps, or modules more accurately will have. These are as follows.

* _"launch.hl"_, which is the file that is evaluated when your app is launched
* _"startup.hl"_, which is evaluated every time the server restarts for some reasons
* _"install.hl"_, which is evaluated when the app is installed
* _"uninstall.hl"_, which is evaluated when the app is uninstalled
* _"desktop.hl"_, which becomes your app's _"desktop icon"_

All of the above files are optional when creating a new module, and some modules will have some of the above
files, but not all of them. Our little _"Hello World"_ app will have all 5 of these files. This is because
it serves a dual purpose, which is to also be a nice starting ground for your own apps, as a _"template project"_
when you start out new projects. Feel free to open these files, and study them, to get more understanding of
what they actually do. They should be heavily commented.

### Analyzing our code

In our _"launch.hl"_ file, we have a **[create-widget]** invocation. This declares a widget for us. Our widget has
several arguments, such as a **[class]** node, and a **[widgets]** node. The latter contains our **[container]**
widget's children widgets. In addition, we invoke **[micro.css.include]**. This will include Micro's CSS files.
Micro is a subject of its own in this documentation, but basically it is an alternative to Bootstrap CSS.

### Ajax events

In our Hello World app's _"launch.hl"_ file, we have an **[onclick]** Ajax event for our button. This will
create an *"onclick"* DOM event handler for us, which when raised, will create an Ajax request for us. When
our Ajax request reaches our server, the lambda object inside of our **[onclick]** event handler will be
evaluated.

Lambda objects, such as the one we have declared inside our **[onclick]** event handler, is often referred
to as simply _"lambda"_. Lambda objects are basicall stored function objects, which are evaluated,
whenever some condition is being met - Or we wish to for some reasons execute our lambda. The simplicity
in declaring such _"lambda objects"_, is the reason why Hyperlambda got its name.

The *"lambda"* object for our _"Hello World"_ button, simply invokes the **[set-widget-property]** Active Event, with the ID of *"foo-button"*, 
and an **[innerValue]** argument of *"Hello World"*. This changes the **[innerValue]** property of our *"foo-button"* widget, to whatever HTML we 
pass in as the value of our **[innerValue]** argument.

### The CSS structure

Micro, which is the CSS framework we are using here, has a similar structure to Bootstrap CSS. Basically a _"container/row/col"_ structure, 
where each _"col"_ must be put into a _"row"_, and each _"row"_ must be put into a _"container"_. This is why we need to create some
wrapper widgets, wrapping our button, before we can declare our actual button. We could of course have create our button directly, without
neither any Micro CSS parts, nor any of the wireframing widgets around it - But this would create an _"ugly"_ result for us. Yet again, Micro
is the subject of another part of this documentation.

### The files

The _"install.hl"_ file is being evaluated by the core Desktop module when your application is installed.
While the _"uninstall.hl"_ file is evaluated when your app is uninstalled. In our app, these files are strictly
speaking not necessary - But since the _"Hello World"_ template also serves as a _"new module template"_, Hyper
IDE will automatically add these files, simply as dummy files for you, when you create a new project of
the type _"hello-world"_.

The _"launch.hl"_ file, is the file that is evaluated when a client requests the URL _"/hello-world"_, or the
URL to your app, whatever that is. While the _"desktop.hl"_ file, is expected to declare some widget, which
the Desktop module will inject into your main Desktop, serving as your app's _"desktop icon"_.

The _"startup.hl"_ file is evaluated every time your web server process is recycled, or your server is rebooted.
Basically, it is expected to contain initialization logic, such as creating any Active Events that your app
is dependent upon.

### Wrapping up

In this chapter we created our first real application, a _"Hello World"_ application, and we analyzed parts of
its content. Feel free to play with your Hello World application, and modify it as you see fit. Suggestion,
try to inject a **[video]** widget into it ...
