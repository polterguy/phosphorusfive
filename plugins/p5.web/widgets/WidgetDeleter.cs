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

using System.Web.UI;
using System.Collections;
using p5.core;
using p5.ajax.widgets;
using p5.exp.exceptions;

namespace p5.web.widgets
{
    /// <summary>
    ///     Class encapsulating deletion of Ajax widgets.
    /// </summary>
    public class WidgetDeleter : BaseWidget
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="WidgetCreator"/> class.
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="manager">PageManager owning this instance</param>
        public WidgetDeleter (ApplicationContext context, PageManager manager)
            : base (context, manager)
        { }

        /// <summary>
        ///     Deletes the given widget(s) from page.
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "delete-widget")]
        [ActiveEvent (Name = "p5.web.widgets.delete")]
        public void p5_web_widgets_delete (ApplicationContext context, ActiveEventArgs e)
        {
            // Iterating through all widget IDs given.
            foreach (var idxWidget in FindWidgets<Widget> (context, e.Args)) {

                // Deleting currently iterated widget.
                DeleteWidget (context, e.Args, idxWidget);
            }
        }

        /// <summary>
        ///     Clears the given widget(s), removing all of its children widgets.
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "clear-widget")]
        [ActiveEvent (Name = "p5.web.widgets.clear")]
        public void p5_web_widgets_clear (ApplicationContext context, ActiveEventArgs e)
        {
            // Iterating through all widget IDs given.
            foreach (var idxWidget in FindWidgets<Container> (context, e.Args)) {

                // Then iterating through all of its children controls.
                // Notice, we cannot use Linq's ToList on Controls collections.
                foreach (Widget idxChildWidget in new ArrayList (idxWidget.Controls)) {

                    // Deleting currently iterated child widget.
                    DeleteWidget (context, e.Args, idxChildWidget);
                }
            }
        }

        /*
         * Helper to delete a widget from Page.
         * Removes all Ajax and lambda events, recursively, for the specified widget, and all of its children widgets.
         */
        void DeleteWidget (
            ApplicationContext context, 
            Node args, 
            Widget widget)
        {
            // Sanity check.
            var parent = widget.Parent as Container;
            if (parent == null)
                throw new LambdaException (
                    "You cannot delete a widget who's parent is not a p5.ajax Container widget",
                    args,
                    context);

            // Removing all Ajax and lambda events for widget recursively.
            DeleteEvents (widget);

            // Removing widget from parent's Controls collection persistently.
            parent.RemoveControlPersistent (widget);
        }

        /*
         * Deleting all events for widget recursively.
         */
        void DeleteEvents (Control control)
        {
            // Checking if currently iterated Control is Widget, since only widgets have Ajax and lambda events.
            if (control is Widget) {

                // Deleting all Ajax events for widget.
                Manager.WidgetAjaxEventStorage.RemoveFromKey1(control.ID);

                // Deleting all lambda events for widget.
                Manager.WidgetLambdaEventStorage.RemoveFromKey2(control.ID);
            }

            // Recursively invoking "self" for all children controls.
            foreach (Control idxChild in control.Controls) {

                // Deleting all events for currently iterated child.
                DeleteEvents (idxChild);
            }
        }
    }
}
