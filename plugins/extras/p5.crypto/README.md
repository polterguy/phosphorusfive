Cryptography in Phosphorus Five
===============

Actually, most cryptography features in Phosphorus Five, can be found indirectly, in the [p5.mime](/plugins/extras/p5.mime/) project.
p5.mime is using MimeKit internally, and allows for PGP encryption, decryption, and cryptographically signing MIME messages. However, some
Active Events, which doesn't necessarily belong together with MIME, can be found in this p5.crypto. These Active Events are mostly events
related to creating a cryptographically secure hash, in addition to creating a cryptographic random number.

## Creating a cryptographic hash

There are two Active Events in P5 related to creating a hash.

* [p5.crypto.create-sha256-hash], creates a 256 bits SHA hash of the given data
* [p5.crypto.create-sha512-hash], creates a 512 bits SHA hash of the given data

Below is an example of how to use them both.

```
_hash-data:This string, once hashed, cannot be determined, unless you have the original string
p5.crypto.create-sha256-hash:x:/-?value
p5.crypto.create-sha512-hash:x:/-2?value
```

The first invocation, will create a 512 bit long SHA of the given data, the second a 512 bit long SHA. Hashed values are used in P5, among other things,
when storing passwords and such. To understand how hashed values work, and when and how to use them, feel free to check them out at e.g. WikiPedia
or something similar.

In P5, both of these Active Events, returns the base64 encoded values of the SHA created back to caller.

## Creating a cryptographically secure random number

When dealing with cryptography, acquiring random numbers is key. P5 have one Active Event related to creating a cryptographically secure random number.
This Active Event is called *[p5.crypto.create-random]*. Below is an example of how to use it.

```
p5.crypto.create-random
  resolution:10
```

The above invocation, will fill the return value of the invocation node, with 10 bytes of random numbers, ranging from 0-255. Then it will base64 encode
its result, and return to caller. This event is internally used in P5 to "seed" the generator when creating PGP keypairs, among other things. For those
curious, it is not the only seed source used.



