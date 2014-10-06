<%@ Page 
    Language="C#" 
    Inherits="phosphorus.ajax.samples.JavaScript"
    Codebehind="JavaScript.aspx.cs" %>

<!DOCTYPE html>
<html>
    <head>
    	<title>phosphorus.ajax javascript example</title>
        <link rel="stylesheet" type="text/css" href="media/main.css">
        <meta charset="utf-8">
    </head>
    <body>
        <form id="form1" runat="server" autocomplete="off">
            <div class="container">

                <h1>javascript example</h1>

                <p>this example shows how you can intercept phosphorus.ajax http requests

                <p><pf:Literal
                        runat="server"
                        id="javascript_widget"
                        Tag="strong"
                        onclick="pf_samples.javascript_widget_onclick()"
                        onclicked="javascript_widget_onclicked">click me</pf:Literal> 
                   to see an example of how to intercept an http request, both before it is being sent, and after it returns from the server.
                   &nbsp;&nbsp;in this example we add a custom http parameter from the client, which we use on the server.&nbsp;&nbsp;in addition, 
                   we also change one of the return values from the server, before the dom is updated

                <p>to accomplish this, we create one hidden event called <em>"onclicked"</em> on our widget, which is mapped towards the method 
                   <em>"javascript_widget_onclicked"</em> on the server.&nbsp;&nbsp;while we add our own javascript for the <em>"onclick"</em>
                   event, where we manually raise our hidden event, adding up <em>"onbefore"</em> and <em>"onsuccess"</em> handlers for our 
                   event when we raise it

                <p>in addition, you can check the checkbox below to make sure no viewstate is sent from the client to the server.&nbsp;&nbsp;what 
                   this really means though, is explained in our <a href="ViewState.aspx">viewstate section</a>.&nbsp;&nbsp;be sure to read this 
                   section before you start excluding viewstate on everything you do in phosphorus.ajax

                <p><input type="checkbox" name="no_viewstate" id="no_viewstate"><label for="no_viewstate">no viewstate</label>

                <p>onwards to the <a href="HTML5.aspx">html5 example</a>

            </div>
        </form>
        <script type="text/javascript">

(function() {

  // namespace for our custom javascript
  pf_samples = {};

  // invoked when javascript_widget is clicked
  pf_samples.javascript_widget_onclick = function() {

      // finding element for widget and raising our 'onclicked' event
      var el = pf.$('javascript_widget');
      el.raise('onclicked', {
        onbefore: pf_samples.javascript_widget_onclick_onbefore,
        onsuccess: pf_samples.javascript_widget_onclick_onsuccess
      });
  }

  // callback invoked just before request is sent to server
  // here you can add up your own custom parameters, or even remove existing parameters, before the 
  // http request is sent
  pf_samples.javascript_widget_onclick_onbefore = function(pars, evt) {

    // checking to see if we should exclude viewstate in request
    // you can exclude any form element data you wish with this method
    var noState = pf.$('no_viewstate').el.checked;
    if (noState) {
      delete pars.__VIEWSTATE;
    }

    // adding up a custom parameter
    // with this logic, you can transfer stuff that's not form element data
    pars.custom_data = 'your browser says; \'hello\'';
  }

  // callback invoked when request returns from server, but before json is parsed
  // here you can massage, add, parse and remove objects from the json return value before it is parsed
  // or do other things ... :)
  pf_samples.javascript_widget_onclick_onsuccess = function(json, evt) {
    if(json.widgets && json.widgets.javascript_widget) {
      json.widgets.javascript_widget.innerHTML += '. source of howdies was; \'' + evt + 
        '\'. before dom was updated, the widget had; \'' + this.el.innerHTML + '\' as its html :)';
    }
  }
})();
        </script>
    </body>
</html>
