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
        [ActiveEvent (Name = "p5.flickr.search")]
        public static void p5_flickr_search (ApplicationContext context, ActiveEventArgs e)
        {
            // Making sure we clean up and remove all arguments passed in after execution
            using (new Utilities.ArgsRemover (e.Args, true)) {

                // Instantiating Flickr.NET and doing search, supplying tags user wants to see
                Flickr flickr = new Flickr ();
                flickr.ApiKey = context.Raise (
                    ".get-config-setting",
                    new Node ("", ".p5.flickr.api-key"))[0].Get<string> (context);
                var options = new PhotoSearchOptions { 
                    Text = e.Args.GetExChildValue<string> ("text", context),
                    Tags = e.Args.GetExChildValue<string> ("tags", context),
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
                    e.Args.LastChild.Add ("web-url", idxRes.WebUrl);
                    e.Args.LastChild.Add ("user-id", idxRes.UserId);
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

