/*
 * phosphorus five, copyright 2014 - Mother Earth, Jannah, Gaia
 * phosphorus five is licensed as mitx11, see the enclosed LICENSE file for details
 */

namespace phosphorus.five.applicationpool
{
    using System;
    using System.Web;
    using System.Web.UI;
    using System.Configuration;
    using System.Collections.Generic;
    using phosphorus.core;
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
         * contains all ajax events, and their associated pf.lambda code, for all controls on page
         */
        private Dictionary<string, Dictionary<string, List<Node>>> AjaxEvents {
            get {
                if (ViewState ["AjaxEvents"] == null)
                    ViewState ["AjaxEvents"] = new Dictionary<string, Dictionary<string, List<Node>>> ();
                return ViewState ["AjaxEvents"] as Dictionary<string, Dictionary<string, List<Node>>>;
            }
        }

        protected override void OnInit (EventArgs e)
        {
            // creating application context
            _context = Loader.Instance.CreateApplicationContext ();

            // registering "this" web page as listener object
            _context.RegisterListeningObject (this);

            if (!IsPostBack) {
                Load += delegate {

                    // retrieving "form name", and passing it into [pf.load-form], which for web is the local path and name of webpage requested
                    // minus ".aspx" parts, in lower characters
                    string formName = Request.Url.LocalPath.TrimStart ('/').ToLower ();
                    formName = formName.Substring (0, formName.LastIndexOf ("."));

                    // raising our [pf.form-load] Active Event
                    Node args = new Node ();
                    args.Add (new Node ("_form", formName));

                    // checking to see if we have GET parameters we should pass in
                    if (!string.IsNullOrEmpty (Request.Url.Query)) {

                        // retrieving all GET parameters and passing in as [_args]
                        args.Add (new Node ("_args"));
                        string[] getArgs = Request.Url.Query.TrimStart ('?').Split ('&');
                        foreach (string idxArg in getArgs) {
                            string[] idxArgEntities = idxArg.Split ('=');
                            args [1].Add (new Node (idxArgEntities [0], idxArgEntities [1]));
                        }
                    }
                    _context.Raise ("pf.load-ui", args);
                };
            }
            base.OnInit (e);
        }

        /// <summary>
        /// creates a web form specified through its children nodes
        /// </summary>
        /// <param name="context"><see cref="phosphorus.Core.ApplicationContext"/> for Active Event</param>
        /// <param name="e">parameters passed into Active Event</param>
        [ActiveEvent (Name = "pf.create-form")]
        private void pf_create_form (ApplicationContext context, ActiveEventArgs e)
        {
            // finding parent widget first, which defaults to "container" widget, if no widget is given
            Container parent = container;
            foreach (Node idxNode in e.Args.Children) {
                if (idxNode.Name == "parent") {
                    parent = FindContainer (idxNode.Get<string> (), Page);
                }
            }

            // creating, and attaching widget, to parent control's collection
            parent.Controls.Add (CreateForm (context, e.Args.Clone (), parent));
        }

        [ActiveEvent (Name = "pf.add-widget-event")]
        private void pf_add_widget_event (ApplicationContext context, ActiveEventArgs e)
        {
            string widgetId = e.Args.Get<string> ();
            if (!AjaxEvents.ContainsKey (widgetId))
                AjaxEvents [widgetId] = new Dictionary<string, List<Node>> ();
            string eventName = e.Args [0].Name;
            if (!AjaxEvents [widgetId].ContainsKey (eventName))
                AjaxEvents [widgetId] [eventName] = new List<Node> ();
            AjaxEvents [widgetId] [eventName].Add (e.Args [0].Clone ());
        }

        /*
         * recursively searches through page for Container with specified id, starting from "idx"
         */
        public Container FindContainer (string id, Control idx)
        {
            if (idx.ID == id)
                return idx as Container;
            foreach (Control idxChild in idx.Controls) {
                Container tmpRet = FindContainer (id, idxChild);
                if (tmpRet != null)
                    return tmpRet;
            }
            return null;
        }

        /*
         * creates widget according to node given, and returns to caller
         */
        private Widget CreateForm (ApplicationContext context, Node node, Container parent)
        {
            node.Insert (0, new Node ("__parent", parent));
            return context.Raise ("pf.web.widgets.panel", node).Get<Widget> ();
        }

        [WebMethod]
        protected void common_event_handler (pf.Literal sender, Widget.AjaxEventArgs e)
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

