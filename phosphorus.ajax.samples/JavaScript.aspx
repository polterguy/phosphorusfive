<%@ Page 
    Language="C#" 
    Inherits="phosphorus.ajax.samples.JavaScript"
    Codebehind="JavaScript.aspx.cs" %>

<!DOCTYPE html>
<html>
    <head>
    	<title>javascript example</title>
        <link rel="stylesheet" type="text/css" href="media/main.css">
        <meta charset="utf-8">
    </head>
    <body>
        <form id="form1" runat="server" autocomplete="off">
            <div class="container">

                <h1>javascript example</h1>

                <p>
                    this example shows how you can intercept phosphorus.ajax http requests

                <p>
                    <pf:Literal
                        runat="server"
                        id="javascript_widget"
                        ElementType="strong"
                        onclick="pf_samples.javascript_widget_onclick(event)">click me</pf:Literal>
                <p>
                    above we intercept the <em>"onclick"</em> event, and add a custom parameter to the http request.&nbsp;&nbsp;when the
                    response returns, we change the return from the server, before we allow the dom to update

                <p>
                    onwards to the <a href="DynamicControls.aspx">dynamic example</a>

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
        pars.custom_data += '\'hello\'. ';
      },

      // called when a successful response is returned, but before dom is updated with return value from server
      onsuccess: function(retVal, evt) {
        if(retVal.__pf_change && retVal.__pf_change.javascript_widget) {
          retVal.__pf_change.javascript_widget.innerHTML += 'browser says \'hello again\'';
        }
      },

      // passing in custom data to event
      // note that this can also be done in our 'onbefore' callback
      // also notice that if you have form elements with the same name as your custom parameters, then your 
      // custom parameters will overwrite your form element values
      // you can send any object type you wish this way, and it will bee serialized as json automatically for you
      parameters: {
        custom_data: 'your browser says; '
      }
    });
  };
})();
        </script>
    </body>
</html>
