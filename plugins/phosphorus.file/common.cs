
/*
 * phosphorus five, copyright 2014 - Mother Earth, Jannah, Gaia
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System;
using System.Reflection;
using phosphorus.core;
using phosphorus.expressions;

namespace phosphorus.file
{
    /// <summary>
    /// class containing common methods in file plugin
    /// </summary>
    public static class common
    {
        private static string _rootFolder;
        public static string GetRootFolder (ApplicationContext context)
        {
            if (_rootFolder == null) {
                Node rootNode = new Node ();
                context.Raise ("pf.core.application-folder", rootNode);
                _rootFolder = rootNode.Get<string> (context);
                if (string.IsNullOrEmpty (_rootFolder)) {
                    _rootFolder = Assembly.GetEntryAssembly ().Location;
                    _rootFolder = _rootFolder.Substring (0, _rootFolder.LastIndexOf ("/") + 1);
                }
            }
            return _rootFolder;
        }
    }
}
