/*
 * Phosphorus Five, copyright 2014 - 2016, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details.
 * 
 * Flickr.Net, which this file is created on top of, is licensed under LGPL 2.1 or the Apache 2.0 license, of
 * your choice
 */

using System;
using System.Linq;
using System.Configuration;
using p5.exp;
using p5.core;
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
                flickr.ApiKey = ConfigurationManager.AppSettings ["flickr-api-key"];
                var options = new PhotoSearchOptions { 
                    Tags = e.Args.GetExChildValue<string> ("tags", context), 
                    Text = e.Args.GetExChildValue<string> ("text", context), 
                    Username = e.Args.GetExChildValue<string> ("username", context), 
                    PerPage = e.Args.GetExChildValue ("per-page", context, 50), 
                    Page = e.Args.GetExChildValue<int> ("page", context, 0) + 1, 
                    Extras = 
                        PhotoSearchExtras.Tags | 
                        PhotoSearchExtras.Description | 
                        PhotoSearchExtras.OriginalUrl | 
                        PhotoSearchExtras.Medium640Url | 
                        PhotoSearchExtras.ThumbnailUrl |
                        PhotoSearchExtras.License,
                    SafeSearch = (SafetyLevel)Enum.Parse (typeof (SafetyLevel), e.Args.GetExChildValue<string> ("safety-level", context, "None")),
                    SortOrder = (PhotoSearchSortOrder)Enum.Parse (typeof (PhotoSearchSortOrder), e.Args.GetExChildValue<string> ("sort-order", context, "Relevance")),
                    TagMode = (TagMode)Enum.Parse (typeof (TagMode), e.Args.GetExChildValue<string> ("tag-mode", context, "AllTags"))
                };
                options.Licenses.Add (LicenseType.AttributionCC);
                options.Licenses.Add (LicenseType.AttributionNoDerivativesCC);
                options.Licenses.Add (LicenseType.AttributionNoncommercialCC);
                options.Licenses.Add (LicenseType.AttributionNoncommercialNoDerivativesCC);
                options.Licenses.Add (LicenseType.AttributionNoncommercialShareAlikeCC);
                options.Licenses.Add (LicenseType.AttributionShareAlikeCC);
                options.Licenses.Add (LicenseType.NoKnownCopyrightRestrictions);
                options.Licenses.Add (LicenseType.PublicDomainDedicationCC0);
                options.Licenses.Add (LicenseType.PublicDomainMark);
                options.Licenses.Add (LicenseType.UnitedStatesGovernmentWork);
                PhotoCollection photos = flickr.PhotosSearch (options);

                // Returning results to caller
                foreach (var idxRes in photos) {
                    e.Args.Add ("result");
                    e.Args.LastChild.Add ("title", idxRes.Title);
                    e.Args.LastChild.Add ("description", idxRes.Description);
                    e.Args.LastChild.Add ("thumb", idxRes.SquareThumbnailUrl);
                    e.Args.LastChild.Add ("medium", idxRes.Medium640Url);
                    e.Args.LastChild.LastChild.Add ("width", idxRes.Medium640Width);
                    e.Args.LastChild.LastChild.Add ("height", idxRes.Medium640Height);

                    e.Args.LastChild.Add ("original", idxRes.OriginalUrl);
                    e.Args.LastChild.LastChild.Add ("width", idxRes.OriginalWidth);
                    e.Args.LastChild.LastChild.Add ("height", idxRes.OriginalHeight);

                    e.Args.LastChild.Add ("license", idxRes.License.ToString ());
                    e.Args.LastChild.Add ("tags");
                    e.Args.LastChild.LastChild.AddRange (idxRes.Tags.Select (ix => new Node (ix)));
                }
            }
        }
    }
}

