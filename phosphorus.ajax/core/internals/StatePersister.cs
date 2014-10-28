/*
 * phosphorus five, copyright 2014 - Mother Earth, Jannah, Gaia
 * phosphorus five is licensed as mitx11, see the enclosed LICENSE file for details
 */

using System;
using System.IO;
using System.Text;
using System.Web.UI;
using System.Collections.Generic;

namespace phosphorus.ajax.core
{
    internal class StatePersister : PageStatePersister
    {
        private const string _sessionKey = "__pf_viewstate_session_key";
        private Guid _viewStateId;
        private int _numberOfViewStateEntries;

        public StatePersister (Page page, int numberOfViewStateEntries)
            : base (page)
        {
            _numberOfViewStateEntries = numberOfViewStateEntries;

            if (page.IsPostBack) {
                // this is a postback, need to fetch existing ViewState from session
                _viewStateId = new Guid (page.Request ["__pf_state_key"]);
            } else {
                // this is an initial load, or a reload of a page, need to create a new ViewState key
                _viewStateId = Guid.NewGuid ();
            }
            if (!(page as IAjaxPage).Manager.IsPhosphorusRequest)
            {
                // adding the viewstate ID to the form, such that we can retrieve it again when the
                // client does a postback
                LiteralControl literal = new LiteralControl ();
                literal.Text = string.Format (@"
            <input type=""hidden"" value=""{0}"" name=""__pf_state_key"">
        ", _viewStateId);
                page.Form.Controls.Add (literal);
            }
        }

        public override void Load ()
        {
            if (Page.Session [_sessionKey] == null)
                throw new ApplicationException ("session timeout");

            // to avoid session clogging up with an infinite number of viewstate values, one for each initial loading of a page,
            // we have a list of viewstates, not allowing to exceed "_numberOfViewStateEntries" per session.
            // this means that we have a practical limit of "_numberOfViewStateEntries" open browser windows per session, or more
            // accurately; user cannot reload the same page without invalidating viewstates older than "_numberOfViewStateEntries"
            var viewState = Page.Session [_sessionKey] as List<Tuple<Guid, string>>;

            var entry = viewState.Find (
                delegate (Tuple<Guid, string> idx) {
                return idx.Item1 == _viewStateId;
            });
            if (entry == null) {
                throw new ApplicationException (@"the viewstate for this page has been invalidated du to too many open browser windows 
within the same application, or reloading a page over and over again, 
such that older viewstate entries has been invalidated. try to either 
close some windows, or avoid reloading the same window over and over again");
            }

            LosFormatter formatter = new LosFormatter ();
            Pair pair = formatter.Deserialize (entry.Item2) as Pair;
            ControlState = pair.First;
            ViewState = pair.Second;
        }

        public override void Save ()
        {
            StringBuilder builder = new StringBuilder ();
            using (StringWriter writer = new StringWriter (builder))
            {
                LosFormatter formatter = new LosFormatter ();
                formatter.Serialize (writer, new Pair (ControlState, ViewState));
            }

            // to avoid session clogging up with an infinite number of viewstate values, one for each initial loading of a page,
            // we have a list of viewstates, not allowing to exceed "_numberOfViewStateEntries" per session.
            // this means that we have a practical limit of "_numberOfViewStateEntries" open browser windows per session, or more
            // accurately; user cannot reload the same page without invalidating viewstates older than "_numberOfViewStateEntries"
            var viewState = Page.Session [_sessionKey] as List<Tuple<Guid, string>>;
            if (viewState == null) {
                viewState = new List<Tuple<Guid, string>> ();
                Page.Session [_sessionKey] = viewState;
            }

            // making sure the most recent is at the end
            viewState.RemoveAll (
                delegate (Tuple<Guid, string> idx) {
                return idx.Item1 == _viewStateId;
            });
            viewState.Add (new Tuple<Guid, string> (_viewStateId, builder.ToString ()));

            // making sure we never have more than "_numberOfViewStateEntries" entries
            while (viewState.Count > _numberOfViewStateEntries) {

                // removing oldest entry
                viewState.RemoveAt (0);
            }
        }
    }
}

