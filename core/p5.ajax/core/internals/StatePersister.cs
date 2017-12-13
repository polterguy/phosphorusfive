/*
 * Phosphorus Five, copyright 2014 - 2017, Thomas Hansen, thomas@gaiasoul.com
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
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Collections.Generic;

namespace p5.ajax.core.internals
{
    /// <summary>
    ///     Responsible for storing page's state for all requests.
    ///     Simply puts ViewState data into session, but never more than a maximum number of objects for each session, 
    ///     which is configurable through web.config.
    /// </summary>
    class StatePersister : PageStatePersister
    {
        /// <summary>
        ///     Session timeout exception.
        /// </summary>
        class SessionTimeoutException : Exception
        { }

        // Becomes the Session key for all ViewState entires for current session.
        const string SessionKey = ".p5.ajax.ViewState.Session-Key";
        readonly int _numberOfViewStateEntries;
        readonly Guid _viewStateId;

        /// <summary>
        ///     Initializes a new instance of the <see cref="StatePersister"/> class.
        /// </summary>
        /// <param name="page">Page instance</param>
        /// <param name="numberOfViewStateEntries">Number of view state entries per session</param>
        internal StatePersister (AjaxPage page, int numberOfViewStateEntries)
            : base (page)
        {
            _numberOfViewStateEntries = numberOfViewStateEntries;
            if (_numberOfViewStateEntries < 0 || _numberOfViewStateEntries > 50)
                throw new ApplicationException ("Legal value for '.p5.webapp.viewstate-per-session-entries' web.config setting is between 0 and 50");

            _viewStateId = page.IsPostBack ? new Guid (page.Request ["_p5_state_key"]) : Guid.NewGuid ();
            if (page.IsAjaxRequest) return;

            // Adding the viewstate ID to the form, such that we can retrieve it again when the client does a postback
            var literal = new LiteralControl { Text = string.Format ("\t<input type=\"hidden\" value=\"{0}\" name=\"_p5_state_key\">\r\n\t\t", _viewStateId) };
            if (page.Master != null) {

                // Need to add control to the first ContentPlaceholder that's inside of our main form.
                // Notice, if there are none, we default to the page's Form.
                var parent = FindFormControl (page.Master) ?? page.Form;
                parent.Controls.Add (literal);

            } else {
                
                page.Form.Controls.Add (literal);
            }
        }

        /*
         * Used above, if master pages are being used.
         * 
         * Basically, we'll need to find the first ContentPlaceholder that's inside of our form, if any.
         */
        private Control FindFormControl (Control ctrl)
        {
            if (ctrl is ContentPlaceHolder && FindParentForm (ctrl.Parent) != null)
                return ctrl;
            foreach (Control idx in ctrl.Controls) {
                var tmp = FindFormControl (idx);
                if (tmp != null)
                    return tmp;
            }
            return null;
        }

        /*
         * Used above if master pages are being used.
         */
        private Control FindParentForm (Control ctrl)
        {
            if (ctrl is HtmlForm)
                return ctrl;
            if (ctrl.Parent != null)
                return FindParentForm (ctrl.Parent);
            return null;
        }

        /// <summary>
        ///     Loads the state for the page from Session.
        /// </summary>
        public override void Load ()
        {
            try {

                if (Page.Session [SessionKey] == null)
                    throw new SessionTimeoutException ();

                // To avoid session clogging up with an infinite number of viewstate values, one for each initial loading of a page,
                // we have a list of viewstates, not allowing to exceed "_numberOfViewStateEntries" per session.
                // This means that we have a practical limit of "_numberOfViewStateEntries" open browser windows per session, or more
                // accurately; user cannot reload the same page without invalidating viewstates older than "_numberOfViewStateEntries"
                var viewState = Page.Session [SessionKey] as List<Tuple<Guid, string>>;

                var entry = viewState.Find (ix => ix.Item1 == _viewStateId);
                if (entry == null) {
                    throw new SessionTimeoutException ();
                }

                var formatter = new LosFormatter ();
                var pair = formatter.Deserialize (entry.Item2) as Pair;
                ControlState = pair.First;
                ViewState = pair.Second;

            } catch (SessionTimeoutException) {

                // Making sure we inform client about session timeout, in an adequate way.
                Page.Response.StatusCode = 457;
                Page.Response.StatusDescription = "Session timeout";
                Page.Response.End ();
            }
        }

        /// <summary>
        ///     Saves the state for the page into Session.
        /// </summary>
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