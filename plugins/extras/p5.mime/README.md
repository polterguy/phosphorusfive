Parsing and creating MIME messages in Phosphorus Five
===============

MIME stands for "Multi-purpose Internet Message Exchange", and is usually associated with email. However, in Phosphorus Five, there exists
many other use cases for MIME. Among other things, you can use them as the basis for web service invocations, from one server to another.

This allows you to use the PGP cryptography features of MIME, when invoking web services, or using MIME for any other purpose.

## Creating your first MIME message

To create a simple MIME message, you could do something like this.

```
p5.mime.create
  text:plain
    content:Foo bar
```

The above code, would result in a single MIME entity, with the "Content-Type" of "text/plain", having the content of "Foo bar". The MIME entity, will be
returned as a *[result]* node of the main invocation node. Resulting in the following.

```
p5.mime.create
  result:@"Content-Type: text/plain

Foo bar"
```

You can create multiple MIME entities at once, by simply adding another child node to *[p5.mime.create]*. However, this will not result in a multipart,
which has to be done by explicitly changing the "Content-Type", through the main entity node, such as the following is an example of.

```
p5.mime.create
  multipart:mixed
    text:plain
      content:Some text content
    text:html
      content:<p>Some <strong>HTML</strong> content</p>
```

The above would result in the following.

```
p5.mime.create
  result:@"Content-Type: multipart/mixed; boundary=""=-BDZlXJLJY5lTfzoutnnYew==""

--=-BDZlXJLJY5lTfzoutnnYew==
Content-Type: text/plain

Some text content
--=-BDZlXJLJY5lTfzoutnnYew==
Content-Type: text/html

<p>Some <strong>HTML</strong> content</p>
--=-BDZlXJLJY5lTfzoutnnYew==--
"
```

The above code, creates a multipart MIME entity, that contains two text parts, one being a "plain" text entity, and the other being an "HTML" text
entity. To understand the structure of the above MIME entity, is beyond the scope of this document, and if interested, feel free to check out the
documentation for the MIME standard(s).

Any node you add up as a child node of your entity, which is not defined as a MIME type (text, image, audio, etc), will be assumed is a MIME header.
For instance, if you wish to have your entire multipart above have the additional (custom) header of "X-App", and the first text entity have the
header of "X-Type", then you could do something like this.

```
p5.mime.create
  multipart:mixed
    X-App:P5
    text:plain
      X-Type:Foo bar data
      content:Some text content
    text:html
      content:<p>Some <strong>HTML</strong> content</p>
```

Which would result in the following result.

```
p5.mime.create
  result:@"Content-Type: multipart/mixed; boundary=""=-BoXqtm08oBaCS1a7RYZmBg==""
X-App: P5

--=-BoXqtm08oBaCS1a7RYZmBg==
Content-Type: text/plain
X-Type: Foo bar data

Some text content
--=-BoXqtm08oBaCS1a7RYZmBg==
Content-Type: text/html

<p>Some <strong>HTML</strong> content</p>
--=-BoXqtm08oBaCS1a7RYZmBg==--
"
```

By default, p5.mime supports 9 different MIME media types. These are the different Content-Type values from the MIME standard, and can each have their 
own media sub type, according to whatever sub type is legal for the specific type. The list of media types are as following.

* [multipart] - Used for MIME entities that have children MIME entities
* [text] - Text based MIME entity
* [image] - Images and such
* [application] - Possibly JavaScript and similar types
* [audio]
* [video]
* [message]
* [example]
* [model]

Check up the MIME standard, to understand exactly what the different types represents, and how to use them. However, for most uses, understanding
the multipart, text, and application types, is probably sufficient.

### Leaf MIME types dissected

Except for the multipart, all of the above MIME types are leaf mime types. This means that they cannot have children of their own, but are considered
to be "leaf" entities of a MIME message.

