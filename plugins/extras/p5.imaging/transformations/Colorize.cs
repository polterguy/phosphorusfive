/*
 * Phosphorus Five, copyright 2014 - 2017, Thomas Hansen, thomas@gaiasoul.com
 * 
 * This file is part of Phosphorus Five.
 *
 * Phosphorus Five is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License version 3, as published by
 * the Free Software Foundation.
 *
 *
 * Phosphorus Five is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with Phosphorus Five.  If not, see <http://www.gnu.org/licenses/>.
 * 
 * If you cannot for some reasons use the GPL license, Phosphorus
 * Five is also commercially available under Quid Pro Quo terms. Check 
 * out our website at http://gaiasoul.com for more details.
 */

using System.Linq;
using System.Drawing;
using System.Drawing.Imaging;
using p5.exp;
using p5.core;
using p5.exp.exceptions;

namespace p5.imaging.transformations
{
    /// <summary>
    ///     Class to help colorize an image
    /// </summary>
    public static class Colorize
    {
        /// <summary>
        ///     Creates a colorized image
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = ".p5.imaging.transformations.colorize")]
        public static void _p5_imaging_transformations_colorize (ApplicationContext context, ActiveEventArgs e)
        {
            Bitmap original = e.Args.Get<Bitmap> (context);
            var destination = new Bitmap (original.Width, original.Height);

            // Sanity check.
            if (e.Args["matrix"] == null || 
                e.Args["matrix"].Count != 5)
                throw new LambdaException ("[colorize] requires a [matrix] argument, with 5 children nodes, each with a comma separated value having 5 floating point values.", e.Args, context);

            using (Graphics g = Graphics.FromImage (destination)) {

                // ColorMatrix creation.
                float[][] colors = e.Args["matrix"].Children.Select (ix => ix.GetExValue<string> (context).Split (',').Select(ix2 => float.Parse(ix2)).ToArray ()).ToArray ();
                var matrix = new ColorMatrix (colors);

                // Image attributes, to apply colors to blit operation.
                var attrs = new ImageAttributes ();
                attrs.SetColorMatrix (matrix);

                // Blitting original image to destination image.
                g.DrawImage (
                    original, 
                    new Rectangle (0, 0, original.Width, original.Height),
                    0, 
                    0, 
                    original.Width, 
                    original.Height, 
                    GraphicsUnit.Pixel, 
                    attrs);

                // Returning new image, notice caller is responsible for disposing both images.
                e.Args.Value = destination;
            }
        }
    }
}
