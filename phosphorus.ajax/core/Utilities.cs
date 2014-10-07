/*
 * phosphorus five, copyright 2014 - thomas@magixilluminate.com
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System;

namespace phosphorus.ajax.core
{
    /// <summary>
    /// utility class, containing some useful helpers
    /// </summary>
    public static class Utilities
    {
        /// <summary>
        /// determines whether given name is a legal name for a clr method
        /// </summary>
        /// <returns><c>true</c> if name is legal method name for clr; otherwise, <c>false</c></returns>
        /// <param name="name">Name.</param>
        public static bool IsLegalMethodName (string name)
        {
            foreach (char idxChar in name) {
                if ("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890_".IndexOf (idxChar) == -1)
                    return false;
            }
            return true;
        }
    }
}

