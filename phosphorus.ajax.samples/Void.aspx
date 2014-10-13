<%@ Page 
    Language="C#" 
    Inherits="phosphorus.ajax.samples.Void"
    Codebehind="Void.aspx.cs" %>

<!DOCTYPE html>
<html>
    <head>
    	<title>void example</title>
        <link rel="stylesheet" type="text/css" href="media/main.css">
        <meta charset="utf-8">
    </head>
    <body>
        <form id="form1" runat="server" autocomplete="off">
            <div class="container">

                <h1>void example</h1>

                <p>
                    this example shows how you can create void widgets

                <p>
                    <pf:Void
                        runat="server"
                        id="txt"
                        name="txt"
                        ElementType="input"
                        style="width:250px;"
                        placeholder="type text into me and click update ..."
                        type="text" />
                    <pf:Void
                        runat="server"
                        id="btn"
                        ElementType="input"
                        type="button"
                        value="update"
                        onclick="btn_onclick" />

                <pf:Literal
                    runat="server"
                    id="lbl"
                    RenderType="NoClose"
                    ElementType="p">change is the only constant</pf:Literal>

                <p>
                    a void widget has no content, neither text, nor controls.&nbsp;&nbsp;the void widget is typically used for
                    some form elements, such as <em>"select"</em>, and <em>"input"</em> elements

                <p>onwards to the <a href="JavaScript.aspx">javascript example</a>

            </div>
        </form>
    </body>
</html>
