/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details.
 */

using System;
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
    public static class Resizing
    {
        /// <summary>
        ///     Resizes an image file to another image file
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "p5.imaging.resize", Protection = EventProtection.LambdaClosed)]
        public static void p5_imaging_resize (ApplicationContext context, ActiveEventArgs e)
        {
            // Making sure we clean up and remove all arguments passed in after execution
            using (new Utilities.ArgsRemover (e.Args, true)) {
                int width = e.Args.GetExChildValue ("width", context, -1);
                int height = e.Args.GetExChildValue ("height", context, -1);
                string sourcePath = e.Args.GetExValue<string> (context);
                string destPath = e.Args.GetExChildValue<string> ("dest", context);
                bool keepAspectRatio = e.Args.GetExChildValue ("keep-aspect-ratio", context, false);

                // Sanity check
                if (width == -1 || height == -1 || string.IsNullOrEmpty (sourcePath) || string.IsNullOrEmpty (destPath))
                    throw new LambdaException (
                        "[p5.imaging.resize] requires at least [width], [height], [dest] and value being source", 
                        e.Args, 
                        context);

                // Verifies user is authorised to reading source file, and saving to destination file
                context.RaiseNative ("p5.io.authorize.read-file", new Node ("", sourcePath).Add ("args", e.Args));
                context.RaiseNative ("p5.io.authorize.modify-file", new Node ("", destPath).Add ("args", e.Args));

                var rootFolder = Helpers.GetBaseFolder (context);
                using (var sourceBmp = Image.FromFile (rootFolder + sourcePath)) {
                    Color bgColor = Helpers.GetColor (e.Args.GetExChildValue ("background-color", context, "White"));
                    using (var destBmp = ResizeImage (sourceBmp, width, height, keepAspectRatio, bgColor)) {
                        destBmp.Save (rootFolder + destPath);
                    }
                }
            }
        }

        /*
         * Helper for above
         */
        public static Bitmap ResizeImage (
            Image srcImage, 
            int destWidth, 
            int destHeight, 
            bool keepAspectRatio,
            Color bgColor)
        {
            var destImage = new Bitmap (destWidth, destHeight, PixelFormat.Format32bppArgb);

            int newWidth = destWidth;
            int newHeight = destHeight;

            destImage.SetResolution (srcImage.PhysicalDimension.Width, srcImage.PhysicalDimension.Height);

            using (var graphics = Graphics.FromImage (destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                int top = 0, left = 0;

                if (keepAspectRatio) {

                    // Caller wants to keep aspect ratio, blitting image unto surface, 
                    // making sure outside parts of image defaults to specified background color
                    graphics.Clear (bgColor);

                    newWidth = Convert.ToInt32 (srcImage.PhysicalDimension.Width);
                    newHeight = Convert.ToInt32 (srcImage.PhysicalDimension.Height);

                    // Figure out the ratio
                    double ratioWidth = (double) destWidth / (double) srcImage.PhysicalDimension.Width;
                    double ratioHeight = (double) destHeight / (double) srcImage.PhysicalDimension.Height;
                    double ratio = ratioWidth < ratioHeight ? ratioWidth : ratioHeight;

                    // Getting the new height and width
                    newWidth = Convert.ToInt32(srcImage.PhysicalDimension.Width * ratio);
                    newHeight = Convert.ToInt32(srcImage.PhysicalDimension.Height * ratio);

                    // Calculate the top/left corner
                    top = Convert.ToInt32((destHeight - (srcImage.PhysicalDimension.Height * ratio)) / 2);
                    left = Convert.ToInt32((destWidth - (srcImage.PhysicalDimension.Width * ratio)) / 2);
                }
                graphics.DrawImage (
                    srcImage, 
                    new Rectangle (left, top, newWidth, newHeight), 
                    new Rectangle (0, 0, Convert.ToInt32 (srcImage.PhysicalDimension.Width), Convert.ToInt32 (srcImage.PhysicalDimension.Height)),
                    GraphicsUnit.Pixel);
            }
            return destImage;
        }
    }
}
