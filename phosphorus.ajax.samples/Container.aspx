<%@ Page 
    Language="C#" 
    Inherits="phosphorus.ajax.samples.Container"
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

                <p>this example shows how to use container widgets

                <pf:Container
                    runat="server"
                    id="ul"
                    Tag="ul">
                    <pf:Literal
                        runat="server"
                        id="li1"
                        Tag="li"
                        HasEndTag="false"
                        onclick="element_onclick">first element (click me)</pf:Literal>
                    <pf:Literal
                        runat="server"
                        id="li2"
                        Tag="li"
                        HasEndTag="false"
                        onclick="element_onclick">second element (click me)</pf:Literal>
                </pf:Container>
                <p>above is a container widget containing two literal children controls.&nbsp;&nbsp;we have rendered the container with the 
                   "ul" html tag, while the two inner literal widgets are rendered with the "li" tag.&nbsp;&nbsp;this creates an unordered 
                   list for you, where both of the list items are clickable, and will modify their contents when clicked.&nbsp;&nbsp;the only 
                   real difference between a literal widget and a container widget, is that a container widget can contain children widgets, 
                   while a literal widget will parse its contents as text or html

                <p>you can nest as many container widgets as you wish, to create the exact markup you wish for your page.&nbsp;&nbsp;this 
                   allows you to create more complex html objects, by combining the existing widgets from phosphorus.ajax.&nbsp;&nbsp;if 
                   this is not enough for you, you can also inherit your own widgets from the base widget class; "Widget" to roll your own 
                   widget entirely

                <p>onwards to the <a href="FormData.aspx">form data example</a>
            </div>
        </form>
    </body>
</html>
