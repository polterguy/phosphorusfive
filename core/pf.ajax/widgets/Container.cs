/*
 * Phosphorus.Five, copyright 2014 - 2015, Thomas Hansen, isa.lightbringer@gmail.com
 * Phosphorus.Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.UI;
using pf.ajax.core;

// ReSharper disable PossibleNullReferenceException

/// <summary>
///     Contains all the main widgets in pf.ajax.
/// 
///     The widgets that pf.ajax is composed out of, can be found in this namespace.
/// 
///     * Literal - Allows you to create widgets that contains simple text or HTML values.
///     * Container - A widget that can contain children widgets of its own.
///     * Void - Useful for creating widgets that has neither children widgets, nor any inner HTML. HTML input elements are one example.
///     * Widget - Base class with common functionality for all widgets.
/// 
///     However, since you can override any property, and add any attribute you wish, to any of the above widgets, these widgets are actually
///     enough to be able to create any HTML markup you wish. If you wish to create a container widget, that renders like an HTML address element,
///     then all you have to do, as to set the <em>"ElementType"</em> property of your widget to "address".
/// 
///     If you wish to handle the <em>"onmouseover"</em> DOM event on the server-side, then all you need to do, is to provide an "onmouseover" attribute,
///     referencing the event handler from your codebehind, and voila; You're in <em>"server-land"</em>, whenever anyone hovers their mouse over your
///     HTML element.
/// 
///     pf.ajax will automatically understand the difference between a server-side Event Handler, and a JavaScript DOM client-side
///     event. If you supply something that is not a legal C# or VB.NET method name as the value of any attribute starting with the text "on" in your
///     widgets, then pf.ajax will assume you've supplied a JavaScript DOM event handler, and simply render your widget's attribute, as a 
///     client-side JavaScript piece of code, to be executed when DOM event bubbles up.
/// 
///     Below is some sample code of how to create an Hello World application in C# using pf.ajax;
/// 
///     <strong>Default.aspx</strong>
/// 
/// <pre>&lt;\%\@ Page 
///     Language="C#" 
///     Inherits="samples._Default"
///     Codebehind="Default.aspx.cs" \%&gt;
/// &lt;!DOCTYPE html&gt;
/// &lt;html&gt;
///     &lt;head&gt;
///     &lt;/head&gt;
///     &lt;body&gt;
///         &lt;form id="form1" runat="server" autocomplete="off"&gt;
///             &lt;pf:Literal
///                 runat="server"
///                 id="hello"
///                 ElementType="h1"
///                 onclick="hello_onclick"&gt;click me for hello world&lt;/pf:Literal&gt;
///         &lt;/form&gt;
///     &lt;/body&gt;
/// &lt;/html&gt;
/// </pre>
/// 
/// <strong>Default.aspx.cs</strong>
/// 
/// <pre>
/// using System;
/// using pf.ajax.core;
/// namespace samples
/// {
///     using pf = ajax.widgets;
///     public partial class _Default : AjaxPage
///     {
///         [WebMethod]
///         protected void hello_onclick (<strong>pf.Literal</strong> sender, EventArgs e)
///         {
///             sender.innerValue = "Hello World :)";
///         }
///     }
/// }
/// </pre>
/// 
/// Notice in the above code, how the event handler takes the widget itself as the first argument. Also notice how the event handler must be
/// marked with the <em>WebMethod</em> attribute in your codebehind.
/// 
/// When using pf.ajax, the Ajax mapping functionality, and persistence of attributes, and serialization back and forth from client to server,
/// is automatically taken care of for you. pf.ajax is 100% WebControl compatible, which means you can intermix it with other Web Controls 
/// compatible libraries, and the core ASP.NET WebControls.
/// </summary>
namespace pf.ajax.widgets
{
    /// <summary>
    ///     A widget that can contains children widgets of its own.
    /// 
    ///     This is the main "container" widget in pf.ajax, which means that it can contain children controls (widgets), through its
    ///     Controls property. Also everything between the opening and end declaration of this widget in your .aspx markup will be treated as controls. 
    ///     You can also dynamically add and remove child controls to this widget by using the CreatePersistentControl method and the 
    ///     RemoveControlPersistent</see> method. If you do, then the widget will automatically remember its updated Controls collection across 
    ///     server requests.
    /// 
    ///     If you do not use neither of the above mentioned methods, then the widget behaves exactly like one of the default built-in controls
    ///     from ASP.NET, and you must take care of re-creating dynamically additions, and remove dynamical removals yourself, during HTTP requests, 
    ///     if you have changed your Controls collection.
    /// 
    ///     The closest equivalent you can find in ASP.NET to this widget, is probably the <em>"Panel"</em> ASP.NET WebControl.
    /// </summary>
    [ViewStateModeById]
    public class Container : Widget, INamingContainer
    {
        // contains all the creator objects to create our controls when needed
        // the whole purpose of this bugger is to avoid the use of reflection as much as possible, since it is slow and 
        // requires lowering security settings on server to be used
        // by storing "factory objects" like this in a dictionary with the type being the key, we avoid 
        // having to use anymore reflection than absolutely necessary
        // please notice that this dictionary is static, and hence will be reused across multiple requests and sessions
        private static readonly Dictionary<Type, ICreator> Creators = new Dictionary<Type, ICreator> ();
        private static readonly List<Tuple<string, Type>> TypeMapper = new List<Tuple<string, Type>> ();
        // used to lock GetCreator to make sure we don't get a race condition when instantiating new creators
        private static readonly object Lock = new object ();
        // contains the original controls collection, before we started adding and removing controls for current request
        private List<Control> _originalCollection;
        // overridden to supply default element
        public override string ElementType
        {
            get
            {
                if (string.IsNullOrEmpty (base.ElementType))
                    return "div";
                return base.ElementType;
            }
            set { base.ElementType = value; }
        }

        public override string this [string name]
        {
            get {
                if (name == "value" && ElementType == "select" && AllChildrenHasIds ()) {
                    // special treatment for select HTML elements, to make it resemble what goes on on the client-side
                    string retVal = "";
                    foreach (Control idxCtrl in Controls) {
                        var idxWidget = idxCtrl as Widget;
                        if (idxWidget != null) {
                            if (idxWidget.HasAttribute ("selected"))
                                retVal += idxWidget ["value"] + ",";
                        }
                    }
                    return retVal.TrimEnd (',');
                }
                return base [name];
            }
            set {
                if (name == "innerValue")
                    throw new ArgumentException ("you cannot set the 'innerValue' property of the '" + ID + "' Container widget");
                if (name == "value" && ElementType == "select" && AllChildrenHasIds ()) {
                    // special treatment for select HTML elements, to make it resemble what goes on on the client-side
                    var splits = value.Split (',');
                    foreach (Control idxCtrl in Controls) {
                        var idxWidget = idxCtrl as Widget;
                        if (idxWidget != null) {
                            idxWidget.RemoveAttribute ("selected");
                        }
                    }
                    foreach (string idxSplit in splits) {
                        foreach (Control idxCtrl in Controls) {
                            var idxWidget = idxCtrl as Widget;
                            if (idxWidget != null) {
                                if (idxWidget ["value"] == idxSplit) {
                                    idxWidget ["selected"] = null;
                                }
                            }
                        }
                    }
                    return;
                }
                base [name] = value;
            }
        }

        public override bool HasAttribute (string name)
        {
            if (name == "value" && ElementType == "select") {
                // special treatment for select HTML elements, to make it resemble what goes on on the client-side
                foreach (Control idxCtrl in Controls) {
                    var idxWidget = idxCtrl as Widget;
                    if (idxWidget != null) {
                        if (idxWidget.HasAttribute ("selected"))
                            return true;
                    }
                }
            }
            return base.HasAttribute (name);
        }

        protected override bool HasContent
        {
            get { return Controls.Count > 0; }
        }

        /// <summary>
        ///     Returns all controls of the given type T from the Controls collection.
        /// 
        ///     Useful to traverse all controls of a specific type from the Controls collection, 
        ///     such that you can traverse for instance all Literal widgets, without having to cast them in your code,
        ///     and having empty filler widgets checks, which are created automatically for you by Mono or .Net clutter your 
        ///     iteration code.
        /// </summary>
        /// <returns>All controls of type T from the Controls property.</returns>
        /// <typeparam name="T">Type of controls to retrieve.</typeparam>
        public IEnumerable<T> GetChildControls<T> () where T : Control
        {
            return from Control idx in Controls let tmp = idx as T where idx != null select tmp;
        }

        /// <summary>
        ///     Creates a persistent child control, that will be automatically re-created during future server requests.
        /// 
        ///     This method allws you to create a <em>"persistent web control"</em>, which means that the Container widget will remember
        ///     that control, and its position, and automatically re-create that control automatically for you in future server-requests.
        /// 
        ///     This feature does add a small object to your ViewState, but actually less than what you think, since instead of storing
        ///     the whole type in the ViewState, it stores an integer, being a reference to a type, and a type creator.
        /// 
        ///     You can create any Control here you wish, but your control must have a public constructor 
        ///     taking no arguments. Only controls created through this method, will be persisted, and 
        ///     automatically re-created during future server requests.
        /// 
        ///     If you wish, you can supply an onLoad EventHandler or Delegate, which will be executed during the <em>"LoadComplete"</em>
        ///     event of your Page. Which allows you to create initialization code for your widget.
        /// </summary>
        /// <returns>The persistent control.</returns>
        /// <param name="id">ID of your control. If null, and automatic id will be created and assigned.</param>
        /// <param name="index">Index of where to insert control. If -1, the control will be appended into Controls collection.</param>
        /// <param name="onLoad">Event handler callback for what to do during OnLoad. If you supply an event handler here, then your 
        /// method will be invoked during LoadComplete of your Page, allowing you to have initialization functionality for your control.</param>
        /// <typeparam name="T">The type of control you wish to create.</typeparam>
        public T CreatePersistentControl<T> (string id = null, int index = -1, EventHandler onLoad = null) where T : Control, new ()
        {
            StoreOriginalControls ();
            ReRenderChildren ();

            // creating new control, and adding to the controls collection
            var control = GetCreator<T> ().Create () as T;
            control.ID = string.IsNullOrEmpty (id) ? CreateId () : id;

            if (index == -1)
                Controls.Add (control);
            else
                Controls.AddAt (index, control);

            if (onLoad != null) {
                control.Page.LoadComplete += delegate {
                    onLoad (control, new EventArgs ());
                };
            }

            // returning newly created control back to caller, such that he can set his properties and such for it
            return control;
        }

        /// <summary>
        ///     Removes a control from the control collection, and persist the change.
        /// 
        ///     Using this method, together with CreatePersistentControl, you can change the Controls collection of your
        ///     container widgets, and have the changes persist across multiple server requests.
        ///     This allows you to add, remove, and change the Controls collection of your container widgets, and have the
        ///     widget remember its changes across requests.
        /// 
        ///     Using this feature, will increase the amount of ViewState your controls uses, since it persists the changes
        ///     into the ViewState.
        /// </summary>
        /// <param name="control">Control to remove.</param>
        public void RemoveControlPersistent (Control control)
        {
            StoreOriginalControls ();
            Controls.Remove (control);
            ReRenderChildren ();
        }

        /// <summary>
        ///     Removes a control from the control collection, at the given index, and persists the change.
        /// 
        ///     See <see cref="pf.ajax.widgets.Container.RemoveControlPersistent">RemoveControlPersistent</see> to understand how this method 
        ///     works. This method works the same, except it removes a control at a specified index, and not a specific control.
        /// </summary>
        /// <param name="index">Index of control to remove.</param>
        public void RemoveControlPersistentAt (int index)
        {
            StoreOriginalControls ();
            Controls.RemoveAt (index);
            ReRenderChildren ();
        }

        protected override void LoadViewState (object savedState)
        {
            // reloading persisted controls, if there are any
            var tmp = savedState as object[];
            if (tmp != null && tmp.Length > 0 && tmp [0] is string[][]) {
                // we're managing our own controls collection, and need to reload from viewstate all the 
                // control types and ids. first figuring out which controls actually exists in this control at the moment
                var ctrlsViewstate = (from idx in (string[][]) tmp [0] select new Tuple<string, string> (idx [0], idx [1])).ToList ();

                // then removing all controls that is not persisted, and all LiteralControls since they tend to mess up their IDs
                var toRemove = Controls.Cast<Control> ().Where (
                    idxControl => string.IsNullOrEmpty (idxControl.ID) || !ctrlsViewstate.Exists (idxViewstate => idxViewstate.Item2 == idxControl.ID)).ToList ();
                foreach (var idxCtrl in toRemove) {
                    Controls.Remove (idxCtrl);
                }

                // then adding all controls that are persisted but does not exist in the controls collection
                var controlPosition = 0;
                foreach (var idxTuple in ctrlsViewstate) {
                    var exist = Controls.Cast<Control> ().Any (idxCtrl => idxTuple.Item2 == idxCtrl.ID);
                    if (!exist) {
                        var control = Creators [GetTypeFromId (idxTuple.Item1)].Create ();
                        control.ID = idxTuple.Item2;
                        Controls.AddAt (controlPosition, control);
                    }
                    controlPosition += 1;
                }

                StoreOriginalControls ();

                base.LoadViewState (tmp [1]);
            } else {
                base.LoadViewState (savedState);
            }
        }

        protected override object SaveViewState ()
        {
            // making sure all dynamically added controls are persistent to the control state, if there are any
            if (_originalCollection != null) {
                // yup, we're managing our own control collection, and need to save to viewstate all of the controls
                // types and ids that exists in our control collection
                var tmp = new object[2];
                tmp [0] = (from Control idx in Controls where !string.IsNullOrEmpty (idx.ID) select new[] {GetTypeId (idx.GetType ()), idx.ID}).ToArray ();
                tmp [1] = base.SaveViewState ();
                return tmp;
            }
            // not managing controls
            return base.SaveViewState ();
        }

        protected override void OnInit (EventArgs e)
        {
            // making sure all the automatically generated LiteralControls are removed, since they mess up their IDs,
            // but not in a normal postback, or initial loading of the page, since we need the formatting they provide
            if ((Page as IAjaxPage).Manager.IsPhosphorusRequest) {
                var ctrls = Controls.Cast<Control> ().Where (idx => string.IsNullOrEmpty (idx.ID)).ToList ();
                foreach (var idx in ctrls) {
                    Controls.Remove (idx);
                }
            }
            base.OnInit (e);
        }

        protected override void RenderChildrenWidgetsAsJson (HtmlTextWriter writer)
        {
            if (_originalCollection == null) {
                base.RenderChildrenWidgetsAsJson (writer);
            } else {
                RenderAddedControls ();
                RenderRemovedControls ();
                RenderOldControls (writer);
            }
        }

        /*
         * creates a new unique ID
         */
        private string CreateId ()
        {
            var retVal = Guid.NewGuid ().ToString ().Replace ("-", "");
            retVal = "x" + retVal [0] + retVal [5] + retVal [10] + retVal [15] + retVal [20] + retVal [25] + retVal [30];
            return retVal;
        }

        // renders all controls that was added this request, and return list back to caller
        private void RenderAddedControls ()
        {
            var widgets = new List<Tuple<string, int>> ();
            foreach (Control idx in Controls) {
                if (_originalCollection.Contains (idx) || string.IsNullOrEmpty (idx.ID))
                    continue; // control has already been rendered, or is a literal control without an ID

                // getting control's html
                string html;
                using (var stream = new MemoryStream ()) {
                    using (var txt = new HtmlTextWriter (new StreamWriter (stream))) {
                        idx.RenderControl (txt);
                        txt.Flush ();
                    }
                    stream.Seek (0, SeekOrigin.Begin);
                    using (TextReader reader = new StreamReader (stream)) {
                        html = reader.ReadToEnd ();
                    }
                }
                var position = Controls.IndexOf (idx);
                widgets.Add (new Tuple<string, int> (html, position));
            }

            // we have to insert such that the first controls becomes added before controls behind it, such that the dom position
            // don't become messed up
            widgets.Sort (
                (lhs, rhs) => lhs.Item2.CompareTo (rhs.Item2));

            // informing our manager that the current widget has changes, if we should
            if (widgets.Count > 0) {
                foreach (var idx in widgets) {
                    (Page as IAjaxPage).Manager.RegisterWidgetChanges (ClientID, "__pf_add_" + idx.Item2, idx.Item1);
                }
            }
        }

        // renders all controls that was removed, and returns list back to caller
        private void RenderRemovedControls ()
        {
            foreach (var idxOriginal in _originalCollection) {
                var exist = Controls.Cast<Control> ().Any (idxActual => idxActual.ID == idxOriginal.ID);
                if (!exist && !string.IsNullOrEmpty (idxOriginal.ID))
                    (Page as IAjaxPage).Manager.RegisterDeletedWidget (idxOriginal.ClientID);
            }
        }

        private void RenderOldControls (HtmlTextWriter writer)
        {
            var old = RenderMode;
            RenderMode = RenderingMode.Default;
            foreach (Control idx in Controls) {
                if (_originalCollection.Contains (idx)) {
                    idx.RenderControl (writer);
                }
            }
            RenderMode = old;
        }

        // storing original controls that were there before we started adding and removing controls
        private void StoreOriginalControls ()
        {
            if (_originalCollection == null) {
                _originalCollection = new List<Control> ();
                foreach (Control idxCtrl in Controls) {
                    _originalCollection.Add (idxCtrl);
                }
            }
        }

        // use to make sure we store a reference to our creator instance for later requests
        private static ICreator GetCreator<T> () where T : Control, new ()
        {
            if (!Creators.ContainsKey (typeof (T))) {
                lock (Lock) {
                    if (!Creators.ContainsKey (typeof (T)))
                        Creators [typeof (T)] = new Creator<T> ();
                }
            }
            return Creators [typeof (T)];
        }

        // used to "pack" the types stored in the ViewState to make viewstate as small as possible
        private static string GetTypeId (Type type)
        {
            foreach (var idx in TypeMapper) {
                if (idx.Item2 == type)
                    return idx.Item1;
            }

            // didn't exist, need to create it
            lock (Lock) {
                if (!TypeMapper.Exists (
                    idx => idx.Item2 == type))
                    TypeMapper.Add (new Tuple<string, Type> (TypeMapper.Count.ToString (), type));
            }

            // recursively calling self for simplicity
            return GetTypeId (type);
        }

        // used to retrieve the type from the type mapper collection
        private static Type GetTypeFromId (string id)
        {
            return TypeMapper.Find (idx => idx.Item1 == id).Item2;
        }

        // interface to create controls to avoid reflection as much as possible
        private interface ICreator
        {
            Control Create ();
        }

        // some "anti-reflection magic"
        private class Creator<T> : ICreator where T : Control, new ()
        {
            public Control Create () { return new T (); }
        }
    }
}
