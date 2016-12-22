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
using System.Text;
using System.Reflection;
using p5.exp;
using p5.core;

namespace lambda_exe
{
    /// <summary>
    ///     Main class for the Hyperlambda console executor.
    /// </summary>
    public static class Program
    {
        /// <summary>
        ///     Allows you to write one line of text back to the console.
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Active Event arguments</param>
        [ActiveEvent (Name = "p5.console.write-line")]
        public static void p5_console_write_line (ApplicationContext context, ActiveEventArgs e)
        {
            Console.WriteLine (XUtil.Single<string> (context, e.Args));
        }

        /// <summary>
        ///     Allows you to read one line of text from the console.
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Active Event arguments</param>
        [ActiveEvent (Name = "p5.console.read-line")]
        public static void p5_console_read_line (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value = Console.ReadLine();
        }

        /// <summary>
        ///     Entry point for application.
        /// </summary>
        /// <param name="args">Command line arguments</param>
        public static void Main (string[] args)
        {
            try
            {
                // Checking to see if we're given any arguments at all.
                if (args == null || args.Length == 0) {

                    // No arguments given, outputting instructions, before exiting.
                    OutputInstructions ();

                } else {

                    // Loading the default plugins.
                    LoadDefaultPlugins ();

                    // Handling our command-line arguments, which might load up more plugins.
                    bool immediate;
                    var exeNode = ParseArguments (args, out immediate);

                    // Creating application context after parameters are loaded, since there might be
                    // additional plugins loaded during the parsing of our command line arguments.
                    var context = Loader.Instance.CreateApplicationContext ();

                    // Raising our application startup Active Event, in case there are plugins depending upon it.
                    context.RaiseEvent (".p5.core.application-start", new Node ());

                    // Checking if we're in "immediate mode".
                    if (immediate) {

                        // Starting immediate mode, allowing user to type in Hyperlambda to be evaluated.
                        ImmediateMode (context);

                    } else {

                        // Loads specified Hyperlambda file, pass in arguments, and evaluate it.
                        var convertExeFile = context.RaiseEvent ("p5.io.file.load", new Node ("", exeNode.Value)) [0];
                        exeNode.AddRange (convertExeFile.Children);
                        context.RaiseEvent ("eval", exeNode);
                    }
                }
            }
            catch (Exception err)
            {
                // Writing exception and stack trace to console before exiting.
                Console.WriteLine (err.Message);
                Console.WriteLine ();
                Console.WriteLine (err.StackTrace);
            }
        }

