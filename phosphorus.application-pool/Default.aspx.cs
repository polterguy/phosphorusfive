/*
 * phosphorus five, copyright 2014 - Mother Earth, Jannah, Gaia
 * phosphorus five is licensed as mitx11, see the enclosed LICENSE file for details
 */

namespace phosphorus.five.applicationpool
{
    using System;
    using System.Web;
    using System.Text;
    using System.Web.UI;
    using System.Configuration;
    using System.Collections.Generic;
    using phosphorus.core;
    using phosphorus.lambda;
    using phosphorus.ajax.core;
    using phosphorus.ajax.widgets;
    using pf = phosphorus.ajax.widgets;

    public partial class Default : AjaxPage
    {
        // Application Context for page life cycle
        private ApplicationContext _context;

        // main container for all widgets
        protected Container container;

        /*
         * contains all ajax events, and their associated pf.lambda code, for all controls on page. please notice that
         * when we do this like this, we have to store the ViewState into the Session object, which we're doign automatically in
         * OnInit, since otherwise the server-side functionality will follow the page to the client, and allow for the client side
         * to tamper with the server-side functionality, and modify server-side methods or Active Events
         */
        private Dictionary<string, Dictionary<string, List<Node>>> AjaxEvents {
            get {
                if (ViewState ["AjaxEvents"] == null)
                    ViewState ["AjaxEvents"] = new Dictionary<string, Dictionary<string, List<Node>>> ();
                return ViewState ["AjaxEvents"] as Dictionary<string, Dictionary<string, List<Node>>>;
            }
        }

        /*
         * overridden to create context, and do other initialization, such as mapping up our Page_Load event,
         * but only for the initial loading of our page
         */
        protected override void OnInit (EventArgs e)
        {
            // creating application context
            _context = Loader.Instance.CreateApplicationContext ();

            // registering "this" web page as listener object
            _context.RegisterListeningObject (this);

            // rewriting path to what was actually requested, such that HTML form element doesn't become garbage ...
            // this ensures that our HTML form element stays correct. basically "undoing" what was done in Global.asax.cs
            // in addition, when retrieving request URL later, we get the "correct" request URL, and not the URL to "Default.aspx"
            HttpContext.Current.RewritePath (HttpContext.Current.Items ["__pf_original_url"] as string);

            // mapping up our Page_Load event for initial loading of web page
            if (!IsPostBack)
                Load += Page_LoadInitialLoading;

            // call base
            base.OnInit (e);
        }

        /*
         * invoked for the first loading of web page to make sure we load up our UI, passing in any arguments
         */
        private void Page_LoadInitialLoading (object sender, EventArgs e)
        {
            // raising our [pf.form-ui] Active Event, creating the node to pass in first
            // where the [_form] node becomes the name of thr form requested
            Node args = new Node ();
            args.Add (new Node ("_form", HttpContext.Current.Items ["__pf_original_url"] as string));

            // making sure we pass in any HTTP GET parameters
            if (Request.QueryString.Keys.Count > 0) {

                // retrieving all GET parameters and passing in as [_args]
                args.Add (new Node ("_args"));
                foreach (var idxArg in Request.QueryString.AllKeys) {
                    args [1].Add (new Node (idxArg, Request.QueryString [idxArg]));
                }
            }

            // invoking the Active Event that actually loads our UI, now with a [_file] node, and possibly an [_args] node
            _context.Raise ("pf.load-ui", args);
        }

        /// <summary>
        /// creates a web form specified through its children nodes
        /// </summary>
        /// <param name="context"><see cref="phosphorus.Core.ApplicationContext"/> for Active Event</param>
        /// <param name="e">parameters passed into Active Event</param>
        [ActiveEvent (Name = "pf.create-widget")]
        private void pf_create_widget (ApplicationContext context, ActiveEventArgs e)
        {
            // finding parent widget first, which defaults to "container" widget, if no widget is given
            Node parentNode = e.Args.Find (
                delegate (Node idx) {
                    return idx.Name == "parent";
            });
            Container parent = parentNode != null ? FindControl<Container> (parentNode.Get<string> (), Page) : container;

            // creating widget
            CreateForm (context, e.Args.Clone (), parent);
        }
        
