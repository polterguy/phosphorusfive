<%@ Page 
    Language="C#" 
    Inherits="phosphorus.ajax.samples.Literal"
    Codebehind="Literal.aspx.cs" %>

<!DOCTYPE html>
<html>
    <head>
    	<title>literal sample</title>
        <link rel="stylesheet" type="text/css" href="media/main.css" />
    </head>
    <body>
        <form id="form1" runat="server" autocomplete="off">
            <div class="container">
                <h1>literal sample</h1>
                <p>
                    this sample shows you how to create a literal widget
                </p>
                <pf:Literal
                    runat="server"
                    id="element"
                    Tag="p"
                    onclick="element_onclick">
                    click me
                </pf:Literal>
                <p>
                    phosphorus.ajax really has only two different types of widgets; the literal widget is for widgets with text or html content, 
                    while the container widget is for widgets having children widgets.&nbsp;&nbsp;the literal widget has an "innerHTML" property, 
                    that allows you to set its content from ajax requests.&nbsp;&nbsp;the container widget can have children added to it through 
                    its "Controls" propert.&nbsp;&nbsp;everything inside of a literal widget in your aspx markup will be rendered as plain text 
                    or html, while everything inside a container will be rendered as children controls
                </p>
                <p>
                    this means that you can only modify the contents of a container widget through its Controls collection, and only modify the 
                    contents of a literal widget through its innerHTML property
                </p>
                <p>
                    by using the "Tag" property, and combining it with custom attributes in your widgets, you can create any html markup you wish 
                    in your page, using only these two types of widgets.&nbsp;&nbsp;instead of giving you hundreds of different widgets, 
                    phosphorus.ajax gives you the tools you need to create the exact html markup you wish, while at the same time building a 
                    bridge between your servers and its clients, making ajax updates to your pages become easy
                </p>
                <p>
                    onwards to the <a href="Container.aspx">container sample</a>
                </p>
            </div>
        </form>
    </body>
</html>
