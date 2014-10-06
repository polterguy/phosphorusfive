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
                    id="literal"
                    Tag="p">
                    some piece of text
                </pf:Literal>
                <ul>
                    <li><pf:Literal 
                            runat="server"
                            id="toggleClass"
                            Tag="input"
                            type="button"
                            value="toggle class"
                            onclick="toggleClass_onclick"/> attribute
                    </li>
                    <li><pf:Literal 
                            runat="server"
                            id="changeTag"
                            Tag="input"
                            type="button"
                            value="change tag"
                            onclick="changeTag_onclick"/> of element
                    </li>
                    <li><pf:Literal 
                            runat="server"
                            id="toggleVisibility"
                            Tag="input"
                            type="button"
                            value="toggle visibility"
                            onclick="toggleVisibility_onclick"/> of element
                    </li>
                </ul>
                <p>
                    above is an example of how you can modify the attributes, class name, html tag, and visibility of your widgets, 
                    from server side events.&nbsp;&nbsp;if you inspect the http traffic, you will see that only tiny amounts of 
                    data is sent to and from the server, still the framework is able to maintain state on your page, and keep track 
                    of all the changes you do, and only send back the changes, and nothing more
                </p>
                <p>
                    if you inspect the dom of your page, you will notice that when the widget above becomes invisible, it is still 
                    rendering an invisible span html element.&nbsp;&nbsp;this is a necessary evil, since an invisible widget might 
                    become visible some day, and unless we render that invisible span tag, then we have no ways to determine where 
                    to put the widget, once it becomes visible.&nbsp;&nbsp;this is only true for the outer most widgets though, and 
                    does not apply to entire hierarchies of widgets.&nbsp;&nbsp;if you have a widget which is invisible, that 
                    contains hundreds of children widgets, then only the outer most root widget of your hierarchy will be rendered with 
                    its invisible span element.&nbsp;&nbsp;if you wish, you can override the invisible tag element being used with 
                    the <em>"InvisibleTag"</em> property.&nbsp;&nbsp;changing the invisible tag is sometimes necessary when rendering 
                    invisible list item elements for instance
                </p>
            </div>
        </form>
    </body>
</html>
