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

Notice also that all file manipulation Active Events in p5.io, relies upon the type conversion, normally implemented in "p5.types", which
again will use UTF8 exclusively as its conversion encoding, when for instance saving files, and also loading files. This means that
all files created using p5.io will be created as UTF8 files. In addition, all files loaded with p5.io, will be loaded with UTF8 encoding.

In general, at the time of this writing, p5.io exclusively support UTF8 text files.

Also notice that although you _can_ load and save binary data with p5.io, Hyperlisp and p5.lambda, is not in general terms very adequate
for manipulating binary data. This means that you can load binary blob data, but for the most parts, the only intelligent thing you can do
with it, is to base64 encode this data, and/or pass it into other Active Events that knows how to handle your binary data.

## How to handle files in your system

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

#### Saving multiple files, with relative sources

If you wish, you can save multiple files at the same time, and use expressions pointing to filenames, and content of your files be relative
to your filename node. Imagine the following code.

```
_files
  name:/foo.txt
    content:Foo was here
  name:/bar.txt
    content:Bar was here
save-file:x:/-/*?name
  eval:x:/./+
_get-content
  return:x:/../*/_dn/#/*/content?value
```

What the above lambda object does, is to iterate each filename given in the *[name]* nodes beneath *[_files]*, for the to invoke the *[eval]*
Active Event once for each destination, passing in the *[_dn]* being relative for each destination. Then our *[_get-content]* lambda object,
returns  relative source, expected to be a *[content]* node, beneath each filepath.

This "Ninja trick" allows you to save multiple files, in one go.

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
  src:foo bar
move-file:/foo.txt
  to:/new-foo.txt
```

Although *[move-file]* perfectly well handles expressions, it does not accept an expression leading to multiple sources. Neither as 
its "source", nor as its "destination".

The *[move-file]* Active Event, also has the alias of *[rename-file]* which can be used instead of "move-file". However, the logic is the
exact same, and there is no difference in implementation of these two events. They are simply aliases for the same Active Event handler.

### [copy-file], copying a file into a new file

The *[copy-file]* Active Event, does exactly what you think it would do. It copies one source file, and creates a new copy of that file, into
a destination file. The arguments to *[copy-file]* are the same as the arguments to *[move-file]*. Consider this code.

```
save-file:/foo.txt
  src:foo bar
copy-file:/foo.txt
  to:/foo-copy.txt
```

The *[to]* node argument above, which is the child node of *[copy-file]*, is of course the destination filepath, for your copy.

## How to handle folders in your system

The Active Events for handling folders, are almost identical to the events for handling files, with some smaller differences though.
Among other things, there obviously does not exist a *[save-folder]* event, but instead you'll find a *[create-folder]* Active Event,
and so on.

### [create-folder]

Creates a folder at the given path. Notice that the parent folder must exist, and that this Active Event does not "recursively" create folders.
Also notice that if the folder exist from before, an exception will occbr thrown.

This Active Event also handles expressions, and will create all folders your expressions yields as a result, the same way for instance 
the *[load-file]* would load multiple files. Below is some example code that creates two folders.

Every single Active Event that somehow takes a folder, requires the path to both start with a slash (/), in addition to ending with a slash (/).

```
_folders
  folder1:/foo/
  folder2:/bar/
create-folder:x:/-/*?value
```

### [delete-folder]

Delete folder is implemented with the same semantics as *[create-folder]*, except of course, instead of creating folders, it deletes them.
Example code below.

```
_folders
  folder1:/foo/
  folder2:/bar/
delete-folder:x:/-/*?value
```

The above code will delete the folders previously created in our *[create-folder]* example.

### [folder-exist]

This Active Event is implemented with the same semantics as *[file-exist]*, which means if you pass in an expression as its value, and the 
expression is leading to multiple folder paths, then all folders must exist, in order for the Active Event to return "true". Below we are
checking if the folder "/system42/" exists, without any expressions as arguments.

```
folder-exist:/system42/
```

### [copy-folder] and [move-folder]

These two Active Events works exactly like their "file counterparts" ([copy-file] and [move-file]). The *[move-folder]* even has an  alias,
just like "move-file", which is *[rename-folder]*. Below is some sample code using them both.

```
create-folder:/foo-bar/
copy-folder:/foo-bar/
  to:/foo-bar-2/

// We could also use [rename-folder] here
move-folder:/foo-bar/
  to:/foo-bar-new-name/
```

### [list-files] and [list-folders]

These two Active Events allows you to list files or folders in your system. Both of them can be given either a constant as a value, or
an expression leading to multiple folder paths. An example is given below.

```
list-files:/system42/
list-folders:/system42/
```

If you evaluate the above Hyperlisp, you will see that these Active Events returns the files and folders as the "name" part of their children
nodes. This is a general rule in p5.lambda, which is that many Active Events that returns a list of strings, returns these as the names of
the children nodes of their main event node.

#### Filtering files according to type

When you invoke *[list-files]*, you can optionally supply a *[filter]* argument, to make sure you only retrieve files with a
specific extension. The code below for instance, will only retrieve the ".aspx" files from your p5.webapp folder.

```
list-files:/
  filter:aspx
```

### Ninja tricks in p5.io

Below are some "Ninja tricks" when working with p5.io.

#### Automatically getting a unique new folder/file name

If you move, rename or copy a file or folder, and the destination file or folder already exist, then p5.io will create a new unique 
file/folder path for you, automatically, and use this instead of your originally requested path. Regardless of whether or not p5.io
creates a new unique path for you, it will return the path it uses for the destination, as the value of the invocation node. Look
at this code after evaluation to see this in action.

```
create-folder:/foo-bar/
create-folder:/foo-bar-2/
copy-folder:/foo-bar-2/
  to:/foo-bar/
```

After evaluating the above Hyperlisp, you will see that the value of your *[copy-folder]* node, will probably look something like this
"/foo-bar copy 2/". This was because the destination value of your copy-folder invocation was a path to a folder that already exist.

#### Executing every Hyperlisp file within a folder

Combining *[list-files]* and *[eval]*, you can do some interesting things. One of these, is that you can evaluate all Hyperlisp files within
some specific folder, easily, with only 3-4 lines of code. Imagine the following code.

```
list-files:/system42/some-hyperlisp-folder/
  filter:hl
load-file:x:/-/*?name
eval:x:/-/*
```

What the above code actually does, is first of all listing every Hyperlisp file with a specific folder. Then it loads all these files.
As we previously said, *[load-file]* will automatically convert a Hyperlisp file to a p5.lambda structure after loading it. Then we invoke
the *[eval]* event, passing in an expression leading to all children nodes of *[load-file]*, which now should be the root node of all files 
loaded this way. The end result is that all files in some specific folder is automatically evaluated and executed.

PS!
For the record, System42 contains a helper Active Event that does this for you, which you probably rather should use, instead of creating your
own logic.




