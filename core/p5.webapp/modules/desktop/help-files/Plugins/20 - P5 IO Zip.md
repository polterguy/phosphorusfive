
## p5.io.zip - Zipping and unzipping files in Phosphorus Five

This project contains helper Active Events to zip and unzip files and folders in your system. It consists of two
Active Events.

* __[zip]__
* __[unzip]__

### Zipping files

To create a zip file, you can use the **[zip]** Active Event. **[zip]** takes the path to its destination zip file as its value, and one or
more **[src]** nodes, which can be replaced with any Active Event source invocation if you wish. If you supply a **[src]** node, its value 
can be either one constant folder or filename, or an expression leading to multiple filenames. Example given below.

```hyperlambda
_files
  /web.config
  /Default.aspx
zip:~/temp/foo.zip
  src:x:/../*/_files/*?name
```

The above code assumes you're loggged in as root, since it tries to zip files, that are only accessible for root
accounts. The above lambda will create a "foo.zip" file, in your account's temporary folder. Notice how also this
Active Event allows for tilde substitutions, which will unroll to the currently logged in user's user files.

### Zipping entire folders

You can also **[zip]** entire folders, by instead of pointing to files, you'd be pointing to folders as your source.
The following code zips your entire modules folder.

```hyperlambda
zip:~/temp/modules.zip
  src:/modules/
```

### Unzipping files

To unzip an existing zip file, you can use the *[unzip]* Active Event. This time the **[src]** is expected to point
to a valid zip file, and the destination being the value of the **[unzip]** node, is the destination folder where
you wish to unzip your files. Assuming you zipped your _"/modules/"_ folder previously to your _"/temp/"_ folder,
you can unzip it with the following lambda.

```hyperlambda
unzip:~/temp/
  src:~/temp/modules.zip
```

### Encrypting your zip files

The **[zip]** Active Event, support encrypting your zip archive with AES encryption. This is easily done by
simply providing a **[password]** argument, and optionally a **[key-size]** argument. The password can be
anything you wish, just please remember it, otherwise you won't ever be able to unzip your archive correctly later.
The **[key-size]**, which is optionally, can be either 128 or 256, and defines how many bits the archive will
be AES encrypted with. The default value for **[key-size]** is 256.

Example of how to zip and encrypt your "/modules/" folder.

```hyperlambda
zip:~/temp/modules.zip
  src:/modules/
  password:foo-bar
```

If you create an encrypted zip archive, the only way to extract it, is by providing the same **[password]** when
invoking **[unzip]**. This makes sure that nobody can access the files inside of your zip archive, unless
they somehow manage to get to the password, or are able to guess the password somehow. Below is example of how
to unzip the files zipped in our above code.

```hyperlambda
unzip:~/temp/
  src:~/temp/modules.zip
  password:foo-bar
```

Notice, it is only the files themselves which are encrypted if you use this feature. This means that the
filenames and foldernames inside of your zip archive, will still be visible in plain text. The content of your
files, will be protected by AES encryption though. Have this in mind, as sometimes providing meta data,
such as filenames and folder names to an adversary, is almost equally dangerous as providing the files' content
themselves. Also please use strong passwords, such that an adversary cannot guess it in any ways.

If you wish to further hide, also the file names and folder names, you must either double-zip your archive,
or alternatively use PGP cryptography, using e.g. p5.mime, which actually also does a good job in reducing
the size of files, in addition to that it support PGP encryption.

p5.io.zip is internally using [SharpZipLib](https://icsharpcode.github.io/SharpZipLib/), which is licensed
as GPL with the GNU Classpath exception. Unless you modify p5.io.zip itself, this has no consequences for
you or your code. If you modify p5.io.zip, or the underlaying SharpZipLib library, you must release any
changes under the same license terms to all users of your modified library.

In fact, even if SharpZipLib was licensed as pure GPL, this would be no different for you, due to the dynamic
linking of Phosphorus Five, where you never physically link to the libraries (Active Event plugin projects)
that you create.

With P5, you can, as a general rule, link towards GPL code, and only the plugins you create, linking towards
the GPL code, will be affected by the GPL. Which is actually a really nifty feature of P5, for those not
wanting to open source license everything they create. This trait of P5, is possible because of that none
of your plugins, are ever statically linked towards the rest of your project in any ways.