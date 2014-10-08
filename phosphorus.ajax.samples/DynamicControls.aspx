<%@ Page 
    Language="C#" 
    Inherits="phosphorus.ajax.samples.DynamicControls"
    Codebehind="DynamicControls.aspx.cs" %>

<!DOCTYPE html>
<html>
    <head>
    	<title>dynamic controls example</title>
        <link rel="stylesheet" type="text/css" href="media/main.css">
        <meta charset="utf-8">
    </head>
    <body>
        <form id="form1" runat="server" autocomplete="off">
            <div class="container">

                <h1>dynamic controls example</h1>

                <p>this example shows you how you can dynamically add and remove controls to your page

                <pf:Container
                    runat="server"
                    id="list"
                    Tag="ul">
                    <pf:Literal
                        runat="server"
                        id="first"
                        Tag="li"
                        HasEndTag="false"
                        onclick="item_onclick">toilet paper</pf:Literal>
                    <pf:Literal
                        runat="server"
                        id="second"
                        Tag="li"
                        HasEndTag="false"
                        onclick="item_onclick">bacon</pf:Literal>
                </pf:Container>

                <pf:Container
                    runat="server"
                    id="position"
                    style="width:100px;"
                    name="position"
                    Tag="select">
                    <pf:Literal
                        runat="server"
                        Tag="option"
                        value="before"
                        id="before">before</pf:Literal>
                    <pf:Literal
                        runat="server"
                        Tag="option"
                        value="after"
                        id="after">after</pf:Literal>
                </pf:Container>

                <pf:Container
                    runat="server"
                    id="child"
                    name="child"
                    style="width:300px;"
                    Tag="select" />

                <pf:Void
                    runat="server"
                    id="txt"
                    Tag="input"
                    type="text"
                    name="txt"
                    placeholder="text of element ..." />

                <pf:Void
                    runat="server"
                    id="insert"
                    Tag="input"
                    type="button"
                    onclick="insert_onclick"
                    value="insert" />

            </div>
        </form>
    </body>
</html>
