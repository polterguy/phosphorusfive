/*
 * Phosphorus.Five, Copyright 2014 - 2015, Thomas Hansen - thomas@magixilluminate.com
 * Phosphorus.Five is licensed under the terms of the MIT license.
 * See the enclosed LICENSE file for details.
 */

using System.Linq;

namespace phosphorus.ajax.core
{
    /// <summary>
    ///     Utility class, containing useful helpers for Phosphorus.Ajax.
    /// </summary>
    public static class Utilities
    {
        /// <summary>
        ///     Determines whether the given name is a legal name for a clr method.
        /// 
        ///     Used to determine whether or not an event handler is a reference to a C# method, or a piece of JavaScript.
        /// </summary>
        /// <returns><c>true</c> if name is legal method name for clr; otherwise, <c>false</c>.</returns>
        /// <param name="name">Name.</param>
        public static bool IsLegalMethodName (string name)
        {
            return name.All (idxChar => "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890_".IndexOf (idxChar) != -1);
        }
    }
}