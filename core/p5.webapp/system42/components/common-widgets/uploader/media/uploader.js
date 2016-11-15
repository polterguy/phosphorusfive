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


(function () {

    // Constructor.
    p5.uploader = function (widget, cssClass, hoverClass, dropClass, errorClass, filter, multiple, file) {

        // Initializing.
        this._widget = p5.$(widget);
        this._cssClass = cssClass;
        this._hoverClass = hoverClass;
        this._dropClass = dropClass;
        this._errorClass = errorClass;
        this._filter = (filter == '' ? [] : filter.split('|'));
        this._multiple = multiple;
        this._file = p5.$(file).el;

        // Making sure we handle our related DOM events here.
        var self = this;

        // First the DOM event handler for what happens when widget is clicked.
        this._widget.el.addEventListener('click', function (e) {
            self._file.click();
        }, false);
        this._file.addEventListener('click', function (e) {
            e.stopPropagation();
        });

        // Onchange on file input element.
        this._file.addEventListener('change', function (e) {
            if (!self.checkFile(self._file.files)) {
                self._widget.el.className = self._cssClass + " " + self._errorClass;
                setTimeout(function () { self._widget.el.className = self._cssClass;}, 1000);
            } else {

                // Checking if we actually have any files to push, and if so ,starting the pushing, and changing the CSS class of widget.
                if (self._file.files.length > 0) {
                    self.addPreview(self._file.files);
                    self._widget.el.className = self._cssClass + " " + self._dropClass;
                    self._files = [];
                    for (var i = 0; i < self._file.files.length; i++) {
                        self._files.push(self._file.files[i]);
                    }
                    self._count = self._files.length;
                    self.processNext(0);
                }
            }
            self._file.value = null;
        });

        // Then the DOM event handler for what happens when a file is dragged over it.
        this._widget.el.addEventListener('dragover', function (e) {
            e.preventDefault();
            self._widget.el.className = self._cssClass + " " + self._hoverClass;
        }, false);

        // Then the DOM event handler for what happens when the user drags the file away from our widget.
        this._widget.el.addEventListener('dragleave', function (e) {
            e.preventDefault();
            self._widget.el.className = self._cssClass;
        }, false);

        // Then the DOM event handler for what happens when a file is dropped unto widget.
        this._widget.el.addEventListener('drop', function (e) {
            e.preventDefault();
            if (!self.checkFile(e.dataTransfer.files)) {
                self._widget.el.className = self._cssClass + " " + self._errorClass;
                setTimeout(function () { self._widget.el.className = self._cssClass; }, 1000);
            } else {

                // Checking if we actually have any files to push, and if so, starting the pushing, and changing the CSS class of widget.
                if (e.dataTransfer.files.length > 0) {
                    self.addPreview(e.dataTransfer.files);
                    self._widget.el.className = self._cssClass + " " + self._dropClass;
                    self._files = [];
                    for (var i = 0; i < e.dataTransfer.files.length; i++) {
                        self._files.push(e.dataTransfer.files[i]);
                    }
                    self._count = self._files.length;
                    self.processNext(0);
                }
            }
        }, false);
    };

    // Checks if all files are valid extensions according to initialization of object.
    p5.uploader.prototype.checkFile = function (files) {

        // Checking if current instance has a filter.
        if (this._filter.length == 0) {

            // No filters,accepting everything.
            return true;
        }

        // Checking if user provided multiple files, and only one is allowed.
        if (this._multiple === false && files.length > 1) {

            // Widget only allows uploading one file, and multiple files were provided.
            return false;
        }

        // Looping through files, making sure they match at least one filter.
        for (var idx = 0; idx < files.length; idx++) {

            // Filter were provided, looping through them all, to verify file extension can be found in at least one of the filters provided.
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
                return false;
            }
        }
        return true;
    };

    p5.uploader.prototype.addPreview = function (files) {
        for (var idx = 0; idx < files.length; idx++) {
            var img = document.createElement('img');
            var splits = files[idx].name.split('.');
            var ext = splits[splits.length - 1];
            if (splits.length > 1 && (ext == "jpg" || ext == "jpeg" || ext == "gif" || ext == "png")) {
                var objectURL = URL.createObjectURL(files[idx]);
                img.src = objectURL;
            } else {
                img.src = '/system42/components/common-widgets/uploader/media/preview.png';
            }
            img.alt = files[idx].name;
            img.title = files[idx].name;
            this._widget.el.appendChild(img);
        }
    };

    // Processing a single file, and recursively invokes self.
    p5.uploader.prototype.processNext = function (currentIdx) {

        // Retrieving next file in queue, removing it out of our queue, and pushing it to server.
        var f = this._files.splice(0, 1)[0];
        var reader = new FileReader();
        var self = this;
        reader.onload = function (e) {
            self._widget.raise('.onupload', {
                onsuccess: function (serverReturn, evt) {
                    for (var idx = 0; idx < self._widget.el.childNodes.length; idx++) {
                        if (self._widget.el.childNodes[idx].tagName == 'IMG') {
                            self._widget.el.removeChild (self._widget.el.children[idx]);
                            break;
                        }
                    }
                    if (self._files.length > 0) {
                        self.processNext(++currentIdx);
                    } else {
                        self._widget.el.className = self._cssClass;
                    }
                },
                onbefore: function (pars, evt) {
                    pars.push(['sys42.widgets.uploader.count', self._count]);
                    pars.push(['sys42.widgets.uploader.current', currentIdx]);
                    pars.push(['sys42.widgets.uploader.filename', f.name]);
                    pars.push(['sys42.widgets.uploader.content', btoa(e.target.result)]);
                }
            });
        };
        reader.readAsBinaryString(f);
    };
})();