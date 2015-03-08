/*
 * Phosphorus.Five, copyright 2014 - 2015, Mother Earth, Jannah, Gaia - YOU!
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using phosphorus.core;

/// <summary>
///     Main namespace for all file operations in Phosphorus.Five.
/// 
///     Contains all Active Events within the [pf.file.xxx] and [pf.folder.xxx] namespace.
/// 
///     Notice that all of these Active Events can only check for files within the main folder of your application,
///     and they will have the root folder of your application automatically appended, to their files and folder, as they
///     execute.
/// 
///     Due to the above reasons, it is therefor necessary for you to either expose an Active Event in your ApplicationContext
///     returning this root folder, with the name of; [pf.core.application-folder]
/// 
///     This is automatically done for you though, if you use the main phosphorus.application-pool, or the lambda.exe console program
///     as your main application context pool.
/// </summary>
namespace phosphorus.file
{
    /// <summary>
    ///     Class containing common methods for [pf.file.xxx] namespace
    /// </summary>
    public static class Common
    {
        /*
         * contains our root folder
         */
        private static string _rootFolder;

        /// <summary>
        ///     Returns the root folder of application pool back to caller.
        /// </summary>
        /// <returns>the root folder</returns>
        /// <param name="context">application context</param>
        public static string GetRootFolder (ApplicationContext context)
        {
            if (_rootFolder == null) {
                // first time we invoke this bugger, retrieving root folder by raising our
                // "retrieve root folder" Active Event
                var rootNode = new Node ();
                context.Raise ("pf.core.application-folder", rootNode);
                _rootFolder = rootNode.Get<string> (context);

                // making sure we normalize folder separators, to have uniform folder structure
                // for both Linux and Windows
                _rootFolder = _rootFolder.Replace ("\\", "/");
            }
            return _rootFolder;
        }
    }
}