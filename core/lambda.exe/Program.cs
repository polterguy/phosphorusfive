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

using System;
using System.Text;
using System.Reflection;
using p5.exp;
using p5.core;

/// <summary>
///     Main namespace for the lambda.exe console program
/// </summary>
namespace lambda_exe
{
    /// <summary>
    ///     Main class for the lambda console evaluator
    /// </summary>
    public static class Program
    {
        /// <summary>
        ///     Allows you to write one line of text back to the console
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Active Event arguments</param>
        [ActiveEvent (Name = "p5.console.write-line")]
        public static void p5_console_write_line (ApplicationContext context, ActiveEventArgs e)
        {
            Console.WriteLine (XUtil.Single<string> (context, e.Args, true));
        }

        /// <summary>
        ///     Allows you to read one line of text from the console
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Active Event arguments</param>
        [ActiveEvent (Name = "p5.console.read-line")]
        public static void p5_console_read_line (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value = Console.ReadLine();
        }

        /// <summary>
        ///     Entry point of the lambda.exe
        /// </summary>
        /// <param name="args">Command-line arguments</param>
        public static void Main (string[] args)
        {
            try
            {
                // Checking to see if we're given any arguments at all
                if (args == null || args.Length == 0) {

                    // No arguments given, outputting instructions, for then to exit
                    OutputInstructions ();
                } else {

                    // Loading the default plugins
                    LoadDefaultPlugins ();

                    // Handling our command-line arguments, which might load up more plugins
                    bool immediate;
                    var exeNode = ParseArguments (args, out immediate);

                    // Creating application context after parameters are loaded, since there might be
                    // additional plugins requested during the parsing of our command-line arguments
                    var context = Loader.Instance.CreateApplicationContext ();

                    // Raising our application startup Active Event, in case there are modules loaded depending upon it
                    context.Raise (".p5.core.application-start", new Node ());

                    // Checking if we're in "immediate mode" (which means user can type in Hyperlambda into the console to be evaluated)
                    if (immediate) {

                        // Starting immediate mode, allowing user to type in Hyperlambda to be evaluated
                        ImmediateMode (context);
                    } else {

                        // Loads given file as lambda
                        var convertExeFile = context.Raise ("load-file", new Node ("", exeNode.Value)) [0];

                        // Appending nodes from lambda file into execution objects, and execute lambda file given through command-line arguments
                        exeNode.AddRange (convertExeFile.Children);
                        context.Raise ("eval", exeNode);
                    }
                }
            }
            catch (Exception err)
            {
                // Writing exception and stack trace to console
                Console.WriteLine (err.Message);
                Console.WriteLine ();
                Console.WriteLine (err.StackTrace);
            }
        }

