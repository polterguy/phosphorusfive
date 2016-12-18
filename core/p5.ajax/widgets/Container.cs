/*
 * Phosphorus Five, copyright 2014 - 2016, Thomas Hansen, thomas@gaiasoul.com
 * 
 * This file is part of Phosphorus Five.
 *
 * Phosphorus Five is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License version 3, as published by
 * the Free Software Foundation.
 *
 *
 * Phosphorus Five is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with Phosphorus Five.  If not, see <http://www.gnu.org/licenses/>.
 * 
 * If you cannot for some reasons use the GPL license, Phosphorus
 * Five is also commercially available under Quid Pro Quo terms. Check 
 * out our website at http://gaiasoul.com for more details.
 */

using System;
using System.IO;
using System.Linq;
using System.Web.UI;
using System.Collections.Generic;
using p5.ajax.core;

namespace p5.ajax.widgets
{
    /// <summary>
    ///     A widget that can contains children widgets.
    /// 
    ///     Useful for widgets where you need to explicitly de-reference its children widgets.
    /// </summary>
    [ViewStateModeById]
    public class Container : Widget, INamingContainer
    {
        /*
         * Interface used to create controls.
         * This is here to be able to map a string to a concrete implementation of this interface, who's sole purpose it is
         * to re-create the accurate type upon postbacks and Ajax requests to the server, that had its typename stored in the ViewState.
         * 
         * Together with the implementation below, and the "_creators" static field in class, it allows us to create a specific object 
         * type (widget type), by mapping from its type's FullName, to an implementation, that simply creates a new object of the specified type.
         */
        private interface ICreator
        {
            Control Create ();
        }

        /*
         * See the above comment for the ICreator interface.
         * 
         * Concrete implementation, which there should exist one of, for every single widget type in system, who's sole purpose it is to create a Widget,
         * given a string.
         */
        private class Creator<T> : ICreator where T : Control, new()
        {
            public Control Create () { return new T (); }
        }

        /* 
         * Contains all the creator objects to create our controls when needed.
         * 
         * When we store a dynamically created widget in the ViewState, we only store its Type's FullName, and its ID. Once we later re-create
         * a control with the same type, and the same ID, and "root it" into our Page's Control hierarchy, all other ViewState properties are 
         * automatically re-created from the ViewState.
         * 
         * This guy simply allows us to map from a Widget's type's FullName, to an implementation of an ICreator, who's purpose it is to
         * know exactly which type of Widget to create.
         * The entries in this dictionary, are dynamically populated, as one widget type after the other is created.
         * We could get rid of this, and the above template class and interface if we wanted. This would however require us to use for instance 
         * reflection, or less flexible solutions to be able to re-create our widgets upon postbacks to the server.
         */
        private static readonly Dictionary<string, ICreator> _creators = new Dictionary<string, ICreator> ();

        /*
         * Contains the original controls collection, before we started adding and removing controls for current request.
         * 
         * Notice, we must store this, such that we can delete items, that are for instance statically declared in .aspx markup, and have
         * the changes persist into the ViewState.
         */
        private List<Control> _originalCollection;

        /*
         * Overridden to make sure the default element type for this widget is "div".
         */
        public override string Element
        {
            get {
                if (string.IsNullOrEmpty (base.Element))
                    return "div";
                return base.Element;
            }
            set { base.Element = value == "div" ? null : value; }
        }

