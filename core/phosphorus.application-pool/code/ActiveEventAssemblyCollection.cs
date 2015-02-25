/*
 * phosphorus five, copyright 2014 - Mother Earth, Jannah, Gaia
 * phosphorus five is licensed as mitx11, see the enclosed LICENSE file for details
 */

using System.Configuration;

namespace phosphorus.five.applicationpool.code
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class ActiveEventAssemblyCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement () { return new ActiveEventAssembly (); }
        protected override object GetElementKey (ConfigurationElement element) { return ((ActiveEventAssembly) element).Assembly; }
    }
}