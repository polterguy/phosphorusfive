
<%@ Page 
    Language="C#" 
    AutoEventWireup="true"
    Inherits="p5.samples.Postback"
    RemoveViewState="false"
    Codebehind="Postback.aspx.cs" %>

<!DOCTYPE html>
<html>
    <head>
        <title>p5.ajax Postback example</title>
        <link rel="stylesheet" type="text/css" href="media/main.css">
        <meta charset="utf-8">
    </head>
    <body>
        <form id="form1" runat="server" autocomplete="off">
            <div class="container">

				<h1>Mixing p5.ajax with traditional WebControls</h1>

    			<p>
    				Notice, if you wish to have p5.ajax interact with traditional controls, you'll need to explicitly mark
    				your page in its page directive as "RemoveViewState", and set its value to "false". This is because of
    				that p5.ajax by default will remove all traces of ViewState unless told otherwise. And unless you have at
    				least an empty __VIEWSTATE HTTP POST parameter, then traditional ASP.NET WebControls will not work.

				<p>
                    <p5:Literal
                        runat="server"
                        ID="btnAdd"
                        Element="button"
                        onclick="add_ctrl">
                        Click me!
                    </p5:Literal>

                    <p5:Container
                        runat="server"
                        ID="mainCtr"
                        Element="ul" />
                    
                    <asp:Button ID="btnPostback" 
            			runat="server" 
            			Text="Postback!!" 
            			OnClick="btnPostback_Click" />

			</div>
        </form>
    </body>
</html>