        /// <summary>
        /// reloads the current URL
        /// </summary>
        /// <param name="context"><see cref="phosphorus.Core.ApplicationContext"/> for Active Event</param>
        /// <param name="e">parameters passed into Active Event</param>
        [ActiveEvent (Name = "pf.reload-location")]
        private void pf_reload_location (ApplicationContext context, ActiveEventArgs e)
        {
            Manager.SendJavaScriptToClient ("location.reload();");
        }

        /// <summary>
        /// sends the given JavaScript to the client. JavaScript is given as value of [pf.send-javascript], and can
        /// be a constant, an expression or a formatting expression
        /// </summary>
        /// <param name="context"><see cref="phosphorus.Core.ApplicationContext"/> for Active Event</param>
        /// <param name="e">parameters passed into Active Event</param>
        [ActiveEvent (Name = "pf.send-javascript")]
        private void pf_send_javascript (ApplicationContext context, ActiveEventArgs e)
        {
            string js = e.Args.Get<string> ();
            if (Expression.IsExpression (js)) {
                var match = Expression.Create (js).Evaluate (e.Args);
                if (match.TypeOfMatch != Match.MatchType.Value)
                    throw new ArgumentException ("[pf.send-javascript] can only take expressions of type 'value'");

                StringBuilder builder = new StringBuilder ();
                foreach (Node idx in match.Matches) {
                    builder.Append (idx.Value);
                }
                js = builder.ToString ();
            } else if (e.Args.Count > 0) {
                js = Expression.FormatNode (e.Args);
            }
            Manager.SendJavaScriptToClient (js);
        }
        
        /// <summary>
        /// send the given string back to browser as JSON
        /// </summary>
        /// <param name="context"><see cref="phosphorus.Core.ApplicationContext"/> for Active Event</param>
        /// <param name="e">parameters passed into Active Event</param>
        [ActiveEvent (Name = "pf.send-string")]
        private void pf_send_string (ApplicationContext context, ActiveEventArgs e)
        {
            string key = e.Args.Get<string> ();
            string str = e.Args [0].Get<string> ();
            if (Expression.IsExpression (str)) {
                var match = Expression.Create (str).Evaluate (e.Args [0]);
                if (match.TypeOfMatch != Match.MatchType.Value)
                    throw new ArgumentException ("cannot use anything but a 'value' expression in [pf.send-string]");
                StringBuilder builder = new StringBuilder ();
                foreach (Node idx in match.Matches) {
                    builder.Append (idx.Get<string> ());
                }
                str = builder.ToString ();
            } else if (e.Args [0].Count > 0) {
                str = Expression.FormatNode (e.Args [0]);
            }
            Manager.SendObject (key, str);
        }

        /// <summary>
        /// clear the given widget, by removing all its child controls
        /// </summary>
        /// <param name="context"><see cref="phosphorus.Core.ApplicationContext"/> for Active Event</param>
        /// <param name="e">parameters passed into Active Event</param>
        [ActiveEvent (Name = "pf.clear-widget")]
        private void pf_clear_widget (ApplicationContext context, ActiveEventArgs e)
        {
            Container ctrl = FindControl<Container> (e.Args.Get<string> (), Page);
            ctrl.Controls.Clear ();
            ctrl.ReRenderChildren ();
        }

        /// <summary>
        /// deletes the widget with the id of the value of the [pf.delete-widget]
        /// </summary>
        /// <param name="context"><see cref="phosphorus.Core.ApplicationContext"/> for Active Event</param>
        /// <param name="e">parameters passed into Active Event</param>
        [ActiveEvent (Name = "pf.remove-widget")]
        private void pf_remove_widget (ApplicationContext context, ActiveEventArgs e)
        {
            // finding widget to remove
            Widget widget = FindControl<Widget> (e.Args.Get<string> (), Page);

            // removing all event handlers for widget
            RemoveEvents (widget);

            // actually removing widget from Page control collection, and persisting our change
            Container parent = widget.Parent as Container;
            parent.RemoveControlPersistent (widget);
        }

