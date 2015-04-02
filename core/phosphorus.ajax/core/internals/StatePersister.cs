/*
 * Phosphorus.Five, Copyright 2014 - 2015, Thomas Hansen - thomas@magixilluminate.com
 * Phosphorus.Five is licensed under the terms of the MIT license.
 * See the enclosed LICENSE file for details.
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web.UI;

// ReSharper disable PossibleNullReferenceException

namespace phosphorus.ajax.core.internals
{
    /// <summary>
    ///     Responsible for storing page's state for all requests in system.
    /// 
    ///     Persists state of page to session, and renders a hidden input field back to client, with the name of "__pf_state_key", being a
    ///     key to the state for the page, while eliminating ViewState rendering for page. You rarely, if ever, have to fiddle with 
    ///     this class yourself.
    /// </summary>
    internal class StatePersister : PageStatePersister
    {
        private const string SessionKey = "__pf_viewstate_session_key";
        private readonly int _numberOfViewStateEntries;
        private readonly Guid _viewStateId;

        /// <summary>
        ///     Initializes a new instance of the <see cref="phosphorus.ajax.core.internals.StatePersister"/> class.
        /// </summary>
        /// <param name="page">Page instance.</param>
        /// <param name="numberOfViewStateEntries">Number of view state entries per session.</param>
        internal StatePersister (Page page, int numberOfViewStateEntries)
            : base (page)
        {
            _numberOfViewStateEntries = numberOfViewStateEntries;

            _viewStateId = page.IsPostBack ? new Guid (page.Request ["__pf_state_key"]) : Guid.NewGuid ();
            if ((page as IAjaxPage).Manager.IsPhosphorusRequest) return;
            // adding the viewstate ID to the form, such that we can retrieve it again when the
            // client does a postback
            var literal = new LiteralControl {Text = string.Format (@"
            <input type=""hidden"" value=""{0}"" name=""__pf_state_key"">
        ", _viewStateId)};
            page.Form.Controls.Add (literal);
        }

        public override void Load ()
        {
            if (Page.Session [SessionKey] == null)
                throw new ApplicationException ("session timeout");

            // to avoid session clogging up with an infinite number of viewstate values, one for each initial loading of a page,
            // we have a list of viewstates, not allowing to exceed "_numberOfViewStateEntries" per session.
            // this means that we have a practical limit of "_numberOfViewStateEntries" open browser windows per session, or more
            // accurately; user cannot reload the same page without invalidating viewstates older than "_numberOfViewStateEntries"
            var viewState = Page.Session [SessionKey] as List<Tuple<Guid, string>>;

            var entry = viewState.Find (idx => idx.Item1 == _viewStateId);
            if (entry == null) {
                throw new ApplicationException (@"The state for this page is not longer valid, probable cause was that there was too
many viewstate values for current session, and hence current viewstate was removed. If you wish to see this page again, you must reload it");
            }

            var formatter = new LosFormatter ();
            var pair = formatter.Deserialize (entry.Item2) as Pair;
            ControlState = pair.First;
            ViewState = pair.Second;
        }

        public override void Save ()
        {
            var builder = new StringBuilder ();
            using (var writer = new StringWriter (builder)) {
                var formatter = new LosFormatter ();
                formatter.Serialize (writer, new Pair (ControlState, ViewState));
            }

            // to avoid session clogging up with an infinite number of viewstate values, one for each initial loading of a page,
            // we have a list of viewstates, not allowing to exceed "_numberOfViewStateEntries" per session.
            // this means that we have a practical limit of "_numberOfViewStateEntries" open browser windows per session, or more
            // accurately; user cannot reload the same page without invalidating viewstates older than "_numberOfViewStateEntries"
            var viewState = Page.Session [SessionKey] as List<Tuple<Guid, string>>;
            if (viewState == null) {
                viewState = new List<Tuple<Guid, string>> ();
                Page.Session [SessionKey] = viewState;
            }

            // making sure the most recent is at the end
            viewState.RemoveAll (idx => idx.Item1 == _viewStateId);
            viewState.Add (new Tuple<Guid, string> (_viewStateId, builder.ToString ()));

            // making sure we never have more than "_numberOfViewStateEntries" entries
            while (viewState.Count > _numberOfViewStateEntries) {
                // removing oldest entry
                viewState.RemoveAt (0);
            }
        }
    }
}