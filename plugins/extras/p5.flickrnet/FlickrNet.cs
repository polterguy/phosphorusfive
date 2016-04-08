/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details.
 */

using System;
using System.Linq;
using p5.core;
using p5.exp;
using FlickrNet;

/// <summary>
///     Main namespace for handling Flickr.NET
/// </summary>
namespace p5.flickrnet
{
    public static class FlickrNet
    {
        [ActiveEvent (Name = "p5.flickrnet.search", Protection = EventProtection.LambdaClosed)]
        public static void p5_flickrnet_search (ApplicationContext context, ActiveEventArgs e)
        {
            // Making sure we clean up and remove all arguments passed in after execution
            using (new Utilities.ArgsRemover (e.Args, true)) {

                // Instantiating Flickr.NET and doing search, supplying tags user wants to see
                Flickr flickr = new Flickr ();
                flickr.ApiKey = "f557f81d899a4515c304b4733794cc00";
                var options = new PhotoSearchOptions { 
                    Tags = e.Args.GetExChildValue ("tags", context, ""), 
                    PerPage = e.Args.GetExChildValue ("count", context, 50), 
                    Page = 1,
                    Extras = PhotoSearchExtras.Tags | PhotoSearchExtras.Description
                };
                PhotoCollection photos = flickr.PhotosSearch (options);

                // Returning results to caller
                foreach (var idxRes in photos) {
                    e.Args.Add ("result");
                    e.Args.LastChild.Add ("title", idxRes.Title);
                    e.Args.LastChild.Add ("description", idxRes.Description);
                    e.Args.LastChild.Add ("thumb", idxRes.SquareThumbnailUrl);
                    e.Args.LastChild.Add ("normal", idxRes.Medium640Url);
                    e.Args.LastChild.Add ("tags");
                    e.Args.LastChild.LastChild.AddRange (idxRes.Tags.Select (ix => new Node (ix)));
                }
            }
        }
    }
}

