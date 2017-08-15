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
using System.Collections.Generic;
using p5.exp;
using p5.core;
using p5.exp.exceptions;

namespace p5.web.widgets
{
    /// <summary>
    ///     Base class for Active Event classes related to widgets.
    ///     Contains common functionality.
    /// </summary>
    public abstract class BaseWidget
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="BaseWidget"/> class.
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="manager">PageManager owning this instance</param>
        protected BaseWidget (ApplicationContext context, PageManager manager)
        {
            // Setting WidgetManager for this instance
            Manager = manager;
        }

        /*
         * PageManager for this instance.
         */
        protected PageManager Manager {
            get;
            set;
        }

        /*
         * Recursively searches through page for a control with the specified id, starting from "startWidget".
         */
        protected T FindControl<T> (string id, Control startControl) where T : Control
        {
            if (id == null || startControl == null)
                return null;
            if (startControl.ID == id)
                return startControl as T;
            foreach (Control idx in startControl.Controls) {
                var tmpRet = FindControl<T> (id, idx);
                if (tmpRet != null)
                    return tmpRet;
            }
            return null;
        }

        /*
         * Helper to iterate a node and retrieve a list of widgets from the Node.
         * Throws if widget with the specified ID does not exist.
         */
        protected IEnumerable<T> FindWidgets<T> (ApplicationContext context, Node args) where T : Control
        {
            // Iterating through all widget IDs supplied by caller, finding widget with specified ID.
            foreach (var idxWidgetID in XUtil.Iterate<string> (context, args)) {

                // Retrieving Widget with currently iterated ID.
                var idxWidget = FindControl<T>(idxWidgetID, Manager.AjaxPage);

                // Throwing exception if widget does not exist.
                if (idxWidget == null)
                    throw new LambdaException(
                        string.Format ("Couldn't find widget with ID '{0}', are you sure it exists and is an actual p5.ajax widget?", idxWidgetID),
                        args, 
                        context);

                // Returning widget to caller.
                yield return idxWidget;
            }
        }
    }
}
