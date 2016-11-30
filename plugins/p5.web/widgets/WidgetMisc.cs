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
using System.Linq;
using System.Web.UI;
using p5.exp;
using p5.core;
using p5.ajax.widgets;
using p5.exp.exceptions;

namespace p5.web.widgets
{
    /// <summary>
    ///     Class encapsulating miscelanous Active Events for widgets
    /// </summary>
    public class WidgetMisc : BaseWidget
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="p5.web.widgets.WidgetProperties"/> class
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="manager">PageManager owning this instance</param>
        public WidgetMisc (ApplicationContext context, PageManager manager)
            : base (context, manager)
        { }

        #region [ -- Widget misc Active Events -- ]

        /// <summary>
        ///     Entirely re-renders widget.
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "re-render-widget")]
        public void re_render_widget (ApplicationContext context, ActiveEventArgs e)
        {
            // Making sure we clean up and remove all arguments passed in after execution
            using (new Utilities.ArgsRemover (e.Args, true)) {

                // Looping through all widget IDs given by caller
                foreach (var idxWidget in FindWidgets<Widget> (context, e.Args, "re-render-widget")) {
                    idxWidget.ReRender ();
                }
            }
        }

        #endregion
    }
}
