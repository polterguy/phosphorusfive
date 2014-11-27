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
        /// <param name="context">Context.</param>
        /// <param name="e">E.</param>
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
        /// <param name="context">Context.</param>
        /// <param name="e">E.</param>
        [ActiveEvent (Name = "pf.console.write")]
        private static void console_write (ApplicationContext context, ActiveEventArgs e)
        {
            string value = Expression.FormatNode (e.Args);
            if (value != null)
                Console.WriteLine (value);
        }

        // yup, you know this bugger :)
        public static void Main (string[] args)
        {
            // phosphorus.core initialization
            Loader.Instance.LoadAssembly (Assembly.GetExecutingAssembly ());
#if DEBUG
            Loader.Instance.LoadAssembly ("phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("phosphorus.lambda");
            Loader.Instance.LoadAssembly ("phosphorus.file");
#else
            Loader.Instance.LoadAssembly ("plugins/", "phosphorus.hyperlisp");
            Loader.Instance.LoadAssembly ("plugins/", "phosphorus.lambda");
            Loader.Instance.LoadAssembly ("plugins/", "phosphorus.file");
#endif
            ApplicationContext context = Loader.Instance.CreateApplicationContext ();

            Node exeNode = CreateParameters (args);

            // loads file and converts to nodes
            Node loadFile = new Node (string.Empty, exeNode.Value);
            context.Raise ("pf.file.load", loadFile);
            string fileContent = loadFile [0].Get<string> ();
            Node convert = new Node (string.Empty, fileContent);
            context.Raise ("pf.code-2-nodes", convert);

            // appending nodes from lambda file into execution objects
            exeNode.AddRange (convert.Children);
            context.Raise ("lambda", exeNode);
        }

        /*
         * creates our node parameter collection to pass into pf.lambda execution engine
         */
        private static Node CreateParameters (string[] args)
        {
            Node exeNode = new Node ("input-file");
            bool nextIsInput = false;
            foreach (string idx in args) {
                if (nextIsInput && exeNode.Value == null) {
                    exeNode.Value = idx;
                    nextIsInput = false;
                } else if (nextIsInput) {
                    throw new ArgumentException ("you cannot submit more than one execution file to pf.lambda executor");
                } else if (idx == "-f") {
                    nextIsInput = true;
                } else if (exeNode.Count == 0 || exeNode [exeNode.Count - 1].Value != null) {
                    exeNode.Add (new Node (idx));
                } else {
                    exeNode [exeNode.Count - 1].Value = idx;
                }
            }

            if (exeNode.Value == null)
                throw new ArgumentException ("no execution file given to lambda executor. hint; use the '-f' parameter and pass in a valid path to a hyperlisp file to execute it");
            return exeNode;
        }
    }
}