They all share the same set of arguments, which are MIME headers, and you can create any MIME header you wish. Though, realize, a valid MIME header 
is defined, as having a name, where it contains at least one UPPERCASE letter, where every word is separated by a hyphen (-), and each new
word in the header's name, starts with a capital letter. Example given; "My-Mime-Header". Most of the standard MIME headers, are intelligently
understood by the internals of the library, due to the brilliance of the underlaying MimeKit library, which is used internally by p5.mime.

Notice, instead of providing *[content]* to your leaf entities, you can instead provide a *[filename]*, which would result in that the content
of your leaf entity, would be created from the content of a file, and not its *[content]* node.

Below is an example of creating a leaf entity from a filename.

```
p5.mime.create
  application:X-Hyperlambda
    filename:/system42/application-startup.hl
```

Notice how the above code, would by default create a "Content-Disposition" MIME header for you, putting in the original filename into its
"filename" argument, and making sure the entity is marked as an "attachment". You can explicitly override this behavior, by creating your own
Content-Disposition MIME header. Below is what the above lambda would result in.

```
p5.mime.create
  result:@"Content-Type: application/X-Hyperlambda
Content-Disposition: attachment; filename=application-startup.hl


... file content ...
"
```

Some of the Active Events in P5, such as the *[p5.smtp.send]* event, from [p5.mail](/plugins/extras/p5.mail/), will actually directly
serialize the file to the socket, and never load the file into memory, if you use the above *[filename]* argument, instead of loading
the file into memory first. Which of course, means saving significant amounts of resources for your P5 server.

### Binary MIME entities

You can override the Content-Transfer-Encoding of your MIME messages, which is useful for binary data. Below is an example of creating
a MIME message, with one binary attachment, transferred as a base64 encoded MIME entity.

```
p5.mime.create
  image:png
    Content-Transfer-Encoding:base64
    filename:/system42/apps/CMS/media/p5.png
```

The above lambda would result in something like the following.

```
p5.mime.create
  result:@"Content-Type: image/png
Content-Transfer-Encoding: base64
Content-Disposition: attachment; filename=p5.png

iVBORw0KGgoAAAANSUhEUgAAAGQAAABkCAYAAABw4pVUAAAABmJLR0QA/wD/AP+gvaeTAAAA
CXBIWXMAAAsTAAALEwEAmpwYAAAAB3RJTUUH4QUZBzIy1NXmlgAAGflJREFUeNrtXWmYXGWV
[ ... snipped away ...]
rpAnmP5EO3XgV70+R8ozZ5F3Zc4WB21VNXz92xBI/4erfNndgryqgD4h3C9DJHsByDwrLSFm
rE6ChAnCHc4SsvP81Cheo1ggQwiqvybkbifvq4ZDcZmH3iUfug+7tuDim6si6/8BAk2DFgFq
oVgAAAAASUVORK5CYII=
"
```

You can also use "binary", and all other Content-Transfer-Encoding values supported by the MIME standard.

### The Multipart type dissected

A multipart MIME type is special, since it can contain multiple children MIME entities, in addition to a *[preamble]* and an *[epilogue]*. 
To create a basic multipart, with two children MIME entities, having a preamble and an epilogue, could be done like this.

```
p5.mime.create
  multipart:mixed
    preamble:This is the preamble
    text:plain
      content:Foo bar 1
    text:plain
      content:Foo bar 2
    epilogue:This is the epilogue
```

The above would result in the following.

```
p5.mime.create
  result:@"Content-Type: multipart/mixed; boundary=""=-iwCOwy10ijdRf3xIkKmXfg==""

This is the preamble
--=-iwCOwy10ijdRf3xIkKmXfg==
Content-Type: text/plain

Foo bar 1
--=-iwCOwy10ijdRf3xIkKmXfg==
Content-Type: text/plain

Foo bar 2
--=-iwCOwy10ijdRf3xIkKmXfg==--
This is the epilogue
"
```

### Encrypting and signing your MIME messages

If you wanted to encrypt your MIME message, this could easily be accomplished, by simply adding an *[encrypt]* child node to it, 
containing the list of recipients, such as the following illustrates.

