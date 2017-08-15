Flickr module
===============

This folder contains an Active Event that allows you to search for images on Flickr. Its name is *[p5.flickr.search]*. To use it, you could
do something like the following.

```
p5.flickr.search
  api-key:YOUR_FLICKR_API_KEY
  text:foo bar
```

The return value, will resemble something like this.

```
p5.flickr.search
  result
    title:lion
    description:lion
    web-url:"https://www.flickr.com/photos/91501748@N07/26260010225/"
    user-id:91501748@N07
    thumb:"https://farm2.staticflickr.com/1609/26260010225_c25241c989_s.jpg"
    medium:"https://farm2.staticflickr.com/1609/26260010225_c25241c989_z.jpg"
      width:int:640
      height:int:424
    original:"https://farm2.staticflickr.com/1609/26260010225_2ba8af751b_o.jpg"
      width:int:4180
      height:int:2769
    license:PublicDomainDedicationCC0
    tags
      lion
  result
    title:lion
    description:lion

  /* ... etc ... */
```

The *[p5.flickr.search]* takes the following arguments.

* [text] - Query to search for
* [tags] - Tags to match
* [username] - Username results must belong to
* [per-page] - Number of return values (page size)
* [page] - Page of return
* [safety-level] - Safety level of return values. Legal values are 'None', 'Safe', 'Moderate' and 'Restricted'.
* [sort-order] - How to sort the results. Legal values are 'None', 'DatePostedAscending', 'DatePostedDescending', 'DateTakenAscending', 'DateTakenDescending', 'InterestingnessAscending', 'InterestingnessDescending' and 'Relevance'.
* [tag-mode] - Tag mode. Legal values are 'None', 'AnyTags', 'AllTags' and 'Boolean'.

Internally *[p5.flickr.search]* is using [FlickrNet](https://github.com/samjudson/flickr-net), where you can find the documentation for what the different 
options and arguments signifies. FlickrNet is licensed under the Apache License, Version 2.0, and the copyright of Sam Judson.

The Active Event *[p5.flickr.search]*, will by default, only return _"free"_ images, which are images under the following licenses.

* Creative Commons Attribution
* Creative Commons Attribution No Derivatives
* Creative Commons Attribution Non Commercial
* Creative Commons Attribution Non Commercial, No Derivatives
* Creative Commons Attribution Non Commercial, Share-Alike
* Creative Commons Attribution Share-Alike
* No known copyright restrictions.
* Public Domain dedicated
* Public Domain Mark
* United states government work

To use it, make sure you provide your own Flickr API key. You can get your own API key, by applying for one at [Flickr](https://www.flickr.com/).
