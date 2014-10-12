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
        <style>
.turtles {
    background-color:Cyan;
}
        </style>
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
                        style="width:100px;"
                        name="txt"
                        placeholder="text of element ..." />

                    <pf:Void
                        runat="server"
                        id="append"
                        Tag="input"
                        type="button"
                        onclick="append_onclick"
                        value="append" />

                    <pf:Void
                        runat="server"
                        id="insert_top"
                        Tag="input"
                        type="button"
                        onclick="insert_top_onclick"
                        value="insert at top" />

                    <pf:Void
                        runat="server"
                        id="insert_at_random"
                        Tag="input"
                        type="button"
                        onclick="insert_at_random_onclick"
                        value="insert at random" />

                    <pf:Void
                        runat="server"
                        id="replace_random"
                        Tag="input"
                        type="button"
                        onclick="replace_random_onclick"
                        value="replace random element" />

                    <pf:Void
                        runat="server"
                        id="turtle_insert"
                        Tag="input"
                        type="button"
                        onclick="turtle_insert_onclick"
                        value="append and show random love" />

                    <pf:Void
                        runat="server"
                        id="love_bomb"
                        Tag="input"
                        type="button"
                        onclick="love_bomb_onclick"
                        value="love bomb" />

                    <pf:Void
                        runat="server"
                        id="cut_the_crap"
                        Tag="input"
                        type="button"
                        onclick="cut_the_crap_onclick"
                        value="cut the crap" />

                    <br />

            </div>
        </form>
    </body>
</html>
