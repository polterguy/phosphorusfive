/*
 * Phosphorus.Five, copyright 2014 - 2015, Mother Earth, Jannah, Gaia - YOU!
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using phosphorus.core;

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