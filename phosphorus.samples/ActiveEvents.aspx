<%@ Page 
    Language="C#" 
    Inherits="phosphorus.five.samples.ActiveEvents"
    Codebehind="Literal.aspx.cs" %>

<!DOCTYPE html>
<html>
    <head>
    	<title>active events example</title>
        <link rel="stylesheet" type="text/css" href="media/main.css">
        <meta charset="utf-8">
    </head>
    <body>
        <form id="form1" runat="server" autocomplete="off">
            <div class="container">

                <h1>active events example</h1>

                <p>
                    this example shows you how to create and consume active events

                <pf:Literal
                    runat="server"
                    id="element"
                    RenderType="NoClose"
                    ElementType="p"
                    onclick="element_onclick">click me</pf:Literal>

                <p>
                    phosphorus contains an active event implementation.&nbsp;&nbsp;an active event, is a way to create functionality, that
                    can cross component boundaries.&nbsp;&nbsp;using active events, you can easily create late binding between components, 
                    without having your components know anything about the internals of the other party.&nbsp;&nbsp;this creates better
                    encapsulation and better polymorphism than traditional object oriented programming.&nbsp;&nbsp;it also allows function
                    invocations to cross server boundaries, serializing function invocations over the wire, to transparently invoke functionality
                    in other servers

                <p>
                    back to <a href="Default.aspx">example's home</a>

            </div>
        </form>
    </body>
</html>
