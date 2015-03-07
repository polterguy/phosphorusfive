/*
 * Phosphorus.Five, copyright 2014 - 2015, Mother Earth, Jannah, Gaia - YOU!
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System;

/** \defgroup ActiveEvents Active Events
 */

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
 * PS!<br/>
 * Even though this is the documentation for the C# parts of the system, it also provides extensive documentation for the pure 
 * <em>"pf.lambda"</em> user, who exclusively wishes to use the pf.lambda parts of Phosphorus.Five, which can be seen through classes such as
 * the <see cref="phosphorus.expressions.Expression">Expression</see> class, providing extensive documentation about the pf.lambda
 * expressions for the library.
 * 
 * Also the <see cref="phosphorus.expressions.iterators.Iterator">Iterators</see> are highly useful for the pure pf.lambda user to read up about,
 * since they document every type of pf.lambda iterator you can use when composing your pf.lambda expressions.
 * 
 */

namespace phosphorus.core
{
    /// <summary>
    ///     Attribute used for marking your methods as Active Events.
    /// 
    ///     Declare an Active Event by adding this attribute to your class, and make sure it has a Name property. Optionally, make your 
    ///     Active Event override other existing Active Events by using the Overrides property.
    /// 
    ///     Active Events to a large extends replaces conventional functions and methods, and allows you to invoke methods by using the 
    ///     <see cref="phosphorus.core.ApplicationContext.Raise">Raise</see> method, and similar constructs. This allows you to completely
    ///     loosely couple together your modules in your application, without having any dependencies between them at all, which facilitates for
    ///     much more <em>Agile</em> software.
    /// 
    ///     To raise an Active Event, use the <see cref="phosphorus.core.ApplicationContext.Raise">Raise</see> method. For instance;
    ///     <pre>
    ///     Node args = new Node ("arg-name", "arg-value"); // passsing in parameters
    ///     _myContext.Raise ("foo.bar", args); // Name of Active Event</pre>
    /// 
    ///     Example of how to create a static Active Event handler, that will be automatically invoked every time an Active Event is raised;
    /// 
    ///     <pre>
    ///     [ActiveEvent (Name = "foo.bar")]
    ///     private static void static_foo_bar (ApplicationContext context, ActiveEventArgs e)
    ///     {
    ///         // retrieve args passed in through e.Args
    ///         // return values to caller by manipulating e.Args
    ///     }</pre>
    /// 
    ///     To create instance Active Events, you must make sure somehow that your instance objects are registered within your ApplicationContext,
    ///     by using ApplicationContext.RegisterListeningObject for your object that contains instance Active Events. Here's an example, containing
    ///     all the wire code you'll need to create to do such a thing;
    /// 
    ///     <pre>
    ///     public class foo_bar
    ///     {
    ///         [ActiveEvent (Name = "foo.bar")]
    ///         private void static_foo_bar (ApplicationContext context, ActiveEventArgs e)
    ///         {
    ///             // retrieve args passed in through e.Args
    ///             // return values to caller by manipulating e.Args
    ///         }
    ///     }
    /// 
    ///     // somewhere else in your code, from where you wish to invoke the above method ...
    /// 
    ///     var foo_bar_instance = new foo_bar ();
    ///     myAppContext.RegisterListeningObject (foo_bar_instance);
    ///     myAppContext.Raise ("foo.bar", someNodeArgs);</pre>
    ///     
    ///     Since you can pass in any <see cref="phosphorus.core.Node">Node</see> hierarchy you wish into your Active Events, you
    ///     can effectively pass in, and return, any number, and types of arguments, from your Active Events.
    /// 
    ///     Please notice that you can also create "null Active Events", which are Active Events that will be raised every single time
    ///     any Active Event is being raised, by setting its name to string.Empty (""). This is useful if you have Active Events that are
    ///     supposed to handle a whole range of other Active Events, where you do not know until run-time which Active Events it is supposed
    ///     to actually handle.
    /// 
    ///     The Active Event design pattern in Phosphorus.Five is THE core feature of the library, and what facilitates everything else to
    ///     exist. Active Events to a large extent effectively replaces most other existing Design Patterns on the planet, more or less.
    ///     At the very least, most of the original 23 Design Patterns created by the Gang of Four (GoF) are basically rendered "obsolete"
    ///     when you realize how to use Active Events.
    /// 
    ///     Active Events completely replaces traditional Object Oriented Programming (OOP), since it repllaces the very way you invoke
    ///     functions and methods in your solution, and is the facilitator behind the entirety of Phosphorus.Five, especially the "pf.lambda"
    ///     parts, and its extremely Agile plugin Architecture.
    ///     \ingroup ActiveEvents
    /// </summary>
    [AttributeUsage (AttributeTargets.Method, AllowMultiple = true)]
    public class ActiveEventAttribute : Attribute
    {
        /// <summary>
        ///     The name of your Active Event.
        /// 
        ///     This is the name used to raise your Active Event.
        /// </summary>
        /// <value>the name</value>
        public string Name { get; set; }

        /// <summary>
        ///     The name of the Active Event this Active Event is overriding.
        /// 
        ///     If your Active Event is overriding an existing Active Event, then the name of the base Active Event can be declared
        ///     through this property.
        /// </summary>
        /// <value>the Active Event override</value>
        public string Overrides { get; set; }
    }
}
