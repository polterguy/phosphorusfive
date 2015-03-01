/*
 * phosphorus five, copyright 2014 - Mother Earth, Jannah, Gaia
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System;
using System.Text;
using System.Reflection;
using phosphorus.core;
using phosphorus.expressions;

// ReSharper disable UnusedMember.Local

namespace phosphorus.exe
{
    /// <summary>
    ///     main class for pf.lambda executor. this project builds a mono exe file that can execute hyperlisp, and other types
    ///     of "pf.lambda" objects
    /// </summary>
    internal static class MainClass
    {
        /// <summary>
        ///     Returns the application base path as value of given args node. 
        ///     Necessary to make our [pf.file.xxx] namespace work correctly.
        /// </summary>
        /// <param name="context">Application context Active Event is raised within</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "pf.core.application-folder")]
        private static void pf_core_application_folder (ApplicationContext context, ActiveEventArgs e)
        {
            string path = Assembly.GetExecutingAssembly().Location;
            path = path.Replace ("\\", "//");
            path = path.Substring (0, path.LastIndexOf ("/") + 1);
            e.Args.Value = path;
        }

        /// <summary>
        ///     Allows you to write one line of text back to the console, appending CR/LF at the end automatically
        /// </summary>
        /// <param name="context">application context</param>
        /// <param name="e">active event arguments</param>
        [ActiveEvent (Name = "pf.console.write-line")]
        private static void console_write_line (ApplicationContext context, ActiveEventArgs e)
        {
            var value = XUtil.Single<string> (e.Args, context, "");
            Console.WriteLine (value);
        }

        /// <summary>
        ///     Allows you to write any text back to the console, without ending your text with CR/LF
        /// </summary>
        /// <param name="context">application context</param>
        /// <param name="e">active event arguments</param>
        [ActiveEvent (Name = "pf.console.write")]
        private static void console_write (ApplicationContext context, ActiveEventArgs e)
        {
            var value = XUtil.Single<string> (e.Args, context, "");
            Console.Write (value);
        }

        /// <summary>
        ///     The entry point of the program, where the program control starts and ends
        /// </summary>
        /// <param name="args">command-line arguments.</param>
        public static void Main (string[] args)
        {
            try {
                if (args == null || args.Length == 0) {

                    // outputting instructions, and then exiting
                    OutputInstructions ();
                } else {

                    // initializing plugins that must be here in order for lambda executioner to function
                    Loader.Instance.LoadAssembly (Assembly.GetExecutingAssembly ());
                    Loader.Instance.LoadAssembly ("plugins/", "phosphorus.hyperlisp");
                    Loader.Instance.LoadAssembly ("plugins/", "phosphorus.lambda");
                    Loader.Instance.LoadAssembly ("plugins/", "phosphorus.file");

                    // handling our command-line arguments
                    bool immediate;
                    var exeNode = ParseArguments (args, out immediate);

                    // creating application context after parameters are loaded, since there might be
                    // additional plugins requested during the parsing of our command-line arguments
                    var context = Loader.Instance.CreateApplicationContext ();

                    // raising our application startup Active Event, in case there are modules loaded depending upon it
                    context.Raise ("pf.core.application-start", new Node ());

                    if (immediate) {

                        // starting immediate mode
                        ImmediateMode (context);
                    } else {

                        // loads and convert file to lambda nodes
                        var convertExeFile = 
                            context.Raise ("pf.hyperlisp.hyperlisp2lambda", 
                                           new Node (string.Empty, 
                                           context.Raise ("pf.file.load", 
                                               new Node (string.Empty, exeNode.Value)) [0].Get<string> (context)));

                        // appending nodes from lambda file into execution objects, and execute lambda file given through command-line arguments
                        exeNode.AddRange (convertExeFile.Children);
                        context.Raise ("lambda", exeNode);
                    }
                }
            } catch (Exception err) {
                while (err.InnerException != null)
                    err = err.InnerException;
                Console.WriteLine (err.Message);
                Console.WriteLine (err.StackTrace);
            }
        }

        /*
         * starts immediate mode, allowing user to type in a bunch of Hyperlisp, executing when empty line is submitted
         */ 
        private static void ImmediateMode (ApplicationContext context)
        {
            while (true) {
                StringBuilder builder = new StringBuilder ();
                while (true) {
                    Console.Write("pf>");
                    string line = Console.ReadLine ();
                    if (line == string.Empty)
                        break; // breaking and executing given code
                    if (line == "exit") {

                        // discarding input and signaling exit of outer loop
                        builder = new StringBuilder ("exit");
                        break;
                    }
                    builder.Append (line + "\r\n");
                }
                string hyperlisp = builder.ToString ();
                if (hyperlisp == "exit") {
                    break;
                } else if (hyperlisp.Trim () == string.Empty) {
                    Console.WriteLine ("nothing to do here");
                } else {
                    Node convert = context.Raise ("pf.hyperlisp.hyperlisp2lambda", new Node (string.Empty, hyperlisp));
                    context.Raise ("lambda", convert);
                    Console.WriteLine ();
                }
            }
        }

        /*
         * outputs instructions for how to use the lambda executor to the console
         */
        private static void OutputInstructions ()
        {
            Console.WriteLine ();
            Console.WriteLine ();
            Console.WriteLine ("********************************************************************************");
            Console.WriteLine ("*****    Instructions for Phosphorus.Five command line pf.lambda executor  *****");
            Console.WriteLine ("********************************************************************************");
            Console.WriteLine ();
            Console.WriteLine ("The lambda executor allows you to execute pf.lambda Hyperlisp files.");
            Console.WriteLine ();
            Console.WriteLine ("-f is mandatory, and is your lambda file, for instance; -f some-lambda-file");
            Console.WriteLine ();
            Console.WriteLine ("-p allows you to load additional plugins, for instance; -p \"plugins/my.plugin\"");
            Console.WriteLine ("   you can repeat the -p argument as many times as you wish");
            Console.WriteLine ();
            Console.WriteLine ("-i starts 'immediate mode', allowing you to write in any Hyperlisp, ending and");
            Console.WriteLine ("executing your code with an empty line. End immediate mode with 'exit'");
            Console.WriteLine ();
            Console.WriteLine ("All other arguments are passed into the execution tree of the lambda file you "
                               + "are executing as a key/value pair, e.g; _var \"x\" creates a new node for you "
                               + "at the top of your execution file called '_var' with the content of 'x'");
            Console.WriteLine ();
            Console.WriteLine ("The lambda executor contains two Active Events itself, which you can use from "
                               + "your lambda execution files called, \"pf.console.write-line\", and "
                               + "\"pf.console.write\", which allows you to write a text to the console, either "
                               + "as a line with CR/LF appended at the end, or without CR/LF at the end");
        }

        /*
         * creates our node parameter collection to pass into pf.lambda execution engine
         */
        private static Node ParseArguments (string[] args, out bool immediate)
        {
            immediate = false;
            var exeNode = new Node ("input-file");
            var nextIsInput = false;
            var nextIsPlugin = false;
            foreach (var idx in args) {
                if (nextIsInput && exeNode.Value == null) {
                    exeNode.Value = idx;
                    nextIsInput = false;
                } else if (nextIsInput) {
                    throw new ArgumentException ("You cannot submit more than one execution file to the lambda executor. You can though create one Hyperlisp file, that executes multiple files, and execute this file.");
                } else if (idx == "-f") {
                    nextIsInput = true;
                } else if (nextIsPlugin) {
                    Loader.Instance.LoadAssembly (idx);
                    nextIsPlugin = false;
                } else if (idx == "-p") {
                    nextIsPlugin = true;
                } else if (idx == "-i") {
                    immediate = true;
                } else if (exeNode.Count == 0 || exeNode [exeNode.Count - 1].Value != null) {
                    exeNode.Add (new Node (idx));
                } else {
                    exeNode [exeNode.Count - 1].Value = idx;
                }
            }

            if (exeNode.Value == null && !immediate)
                throw new ArgumentException ("No execution file given to lambda executor, neither was immediate mode chosen");
            return exeNode;
        }
    }
}
