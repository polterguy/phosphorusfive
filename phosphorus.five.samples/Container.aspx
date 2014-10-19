<%@ Page 
    Language="C#" 
    Inherits="phosphorus.five.samples.Container"
    Codebehind="Container.aspx.cs" %>

<!DOCTYPE html>
<html>
    <head>
    	<title>container example</title>
        <link rel="stylesheet" type="text/css" href="media/main.css">
        <meta charset="utf-8">
    </head>
    <body>
        <form id="form1" runat="server" autocomplete="off">
            <div class="container">

                <h1>container example</h1>

                <p>
                    this example shows how to use container widgets

                <pf:Container
                    runat="server"
                    id="ul"
                    ElementType="ul">
                    <pf:Literal
                        runat="server"
                        id="li1"
                        ElementType="li"
                        RenderType="NoClose"
                        onclick="element_onclick">first element (click me)</pf:Literal>
                    <pf:Literal
                        runat="server"
                        id="li2"
                        ElementType="li"
                        RenderType="NoClose"
                        onclick="element_onclick">second element (click me)</pf:Literal>
                </pf:Container>

                <p>
                    above is a container widget.&nbsp;&nbsp;a container widget can contain other widgets and controls.&nbsp;&nbsp;the container
                    widget can have children declared in its aspx markup, or added through its <em>"Controls"</em> collection

                <p>
                    onwards to the <a href="Void.aspx">void example</a>
            </div>
        </form>
    </body>
</html>
