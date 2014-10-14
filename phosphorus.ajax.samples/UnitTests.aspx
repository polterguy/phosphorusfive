<%@ Page 
    Language="C#" 
    Inherits="phosphorus.ajax.samples.UnitTests"
    Codebehind="UnitTests.aspx.cs" %>

<!DOCTYPE html>
<html>
    <head>
    	<title>unit tests</title>
        <link rel="stylesheet" type="text/css" href="media/main.css">
        <link rel="stylesheet" type="text/css" href="media/tests.css">
        <meta charset="utf-8">
    </head>
    <body>
        <form id="form1" runat="server" autocomplete="off">
            <div class="container">

                <h1>unit tests</h1>

                <table id="tests">
                    <tr class="spacer">
                        <th>
                            <h3>description</h3>
                        </th>
                        <th>
                            <input type="button" id="run_all" class="undetermined" value="run all" onclick="tests.run_all(event)">
                        </th>
                    </tr>
                    <tr class="spacer">
                        <td colspan="2">
                            these are the unit tests for phosphorus.ajax
                        </td>
                    </tr>
                    <tr class="spacer">
                        <td colspan="2">
                            <h3>basic event handling</h3>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            invoke nothing event handler
                        </td>
                        <td>
                            <input type="button" id="invoke_empty" class="undetermined" value="run" onclick="tests.invoke_empty(event)">
                        </td>
                    </tr>
                    <tr>
                        <td>
                            invoke exception event handler
                        </td>
                        <td>
                            <input type="button" id="invoke_exception" class="undetermined" value="run" onclick="tests.invoke_exception(event)">
                        </td>
                    </tr>
                    <tr>
                        <td>
                            invoke non existing event handler
                        </td>
                        <td>
                            <input type="button" id="invoke_non_existing" class="undetermined" value="run" onclick="tests.invoke_non_existing(event)">
                        </td>
                    </tr>
                    <tr>
                        <td>
                            invoke event handler not marked as WebMethod
                        </td>
                        <td>
                            <input type="button" id="invoke_no_webmethod" class="undetermined" value="run" onclick="tests.invoke_no_webmethod(event)">
                        </td>
                    </tr>
                    <tr class="spacer">
                        <td colspan="2">
                            <h3>attributes</h3>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            change innerHTML
                        </td>
                        <td>
                            <input type="button" id="invoke_change_content" class="undetermined" value="run" onclick="tests.invoke_change_content(event)">
                        </td>
                    </tr>
                    <tr>
                        <td>
                            change two attributes on widget
                        </td>
                        <td>
                            <input type="button" id="invoke_change_two_properties" class="undetermined" value="run" onclick="tests.invoke_change_two_properties(event)">
                        </td>
                    </tr>
                    <tr>
                        <td>
                            change attribute twice in the same event handler
                        </td>
                        <td>
                            <input type="button" id="invoke_change_twice" class="undetermined" value="run" onclick="tests.invoke_change_twice(event)">
                        </td>
                    </tr>
                    <tr>
                        <td>
                            add attribute, then remove it in the same event handler
                        </td>
                        <td>
                            <input type="button" id="invoke_add_remove_same" class="undetermined" value="run" onclick="tests.invoke_add_remove_same(event)">
                        </td>
                    </tr>
                    <tr>
                        <td>
                            change attribute declared in markup
                        </td>
                        <td>
                            <input type="button" id="invoke_change_markup_attribute" class="undetermined" value="run" onclick="tests.invoke_change_markup_attribute(event)">
                        </td>
                    </tr>
                    <tr>
                        <td>
                            remove attribute declared in markup
                        </td>
                        <td>
                            <input type="button" id="invoke_remove_markup_attribute" class="undetermined" value="run" onclick="tests.invoke_remove_markup_attribute(event)">
                        </td>
                    </tr>
                    <tr>
                        <td>
                            add attribute in event handler, then remove the same attribute in a new event handler
                        </td>
                        <td>
                            <input type="button" id="invoke_add_remove" class="undetermined" value="run" onclick="tests.invoke_add_remove(event)">
                        </td>
                    </tr>
                    <tr>
                        <td>
                            remove attribute declared in markup, then add it back up in a new event handler
                        </td>
                        <td>
                            <input type="button" id="invoke_remove_add_markup_attribute" class="undetermined" value="run" onclick="tests.invoke_remove_add_markup_attribute(event)">
                        </td>
                    </tr>
                    <tr>
                        <td>
                            concatenate long attribute
                        </td>
                        <td>
                            <input type="button" id="invoke_concatenate_long_attribute" class="undetermined" value="run" onclick="tests.invoke_concatenate_long_attribute(event)">
                        </td>
                    </tr>
                    <tr>
                        <td>
                            create attribute then concatenate long string
                        </td>
                        <td>
                            <input type="button" id="invoke_create_concatenate_long_attribute" class="undetermined" value="run" onclick="tests.invoke_create_concatenate_long_attribute(event)">
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
                        id="sandbox_invoke_non_existing"
                        RenderType="NoClose"
                        ElementType="p" />
                    <pf:Literal
                        runat="server"
                        id="sandbox_invoke_no_webmethod"
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
                        id="sandbox_invoke_add_remove_same"
                        RenderType="NoClose"
                        ElementType="p" />
                    <pf:Literal
                        runat="server"
                        id="sandbox_invoke_add_remove"
                        RenderType="NoClose"
                        ElementType="p" />
                    <pf:Literal
                        runat="server"
                        id="sandbox_invoke_change_twice"
                        RenderType="NoClose"
                        ElementType="p" />
                    <pf:Literal
                        runat="server"
                        id="sandbox_invoke_change_markup_attribute"
                        RenderType="NoClose"
                        class="foo"
                        ElementType="p" />
                    <pf:Literal
                        runat="server"
                        id="sandbox_invoke_remove_markup_attribute"
                        RenderType="NoClose"
                        class="foo"
                        ElementType="p" />
                    <pf:Literal
                        runat="server"
                        id="sandbox_invoke_remove_add_markup_attribute"
                        RenderType="NoClose"
                        class="foo"
                        ElementType="p" />
                    <pf:Literal
                        runat="server"
                        id="sandbox_invoke_concatenate_long_attribute"
                        RenderType="NoClose"
                        class="x1234567890abcdefghijklmnopqrstuvwxyz"
                        ElementType="p" />
                    <pf:Literal
                        runat="server"
                        id="sandbox_invoke_create_concatenate_long_attribute"
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
