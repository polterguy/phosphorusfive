<%@ Page 
    Language="C#" 
    Inherits="phosphorus.ajax.samples.DynamicControls"
    StoreViewStateInSession="true"
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

                <p>
                    this example shows you how you can dynamically add, remove and change controls

                <pf:Container
                    runat="server"
                    id="list"
                    ElementType="ul">
                    <pf:Literal
                        runat="server"
                        id="static1"
                        ElementType="li"
                        RenderType="NoClose"
                        onclick="item_onclick">1. static</pf:Literal>
                    <pf:Literal
                        runat="server"
                        id="static2"
                        ElementType="li"
                        RenderType="NoClose"
                        onclick="item_onclick">2. static</pf:Literal>
                </pf:Container>

                <p>
                    <pf:Void
                        runat="server"
                        id="txt"
                        ElementType="input"
                        type="text"
                        style="width:100px;"
                        name="txt"
                        placeholder="text of element ..." />

                    <pf:Void
                        runat="server"
                        id="append"
                        ElementType="input"
                        type="button"
                        onclick="append_onclick"
                        value="append" />

                    <pf:Void
                        runat="server"
                        id="insert_top"
                        ElementType="input"
                        type="button"
                        onclick="insert_top_onclick"
                        value="insert at top" />

                    <pf:Void
                        runat="server"
                        id="insert_at_random"
                        ElementType="input"
                        type="button"
                        onclick="insert_at_random_onclick"
                        value="insert at random" />

                    <pf:Void
                        runat="server"
                        id="replace_random"
                        ElementType="input"
                        type="button"
                        onclick="replace_random_onclick"
                        value="replace random element" />

                    <pf:Void
                        runat="server"
                        id="love_bomb"
                        ElementType="input"
                        type="button"
                        onclick="love_bomb_onclick"
                        value="love bomb" />

                    <pf:Void
                        runat="server"
                        id="harvest_love"
                        ElementType="input"
                        type="button"
                        onclick="harvest_love_onclick"
                        value="harvest love" />

                    <pf:Void
                        runat="server"
                        id="update"
                        ElementType="input"
                        type="button"
                        onclick="update_onclick"
                        value="update" />

                <p>
                    the <em>"Container"</em> widget allows for dynamically adding, removing and updating its controls.&nbsp;&nbsp;use the
                    <em>"CreatePersistentControl"</em> method to create a control you wish to let the widget remember across postbacks

                <p>
                    this example also stores its viewstate in the session by utilizing the <em>"StoreViewStateInSession"</em> feature of 
                    the <em>"AjaxPage"</em> helper class

                <p>
                    back to <a href="Default.aspx">example's home</a>
            </div>
        </form>
    </body>
</html>
