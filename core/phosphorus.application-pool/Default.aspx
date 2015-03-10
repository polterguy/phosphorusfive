<%@ Page
    Language="C#"
    Inherits="phosphorus.applicationpool.Default"
    ValidateRequest="false"
    Codebehind="Default.aspx.cs" %>

<!DOCTYPE html>
<html>
    <head runat="server">
        <meta charset="utf-8">
        <title>phosphorus five application pool</title>
    </head>
    <body>
        <form id="form1" runat="server" autocomplete="off">
            <pf:Container
                runat="server"
                class="container"
                id="container"/>
        </form>
    </body>
</html>