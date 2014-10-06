<%@ Page 
    Language="C#" 
    Inherits="phosphorus.ajax.samples.ViewState"
    Codebehind="ViewState.aspx.cs" %>

<!DOCTYPE html>
<html>
    <head>
    	<title>viewstate example</title>
        <link rel="stylesheet" type="text/css" href="media/main.css">
        <style>
.green {
    background-color:LightGreen;
}

.red {
    background-color:#ffaaaa;
}
        </style>
        <meta charset="utf-8">
    </head>
    <body>
        <form id="form1" runat="server" autocomplete="off">
            <div class="container">

                <h1>viewstate example</h1>

                <p>this example shows you how phosphorus.ajax handles viewstate

                <pf:Literal
                    runat="server"
                    id="content" 
                    HasEndTag="false"
                    Tag="p">
                    this is a fairly long piece of text, and by default, all properties and attributes of literal controls will 
                    be saved into the viewstate.&nbsp;&nbsp; this makes the bandwidth usage increase significantly.&nbsp;&nbsp;
                    often this is convenient however, since it allows you to not having to track the state of your widgets yourself.
                    &nbsp;&nbsp;but sometimes this is not necessary, and only adds overhead for your http requests.&nbsp;&nbsp;on 
                    this particular widget, and the associated textarea, we have disabled viewstate by default, since it is not 
                    needed.&nbsp;&nbsp;watch how this decrease the size of your http requests.&nbsp;&nbsp;if you wish to see the 
                    difference in size of your http requests, then change this behavior by checking the <em>"viewstate"</em> 
                    checkbox, and inspect your http requests to see how this makes your http requests blow up in size
                </pf:Literal>

                <p>
                    <pf:Literal
                        runat="server"
                        id="txt"
                        name="txt"
                        rows="10"
                        EnableViewState="false"
                        style="width:950px;display:block;float:left;"
                        Tag="textarea">Lorem ipsum dolor sit amet, consectetur adipiscing elit.  Morbi malesuada, turpis id dapibus elementum, risus justo volutpat nisl, ac consequat turpis risus eget diam.  Donec tristique pellentesque nisi, eget blandit dolor aliquet a.  Aliquam erat volutpat.  Curabitur id massa dapibus, maximus neque non, imperdiet lacus.  Pellentesque posuere ligula tortor, nec eleifend nisi auctor nec.  Integer semper dapibus turpis ac posuere.  Morbi sed massa a mi lobortis viverra.  Vestibulum ante ipsum primis in faucibus orci luctus et ultrices posuere cubilia Curae; Morbi sodales sem at urna eleifend, eu molestie dolor dapibus</pf:Literal>

                    <pf:Literal
                        runat="server"
                        id="viewState"
                        name="viewState"
                        Tag="input"
                        HasEndTag="false"
                        style="display:block;clear:both;float:left;width:20px;"
                        type="checkbox" /><label for="viewState" style="width:120px;float:left;display:block;">viewstate</label>

                    <pf:Literal
                        runat="server"
                        id="submit"
                        Tag="input"
                        type="button" 
                        HasEndTag="false"
                        style="display:block;float:left;width:200px;"
                        value="submit new text"
                        onclick="submit_onclick" />

                    <pf:Literal
                        runat="server"
                        id="changeClass"
                        Tag="input"
                        type="button" 
                        HasEndTag="false"
                        style="display:block;float:left;width:200px;"
                        value="change class"
                        onclick="changeClass_onclick" />

                    <pf:Literal
                        runat="server"
                        id="makeInVisible"
                        Tag="input"
                        type="button" 
                        style="display:block;float:left;width:200px;"
                        HasEndTag="false"
                        value="make invisible"
                        onclick="makeInVisible_onclick" />

                    <pf:Literal
                        runat="server"
                        id="makeVisible"
                        Tag="input"
                        type="button" 
                        style="display:block;float:left;width:200px;"
                        HasEndTag="false"
                        value="make visible"
                        onclick="makeVisible_onclick" />

                <p>try to click all buttons above in order from left to right, and notice the value of the paragraph when it is being 
                   rendered visible again.&nbsp;&nbsp;then try to turn on viewstate, and do the same, and compare what your result 
                   becomes when you make the widget visible again

                <p>when you're not tracking viewstate for the paragraph literal control, the control will not be able to remember 
                   its content when it is set to invisible.&nbsp;&nbsp;try to change its content through the textarea, and its 
                   class, for then to make it invisible and then again visible.&nbsp;&nbsp;both with viewstate turned on, and with 
                   viewstate turned off, and see the difference.&nbsp;&nbsp;this is the disadvantage you get when you turn off viewstate

                <p>if you turn off viewstate, and debug this page, and set a breakpoint on the server side, you will also see that none 
                   of the attributes and properties of the textarea and your paragraph are being remembered across http requests.&nbsp;&nbsp;
                   have this in mind before you turn off viewstate on your controls.&nbsp;&nbsp;without viewstate, your controls are no 
                   longer remembering any of their state, except whether or not they're visible or not

                <p>by enabling viewstate on your widgets, your http traffic can easily grow by several orders of magnitude.&nbsp;&nbsp;it is 
                   important that you think carefully about which widgets you wish to enable viewstate for, and which you wish to disable 
                   it for.&nbsp;&nbsp;this is not unique for phosphorus.ajax.&nbsp;&nbsp;but what is unique though, is its beautiful 
                   handling of viewstate, allowing you to turn it off, while still functioning perfectly, as it should do

                <p>another thing that is unique for phosphorus.ajax, is its ability to incrementally update all values of its widgets, both 
                   the form elements, viewstate, and all other widgets on your page.&nbsp;&nbsp;if you for instance add one character to 
                   the textarea above, and click submit, then only that single character you added will be returned from the server, if you 
                   have viewstate enabled.&nbsp;&nbsp;if you inspect the http request using your browser, you can see this for yourself.&nbsp;&nbsp;
                   below you can try this for yourself

                <p><pf:Literal
                        runat="server"
                        id="addOne"
                        Tag="input"
                        type="button" 
                        HasEndTag="false"
                        value="add one x"
                        onclick="addOne_onclick" />

                <p>the above feature, combined with the ability to turn off viewstate on individual controls, should easily allow you to 
                   save at least 50% of the bandwidth usage, compared to other ajax libraries that does not have these features that are built 
                   on top of asp.net

                <h2>completely remove viewstate sent from server</h2>

                <p>you can also completely eliminate viewstate sent back from the server, by setting the EnableViewState property on the 
                   Manager class to false.&nbsp;&nbsp;but this might break your page.&nbsp;&nbsp;especially if you're making widgets visible 
                   or invisible during the request, since it will also eliminate all ControlState for your requests.&nbsp;&nbsp;sometimes 
                   this might be useful though for some of your http requests that are returning only json custom objects back to the client, 
                   and who are not meant to change properties or attributes of widgets on your page.&nbsp;&nbsp;this will make your http 
                   response <strong>ultra lightweight</strong> though, and can be useful for some scenarios, such as when interacting with 3rd 
                   party libraries, such as ExtJS, jQuery and such, to databind values on the client side, fetched from the server

                <p>if you wish, you can also eliminate viewstate sent from the client side, by raising an event manually, as shown in our 
                   <a href="JavaScript.aspx">javascript example</a>, and remove the '__VIEWSTATE' parameter in your <em>'onbefore'</em>
                   callback.&nbsp;&nbsp;by careful manipulation of the viewstate, you can easily make your bandwidth usage become an order 
                   of magnitude less than if you simply keep all defaults, though to do this, you <strong>must understand the consequences 
                   of your actions</strong>

                <p>as a general rule of thumb, you should never completely eliminate viewstate on your page, unless you are certain that the 
                   http request as a whole does not update or change attributes or properties on any of your widgets on your page

                <p>and as an even more general rule, you should not mess with the viewstate at all in fact, unless you are certain that 
                   you understand the consequences of your actions.&nbsp;&nbsp;these types of micro-optimizations tends to very often 
                   return to you and bite you in your back!

                <p><em>"premature optimization is the root of all evil"</em>
                   <br><br>
                   --
                   <br>
                   Donald Knuth

            </div>
        </form>
    </body>
</html>