        /// <summary>
        ///     Returns the application base path as value of given args node
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = ".p5.core.application-folder")]
        private static void p5_core_application_folder (ApplicationContext context, ActiveEventArgs e)
        {
            string retVal = Assembly.GetExecutingAssembly().Location.Replace ("\\", "/");
            if (retVal.EndsWith("/"))
                retVal = retVal.Substring(0, retVal.Length - 1);
            e.Args.Value = retVal;
        }

        /*
         * Loads the default plugins
         */
        private static void LoadDefaultPlugins ()
        {
            // Initializing plugins that must be here in order for lambda executioner to function
            Loader.Instance.LoadAssembly (Assembly.GetExecutingAssembly());
            Loader.Instance.LoadAssembly ("plugins/", "p5.hyperlambda");
            Loader.Instance.LoadAssembly ("plugins/", "p5.types");
            Loader.Instance.LoadAssembly ("plugins/", "p5.lambda");
            Loader.Instance.LoadAssembly ("plugins/", "p5.math");
            Loader.Instance.LoadAssembly ("plugins/", "p5.io");
            Loader.Instance.LoadAssembly ("plugins/", "p5.crypto");
            Loader.Instance.LoadAssembly ("plugins/", "p5.html");
            Loader.Instance.LoadAssembly ("plugins/", "p5.io.zip");
            Loader.Instance.LoadAssembly ("plugins/", "p5.net");
        }

        /*
         * Starts immediate mode, allowing user to type in a bunch of Hyperlambda, executing when empty line is submitted
         */ 
        private static void ImmediateMode (ApplicationContext context)
        {
            // Looping until user types "exit"
            while (true) {

                // Retrieving next piece of Hyperlambda to executed from console
                string hyperlambda = GetNextHyperlisp (context);

                // Checking if user wants to exit program
                if (hyperlambda.Trim () == "exit") {

                    // Exiting program entirely
                    break;
                } else if (hyperlambda.Trim () == "") {

                    // User didn't type anything at all
                    Console.WriteLine ("Nothing to do here, type 'exit' to exit program");
                } else {

                    // Converting Hyperlambda collected above to lambda and executing
                    Node convert = context.Raise ("hyper2lambda", new Node ("", hyperlambda));
                    context.Raise ("eval", convert);
                    Console.WriteLine ();
                }
            }
        }

        /*
         * Retrieves Hyperlambda from console
         */
        private static string GetNextHyperlisp (ApplicationContext context)
        {
            // Used as buffer to hold Hyperlambda
            StringBuilder builder = new StringBuilder();

            // Looping until user types in empty line or "exit"
            while (true) {

                // Making sure user understands where he is
                Console.Write("p5>");

                // Reading next line of input
                string line = Console.ReadLine ();

                // Checking what to do according to input given
                if (line == "")
                    break; // Breaking and executing given code

                // Appending carriage return, to create understandable Hyperlambda
                builder.Append (line + "\r\n");
            }

            // Returning Hyperlambda to caller
            return builder.ToString ();
        }

        /*
         * Outputs instructions for how to use the lambda executor to the console
         */
        private static void OutputInstructions ()
        {
            Console.WriteLine ();
            Console.WriteLine ();
            Console.WriteLine ("********************************************************************************");
            Console.WriteLine ("*****    Instructions for Phosphorus Five command line p5 lambda executor  *****");
            Console.WriteLine ("********************************************************************************");
            Console.WriteLine ();
            Console.WriteLine ("The lambda executor allows you to execute Hyperlambda files or code");
            Console.WriteLine ();
            Console.WriteLine ("-f Mandatory, unless you're in immediate mode, and is your lambda file, ");
            Console.WriteLine ("   for instance; -f some-lambda-file");
            Console.WriteLine ();
            Console.WriteLine ("-p Allows you to load additional plugins, for instance; -p \"plugins/my.plugin\"");
            Console.WriteLine ("   you can repeat the -p argument as many times as you wish");
            Console.WriteLine ();
            Console.WriteLine ("-i Starts 'immediate mode', allowing you to write in any Hyperlambda, ending and");
            Console.WriteLine ("   executing your code with an empty line. End immediate mode with 'exit'");
            Console.WriteLine ();
            Console.WriteLine ("All other arguments are passed into the execution tree of the Hyperlambda file you "
                               + "are executing as a key/value pair, e.g; _var \"x\" creates a new node for you "
                               + "at the top of your execution file called '_var' with the content of 'x'");
            Console.WriteLine ();
            Console.WriteLine ("The lambda executor contains one Active Events itself, which you can use from "
                               + "your lambda execution files called, \"p5.console.write-line\", which allows "
                               + "you to write text to the console");
        }

        /*
         * Parses command line arguments
         */
        private static Node ParseArguments (string[] args, out bool immediate)
        {
            immediate = false;
            var exeNode = new Node ("input-file");
            var nextIsInput = false;
            var nextIsPlugin = false;

            // Looping through all args
            foreach (var idx in args) {

                // Checking what type of argument this is
                if (nextIsInput && exeNode.Value == null) {

                    // This is an input file
                    exeNode.Value = idx;
                    nextIsInput = false;
                } else if (nextIsInput) {

                    // User tried to supply more than one input file
                    throw new ArgumentException (
                        "You cannot submit more than one execution file to the lambda executor");
                } else if (idx == "-f") {

                    // Next argument is a path to an input Hyperlambda file
                    nextIsInput = true;
                } else if (nextIsPlugin) {

                    // This is a plugin declaration, path to a plugin
                    Loader.Instance.LoadAssembly (idx);
                    nextIsPlugin = false;
                } else if (idx == "-p") {

                    // Next arg is a plugin declaration
                    nextIsPlugin = true;
                } else if (idx == "-i") {

                    // User wants to enter immediate mode
                    immediate = true;
                } else if (exeNode.Children.Count == 0 || exeNode [exeNode.Children.Count - 1].Value != null) {

                    // Arbitrary argument name passed into Hyperlambda file
                    exeNode.Add (new Node (idx));
                } else {

                    // Arbitrary argument value passed into Hyperlambda file
                    exeNode [exeNode.Children.Count - 1].Value = idx;
                }
            }

            // Basic syntax checking
            if (exeNode.Value == null && !immediate)
                throw new ArgumentException (
                    "No execution file given to lambda executor, neither was immediate mode chosen");

            // Returning lambda to caller
            return exeNode;
        }
    }
}
