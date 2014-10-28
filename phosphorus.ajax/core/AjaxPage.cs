/*
 * phosphorus five, copyright 2014 - Mother Earth, Jannah, Gaia
 * phosphorus five is licensed as mitx11, see the enclosed LICENSE file for details
 */

using System;
using System.Web.UI;

namespace phosphorus.ajax.core
{
    /// <summary>
    /// helper class for inheriting your page from. if you do not wish to inherit from this class,
    /// you can implement the <see cref="phosphorus.ajax.core.IAjaxPage"/> interface on your page instead,
    /// and create an instance of the <see cref="phosphorus.ajax.core.Manager"/> for instance during the
    /// initialization of your page
    /// </summary>
    public class AjaxPage : Page, IAjaxPage
    {
        private Manager _manager;
        private PageStatePersister _statePersister;

        protected override void OnPreInit (EventArgs e)
        {
            _manager = new Manager (this);
            base.OnPreInit (e);
        }

        public Manager Manager {
            get { return _manager; }
        }

        /// <summary>
        /// gets or sets a value indicating whether this <see cref="phosphorus.ajax.core.AjaxPage"/> stores its viewstate in the session object
        /// </summary>
        /// <value><c>true</c> if it should store its viewstate in the session object; otherwise, <c>false</c></value>
        public bool StoreViewStateInSession {
            get;
            set;
        }

        /// <summary>
        /// overide this one if you wish to increase or decrease the number of viewstate entries per session, meaning for
        /// all practical concerns, the number of legally open browser windows within the same application. the default value
        /// is 10 viewstate entries for each session
        /// </summary>
        /// <value>the number of valid viewstate entries for each session</value>
        protected virtual int ViewStateSessionEntries {
            get { return 10; }
        }

        protected override System.Web.UI.PageStatePersister PageStatePersister
        {
            get
            {
                if (StoreViewStateInSession) {
                    if (_statePersister == null)
                        _statePersister = new StatePersister (this, ViewStateSessionEntries);
                    return _statePersister;
                }
                return base.PageStatePersister;
            }
        }
    }
}

