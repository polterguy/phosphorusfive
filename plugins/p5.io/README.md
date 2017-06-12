﻿File IO in Phosphorus Five
========

The p5.io library, and its Active Events, allows you to easily load, create, modify, and delete files and folders in your system.
It contains most methods necessary to handle your file system, for most problems you'd encounter, while using P5. Although mostly
useful for text files, it also to some extent allows handling binary files.

Notice, that all IO operations within Phosphorus Five, and its "p5.io" library, expects the path you supply to start with a "/". If
what you are referring to is a folder, it also expects you to _end_ your path with a forward slash (/). Unless you create your paths
like this, exceptions will be thrown during evaluation of your code.

Regardless of which platform you are using underneath, you'll have to use forwards slash "/" to separate folders. This is true for both
Windows, Linux and Mac OS.

Also realize, that unless you are authorized to load, save, change, or delete a specific file, or folder, then a security exception will
be thrown. For instance, a user does not by default have access to files belonging to another user, existing within 
another user's "home" folder. (e.g. /users/username/some-folder/)

These authorisation and authentication features of P5 are implemented in the [p5.io.authorization](/plugins/extras/p5.io.authorization/) project,
but can easily be exchanged with your own logic, if you need more fine-grained access control.

Notice also, that all file IO Active Events in p5.io, relies upon the type conversion, normally implemented in [p5.types](/plugins/p5.types/), 
which in turn will use UTF8 exclusively, as its conversion encoding, when for instance saving files, and also loading files. This means that
all files created, using p5.io, will be created as UTF8 files. In addition, all files loaded with p5.io, will be assumed to be encoded as
UTF8. This is true for all text files. However, binary data can still be saved as such.

In general, at the time of this writing, p5.io exclusively support UTF8 text files, in addition to some rudimentary support for binary files.
Hyperlambda files, are considered text files.

All Active Events in p5.io, will also automatically substitute a path, with "/users/logged-in-username" if it starts with `~`. For instance, 
if you are logged in as username "root", then `~/documents/foo.txt` will unroll to "/users/root/documents/foo.txt". This allows you to
transparently refer to files in a user's folder as `~/something.txt`. This is the only exception to the rule of that any paths must start with 
a slash "/".

Also notice, that although you _can_ load and save binary data with p5.io - Hyperlambda and p5.lambda, is not in general terms, very adequate
for manipulating binary data. This means that you can load binary data, but for the most parts, the only intelligent thing you can do
with it, is to base64 encode this data, and/or, pass it into other Active Events, that knows how to handle your binary data.

## Handling files in your system

Below you can find the documentation for how to handle files in your system.

### [load-file], loading files

To load a file, simply use the *[load-file]* Active Event. An example of this event is shown below.

```
load-file:/application-startup.hl
```

The above invocation will load P5's startup file for you. Notice that this is a Hyperlambda file, which the *[load-file]* Active
Event will automatically determine, and hence parse the file for you automatically, to become a lambda structure. If you do not wish to 
automatically parse the file, but rather load the file "raw", as a piece of text, not transforming it into a lambda object, you must 
add the argument *[convert]*, and set its value to "false". An example is shown below.

```
load-file:/application-startup.hl
  convert:false
```

