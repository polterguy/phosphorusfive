
/*
 * phosphorus five, copyright 2014 - Mother Earth, Jannah, Gaia
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System;
using System.Collections.Generic;
using phosphorus.core;
using phosphorus.expressions;

namespace phosphorus.lambda
{
    /// <summary>
    /// class wrapping the [remove-override] keyword
    /// </summary>
    public static class remove_override
    {
        /// <summary>
        /// removes zero or more dynamically created overrides
        /// </summary>
        /// <param name="context"><see cref="phosphorus.Core.ApplicationContext"/> for Active Event</param>
        /// <param name="e">parameters passed into Active Event</param>
        [ActiveEvent (Name = "remove-override")]
        private static void lambda_remove_override (ApplicationContext context, ActiveEventArgs e)
        {
            // finding all super events given
            List<Node> supers = new List<Node> (e.Args.FindAll (
                delegate (Node idxNode) {
                    return idxNode.Name == "super";
            }));
            if (supers.Count != 1)
                throw new ArgumentException ("[remove-override] requires one [super] parameter");
            List<string> superEvents = new List<string> (XUtil.Iterate<string> (supers [0], context));

            // iterating through all base overrides to delete
            foreach (var idxBase in XUtil.Iterate<string> (e.Args, context)) {

                // deleting all events with idxBase in list of superEvents
                foreach (var idxSuper in superEvents) {
                    events_common.RemoveOverride (context, idxBase, idxSuper);
                }
            }
        }
    }
}
