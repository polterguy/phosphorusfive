Zipping and unzipping files in Phosphorus Five
===============

This project contains helper Active Events to zip and unzip files and folders in your system. It consists of two Active Events.

* [zip]
* [unzip]

## Zipping files

To create a zip file, you use the *[zip]* Active Event. *[zip]* takes the path to its destination zip file as its value, and a *[src]* node,
which can be replaced with any Active Event source invocation if you wish. If you supply a *[src]* node, its value can be either one constant 
filename, or an expression leading to multiple filenames. Example given below.

```
_files
  /web.config
  /Default.aspx
zip:~/temp/foo.zip
  src:x:/../*/_files/*?name
```

The above code assumes you're loggged in as "root", since it tries to zip files, that are only accessible for root accounts.
The above lambda will create a "foo.zip" file, in your account's temporary folder. Notice how also this Active Event allows for "tilde 
substitutions", which will unroll to the currently logged in user's user files.

### Zipping entire folders

You can also *[zip]* entire folders, by instead of pointing to files, you'd be pointing to folders as your source. The following code zips your
entire "system42" folder.

```
zip:~/temp/system42.zip
  src:/system42/
```

## Unzipping files

To unzip an existing zip file, you can use the *[unzip]* Active Event. This time the *[src]* is expected to point to a valid zip file, and 
the destination, being the value of the *[unzip]* node, is the destination folder where you wish to unzip your files. Assuming you zipped your
"system42" folder previously to your "temp" folder, you can unzip it with the following lambda.

```
unzip:~/temp/
  src:~/temp/system42.zip
```

## Encrypting you zip files

The *[zip]* Active Event, support encrypting your zip archive with AES encryption. This is easily done by simply providing a *[password]* argument,
and optionally a *[key-size]* argument. The *[password]* can be anything you wish, just please remember it, otherwise you won't ever be able to unzip
your archive correctly. The *[key-size]*, which is optionally, can be either 128 or 256, and defines how many bits the archive will be AES encrypted
with. The default value for *[key-size]* is 256.

Example of how to zip and encrypt your "/system42/startup/" folder.

```
zip:~/temp/startup.zip
  src:/system42/startup/
  password:foo-bar
```

Notice, that it is only the files which are encrypted if you use this feature. This means that the filenames and foldernames inside of your
zip archive, will still be visible in plain text. But the content of your files, will be protected by AES encryption.

If you wish to further hide, also the file names and folder names, you must either double-zip your archive, or alternatively use PGP cryptography,
using e.g. [p5.mime](/plugins/extras/p5.mime/), which actually also does a good job in reducing the size of files.

p5.io.zip is internally using [SharpZipLib](https://icsharpcode.github.io/SharpZipLib/), which is licensed as GPL with the GNU Classpath exception.
Unless you modify p5.io.zip itself, this has no consequences for you and your code. If you modify p5.io.zip, or the underlaying SharpZipLib library,
you must release any changes back under the same license terms.

In fact, even if SharpZipLib was licensed as "pure" GPL, this would be no different for you, due to the dynamic linking of Phosphorus Five,
where you never physically link to the libraries (Active Event plugin projects) you create.

