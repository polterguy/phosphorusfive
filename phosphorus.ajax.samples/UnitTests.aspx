<%@ Page 
    Language="C#" 
    Inherits="phosphorus.ajax.samples.UnitTests"
    Codebehind="UnitTests.aspx.cs" %>

<!DOCTYPE html>
<html>
    <head>
    	<title>unit tests</title>
        <link rel="stylesheet" type="text/css" href="media/main.css">
        <meta charset="utf-8">
        <style>
.failed {
    background-color:Red;
}
.success {
    background-color:LightGreen;
}
        </style>
    </head>
    <body>
        <form id="form1" runat="server" autocomplete="off">
            <div class="container">

                <h1>unit tests</h1>

                <p>
                    these are the unit tests for phosphorus

                <table id="tests">
                    <tr>
                        <th>description</th>
                        <th>
                            <input type="button" id="run_all" class="undetermined" value="run all" onclick="tests.run_all(event)">
                        </th>
                    </tr>
                    <tr>
                        <td colspan="2">
                            <h3>basic event handling</h3>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            will verify that an event handler can be successfully invoked, returning nothing,
                            if nothing changes on the server
                        </td>
                        <td>
                            <input type="button" id="invoke_empty" class="undetermined" value="run" onclick="tests.invoke_empty(event)">
                        </td>
                    </tr>
                    <tr>
                        <td>
                            will verify that an event handler that throws an exception will invoke the 'onerror' javascript callback
                        </td>
                        <td>
                            <input type="button" id="invoke_exception" class="undetermined" value="run" onclick="tests.invoke_exception(event)">
                        </td>
                    </tr>
                    <tr>
                        <td>
                            will verify that an event handler can successfully change a widget's innerHTML
                        </td>
                        <td>
                            <input type="button" id="invoke_change_content" class="undetermined" value="run" onclick="tests.invoke_change_content(event)">
                        </td>
                    </tr>
                    <tr>
                        <td>
                            will verify that an event handler can successfully change two properties at the same time for a widget
                        </td>
                        <td>
                            <input type="button" id="invoke_change_two_properties" class="undetermined" value="run" onclick="tests.invoke_change_two_properties(event)">
                        </td>
                    </tr>
                    <tr>
                        <td>
                            will verify that an event handler can add a property, and another event handler remove it successfully later
                        </td>
                        <td>
                            <input type="button" id="invoke_add_remove" class="undetermined" value="run" onclick="tests.invoke_add_remove(event)">
                        </td>
                    </tr>
                    <tr>
                        <td>
                            will verify that if an event handler adds an attribute, and later removes it in the same request, then nothing is returned
                        </td>
                        <td>
                            <input type="button" id="invoke_add_remove_same" class="undetermined" value="run" onclick="tests.invoke_add_remove_same(event)">
                        </td>
                    </tr>
                    <tr>
                        <td>
                            will verify that if an event handler changes an attribute twice, the correct value is returned
                        </td>
                        <td>
                            <input type="button" id="invoke_change_twice" class="undetermined" value="run" onclick="tests.invoke_change_twice(event)">
                        </td>
                    </tr>
                </table>

                <div style="display:none;" id="sandbox">
                    <pf:Literal
                        runat="server"
                        id="sandbox_invoke_empty"
                        RenderType="NoClose"
                        ElementType="p" />
                    <pf:Literal
                        runat="server"
                        id="sandbox_invoke_exception"
                        RenderType="NoClose"
                        ElementType="p" />
                    <pf:Literal
                        runat="server"
                        id="sandbox_invoke_change_content"
                        RenderType="NoClose"
                        ElementType="p" />
                    <pf:Literal
                        runat="server"
                        id="sandbox_invoke_change_two_properties"
                        RenderType="NoClose"
                        ElementType="p" />
                    <pf:Literal
                        runat="server"
                        id="sandbox_invoke_add_remove"
                        RenderType="NoClose"
                        ElementType="p" />
                    <pf:Literal
                        runat="server"
                        id="sandbox_invoke_add_remove_same"
                        RenderType="NoClose"
                        ElementType="p" />
                    <pf:Literal
                        runat="server"
                        id="sandbox_invoke_change_twice"
                        RenderType="NoClose"
                        ElementType="p" />
                </div>

                <p>
                    back to the <a href="Default.aspx">main examples</a>

            </div>
        </form>
    <script type="text/javascript" src="media/tests.js"></script>
    </body>
</html>