        /// <summary>
        ///     Returns the application base path as value of given args node.
        ///     Some components are dependent upon this Active Event.
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = ".p5.core.application-folder")]
        private static void p5_core_application_folder (ApplicationContext context, ActiveEventArgs e)
        {
            // Retrieving location for currently executed assembly, and making sure we remove the trailing slash "/".
            string retVal = Assembly.GetExecutingAssembly().Location.Replace ("\\", "/");
            if (retVal.EndsWith("/"))
                retVal = retVal.Substring(0, retVal.Length - 1);
            e.Args.Value = retVal;
        }

        /*
         * Loads the default plugins.
         */
        private static void LoadDefaultPlugins ()
        {
            // Initializing the default plugins, including executing assembly, to wire up the console read-line/write-line events.
            Loader.Instance.RegisterAssembly (Assembly.GetExecutingAssembly());
            Loader.Instance.LoadAssembly ("plugins/", "p5.hyperlambda");
            Loader.Instance.LoadAssembly ("plugins/", "p5.types");
            Loader.Instance.LoadAssembly ("plugins/", "p5.lambda");
            Loader.Instance.LoadAssembly ("plugins/", "p5.math");
            Loader.Instance.LoadAssembly ("plugins/", "p5.io");
        }

        /*
         * Starts immediate mode, allowing user to type in a bunch of Hyperlambda, executing lambda when empty line is submitted,
         * and exiting when user types in "exit" as Hyperlambda.
         */ 
        private static void ImmediateMode (ApplicationContext context)
        {
            // Looping until user types "exit".
            while (true) {

                // Retrieving next piece of Hyperlambda to executed from console.
                string hyperlambda = GetNextHyperlambda (context);

                // Checking if user wants to exit program.
                if (hyperlambda.Trim () == "exit") {

                    // Exiting program entirely.
                    break;

                } else if (hyperlambda.Trim () == "") {

                    // User didn't type anything at all.
                    Console.WriteLine ("Nothing to do here, type 'exit' to exit program");

                } else {

                    // Converting Hyperlambda collected above to lambda and evaluating it.
                    Node convert = context.RaiseEvent ("hyper2lambda", new Node ("", hyperlambda));
                    context.RaiseEvent ("eval", convert);
                    Console.WriteLine ();
                }
            }
        }

        /*
         * Retrieves Hyperlambda from console.
         * Basically retrieves a line of input, until the user submits an empty line.
         */
        private static string GetNextHyperlambda (ApplicationContext context)
        {
            // Used as buffer to hold Hyperlambda.
            StringBuilder builder = new StringBuilder();

            // Looping until user types in an empty line.
            while (true) {

                // Making sure user understands where he is.
                Console.Write("p5>");

                // Reading next line of input.
                string line = Console.ReadLine ();

                // Checking what to do according to input given.
                if (line == "")
                    break; // Breaking and executing given code

                // Appending carriage return, to create understandable Hyperlambda.
                builder.Append (line + "\r\n");
            }

            // Returning Hyperlambda to caller.
            return builder.ToString ();
        }

        /*
         * Outputs instructions on how to use the lambda executor to the console.
         */
        private static void OutputInstructions ()
        {
            Console.WriteLine ();
            Console.WriteLine ();
            Console.WriteLine ("********************************************************************************");
            Console.WriteLine ("***    Instructions for Phosphorus Five command line Hyperlambda evaluator   ***");
            Console.WriteLine ("********************************************************************************");
            Console.WriteLine ();
            Console.WriteLine ("The lambda evaluator allows you to evaluate Hyperlambda files or code.");
            Console.WriteLine ();
            Console.WriteLine ("-f Mandatory, unless you're in immediate mode. Is your lambda file, ");
            Console.WriteLine ("   for instance; -f some-lambda-file.hl");
            Console.WriteLine ();
            Console.WriteLine ("-p Allows you to load additional plugins, for instance; -p \"plugins/my.plugin\"");
            Console.WriteLine ("   you can repeat the -p argument as many times as you wish.");
            Console.WriteLine ();
            Console.WriteLine ("-i Starts 'immediate mode', allowing you to write in any Hyperlambda, executing");
            Console.WriteLine ("   your code with an empty line. End immediate mode by supplying 'exit'.");
            Console.WriteLine ();
            Console.WriteLine ("All other arguments are passed into the execution tree of the Hyperlambda file you "
                               + "are executing as a key/value pair, e.g; _var \"x\" creates a new node for you "
                               + "at the top of your execution file called '_var' with the content of 'x'.");
            Console.WriteLine ();
            Console.WriteLine ("The lambda executor contains two Active Events itself, which you can use from "
                               + "your lambda execution files called, \"p5.console.write-line\", and "
                               + "\"p5.console.read-line\" which allows you to write and read input from the console.");
        }

        /*
         * Parses command line arguments.
         * Returns filename to evaluate as value of returned node, alternatively sets "immediate" to true, if user requested
         * to initiate "immediate mode".
         */
        private static Node ParseArguments (string[] args, out bool immediate)
        {
            immediate = false;
            var exeNode = new Node ("input-file");
            var nextIsInput = false;
            var nextIsPlugin = false;

            // Looping through all args.
            foreach (var idx in args) {

                // Checking what type of argument this is.
                if (nextIsInput && exeNode.Value == null) {

                    // This is an input file.
                    exeNode.Value = idx;
                    nextIsInput = false;

                } else if (nextIsInput) {

                    // User tried to supply more than one input file.
                    throw new ArgumentException ("You cannot submit more than one input file to the Hyperlambda evaluator");

                } else if (idx == "-f") {

                    // Next argument is a path to an input Hyperlambda file.
                    nextIsInput = true;

                } else if (nextIsPlugin) {

                    // This is a plugin declaration, meaning a filename/path to a plugin we should load.
                    Loader.Instance.LoadAssembly (idx);
                    nextIsPlugin = false;

                } else if (idx == "-p") {

                    // Next arg is a plugin declaration
                    nextIsPlugin = true;

                } else if (idx == "-i") {

                    // User wants to enter immediate mode.
                    immediate = true;

                } else if (exeNode.Count == 0 || exeNode.LastChild.Value != null) {

                    // Arbitrary argument name passed in from console.
                    exeNode.Add (new Node (idx));

                } else {

                    // Arbitrary argument value passed in from console.
                    exeNode.LastChild.Value = idx;
                }
            }

            // Sanity check.
            if (exeNode.Value == null && !immediate)
                throw new ArgumentException ("No execution file given to lambda evaluator, neither was immediate mode chosen");

            // Returning parsed arguments to caller.
            return exeNode;
        }
    }
}
