/*
 * Phosphorus.Five, copyright 2014 - 2015, Mother Earth, Jannah, Gaia - YOU!
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System.Collections.Generic;
using phosphorus.core;
using phosphorus.expressions;
using phosphorus.expressions.exceptions;

// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedMember.Global

namespace phosphorus.lambda.events
{
    /// <summary>
    ///     class wrapping the [remove-override] keyword
    /// </summary>
    public static class RemoveOverride
    {
        /// <summary>
        ///     removes zero or more dynamically created overrides
        /// </summary>
        /// <param name="context">Application context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "remove-override")]
        private static void lambda_remove_override (ApplicationContext context, ActiveEventArgs e)
        {
            // finding all super events given
            var supers = new List<Node> (e.Args.FindAll (idxNode => idxNode.Name == "super"));
            if (supers.Count != 1)
                throw new LambdaException ("[remove-override] requires exactly one [super] parameter", e.Args, context);
            var superEvents = new List<string> (XUtil.Iterate<string> (supers [0], context));

            // iterating through all base overrides to delete
            foreach (var idxBase in XUtil.Iterate<string> (e.Args, context)) {
                // deleting all events with idxBase in list of superEvents
                foreach (var idxSuper in superEvents) {
                    EventsCommon.RemoveOverride (context, idxBase, idxSuper);
                }
            }
        }
    }
}