        /*
         * Overridden to make sure we handle the "value" attribute for "select" HTML widgets correctly, in addition to throwing an exception,
         * if user tries to set or get the "innerValue" property/attribute of widget.
         */
        public override string this [string name]
        {
            get {
                if (name == "innerValue")
                    throw new ArgumentException ("You cannot get the 'innerValue' property of a Container widget");

                // Special treatment for select HTML elements, to make it resemble what goes on on the client-side.
                if (name == "value" && Element == "select") {

                    // Returning each selected "option" element separated by comma, in case this is a multi select widget.
                    string retVal = "";
                    foreach (Widget idxWidget in Controls) {
                        if (idxWidget.HasAttribute ("selected"))
                            retVal += idxWidget ["value"] + ",";
                    }
                    return retVal.TrimEnd (','); // Removing last comma ",".

                } else {

                    // No special treatment required.
                    return base [name];
                }
            }
            set {
                if (name == "innerValue")
                    throw new ArgumentException ("You cannot set the 'innerValue' property of a Container widget");

                // Special treatment for select HTML elements, to make it resemble what goes on on the client-side.
                if (name == "value" && Element == "select") {

                    // Splitting specified value by comma ",", and adding the "selected" attribute for each option element with a value
                    // matching anything in the split results.
                    foreach (Widget idxWidget in Controls) {
                        idxWidget.DeleteAttribute ("selected"); // DeleteAttribute will check if attribute exists before attempting to delete it.
                    }
                    foreach (string idxSplit in value.Split (',')) {
                        foreach (Widget idxWidget in Controls) {
                            if (idxWidget["value"] == idxSplit) {
                                idxWidget ["selected"] = null;
                            }
                        }
                    }
                } else {

                    // No special treatment required.
                    base [name] = value;
                }
            }
        }

        /*
         * Overridden to make sure we can correctly handle "select" HTML widgets.
         */
        public override bool HasAttribute (string name)
        {
            if (name == "value" && Element == "select") {

                // Special treatment for select HTML elements, to make it resemble what goes on on the client-side.
                foreach (Widget idxWidget in Controls) {
                    if (idxWidget.HasAttribute ("selected"))
                        return true;
                }
                return false;

            } else {

                // No special treatment required.
                return base.HasAttribute (name);
            }
        }

        /// <summary>
        ///     Returns all controls of the specified type T from the Controls collection.
        /// 
        ///     Useful to avoid returning automatically created LiteralControls, due to formatting applied in .aspx file, etc.
        /// </summary>
        /// <returns>All controls of type T from the Controls property.</returns>
        /// <typeparam name="T">Type of controls to retrieve.</typeparam>
        public IEnumerable<T> ControlsOfType<T> () where T : Control
        {
            return from Control idx in Controls let tmp = idx as T where tmp != null select tmp;
        }

        /// <summary>
        ///     Creates a persistent child control, that will be automatically re-created during future server requests.
        /// </summary>
        /// <returns>The persistent control.</returns>
        /// <param name="id">ID of your control. If null, an automatic id will be created and assigned control.</param>
        /// <param name="index">Index of where to insert control. If -1, the control will be appended into Controls collection.</param>
        /// <typeparam name="T">The type of control you want to create.</typeparam>
        public T CreatePersistentControl<T> (string id = null, int index = -1) where T : Control, new ()
        {
            // Making sure we store the fact that the children collection of this widget has changed.
            ChildrenCollectionIsModified ();

            // Then we must make sure we store our original controls, before we start adding new ones to the Controls collection.
            // Notice, this is only done the first time we create a new child control for a Container widget.
            MakeSureOriginalControlsAreStored ();

            // Creating a new control, and adding to the controls collection.
            // Notice, we must use our GetCreator<T> method, since it populates our "_creators" field, with a string being the type's FullName and
            // the ICreator implementation creating a new Widget of type T.
            // Otherwise, our "_creator" static Dictionary object, will not contain an entry for the given type, when we're about to re-create our
            // widget upon our next callback/postback to the server.
            var control = GetCreator<T> ().Create () as T;
            control.ID = string.IsNullOrEmpty (id) ? CreateUniqueId () : id;

            if (index == -1)
                Controls.Add (control);
            else
                Controls.AddAt (index, control);

            // Returning newly created control back to caller, such that he can set other properties and such for it.
            return control;
        }

        /// <summary>
        ///     Removes a control from the control collection, and persists the change.
        /// </summary>
        /// <param name="control">Control to remove</param>
        public void RemoveControlPersistent (Control control)
        {
            // Making sure we store the fact that the children collection of this widget has changed.
            ChildrenCollectionIsModified ();

            // Then making sure we store original controls, the first time the Controls collection is changed, such that we now which widgets to render,
            // and which to remove on the client side.
            MakeSureOriginalControlsAreStored ();

            // Now we can remove control from Controls collection.
            Controls.Remove (control);
        }

