/*
 * phosphorus five, copyright 2014 - Mother Earth, Jannah, Gaia
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System;
using System.Reflection;
using System.Collections.Generic;
using phosphorus.core;

namespace phosphorus.lambda
{
    /// <summary>
    /// class wrapping an override from one Active Event to another
    /// </summary>
    public class Override
    {
        /// <summary>
        /// initializes a new instance of the <see cref="phosphorus.lambda.Override"/> class
        /// </summary>
        /// <param name="newActiveEvent">name of the Active Event that is the override</param>
        /// <param name="oldActiveEvent">name of the Active Event that is overridden</param>
        public Override (string overrideEvent, string overriddenEvent)
        {
        }

        /// <summary>
        /// name of the override Active Event
        /// </summary>
        /// <value>the override Active Event</value>
        public string OverrideEvent {
            get;
            set;
        }
        
        /// <summary>
        /// name of the overridden Active Event
        /// </summary>
        /// <value>the overridden Active Event</value>
        public string OverriddenEvent {
            get;
            set;
        }
    }
}

