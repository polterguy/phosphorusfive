/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details.
 */

using System;
using System.Linq;
using System.Web.UI;
using System.Collections;
using System.Collections.Generic;
using p5.exp;
using p5.core;
using p5.ajax.core;
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
         * Helper to figure out if Active Event is protected or not
         */
        protected bool CanOverrideEventInLambda (ApplicationContext context, string evt)
        {
            // Verifying Active Event is not protected, first checking native handlers
            if (context.HasEvent (evt)) {

                // There exist a native handler for this Active Event, now getting protection level of event
                if (context.GetEventProtection(evt) == EventProtection.LambdaOpen)
                    return true;
            }

            // Checking if protected events contains given event name, and if so, returning true, else returning false
            return context.RaiseNative("p5.lambda.get-protected-events")[evt] == null;
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
