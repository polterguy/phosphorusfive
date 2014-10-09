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
                        id="static1"
                        Tag="li"
                        HasEndTag="false"
                        onclick="item_onclick">1. static</pf:Literal>
                    <pf:Literal
                        runat="server"
                        id="static2"
                        Tag="li"
                        HasEndTag="false"
                        onclick="item_onclick">2. static</pf:Literal>
                </pf:Container>

                <p>
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