```
p5.mime.create
  multipart:mixed
    encrypt
      email:thomas@gaiasoul.com
      // OR alternatively use the "fingerprint" of the certificate
      fingerprint:xxxx-some-PGP-certificate-fingerprint
    text:plain
      content:Foo bar 1
    text:plain
      content:Foo bar 2
```

Internally, the cryptography features of P5, uses GnuPG (Gnu Privacy Guard) to retrieve certificates and private keys used in cryptography and
signing of messages. This means you'll have to have GnuPG installed on your system to use cryptography in combination with MIME creation.

You would also (obviously) have to have the certificate used for encryption installed into your GnuPG database. The above lambda, will throw an
exception, simply because it won't find the public PGP key needed to encrypt your message (assuming you don't have a public key
for "thomas@gaiasoul.com").

Signing a multipart, with a private PGP key, is just as simple. Except, this time you would have to declare which _private key_ to use. Which requires
you to also supply a *[password]* node, beneath the *[email]/[fingerprint]* argument you supply, to have the GnuPG database release your private key.

```
p5.mime.create
  multipart:mixed

    // Notice, this time we only sign the multipart
    sign
      email:thomas@gaiasoul.com
        password:Your-GnuPG-password
    text:plain
      content:Foo bar 1
    text:plain
      content:Foo bar 2
```

You can of course, _both_ sign and encrypt a MIME entity, supplying both an *[encrypt]* argument, in addition to a *[sign]* argument.

Notice, _do not_ mark your encrypted/signed multiparts explicitly as "multipart:encrypted" or "multipart:signed", since the encryption/signing 
process automatically wraps your entire message into another wrapping "multipart:encrypted/signed" MIME entity. To see this in action, 
run the following code through your System42's Executor, and see how your single "text:plain" entity, 
is wrapped inside an outer "multipart/signed" entity.

```
p5.mime.create
  text:plain
    sign
      email:YOUR_GMAIL_EMAIL@gmail.com
        password:Your-GnuPG-password
    content:Foo bar 1
```

The above results in something resembling the following.

```
p5.mime.create
  result:@"Content-Type: multipart/signed; boundary=""=-+2ZNhGoLEPVT/UQIzXf0PA=="";
    protocol=""application/pgp-signature""; micalg=pgp-sha256

--=-+2ZNhGoLEPVT/UQIzXf0PA==
Content-Type: text/plain

Foo bar 1
--=-+2ZNhGoLEPVT/UQIzXf0PA==
Content-Type: application/pgp-signature; name=signature.pgp
Content-Disposition: attachment; filename=signature.pgp
Content-Transfer-Encoding: 7bit

-----BEGIN PGP MESSAGE-----
Version: BCPG C# v1.8.1.0

owJ4nAGeAGH/iJwEAQEIAAYFAllBBcsACgkQp+gOGJZNs4GcNQP+KxdJKpUXHb6v
smR86C+chDzrRqLcKI5O44xL44c3o74SBF3K1mUP2Cupf4UG12SmgzCh45lGxrzc
tF1NQZ+GLEQnPeqkeqFzm21IBWsEg/hcH+YTNDXPmhQ48CebHzv13uazw1f+B0rR
m8xshsVW8KnvhdpLBp0wmNIqg0d3COrorkbT
=7LsO
-----END PGP MESSAGE-----

--=-+2ZNhGoLEPVT/UQIzXf0PA==--
"
```

Notice how your single text entity is wrapped inside a "multipart/signed" entity above. This is necessary to make sure we also attach the actual
signature of your MIME message.

## Parsing MIME messages

Parsing MIME messages is, if possible, even more easy than creating them. Simply use the *[p5.mime.parse]* Active Event. In the example below,
we are first creating a MIME message, for then to parse it.

```
// Creating a MIME message
p5.mime.create
  multipart:mixed
    text:plain
      content:Foo bar 1
    text:plain
      content:Foo bar 2

// Parsing your MIME message
p5.mime.parse:x:/@p5.mime.create/*/result?value
```

When you parse a MIME message, you can supply a lot of optional arguments. The most important ones being.

