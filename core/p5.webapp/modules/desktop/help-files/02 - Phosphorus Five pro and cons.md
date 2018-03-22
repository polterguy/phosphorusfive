## Pros and cons with Phosphorus Five

<img style="margin-left:1rem; float:right;max-width: 20%;" src="modules/hyper-ide/media/logo.svg" />

The main idea with Phosphorus Five is to significantly simplify the development of web apps, that responsively
renders on all devices. This implies that some _"shortcuts"_ has to be applied. Among other things, Phosphorus Five
in general is a server-side stateful web application framework. This means that your widgets are stored on the server
side. This again creates a beautiful development model, where you don't have to manually track which widgets
have been created, and re-create them during consecutive callbacks/postbacks.

This results in a _"ridiculously easy to understand"_ development model, arguably resembling the
development model of Visual Basic in simplicity, or FoxPro for that matter. However, the flipside is
that it requires much more server resources, particularly memory than a completely stateless development model.
This results in that it is extremely easy to create highly advanced web applications, while it is also difficult
(but _not_ impossible) to create apps that scales very well.

In general, for the above reasons, you should not expect Phosphorus Five to scale _"into heaven"_, allowing
for millions of consecutive users simultaneously accessing your server - Unless you bypass parts of the core,
and for instance use Hyper Core, instead of the Ajax Widgets.

The above results in that Phosphorus Five is an extremely easy development model for enterprise applications,
and apps that require no more than some few hundred, maybe some few thousands simultaneous users - While not
necessarily perfect for social media websites, or websites that require scaling into hundreds of thousands of
consecutive server requests, etc.

Phosphorus Five was created to allow for easily creating rich and interactive web apps, not social media hubs, stealing
the hearts and minds of half the world. If you want to create _"the next Facebook"_, I wish you good luck, as I
point you as far away from Phosphorus Five as I can possibly do ... ;)
