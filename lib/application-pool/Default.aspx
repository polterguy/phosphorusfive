<%@ Page 
    Language="C#" 
    Inherits="phosphorus.five.applicationpool.Default"
    ValidateRequest="false"
    Codebehind="Default.aspx.cs" %>

<!DOCTYPE html>
<html>
    <head>
    	<title>phosphorus five application pool</title>
        <meta charset="utf-8">
        <link rel="stylesheet" type="text/css" href="/media/css/main.css">
    </head>
    <body>
        <form id="form1" runat="server" autocomplete="off">
            <pf:Container
                runat="server"
                class="container"
                id="container" />
        </form>
    </body>
</html>
