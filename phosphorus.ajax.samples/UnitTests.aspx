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
                    <tr>
                        <td>
                            invoke event handler as 'onclick'
                        </td>
                        <td>
                            <input type="button" id="invoke_normal" class="undetermined" value="run" onclick="tests.invoke_normal(event)">
                        </td>
                    </tr>
                    <tr>
                        <td>
                            run multiple consecutive requests at the same time while server sleeps .1 seconds
                        </td>
                        <td>
                            <input type="button" id="invoke_multiple" class="undetermined" value="run" onclick="tests.invoke_multiple(event)">
                        </td>
                    </tr>
                    <tr>
                        <td>
                            add custom parameters from javascript
                        </td>
                        <td>
                            <input type="button" id="invoke_javascript" class="undetermined" value="run" onclick="tests.invoke_javascript(event)">
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
                    <tr class="spacer">
                        <td colspan="2">
                            <h3>container widgets</h3>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            change attribute of child of container widget
                        </td>
                        <td>
                            <input type="button" id="invoke_change_container_child" class="undetermined" value="run" onclick="tests.invoke_change_container_child(event)">
                        </td>
                    </tr>
                    <tr>
                        <td>
                            make container widget visible with visible children
                        </td>
                        <td>
                            <input type="button" id="invoke_make_container_visible" class="undetermined" value="run" onclick="tests.invoke_make_container_visible(event)">
                        </td>
                    </tr>
                    <tr>
                        <td>
                            make container widget visible with invisible children
                        </td>
                        <td>
                            <input type="button" id="invoke_make_container_visible_invisible_child" class="undetermined" value="run" onclick="tests.invoke_make_container_visible_invisible_child(event)">
                        </td>
                    </tr>
                    <tr>
                        <td>
                            make child invisible, then container visible
                        </td>
                        <td>
                            <input type="button" id="invoke_make_container_visible_child_invisible" class="undetermined" value="run" onclick="tests.invoke_make_container_visible_child_invisible(event)">
                        </td>
                    </tr>
                    <tr>
                        <td>
                            make child visible, then container visible
                        </td>
                        <td>
                            <input type="button" id="invoke_make_container_visible_child_visible" class="undetermined" value="run" onclick="tests.invoke_make_container_visible_child_visible(event)">
                        </td>
                    </tr>
                    <tr>
                        <td>
                            add child to container
                        </td>
                        <td>
                            <input type="button" id="invoke_add_child" class="undetermined" value="run" onclick="tests.invoke_add_child(event)">
                        </td>
                    </tr>
                    <tr>
                        <td>
                            insert child to container at top
                        </td>
                        <td>
                            <input type="button" id="invoke_insert_child" class="undetermined" value="run" onclick="tests.invoke_insert_child(event)">
                        </td>
                    </tr>
                    <tr>
                        <td>
                            add child to container, check exist
                        </td>
                        <td>
                            <input type="button" id="invoke_add_child_check_exist" class="undetermined" value="run" onclick="tests.invoke_add_child_check_exist(event)">
                        </td>
                    </tr>
                    <tr>
                        <td>
                            insert child to container at top, check exist
                        </td>
                        <td>
                            <input type="button" id="invoke_insert_child_check_exist" class="undetermined" value="run" onclick="tests.invoke_insert_child_check_exist(event)">
                        </td>
                    </tr>
                    <tr>
                        <td>
                            remove child from container
                        </td>
                        <td>
                            <input type="button" id="invoke_remove_child" class="undetermined" value="run" onclick="tests.invoke_remove_child(event)">
                        </td>
                    </tr>
                    <tr>
                        <td>
                            remove multiple children from container
                        </td>
                        <td>
                            <input type="button" id="invoke_remove_multiple" class="undetermined" value="run" onclick="tests.invoke_remove_multiple(event)">
                        </td>
                    </tr>
                    <tr>
                        <td>
                            append child to container and remove in same request
                        </td>
                        <td>
                            <input type="button" id="invoke_append_remove" class="undetermined" value="run" onclick="tests.invoke_append_remove(event)">
                        </td>
                    </tr>
                    <tr>
                        <td>
                            remove and add multiple controls from complex control tree, and check they exist in next response
                        </td>
                        <td>
                            <input type="button" id="invoke_remove_many" class="undetermined" value="run" onclick="tests.invoke_remove_many(event)">
                        </td>
                    </tr>
                    <tr class="spacer">
                        <td colspan="2">
                            <h3>active events</h3>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            raise an active event handled on page object
                        </td>
                        <td>
                            <input type="button" id="invoke_raise_page" class="undetermined" value="run" onclick="tests.invoke_raise_page(event)">
                        </td>
                    </tr>
                    <tr>
                        <td>
                            register an active event listener twice
                        </td>
                        <td>
                            <input type="button" id="invoke_register_twice" class="undetermined" value="run" onclick="tests.invoke_register_twice(event)">
                        </td>
                    </tr>
                    <tr>
                        <td>
                            unregister an active event listener
                        </td>
                        <td>
                            <input type="button" id="invoke_unregister" class="undetermined" value="run" onclick="tests.invoke_unregister(event)">
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
                        id="sandbox_invoke_normal"
                        RenderType="NoClose"
                        onclick="sandbox_invoke_normal_onclick"
                        ElementType="p" />
                    <pf:Literal
                        runat="server"
                        id="sandbox_invoke_multiple"
                        RenderType="NoClose"
                        onclick="sandbox_invoke_multiple_onclick"
                        ElementType="p" />
                    <pf:Literal
                        runat="server"
                        id="sandbox_invoke_javascript"
                        RenderType="NoClose"
                        onclick="sandbox_invoke_javascript_onclick"
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
                    <pf:Container
                        runat="server"
                        id="sandbox_invoke_change_container_child"
                        ElementType="p">
                        <pf:Literal
                            runat="server"
                            id="sandbox_invoke_change_container_child_child"
                            class="foo"
                            ElementType="strong">foo</pf:Literal>
                    </pf:Container>
                    <pf:Container
                        runat="server"
                        id="sandbox_invoke_make_container_visible"
                        Visible="false"
                        RenderType="NoClose"
                        ElementType="p">
                        <pf:Literal
                            runat="server"
                            id="sandbox_invoke_make_container_visible_child"
                            ElementType="strong">foo</pf:Literal>
                    </pf:Container>
                    <pf:Container
                        runat="server"
                        id="sandbox_invoke_make_container_visible_invisible_child"
                        Visible="false"
                        RenderType="NoClose"
                        ElementType="p">
                        <pf:Literal
                            runat="server"
                            Visible="false"
                            id="sandbox_invoke_make_container_visible_child_invisible_child"
                            ElementType="strong">foo</pf:Literal>
                    </pf:Container>
                    <pf:Container
                        runat="server"
                        id="sandbox_invoke_make_container_visible_child_invisible"
                        Visible="false"
                        RenderType="NoClose"
                        ElementType="p">
                        <pf:Literal
                            runat="server"
                            id="sandbox_invoke_make_container_visible_child_child_invisible"
                            ElementType="strong">foo</pf:Literal>
                    </pf:Container>
                    <pf:Container
                        runat="server"
                        id="sandbox_invoke_make_container_visible_child_visible"
                        Visible="false"
                        RenderType="NoClose"
                        ElementType="p">
                        <pf:Literal
                            runat="server"
                            Visible="false"
                            id="sandbox_invoke_make_container_visible_child_child_visible"
                            ElementType="strong">foo</pf:Literal>
                    </pf:Container>
                    <pf:Container
                        runat="server"
                        id="sandbox_invoke_add_child"
                        RenderType="NoClose"
                        ElementType="p">
                        <pf:Literal
                            runat="server"
                            id="sandbox_invoke_add_child_child1"
                            ElementType="strong">foo</pf:Literal>
                    </pf:Container>
                    <pf:Container
                        runat="server"
                        id="sandbox_invoke_insert_child"
                        RenderType="NoClose"
                        ElementType="p">
                        <pf:Literal
                            runat="server"
                            id="sandbox_invoke_insert_child_child1"
                            ElementType="strong">foo</pf:Literal>
                    </pf:Container>
                    <pf:Container
                        runat="server"
                        id="sandbox_invoke_add_child_check_exist"
                        RenderType="NoClose"
                        ElementType="p">
                        <pf:Literal
                            runat="server"
                            id="sandbox_invoke_add_child_check_exist_child1"
                            ElementType="strong">foo</pf:Literal>
                    </pf:Container>
                    <pf:Container
                        runat="server"
                        id="sandbox_invoke_insert_child_check_exist"
                        RenderType="NoClose"
                        ElementType="p">
                        <pf:Literal
                            runat="server"
                            id="sandbox_invoke_insert_child_check_exist_child1"
                            ElementType="strong">foo</pf:Literal>
                    </pf:Container>
                    <pf:Container
                        runat="server"
                        id="sandbox_invoke_append_remove"
                        RenderType="NoClose"
                        ElementType="p">
                        <pf:Literal
                            runat="server"
                            id="sandbox_invoke_append_remove_child"
                            ElementType="strong">foo</pf:Literal>
                    </pf:Container>
                    <pf:Container
                        runat="server"
                        id="sandbox_invoke_remove_child"
                        RenderType="NoClose"
                        ElementType="p">
                        <pf:Literal
                            runat="server"
                            id="sandbox_invoke_remove_child_child"
                            ElementType="strong">foo</pf:Literal>
                    </pf:Container>
                    <pf:Container
                        runat="server"
                        id="sandbox_invoke_remove_multiple"
                        RenderType="NoClose"
                        ElementType="p">
                        <pf:Literal
                            runat="server"
                            id="sandbox_invoke_remove_multiple_child1"
                            ElementType="strong">foo</pf:Literal>
                        <pf:Literal
                            runat="server"
                            id="sandbox_invoke_remove_multiple_child2"
                            ElementType="strong">bar</pf:Literal>
                    </pf:Container>
                    <pf:Container
                        runat="server"
                        id="sandbox_invoke_remove_many"
                        ElementType="div">
                        <pf:Literal
                            runat="server"
                            id="sandbox_invoke_remove_many_1"
                            ElementType="p">foo</pf:Literal>
                        <pf:Literal
                            runat="server"
                            id="sandbox_invoke_remove_many_2"
                            ElementType="p">bar</pf:Literal>
                        <pf:Container
                            runat="server"
                            id="sandbox_invoke_remove_many_3"
                            ElementType="div">
                            <pf:Literal
                                runat="server"
                                id="sandbox_invoke_remove_many_4"
                                ElementType="strong">foo</pf:Literal>
                            <pf:Container
                                runat="server"
                                id="sandbox_invoke_remove_many_5"
                                ElementType="div">bar</pf:Literal>
                                <pf:Literal
                                    runat="server"
                                    id="sandbox_invoke_remove_many_8"
                                    ElementType="strong">foo</pf:Literal>
                                <pf:Literal
                                    runat="server"
                                    id="sandbox_invoke_remove_many_9"
                                    ElementType="strong">bar</pf:Literal>
                                <pf:Literal
                                    runat="server"
                                    id="sandbox_invoke_remove_many_10"
                                    ElementType="strong">bar 2</pf:Literal>
                            </pf:Container>
                            <pf:Literal
                                runat="server"
                                id="sandbox_invoke_remove_many_6"
                                ElementType="strong">bar 2</pf:Literal>
                        </pf:Container>
                    </pf:Container>
                    <pf:Literal
                        runat="server"
                        id="sandbox_invoke_raise_page"
                        RenderType="NoClose"
                        ElementType="p" />
                    <pf:Literal
                        runat="server"
                        id="sandbox_invoke_register_twice"
                        RenderType="NoClose"
                        ElementType="p" />
                    <pf:Literal
                        runat="server"
                        id="sandbox_invoke_unregister"
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
