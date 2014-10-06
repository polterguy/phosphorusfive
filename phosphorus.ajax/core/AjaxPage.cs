/*
 * phosphorus five, copyright 2014 - thomas@magixilluminate.com
 * phosphorus five is licensed as mitx11, see the enclosed LICENSE file for details
 */

using System;
using System.Web.UI;

namespace phosphorus.ajax.core
{
    /// <summary>
    /// helper class for inheriting your page from. if you do not wish to inherit from this class, 
    /// you can implement the <see cref="phosphorus.ajax.core.IAjaxPage"/> interface on your page instead 
    /// and create an instance of the <see cref="phosphorus.ajax.core.Manager"/> for instance during the 
    /// initialization of your page
    /// </summary>
    public class AjaxPage : Page, IAjaxPage
    {
        private Manager _manager;

        /// <summary>
        /// raises the pre init event.
        /// </summary>
        /// <param name="e">parameter passed in from asp.net</param>
        protected override void OnPreInit (EventArgs e)
        {
            _manager = new Manager (this);
            base.OnPreInit (e);
        }

        /// <summary>
        /// returns the manager for your page
        /// </summary>
        /// <value>the manager</value>
        public Manager Manager {
            get { return _manager; }
        }
    }
}

