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

### [load-file], loading files

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

### [save-file], saving files

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
the expression you pass into your *[src]* node, are saved to disc. This allows you to save sub-sections of your trees, and even combine
multiple pieces of text, and/or p5.lambda, and save the combined results to disc.

You can also use Active Event invocations as an alternative to the *[src]* node. Conssider the following code.

```
_exe
  return:Content of file
save-file:/foo.txt
  eval:x:/../*/_exe
```

The above example, will create a file, named "foo.txt", at the root of your p5.webapp folder, who's content is "Content of file".

### [delete-file],deleting one or more files

Just like *[load-file]*, *[delete-file]* can react upon several files at the same time. Its arguments work the same way as load-file, except of
course, instead of loading the file(s), it deletes them instead. To delete the files created above in one of our *[save-file]* examples, you
can use the following code.

```
delete-file:/foo.txt
```

The Active Event *[delete-file]*, does not take any arguments, besides a single constant value, or an expression leading to multiple file paths.
However, just like the other file manipulation Active Events, it requires a fully qualified path, which must start with "/". To delete a file,
the user context object, must be authorized to deleting it. Otherwise, and exception will be thrown.

### [file-exist], checking if files exist

Also *[file-exist]* takes its arguments the same way as for instance *[delete-file]* does. However, *[file-exist]* will return true, only if 
all files you check the existance of actually exists. If one of the files does not exist, then *[file-exist]* will return false. Let us show
that with an example.

```
// This file exist
file-exist:/system42/application-startup.hl

// Data segment where one of the files does not exist
_data
  file1:/system42/application-startup.hl
  file2:/does-not-exist/foo.txt
file-exist:x:/-/*?value
```

As you can see in the first invocation to *[file-exist]*, it yields "true", while the second invocation yields "false", since the "foo.txt"
file does not exist.

### [move-file], moving or renaming a file

With *[move-file]*, you can either rename a file, or entirely move it into for instance a different folder. The Active Event takes the 
"source file" as its value, and the "destinatin filepath/value" as the value of a *[to]* child node. Let's show this with an example.

```
save-file:/foo.txt
move-file:/foo.txt
  to:/new-foo.txt
```

Although *[move-file]* perfectly handles expressions, it does not accept an expression leading to multiple sources. Neither as its "source",
nor as its "destination".


