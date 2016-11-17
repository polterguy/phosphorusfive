Image manipulation
===============

This folder contains Active Events that allows you to manipulate images in Phosphorus Five. Currently there are two Active Events in this plugin.

* [p5.imaging.get-size] - Returns the size of an image on your server.
* [p5.imaging.transform] - Transforms, or resize an image on your server.

The *[p5.imaging.transform]* Active Event, has an alias, which is *[p5.imaging.resize]*.

## Retrieving the size of an image

To retrieve the size of an image, you can use the following code.

```
p5.imaging.get-size:~/thomas.jpg
```

The above code assumes you've got an image in your main home folder called _"thomas.jpg"_, and it will return something resembling the following.

```
p5.imaging.get-size
  /users/root/thomas.jpg
    width:int:326
    height:int:294
```

You can supply an expression leading to multiple paths, to retrieve the size of multiple images at once, if you wish.

## Resizing, clipping and transforming your images

To change the size of an image, requires some explanation before we dive into it. The Active Event you use to change an image's size, is 
called *[p5.imaging.resize]* or *[p5.imaging.transform]*. This Active Event takes the following arguments.

* [dest-width] - New width of image
* [dest-height] - New height of image
* [destination] - Destination for the resized image
* [src-rect] - An optional rectangle declaring where to clip your source image, to create the destination image

The *[src-rect]* argument, takes the following children.

* [left]
* [top]
* [width]
* [height]

If you supply a *[src-rect]*, then the source image, will be clipped according to your *[src-rect]* argument, before it is resized to your *[width]* 
and *[height]*, and stored to your *[destination]* filename. This allows you to clip the image, as you are resizing it. Consider the following, assuming 
you've got our first _"thomas.jpg"_ example image, from our first example, and that your image is at least 300x200 in size.

```
p5.imaging.transform:~/thomas.jpg
  destination:~/thomas-new.jpg
  dest-width:500
  dest-height:100
  src-rect
    left:0
    top:0
    width:300
    height:200
```

The above code, will first of all cut your image, from the top left corner, removing all but the first 200x300 pixels. Then it will stretch that result,
into your destination, making it become 500x100 pixels in size. The result will probably look _"weird"_, and highly skewed, depending upon your source image.
Below is an example of before and after, having transformed an image of 326x294 pixels.

Before image.

![alt tag](screenshots/thomas.jpg)

After image.

![alt tag](screenshots/thomas-new.jpg)

Notice, your *[src-rect]* must be within the boundaries of the size of your source image. All arguments beneath *[src-rect]* are also optional, and if not supplied, 
will be default to either 0, 0, _"rest-of-source-image-width"_, _"rest-of-source-image-height"_.

Notice!
If your destination image already exist, it will be silently overwritten.

The *[p5.imaging.transform]* Active Event can take expressions, but only one image can be resized at the same time. If you do not supply a *[src-rect]*, the
entire image will be resized, and the *[src-rect]* will default to encompass your entire source image. Consider the following.

```
p5.imaging.transform:~/thomas.jpg
  destination:~/new.jpg
  dest-width:500
  dest-height:100
```

The above code, will not in any ways _"clip"_ your image, but keep the entire source image, only resizing it, probably making it _"skewed"_ in the process.

### Applying additional transformations

When you invoke *[p5.imaging.transform]*, you can also apply additional transformations. These are transformations to your destination image, 
applied after creating your destination image, but before it is saved to disc. The following transformations exist in Phosphorus Five out of the box.

* [rotate] - Requires a [degrees] argument, with the legal values of "90", "180" or "270".
* [flip] - Requires a [direction] argument, with the legal values of "x", "y" or "both".
* [grayscale] - No arguments. Creates a grayscale version.
* [colorize] - Requires a [matrix] argument, with 5 children, containing a comma separated value with 5 integers between 0 and 1.

#### Rotating and flipping your image

An example of how to transform an image, applying an additional *[rotate]* and *[flip]* transformation, is shown below.

```
p5.imaging.transform:~/thomas.jpg
  destination:~/flip-rotate.jpg
  transformations
    flip
      direction:x
    rotate
      degrees:90
```

If you start out with the _"thomas.jpg"_ image from above, your result will look like the following, after having applied the above transformations.

![alt tag](screenshots/flip-rotate.jpg)

The above tranformation will not resize the image in any ways, but simply flip it in the x direction, and rotate it 90 degrees. You can also 
resize the image in the same operation, such as the following illustrates.

```
p5.imaging.transform:~/thomas.jpg
  destination:~/thomas-resized-and-transformed.jpg
  quality:20
  src-rect
    top:50
    left:150
    height:100
  dest-width:500
  dest-height:300
  transformations
    rotate
      degrees:90
```

Tranformations will be applied sequentially, in the order they appear in your Hyperlambda.

#### Creating a grayscale of your image

