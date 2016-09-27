/*
 * Phosphorus Five, copyright 2014 - 2016, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System.Text;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using p5.exp;
using p5.core;
using p5.exp.exceptions;

namespace p5.strings.keywords
{
    /// <summary>
    ///     Class wrapping the [replace] keyword in p5 lambda.
    /// </summary>
    public static class Replace
    {
        /// <summary>
        ///     The [replace] keyword, allows you to replace occurrencies of a string or regular expression with another strings
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "replace")]
        public static void lambda_replace (ApplicationContext context, ActiveEventArgs e)
        {
            // Making sure we clean up and remove all arguments passed in after execution
            using (new Utilities.ArgsRemover (e.Args)) {

                // Figuring out source value
                string source = XUtil.Single<string> (context, e.Args, true);
                if (source == null)
                    return; // Nothing to check

                // Retrieving what source to look for
                var src = XUtil.Source (context, e.Args, e.Args, "src", new List<string> (new string[] { "dest" }));
                var dest = XUtil.Source (context, e.Args, e.Args, "dest", null, 1);
                if (dest.Count > 1)
                    throw new LambdaException ("There can only be one 'destination' for [replace]", e.Args, context);
                var with = dest.Count == 1 ? Utilities.Convert<string> (context, dest[0]) : "";

                foreach (var what in src) {

                    // Checking type of what
                    if (what is Regex) {

                        // Regular expression search
                        var regex = what as Regex;
                        e.Args.Value = regex.Replace (source, with);
                    } else {

                        // Simple string search, checking if this is a [not-of] operation, or a [what] operation
                        if (what != null) {

                            // Simple version
                            e.Args.Value = source.Replace (Utilities.Convert<string> (context, what), with);
                        } else {

                            // Replacing everything we cannot find in [not-of] with whatever we find in [with] 
                            // (or empty if no with is given)
                            var retVal = new StringBuilder ();
                            for (int idxNo = 0; idxNo < source.Length; idxNo++) {
                                retVal.Append (with);
                            }
                            e.Args.Value = retVal.ToString ();
                        }
                    }
                    source = e.Args.Get<string> (context);
                }
            }
        }
    }
}
