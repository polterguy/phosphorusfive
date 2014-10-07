<%@ Page 
    Language="C#" 
    Inherits="phosphorus.ajax.samples.HTML5"
    Codebehind="HTML5.aspx.cs" %>

<!DOCTYPE html>
<html>
    <head>
    	<title>html5 example</title>
        <link rel="stylesheet" type="text/css" href="media/main.css">
        <meta charset="utf-8">
    </head>
    <body>
        <form id="form1" runat="server" autocomplete="off">
            <div class="container">

                <h1>html5 example</h1>

                <p>this example shows you how to create an html5 video element, thx to <a href="http://www.bigbuckbunny.org/">big buck bunny</a>

                <p><pf:Literal
                    runat="server"
                    id="video"
                    Tag="video"
                    width="320"
                    onclick="video_onclick"
                    controls>
                        <source src="http://download.blender.org/peach/trailer/trailer_1080p.ogg" type="video/ogg">
                        your browser sucks!
                   </pf:Literal>

                <p>ps, try to click the video element above

                <p>above you can see an example of how to create the html markup you wish.&nbsp;&nbsp;the attributes <em>"width"</em> and 
                   <em>"controls"</em> are not properties that exists in the literal widget class.&nbsp;&nbsp;still they are automatically 
                   parsed, and added to the markup, automatically for you, by the engine

                <p>with phosphorus.ajax, you can add any attributes you wish for your html elements, to create any html element, both in 
                   existence today, and in future versions of the html standard

                <p>onwards to the <a href="Attributes.aspx">attributes example</a>

            </div>
        </form>
    </body>
</html>