        /// <summary>
        ///     Removes a control from the control collection, at the given index, and persists the change.
        /// </summary>
        /// <param name="index">Index of control to remove</param>
        public void RemoveControlPersistentAt (int index)
        {
            // Making sure we store the fact that the children collection of this widget has changed.
            ChildrenCollectionIsModified ();

            // Then making sure we store original controls, the first time the Controls collection is changed, such that we now which widgets to render,
            // and which to remove on the client side.
            MakeSureOriginalControlsAreStored ();

            // Now we can remove control from Controls collection.
            Controls.RemoveAt (index);
        }

        /// <summary>
        ///     Creates a new Unique ID for a Control.
        /// </summary>
        /// <returns>The new unique ID</returns>
        public static string CreateUniqueId ()
        {
            var retVal = Guid.NewGuid ().ToString ().Replace ("-", "");
            retVal = "x" + retVal [0] + retVal [5] + retVal [10] + retVal [15] + retVal [20] + retVal [25] + retVal [30];
            return retVal;
        }

        /*
         * Implementation of abstract base class method, to make sure we return true, only if widget has children widgets.
         */
        protected override bool HasContent
        {
            get { return Controls.Count > 0; }
        }

        /*
         * Overridden to make sure we remove all LiteralControls during Ajax requests.
         */
        protected override void OnInit (EventArgs e)
        {
            // Making sure all the automatically generated LiteralControls are removed, since they mess up their IDs,
            // but not in a normal postback, or initial loading of the page, since we need the formatting they provide.
            if (AjaxPage.IsAjaxRequest) {
                foreach (var idx in Controls.Cast<Control> ().Where (idx => string.IsNullOrEmpty (idx.ID)).ToList ()) {
                    Controls.Remove (idx);
                }
            }
            base.OnInit (e);
        }

        /*
         * Overridden to make sure we can correctly reload the Controls collection of widget, as persisted during SaveViewState.
         */
        protected override void LoadViewState (object savedState)
        {
            // Reloading persisted controls, if there are any.
            var tmp = savedState as object[];
            if (tmp != null && tmp.Length > 0 && tmp [0] is string[][]) {

                // We're managing our own controls collection, and need to reload from ViewState all the 
                // control types and ids. First figuring out which controls actually exists in this control at the moment.
                var ctrlsViewstate = (from idx in (string[][]) tmp [0] select new Tuple<string, string> (idx [0], idx [1])).ToList ();

                // Then removing all controls that are not persisted, and all LiteralControls since they tend to mess up their IDs.
                var toRemove = Controls.Cast<Control> ().Where (
                    idxControl => string.IsNullOrEmpty (idxControl.ID) || !ctrlsViewstate.Exists (idxViewstate => idxViewstate.Item2 == idxControl.ID)).ToList ();
                foreach (var idxCtrl in toRemove) {
                    Controls.Remove ((Control)idxCtrl);
                }

                // Then adding all controls that are persisted but does not exist in the controls collection
                var controlPosition = 0;
                foreach (var idxTuple in ctrlsViewstate) {
                    var exist = Controls.Cast<Control> ().Any (idxCtrl => idxTuple.Item2 == idxCtrl.ID);
                    if (!exist) {
                        var control = _creators [idxTuple.Item1].Create ();
                        control.ID = idxTuple.Item2;
                        Controls.AddAt (controlPosition, control);
                    }
                    controlPosition += 1;
                }

                MakeSureOriginalControlsAreStored ();

                base.LoadViewState (tmp [1]);

            } else {

                // Nothing to do here.
                base.LoadViewState (savedState);
            }
        }

        /*
         * Making sure we can persist the Controls collection into ViewState.
         */
        protected override object SaveViewState ()
        {
            // Making sure all dynamically added controls are persistent to the control state, if there are any.
            if (_originalCollection != null) {

                // Yup, we're managing our own control collection, and need to save to viewstate all of the controls
                // types and ids that exists in our control collection
                var tmp = new object [2];
                tmp [0] = (from Control idx in Controls where !string.IsNullOrEmpty (idx.ID) select new [] { idx.GetType ().FullName, idx.ID }).ToArray ();
                tmp [1] = base.SaveViewState ();
                return tmp;

            } else {

                // Nothing to do here.
                return base.SaveViewState ();
            }
        }

