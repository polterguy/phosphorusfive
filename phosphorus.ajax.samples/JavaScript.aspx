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
                        onclick="pf_samples.javascript_widget_onclick(event)">click me</pf:Literal> 
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
  pf_samples.javascript_widget_onclick = function(event) {

    // finding element for widget and raising our 'onclicked' event
    var el = pf.$(event.target);
    el.raise('javascript_widget_onclicked', {

      // called just before request is sent
      onbefore: function(pars, evt) {
        // checking to see if we should exclude viewstate in request
        // you can exclude any form element data you wish with this method
        var noState = pf.$('no_viewstate').el.checked;
        if (noState) {
          delete pars.__VIEWSTATE;
        }
      },

      // called when a successful response is returned, but before dom is updated with return value from server
      onsuccess: function(retVal, evt) {
        if(retVal.widgets && retVal.widgets.javascript_widget) {
          retVal.widgets.javascript_widget.innerHTML += '. source of howdies was; \'' + evt + 
            '\'. before dom was updated, the widget had; \'' + this.el.innerHTML + '\' as its html. widget was clicked ';
        }
      },

      // passing in custom data to event
      // note that this can also be done in our 'onbefore' callback
      // also notice that if you have form elements with the same name as your custom parameters, then your 
      // custom parameters will overwrite your form element values
      // you can send any object type you wish this way, and it will bee serialized as json automatically for you
      parameters: {
        custom_data: 'your browser says; \'hello\''
      }
    });
  };
})();
        </script>
    </body>
</html>
