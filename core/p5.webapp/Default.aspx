<%@ Page
    Language="C#"
    Inherits="p5.webapp.Default"
    ValidateRequest="false"
    Codebehind="Default.aspx.cs" %><!DOCTYPE html>
<html>
    <head runat="server">
        <meta charset="utf-8" />
        <title>Phosphorus Five - In the beginning there was Hyperlambda</title>
    </head>
    <body>
        <form id="form1" runat="server" autocomplete="off">
			<p5:Container
                runat="server"
                id="cnt"/>
        </form>
    </body>
</html>