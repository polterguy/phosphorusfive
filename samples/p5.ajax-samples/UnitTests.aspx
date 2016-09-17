
<%@ Page 
    Language="C#" 
    Inherits="p5.samples.UnitTests"
    ViewStateSessionEntries="5"
    Codebehind="UnitTests.aspx.cs" %>

<!DOCTYPE html>
<html>
    <head>
    	<title>p5.ajax unit tests</title>
        <link rel="stylesheet" type="text/css" href="media/main.css">
        <link rel="stylesheet" type="text/css" href="media/tests.css">
        <meta charset="utf-8">
    </head>
    <body>
        <form id="form1" runat="server" autocomplete="off">
            <div class="container">

                <h1>Unit tests for p5.ajax</h1>

                <table id="tests">
                    <tr class="spacer">
                        <th>
                            <h3>Description</h3>
                        </th>
                        <th>
                            <input type="button" id="run_all" class="undetermined" value="run all" onclick="tests.run_all (event)">
                        </th>
                    </tr>
                    <tr class="spacer">
                        <td colspan="2">
                            These are the unit tests for p5.ajax
                        </td>
                    </tr>
                    <tr class="spacer">
                        <td colspan="2">
                            <h3>Basic ajax event handling</h3>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Invoke nothing event handler
                        </td>
                        <td>
                            <input type="button" id="invoke_empty" class="undetermined" value="run" onclick="tests.invoke_empty(event)">
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Invoke exception event handler
                        </td>
                        <td>
                            <input type="button" id="invoke_exception" class="undetermined" value="run" onclick="tests.invoke_exception(event)">
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Invoke non existing event handler
                        </td>
                        <td>
                            <input type="button" id="invoke_non_existing" class="undetermined" value="run" onclick="tests.invoke_non_existing(event)">
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Invoke event handler not marked as WebMethod
                        </td>
                        <td>
                            <input type="button" id="invoke_no_webmethod" class="undetermined" value="run" onclick="tests.invoke_no_webmethod(event)">
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Invoke event handler as 'onclick'
                        </td>
                        <td>
                            <input type="button" id="invoke_normal" class="undetermined" value="run" onclick="tests.invoke_normal(event)">
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Run multiple consecutive requests at the same time while server sleeps .1 seconds
                        </td>
                        <td>
                            <input type="button" id="invoke_multiple" class="undetermined" value="run" onclick="tests.invoke_multiple(event)">
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Add custom parameters from JavaScript
                        </td>
                        <td>
                            <input type="button" id="invoke_javascript" class="undetermined" value="run" onclick="tests.invoke_javascript(event)">
                        </td>
                    </tr>
                    <tr class="spacer">
                        <td colspan="2">
                            <h3>Ajax widgets attributes</h3>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Change innerValue
                        </td>
                        <td>
                            <input type="button" id="invoke_change_content" class="undetermined" value="run" onclick="tests.invoke_change_content(event)">
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Change two attributes on widget
                        </td>
                        <td>
                            <input type="button" id="invoke_change_two_properties" class="undetermined" value="run" onclick="tests.invoke_change_two_properties(event)">
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Change attribute twice in the same event handler
                        </td>
                        <td>
                            <input type="button" id="invoke_change_twice" class="undetermined" value="run" onclick="tests.invoke_change_twice(event)">
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Add attribute, then remove it in the same event handler
                        </td>
                        <td>
                            <input type="button" id="invoke_add_remove_same" class="undetermined" value="run" onclick="tests.invoke_add_remove_same(event)">
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Change attribute declared in markup
                        </td>
                        <td>
                            <input type="button" id="invoke_change_markup_attribute" class="undetermined" value="run" onclick="tests.invoke_change_markup_attribute(event)">
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Remove attribute declared in markup
                        </td>
                        <td>
                            <input type="button" id="invoke_remove_markup_attribute" class="undetermined" value="run" onclick="tests.invoke_remove_markup_attribute(event)">
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Add attribute in event handler, then remove the same attribute in a new event handler
                        </td>
                        <td>
                            <input type="button" id="invoke_add_remove" class="undetermined" value="run" onclick="tests.invoke_add_remove(event)">
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Remove attribute declared in markup, then add it back up in a new event handler
                        </td>
                        <td>
                            <input type="button" id="invoke_remove_add_markup_attribute" class="undetermined" value="run" onclick="tests.invoke_remove_add_markup_attribute(event)">
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Concatenate long attribute
                        </td>
                        <td>
                            <input type="button" id="invoke_concatenate_long_attribute" class="undetermined" value="run" onclick="tests.invoke_concatenate_long_attribute(event)">
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Create attribute then concatenate long string
                        </td>
                        <td>
                            <input type="button" id="invoke_create_concatenate_long_attribute" class="undetermined" value="run" onclick="tests.invoke_create_concatenate_long_attribute(event)">
                        </td>
                    </tr>
                    <tr class="spacer">
                        <td colspan="2">
                            <h3>Container ajax widgets</h3>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Change attribute of child of container widget
                        </td>
                        <td>
                            <input type="button" id="invoke_change_container_child" class="undetermined" value="run" onclick="tests.invoke_change_container_child(event)">
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Make container widget visible with visible children
                        </td>
                        <td>
                            <input type="button" id="invoke_make_container_visible" class="undetermined" value="run" onclick="tests.invoke_make_container_visible(event)">
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Make container widget visible with invisible children
                        </td>
                        <td>
                            <input type="button" id="invoke_make_container_visible_invisible_child" class="undetermined" value="run" onclick="tests.invoke_make_container_visible_invisible_child(event)">
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Add child to container
                        </td>
                        <td>
                            <input type="button" id="invoke_add_child" class="undetermined" value="run" onclick="tests.invoke_add_child(event)">
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Insert child to container at top
                        </td>
                        <td>
                            <input type="button" id="invoke_insert_child" class="undetermined" value="run" onclick="tests.invoke_insert_child(event)">
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Add child to container, check exist
                        </td>
                        <td>
                            <input type="button" id="invoke_add_child_check_exist" class="undetermined" value="run" onclick="tests.invoke_add_child_check_exist(event)">
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Insert child to container at top, check exist
                        </td>
                        <td>
                            <input type="button" id="invoke_insert_child_check_exist" class="undetermined" value="run" onclick="tests.invoke_insert_child_check_exist(event)">
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Remove child from container
                        </td>
                        <td>
                            <input type="button" id="invoke_remove_child" class="undetermined" value="run" onclick="tests.invoke_remove_child(event)">
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Remove multiple children from container
                        </td>
                        <td>
                            <input type="button" id="invoke_remove_multiple" class="undetermined" value="run" onclick="tests.invoke_remove_multiple(event)">
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Append child to container and remove in same request
                        </td>
                        <td>
                            <input type="button" id="invoke_append_remove" class="undetermined" value="run" onclick="tests.invoke_append_remove(event)">
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Remove and add multiple controls from complex control tree, and check they exist in next response
                        </td>
                        <td>
                            <input type="button" id="invoke_remove_many" class="undetermined" value="run" onclick="tests.invoke_remove_many(event)">
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Remove one control, and add back another control, with the same ID
                        </td>
                        <td>
                            <input type="button" id="invoke_add_similar" class="undetermined" value="run" onclick="tests.invoke_add_similar(event)">
                        </td>
                    </tr>
                </table>

                <p>
                    Back to <a href="Default.aspx">landing page</a>

                <div style="display:none;" id="sandbox">
                    <p5:Literal
                        runat="server"
                        id="sandbox_invoke_empty"
                        RenderType="open"
                        ElementType="p" />
                    <p5:Literal
                        runat="server"
                        id="sandbox_invoke_exception"
                        RenderType="open"
                        ElementType="p" />
                    <p5:Literal
                        runat="server"
                        id="sandbox_invoke_non_existing"
                        RenderType="open"
                        ElementType="p" />
                    <p5:Literal
                        runat="server"
                        id="sandbox_invoke_no_webmethod"
                        RenderType="open"
                        ElementType="p" />
                    <p5:Literal
                        runat="server"
                        id="sandbox_invoke_normal"
                        RenderType="open"
                        onclick="sandbox_invoke_normal_onclick"
                        ElementType="p" />
                    <p5:Literal
                        runat="server"
                        id="sandbox_invoke_multiple"
                        RenderType="open"
                        onclick="sandbox_invoke_multiple_onclick"
                        ElementType="p" />
                    <p5:Literal
                        runat="server"
                        id="sandbox_invoke_javascript"
                        RenderType="open"
                        onclick="sandbox_invoke_javascript_onclick"
                        ElementType="p" />
                    <p5:Literal
                        runat="server"
                        id="sandbox_invoke_change_content"
                        RenderType="open"
                        ElementType="p" />
                    <p5:Literal
                        runat="server"
                        id="sandbox_invoke_change_two_properties"
                        RenderType="open"
                        ElementType="p" />
                    <p5:Literal
                        runat="server"
                        id="sandbox_invoke_add_remove_same"
                        RenderType="open"
                        ElementType="p" />
                    <p5:Literal
                        runat="server"
                        id="sandbox_invoke_add_remove"
                        RenderType="open"
                        ElementType="p" />
                    <p5:Literal
                        runat="server"
                        id="sandbox_invoke_change_twice"
                        RenderType="open"
                        ElementType="p" />
                    <p5:Literal
                        runat="server"
                        id="sandbox_invoke_change_markup_attribute"
                        RenderType="open"
                        class="foo"
                        ElementType="p" />
                    <p5:Literal
                        runat="server"
                        id="sandbox_invoke_remove_markup_attribute"
                        RenderType="open"
                        class="foo"
                        ElementType="p" />
                    <p5:Literal
                        runat="server"
                        id="sandbox_invoke_remove_add_markup_attribute"
                        RenderType="open"
                        class="foo"
                        ElementType="p" />
                    <p5:Literal
                        runat="server"
                        id="sandbox_invoke_concatenate_long_attribute"
                        RenderType="open"
                        class="x1234567890abcdefghijklmnopqrstuvwxyz"
                        ElementType="p" />
                    <p5:Literal
                        runat="server"
                        id="sandbox_invoke_create_concatenate_long_attribute"
                        RenderType="open"
                        ElementType="p" />
                    <p5:Container
                        runat="server"
                        id="sandbox_invoke_change_container_child"
                        ElementType="p">
                        <p5:Literal
                            runat="server"
                            id="sandbox_invoke_change_container_child_child"
                            class="foo"
                            ElementType="strong">foo</p5:Literal>
                    </p5:Container>
                    <p5:Container
                        runat="server"
                        id="sandbox_invoke_make_container_visible"
                        Visible="false"
                        RenderType="open"
                        ElementType="p">
                        <p5:Literal
                            runat="server"
                            id="sandbox_invoke_make_container_visible_child"
                            ElementType="strong">foo</p5:Literal>
                    </p5:Container>
                    <p5:Container
                        runat="server"
                        id="sandbox_invoke_make_container_visible_invisible_child"
                        Visible="false"
                        RenderType="open"
                        ElementType="p">
                        <p5:Literal
                            runat="server"
                            Visible="false"
                            id="sandbox_invoke_make_container_visible_child_invisible_child"
                            ElementType="strong">foo</p5:Literal>
                    </p5:Container>
                    <p5:Container
                        runat="server"
                        id="sandbox_invoke_make_container_visible_child_visible"
                        Visible="false"
                        RenderType="open"
                        ElementType="p">
                        <p5:Literal
                            runat="server"
                            Visible="false"
                            id="sandbox_invoke_make_container_visible_child_child_visible"
                            ElementType="strong">foo</p5:Literal>
                    </p5:Container>
                    <p5:Container
                        runat="server"
                        id="sandbox_invoke_add_child"
                        RenderType="open"
                        ElementType="p">
                        <p5:Literal
                            runat="server"
                            id="sandbox_invoke_add_child_child1"
                            ElementType="strong">foo</p5:Literal>
                    </p5:Container>
                    <p5:Container
                        runat="server"
                        id="sandbox_invoke_insert_child"
                        RenderType="open"
                        ElementType="p">
                        <p5:Literal
                            runat="server"
                            id="sandbox_invoke_insert_child_child1"
                            ElementType="strong">foo</p5:Literal>
                    </p5:Container>
                    <p5:Container
                        runat="server"
                        id="sandbox_invoke_add_child_check_exist"
                        RenderType="open"
                        ElementType="p">
                        <p5:Literal
                            runat="server"
                            id="sandbox_invoke_add_child_check_exist_child1"
                            ElementType="strong">foo</p5:Literal>
                    </p5:Container>
                    <p5:Container
                        runat="server"
                        id="sandbox_invoke_insert_child_check_exist"
                        RenderType="open"
                        ElementType="p">
                        <p5:Literal
                            runat="server"
                            id="sandbox_invoke_insert_child_check_exist_child1"
                            ElementType="strong">foo</p5:Literal>
                    </p5:Container>
                    <p5:Container
                        runat="server"
                        id="sandbox_invoke_append_remove"
                        RenderType="open"
                        ElementType="p">
                        <p5:Literal
                            runat="server"
                            id="sandbox_invoke_append_remove_child"
                            ElementType="strong">foo</p5:Literal>
                    </p5:Container>
                    <p5:Container
                        runat="server"
                        id="sandbox_invoke_remove_child"
                        RenderType="open"
                        ElementType="p">
                        <p5:Literal
                            runat="server"
                            id="sandbox_invoke_remove_child_child"
                            ElementType="strong">foo</p5:Literal>
                    </p5:Container>
                    <p5:Container
                        runat="server"
                        id="sandbox_invoke_remove_multiple"
                        RenderType="open"
                        ElementType="p">
                        <p5:Literal
                            runat="server"
                            id="sandbox_invoke_remove_multiple_child1"
                            ElementType="strong">foo</p5:Literal>
                        <p5:Literal
                            runat="server"
                            id="sandbox_invoke_remove_multiple_child2"
                            ElementType="strong">bar</p5:Literal>
                    </p5:Container>
                    <p5:Container
                        runat="server"
                        id="sandbox_invoke_remove_many"
                        ElementType="div">
                        <p5:Literal
                            runat="server"
                            id="sandbox_invoke_remove_many_1"
                            ElementType="p">foo</p5:Literal>
                        <p5:Literal
                            runat="server"
                            id="sandbox_invoke_remove_many_2"
                            ElementType="p">bar</p5:Literal>
                        <p5:Container
                            runat="server"
                            id="sandbox_invoke_remove_many_3"
                            ElementType="div">
                            <p5:Literal
                                runat="server"
                                id="sandbox_invoke_remove_many_4"
                                ElementType="strong">foo</p5:Literal>
                            <p5:Container
                                runat="server"
                                id="sandbox_invoke_remove_many_5"
                                ElementType="div">
                                <p5:Literal
                                    runat="server"
                                    id="sandbox_invoke_remove_many_8"
                                    ElementType="strong">foo</p5:Literal>
                                <p5:Literal
                                    runat="server"
                                    id="sandbox_invoke_remove_many_9"
                                    ElementType="strong">bar</p5:Literal>
                                <p5:Literal
                                    runat="server"
                                    id="sandbox_invoke_remove_many_10"
                                    ElementType="strong">bar 2</p5:Literal>
                            </p5:Container>
                            <p5:Literal
                                runat="server"
                                id="sandbox_invoke_remove_many_6"
                                ElementType="strong">bar 2</p5:Literal>
                        </p5:Container>
                    </p5:Container>
                    <p5:Container
                        runat="server"
                        id="sandbox_invoke_add_similar"
                        ElementType="div">
                        <p5:Literal
                            runat="server"
                            id="sandbox_invoke_add_similar_child"
                            ElementType="p">foo</p5:Literal>
                    </p5:Container>
                </div>

                <p>
                    back to the <a href="Default.aspx">main examples</a>

            </div>
        </form>
    <script type="text/javascript" src="media/tests.js"></script>
    </body>
</html>
