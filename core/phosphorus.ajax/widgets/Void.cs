/*
 * phosphorus five, copyright 2014 - Mother Earth, Jannah, Gaia
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System;
using System.Web.UI;

namespace phosphorus.ajax.widgets
{
    /// <summary>
    ///     A widget that has no children, and no content, and is self closed.
    /// 
    ///     A Widget that is neither a Container, nor a Literal, meaning you can not set neither its innerValue
    ///     nor its children controls. This type of widget is useful for widgets that does not allow for any content,
    ///     such as input HTML elements of type "button", and "checkbox", etc.
    /// 
    ///     By default, this widget is rendered using RenderingType.open, which means it won't be closed automatically,
    ///     neither by adding an end HTML tag, nor by closing the tag immediately. If you wish to be XHTML compliant,
    ///     you should probably change this logic to let it close immediately, using RenderingType.immediate as its 
    ///     RenderType property value.
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