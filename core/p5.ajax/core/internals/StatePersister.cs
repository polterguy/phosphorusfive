/*
 * Phosphorus Five, copyright 2014 - 2016, Thomas Hansen, mr.gaia@gaiasoul.com
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
using System.Text;
using System.Web.UI;
using System.Collections.Generic;

namespace p5.ajax.core.internals
{
    /// <summary>
    ///     Responsible for storing page's state for all requests in system
    /// </summary>
    internal class StatePersister : PageStatePersister
    {
        private const string SessionKey = "_p5_viewstate_session_key";
        private readonly int _numberOfViewStateEntries;
        private readonly Guid _viewStateId;

        /// <summary>
        ///     Initializes a new instance of the <see cref="p5.ajax.core.internals.StatePersister"/> class
        /// </summary>
        /// <param name="page">Page instance</param>
        /// <param name="numberOfViewStateEntries">Number of view state entries per session</param>
        internal StatePersister (Page page, int numberOfViewStateEntries)
            : base (page)
        {
            _numberOfViewStateEntries = numberOfViewStateEntries;

            _viewStateId = page.IsPostBack ? new Guid (page.Request ["_p5_state_key"]) : Guid.NewGuid ();
            if ((page as IAjaxPage).Manager.IsPhosphorusAjaxRequest) return;

            // Adding the viewstate ID to the form, such that we can retrieve it again when the client does a postback
            var literal = new LiteralControl {Text = string.Format (@"
            <input type=""hidden"" value=""{0}"" name=""_p5_state_key"">
        ", _viewStateId)};
            page.Form.Controls.Add (literal);
        }

        public override void Load ()
        {
            if (Page.Session [SessionKey] == null)
                throw new ApplicationException ("Session timeout");

            // To avoid session clogging up with an infinite number of viewstate values, one for each initial loading of a page,
            // we have a list of viewstates, not allowing to exceed "_numberOfViewStateEntries" per session.
            // This means that we have a practical limit of "_numberOfViewStateEntries" open browser windows per session, or more
            // accurately; user cannot reload the same page without invalidating viewstates older than "_numberOfViewStateEntries"
            var viewState = Page.Session [SessionKey] as List<Tuple<Guid, string>>;

            var entry = viewState.Find (idx => idx.Item1 == _viewStateId);
            if (entry == null) {
                throw new ApplicationException ("The state for this page is not longer valid, please reload page");
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

            // To avoid session clogging up with an infinite number of viewstate values, one for each initial loading of a page,
            // we have a list of viewstates, not allowing to exceed "_numberOfViewStateEntries" per session.
            // This means that we have a practical limit of "_numberOfViewStateEntries" open browser windows per session, or more
            // accurately; user cannot reload the same page without invalidating viewstates older than "_numberOfViewStateEntries"
            var viewState = Page.Session [SessionKey] as List<Tuple<Guid, string>>;
            if (viewState == null) {
                viewState = new List<Tuple<Guid, string>> ();
                Page.Session [SessionKey] = viewState;
            }

            // Making sure the most recent is at the end
            viewState.RemoveAll (idx => idx.Item1 == _viewStateId);
            viewState.Add (new Tuple<Guid, string> (_viewStateId, builder.ToString ()));

            // Making sure we never have more than "_numberOfViewStateEntries" entries
            while (viewState.Count > _numberOfViewStateEntries) {
                
                // Removing oldest entry
                viewState.RemoveAt (0);
            }
        }
    }
}