<%@ Page 
    Language="C#" 
    Inherits="phosphorus.ajax.samples.JavaScript"
    Codebehind="JavaScript.aspx.cs" %>

<!DOCTYPE html>
<html>
    <head>
    	<title>phosphorus.ajax javascript sample</title>
        <link rel="stylesheet" type="text/css" href="media/main.css" />
    </head>
    <body>
        <form id="form1" runat="server" autocomplete="off">
            <div class="container">
                <h1>javascript sample</h1>
                <p>
                    this example shows how you can intercept phosphorus.ajax http requests
                </p>
                <p>
                    <pf:Literal
                        runat="server"
                        id="javascript_widget"
                        Tag="strong"
                        onclick="pf_samples.javascript_widget_onclick()"
                        onclicked="javascript_widget_onclicked">click me</pf:Literal> 
                    to see an example of how to intercept an http request, both before it is being sent, and after it returns from the server.
                    &nbsp;&nbsp;in this example we add a custom http parameter from the client, which we use on the server.&nbsp;&nbsp;in addition, 
                    we also change one of the return values from the server, before the dom is updated
                </p>
                <p>
                    to accomplish this, we create one hidden event called <em>"onclicked"</em> on our widget, which is mapped towards the method 
                    <em>"javascript_widget_onclicked"</em> on the server.&nbsp;&nbsp;while we add our own javascript for the <em>"onclick"</em>
                    event, where we manually raise our hidden event, adding up <em>"onbefore"</em> and <em>"onsuccess"</em> handlers for our 
                    event when we raise it
                </p>
            </div>
        </form>
    </body>
    <script type="text/javascript">

(function() {

  // namespace for our custom javascript
  pf_samples = {};

  // invoked when javascript_widget is clicked
  pf_samples.javascript_widget_onclick = function() {
      var el = pf.$('javascript_widget');
      el.raise('onclicked', {
        onbefore: pf_samples.javascript_widget_onclick_onbefore,
        onsuccess: pf_samples.javascript_widget_onclick_onsuccess
      });
  }

  pf_samples.javascript_widget_onclick_onbefore = function(pars, evt) {
    pars.custom_data = 'howdy world from client';
  }

  pf_samples.javascript_widget_onclick_onsuccess = function(json, evt) {
    json.wdg.javascript_widget.innerHTML += ' - \'' + evt + 
      '\' was executed successfully, its value before dom was updated was; \'' + this.el.innerHTML + '\'';
  }
})();
    </script>
</html>
