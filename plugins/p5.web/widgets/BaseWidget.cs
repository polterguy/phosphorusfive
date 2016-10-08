/*
 * Phosphorus Five, copyright 2014 - 2016, Thomas Hansen, mr.gaia@gaiasoul.com
 * 
 * This file is part of Phosphorus Five.
 *
 * Phosphorus Five is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
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
using p5.ajax.widgets;
using p5.exp.exceptions;

/// <summary>
///     Main namespace for everything related to widgets
/// </summary>
namespace p5.web.widgets
{
    /// <summary>
    ///     Base class for helper Active Event classes related to widgets
    /// </summary>
    public abstract class BaseWidget
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="p5.web.widgets.BaseWidget"/> class
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="manager">PageManager owning this instance</param>
        public BaseWidget (ApplicationContext context, PageManager manager)
        {
            // Setting WidgetManager for this instance
            Manager = manager;
        }

        /*
         * PageManager for this instance
         */
        protected PageManager Manager {
            get;
            set;
        }

        /*
         * Recursively searches through page for Container with specified id, starting from "startWidget"
         */
        protected T FindControl<T> (string id, Control startWidget) where T : Control
        {
            if (id == null || startWidget == null)
                return null;
            if (startWidget.ID == id)
                return startWidget as T;
            foreach (Control idxChild in startWidget.Controls) {
                var tmpRet = FindControl<T> (id, idxChild);
                if (tmpRet != null)
                    return tmpRet;
            }
            return null;
        }

        /*
         * Helper to retrieve a list of widgets from a Node, throws if widget with specified ID does not exist
         */
        protected IEnumerable<T> FindWidgets<T> (
            ApplicationContext context, 
            Node args, 
            string activeEventName) where T : Control
        {
            // Looping through all Widget IDs supplied by caller, finding widget with specified ID
            foreach (var idxWidgetID in XUtil.Iterate<string> (context, args, true)) {

                // Retrieving Widget with currently iterated ID
                var idxWidget = FindControl<Control>(idxWidgetID, Manager.AjaxPage);

                // Throwing exception if widget does not exist
                if (idxWidget == null)
                    throw new LambdaException(
                        string.Format ("Couldn't find widget with ID '{0}'", idxWidgetID),
                        args, 
                        context);

                // Verifies widget is of requested type
                var retVal = idxWidget as T;
                if (retVal == null) {

                    // Widget was not correct type, figuring out type of widget
                    string typeString = typeof (T).FullName;
                    if (typeof(T).BaseType == typeof(Widget)) {

                        // Using "short version" typename for all widgets inheriting from p5 Ajax Widget
                        typeString = typeString.Substring(typeString.LastIndexOf(".") + 1).ToLower();
                    }

                    // Throwing exception
                    throw new LambdaException(
                        string.Format("You cannot use [{0}] on a Control that is not of type '{1}'", activeEventName, typeString),
                        args,
                        context);
                }

                // Returning widget to caller
                yield return retVal;
            }
        }

        /*
         * Returns typename of specified Control
         */
        protected static string GetTypeName (Control ctrl)
        {
            // Adding type of widget as name, and ID as value
            string typeName = ctrl.GetType().FullName;

            // Making sure we return "condensed typename" if widget type is from p5 Ajax
            if (ctrl is Widget)
                typeName = typeName.Substring(typeName.LastIndexOf(".") + 1).ToLower();

            // Returning typename to caller
            return typeName;
        }
    }
}
