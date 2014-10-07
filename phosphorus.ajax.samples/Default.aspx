<%@ Page 
    Language="C#" 
    Inherits="phosphorus.ajax.samples.Default"
    Codebehind="Default.aspx.cs" %>

<!DOCTYPE html>
<html>
    <head>
    	<title>phosphorus.ajax samples</title>
        <link rel="stylesheet" type="text/css" href="media/main.css">
        <meta charset="utf-8">
    </head>
    <body>
        <form id="form1" runat="server" autocomplete="off">
            <div class="container">

                <h1>phosphorus.ajax examples</h1>

                <p>the examples for phosphorus.ajax are created such that they are both a learning path for you, in addition to being 
                   a reference guide.&nbsp;&nbsp;you can either read them as a <em>book</em>, when learning phosphorus.ajax.&nbsp;&nbsp;
                   or you can use them as a reference guide, when you are stuck with a specific problem.&nbsp;&nbsp;if you're new to 
                   phosphorus, we suggest you start with the first example; the <a href="Literal.aspx">literal example</a>

                <ul>
                    <li><a href="Literal.aspx">literal example</a>
                    <li><a href="Container.aspx">container example</a>
                    <li><a href="FormData.aspx">form data example</a>
                    <li><a href="Events.aspx">events example</a>
                    <li><a href="JavaScript.aspx">javascript example</a>
                    <li><a href="HTML5.aspx">html5 example</a>
                    <li><a href="Attributes.aspx">attributes example</a>
                    <li><a href="ViewState.aspx">viewstate example</a>
                    <li><a href="WebMethods.aspx">web methods example</a>
                </ul>

                <h2>hello world</h2>

                <p>
                    <pf:Literal
                        runat="server"
                        id="hello"
                        Tag="strong"
                        onclick="hello_onclick">click me</pf:Literal> to see the hello world example

                <p>the above simplest example you can possibly create with phosphorus.ajax features the literal control, with one event handler, 
                   setting the value of the widget to <em>"hello world"</em> when clicked.&nbsp;&nbsp;as you can see from the codebehind, the value 
                   is set on the server using c#, and automatically sent back to the client using json.&nbsp;&nbsp;the event handler is mapped 
                   with the <em>"onclick"</em> property, and the tag is set through the <em>"Tag"</em> property in the aspx markup for the control.
                   &nbsp;&nbsp;for security issues, and for clarity, phosphorus.ajax always forces you to mark your event handlers and methods you 
                   wish to invoke from javascript on the client side with the attribute <em>"WebMethod"</em>, as you can see in the codebehind of 
                   this example

                <p>if you look at the html markup of the page, you will notice that the markup is clean, and follows all html best practices, such 
                   as having all script inclusions at the bottom of the markup, no inline javascript in the html, and so on.&nbsp;&nbsp;in addition, 
                   the page, and all its content, is extremely lightweight, and there is no unnecessary overhead taking place in the page.&nbsp;&nbsp;
                   the single javascript file included is also literally tiny in size.&nbsp;&nbsp;if you inspect the http request being sent when 
                   clicking the widget above, you will also see that the request and response created is as small as possible to imagine

                <p>if you look at the code for this example, you will notice that it is intuitive and easy to understand, and contains no security 
                   issues, such as unnecessary javascript code, further opening up the attack surface of your solution

                <h2>our 5 pillars</h2>

                <ul>
                    <li>secure
                    <li>lightweight
                    <li>beautiful
                    <li>flexible
                    <li>intuitive
                </ul>

                <p>the above pillars are the foundation of phosphorus.ajax, and never compromised, regardless of the arguments.&nbsp;&nbsp;if you 
                   wish to verify our claims, you can inspect the html for phosphorus.ajax, and inspect the http requests being sent towards the 
                   server, and compare the results with other ajax libraries out there.&nbsp;&nbsp;the idea of phosphorus.ajax is to facilitate 
                   for you being able to create websites outlasting the pyramids

                <h3>1. secure</h3>

                <p>javascript and the web is inheritingly insecure by design, however, phosphorus.ajax seeks to fix that, by being secure by 
                   default.&nbsp;&nbsp;this means that you must explicitly choose to fuck up things in order to create insecure solutions

                <p><em>"with phosphorus.ajax, you're safe by default"</em>

                <h3>2. lightweight</h3>

                <p>phosphorus.ajax consumes as little bandwidth and resources as possible.&nbsp;&nbsp;the javascript is tiny, the html the same, 
                   and the server side bindings consumes as little resources as possible.&nbsp;&nbsp;unless you explicitly fuck up things here, 
                   you're lightweight by default

                <p><em>"with phosphorus.ajax, you're lightweight by default"</em>

                <h3>3. beautiful</h3>

                <p>with beauty, we don't mean falshy colors and the latest animations.don't get us wrong, you can easily create this type 
                   of beauty yourself, but with beauty, we mean that phosphorus.ajax creates beautiful code, beautiful markup and consists 
                   of beautiful classes on the server side, and beautiful javascript on the client side.&nbsp;&nbsp;unless you explicitly 
                   fuck up things in regards to this, your code will be beautiful and easy to maintain by default

                <p><em>"with phosphorus.ajax, you are beautiful by default"</em>

                <h3>4. flexible</h3>

                <p>phosphorus.ajax is easy to extend.&nbsp;&nbsp;you can extend the server side classes, you can extend the javascript, and 
                   you can modify the existing widgets to create the exact html markup you wish.&nbsp;&nbsp;there's a lot off fuss about html5 
                   these days.&nbsp;&nbsp;phosphorus.ajax already supports html version 15, even though we have no idea of how it looks like.
                   &nbsp;&nbsp;in addition, phosphorus also obviously 100% supports html5

                <p><em>"with phosphorus.ajax, your creativity is your only limit"</em>

                <h3>5. intuitive</h3>

                <p>phosphorus.ajax is intuitive to use, extend and modify.&nbsp;&nbsp;both on the client side and on the server side.&nbsp;&nbsp;
                   you can easily start using phosphorus.ajax today, and be fluent in it before supper

                <p><em>"with phosphorus.ajax, you already know everything you need to know"</em>
            </div>
        </form>
    </body>
</html>
