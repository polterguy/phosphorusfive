
<%@ Page 
    Language="C#" 
    Inherits="p5.samples.Container"
    Codebehind="Container.aspx.cs" %>

<!DOCTYPE html>
<html>
    <head>
    	<title>Container widget example</title>
        <link rel="stylesheet" type="text/css" href="media/main.css">
        <meta charset="utf-8">
    </head>
    <body>
        <form id="form1" runat="server" autocomplete="off">
            <div class="container">

                <h1>Container widget example</h1>

                <p>
                    This example shows how to use container widgets

                <p5:Container
                    runat="server"
                    id="ul"
                    Element="ul">
                    <p5:Literal
                        runat="server"
                        id="li1"
                        Element="li"
                        RenderType="open"
                        onclick="element_onclick">First element (click me)</p5:Literal>
                    <p5:Literal
                        runat="server"
                        id="li2"
                        Element="li"
                        RenderType="open"
                        onclick="element_onclick">Second element (click me)</p5:Literal>
                </p5:Container>

                <p>
                    Above is a container widget. A container widget can contain other widgets and controls. The container
                    widget can have children declared in its aspx markup, or added through its <em>"Controls"</em> collection.

                <p>
                    To see an example of how beautiful the HTML markup of a p5.ajax web page might be, view the source of this page.

                <!--
                     Notice that there is little or no "magic markup", automatically created for you in the HTML of this page. Also, notice how
                     you have 100% perfect control over your resulting HTML. For instance, the "li" HTML elements above, are explicitly
                     declared as "open", which means there will be no "closing" HTML elements at the end of them. This is legal according
                     to the HTML5 standard, and easily achieved in p5.ajax by using the "open" value for the "RenderType" property.
                -->

                <p>
                    Onwards to the <a href="Void.aspx">void example</a>
            </div>
        </form>
    </body>
</html>
