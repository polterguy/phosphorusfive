/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
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
        ///     Returns the application base path as value of given args node
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "p5.core.application-folder", Protection = EventProtection.NativeClosed)]
        private static void p5_core_application_folder (ApplicationContext context, ActiveEventArgs e)
        {
            var path = Assembly.GetExecutingAssembly().Location.Replace ("\\", "//");
            path = path.Substring (0, path.LastIndexOf ("/") + 1);
            e.Args.Value = path;
        }

        /// <summary>
        ///     Allows you to write one line of text back to the console
        /// </summary>
        /// <param name="context">Application context</param>
        /// <param name="e">Active Event arguments</param>
        [ActiveEvent (Name = "p5.console.write", Protection = EventProtection.LambdaClosed)]
        private static void console_write_line (ApplicationContext context, ActiveEventArgs e)
        {
            Console.WriteLine (XUtil.Single<string> (context, e.Args, true));
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

                    // Initializing plugins that must be here in order for lambda executioner to function
                    Loader.Instance.LoadAssembly (Assembly.GetExecutingAssembly ());
                    Loader.Instance.LoadAssembly ("plugins/", "p5.hyperlisp");
                    Loader.Instance.LoadAssembly ("plugins/", "p5.types");
                    Loader.Instance.LoadAssembly ("plugins/", "p5.lambda");
                    Loader.Instance.LoadAssembly ("plugins/", "p5.math");
                    Loader.Instance.LoadAssembly ("plugins/", "p5.io");
                    Loader.Instance.LoadAssembly ("plugins/", "p5.crypto");
                    Loader.Instance.LoadAssembly ("plugins/", "p5.html");
                    Loader.Instance.LoadAssembly ("plugins/", "p5.io.zip");
                    Loader.Instance.LoadAssembly ("plugins/", "p5.net");

                    // Handling our command-line arguments, which might load up more plugins
                    bool immediate;
                    var exeNode = ParseArguments (args, out immediate);

                    // Creating application context after parameters are loaded, since there might be
                    // additional plugins requested during the parsing of our command-line arguments
                    var context = Loader.Instance.CreateApplicationContext ();

                    // Raising our application startup Active Event, in case there are modules loaded depending upon it
                    context.RaiseNative ("p5.core.application-start", new Node ());

                    // Checking if we're in "immediate mode" (which means user can type in Hyperlisp into the console to be evaluated)
                    if (immediate) {

                        // Starting immediate mode, allowing user to type in Hyperlisp to be evaluated
                        ImmediateMode (context);
                    } else {

                        // Loads given file as lambda
                        var convertExeFile = context.RaiseNative ("load-file", new Node ("", exeNode.Value)) [0];

                        // Appending nodes from lambda file into execution objects, and execute lambda file given through command-line arguments
                        exeNode.AddRange (convertExeFile.Children);
                        context.RaiseLambda ("eval", exeNode);
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

        /*
         * Starts immediate mode, allowing user to type in a bunch of Hyperlisp, executing when empty line is submitted
         */ 
        private static void ImmediateMode (ApplicationContext context)
        {
            // Looping until user types "exit"
            while (true) {

                // Retrieving next piece of Hyperlisp to executed from console
                string hyperlisp = GetNextHyperlisp (context);

                // Checking if user wants to exit program
                if (hyperlisp.Trim () == "exit") {

                    // Exiting program entirely
                    break;
                } else if (hyperlisp.Trim () == string.Empty) {

                    // User didn't type anything at all
                    Console.WriteLine ("Nothing to do here, type 'exit' to exit program");
                } else {

                    // Converting Hyperlisp collected above to lambda and executing
                    Node convert = context.RaiseNative ("lisp2lambda", new Node (string.Empty, hyperlisp));
                    context.RaiseLambda ("eval", convert);
                    Console.WriteLine ();
                }
            }
        }

        /*
         * Retrieves Hyperlisp from console
         */
        private static string GetNextHyperlisp (ApplicationContext context)
        {
            // Used as buffer to hold Hyperlisp
            StringBuilder builder = new StringBuilder();

            // Looping until user types in empty line or "exit"
            while (true) {

                // Making sure user understands where he is
                Console.Write("p5>");

                // Reading next line of input
                string line = Console.ReadLine ();

                // Checking what to do according to input given
                if (line == string.Empty)
                    break; // Breaking and executing given code

                // Appending carriage return, to create understandable Hyperlisp
                builder.Append (line + "\r\n");
            }

            // Returning Hyperlisp to caller
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
            Console.WriteLine ("*****    Instructions for Phosphorus Five command line p5.lambda executor  *****");
            Console.WriteLine ("********************************************************************************");
            Console.WriteLine ();
            Console.WriteLine ("The lambda executor allows you to execute Hyperlisp files or code");
            Console.WriteLine ();
            Console.WriteLine ("-f Mandatory, unless you're in immediate mode, and is your lambda file, ");
            Console.WriteLine ("   for instance; -f some-lambda-file");
            Console.WriteLine ();
            Console.WriteLine ("-p Allows you to load additional plugins, for instance; -p \"plugins/my.plugin\"");
            Console.WriteLine ("   you can repeat the -p argument as many times as you wish");
            Console.WriteLine ();
            Console.WriteLine ("-i Starts 'immediate mode', allowing you to write in any Hyperlisp, ending and");
            Console.WriteLine ("   executing your code with an empty line. End immediate mode with 'exit'");
            Console.WriteLine ();
            Console.WriteLine ("All other arguments are passed into the execution tree of the Hyperlisp file you "
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
                    throw new ArgumentException ("You cannot submit more than one execution file to the lambda executor");
                } else if (idx == "-f") {

                    // Next argument is a path to an input Hyperlisp file
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
                } else if (exeNode.Count == 0 || exeNode [exeNode.Count - 1].Value != null) {

                    // Arbitrary argument name passed into Hyperlisp file
                    exeNode.Add (new Node (idx));
                } else {

                    // Arbitrary argument value passed into Hyperlisp file
                    exeNode [exeNode.Count - 1].Value = idx;
                }
            }

            // Basic syntax checking
            if (exeNode.Value == null && !immediate)
                throw new ArgumentException ("No execution file given to lambda executor, neither was immediate mode chosen");

            // Returning lambda to caller
            return exeNode;
        }
    }
}
