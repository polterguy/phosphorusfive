/*
 * phosphorus five, copyright 2014 - Mother Earth, Jannah, Gaia
 * phosphorus five is licensed as mitx11, see the enclosed LICENSE file for details
 */

using System;
using System.Configuration;

namespace phosphorus.five.applicationpool
{
    public class ActiveEventAssemblyCollection : ConfigurationElementCollection
    {
        public ActiveEventAssembly this [int index] {
            get {
                return base.BaseGet(index) as ActiveEventAssembly;
            }
            set {
                if (base.BaseGet (index) != null) {
                    base.BaseRemoveAt (index);
                }
                this.BaseAdd (index, value);
            }
        }

        protected override ConfigurationElement CreateNewElement ()
        {
            return new ActiveEventAssembly ();
        }

        protected override object GetElementKey (ConfigurationElement element)
        {
            return ((ActiveEventAssembly)element).Assembly;
        } 
    }
}

