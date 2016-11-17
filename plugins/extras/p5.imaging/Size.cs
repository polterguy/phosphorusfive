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

using System.Linq;
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
    public static class Size
    {
        /// <summary>
        ///     Resizes an image file to another image file
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "p5.imaging.get-size")]
        public static void p5_imaging_get_size (ApplicationContext context, ActiveEventArgs e)
        {
            // Making sure we clean up and remove all arguments passed in after execution.
            using (new Utilities.ArgsRemover (e.Args, true)) {

                // Iterating each path provided.
                foreach (var idxSrc in XUtil.Iterate<string> (context, e.Args, true)) {

                    // Unrolling path, and verify user is authorized to reading file.
                    var sourcePath = context.Raise (".p5.io.unroll-path", new Node ("", idxSrc)).Get<string> (context);
                    context.Raise (".p5.io.authorize.read-file", new Node ("", sourcePath).Add ("args", e.Args));

                    // Getting root folder, and opening file.
                    var rootFolder = Helpers.GetBaseFolder (context);
                    using (var src = Image.FromFile (rootFolder + sourcePath)) {

                        // Returning width and height to caller.
                        var resultNode = e.Args.Add (sourcePath).LastChild;
                        resultNode.Add ("width", src.Width);
                        resultNode.Add ("height", src.Height);
                    }
                }
            }
        }

        /// <summary>
        ///     Resizes an image file to another image file
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "p5.imaging.resize")]
        public static void p5_imaging_resize (ApplicationContext context, ActiveEventArgs e)
        {
            // Making sure we clean up and remove all arguments passed in after execution
            using (new Utilities.ArgsRemover (e.Args, true)) {

                // Retrieving arguments
                int width = e.Args.GetExChildValue ("width", context, -1);
                int height = e.Args.GetExChildValue ("height", context, -1);
                var sourcePath = context.Raise (".p5.io.unroll-path", new Node ("", e.Args.GetExValue<string> (context))).Get<string> (context);
                var destPath = context.Raise (".p5.io.unroll-path", new Node ("", e.Args.GetExChildValue<string> ("dest", context))).Get<string> (context);

                // Figuring out detination file type
                string destType = destPath.Substring (destPath.LastIndexOf (".") + 1);
                ImageFormat format = null;
                switch (destType) {
                    case "bmp":
                        format = ImageFormat.Bmp;
                        break;
                    case "gif":
                        format = ImageFormat.Gif;
                        break;
                    case "ico":
                        format = ImageFormat.Icon;
                        break;
                    case "png":
                        format = ImageFormat.Png;
                        break;
                    case "jpg":
                    case "jpeg":
                        format = ImageFormat.Jpeg;
                        break;
                }

                // Sanity check
                if (string.IsNullOrEmpty (sourcePath) || string.IsNullOrEmpty (destPath))
                    throw new LambdaException (
                        "[p5.imaging.resize] requires at least [dest] and value", 
                        e.Args, 
                        context);

                // Verifies user is authorised to reading source file, and saving to destination file
                context.Raise (".p5.io.authorize.read-file", new Node ("", sourcePath).Add ("args", e.Args));
                context.Raise (".p5.io.authorize.modify-file", new Node ("", destPath).Add ("args", e.Args));

                var srcRc = e.Args ["src-rect"] ?? new Node ();

                var rootFolder = Helpers.GetBaseFolder (context);
                using (var src = Image.FromFile (rootFolder + sourcePath)) {

                    // Creating our source rectangle, defaulting to "entire image".
                    var rc = new Rectangle (
                        srcRc.GetExChildValue ("left", context, 0),
                        srcRc.GetExChildValue ("top", context, 0),
                        srcRc.GetExChildValue ("width", context, src.Width),
                        srcRc.GetExChildValue ("height", context, src.Height));
                    if (rc.Width + rc.Left > src.Width || rc.Height + rc.Top > src.Height)
                        throw new LambdaException ("Clip rectangle was outside the boundaries of source image", e.Args, context);

                    // Creating our destination image, defaulting size to size of source image, if no [width] and/or [height] was supplied.
                    if (width == -1)
                        width = src.Width;
                    if (height == -1)
                        height = src.Height;

                    using (var dest = new Bitmap (width, height)) {

                        // Blitting source image, unto our destination image.
                        using (var graphics = Graphics.FromImage (dest)) {
                            graphics.CompositingMode = CompositingMode.SourceCopy;
                            graphics.CompositingQuality = CompositingQuality.HighQuality;
                            graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                            graphics.SmoothingMode = SmoothingMode.HighQuality;
                            graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                            graphics.DrawImage (src, new Rectangle (0, 0, width, height), rc, GraphicsUnit.Pixel);
                        }

                        var quality = e.Args.GetExChildValue ("quality", context, -1L);
                        if (quality != -1L) {

                            // Quality of destination explicitly given, making sure we use it.
                            // First sanity check, since this argument only makes sense for JPG images.
                            if (format.Guid != ImageFormat.Jpeg.Guid)
                                throw new LambdaException ("Sorry, the [quality] argument can only be supplied when your destination is a JPG file.", e.Args, context);

                            // Then sanity checking the quality in the range of [0-100]
                            if (quality < 0 || quality > 100)
                                throw new LambdaException ("The [quality] argument needs to be in between 0 and 100.", e.Args, context);

                            // Then finding our codec.
                            ImageCodecInfo codec = ImageCodecInfo.GetImageDecoders ().FirstOrDefault (ix => ix.FormatID == ImageFormat.Jpeg.Guid);
                            if (codec == null)
                                throw new LambdaException ("Sorry, no codec found for modifying the [quality] of your image.", e.Args, context);

                            // Then finding our encoder, according to quality requested by caller.
                            Encoder encoder = Encoder.Quality;
                            EncoderParameters pars = new EncoderParameters (1);
                            pars.Param[0] = new EncoderParameter (encoder, quality);

                            // Now saving image with encoder and parameters to encoder.
                            dest.Save (rootFolder + destPath, codec, pars);
                        } else {

                            // No encoder arguments given.
                            dest.Save (rootFolder + destPath, format);
                        }
                    }
                }
            }
        }
    }
}
