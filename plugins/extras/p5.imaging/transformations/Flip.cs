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
    ///     Class to help flip images, either vertically or horizontally
    /// </summary>
    public static class Flip
    {
        /// <summary>
        ///     Flips an image, either vertically, or horizontally.
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = ".p5.imaging.transformations.flip")]
        public static void _p5_imaging_transformations_flip (ApplicationContext context, ActiveEventArgs e)
        {
            Bitmap bitmap = e.Args.Get<Bitmap> (context);
            switch (e.Args.GetExChildValue ("direction", context, "x")) {
                case "x":
                    bitmap.RotateFlip (RotateFlipType.RotateNoneFlipX);
                    break;
                case "y":
                    bitmap.RotateFlip (RotateFlipType.RotateNoneFlipY);
                    break;
                case "both":
                    bitmap.RotateFlip (RotateFlipType.RotateNoneFlipXY);
                    break;
                default:
                    throw new LambdaException ("I don't know how to [flip] the image according to your specified [direction]. Only 'x', 'y' or 'both' are valid values for [flip].", e.Args, context);
            }
        }
    }
}
