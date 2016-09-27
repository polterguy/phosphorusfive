/*
 * Phosphorus Five, copyright 2014 - 2016, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details.
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
    public static class Resizing
    {
        /// <summary>
        ///     Resizes an image file to another image file
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "p5.imaging.get-size")]
        public static void p5_imaging_get_size (ApplicationContext context, ActiveEventArgs e)
        {
            // Making sure we clean up and remove all arguments passed in after execution
            using (new Utilities.ArgsRemover (e.Args, true)) {

                // Verifying user is authorized to reading file
                string sourcePath = e.Args.GetExValue<string> (context);
                context.Raise ("p5.io.authorize.read-file", new Node ("", sourcePath).Add ("args", e.Args));

                // Getting root folder, and opening file
                var rootFolder = Helpers.GetBaseFolder (context);
                using (var src = Image.FromFile (rootFolder + sourcePath)) {

                    // Returning width and height to caller
                    e.Args.Add ("width", src.Width);
                    e.Args.Add ("height", src.Height);
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
                string sourcePath = e.Args.GetExValue<string> (context);
                string destPath = e.Args.GetExChildValue<string> ("destination-file", context);

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
                if (string.IsNullOrEmpty (sourcePath) || string.IsNullOrEmpty (destPath) || width == -1 || height == -1)
                    throw new LambdaException (
                        "[p5.imaging.resize] requires at least [width], [height], [destination-file] and value", 
                        e.Args, 
                        context);

                // Verifies user is authorised to reading source file, and saving to destination file
                context.Raise ("p5.io.authorize.read-file", new Node ("", sourcePath).Add ("args", e.Args));
                context.Raise ("p5.io.authorize.modify-file", new Node ("", destPath).Add ("args", e.Args));

                var srcRc = e.Args ["src-rect"] ?? new Node ();

                var rootFolder = Helpers.GetBaseFolder (context);
                using (var src = Image.FromFile (rootFolder + sourcePath)) {
                    var rc = new Rectangle (
                        srcRc.GetExChildValue ("left", context, 0),
                        srcRc.GetExChildValue ("top", context, 0),
                        srcRc.GetExChildValue ("width", context, src.Width),
                        srcRc.GetExChildValue ("height", context, src.Height));
                    if (rc.Width + rc.Left > src.Width || rc.Height + rc.Top > src.Height)
                        throw new LambdaException ("Clip rectangle was outside the boundaries of source image", e.Args, context);
                    using (var dest = new Bitmap (width, height)) {
                        using (var graphics = Graphics.FromImage (dest)) {
                            graphics.CompositingMode = CompositingMode.SourceCopy;
                            graphics.CompositingQuality = CompositingQuality.HighQuality;
                            graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                            graphics.SmoothingMode = SmoothingMode.HighQuality;
                            graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                            graphics.DrawImage (src, new Rectangle (0, 0, width, height), rc, GraphicsUnit.Pixel);
                        }
                        dest.Save (rootFolder + destPath, format);
                    }
                }
            }
        }
    }
}
