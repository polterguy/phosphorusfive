Email - SMTP and PO3 support in Phosphorus Five
===============

This project contains the Active Events necessary to send and retrieve emails. It is dependent upon the project 
called [p5.mime](/plugins/extras/p5.mime/), to build and parse your MIME messages. Or rather, to be more correct, it
depends upon the Active Events existing in p5.mime, since there are no dependencies in Active Events plugin in P5.

It contains two Active Events.

* [p5.smtp.send]
* [p5.pop3.get]

There is no IMAP support in P5 at the moment.

## Sending an email using SMTP

To send an email, you have to use the *[p5.smtp.send]* Active Event. This event allows you to send rich MIME messages, with attachments
and multiple entities. Below is some example code, assuming you've got a GMail account. Change the *[username]*, *[password]*, *[Sender]*
and *[To]* parts if you wish to test the code for yourself. Hint, with GMail's SMTP servers, you can send yourself an email. Just make sure you've
got your own email address as both the *[Sender]* and the *[To]* argument.

```
p5.smtp.send
  server:smtp.gmail.com
  port:465
  ssl:true
  username:your-gmail-email@gmail.com
  password:Your-Gmail-Password
  envelope
    Subject:Hello from Phosphorus Five
    Sender
      Thomas Hansen:your-gmail-email@gmail.com
    To
      Thomas Hansen:some-recipient@somewhere.com
    body
      text:plain
        content:Hello there, this is Phosphorus Five sending you an email.
```

The connection arguments to the server are optional, and if not supplied, these will be fetched from your web.config. In fact, to supply them through
your web.config, is actually more secure, since in theory, an exception might occur during sending the email, which might result in the Hyperlambda
stacktrace become accessible to the end user, having him see the password for your SMTP account. The arguments that are optional, and if not supplied
are fetched from your web.config, are listed below.

* [server]
* [port]
* [ssl]
* [username]
* [password]

By not including these arguments directly, it is also easier to change your server settings later, without having to modify multiple invocations 
to [p5.smtp.send], all around your code. The rest of these examples, will assume you've setup your web.config such that it contains
the above mentioned settings.

### Sending multiple emails in one go

With *[p5.smtp.send]*, you can send multiple emails at the same time. Every *[envelope]* argument you supply, becomes one email.
Below is some piece of code, sending two email in on go. This code relies upon your web.config settings, and does not supply credentials or server settings
directly in the code.

```
p5.smtp.send
  envelope
    Subject:Hello from Phosphorus Five
    Sender
      Thomas Hansen:your-gmail-email@gmail.com
    To
      Thomas Hansen:some-recipient@somewhere.com
    body
      text:plain
        content:Hello there, this is Phosphorus Five sending you an email.
  envelope
    Subject:Another hello!
    Sender
      Thomas Hansen:your-gmail-email@gmail.com
    To
      Thomas Hansen:some-recipient@somewhere.com
    body
      text:plain
        content:Hello there, this is ALSO Phosphorus Five sending you an email.
```

As before, make sure you modify the *[To]* and *[Sender]* arguments, before you evaluated the above code.

Each email you send, can contain its own separate attachments, content, recipients, sender arguments, and so on. Think of them as separated "letters",
and your *[p5.smtp.send]* as your "post office mailbox".

### SMTP header arguments

The *[envelope]* header arguments are as following, and correspond with the asssociating SMTP headers. To further understand these headers, feel
free to check out the SMTP standard.

* [Subject] - Simple text string, becoming the subject or title of your email
* [From] - Collection of addresses, where each child node's name is the friendly name (e.g. John Doe) and the value the email address (e.g. john@doe.com)
* [Resent-From] - Collection of addresses, as above
* [To] - Collection of addresses
* [Resent-To] - Collection of addresses
* [Cc] - Collection of carbon copy addresses
* [Resent-Cc] - Collection of addresses
* [Bcc] - Collection of blind copy addresses
* [Resent-Bcc] - Collection of addresses
* [Reply-To] - Collection of addresses
* [Resent-Reply-To] - Collection of addresses
* [Sender] - Single address expected to be found in child node (name/email combination)
* [In-Reply-To] - Value expected to be found in value of node
* [Resent-Message-ID] - Same as above
* [Resent-Sender] - Single address expected to be found in child node (name/email combination)
* [Importance] - One of the following values; "Low", "Normal", "High"
* [Priority] - One of the following values; "NonUrgent", "Normal", "Urgent"

