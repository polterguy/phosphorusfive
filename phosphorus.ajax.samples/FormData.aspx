<%@ Page 
    Language="C#" 
    Inherits="phosphorus.ajax.samples.FormData"
    Codebehind="FormData.aspx.cs" %>

<!DOCTYPE html>
<html>
    <head>
    	<title>form element sample</title>
        <link rel="stylesheet" type="text/css" href="media/main.css" />
    </head>
    <body>
        <form id="form1" runat="server" autocomplete="off">
            <div class="container">
                <h1 id="header">form element sample</h1>
                <p>
                    this example shows how you can create form elements
                </p>
                <pf:Container
                    runat="server"
                    id="txt"
                    name="txt"
                    placeholder="type into me, then click tab"
                    Tag="input"
                    type="text"
                    onchange="txt_onchange" />
                <pf:Literal
                    runat="server"
                    id="lbl"
                    Tag="p" />
                <p>
                    phosphorus.ajax only submits form elements which have a valid 'name' attribute, and only if they are not disabled.&nbsp;&nbsp;
                    this means that if you wish to have a widget behave as a form data element, you must give it a unique 'name' attribute, which 
                    must be unique.&nbsp;&nbsp;the name attribute can most of the times be the same as the 'id' property
                </p>
                <p>
                    the value of the element is automatically taken care of on the server side, and stored into the correct widgets and attributes 
                    of widgets.&nbsp;&nbsp;you can create all sorts of form data elements you wish, by changing the 'Tag' property of your widget, 
                    combined with adding a 'type' attribute to your widgets
                </p>
                <p>
                    onwards to the <a href="Events.aspx">event sample</a>
                </p>
            </div>
        </form>
    </body>
</html>
