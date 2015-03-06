/*
 * phosphorus five, copyright 2014 - Mother Earth, Jannah, Gaia
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System;

/*! \mainpage Installing Phosphorus.Five
 *
 * 1. Make sure you've got <a href="http://www.monodevelop.com/">MonoDevelop</a>, <a href="http://xamarin.com/">Xamarin</a> or 
 * <a href="https://www.visualstudio.com/">Visual Studio</a> on your system. If you use MonoDevelop, you'll have to explicitly install
 * xsp in addition, which is a small web-server allowing you to run ASP.NET applications. On Ubuntu, you can use Ubuntu Software Center, 
 * and search for "MonoDevelop" and "xsp 4.0". On Windows, all you need is Visual Studio, alternatively Xamarin. On Mac OSX, Xamarin should 
 * be sufficient.
 * 
 * 2. Then make sure you download the latest stable release of <a href="https://github.com/polterguy/phosphorus-five/releases">Phosphorus.Five</a>,
 * and extract it on your system.
 * 
 * 3. Open "phosphorus.sln" in either Visual Studio, Xamarin or MonoDevelop.
 * 
 * 4. Choose "Start without Debugging" from the Run or Debug menu.
 * 
 * \subsection system42
 * 
 * If you wish to use <a href="https://github.com/polterguy/system42">System42</a> together with Phosphorus.Five, you must remember
 * to edit your "web.config" file, and make sure it says "system42/application-startup.hl" as the value of your "application-startup-file"
 * appSettings key. And that you have downloaded and extracted System42 into your "core/phosphorus.application-startup" folder.
 * 
 */

/** \defgroup ActiveEvents Active Events
 */

namespace phosphorus.core
{
    /// <summary>
    ///     The ActiveEvent attribute is used for marking your methods as Active Events.
    /// 
    ///     Declare an Active Event by adding
    ///     this attribute to your class, and make sure it has a 
    ///     <see cref="phosphorus.core.ActiveEventAttribute.Name">Name</see> property. Optionally, make your 
    ///     Active Event override other existing Active Events by using <see cref="phosphorus.core.ActiveEventAttribute.Overrides">Overrides</see>.
    ///     Active Events to a large extends replaces conventional functions and methods, and allows you to invoke methods by using the 
    ///     <see cref="phosphorus.core.ApplicationContext.Raise">Raise</see> method, and similar constructs. This allows you to completely
    ///     loosely couple together your modules in your application, without having any dependencies between them at all, which facilitates for
    ///     much more <em>Agile</em> software.
    /// 
    ///     To raise an Active Event, use the <see cref="phosphorus.core.ApplicationContext.Raise">Raise</see> method. For instance;
    ///     <pre>
    ///     Node args = new Node ("arg-name", "arg-value");
    ///     _myContext.Raise ("foo.bar", args); // Name of Active Event</pre>
    ///     
    ///     Since you can pass in any <see cref="phosphorus.core.Node">Node</see> hierarchy you wish into your Active Events, you
    ///     can effectively pass in, and return, any number, and types of arguments, from your Active Events.
    ///     \ingroup ActiveEvents
    /// </summary>
    [AttributeUsage (AttributeTargets.Method, AllowMultiple = true)]
    public class ActiveEventAttribute : Attribute
    {
        /// <summary>
        ///     the name of the Active Event
        /// </summary>
        /// <value>the name</value>
        public string Name { get; set; }

        /// <summary>
        ///     the name of the Active Event this Active Event is overriding
        /// </summary>
        /// <value>the Active Event override</value>
        public string Overrides { get; set; }
    }
}