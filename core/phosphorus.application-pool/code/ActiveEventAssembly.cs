/*
 * phosphorus five, copyright 2014 - Mother Earth, Jannah, Gaia
 * phosphorus five is licensed as mitx11, see the enclosed LICENSE file for details
 */

using System.Configuration;

namespace phosphorus.five.applicationpool.code
{
    public class ActiveEventAssembly : ConfigurationElement
    {
        [ConfigurationProperty ("assembly", IsRequired = true)]
        public string Assembly
        {
            get { return this ["assembly"] as string; }
        }
    }
}