/*
 * Phosphorus Five, copyright 2014 - 2017, Thomas Hansen, thomas@gaiasoul.com
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
using System.Diagnostics;
using p5.exp;
using p5.core;
using p5.exp.exceptions;

namespace p5.lambda.keywords.extras
{
    /// <summary>
    ///     Executes one or more file(s).
    /// </summary>
    public static class Execute
    {
        /// <summary>
        ///     Executes one or more file(s) from local disc.
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "p5.system.platform.execute-file")]
        public static void p5_system_platform_execute_file (ApplicationContext context, ActiveEventArgs e)
        {
            // House cleaning
            using (new ArgsRemover (e.Args, true)) {

                // Getting root folder.
                var rootFolder = context.RaiseEvent (".p5.core.application-folder").Get<string> (context);

                // Checking if we should wait for process to exit, or simply create a "daemon".
                var waitForExit = e.Args.GetExChildValue ("wait-for-exit", context, true);

                // Looping through each file argument
                foreach (var idxFile in XUtil.Iterate<string> (context, e.Args)) {

                    // Retrieving actual system path.
                    var file = context.RaiseEvent (".p5.io.unroll-path", new Node ("", idxFile)).Get<string> (context);

                    // Making sure we have access rights to execute file, making sure we default access to false.
                    var node = new Node ("", false);
                    node.Add ("filter", "p5.system.platform.execute-file");
                    node.Add ("path", file);
                    var access = context.RaiseEvent ("p5.auth.has-access", node).Get<bool> (context);
                    if (!access)
                        throw new LambdaException (string.Format ("You don't have access to execute file '{0}'", file), e.Args, context);

                    // Retrieving current folder, defaulting to folder where file being executed is.
                    var workingFolder = rootFolder + e.Args.GetExChildValue (
                        "working-folder", 
                        context, 
                        file.Substring (0, file.LastIndexOf ("/", StringComparison.InvariantCulture) + 1));

                    // Executing file, and making sure we return whatever result it creates.
                    e.Args.Add (idxFile, ExcuteScript (rootFolder + file, e.Args, context, waitForExit, workingFolder)); 
                }
            }
        }

        /*
         * Helper for above.
         */
        private static string ExcuteScript (string file, Node args, ApplicationContext context, bool waitForExit, string workingFolder)
        {
            // Creating process info pointing to bash, making sure we capture results.
            ProcessStartInfo procInfo = new ProcessStartInfo ();

            // Checking operating system version
            if (Environment.OSVersion.Platform == PlatformID.Unix || Environment.OSVersion.Platform == PlatformID.MacOSX) {

                // xNix operating system type.
                procInfo.FileName = "/bin/bash";
                procInfo.Arguments = file;

            } else {

                // Windows something.
                procInfo.FileName = "cmd.exe";
                procInfo.WindowStyle = ProcessWindowStyle.Hidden;
                procInfo.Arguments = "/C " + file;
            }

            procInfo.ErrorDialog = false;
            procInfo.UseShellExecute = false;
            procInfo.WorkingDirectory = workingFolder;
            procInfo.RedirectStandardOutput = true;
            procInfo.RedirectStandardError = true;
            procInfo.CreateNoWindow = true;

            // Passing in file as argument, starting process, and waiting for process to return.
            Process proc = Process.Start (procInfo);

            // Checking if we're supposed to start a "daemon", or wait for process to finish its job.
            if (waitForExit) {

                // Waiting for process to exit.
                string output = proc.StandardOutput.ReadToEnd ();
                string error = proc.StandardError.ReadToEnd ();
                proc.WaitForExit ();

                // Verifying execution was a success, and if not, throwing an exception.
                if (proc.ExitCode != 0) {
                    throw new LambdaException (error, args, context);
                }

                // Returning output.
                return output;

            } else {

                // "Daemon" mode.
                return null;
            }
        }
    }
}