If you run the above Hyperlambda through for instance your [System42](https://github.com/polterguy/system42) Executor, you will see that 
it now contains simply a text string, preserving all comments
for you, among other things. Unless you explicitly inform the *[load-file]* Active Event that you do not wish for any conversion to occur,
then it will automatically convert all Hyperlambda for you, to lambda objects. This makes it very easy for you to load Hyperlambda, and 
immediately execute your Hyperlambda, without having to convert it yourself.

#### Loading multiple files at the same time

Sometimes, you want to load multiple files at the same time. Often you might even want to treat them as "one aggregated" file result.
For such cases, you can pass in an expression into your *[load-file]* invocation, such as the following is an example of.

```
_files
  file1:/application-startup.hl
  file2:/some-other-file.hl
load-file:x:/-/*?value
```

The above code, will load both of the given files, and append them into a node, beneath *[load-file]*, having the name being the path of
the file loaded. The structure will look roughly like this.

```
_files
  file1:/application-startup.hl
  file2:/some-other-file.hl
load-file
  /application-startup.hl
     ... file 1 content, lambda nodes ...
  /some-other-file.hl
     ... file 2 content, lambda nodes ...
```

Notice, if you try to load a file that does not exist, an exception will be thrown.

### [save-file], saving files

The *[save-file]* Active Event, does the exact opposite of the *[load-file]* event. Meaning, it saves a new file, or overwrites an existing on.
Try the following code.

```
save-file:~/foo.md
  src:@"Hello there file system!
=======

I am a newly created markdown file!"
```

After evaluating the above Hyperlambda, a new file will exist within your main `~/` user's folder, called "foo.md".
If the file already exists, it will be overwritten.

Whatever argument you pass into the *[src]* node, will somehow be converted into a text string, or a single binary piece of blob, and
flushed into the given file, passed in as the value of *[save-file]*. This allows you to create a new file, or overwrite an existing file.
You can also use expression in your *[src]*, such as the following is an example of.

```
_data
  people
    name:Thomas Hansen
  foo-non-save
    name:Not saved!
  people:Howdy world
    first:John
    last:Doe
save-file:~/foo.hl
  src:x:/../*/_data/*
load-file:~/foo.hl
```

The *[load-file]* invocation above, is only there to show the results of your newly created file, and illustrates how only the results of
the expression you pass into your *[src]* to *[save-file]*, are saved to disc. This allows you to save sub-sections of your trees, and even 
combine multiple pieces of text, and/or p5.lambda sub-trees, and save the combined result to disc.

You can also have a "static" source, containing the nodes as children of *[src]*, such as the following is an example of.

```
save-file:~/foo.hl
  src
    person:1
      name:thomas
    person:2
      name:john
```

Yet even more powerful control over what is saved, can be achieved by using an Active Event source. Consider the following code.

```
_exe
  return:Content of file
save-file:~/foo.txt
  eval:x:/../*/_exe
```

The above example, will create a file, named "foo.txt", at the root of your p5.webapp folder, who's content is "Content of file".

#### Saving multiple files, with relative sources

If you wish, you can save multiple files at the same time, and use expressions pointing to your filenames, having the content of your files, 
be relative to your filename node. Imagine the following code.

```
_files
  name:~/foo.txt
    content:Foo was here
  name:~/bar.txt
    content:Bar was here
save-file:x:/-/*?value
  eval:x:/./+
_get-content
  return:x:/../*/_dn/#/*?value
```

What the above lambda object does, is to iterate each filename given in value of the *[name]* nodes beneath *[_files]*. For then to invoke *[eval]*,
once for each destination, passing in *[_dn]*, being relative for each destination. Then our *[_get-content]* lambda object, returns a relative 
source, expected to be the child node(s) beneath each filepath nodes.

This "Ninja trick" allows you to save multiple files, in one go.

### [p5.io.file.delete], deleting one or more files

Just like *[load-file]*, *[p5.io.file.delete]* can react upon several files at the same time. Its arguments work the same way as load-file, except of
course, instead of loading the file(s), it deletes them. To delete the files created above in one of our *[save-file]* examples, you
can use the following code.

```
p5.io.file.delete:~/foo.txt
```

Notice, if the above file does not exist, an exception will be thrown.

The Active Event *[p5.io.file.delete]*, does not take any arguments, besides a single constant value, or an expression leading to multiple file paths.
However, just like the other file manipulation Active Events, it requires a fully qualified path, which must start with "/". To delete a file,
the user context object must be authorized to modifying the file. Otherwise, an exception will be thrown.

### [p5.io.file.exists], checking if one or more files exist

*[p5.io.file.exists]* accepts its arguments the same way *[load-file]* does. However, *[p5.io.file.exists]* will return true for each file that
exists, instead of returning the content of the file. Example given below.

```
_data
  file1:/system42/application-startup.hl
  file2:/does-not-exist/foo.txt
p5.io.file.exists:x:/-/*?value
```

Notice how the above example returns true for the first file, but false for the second file.

### [p5.io.file.move], moving or renaming a file

With *[p5.io.file.move]*, you can either rename a file, or entirely move it into for instance a different folder. The Active Event takes the 
"source file" as its value, and the "destinatin filepath/value", as the value of a *[dest]* child node. Let's show this with an example.

```
save-file:~/foo.txt
  src:foo bar
p5.io.file.move:~/foo.txt
  dest:~/new-foo.txt
```

To move multiple files in one go, you could do something similar to this.

```
save-file:~/temp/foo1.txt
  src:foo1
save-file:~/temp/foo2.txt
  src:foo2
p5.io.file.move:x:/../*/save-file?value
  eval
    p5.string.split:x:/../*/_dn/#?value
      =:.
    return:{0} - new path.{1}
      :x:/../*/p5.string.split/0?name
      :x:/../*/p5.string.split/1?name
```

The above p5.lambda, first creates two text files. Then it uses an expression leading to each *[save-file]*'s value for *[p5.io.file.move]*, with
an Active Event destination, utilizing the *[eval]* Active Event, which returns the old filename, with the text " - new path" appended at 
its end. The end result being, that you end up with two files at the root of your "p5.website" folder called "foo1 - new path.txt" and
"foo2 - new path.txt".

The *[p5.io.file.move]* Active Event, also has the alias of *[p5.io.file.rename]*, which can be used instead of "p5.io.file.move". However, the logic is the
exact same, and there are no differences in implementation of these two events. They are simply aliases for the same Active Event handler.

If the files you are trying to move, does not exist, an exception will be thrown. If there exist a file from before, with the same path as the
new destination filenames for your file(s), then an exception will also be thrown.

### [p5.io.file.copy], copying a file

The *[p5.io.file.copy]* Active Event, does exactly what you think it should do. It copies one source file, and creates a new copy of that file, into
a destination file. Besides from that it actually copies the file(s), instead of moving them, it works 100% identically to *[p5.io.file.move]*. 
The arguments to *[p5.io.file.copy]* are also the same as the arguments to *[p5.io.file.move]*. Consider this code.

```
save-file:~/foo.txt
  src:foo bar
p5.io.file.copy:~/foo.txt
  dest:~/foo-copy.txt
```

The *[dest]* node argument above, which is the child node of *[p5.io.file.copy]*, is of course the destination filepath, for your copy. Here too, you
could have copied several files at once, like we did with *[p5.io.file.move]*.

### [p5.io.file.length], [p5.io.file.get-read-only], [p5.io.file.creation-time] and [p5.io.file.last-access-time]

The *[p5.io.file.length]*, *[p5.io.file.get-read-only]*, *[p5.io.file.creation-time]* and *[p5.io.file.last-access-time]* Active Events, returns the size, read-only state,
creation time, and access time of each file you supply to them.

```
p5.io.file.length:/web.config
p5.io.file.get-read-only:/web.config
p5.io.file.creation-time:/web.config
p5.io.file.last-access-time:/web.config
```

After evaluating the above code, your result will look something like this.

```
p5.io.file.length
  /web.config:long:8084
p5.io.file.get-read-only
  /web.config:bool:false
p5.io.file.creation-time
  /web.config:date:"2016-09-07T15:25:22.285"
p5.io.file.last-access-time
  /web.config:date:"2016-09-18T23:18:41.166"
```

#### Changing the read-only state of a file

You can set one or more files to "read-only" with the *[p5.io.file.set-read-only]* Active Event. In addition, you can remove the "read-only" attribute,
using the *[p5.io.file.delete-read-only]* Active Event. Both of these Active Events takes either a constant or an expression, leading to multiple files.

## How to handle folders in your system

The Active Events for handling folders, are almost identical to the events for handling files, with some smaller differences though.
Among other things, there obviously does not exist a *[save-folder]* event, but instead you'll find a *[p5.io.folder.create]* Active Event,
and so on.

### [p5.io.folder.create]

Creates a folder at the given path. Notice that the parent folder must exist, and that this Active Event does not "recursively" create folders.
Also notice that if the folder exist from before, an exception will be thrown.

This Active Event also handles expressions, and will create all folders your expressions yields as a result, the same way for instance 
the *[load-file]* will load multiple files.

Every single Active Event that somehow takes a folder, requires the path to both start with a slash (/), in addition to ending with a slash (/).

Below is some example code that creates two folders.

```
_folders
  folder1:~/foo/
  folder2:~/bar/
p5.io.folder.create:x:/-/*?value
```

### [p5.io.folder.delete]

Delete folder is implemented with the same semantics as *[p5.io.folder.create]*, except of course, instead of creating folders, it deletes them.
Example code below.

```
_folders
  folder1:~/foo/
  folder2:~/bar/
p5.io.folder.delete:x:/-/*?value
```

The above code will delete the folders previously created in our *[p5.io.folder.create]* example.

### [p5.io.folder.exists]

This Active Event is implemented with the same semantics as *[p5.io.file.exists]*, which means if you pass in an expression as its value, and the 
expression is leading to multiple folder paths, then all folders must exist, in order for the Active Event to return "true". Below we are
checking if the folder "/system42/" exists, without any expressions as arguments. We could have supplied an expression, either leading to
a single path, or multiple paths, if we wanted.

```
p5.io.folder.exists:/system42/
```

### [p5.io.folder.copy] and [p5.io.folder.move]

These two Active Events works exactly like their "file counterparts" ([p5.io.file.copy] and [p5.io.file.move]). The *[p5.io.folder.move]* even has an alias,
just like "p5.io.file.move", which is *[p5.io.folder.rename]*. Below is some sample code using them both.

```
p5.io.folder.create:~/foo-bar/
p5.io.folder.create:~/foo-bar/foo-bar-inner/

// Creating some dummy text file in folder
save-file:~/foo-bar/foo.txt
  src:Foo bar text file
save-file:~/foo-bar/foo-bar-inner/foo2.txt
  src:Foo bar text file

// Then copying the folder we created
p5.io.folder.copy:~/foo-bar/
  dest:~/foo-bar-2/

// Before finally, we move the original folder we created above
// BTW, we could also have used [p5.io.folder.rename] here
p5.io.folder.move:~/foo-bar/
  dest:~/foo-bar-new-name/
```

The above code first creates a folder with an inner folder. Then, for the example, it creates a couple of files within these two folders.
Afterwards, it copies the root folder created like this, before it renames the original root folder created.

### [p5.io.folder.list-files] and [p5.io.folder.list-folders]

These two Active Events, allows you to list files or folders in your system. Both of them can be given either a constant as a value, or
an expression, leading to multiple folder paths. An example is given below.

```
p5.io.folder.list-files:/system42/
p5.io.folder.list-folders:/system42/
```

If you evaluate the above Hyperlambda, you will see that these Active Events returns the files and folders, as the "name" part of their children
nodes. This is a general rule in p5.lambda, which is that in general terms, Active Events that returns a list of strings, returns these as 
the names of the children nodes of their main event node.

Notice the *[p5.io.folder.list-files]* Active Event, can optionally be given a *[filter]*. This is a piece of string, that each file must contain, to yield a match.
For instance, to list only the Hyperlambda files in your System42 folder, you could do something like this.

```
p5.io.folder.list-files:/system42/
  filter:.hl
```

Notice, if you start your filter with a period ".", then *[p5.io.folder.list-files]* assumes that you wish to filter upon file extensions. Otherwise, it will simply
retrieve all files somehow containing your specified search term. Regardless of where this is found in the filename.

#### Filtering files according to type

When you invoke *[p5.io.folder.list-files]*, you can optionally supply a *[filter]* argument, to make sure you only retrieve files with a
specific extension. The code below for instance, will only retrieve the ".aspx" files from your p5.webapp folder.

```
p5.io.folder.list-files:/
  filter:.aspx
```

### Ninja tricks in p5.io

Below are some "Ninja tricks" when working with p5.io.

#### Using path variables

p5.io supports "unrolling paths". This feature allows you to instead of hard coding a path, you can create a variable Active Event, which is invoked
if your path starts with an "@" character. If you defined an Active Event called for instance *[p5.io.unroll-path.@my-documents]*, then you can
refer to your documents folder using _@my-documents_. This gives you more flexibility in regards to your paths, and allows you to later change your
mind, or even install your apps in a different folder, than what you originally decided upon when creating it.

Below is some sample code that creates such a variable Active Event, for then to consume it when creating a file.

```
create-event:p5.io.unroll-path.@my-temp
  return:~/temp
save-file:@my-temp/foo.txt
  src:Foo bar
```

After evaluating the above code, you should have a foo.txt file in your temp folder.

#### Executing every Hyperlambda file within a folder

Combining *[p5.io.folder.list-files]* and *[eval]*, you can do some interesting things. One of these, is that you can evaluate all Hyperlambda files within
some specific folder, easily, with only 3-4 lines of code. Imagine the following code.

```
p5.io.folder.list-files:/system42/some-hyperlambda-folder/
  filter:.hl
load-file:x:/-/*?name
eval:x:/-/*
```

What the above code actually does, is first of all listing every Hyperlambda file with a specific folder. Then it loads all these files.
As we previously said, *[load-file]* will automatically convert a Hyperlambda file to a p5.lambda structure after loading it. Then we invoke
the *[eval]* event, passing in an expression leading to all children nodes of *[load-file]*, which now should be the root node of all files 
loaded this way. The end result, is that all files in some specific folder is automatically evaluated and executed.

System42 contains helper Active Events, both for evaluating single Hyperlambda files, in addition to recursively evaluating all Hyperlambda
files within some specified folder. These are listed below.

* sys42.utilities.execute-lambda-file - Evaluates one or more Hyperlambda files. Pass in either a constant, or an expression leading to one or more files.
* sys42.utilities.execute-lambda-folder - Evaluates all Hyperlambda files within one or more specified folders.

#### Additional helper Active Events

If you create your own file plugin, that uses paths, you can use the *[.p5.io.unroll-path]* Active Event, to make sure for instance "~" is
replaces with the user's home directory. This Active Event takes a path as its input, and returns the "unrolled path" back to caller.

## Warning! WRITE PORTABLE CODE!

The underlaying filesystem on Windows and Linux/Mac OS, have big differences. I have tried to accommodate for most of these differences, to create a
uniform and common way of typing out paths and such. One example is that I consistently replace backslash "\" with forward slash "/", and P5 doesn't 
even accept backslash as a part of a path. However, a filename in Windows called "FOO.txt" points to the same file as "foo.TXT". Windows does not 
differentiate between CAPS in filenames. Linux and Mac OS does however discriminate between CAPS!

This means that you can read the FILENAME.TXT as the same file called "filename.txt" on Windows. When you later port your code to your Linux server,
your code will no longer work, because "FILENAME.TXT" is no longer the same as "filename.txt".

Therefor I highly encourage you to use the _correct_ CAPS of your filenames when developing your apps. Regardless of whether or not it works on Windows.
Otherwise your code will not be portable to run on Linux servers.

I could have verified that filenames had the right CAPS, but this would blow up the code, and use additional resources on your system. Therefor I have
chosen to (for now) allow the underlaying filesystem take care of usage of CAPS. Have this in mind though as you create your own apps!

