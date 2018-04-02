<%@ Page
    Language="C#"
    Inherits="p5.webapp.Default"
    ValidateRequest="false"
    Codebehind="Default.aspx.cs" %><!DOCTYPE html>
<html>
    <head runat="server">
        <meta charset="utf-8" />
        <title>Phosphorus Five - In the beginning there was Hyperlambda</title>
        <link rel="apple-touch-icon" sizes="180x180" href="/media/apple-touch-icon.png">
        <link rel="icon" type="image/png" sizes="32x32" href="/media/favicon-32x32.png">
        <link rel="icon" type="image/png" sizes="16x16" href="/media/favicon-16x16.png">
        <link rel="manifest" href="/media/site.webmanifest">
        <link rel="mask-icon" href="/media/safari-pinned-tab.svg" color="#5bbad5">
        <meta name="msapplication-TileColor" content="#2d89ef">
        <meta name="theme-color" content="#9aa7ed">
    </head>
    <body>
        <form id="form1" runat="server" autocomplete="off">
			<p5:Container
                runat="server"
                id="cnt"/>
        </form>
    </body>
</html>