* [attachment-folder] - A folder on disc where you want to store attachments
* [decrypt] - One or more decryption emails/fingerprints, with passwords, to decrypt encrypted emails

The *[attachment-folder]* argument, allows you to specify an explicit folder, where you want to store attachments. If you do, then
the attachments from your MIME message will not be loaded into memory, but rather saved directly to disc, and its path will be returned.
This will return a *[filename]* argument for you, in addition to a random *[prefix]* and the *[folder]*, created to make sure the file has 
a unique filename on disc. This *[filename]* argument, will in combination with its *[prefix]* child argument, and *[folder]* argument be, 
the path to where the file physically exists on disc. Of course the *[folder]* argument returned, will be the same value as 
your *[attachment-folder]* input argument, and is simply returned for convenience purposes.

The *[decrypt]* argument, is a collection of *[email]*/*[fingerprint]* values, each requiring a *[password]* child, for private PGP keys
to use while decrypting your emails. If you don't supply this, and you are trying to parse an encrypted MIME message - Then you will get the
raw encrypted text back, and not the plain text.

An example of using both of the above constructs can be found beneath.

```
// Creating an encrypted MIME message, with one attachment
p5.mime.create
  multipart:mixed
    encrypt
      email:YOUR_GMAIL_EMAIL@gmail.com
    text:plain
      content:Foo bar 1
    text:X-Hyperlambda
      filename:/application-startup.hl

// Parsing your MIME message, supplying an attachment folder, and decryption key
p5.mime.parse:x:/@p5.mime.create/*/result?value
  attachment-folder:~/temp/
  decrypt
    email:YOUR_GMAIL_EMAIL@gmail.com
      password:Your-GnuPG-password
```

The return value from the above *[p5.mime.parse]* invocation, will resemble the following.

```
p5.mime.parse
  multipart:encrypted
    decryption-key:YOUR_GMAIL_EMAIL@gmail.com
    multipart:mixed
      text:plain
        content:Foo bar 1
      text:X-Hyperlambda
        Content-Disposition:attachment; filename=application-startup.hl
        filename:application-startup.hl
          prefix:e8b4ea8eddb94c7ea9caffad6a4982b6-
          folder:/users/root/temp/
```

## Sending an encrypted and signed email

If you are using p5.mime in combination with [p5.mail](/plugins/extras/p5.mail/), you can use the following code to send an encrypted and signes
email. Please change the settings to your settings, and the private/public PGP keypair to something existing in your own GnuPG database.

```
p5.smtp.send
  server:smtp.gmail.com
  port:465
  ssl:true
  username:YOUR_GMAIL_USERNAME@gmail.com
  password:YOUR_GMAIL_PASSWORD
  envelope
    Subject:Hello from Phosphorus Five
    Sender
      Thomas Hansen:YOUR_GMAIL_USERNAME@gmail.com
    To
      Thomas Hansen:SOMEBODY@SOMEWHERE.COM
    body
      multipart:mixed
        encrypt
          email:SOMEBODY@SOMEWHERE.COM
        sign
          email:YOUR_GMAIL_USERNAME@gmail.com
            password:YOUR_GNUPG_PASSWORD
        text:plain
          content:Hello there, this is Phosphorus Five sending you an ENCRYPTED email.
```

## Fetching encrypted and signed MIME emails

If you are using p5.mime in combination with [p5.mail](/plugins/extras/p5.mail/), you can retrieve emails, decrypting them in the process, with the 
following code.

```
p5.pop3.get
  server:pop.gmail.com
  port:995
  ssl:true
  username:YOUR_GMAIL_USERNAME@gmail.com
  password:YOUR_GMAIL_PASSWORD
  decrypt
    email:YOUR_GMAIL_USERNAME@gmail.com
      password:YOUR_GNUPG_PASSWORD
