/*
 * Phosphorus.Five, copyright 2014 - 2015, Mother Earth, Jannah, Gaia - YOU!
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System;
using System.Web.UI;

namespace phosphorus.ajax.widgets
{
    /// <summary>
    ///     A widget that has neither children controls, nor content.
    /// 
    ///     A widget that is neither a Container, nor a Literal, meaning you can set neither its innerValue,
    ///     nor its children controls. This type of widget is useful for widgets that does not allow for any content,
    ///     such as input HTML elements of type "button", and "checkbox", etc.
    /// 
    ///     By default, this widget is rendered using <em>"RenderingType.open"</em>, which means it won't be closed automatically,
    ///     neither by adding an end HTML tag, nor by closing the tag immediately. If you wish to be XHTML compliant,
    ///     you should probably change this logic to let it close either immediately, using <em>"RenderingType.immediate"</em> as its 
    ///     RenderType property value, or closing it normally, by using <em>"RenderingType.normal"</em>.
    /// </summary>
    [ViewStateModeById]
    public class Void : Widget
    {
        public Void ()
        {
            RenderType = RenderingType.open;
        }

        // overridden to throw an exception if user tries to explicitly set the innerValue attribute of this control
        public override string this [string name]
        {
            get { return base [name]; }
            set {
                if (name == "innerValue")
                    throw new ArgumentException ("you cannot set the 'innerValue' property of a Void widget");
                base [name] = value;
            }
        }

        protected override bool HasContent
        {
            get { return false; }
        }

        protected override void AddedControl (Control control, int index)
        {
            throw new ArgumentException ("Void widget cannot have children");
        }
    }
}