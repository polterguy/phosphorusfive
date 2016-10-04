Parsing and creating MIME messages in Phosphorus Five
===============

MIME stands for "Multi-purpose Internet Message Exchange", and is usually associated with email. However, in Phosphorus Five, there exists
many other use cases for MIME. Among other things, you can use them as the basis for web service invocations, from one server to another.

This allows you to use the PGP cryptography features of MIME, when invoking web services, or using MIME for any other purpose. If you
define a "marvinPgpKey" and password in your app's config file for instance, then the "auth" file (file containing usernames and hashed passswords, etc),
will be encrypted as a MIME message, ussing PGP for encryption.

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

You can create multiple MIME entities at once, by simply adding another child node to *[p5.mime.create]*. However, this will not result in a "multipart",
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

The above code, creates a "multipart" MIME entity, that contains two "text" parts, one being a "plain" text entity, and the other being an "HTML" text
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

By default, p5.mime supports 9 different MIME "media types". These are Content-Types from the MIME standard, and can each have their 
own "media sub type", according to whatever sub type is legal, for the specific type. The list of "media types" are as following.

* [multipart] - Used for MIME entities that have children MIME entities
* [text] - Text based MIME entity
* [image] - Images and such
* [application] - Possibly JavaScript and similar types
* [audio]
* [video]
* [message]
* [example]
* [model]

Check up the MIME standard, to understand exactly what the different types represents, and how to use them. However, for most usage, understanding
the "multipart", "test" and "application" types, is probably enough.

### "Leaf" MIME types dissected

Except for the "multipart", all of the above MIME types are "leaf mime types". This means that they cannot have children of their own, but are considered
to be the "leaf" entities of a MIME message.

They all share the same set of arguments, which are MIME headers, and you can create any MIME header you wish. Though, realize, a valid MIME header 
is defined, as having a name, where it contains at least one UPPERCASE letter, where every "word" is separated by a hyphen (-), and each new
word in the header's name, starts with a capital letter. Example given; "My-Mime-Header". Most of the standard MIME headers, are intelligently
understood by the internals of the library, due to the brilliance of the underlaying MimeKit library, which is used internally by p5.mime.

Notice, instead of providing *[content]* to your leaf entities, you can instead provide a *[filename]*, which would result in that the content
of your leaf entity, would be created from thee content of a file, and not its *[content]* node.

Below is an example of creating a leaf entity from a filename.

```
p5.mime.create
  application:Hyperlambda
    filename:/system42/application-startup.hl
```

Notice how the above code, would by default create a "Content-Disposition" MIME header for you, putting in the original filename into its
"filename" argument, and making sure the entity is marked as an "attachment".

Some of the Active Events in P5, such as the *[p5.smtp.send-email]* event, from [p5.mail](/plugins/extras/p5.mail/), will actually directly
serialize the file to the socket, and never load the (whole) file into memory, if you use the above *[filename]* argument, instead of loading
the file into memory first. Which of course, means saving significant amounts of resources for your P5 server.

### The "Multipart" type dissected

A "multipart" MIME type is special. First of all because it can contain multiple children MIME entities. Secondly because it can be cryptographically
signed, and encrypted. In addition, a multipart can optionally contain a *[preamble]* and an *[epilogue]*. To create a basic multipart, with two 
children MIME entities, having a preamble and an epilogue, could be done like this.

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

If you wanted to encrypt your multipart MIME message, this could easily be accomplished, by simply adding an *[encryption]* child node to it, 
containing the list of recipients, such as the following illustrates.

```
p5.mime.create
  multipart:mixed
    encryption
      email:phosphorusfive@gmail.com
      // OR alternatively use the "fingerprint" of the certificate
      fingerprint:xxxx-some-PGP-certificate-fingerprint
    text:plain
      content:Foo bar 1
    text:plain
      content:Foo bar 2
```

Internally, the cryptography features of P5, uses GnuPG (Gnu Privacy Guard) to retrieve certificates and private keys, used in cryptography and
signing of messages. This means you'll have to have GnuPG installed on your system, where you wish to use PGP cryptography, combined with p5.mime.

You would also (obviously) have to have the certificate used for encryption installed into your GnuPG database. The above p5.lambda, will throw an
exception, simply because it won't find the certificate needed to encrypt your message (assuming you haven't created a keypair 
for "phosphorusfive@gmail.com")

Signing a multipart, with a private PGP key, is just as simple. Except, this time you would have to declare which _private key_ to use. Which requires
you to also supply a *[password]* node, beneath the *[email]/[fingerprint]* argument you supply, to have the GnuPG database release your private key,
for using it to sign your entity.

```
p5.mime.create
  multipart:mixed

    // Notice, this time we only sign the multipart
    signature
      email:phosphorusfive@gmail.com
        password:Your-GnuPG-password
    text:plain
      content:Foo bar 1
    text:plain
      content:Foo bar 2
```

You can of course, _both_ sign and encrypt a multipart, supplying both an *[encryption]* argument, in addition to a *[signature]* argument.

Only "multipart" MIME entities can be encrypted and/or signed though. If you wish to encrypt a "leaf" entity, you'll have to first put it into
a "multipart" somehow.