To create a grayscale version of your original image, consider this code, assuming you've got an image called _"donald.png"_ in your home folder.

```
p5.imaging.transform:~/donald.png
  destination:~/donald-gray.png
  transformations
    grayscale
```

The above will transform the following image.

![alt tag](screenshots/donald.png)

Into this.

![alt tag](screenshots/donald-gray.png)

#### Colorizing an image

To colorize an image, you can use something such as the following, assuming you've got the _"donald.png"_ image in your home folder.

```
p5.imaging.transform:~/donald.png
  destination:~/donald-colorized.png
  transformations
    colorize
      matrix
        _:1,   0,   0,   0,   0
        _:0,   1,   1,   0,   0
        _:0,   0,   1,   0,   0
        _:0,   0,   0,   1,   0
        _:0,   0,   0.5, 0,   1
```

The *[colorize]* transformation, requires you to supply a 5x5 dimensional color transformation matrix, which is used to transform each pixel in
your image. Exactly how this is done, is beyond the scope of this article, but the above color matrix, will make your image appear _"blue'ish"_,
by applying additional blue multiplications to each pixel in your original image. Think _"bluescale"_ to understand the above *[colorize]* invocation.

An example of the results of the above, can be seen below. Image is taken from wikipedia.org. To the left is the original, and to the right is the
same image, after being colorized.

![alt tag](screenshots/colorized.jpg)

Some useful *[colorize]* matrixes can be found below.

##### Doubling the intensity of red

```
p5.imaging.transform:~/donald.png
  destination:~/donald-red.png
  transformations
    colorize
      matrix
        _:2,   0,   0,   0,   0
        _:0,   1,   0,   0,   0
        _:0,   0,   1,   0,   0
        _:0,   0,   0,   1,   0
        _:0,   0,   0,   0,   1
```

![alt tag](screenshots/donald-red.png)

##### Negating colors

```
p5.imaging.transform:~/donald.png
  destination:~/donald-negative.png
  transformations
    colorize
      matrix
        _:-1,  0,   0,   0,   0
        _:0,  -1,   0,   0,   0
        _:0,   0,  -1,   0,   0
        _:0,   0,   0,   1,   0
        _:1,   1,   1,   0,   1
```

![alt tag](screenshots/donald-negative.png)

##### Creating a semi-transparent image

```
p5.imaging.transform:~/donald.png
  destination:~/donald-semi-transparent.png
  transformations
    colorize
      matrix
        _:1,   0,   0,   0,   0
        _:0,   1,   0,   0,   0
        _:0,   0,   1,   0,   0
        _:0,   0,   0, 0.5,   0
        _:0,   0,   0,   0,   1
```

![alt tag](screenshots/donald-semi-transparent.png)

### Converting an image

If you wish, you can use the *[p5.imaging.resize]* Active Event to convert between image types, without resizing your image. In such a case, you only
need to supply a *[destination]* argument, and not pass in any *[dest-width]* or *[dest-height]*, at which case, the height and width of your new image, will be the
same as your source image. Consider this.

```
p5.imaging.transform:~/thomas.jpg
  destination:~/thomas.png
```

Notice above, that the only difference in the source and the destination, is the file extension that is being changed from _".jpg"_ to _".png"_. This will 
result in the exact same image, but with a different image format. 

### Changing the quality of your image

If your destination image is of type JPG, meaning its file extension is either _".jpg"_ or _".jpeg"_, you can change the quality of your destination image, 
by providing a *[quality]* argument. This argument must have a value between 0 and 100, where 100 is best quality, and 0 is worst quality. For instance, to
aggressively reduce the quality, and hence the size of your image, you could do something like the following. 

```
p5.imaging.transform:~/thomas.jpg
  dest:~/thomas-compact.jpg
  quality:5
```

The above level for your *[quality]* argument will significantly reduce the quality of your image. A better value, yet still an agressive value, would probably
be somewhere between 15 and 25, depending upon your source image.

To understand the difference, realize that this image.

![alt tag](screenshots/thomas.jpg)

Was reduced from 13 KB on disc, to 3 KB after reducing its quality with a *[quality]* value of _"5"_. However, the result became pretty bad, as you can see below.

![alt tag](screenshots/thomas-compact.jpg)

A value of 20 however, still reduces the image size significantly, to 6KB, while still retaining a relatively "OK" quality for your image.

![alt tag](screenshots/thomas-less.jpg)

Notice, sometimes, depending upon how your image looks like, you can also drastically reduce your image size, by going from JPG to PNG for instance, 
if your image contains a lot of the same colors, and big surfaces. The opposite is true if your image contains a log of different colors, with 
more _"organic"_ content.

The above image of your truly for instance, is probably much better to display as a JPG image. If it had lots of big surfaces, with similar colors though, 
it would probably be smaller as a PNG image.
