/*
 * Phosphorus Five, copyright 2014 - 2016, Thomas Hansen, mr.gaia@gaiasoul.com
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
using System.Drawing.Drawing2D;
using p5.exp;
using p5.core;
using p5.exp.exceptions;
using p5.imaging.helpers;

/// <summary>
///     Main namespace for handling images
/// </summary>
namespace p5.imaging
{
    /// <summary>
    ///     Class to help resize images
    /// </summary>
    public static class GetSize
    {
        /// <summary>
        ///     Returns the size of one or more image.
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "p5.imaging.get-size")]
        public static void p5_imaging_get_size (ApplicationContext context, ActiveEventArgs e)
        {
            // Making sure we clean up and remove all arguments passed in after execution.
            using (new Utilities.ArgsRemover (e.Args, true)) {

                // Looping through each image caller requests the size for.
                foreach (var idxPath in XUtil.Iterate<string> (context, e.Args, true)) {

                    // Unrolling path, and verifying user is authorized to reading file.
                    var source = context.Raise (".p5.io.unroll-path", new Node ("", idxPath)).Get<string> (context);
                    context.Raise (".p5.io.authorize.read-file", new Node ("", source).Add ("args", e.Args));

                    // Getting root folder, and opening file.
                    var rootFolder = Helpers.GetBaseFolder (context);
                    using (var src = Image.FromFile (rootFolder + source)) {

                        // Returning width and height to caller.
                        var resultNode = e.Args.Add (source).LastChild;
                        resultNode.Add ("width", src.Width);
                        resultNode.Add ("height", src.Height);
                    }
                }
            }
        }
    }
}