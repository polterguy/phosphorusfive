Ajax C# examples
========

This folder contains the samples for p5.ajax, in addition to some basic unit tests, for
testing the integrity of p5.ajax.

Contains some examples of how to use p5.ajax as a stand alone library, without anything else,
but the Ajax library itself, directly from C#, as an ASP.NET Web Forms application.

If you want to test this project, make sure you set the "p5.ajax-samples" as your startup project in Visual Studio,
Xamarin or MonoDevelop, and start your debugger. This project can be found in the "samples" folder in your IDE.

There are multiple pages, explaining some aspect of p5.ajax each. How to modify a Container widget's children collection
for instance, changing attributes of widgets during Ajax callbacks, using the JavaScript API directly to invoke Ajax "web methods"
through JavaScript, etc.

The code should be fairly self-explaining, and well documented. Below is a list of some of the things this example demonstrates.

* Persistently changing a Container widget's children collection in callbacks
* Modifying attributes of widgets in callbacks
* Dynamically injecting widgets (controls) into your page during callbacks
* Usage examples of all widgets; Void, Literal and Container
* Intercepting Ajax HTTP requests with your own custom JavaScript
* Invoking Ajax web methods from the client to the server
* Declare p5.ajax Ajax widgets into your .aspx markup
* etc ...




