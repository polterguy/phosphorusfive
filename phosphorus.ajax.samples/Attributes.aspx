<%@ Page 
    Language="C#" 
    Inherits="phosphorus.ajax.samples.Attributes"
    Codebehind="Attributes.aspx.cs" %>

<!DOCTYPE html>
<html>
    <head>
    	<title>attributes example</title>
        <link rel="stylesheet" type="text/css" href="media/main.css" />
        <style>
.green {
    background-color: LightGreen;
}

.container div {
    border:1px dashed black;
}
        </style>
    </head>
    <body>
        <form id="form1" runat="server" autocomplete="off">
            <div class="container">
                <h1>attributes example</h1>
                <p>
                    this example shows you how you can modify the attributes and properties of your widgets
                </p>
                <pf:Literal
                    runat="server"
                    id="literal">
                    some piece of text
                </pf:Literal>
                <ul>
                    <li><pf:Literal 
                            runat="server"
                            id="addClass"
                            Tag="input"
                            type="button"
                            value="add"
                            onclick="addClass_onclick"/> class attribute
                    </li>
                    <li><pf:Literal 
                            runat="server"
                            id="changeTag"
                            Tag="input"
                            type="button"
                            value="change"
                            onclick="changeTag_onclick"/> tag of element
                    </li>
                    <li><pf:Literal 
                            runat="server"
                            id="visibleChange"
                            Tag="input"
                            type="button"
                            value="make element invisible"
                            onclick="visibleChange_onclick"/>
                    </li>
                </ul>
                <p>
                    above is an example of how you can modify the attributes, class name, html tag and visibility of your widgets from 
                    server side events
                </p>
            </div>
        </form>
    </body>
</html>
