
<%@ Page 
    Language="C#" 
    Inherits="p5.samples.JavaScript"
    Codebehind="JavaScript.aspx.cs" %>

<!DOCTYPE html>
<html>
    <head>
    	<title>JavaScript example</title>
        <link rel="stylesheet" type="text/css" href="media/main.css">
        <meta charset="utf-8">
    </head>
    <body>
        <form id="form1" runat="server" autocomplete="off">
            <div class="container">

                <h1>JavaScript example</h1>

                <p>
                    This example shows how you can intercept p5.ajax HTTP requests using javascript

                <p>
                    <p5:Literal
                        runat="server"
                        id="javascript_widget"
                        Element="pre"
                        style="background-color:#eee;padding:15px;"
                        onclick="p5_samples.javascript_widget_onclick (event)">Click me</p5:Literal>

                <p>
                    Above we create our own JavaScript <em>"onclick"</em> event handler, and create our own Ajax HTTP request, 
                    using the JavaScript API of p5.ajax. In this request, we supply an <em>"onbefore"</em> serialization handler,
                    which is invoked before the HTTP request is submitted to the server. When the response returns, we yet again 
                    add up our own custom string, before we manually change the innerHTML value of our widget to the combined value 
                    from both the server, and the callback at the client. Since the server returns the string created in our <em>"onbefore"</em>
                    callback, the combined results becomes that of both of our two JavaScript callback strgins, in addition to the server generated
                    string inbetween these two strings.

                <p>
                    If you intercept the HTTP traffic of your requests, you will see that the server returns pure JSON back, which
                    also contains a <em>"custom_return_data"</em> value, which was dynamically added to the response on the server-side
                    C# code using the <em>"Manager.SendObject"</em> method.

                <p>
                    Onwards to the <a href="DynamicControls.aspx">dynamic example</a>

            </div>
        </form>
        <script type="text/javascript">

(function () {

  // Namespace for our custom JavaScript
  p5_samples = {};

  // Invoked when javascript_widget is clicked
  p5_samples.javascript_widget_onclick = function (event) {

    // Finding element for widget and raising our 'onclicked' event
    var el = p5.$ (event.target);

    // Notice, that this is a server-side Ajax Web Method, which we can raise indirectly using the JavaScript API of p5.ajax
    el.raise ('javascript_widget_onclicked', {

      // Called just before request is sent.
      // Here we just add up a "custom HTTP post parameter" called "custom_data", which we extract in C# on the server side.
      onbefore: function (pars, evt) {

        // Here we push our own "custom_data" HTTP parameter into the request that is created.
        pars.push(['custom_data', 'And thus your browser spoke, before the HTTP request.\r\n']);
      },

      // Called when a successful response is returned, but before DOM is updated, with the return value from server
      onsuccess: function (retVal, evt) {

        // Notice that the "el" variable above, will return the p5.element wrapping our Literal widget above, which
        // just so happens to have an "el" property, returning the "raw DOM element", hence the "funny double indirection retriever".
        // Also notice that the "retVal.custom_return_data" was dynamically added to the response from our server-side C# code, which
        // can see in the codebehind for our page.
        // If you inspect the "retVal" parameter, you will see that it is a pure JSON object, containing all changes from the server,
        // including also our "custom_return_data" dynamically added to the response from the server using "Manager.SendObject".
        el.el.innerHTML = retVal.custom_return_data + 'And thus your browser spoke after the HTTP request.';
      }
    });
  };
})();
        </script>
    </body>
</html>