        /// <summary>
        /// creates an ajax event containing pf.lambda code for the given widget's event
        /// </summary>
        /// <param name="context"><see cref="phosphorus.Core.ApplicationContext"/> for Active Event</param>
        /// <param name="e">parameters passed into Active Event</param>
        [ActiveEvent (Name = "pf.web.add-widget-event")]
        private void pf_web_add_widget_event (ApplicationContext context, ActiveEventArgs e)
        {
            // retrieving widget id, and creating an event collection for the given widget
            Widget widget = e.Args.Get<Widget> ();
            if (!AjaxEvents.ContainsKey (widget.ID))
                AjaxEvents [widget.ID] = new Dictionary<string, List<Node>> ();

            // creating an event collection for the given event for the given widget. notice that one widget might
            // create multiple pf.lambda objects for the same event, meaning one widget might have several ajax event handlers
            // for the same ajax event
            string eventName = e.Args [0].Name;
            if (!AjaxEvents [widget.ID].ContainsKey (eventName))
                AjaxEvents [widget.ID] [eventName] = new List<Node> ();

            // appending our pf.lambda object to the list of pf.lambda objects for the given widget's given event
            AjaxEvents [widget.ID] [eventName].Add (e.Args [0].Clone ());

            // mapping the widget's ajax event to our common event handler on page
            widget [eventName] = "common_event_handler";
        }
        
        /// <summary>
        /// returns the control with the given ID as first child of args, from optionally [parent] control's ID given
        /// </summary>
        /// <param name="context"><see cref="phosphorus.Core.ApplicationContext"/> for Active Event</param>
        /// <param name="e">parameters passed into Active Event</param>
        [ActiveEvent (Name = "pf.find-control")]
        private void pf_find_control (ApplicationContext context, ActiveEventArgs e)
        {
            // defaulting parent to page object, but checking to see if an explicit parent is given through e.Args
            Control parentCtrl = Page;
            Node parentNode = e.Args.Find (
                delegate (Node idx) {
                    return idx.Name == "parent";
            });
            if (parentNode != null)
                parentCtrl = FindControl<Control> (parentNode.Get<string> (), Page);

            // returning control as first child of e.Args
            e.Args.Insert (0, new Node (string.Empty, FindControl<Control> (e.Args.Get<string> (), parentCtrl)));
        }

        /// <summary>
        /// includes a JavaScript file on the client side
        /// </summary>
        /// <param name="context"><see cref="phosphorus.Core.ApplicationContext"/> for Active Event</param>
        /// <param name="e">parameters passed into Active Event</param>
        [ActiveEvent (Name = "pf.insert-javascript-file")]
        private void pf_insert_javascript_file (ApplicationContext context, ActiveEventArgs e)
        {
            string file = e.Args.Get<string> ();
            RegisterJavaScriptFile (file);
        }

        /// <summary>
        /// includes a stylesheet file on the client side
        /// </summary>
        /// <param name="context"><see cref="phosphorus.Core.ApplicationContext"/> for Active Event</param>
        /// <param name="e">parameters passed into Active Event</param>
        [ActiveEvent (Name = "pf.insert-stylesheet-file")]
        private void pf_insert_stylesheet_file (ApplicationContext context, ActiveEventArgs e)
        {
            string file = e.Args.Get<string> ();
            RegisterStylesheetFile (file);
        }

        /*
         * recursively searches through page for Container with specified id, starting from "idx"
         */
        private T FindControl<T> (string id, Control idx) where T : Control
        {
            if (idx.ID == id)
                return idx as T;
            foreach (Control idxChild in idx.Controls) {
                T tmpRet = FindControl<T> (id, idxChild);
                if (tmpRet != null)
                    return tmpRet;
            }
            return null;
        }

        /*
         * creates widget according to node given, and returns to caller
         */
        private void CreateForm (ApplicationContext context, Node node, Container parent)
        {
            node.Insert (0, new Node ("__parent", parent));
            node.Insert (1, new Node ("_form-id", node.Value));
            context.Raise ("pf.web.widgets.container", node);
        }
        
        /*
         * recursively removes all events for control and all of its children controls
         */
        private void RemoveEvents (Control idx)
        {
            // removing all ajax events belonging to widget
            if (AjaxEvents.ContainsKey (idx.ID)) {
                AjaxEvents.Remove (idx.ID);
            }

            // recursively removing all ajax events for all of control's children controls
            foreach (Control idxChild in idx.Controls) {
                RemoveEvents (idxChild);
            }
        }

        /*
         * common ajax event handler for all widget's events on page
         */
        [WebMethod]
        protected void common_event_handler (pf.Widget sender, Widget.AjaxEventArgs e)
        {
            string id = sender.ID;
            string eventName = e.Name;
            List<Node> lambdas = AjaxEvents [id] [eventName];
            foreach (Node idxLambda in lambdas) {
                _context.Raise ("lambda", idxLambda.Clone ());
            }
        }
    }
}

