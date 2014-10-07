<%@ Page 
    Language="C#" 
    Inherits="phosphorus.ajax.samples.Events"
    Codebehind="Events.aspx.cs" %>

<!DOCTYPE html>
<html>
    <head>
    	<title>phosphorus.ajax events example</title>
        <link rel="stylesheet" type="text/css" href="media/main.css">
        <meta charset="utf-8">
    </head>
    <body>
        <form id="form1" runat="server" autocomplete="off">
            <div class="container">

                <h1>events example</h1>

                <p>this example shows how you can raise all sorts of different events on widgets

                <pf:Container
                    runat="server"
                    id="ul"
                    Tag="ul">

                    <pf:Literal
                        runat="server"
                        id="li1"
                        Tag="li"
                        HasEndTag="false"
                        onclick="li1_onclick">first element (click me)</pf:Literal>

                    <pf:Literal
                        runat="server"
                        id="li2"
                        Tag="li"
                        HasEndTag="false"
                        onmouseover="li2_onmouseover"
                        onmouseout="li2_onmouseout">second element (hover me)</pf:Literal>

                    <pf:Literal
                        runat="server"
                        id="li3"
                        Tag="li"
                        HasEndTag="false"
                        onmousedown="li3_onmousedown"
                        onmouseup="li3_onmouseup">third element (click mouse and hold down)</pf:Literal>

                    <pf:Container
                        runat="server"
                        id="li4"
                        HasEndTag="false"
                        Tag="li"><pf:Void
                            runat="server"
                            id="li4_txt"
                            name="li4_txt"
                            Tag="input"
                            type="text"
                            onchange="li4_txt_onchange"
                            placeholder="type into me and click tab ..." /> - <pf:Literal
                                runat="server"
                                id="li4_span"
                                Tag="span" /></pf:Container>

                    <pf:Literal
                        runat="server"
                        id="li5"
                        Tag="li"
                        HasEndTag="false"
                        onclick="alert('from client side')">fifth element (click me)</pf:Literal>
                </pf:Container>

                <p>the above example shows a container widget with a couple of child widgets rendered as a list, each handling different types of 
                   events.&nbsp;&nbsp;all attributes starting with "on" will be treated as client side events.&nbsp;&nbsp;if the contents of an 
                   attribute which is an event contains only characters which are legal method names for a .net method, then the event will be 
                   assumed to be a server side event, and send mapping code to the client that maps up the event automatically towards your server 
                   side event.&nbsp;&nbsp;if the event contains any letters that are not legal characters for a method in .net, then the event 
                   will be assumed to be a piece of javascript, and sent unchanged from the server when the attribute is rendered
                   
                <p>legal characters for a .net method are a-z, A-Z, 0-9 and _
                
                <p>this allows you to easily create both javascript onxxx output, while at the same time wire up .net methods on the server side
                   automatically.&nbsp;&nbsp;the fifth element above demonstrates client side events, all other elements have server side mapped 
                   events

                <p>onwards to the <a href="JavaScript.aspx">javascript example</a>

            </div>
        </form>
    </body>
</html>
