p5.io, the main file input/output parts of Phosphorus Five
========

The p5.io library, and its Active Events, allows you to easily load, create, modify and delete files and folders in your system.
It contains all the methods expected to handle your file system, for most problems you'd encounter while using P5.

Notice that all IO operations within Phosphorus Five, and its "p5.io" library, expects the path you supply to start with a "/". If
what you are referring to, is a folder, it also expects you to _end_ your path with a forward slash (/). Unless you create your paths
like this, exceptions will occur during evaluation of your code.

Also realize that unless you are authorized to load, save, change or delete a specific file or folder, then a security exception will
be thrown by the framework. For instance, a user does not by default have access to files belonging to another user, existing within
another user's "home" folder. (/users/username/... folder)

## How to load, save, delete and query files in your system

Below you can find the documentation for how to handle files in your system.

### [load-file], loading files the easy way.

To load a file, simply use the *[load-file]* Active Event. An example of this event is shown below.

```
load-file:/system42/application-startup.hl
```

The above invocation will load the System42 "startup file" for you. Notice that this is a Hyperlisp file, which the *[load-file]* Active
Event will determine by itself, and automatically parse the file for you to a p5.lambda structure. If you do not wish to automatically
parse the file, but rather load is "raw", as a piece of text, not transforming it into a p5.lambda object for you, you must add up the
argument "convert" and set its value to "false". An example is shown below.

```
load-file:/system42/application-startup.hl
  convert:false
```

If you run the above Hyperlisp through your System42/executor, you will see that now it contains simply a text string, preserving all comments
for you, among other things. Unless you explicitly inform the *[load-file]* Active Event that you do not wish for any conversion to occur,
it will automatically convert all Hyperlisp for you, to p5.lambda objects. This makes it very easy for you to load Hyperlisp, and immediately 
execute your Hyperlisp, without having to convert it yourself.

#### Loading multiple files at the same time

Sometimes, you want to load multiple files at the same time. Often you might even want to treat them as "one aggregated" file result, 
for instance for those cases where you wish to load multiple files, and evaluate the combined result as a single piece of p5.lambda object.

For such cases, you can pass in an expression into  your *[load-file]* invocation, such as the following showss an example of.

```
_files
  file1:/system42/application-startup.hl
  file2:/system42/startup/pf.web.load-ui.hl
load-file:x:/-/*?value
```

The above code will load both of the given files, and append them into a node, beneath *[load-file]*, having the name being the path of
the file loaded. The structure will look roughly like this.

```
_files
  file1:/system42/application-startup.hl
  file2:/system42/startup/pf.web.load-ui.hl
load-file
  /system42/application-startup.hl
     ... file 1 content ...
  /system42/startup/pf.web.load-ui.hl
     ... file 2 content ...
```

### [save-file], without the hassle

The *[save-file]* Active Event, does the exact opposite of the *[load-file]* event. Try the following code.

```
save-file:/foo.txt
  src:@"Hello there stranger!
=======

I am a newly created file! :)"
```

After evaluating the above Hyperlisp, a new file will exist within your main "/phosphorusfive/core/p5.webapp/" folder called "foo.txt".

Whatever argument you pass into the *[src]* node, will somehow be converted into a text string, or a single binary piece of blob, and
flushed into the file path given as the value of *[save-file]*. This allows you to create a new file, consisting of the results of an 
expression, such as the following is an example of.

```
_data
  people
    name:Thomas Hansen
  foo-non-save
    name:Not saved!
  people:Howdy world
    first:John
    last:Doe
save-file:/foo.hl
  src:x:/../*/_data/*/people
load-file:/foo.hl
```

The *[load-file]* invocation above, is there simply to show the results of your newly created file, and illustrates how only the results of
the expression you pass into your *[src]* node becomes saved to disc. This allows you to save sub-sections for your trees, and even combine
multiple pieces of text and/or p5.lambda, and save the combined results to disc.





