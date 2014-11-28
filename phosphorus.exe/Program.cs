/*
 * phosphorus five, copyright 2014 - Mother Earth, Jannah, Gaia
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System;
using System.Reflection;
using phosphorus.core;
using phosphorus.lambda;

namespace phosphorus.exe
{
    /// <summary>
    /// main class for pf.lambda executor. this project builds a mono exe file that can execute hyperlisp, and other types
    /// of "pf.lambda" objects
    /// </summary>
    class MainClass
    {
        /// <summary>
        /// helper active event to make it possible to output stuff to console
        /// </summary>
        /// <param name="context">application context</param>
        /// <param name="e">active event arguments</param>
        [ActiveEvent (Name = "pf.console.write-line")]
        [ActiveEvent (Name = "pf.console.output")]
        private static void console_write_line (ApplicationContext context, ActiveEventArgs e)
        {
            string value = Expression.FormatNode (e.Args);
            Console.WriteLine (value ?? "");
        }

        /// <summary>
        /// helper active event to make it possible to output stuff to console
        /// </summary>
        /// <param name="context">application context</param>
        /// <param name="e">active event arguments</param>
        [ActiveEvent (Name = "pf.console.write")]
        private static void console_write (ApplicationContext context, ActiveEventArgs e)
        {
            string value = Expression.FormatNode (e.Args);
            if (value != null)
                Console.WriteLine (value);
        }

        /// <summary>
        /// the entry point of the program, where the program control starts and ends
        /// </summary>
        /// <param name="args">command-line arguments.</param>
        public static void Main (string[] args)
        {
            if (args == null || args.Length == 0) {
                OutputInstructions (); // outputting instructions, and then exiting
            } else {

                // initializing plugins that must be here in order for lambda executioner to function
                Loader.Instance.LoadAssembly (Assembly.GetExecutingAssembly ());
                Loader.Instance.LoadAssembly ("plugins/", "phosphorus.hyperlisp");
                Loader.Instance.LoadAssembly ("plugins/", "phosphorus.lambda");
                Loader.Instance.LoadAssembly ("plugins/", "phosphorus.file");

                // handling our command-line arguments
                string language;
                Node exeNode = ParseArguments (args, out language);

                // creating application context after parameters are loaded, since there might be
                // additional plugins requested during the parsing of our command-line arguments
                ApplicationContext context = Loader.Instance.CreateApplicationContext ();

                // change our default execution language, if we should
                if (!string.IsNullOrEmpty (language))
                    context.Raise ("pf.set-default-execution-language", new Node (string.Empty, language));

                // raising our application startup Active Event, in case there are modules loaded depending upon its
                context.Raise ("pf.application-start", new Node ());

                // loads and convert file to lambda nodes
                Node convertExeFile = context.Raise ("pf.code-2-nodes", new Node (string.Empty, 
                    context.Raise ("pf.file.load", new Node (string.Empty, exeNode.Value)) [0].Get<string> ()));

                // appending nodes from lambda file into execution objects, and execute lambda file given through command-line arguments
                exeNode.AddRange (convertExeFile.Children);
                context.Raise ("lambda", exeNode);
            }
        }

        /*
         * outputs instructions for how to use the lambda executor to the console
         */
        private static void OutputInstructions()
        {
            Console.WriteLine ();
            Console.WriteLine ();
            Console.WriteLine ("********************************************************************************");
            Console.WriteLine ("******   instructions for phosphorus.five command line lambda executor  ********");
            Console.WriteLine ("********************************************************************************");
            Console.WriteLine ();
            Console.WriteLine ("the lambda executor allows you to execute lambda files. "
                + "for it to function, it needs at the very least the \"phosphorus.hyperlisp\","
                + "\"phosphorus.lambda\" and \"phosphorus.file\" plugins in a directory called "
                + "\"plugins\", directly underneath the lambda.exe file itself");
            Console.WriteLine ();
            Console.WriteLine ("command-line arguments;");
            Console.WriteLine ();
            Console.WriteLine ("-f is mandatory, and is your lambda file, e.g; -f some-lambda-file");
            Console.WriteLine ();
            Console.WriteLine ("-p allows you to load additional plugins, e.g; -p \"plugins/my.plugin\"");
            Console.WriteLine ("   you can repeat the -p argument as many times as you wish");
            Console.WriteLine ();
            Console.WriteLine ("-l is optional, and defines your default execution language, e.g; -l xml");
            Console.WriteLine ("   the default lambda execution language is hyperlisp");
            Console.WriteLine ();
            Console.WriteLine ("all other arguments are passed into the execution tree of the lambda file you "
                + "are executing as a key/value pair, e.g; _var \"x\" creates a new node for you "
                + "at the top of your execution file called '_var' with the content of 'x'");
            Console.WriteLine ();
            Console.WriteLine ("the lambda executor contains two Active Events, which you can use from "
                + "your lambda execution files called, \"pf.console.write-line\" and "
                + "\"pf.console.write\", which allows you to write a text to the console, either "
                + "as a line with CR/LF appended at the end, or without CR/LF at the end");
        }

        /*
         * creates our node parameter collection to pass into pf.lambda execution engine
         */
        private static Node ParseArguments (string[] args, out string language)
        {
            language = null;
            Node exeNode = new Node ("input-file");
            bool nextIsInput = false;
            bool nextIsPlugin = false;
            bool nextIsLanguage = false;
            foreach (string idx in args) {
                if (nextIsInput && exeNode.Value == null) {
                    exeNode.Value = idx;
                    nextIsInput = false;
                } else if (nextIsInput) {
                    throw new ArgumentException ("you cannot submit more than one execution file to the lambda executor");
                } else if (idx == "-f") {
                    nextIsInput = true;
                } else if (nextIsPlugin) {
                    Loader.Instance.LoadAssembly (idx);
                    nextIsPlugin = false;
                } else if (idx == "-p") {
                    nextIsPlugin = true;
                } else if (nextIsLanguage) {
                    if (language != null)
                        throw new ArgumentException ("you cannot define execution language twice");
                    language = idx;
                } else if (idx == "-l") {
                    nextIsLanguage = true;
                } else if (exeNode.Count == 0 || exeNode [exeNode.Count - 1].Value != null) {
                    exeNode.Add (new Node (idx));
                } else {
                    exeNode [exeNode.Count - 1].Value = idx;
                }
            }

            if (exeNode.Value == null)
                throw new ArgumentException ("no execution file given to lambda executor");
            return exeNode;
        }
    }
}
