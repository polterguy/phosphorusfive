
<%@ Page 
    Language="C#" 
    Inherits="p5.samples.DynamicControls"
    Codebehind="DynamicControls.aspx.cs" %>

<!DOCTYPE html>
<html>
    <head>
    	<title>Dynamic widget example</title>
        <link rel="stylesheet" type="text/css" href="media/main.css">
        <meta charset="utf-8">
    </head>
    <body>
        <form id="form1" runat="server" autocomplete="off">
            <div class="container">

                <h1>Dynamic widget example</h1>

                <p>
                    This example shows you how you can dynamically add and remove children widgets of the Container widget

                <p5:Container
                    runat="server"
                    id="List"
                    Element="ul">
                    <p5:Literal
                        runat="server"
                        id="static1"
                        onclick="item_onclick"
                        Element="li">Static 1</p5:Literal>
                    <p5:Literal
                        runat="server"
                        id="static2"
                        onclick="item_onclick"
                        Element="li">Static 2</p5:Literal>
                </p5:Container>

                <p>
                    <p5:Void
                        runat="server"
                        id="Txt"
                        Element="input"
                        type="text"
                        style="width:150px;padding:5px;"
                        name="txt"
                        placeholder="Text of element ..." />

                    <p5:Void
                        runat="server"
                        id="append"
                        Element="input"
                        type="button"
                        onclick="append_onclick"
                        value="Append" />

                    <p5:Void
                        runat="server"
                        id="insert_top"
                        Element="input"
                        type="button"
                        onclick="insert_top_onclick"
                        value="Insert at top" />

                    <p5:Void
                        runat="server"
                        id="insert_at_random"
                        Element="input"
                        type="button"
                        onclick="insert_at_random_onclick"
                        value="Insert at random" />

                    <p5:Void
                        runat="server"
                        id="replace_random"
                        Element="input"
                        type="button"
                        onclick="replace_random_onclick"
                        value="Replace random element" />

                    <p5:Void
                        runat="server"
                        id="love_bomb"
                        Element="input"
                        type="button"
                        onclick="love_bomb_onclick"
                        value="Love bomb" />

                    <p5:Void
                        runat="server"
                        id="harvest_love"
                        Element="input"
                        type="button"
                        onclick="harvest_love_onclick"
                        value="Harvest love" />

                    <p5:Void
                        runat="server"
                        id="Update"
                        Element="input"
                        type="button"
                        onclick="update_onclick"
                        value="Update" />

                <p>
                    The <em>"Container"</em> widget allows for dynamically adding, removing and updating its controls. Use the
                    <em>"CreatePersistentControl"</em> method to create a control you wish to let the widget remember across postbacks.

                <p>
                    All widgets appended to a <em>"Container"</em> widget using the <em>"CreatePersistentControl"</em> method, will automatically be remembered
                    across postbacks for you, and re-created on the server-side upon every request.

                <p>
                    If you wish to <em>"persistently remove"</em> a widget from a Container widget, you can have p5.ajax remember which widgets were dynamically
                    removed automatically for you, by using the <em>"RemoveControlPersistent"</em> method on your Container widget.

                <p>
                    Below is an example of how this would work with "select" HTML elements.

                <p5:Literal
                    runat="server"
                    id="Literal5"
                    innerValue="Selected option" />
                <p>
                    <p5:Container
                        runat="server"
                        id="Container2"
                        onchange="select_change"
                        name="Container2"
                        Element="select">
                        <p5:Literal
                            runat="server"
                            id="Literal3"
                            value="Option1"
                            Element="option">Option 1</p5:Literal>
                        <p5:Literal
                            runat="server"
                            id="Literal4"
                            value="Option2"
                            selected
                            Element="option">Option 2</p5:Literal>
                        <p5:Literal
                            runat="server"
                            id="Literal6"
                            value="Option,with,comma"
                            Element="option">Option with comma value</p5:Literal>
                    </p5:Container>
                    <p5:Literal
                        runat="server"
                        id="myBtn"
                        Element="button"
                        innerValue="Append options"
                        onclick="myBtn_onclick" />
                    <p5:Literal
                        runat="server"
                        id="myBtn2"
                        Element="button"
                        innerValue="Select 2"
                        onclick="myBtn2_onclick" />

                <p>
                    Back to <a href="Default.aspx">landing page</a>
            </div>
        </form>
    </body>
</html>