In addition, you can create your own custom SMTP headers, by appending them as "name/value" pairs, in a *[headers]* argument. Notice, it is normally
considered "best practice" to start your custom SMTP header names with the string "X-", to signify to any recipient's email servers, that these 
are custom and non-standard arguments.

### The [body] argument

How to create your *[body]*, is actually beyond the scope of this documentation, since it is actually being handled 
by [p5.mime](/plugins/extras/p5.mime/). But basically, you can send any MIME messages you wish, including PGP encrypted messages, and cryptographically
signed messages as you see fit. You can also add any amount and types of attachments you wish.

Notice, there must be exactly _one_ *[body]* for each *[envelope]*, and each *[body]* must have exactly one MIME entity, which of course can be
a multipart, containing multiple child MIME entities.

[See p5.mime for how to create a body](/plugins/extras/p5.mime/)

## Retrieving emails with POP3

The *[p5.pop3.get]* Active Event, allows you to retrieve emails from a POP3 server. It has the same types of "connection arguments" as 
the *[p5.smtp.send]* Active Event, which also can be stored in your web.config file, for simplicity.

Assuming you've setup GMail to be able to handle incoming POP3 requests, you can use the following code to retrieve 5 messages from GMail.

```
p5.pop3.get
  server:pop.gmail.com
  port:995
  ssl:true
  username:your-gmail-email@gmail.com
  password:your-gmail-password
  count:5
```

The above code will (normally) retrieve your 5 oldest emails. It will automatically parse any MIME entities, and return the entities as a 
tree-structure. It will not delete the message on your POP3 server. To have your messages deleted, as you retrieve them, make sure you pass in
a *[delete]* argument, and set its value to true. Notice, if an exception occurs during fetching your emails, no emails will be deleted on 
your POP3 server. This is true because the QUIT signal will not be sent to your POP3 server unless it is gracefully finishing, without exceptions.

The message ID, as given by your POP3 server, will be returned as the value of each *[envelope]* returned from this Active Event. The message ID,
can be used to uniquely identify the message later.

The standard headers you can possibly expect to have each of your *[envelope]*s return, are listed below. To understand their meaning, feel
free to check up the POP3 standard documents.

* [Subject] - The subject of the email
* [From] - Collection of emails
* [Resent-From] - Collection of addresses
* [Bcc] - Collection of addresses
* [Cc] - Collection of addresses
* [Resent-CC] - Collection of addresses
* [Reply-To] - Collection of addresses
* [Resent-Reply-To] - Collection of addresses
* [To] - Collection of addresses
* [Resent-To] - Collection of addresses
* [Date]
* [Resent-Message-ID]
* [Sender] - A single name/email child node
* [Resent-Sender] - A single name/email child node
* [MIME-Version]
* [Resent-Date]
* [Importance]
* [In-Reply-To]
* [Priority]
* [References] - A list of referenced emails

In addition, any custom" POP3 headers, will be returned in an *[X-Headers]* section, beneath each *[envelope]*, as a name/value pair.

### Supplying your own callback handler for messages

If you wish, you can supply a piece of lambda callback as *[.onfinished]*, which is invoked after all emails requested is fetched. This is useful for
providing your own piece of lambda callback, which you can use to store your emails into e.g. a database or something. Your *[.onfinished]* lambda
callback is invoked before the QUIT signal is sent to your POP3 server, which is useful, since if an exception occurs during your lambda callback,
no emails will be deleted on the POP3 server, if it is correctly configured.

An example is given below.

```
p5.pop3.get
  server:pop.gmail.com
  port:995
  ssl:true
  username:your-gmail-email@gmail.com
  password:your-gmail-password
  count:3
  .onfinished
    sys42.windows.show-lambda:x:/..
```

### How parsing of your POP3 MIME messages happens

The *[p5.pop3.get]* Active Event, relies upon [p5.mime](/plugins/extras/p5.mime/), just like its counterpart SMTP Active Event.
Among other things, you can supply an *[attachment-folder]* argument, which will be transferred into the MIME parser Active Events, 
which declares a folder on disc, where you want to automatically save attachments. This will significantly reduce memory consumption, while
parsing your MIME/email messages, since no attachments will be loaded into memory, but directly saved to disc.

You can also supply a *[decrypt]* node as an argument, which also will be transferred into each MIME/email parsing invocation to
p5.mime. If you do, then any encrypted messages, will be attempted decrypted, using the supplied GnuPG keys, declared through your *[encrypt]*
argument.

[Check out p5.mime for details about MIME parsing](/plugins/extras/p5.mime) and cryptography.
