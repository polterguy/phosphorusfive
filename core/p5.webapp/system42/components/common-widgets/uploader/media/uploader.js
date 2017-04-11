/*
 * Phosphorus Five, copyright 2014 - 2017, Thomas Hansen, thomas@gaiasoul.com
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


(function () {

    // Constructor.
    p5.uploader = function (widget, cssClass, hoverClass, dropClass, errorClass, filter, multiple, file, clickable, preview) {

        // Initializing.
        this._widget = p5.$(widget);
        this._cssClass = cssClass;
        this._hoverClass = hoverClass;
        this._dropClass = dropClass;
        this._errorClass = errorClass;
        this._filter = (filter == '' ? [] : filter.split('|'));
        this._multiple = multiple;
        this._file = p5.$(file).el;
        this._preview = preview;

        // Storing this as "self" to have access to it inside of event handlers further down.
        var self = this;

        // First the DOM event handler for what happens when main widget is clicked.
        // We simply raise "click" on the file input.
        if (clickable) {
            this._widget.el.addEventListener('click', function (e) {self._file.click();}, false);
        }

        // Since we trigger click on file input when main widget is clicked above, and file input is a child of main widget, we
        // need to stop propagation, to prevent never ending recursive bubbling, when file input element is clicked.
        this._file.addEventListener('click', function (e) {e.stopPropagation();});

        // Handling onchange event on file input element.
        this._file.addEventListener('change', function () {self.onFileInputChanged();});

        // Then the DOM event handler for what happens when a file is dropped unto widget.
        this._widget.el.addEventListener('drop', function (e) { self.onDrop(e);}, false);

        // Then the DOM event handler for what happens when a file is dragged over it.
        this._widget.el.addEventListener('dragover', function (e) {self.onDragOver(e);}, false);

        // Then the DOM event handler for what happens when the user drags the file away from our widget.
        this._widget.el.addEventListener('dragleave', function (e) {self.onDragLeave(e);}, false);
    };


    // Allows you to show the "browse for file" window explicitly.
    p5.uploader.prototype.browse = function () {

        // Clicks the file input element.
        this._file.click ();
    };


    // Triggered when file input's value was changed.
    p5.uploader.prototype.onFileInputChanged = function () {

        // Forwarding to common uploader function.
        this.uploadFiles(this._file.files);

        // Making sure we set the file input's value to null, in case user tries to upload the same file once more later.
        this._file.value = null;
    };


    // Invoked when user drops a file unto surface of main widget.
    p5.uploader.prototype.onDrop = function (e) {

        // Making sure we prevent default logic, since that would simply load up the files in browser.
        e.preventDefault ();

        // Forwarding to common uploader function.
        this.uploadFiles (e.dataTransfer.files);
    };


    // Triggered when file input's value was changed.
    p5.uploader.prototype.uploadFiles = function (files) {

        // Checking if file input's files are accepted as valid input.
        if (!this.checkFile (files)) {

            // File input is not valid, making sure we provide visual clues to user by setting its error CSS class.
            // Also making sure we remove the error CSS class after one second, such that we can set it again later successfully,
            // in case user does something else later, that also triggers an error.
            this._widget.el.className = this._cssClass + " " + this._errorClass;
            var self = this;
            setTimeout(function () { self._widget.el.className = self._cssClass; }, 1000);

        } else {

            // Checking if we actually have any files to push, and if so, starting the pushing, and changing the CSS class of widget.
            if (files.length > 0) {

                // This will create previewing of files uploaded.
                this.addPreview(files);

                // Changing CSS class to the specified "drop" CSS class.
                this._widget.el.className = this._cssClass + " " + this._dropClass;

                // Storing the files user wants to upload as an array.
                this._files = [];
                for (var i = 0; i < files.length; i++) {
                    this._files.push(files[i]);
                }

                // Storing the total file count on this, such that we can pass it in, on every Ajax upload request to server.
                this._count = this._files.length;
                this.processNext(0);
            }
        }
    };


    // Invoked when a dragleave operation occurs.
    p5.uploader.prototype.onDragLeave = function (e) {

        // Preventing default, and setting back CSS class to default class.
        e.preventDefault();
        this._widget.el.className = this._cssClass;
    };


    // Invoked when a dragover operation occurs.
    p5.uploader.prototype.onDragOver = function (e) {

        // Preventing default, and setting CSS class to "hover class".
        e.preventDefault();
        this._widget.el.className = this._cssClass + " " + this._hoverClass;
    };


    // Checks if all files are valid extensions according to initialization of object.
    // Notice, for security reasons, this logic is mirrored on the server.
    // However, to create an "early abort" for file types, and input, not accepted, we also do it here ...
    // This avoids user from uploading huge files, only to get a message that his huge file(s) was not accepted after having spent
    // tons of bandwidth and waiting for the file(s) to upload.
    p5.uploader.prototype.checkFile = function (files) {

        // Checking if user provided multiple files, and only one is allowed.
        if (this._multiple === false && files.length > 1) {

            // Widget only allows uploading one file, and multiple files were provided.
            return false;
        }

        // Checking if current instance has a filter.
        if (this._filter.length == 0) {

            // No filters, accepting everything.
            return true;
        }

        // Looping through files, making sure they match at least one of our filters.
        for (var idx = 0; idx < files.length; idx++) {

            // Filter(s) were provided, looping through them all, to verify file extension can be found in at least one of the filters provided.
            var splits = files[idx].name.split('.');
            var ext = splits[splits.length - 1];
            var found = false;
            for (var idxSplit = 0; idxSplit < this._filter.length; idxSplit++) {
                if (this._filter[idxSplit] == ext) {
                    found = true;
                    break;
                }
            }
            if (!found) {

                // File's extension was not found in our filters.
                // Hence, file(s) were not accepted.
                return false;
            }
        }

        // All files were accepted as input.
        return true;
    };


    // Creates previews of all supplied files.
    // If file is an image, the image will be displayed, otherwise a generic preview image will be displayed.
    p5.uploader.prototype.addPreview = function (files) {

        // Checking if we should add preview.
        if (this._preview === false) { return; }

        // Looping through all files supplied, and creating an image for each of them, appending it into main widget's surface as a child element.
        for (var idx = 0; idx < files.length; idx++) {

            // Checking if file is an image type.
            var splits = files[idx].name.split('.');
            var ext = (splits[splits.length - 1] || "").toLowerCase ();
            if (splits.length > 1 && (ext == "jpg" || ext == "jpeg" || ext == "gif" || ext == "png" || ext == "svg" || ext == "bmp")) {

                // File was an image, displaying actual image as preview.
                var img = document.createElement('img');
                var objectURL = URL.createObjectURL(files[idx]);
                img.src = objectURL;
                img.alt = files[idx].name;
                img.title = files[idx].name;
                img.className = 'uploader-widget-preview';
                this._widget.el.appendChild(img);

            } else {

                // File was not an image, displaying the "generic" preview span element instead.
                var span = document.createElement('span');
                span.className = 'uploader-widget-preview';
                this._widget.el.appendChild(span);
            }
        }
    };


    // Processing a single file, and recursively invokes self after upload is finished.
    // This means uploading image to server, and when file is uploaded, start processing the next file.
    p5.uploader.prototype.processNext = function (currentIdx) {

        // Retrieving next file in queue, removing it out of our queue, and pushing it to server.
        var f = this._files.splice(0, 1)[0];
        var reader = new FileReader();
        var self = this;
        reader.onload = function (e) {

            // Raising hidden ".onupload" Ajax event on main widget, supplying an "onbefore" callback to p5.ajax, where we
            // add the actual image data to the HTTP POST parameters collection.
            // In addition, we provide an "onsuccess" callback, where we remove the preview image, and start processing the next file.
            self._widget.raise('.onupload', {

                // Here we simply add up all parameters to our Ajax request, which means our currently iterated image data.
                onbefore: function (pars, evt) {
                    pars.push(['sys42.widgets.uploader.count', self._count]);
                    pars.push(['sys42.widgets.uploader.current', currentIdx]);
                    pars.push(['sys42.widgets.uploader.filename', f.name]);
                    pars.push(['sys42.widgets.uploader.content', btoa(e.target.result)]);
                },

                // Removing preview image for currently iterated file, and recursively invokes "self" to start uploading our next file, 
                // if there are any more files.
                // Otherwise, sets back CSS class to default class.
                onsuccess: function (serverReturn, evt) {

                    // Checking if we have added previews.
                    var w = self._widget.el;
                    if (self._preview) {
                        for (var idx = 0; idx < w.childNodes.length; idx++) {
                            var ix = w.childNodes[idx];
                            if (ix.tagName == 'IMG' || ix.tagName == 'SPAN') {
                                w.removeChild(ix);
                                break;
                            }
                        }
                    }
                    if (self._files.length > 0) {

                        // Processing next file.
                        self.processNext(++currentIdx);
                    } else {

                        // We're done, no more files.
                        w.className = self._cssClass;
                    }
                }
            });
        };
        reader.readAsBinaryString(f);
    };
})();