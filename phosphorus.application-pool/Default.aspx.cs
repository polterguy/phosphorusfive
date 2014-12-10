/*
 * phosphorus five, copyright 2014 - Mother Earth, Jannah, Gaia
 * phosphorus five is licensed as mitx11, see the enclosed LICENSE file for details
 */

namespace phosphorus.five.applicationpool
{
    using System;
    using System.Configuration;
    using System.Web;
    using System.Web.UI;
    using phosphorus.core;
    using phosphorus.ajax.core;
    using phosphorus.ajax.widgets;
    using pf = phosphorus.ajax.widgets;

    public partial class Default : AjaxPage
    {
        // Application Context for page life cycle
        private ApplicationContext _context;

        protected override void OnInit (EventArgs e)
        {
            // creating application context
            _context = Loader.Instance.CreateApplicationContext ();

            // registering "this" web page as listener object
            _context.RegisterListeningObject (this);

            if (!IsPostBack) {
                Init += delegate {

                    // TODO: pass in http GET parameters
                    // retrieving "form name", and passing it into [pf.load-form], which for web is the local path and name of webpage requested
                    // minus ".aspx" parts, in lower characters
                    string formName = Request.Url.LocalPath.TrimStart ('/').ToLower ();
                    formName = formName.Substring (0, formName.LastIndexOf ("."));

                    // raising our [pf.form-load] Active Event
                    _context.Raise ("pf.load-form", new Node (string.Empty, formName));
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
            Form.Controls.Add (CreateWidget (e.Args));
        }

        /*
         * creates widget according to node given, and returns to caller
         */
        private Widget CreateWidget (Node node)
        {
            Widget widget = InstantiateWidget (node);
            foreach (Node idxArg in node.Children) {
                switch (idxArg.Name) {
                case "ID":
                    widget.ID = idxArg.Get<string> ();
                    break;
                case "ElementType":
                    widget.ElementType = idxArg.Get<string> ();
                    break;
                case "InvisibleElement":
                    widget.InvisibleElement = idxArg.Get<string> ();
                    break;
                case "RenderType":
                    widget.RenderType = (Widget.RenderingType)Enum.Parse (typeof(Widget.RenderingType), idxArg.Get<string> ());
                    break;
                case "Controls":
                    foreach (var idxChild in idxArg.Children) {
                        widget.Controls.Add (CreateWidget (idxChild));
                    }
                    break;
                default:
                    widget [idxArg.Name] = idxArg.Get<string> ();
                    break;
                }
            }
            return widget;
        }

        /*
         * instantiates widget
         */
        private Widget InstantiateWidget (Node node)
        {
            switch (node.Name) {
            case "Literal":
                return new Literal ();
            case "Void":
                return new phosphorus.ajax.widgets.Void ();
            default:
                return new Container ();
            }
        }
    }
}