```

Notice, we are using GMail in these examples, assuming you have a GMail account. You can of course use any POP3/SMTP servers you wish, including the
ones from your ISP, or servers you have set up for yourselves.

You can also supply an *[attachment-folder]* when fetching emails from your POP3
server, which will be passed into the MIME parser automatically for you, and never load up attachments into memory - Which will conserve a significant
amount of resources on your server, for big attachments fetched from your POP3 server.

## Additional PGP related Active Events

There are also many other PGP related Active Events in this component. Below is an exhaustive list.

* [p5.crypto.create-pgp-keypair] - Creates a PGP keypair, and installs into your GnuPG database
* [p5.crypto.list-private-keys] - List all private keys from GnuPG
* [p5.crypto.list-public-keys] - List all public keys from GnuPG
* [p5.crypto.get-key-details] - Returns the details for a PGP key
* [p5.crypto.get-public-key] - Returns one or more public PGP keys
* [p5.crypto.get-private-key] - Returns one or more private PGP keys
* [p5.crypto.delete-private-key] - Delets a private PGP key
* [p5.crypto.delete-public-key] - Delets a public PGP key
* [p5.crypto.import-public-pgp-key] - Imports a public PGP key
* [p5.crypto.import-private-pgp-key] - Imports a private PGP key
* [.p5.mime.parse-native] - C# special Active Event for hooking into library for C# developers
* [.p5.mime.create-native] - C# special Active Event for hooking into library for C# developers

The last two Active Events are only for extension developers, who wants to use MIME natively in their own components, and hence beyond the
scope of this documentation. To see example usage of them though, feel free to check out for instance the code of p5.mail project, that uses
them internally.

The first 10 Active Events in the above list, allows you to manage your GnuPG database, by creating new keys, and modifying existing keys.
Below is the documentation for them.

### [p5.crypto.create-pgp-keypair]

This event creates a private/public keypair for you, and installs it into your GnuPG database. To use it, you could use something like the
following.

```
p5.crypto.create-pgp-keypair

  // Mandatory arguments
  identity:foo@bar.com
  password:Your-GnuPG-Password
  
  // Optional arguments
  expires:date:"2020-06-14T10:51:59.018"
  strength:1024
  public-exponent:long:65537
  certainty:5
  seed:xyzqwerty124233456&$%
```

The above will create a PGP keypair for you, with the identity of "foo@bar.com", and the password of "Your-GnuPG-Password", and install it into
your GnuPG database. The rest of the arguments are optional, with "sane defaults". One detail though, is that you can explicitly add to the seeding
of the randomb number generator by adding a *[seed]* argument - Which is probably wise, to make sure your random numbers used to generate the keypair
is less predictable. The last point of course, makes your keypair much more difficult to predict. Preferably this should be some user supplied
string, of a certain length, for every time you create a keypair. Notice that the seed is by default relatively unique, with a very large entropy,
but further adding to it, makes it even less predictable.

### [p5.crypto.list-private-keys] and [p5.crypto.list-public-keys]

These two Active Events simply lists your private and public PGP keys from your GnuPG database. Below is an example of usage.

```
p5.crypto.list-private-keys
p5.crypto.list-public-keys
```

The above will result in something like the following.

```
p5.crypto.list-private-keys
  3a043739292f87e371116352a7e80e18964db381
  883c57f331661a58b2af5aa8a9ea8f251c05c160
  7fa6f6f56a37840bb4c5a820ee1646290fa70f72
  aabea2c7ecf44174f918e569968463c52ad8d65f
p5.crypto.list-public-keys
  3a043739292f87e371116352a7e80e18964db381
  883c57f331661a58b2af5aa8a9ea8f251c05c160
  7fa6f6f56a37840bb4c5a820ee1646290fa70f72
  aabea2c7ecf44174f918e569968463c52ad8d65f
```

Notice that what you get back from these Active Events, are the fingerprints of your keys. Both of these Active Events takes an optional filter,
for fingerprints you wish to look for. E.g., assuming you have a private key with the specified fingerprint, you could use something like the following.

```
_fingerprints
  3a043739292f87e371116352a7e80e18964db381
  883c57f331661a58b2af5aa8a9ea8f251c05c160
