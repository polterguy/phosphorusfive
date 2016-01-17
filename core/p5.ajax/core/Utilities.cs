/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System.Linq;

namespace p5.ajax.core
{
    /// <summary>
    ///     Utility class, containing useful helpers for p5.ajax
    /// </summary>
    public static class Utilities
    {
        /// <summary>
        ///     Determines whether the given name is a legal name for a clr method
        /// </summary>
        /// <returns><c>true</c> if name is legal method name for clr; otherwise, <c>false</c></returns>
        /// <param name="name">Name</param>
        public static bool IsLegalMethodName (string name)
        {
            return name.All (idxChar => "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890_".IndexOf (idxChar) != -1);
        }
    }
}