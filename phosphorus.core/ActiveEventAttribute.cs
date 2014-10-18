/*
 * phosphorus five, copyright 2014 - thomas@magixilluminate.com
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System;

namespace phosphorus.core
{
    [AttributeUsage (AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class ActiveEventAttribute : Attribute
    {
        public ActiveEventAttribute ()
        { }

        public string Name {
            get;
            set;
        }
    }
}

