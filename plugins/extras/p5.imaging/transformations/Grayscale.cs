/*
 * Phosphorus Five, copyright 2014 - 2016, Thomas Hansen, thomas@gaiasoul.com
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

using System.Drawing;
using System.Drawing.Imaging;
using p5.core;

namespace p5.imaging.transformations
{
    /// <summary>
    ///     Class to help create a grayscale image
    /// </summary>
    public static class Grayscale
    {
        /// <summary>
        ///     Creates a grayscale image
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = ".p5.imaging.transformations.grayscale")]
        public static void _p5_imaging_transformations_grayscale (ApplicationContext context, ActiveEventArgs e)
        {
            Bitmap original = e.Args.Get<Bitmap> (context);
            var destination = new Bitmap (original.Width, original.Height);

            using (Graphics g = Graphics.FromImage (destination)) {

                // Grayscale ColorMatrix creation.
                var colors = new ColorMatrix (
                   new float[][] {
                         new float[] {.3f, .3f, .3f, 0, 0},
                         new float[] {.59f, .59f, .59f, 0, 0},
                         new float[] {.11f, .11f, .11f, 0, 0},
                         new float[] {0, 0, 0, 1, 0},
                         new float[] {0, 0, 0, 0, 1}
                   });

                // Image attributes, to apply colors to blit operation.
                var attrs = new ImageAttributes ();
                attrs.SetColorMatrix (colors);

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
