/*
 * phosphorus five, copyright 2014 - thomas@magixilluminate.com
 * phosphorus five is licensed as mit, see the enclosed license.txt file for details
 */

using System;
using System.Web.UI;
using System.Collections.Generic;
using phosphorus.types;
using pf = phosphorus.ajax.widgets;
using phosphorus.ajax.core.filters;

[assembly: WebResource("phosphorus.ajax.javascript.manager.js", "application/javascript")]

namespace phosphorus.ajax.core
{
    /// <summary>
    /// manages an IAjaxPage by providing services to the page, and helps render the page correctly, by 
    /// adding a filter stream in the filtering chain that renders the page according to how it is supposed to be rendered, 
    /// depending upon what type of request the http request is
    /// </summary>
    public class Manager
    {
        /// <summary>
        /// contains all changes that needs to be serialized back to client
        /// </summary>
        private Node _changes;

        /// <summary>
        /// initializes a new instance of the <see cref="phosphorus.ajax.core.Manager"/> class
        /// </summary>
        /// <param name="page">the page the manager is managing</param>
        public Manager (Page page)
        {
            Page = page;
            JavaScriptFiles = new List<string> ();
            _changes = new Node ();

            // determining if we should create an ajax filter or a normal html filter for rendering the response
            if (IsPhosphorusRequest) {
                // rendering ajax json back to the client
                Page.Response.Filter = new JsonFilter (this);
            } else {
                // rendering plain html back to client
                Page.Response.Filter = new HtmlFilter (this);
            }

            // including main javascript file
            string coreScriptFileUrl = Page.ClientScript.GetWebResourceUrl (typeof(Manager), "phosphorus.ajax.javascript.manager.js");
            AddJavaScriptFile (coreScriptFileUrl);
        }

        /// <summary>
        /// returns the page for this instance
        /// </summary>
        /// <value>the page</value>
        public Page Page {
            get;
            private set;
        }

        /// <summary>
        /// returns true if this request is an ajax request
        /// </summary>
        /// <value><c>true</c> if this instance is an ajax request; otherwise, <c>false</c></value>
        public bool IsPhosphorusRequest {
            get { return Page.Request.Params ["__pf_ajax"] == "1"; }
        }

        /// <summary>
        /// adds a javascript file to include for the current response
        /// </summary>
        /// <param name="file">file</param>
        public void AddJavaScriptFile (string file)
        {
            // making sure we only include each javascript file once
            if (!JavaScriptFiles.Exists (
                delegate (string idxFile) {
                return idxFile.Equals (file, StringComparison.InvariantCultureIgnoreCase);
            }))
                JavaScriptFiles.Add (file);
        }

        /// <summary>
        /// registers widget as a changed widget that needs to send updates back to client
        /// </summary>
        /// <param name="id">id of widget to register</param>
        /// <param name="attributes">attributes to register for sending back to client</param>
        internal void RegisterWidgetChanges (Node node)
        {
            _changes ["wdg"] [node.Name] = node;
        }

        public void SendObject (string id, object value)
        {
            _changes [id].Value = value;
        }

        /// <summary>
        /// returns the changes for this request
        /// </summary>
        /// <value>The changes.</value>
        internal Node Changes {
            get { return _changes; }
        }

        /// <summary>
        /// returns the javascript files to include for the current response
        /// </summary>
        /// <value>the javascript files</value>
        internal List<string> JavaScriptFiles {
            get;
            private set;
        }
    }
}