        /*
         * Overridden to correctly render children widgets when JSON rendering of children is necessary.
         */
        protected override void RenderChildrenWidgetsAsJson (HtmlTextWriter writer)
        {
            if (_originalCollection == null) {

                base.RenderChildrenWidgetsAsJson (writer);

            } else {

                RenderDeletedWidgets ();
                RenderAddedWidgets ();
                RenderOldWidgets (writer);
            }
        }

        /*
         * Renders all controls that was removed this request.
         */
        private void RenderDeletedWidgets ()
        {
            // Iterates through all Controls that were removed during this request, and make sure we register them for deletion on client side.
            // Notice, we don't care about LiteralControls, which have empty IDs.
            var controls = Controls.Cast<Control> ();
            foreach (var idxControl in _originalCollection.Where (ix => !string.IsNullOrEmpty (ix.ID) && !controls.Any (ix2 => ix2 == ix))) {
                AjaxPage.RegisterDeletedWidget (idxControl.ClientID);
            }
        }

        /*
         * Renders all controls that was added this request.
         */
        private void RenderAddedWidgets ()
        {
            // Looping through all Controls, figuring out which were not there in the "_originalCollection", before it was changed, and retrieving their
            // HTML, such that we can pass it to the client, as a JSON insertion.
            for (var idxNo = 0; idxNo < Controls.Count; idxNo++) {

                var idx = Controls [idxNo];
                if (_originalCollection.Contains (idx) || string.IsNullOrEmpty (idx.ID))
                    continue; // Control has already been rendered, or is a literal control without an ID

                // Getting control's HTML, by rendering it into a MemoryStream, and for then to pass it on as an "insertion" to our AjaxPage.
                using (var stream = new MemoryStream()) {
                    using (var txt = new HtmlTextWriter(new StreamWriter(stream))) {
                        idx.RenderControl (txt);
                        txt.Flush ();
                    }

                    // Now we can read the HTML from our MemoryStream.
                    stream.Seek (0, SeekOrigin.Begin);
                    using (TextReader reader = new StreamReader(stream)) {

                        // Registering currently iterated widget's HTML as an insertion on client side.
                        AjaxPage.RegisterWidgetChanges (ClientID, "__p5_add_" + idxNo, reader.ReadToEnd ());
                    }
                }
            }
        }

        /*
         * Rendering all controls that was neither added nor removed during this request.
         */
        private void RenderOldWidgets (HtmlTextWriter writer)
        {
            // Notice, to avoid re-rendering the old widgets, we need to change the state of our RenderMode enumeration, to avoid
            // having the old widgets entirely re-rendered.
            // Since these are the last widgets rendered in our collection, we don't need to reset the RenderMode enum afterwards either.
            var old = RenderMode;
            RenderMode = RenderingMode.Default;

            // Looping through all Controls that were in Controls collection before it was tampered with, and that are still in the Controls collection,
            // and simply render them "normally".
            foreach (Control idx in _originalCollection.Where (ix => Controls.Contains (ix))) {
                idx.RenderControl (writer);
            }
        }

        /*
         * Storing original controls that were there before we started adding and removing controls.
         * Necessary to keep track of "old controls", such that we only render added and deleted controls.
         */
        private void MakeSureOriginalControlsAreStored ()
        {
            if (_originalCollection == null) {
                _originalCollection = new List<Control> (Controls.Cast<Control> ());
            }
        }

        /*
         * Use to make sure we store a reference to our creator instance for later requests.
         * 
         * The Creator<T> which is returned from here, is responsible for creating Widgets according to values found in ViewState, among other things.
         */
        private static ICreator GetCreator<T> () where T : Control, new ()
        {
            var fullName = typeof (T).FullName;
            if (!_creators.ContainsKey (fullName))
                _creators [fullName] = new Creator<T> ();
            return _creators [fullName];
        }
    }
}