p5.crypto.list-public-keys:x:/-/*?name
```

You can also supply one or more identities to look for as a filter condition. Example is given below.

```
// Looking for a specific ID
p5.crypto.list-private-keys:phosphorusfive@gmail.com

// Or listing all gmail.com keys
p5.crypto.list-private-keys:@gmail.com
```

### [p5.crypto.get-key-details]

This Active Event will return the details of one or more keys. Below is an example.

```
p5.crypto.get-key-details:phosphorusfive@gmail.com
```

The above code will return something like the following.

```
p5.crypto.get-key-details
  3a043739292f87e371116352a7e80e18964db381
    id:964DB381
    algorithm:RsaGeneral
    strength:int:1024
    creation-time:date:"2017-06-01T17:22:32"
    is-encryption-key:bool:true
    is-master-key:bool:true
    is-revoked:bool:false
    version:int:4
    expires:date:"2020-06-01T17:22:31"
    user-ids
      :Thomas Hansen <phosphorusfive@gmail.com>
    signed-by
      964DB381:date:"2017-06-01T17:22:32"
```

This event can also return multiple results. For instance, to return all details from all GMail keys, can be done with something resembling
the following.

```
p5.crypto.get-key-details:@gmail.com
```

### [p5.crypto.get-public-key] and [p5.crypto.get-private-key]

These two Active Events will return the actual key content. Usage can be found below.

```
p5.crypto.get-public-key:phosphorusfive@gmail.com
p5.crypto.get-private-key:phosphorusfive@gmail.com
```

Which will result in something like the following.

```
p5.crypto.get-public-key
  3a043739292f87e371116352a7e80e18964db381:@"-----BEGIN PGP PUBLIC KEY BLOCK-----
Version: BCPG C# v1.8.1.0

mI0EWTBNWAEEAMhVnsHog1ZcFUD17SyEV5jt+OR7oRgLpozvl4cLtni9cMHx338d
ybY/u765u0weYx8gRWguRmJq0d4aDNq1z31iICAbooTKs1wWUPCD6iSAwWfnI1Ie
2Gk2S8PMqaKGKvpzaeIEajn6gvfAs5/Ud3iWsrP/gLVAlSrR7qKQERTvABEBAAG0
KFRob21hcyBIYW5zZW4gPHBob3NwaG9ydXNmaXZlQGdtYWlsLmNvbT6IsQQQAQIA
GwUCWTBNWAIbAwQLCQgHBhUIAgkKCwUJBaTr/wAKCRCn6A4Ylk2zgXIZBAChmCpX
P8mXcsvXpr9E0MfvlshY9Z15a54tEhrMpe6eICOemh61W0vFfCPoJvRO52/0gSot
RwGjtetMDN5PyTZhpkUEiAP+fKTPLlGJyTWmAcrIXaGYuPE0lmo/mc3DAlU2FGcl
AsLtZrGjPRdoBoyhZVH/+vkYANj9gnwnvcHMMQ==
=oHRj
-----END PGP PUBLIC KEY BLOCK-----
"
p5.crypto.get-private-key
  3a043739292f87e371116352a7e80e18964db381:@"-----BEGIN PGP PRIVATE KEY BLOCK-----
Version: BCPG C# v1.8.1.0

lQIGBFkwTVgBBADIVZ7B6INWXBVA9e0shFeY7fjke6EYC6aM75eHC7Z4vXDB8d9/
Hcm2P7u+ubtMHmMfIEVoLkZiatHeGgzatc99YiAgG6KEyrNcFlDwg+okgMFn5yNS
HthpNkvDzKmihir6c2niBGo5+oL3wLOf1Hd4lrKz/4C1QJUq0e6ikBEU7wARAQAB
/gkDApNbDFq6Sf3aYMCehK38l9siFlzy8VzBBgjVm5/fqRHYTj4YVOamzaN67mZk
fgHMHT2zZsqPrlNxVk0lXdK6hGtkB7tpr+zNV3jfGTctNUTClApzflJTQ0EIDAEX
IqHgR//jLs/mBnTvIF7BDDBjN6wljrrdriU+O96MEGwRI/Dkxq6ovk2U3Kr9D/hK
Xh3pGEcx2nQKGCquyqasGq0Eb4PpY8Qt3HtkSNh/u1o3C8HKqGLKj6wYsiIvOFxS
qjmDhM3uKXVVufuVLtIVC7T3HTvwDtDH7snwgdo01C05mQKPpU7W0hPWoyRNOgmC
O6rA7v8sE6poVLVl9sijVcS55HKDClYRoZ6+V5TapXuFJrK9A23dO6jaGAeXFCyf
VgA1lvqNjHSOhC4a+uuh+R0sIcKnv5L+cR6O6dsAhoWZRWsTfOlTl1fJ55f7ZMK7
8XMmdCAt5wWWxEW2aEu9BxEP13HAXy6ksvZ6miWvqtFmdrOWADMMzj+0KFRob21h
cyBIYW5zZW4gPHBob3NwaG9ydXNmaXZlQGdtYWlsLmNvbT6IsQQQAQIAGwUCWTBN
WAIbAwQLCQgHBhUIAgkKCwUJBaTr/wAKCRCn6A4Ylk2zgXIZBAChmCpXP8mXcsvX
pr9E0MfvlshY9Z15a54tEhrMpe6eICOemh61W0vFfCPoJvRO52/0gSotRwGjtetM
DN5PyTZhpkUEiAP+fKTPLlGJyTWmAcrIXaGYuPE0lmo/mc3DAlU2FGclAsLtZrGj
PRdoBoyhZVH/+vkYANj9gnwnvcHMMQ==
=vGHI
-----END PGP PRIVATE KEY BLOCK-----
"
```

Notice, my private key above, is (obviously) a bogus testing key, and not my real actual PGP key.

This allows you to export your private and public keys, to create backups or something out of them.
Realize though that private keys, will be exported in "armored format", which means you will still need the 
password to the key, to retrieve its actual content.

### [p5.crypto.delete-private-key] and [p5.crypto.delete-public-key]

These Active Events simply deletes the specified public or private key(s).
For these Active events you must supply the fingerprint, and none of them will tolerate an identity, such as an email address, etc.
This is to make sure you actually delete the right key, and not accidentally delete a private key, which is still in use.

### [p5.crypto.import-public-pgp-key] and [p5.crypto.import-private-pgp-key]

These Active Events allows you to import a private and public key. Below we are importing a public and private key, with the
identity of "foo@bar.com".

```
p5.crypto.import-public-pgp-key:@"-----BEGIN PGP PUBLIC KEY BLOCK-----
Version: BCPG C# v1.8.1.0

mI0EWUEVnAEEAMQWCN96diwls+9SR/XMEFWppRo8ktzOTKQZV/qYjakH5yZxMwXF
sS2I8Zxw4DfLy3TDqPY2ogHICJXnCAfPQFS8kbDoRPe2KUkGvhfg56qlErCqOXEk
0fCngUDTnhxL3eKOILSDbKIUDFGFpebkKacaOGmYtnkLNwz2VRtr/1lNABEBAAG0
C2Zvb0BiYXIuY29tiLEEEAECABsFAllBFZ0CGwMECwkIBwYVCAIJCgsFCQWkwYIA
CgkQ7hZGKQ+nD3KPLwQAg2S1LlA3V6XUkzDVwQ+eUA5lMTFLq5Avbx11ucAbd+/c
osk1QmZR9/B3x8LCLfoA7MKPf71PNirFC64htjAqErWjFy5JANAvntPSbU7AeniJ
CecA7GNaupQwLVLkidKxhLF0IT9gNkK//8ns8TWRe6j8GmkDTxe2VsFHq4eI9LI=
=Bzbm
-----END PGP PUBLIC KEY BLOCK-----"
```

Check out your GnuPG database afterwards, by doing for instance something like
the following.

```
p5.crypto.get-key-details:foo@bar.com
```


