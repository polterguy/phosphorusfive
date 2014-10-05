<%@ Page 
    Language="C#" 
    Inherits="phosphorus.ajax.samples.HTML5"
    Codebehind="HTML5.aspx.cs" %>

<!DOCTYPE html>
<html>
    <head>
    	<title>html5 sample</title>
        <link rel="stylesheet" type="text/css" href="media/main.css" />
    </head>
    <body>
        <form id="form1" runat="server" autocomplete="off">
            <div class="container">
                <h1>html5 sample</h1>
                <p>
                    this sample shows you how to create an html5 video element, thx to <a href="http://www.bigbuckbunny.org/">big buck bunny</a>
                </p>
                <pf:Literal
                    runat="server"
                    id="video"
                    Tag="video"
                    width="320"
                    ondblclick="video_ondblclick"
                    controls>
                    <source src="http://download.blender.org/peach/trailer/trailer_1080p.ogg" type="video/ogg" />
                    your browser sucks!
                </pf:Literal>
                <p>
                    ps, try to double click the video element above
                </p>
            </div>
        </form>
    </body>
</html>
