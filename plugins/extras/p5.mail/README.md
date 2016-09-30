Email - SMTP and PO3 support in Phosphorus Five
===============

This project contains the Active Events necessary to send and retrieve emails. It is dependent upon the project 
called [p5.mime](/plugins/extras/p5.mime/), to build and parse your MIME messages.

It contains two Active Events.

* [p5.mail.smtp.send-email]
* [p5.mail.pop3.get-emails]

There is no IMAP support in P5 at the moment.

## Sending an email using SMTP

To send an email, you have to use the *[p5.mail.smtp.send-email]* Active Event. This event allowss you to send rich MIME messages, with attachments,
and multiple entities. Below is some example code, assuming you've got a GMail account. Change the *[username]*, *[password]*, *[Sender]*
and *[To]* parts, if you wish to test the code for yourself. Hint, with GMail's SMTP servers, you can send yourself an email. Just make sure you've
got your own email address as both the *[Sender]* and the *[To]* argument.

```
p5.mail.smtp.send-email
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
your web.config, is actually more secure, since in theory, an exception might occur during transmitting the email, which might result in the Hyperlisp
stacktrace become accessible to the end user, having him see the password for your SMTP account. The arguments that are optional, and if not supplied
are fetched from your web.config, are listed below.

* [server]
* [port]
* [ssl]
* [username]
* [password]

By not including these arguments directly, it is also easier to change your server settings later, without having to modify multiple invocations 
to [p5.mail.smtp.send-email], all around your code. The rest of these examples, will assume you've setup your web.config such that it contains
the above mentioned settings.

### Sending multiple emails in one go

With *[p5.mail.smtp.send-email]*, you can send multiple emails at the same time. Every *[envelope]* argument you supply, becomes one single email.
Below is some piece of code, sending two email in on go.

```
p5.mail.smtp.send-email
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
and your *[p5.mail.smtp.send-email]* as your "post office mailbox".

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
are "custom" arguments.

### The [body] argument

How to create your *[body]*, is actually beyond the scope of this documentation, since it is actually being handled 
by [p5.mime](/plugins/extras/p5.mime/). But basically, you can send any MIME messages you wish, including PGP encrypted messages, and cryptographically
signed messages, as you see fit. You can also add any amount and types of attachments you wish.

## Retrieving emails with POP3






