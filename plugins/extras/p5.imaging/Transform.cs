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
    public static class Transform
    {
        /// <summary>
        ///     Tranforms from one image to another, applying transsformations in between.
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "p5.imaging.transform")]
        [ActiveEvent (Name = "p5.imaging.resize")]
        public static void p5_imaging_transform (ApplicationContext context, ActiveEventArgs e)
        {
            // Making sure we clean up and remove all arguments passed in after execution.
            using (new Utilities.ArgsRemover (e.Args, true)) {

                // Retrieving source path.
                string source = GetSourcePath (context, e.Args);

                string destination = GetDestinationPath (context, e.Args);

                // Retrieving destination width and height.
                int destinationWidth = e.Args.GetExChildValue ("dest-width", context, -1);
                int destinationHeight = e.Args.GetExChildValue ("dest-height", context, -1);

                // Retrieving app's root folder.
                var rootFolder = Helpers.GetBaseFolder (context);

                // Loading source image.
                using (var srcImage = Image.FromFile (rootFolder + source)) {

                    // Retrieving source rectangle, for what to extract from source image, defaulting to entire image.
                    Rectangle srcRect = GetSourceRect (context, e.Args, srcImage);

                    // Making sure we default destinationWidth and destinationHeight to [src-rect] width and height, unless explicitly given.
                    destinationWidth = destinationWidth == -1 ? srcRect.Width : destinationWidth;
                    destinationHeight = destinationHeight == -1 ? srcRect.Height : destinationHeight;

                    // Figuring out destination file type.
                    ImageFormat format = GetDestinationFormat (destination);

                    // Creating a destination image, and blitting source image unto it.
                    // Notice, little trickery to make sure we dispose Bitmap after having applied all transformations.
                    var destinationBitmap = new Bitmap (destinationWidth, destinationHeight);
                    try {

                        // Blitting source image, unto our destination image.
                        BlitSourceImage (destinationWidth, destinationHeight, srcImage, srcRect, destinationBitmap);

                        // Applying transformations.
                        ApplyTransformations (context, e.Args, ref destinationBitmap);

                        // Checking if we've got a [quality] argument, and if we do, making sure we apply it during our saving operation.
                        SaveDestinationImage (context, e.Args, destination, rootFolder, format, destinationBitmap);

                    } finally {
                        destinationBitmap.Dispose ();
                    }
                }
            }
        }

        /*
         * Applies transformations to bitmap, if any.
         */
        private static void ApplyTransformations (ApplicationContext context, Node args, ref Bitmap bitmap)
        {
            // Checking if there are any transformations.
            if (args ["transformations"] == null)
                return;

            // Looping through each transformation, applying them sequentially.
            foreach (var idxTransNode in args["transformations"].Children) {

                // Finding Active Event name, and invoking Active Event.
                var activeEventName = ".p5.imaging.transformations." + idxTransNode.Name;
                idxTransNode.Value = bitmap;
                try {
                    context.RaiseActiveEvent (activeEventName, idxTransNode);
                    if (idxTransNode.Value != bitmap) {
                        bitmap.Dispose ();
                        bitmap = idxTransNode.Get<Bitmap> (context);
                    }
                } finally {

                    // Making sure we clean up, in case of exceptions
                    idxTransNode.Value = null;
                }
            }
        }

        /*
         * Saves destination image.
         */
        private static void SaveDestinationImage (
            ApplicationContext context, 
            Node args, 
            string destination, 
            string rootFolder, 
            ImageFormat format, 
            Bitmap dest)
        {
            var quality = args.GetExChildValue ("quality", context, -1L);
            if (quality != -1L) {

                // Quality of destination explicitly given, making sure we use it.
                // First sanity check, since this argument only makes sense for JPG images.
                if (format.Guid != ImageFormat.Jpeg.Guid)
                    throw new LambdaException ("Sorry, the [quality] argument can only be supplied when your destination is a JPG file.", args, context);

                // Then sanity checking the quality in the range of [0-100]
                if (quality < 0 || quality > 100)
                    throw new LambdaException ("The [quality] argument needs to be in between 0 and 100.", args, context);

                // Then finding our codec.
                ImageCodecInfo codec = ImageCodecInfo.GetImageDecoders ().FirstOrDefault (ix => ix.FormatID == ImageFormat.Jpeg.Guid);
                if (codec == null)
                    throw new LambdaException ("Sorry, no codec found for modifying the [quality] of your image.", args, context);

                // Then finding our encoder, according to quality requested by caller.
                Encoder encoder = Encoder.Quality;
                EncoderParameters pars = new EncoderParameters (1);
                pars.Param[0] = new EncoderParameter (encoder, quality);

                // Now saving image with encoder and parameters to encoder.
                dest.Save (rootFolder + destination, codec, pars);
            } else {

                // No encoder arguments given.
                dest.Save (rootFolder + destination, format);
            }
        }

        /*
         * Draws source image unto destination image.
         */
        private static void BlitSourceImage (
            int destinationWidth, 
            int destinationHeight, 
            Image srcImage, 
            Rectangle srcRect, 
            Bitmap dest)
        {
            using (var graphics = Graphics.FromImage (dest)) {

                // Applying options to graphics object, to make sure we blit our image as smoothly as possible.
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                // Now we can actually draw our source image unto our destination image.
                graphics.DrawImage (srcImage, new Rectangle (0, 0, destinationWidth, destinationHeight), srcRect, GraphicsUnit.Pixel);
            }
        }

        /*
         * Returns the destination image format.
         */
        private static ImageFormat GetDestinationFormat (string destination)
        {
            ImageFormat format = null;
            switch (destination.Substring (destination.LastIndexOf (".") + 1)) {
                case "bmp":
                    format = ImageFormat.Bmp;
                    break;
                case "gif":
                    format = ImageFormat.Gif;
                    break;
                case "png":
                    format = ImageFormat.Png;
                    break;
                case "jpg":
                case "jpeg":
                    format = ImageFormat.Jpeg;
                    break;
            }

            return format;
        }

        /*
         * Returns the source Rectangle.
         */
        private static Rectangle GetSourceRect (ApplicationContext context, Node args, Image srcImage)
        {
            var srcRectNode = args["src-rect"];
            var srcRect = srcRectNode == null ?
                new Rectangle (0, 0, srcImage.Width, srcImage.Height) :
                new Rectangle (
                    srcRectNode.GetExChildValue ("left", context, 0),
                    srcRectNode.GetExChildValue ("top", context, 0),
                    srcRectNode.GetExChildValue ("width", context, srcImage.Width - srcRectNode.GetExChildValue ("left", context, 0)),
                    srcRectNode.GetExChildValue ("height", context, srcImage.Height - srcRectNode.GetExChildValue ("top", context, 0)));
            return srcRect;
        }

        /*
         * Returns the source path.
         */
        private static string GetSourcePath (ApplicationContext context, Node args)
        {
            var source = context.RaiseActiveEvent (".p5.io.unroll-path", new Node ("", args.GetExValue<string> (context))).Get<string> (context);
            context.RaiseActiveEvent (".p5.io.authorize.read-file", new Node ("", source).Add ("args", args));

            // Sanity check.
            if (string.IsNullOrEmpty (source))
                throw new LambdaException (
                    "[p5.imaging.transform] requires at least [destination] and source being value.",
                    args,
                    context);
            return source;
        }

        /*
         * Returns the destination path.
         */
        private static string GetDestinationPath (ApplicationContext context, Node args)
        {
            var destination = context.RaiseActiveEvent (".p5.io.unroll-path", new Node ("", args.GetExChildValue<string> ("destination", context))).Get<string> (context);
            context.RaiseActiveEvent (".p5.io.authorize.modify-file", new Node ("", destination).Add ("args", args));

            // Sanity check.
            if (string.IsNullOrEmpty (destination))
                throw new LambdaException (
                    "[p5.imaging.transform] requires at least [destination] and source being value.",
                    args,
                    context);
            return destination;
        }
    }
}
