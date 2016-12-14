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
using System.Collections.Generic;
using System.Text.RegularExpressions;
using p5.exp;
using p5.core;
using p5.exp.exceptions;

namespace p5.strings.keywords
{
    /// <summary>
    ///     Class wrapping the [p5.string.split] Active Event.
    /// </summary>
    public static class Split
    {
        /// <summary>
        ///     The [p5.string.split] event, allows you to split a string into multiple strings, either by index, or by each occurrence of a string.
        ///     It will perform necessary conversion.
        ///     Pass in [keep-empty] as boolean true, if you wish to keep empty occurrences, and [trim] as true, if you wish to trim each entity.
        ///     What you wish to split upon, can be passed in as either a single [=], or multiple, which can be a constant, or an expression.
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "split")]
        [ActiveEvent (Name = "p5.string.split")]
        public static void p5_string_split (ApplicationContext context, ActiveEventArgs e)
        {
            // Making sure we clean up and remove all arguments passed in after execution.
            using (new Utilities.ArgsRemover (e.Args, true)) {

                // Figuring out source value of [p5.string.split], and returning early if there is none.
                string source = XUtil.Single<string> (context, e.Args);
                if (source == null)
                    return;

                // Checking if we should explicitly keep empty items.
                StringSplitOptions options = e.Args.GetExChildValue ("keep-empty", context, false) ? 
                    StringSplitOptions.None : 
                    StringSplitOptions.RemoveEmptyEntries;

                // Checking if each item should be trimmed before returned.
                var trim = e.Args.GetExChildValue ("trim", context, false);

                // Retrieving separator objects, which might be multiple integers, multiple strings, or a single regular expression.
                var sepObjects = e.Args.Children
                    .Where (ix => ix.Name == "=")
                    .Select (ix => ix.GetExValue<object> (context))
                    .ToList ();

                // Checking if there were any separators.
                if (sepObjects.Count > 0) {

                    // We have separators, meaning we should actually perform a split operation.
                    RunSplit (context, e.Args, source, options, trim, sepObjects);

                } else {

                    // Special case, no separators, splitting entire string into characters.
                    // Which is actually the only way we can iterate over each character in a string in P5.
                    e.Args.AddRange (source.Select (ix => new Node (ix.ToString ())));
                }
            }
        }

        /*
         * Implementation of actual split operation.
         */
        static void RunSplit (
            ApplicationContext context, 
            Node args, 
            string source, 
            StringSplitOptions options, 
            bool trim, 
            List<object> sepObjects)
        {
            // We have separator objects, now checking type of separator objects.
            if (sepObjects [0] is string) {

                // String split operation.
                StringSplit (context, args, source, options, trim, sepObjects);

            } else if (sepObjects [0] is int) {

                // Integer split operation, sanity check first.
                if (trim || options == StringSplitOptions.None)
                    throw new LambdaException ("You cannot trim or keep empty occurrences when using integer split", args, context);
                IntegerSplit (context, args, source, trim, sepObjects);

            } else if (sepObjects [0] is Regex) {

                // Regex split operation, sanity check first.
                if (options == StringSplitOptions.None)
                    throw new LambdaException ("You cannot keep empty occurrences when using regex split", args, context);
                RegexSplit (context, args, source, trim, sepObjects);

            } else {

                // Oops ...!!
                throw new LambdaException (
                    "Don't know how to split upon anything else but integers, strings and a single regular expressions",
                    args,
                    context);
            }
        }

        /*
         * Splitting upon one or more strings.
         */        static void StringSplit (
            ApplicationContext context,
            Node args,
            string source,
            StringSplitOptions options,
            bool trim,
            List<object> sepObjects)
        {
            var sepStrings = sepObjects.Select (ix => Utilities.Convert<string> (context, ix)).ToList ();
            args.AddRange (
                source.Split (
                    sepStrings.ToArray (),
                    options).Select (ix => new Node (trim ? ix.Trim () : ix)));

            // If string ends with split string, we add an empty item back to caller, if options is to keep empty items.
            if (options == StringSplitOptions.None) {
                bool addEmpty = false;
                foreach (var idx in sepObjects.Select (ix => Utilities.Convert<string> (context, ix))) {
                    if (source.EndsWith (idx)) {
                        addEmpty = true;
                        break;
                    }
                }
                if (addEmpty)
                    args.Add ("");
            }
        }

        /*
         * Integer split operation.
         */        static void IntegerSplit (
            ApplicationContext context, 
            Node args, 
            string source, 
            bool trim, 
            List<object> sepObjects)
        {
            var sepIntegers = sepObjects.Select (ix => Utilities.Convert<int> (context, ix, -1)).ToList ();
            sepIntegers.Sort ();
            var start = 0;
            foreach (var idx in sepIntegers) {
                var ix = source.Substring (start, idx - start);
                args.Add (trim ? ix.Trim () : ix);
                start = idx;
            }
            args.Add (trim ? source.Substring (start).Trim () : source.Substring (start));
        }

        /*
         * Regex split operation.
         */
        static void RegexSplit (
            ApplicationContext context, 
            Node args, 
            string source, 
            bool trim, 
            List<object> sepObjects)
        {
            // Sanity check.
            if (sepObjects.Count > 1)
                throw new LambdaException (
                    "When supplying a regex to [p5.string.split], only one [=] operator is allowed", 
                    args, 
                    context);

            // Doing regex split.
            args.AddRange ((sepObjects [0] as Regex).Split (source).Select (ix => new Node (trim ? ix.Trim () : ix)));
        }
    }
}
