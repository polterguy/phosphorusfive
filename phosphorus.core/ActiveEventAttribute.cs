/*
 * phosphorus five, copyright 2014 - Mother Earth, Jannah, Gaia
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System;

namespace phosphorus.core
{
    /// <summary>
    /// Active Event attribute
    /// </summary>
    [AttributeUsage (AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class ActiveEventAttribute : Attribute
    {
        /// <summary>
        /// initializes a new instance of the <see cref="phosphorus.core.ActiveEventAttribute"/> class
        /// </summary>
        public ActiveEventAttribute ()
        { }

        /// <summary>
        /// the name of the Active Event
        /// </summary>
        /// <value>the name</value>
        public string Name {
            get;
            set;
        }

        /// <summary>
        /// the name of the Active Event this Active Event is overriding
        /// </summary>
        /// <value>the Active Event override</value>
        public string Overrides {
            get;
            set;
        }
    }
}

