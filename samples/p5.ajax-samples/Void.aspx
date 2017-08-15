
<%@ Page 
    Language="C#" 
    Inherits="p5.samples.Void"
    Codebehind="Void.aspx.cs" %>

<!DOCTYPE html>
<html>
    <head>
    	<title>Void widget example</title>
        <link rel="stylesheet" type="text/css" href="media/main.css">
        <meta charset="utf-8">
    </head>
    <body>
        <form id="form1" runat="server" autocomplete="off">
            <div class="container">

                <h1>Void widget example</h1>

                <p>
                    This example shows how you can create void widgets

                <p>
                    <p5:Void
                        runat="server"
                        id="txtBox"
                        name="txtBox"
                        Element="input"
                        style="width:250px;"
                        placeholder="type text into me and click update ..."
                        type="text" />
                    <p5:Void
                        runat="server"
                        id="btn"
                        Element="input"
                        type="button"
                        value="update"
                        onclick="btn_onclick" />

                <p5:Literal
                    runat="server"
                    id="lbl"
                    RenderType="open"
                    ElementType="p">Change is the only constant in the Universe</p5:Literal>

                <p>
                    A void widget has no content, neither text, nor controls. The void widget is typically used for
                    some form elements, such as <em>"input"</em> elements.

                <p>
                    Typically, you would in addition to giving your input elements an id, also give them a <em>"name"</em> property also.
                    If you do not, they will not have their values serialized back to the server upon Ajax callbacks. Normally, you would
                    use the same value for the <em>"name"</em> property, as you use for their <em>"id"</em> property.

                <p>
                    Onwards to the <a href="JavaScript.aspx">JavaScript example</a>

            </div>
        </form>
    </body>
</html>
