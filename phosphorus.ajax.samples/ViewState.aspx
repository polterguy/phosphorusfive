<%@ Page 
    Language="C#" 
    Inherits="phosphorus.ajax.samples.ViewState"
    Codebehind="ViewState.aspx.cs" %>

<!DOCTYPE html>
<html>
    <head>
    	<title>viewstate example</title>
        <link rel="stylesheet" type="text/css" href="media/main.css" />
        <style>
.green {
    background-color:LightGreen;
}
        </style>
    </head>
    <body>
        <form id="form1" runat="server" autocomplete="off">
            <div class="container">
                <h1>viewstate example</h1>
                <p>
                    this example shows you how phosphorus.ajax handles viewstate
                </p>
                <pf:Literal
                    runat="server"
                    id="content" 
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
                        style="display:block;clear:both;float:left;width:20px;"
                        type="checkbox" />
                    <label for="viewState" style="width:120px;float:left;display:block;">viewstate</label>
                    <pf:Literal
                        runat="server"
                        id="submit"
                        Tag="input"
                        type="button" 
                        style="display:block;float:left;width:200px;"
                        value="submit new text"
                        onclick="submit_onclick" />
                    <pf:Literal
                        runat="server"
                        id="changeClass"
                        Tag="input"
                        type="button" 
                        style="display:block;float:left;width:200px;"
                        value="change class"
                        onclick="changeClass_onclick" />
                    <pf:Literal
                        runat="server"
                        id="makeInVisible"
                        Tag="input"
                        type="button" 
                        style="display:block;float:left;width:200px;"
                        value="make invisible"
                        onclick="makeInVisible_onclick" />
                    <pf:Literal
                        runat="server"
                        id="makeVisible"
                        Tag="input"
                        type="button" 
                        style="display:block;float:left;width:200px;"
                        value="make visible"
                        onclick="makeVisible_onclick" />
                </p>
                <p>
                    since we're not tracking viewstate for the paragraph literal control, the control will not be able to remember 
                    its content when it is set to invisible though.&nbsp;&nbsp;try to change its content through the textarea, and its 
                    class, for then to make it invisible and then again visible.&nbsp;&nbsp;both with viewstate turned on, and with 
                    viewstate turned off, and see the difference.&nbsp;&nbsp;this is the disadvantage you add when you turn off viewstate
                </p>
                <p>
                    if you turn off viewstate, and debug this page, and set a breakpoint on the server side, you will also see that none 
                    of the attributes and properties of the textarea and your paragraph are being remembered across http requests.&nbsp;&nbsp;
                    have this in mind before you turn off viewstate on your controls.&nbsp;&nbsp;without viewstate, your controls are no 
                    longer remembering any of their state, except whether or not they're visible or not
                </p>
                <p>
                    by enabling viewstate on your widgets, your http traffic can easily grow by several orders of magnitude.&nbsp;&nbsp;it is 
                    important that you think carefully about which widgets you wish to enable viewstate for, and which you wish to disable 
                    it for.&nbsp;&nbsp;this is not unique for phosphorus.ajax.&nbsp;&nbsp;but what is unique though, is its beautiful 
                    handling of viewstate, allowing you to turn it off, while still functioning perfectly, as it should do
                </p>
                <p>
                    another thing that is unique for phosphorus.ajax, is its ability to incrementally update all values of its widgets, both 
                    the form elements, viewstate, and all other widgets on your page.&nbsp;&nbsp;if you for instance add one character to 
                    the textarea above, and click cubmit, then only that single character you added will be returned from the server, and 
                    be added to your paragraph widget, instead of having to send the entire text back from the server.&nbsp;&nbsp;if you inspect 
                    the http request using your browser, you can see this for yourself
                </p>
                <p>
                    the above feature, combined with the ability to turn off viewstate on individual controls, should easily allow you to 
                    save at least 50% of the bandwidth usage, compared to other ajax libraries that does not have these features
                </p>
            </div>
        </form>
    </body>
</html>
