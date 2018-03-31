## Pros and cons with Phosphorus Five

<img class="desktop-help-icon-image" src="/modules/desktop/media/logo.svg" />

The main idea with Phosphorus Five is to significantly simplify the development of web apps, that responsively
renders on all devices. This implies that some _"shortcuts"_ had to be applied. Among other things, Phosphorus
Five is in general a stateful server-side web application framework. This implies that your widgets are stored
on the server, which creates a beautiful development model, where you don't have to manually track which
widgets have been created, or re-create them during consecutive callbacks/postbacks.

This results in a _"ridiculously easy to understand"_ development model, arguably resembling the
development model of Visual Basic or FoxPro in simplicity. However, the flipside is
that it requires much more server resources, particularly memory, compared to a completely stateless development model.
This results in that it is extremely easy to create highly advanced web applications, while it is also difficult
(but _not_ impossible) to create apps that scales very well. Hyperlambda does also have some overhead
compared to a language that's closer to the hardware, such as C# or C++ - So it is not intended for creating
CPU resource intensive algorithms in. If you need more power, you can create Active Events in C#, for then
to consume these in Hyperlambda.

In general, for the above reasons, you should not expect Phosphorus Five to scale _"into heaven"_, allowing
for millions of consecutive users simultaneously accessing your server - Unless you bypass parts of the core,
and for instance use Hyper Core, instead of its Ajax Widgets.

The above results in that Phosphorus Five is an extremely easy development model for enterprise applications,
and apps that require no more than some few hundred, maybe some few thousands simultaneous users - While not
necessarily perfect for social media websites, or websites that require scaling into hundreds of thousands of
consecutive server requests, etc. Phosphorus Five was created to allow for easily creating rich, secure, and
interactive web apps - _Not_ social media hubs, stealing the hearts and minds of half the world.

> Hardware is cheap, developers not!
