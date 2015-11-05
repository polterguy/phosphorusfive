<%@ Page
    Language="C#"
    Inherits="p5.webapp.Default"
    ValidateRequest="false"
    Codebehind="Default.aspx.cs" %>

<!DOCTYPE html>
<html>
    <head runat="server">
        <meta charset="utf-8">
        <title>phosphorus five</title>
    </head>
    <body>
        <form id="form1" runat="server" autocomplete="off">
            <p5:Container
                runat="server"
                class="container"
                id="container"/>
        </form>
    </body>
</html>