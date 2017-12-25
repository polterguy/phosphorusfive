Cryptography in Phosphorus Five
===============

Although most cryptography features in Phosphorus Five, can be found indirectly, in the [p5.mime](/plugins/extras/p5.mime/) project -
Some cryptography helper events can be found in this project.

## Creating a cryptographic hash

There are two Active Events in P5 related to creating a hash.

* __[p5.crypto.hash.create-sha1]__, creates a SHA1 hash of the given data
* __[p5.crypto.hash.create-sha1-file]__, creates SHA1 hash of the given file
* __[p5.crypto.hash.create-sha256]__, creates a 256 bits SHA hash of the given data
* __[p5.crypto.hash.create-sha256-file]__, creates a 256 bits SHA hash of the given file
* __[p5.crypto.hash.create-sha512]__, creates a 512 bits SHA hash of the given data
* __[p5.crypto.hash.create-sha512-file]__, creates a 512 bits SHA hash of the given file

Below is an example of how to use them.

```
_hash-data:This string, once hashed, cannot be determined, unless you have the original string
p5.crypto.hash.create-sha256:x:/-?value
p5.crypto.hash.create-sha512-file:/web.config
```

The first invocation, will create a 256 bit long SHA of the given data, the second will create a 512 bit long SHA of the specified file. Hashed values are used in P5, 
among other things when storing passwords. To understand how hash values work, and when and how to use them, feel free to check them out at e.g. WikiPedia.
In P5, both of these Active Events, returns the base64 encoded values of the hash created back to caller. But all hash active events also optionally
takes a parameters **[raw]**, which will return the bytes as raw bytes - Or optionally a **[hex]**, which will return the hex values of the hash.
The **[raw]** arguments and the **[hex]** argument, are obviously mutually exclusive.

## Creating a cryptographically secure random number

When dealing with cryptography, acquiring random numbers is crucial. P5 have one Active Event related to creating a cryptographically secure random number.
This Active Event is called **[p5.crypto.create-random]**. Below is an example of how to use it.

```
p5.crypto.create-random
  resolution:10
```

The above invocation, will fill the return value of the invocation node, with 10 bytes of random numbers, ranging from 0-255. Then it will base64 encode
its result, and return to caller. This event is internally used in P5 to seed the random number generator when creating PGP keypairs, among other things. 
For the record, it is _not the only_ seed source used. This even also optionally takes a **[raw]** or a **[hex]** value, declaring what type of return
value you wish to retrieve.
