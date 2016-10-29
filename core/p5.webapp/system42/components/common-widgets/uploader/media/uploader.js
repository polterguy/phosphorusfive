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
    window.p5.uploader = function (widget, cssClass, hoverClass, dropClass) {

        // Initializing.
        this._widget      = p5.$(widget);
        this._cssClass    = cssClass;
        this._hoverClass  = hoverClass;
        this._dropClass   = dropClass;

        // Making sure we handle our related DOM events here.
        var self = this;

        // First the DOM event handler for what happens when a file is dragged over it.
        this._widget.el.addEventListener('dragover', function (e) {
            e.preventDefault();
            self._widget.el.className = self._hoverClass;
        }, false);

        // Then the DOM event handler for what happens when the user drags the file away from our widget.
        this._widget.el.addEventListener('dragleave', function (e) {
            e.preventDefault();
            self._widget.el.className = self._cssClass;
        }, false);

        // Then the DOM event handler for what happens when a file is dropped unto widget.
        this._widget.el.addEventListener('drop', function (e) {
            e.preventDefault();

            // Checking if we actually have any files to push, and if so ,starting the pushing, and changing the CSS class of widget.
            if (e.dataTransfer.files.length > 0) {
                self._widget.el.className = self._dropClass;
                self._files = [];
                for (var i = 0; i < e.dataTransfer.files.length; i++) {
                    self._files.push(e.dataTransfer.files[i]);
                }
                self._count = self._files.length;
                self.processNext(1);
            }
        }, false);
    };

    // Processing a single file, and recursively invokes self.
    window.p5.uploader.prototype.processNext = function (currentIdx) {

        // Retrieving next file in queue, removing it out of our queue, and pushing it to server.
        var f = this._files.splice (0,1)[0];
        var reader = new FileReader();
        var self = this;
        reader.onload = function (e) {
            self._widget.raise('_onupload', {
                onsuccess: function (serverReturn, evt) {
                    if (self._files.length > 0) {
                        self.processNext (++currentIdx);
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
