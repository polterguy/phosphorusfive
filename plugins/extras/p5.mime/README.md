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

By default, p5.mime supports 9 different MIME media types. These are Content-Types from the MIME standard, and can each have their 
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
Content-Disposition MIME header.

Some of the Active Events in P5, such as the *[p5.smtp.send]* event, from [p5.mail](/plugins/extras/p5.mail/), will actually directly
serialize the file to the socket, and never load the (whole) file into memory, if you use the above *[filename]* argument, instead of loading
the file into memory first. Which of course, means saving significant amounts of resources for your P5 server.

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

### Binary MIME entities

You can override the Content-Transfer-Encoding of your MIME messages, which is useful for binary data. Below is an example of creating
a MIME message, with one binary attachment, transferred as a base64 encoded MIME entity.

```
p5.mime.create
  multipart:mixed
    text:plain
      content:Foo bar 1
    image:png
      Content-Transfer-Encoding:base64
      filename:/system42/apps/CMS/media/p5.png
```

You can also use "binary", and all other Content-Transfer-Encoding values supported by the MIME standard.

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

Notice, _do not_ mark your multipart as "multipart:encrypted" or "multipart:signed", since the encryption/signing process automatically wraps 
your entire message into another wrapping "multipart:encrypted/signed" MIME entity. To see this in action, run the following code through your
System42's Executor, and see how your single "text:plain" entity, is wrapped inside an outer "multipart/signed" entity.

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
* [decrypt] - One or more decryption emails, with passwords, to decrypt any encrypted emails.

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

