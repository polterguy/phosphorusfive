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

using System.Drawing;
using p5.exp;
using p5.core;
using p5.exp.exceptions;

namespace p5.imaging.transformations
{
    /// <summary>
    ///     Class to help rotate images
    /// </summary>
    public static class Rotate
    {
        /// <summary>
        ///     Flips an image, either vertically, or horizontally.
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = ".p5.imaging.transformations.rotate")]
        public static void _p5_imaging_transformations_rotate (ApplicationContext context, ActiveEventArgs e)
        {
            Bitmap bitmap = e.Args.Get<Bitmap> (context);
            switch (e.Args.GetExChildValue ("degrees", context, "90")) {
                case "90":
                    bitmap.RotateFlip (RotateFlipType.Rotate90FlipNone);
                    break;
                case "180":
                    bitmap.RotateFlip (RotateFlipType.Rotate180FlipNone);
                    break;
                case "270":
                    bitmap.RotateFlip (RotateFlipType.Rotate270FlipNone);
                    break;
                default:
                    throw new LambdaException ("I don't know how to [rotate] the image according to your specified [degrees]. Only '90', '180' or '270' are valid values for [rotate].", e.Args, context);
            }
        }
    }
}
