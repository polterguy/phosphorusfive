<%@ Page
    Language="C#"
    Inherits="p5.webapp.Default"
    ValidateRequest="false"
    Codebehind="Default.aspx.cs" %><!DOCTYPE html>
<html>
    <head runat="server">
        <meta charset="utf-8" />
        <title>Phosphorus Five - Got Privacy?</title>
        <meta name="viewport" content="width=device-width, initial-scale=1" />
        <link rel="apple-touch-icon" sizes="180x180" href="/apple-touch-icon.png">
        <link rel="icon" type="image/png" sizes="32x32" href="/favicon-32x32.png">
        <link rel="icon" type="image/png" sizes="16x16" href="/favicon-16x16.png">
        <link rel="manifest" href="/manifest.json">
        <link rel="mask-icon" href="/safari-pinned-tab.svg" color="#5bbad5">
        <meta name="theme-color" content="#ffffff">
    </head>
    <body>
        <form id="form1" runat="server" autocomplete="off">
			<p5:Container
                runat="server"
                id="cnt"/>
        </form>
    </body>
</html>