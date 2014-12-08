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
    /// class wrapping a dynamically created Active Event
    /// </summary>
    public class Event
    {
        /// <summary>
        /// initializes a new instance of the <see cref="phosphorus.lambda.Event"/> class
        /// </summary>
        /// <param name="name">name of Active Event</param>
        /// <param name="lambda">lambda object to execute when Active Event is raised</param>
        public Event (string name, Node lambda)
        {
            Name = name;
            Lambda = lambda;
        }

        /// <summary>
        /// name of the Active Event
        /// </summary>
        /// <value>the name of the Active Event</value>
        public string Name {
            get;
            set;
        }

        /// <summary>
        /// actual lambda object that executes when event is raised
        /// </summary>
        /// <value>lambda code</value>
        public Node Lambda {
            get;
            set;
        }
    }
}

