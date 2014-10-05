/*
 * phosphorus five, copyright 2014 - thomas@magixilluminate.com
 * phosphorus five is licensed as mit, see the enclosed license.txt file for details
 */

using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Web.Script.Serialization;
using phosphorus.types;
using phosphorus.ajax.core;
using pf = phosphorus.ajax.widgets;

namespace phosphorus.ajax.core.filters
{
    /// <summary>
    /// the http response filter for rendering json back to client in ajax requests
    /// </summary>
    public class JsonFilter : Filter
    {
        /// <summary>
        /// initializes a new instance of the <see cref="phosphorus.ajax.core.filters.JsonFilter"/> class
        /// </summary>
        /// <param name="manager">the manager this instance is rendering for</param>
        public JsonFilter (Manager manager)
            : base (manager)
        { }

        /// <summary>
        /// renders the response
        /// </summary>
        /// <returns>the response returned back to client</returns>
        protected override string RenderResponse ()
        {
            Manager.Page.Response.Headers ["Content-Type"] = "application/json";

            TextReader reader = new StreamReader (this, Manager.Page.Response.ContentEncoding);
            string content = reader.ReadToEnd ();

            // registering viewstate for change
            string viewstate = GetViewState (content);
            string oldViewstate = Manager.Page.Request.Params ["__VIEWSTATE"];
            if (viewstate != oldViewstate) {
                Node node = new Node ("__VIEWSTATE");
                Node viewStateNode = new Node ();
                viewStateNode.Name = oldViewstate;
                viewStateNode.Value = viewstate;
                node ["value"].Value = viewStateNode;
                Manager.RegisterWidgetChanges (node);
            }

            // we could just use the JavaScriptSerializer here, but that would create a significant larger json for us
            // hence we're rolling our own implementation here, with some help from the JavaScriptSerializer class
            return ToJson (Manager.Changes);
        }

        /// <summary>
        /// creates a valid client side json string from the node given
        /// </summary>
        /// <returns>json</returns>
        /// <param name="node">node to serialize</param>
        private string ToJson (Node node)
        {
            StringBuilder builder = new StringBuilder ();
            builder.Append ("{");
            List<Node> nodes = node.Get<List<Node>> ();
            if (nodes != null && nodes.Count > 0) {
                bool isFirst = true;
                foreach (Node idx in nodes) {
                    if (!string.IsNullOrEmpty (idx.Name)) {
                        if (isFirst) {
                            isFirst = false;
                        } else {
                            builder.Append (",");
                        }
                        builder.Append (string.Format (@"""{0}"":", idx.Name));
                        ObjectToJson (builder, idx.Value);
                    }
                }
            }
            builder.Append ("}");
            return builder.ToString ();
        }

        /// <summary>
        /// creates a json value of the given value
        /// </summary>
        /// <param name="builder">builder to render json into</param>
        /// <param name="value">what to render</param>
        private void ObjectToJson (StringBuilder builder, object value)
        {
            if (value == null) {
                builder.Append ("null");
            } else {
                List<Node> lst = value as List<Node>;
                if (lst != null) {
                    builder.Append ("{");
                    bool isFirst = true;
                    foreach (Node idx in lst) {
                        if (isFirst) {
                            isFirst = false;
                        } else {
                            builder.Append (",");
                        }
                        builder.Append (string.Format (@"""{0}"":", idx.Name));
                        ObjectToJson (builder, idx.Value);
                    }
                    builder.Append ("}");
                } else {
                    Node single = value as Node;
                    if (single != null) {
                        new JavaScriptSerializer ().Serialize (GetPropertyChanges (single), builder);
                    } else {
                        new JavaScriptSerializer ().Serialize (value, builder);
                    }
                }
            }
        }

        /// <summary>
        /// returns a json string containing the begin and end offset of the changes to the property
        /// </summary>
        /// <returns>The property changes.</returns>
        /// <param name="single">Single.</param>
        private object GetPropertyChanges (Node single)
        {
            string old = single.Name;
            string ne = single.Get<string> ();
            if (old == null || old.Length < 10 || ne == null || ne.Length < 10) {
                return ne; // no need to reduce size
            } else {
                // finding the position of where the changes start such that we can return 
                // as small amounts of changes back to client as possible, to conserve bandwidth and make response smaller
                if (old == ne) {
                    return new object[] { };
                }
                int start = -1;
                string update = null;
                for (int idx = 0; idx < old.Length && idx < ne.Length; idx++) {
                    if (old [idx] != ne [idx]) {
                        start = idx;
                        if (idx < ne.Length) {
                            update = ne.Substring (idx);
                        }
                        break;
                    }
                }
                if (start == -1 && ne.Length > old.Length) {
                    return new object[] { old.Length, ne.Substring (old.Length) };
                } else if (start == -1 && ne.Length < old.Length) {
                    return new object[] { ne.Length };
                } else if (start < 5) {
                    return ne; // we cannot save anything here ...
                } else {
                    return new object[] { start, update };
                }
            }
        }

        /// <summary>
        /// returns viewstate from html content
        /// </summary>
        /// <returns>The view state.</returns>
        /// <param name="html">Html.</param>
        private string GetViewState (string html)
        {
            Regex regex = new Regex (@"<input[\s\n]+type=""hidden""[\s\n]+name=""__VIEWSTATE""[\s\n]+id=""__VIEWSTATE""[\s\n]+value=""(.+[^""])""[\s\n]*/>", RegexOptions.Compiled);
            Match match = regex.Match (html);
            return match.Groups[1].Value;
        }
    }
}

