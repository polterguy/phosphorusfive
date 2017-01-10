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

(function() {


    /*
     * Main namespace.
     * 
     * All javascript functionality in p5.ajax can be found in 
     * the 'p5' namespace.
     * 
     * As a general rule, all functions and properties starting with an 
     * underscore ('_') are not meant to be used directly, but are for 
     * internal use in the library. All other methods can be used for 
     * your convenience as you wish.
     */
    p5 = {};


    /*
     * Extends 's' with values from 'b'.
     * 
     * Extends the 's' object with all values from the 'b' object 
     * and returns 's' back to caller.
     */
    p5.extend = function(s, b) {
        for (var i in b) {
            if (b.hasOwnProperty(i)) {
                s[i] = b[i];
            }
        }
        return s;
    };


    /*
     * Returns a 'p5.el' wrapping a dom element.
     * 
     * Pass in the 'id' (as 'i') of the element you wish to wrap.
     */
    p5.$ = function(i) {
        if (i instanceof HTMLElement) {
            return new p5.el(i);
        } else {
            return new p5.el(document.getElementById(i));
        }
    };


    /*
     * Raise a server side event.
     *
     * Raises a server side event for a DOM element.
     * 'e' is the DOM event.
     */
    p5.e = function(e) {
        var el = p5.$(e.currentTarget);
        el.raise('on' + e.type);
        e.preventDefault();
        e.stopPropagation();
    };


    /*
     * Wraps a DOM element, providing helper functions.
     *
     * The p5.el type wraps a dom element with some handy helper functions for raising server side http ajax requests, 
     * and help handle the return from the server to update the DOM, among other things.
     */
    p5.el = function(e) {
        this.el = e;
    };


    /*
     * Prototype for our 'p5.el' type.
     */
    p5.el.prototype = {


        /*
         * Returns the form ancestor HTML element, from the given element.
         */
        _form: function() {
            var n = this.el;
            while (n.tagName != 'FORM') {
                n = n.parentNode;
            }
            return p5.$(n);
        },


        /*
         * These functions are specialized JSON handlers for changing attributes given from server as JSON.
         */
        outerHTML: function(v) {
            var i = this.el.id;
            this.el.outerHTML = v;

            // Updating element, since previous element is now gone.
            this.el = p5.$(i).el;
        },


        innerValue: function (v) {
            if (this.el.tagName === 'TEXTAREA') {

                // Special handling of textarea type of element, since its innerHTML actually is its value.
                this.el.value = v;
            } else {

                // All other types of elements, besides textares.
                this.el.innerHTML = v;
            }
        },


        "class": function(v) {
            this.el.className = v;
        },


        style: function (v) {

            this.el.style.cssText = v;
        },


        value: function (v) {

            this.el.value = v;
        },


        /*
         * These next functions are handlers for deleting attributes.
         *
         * First the 'generic' version.
         */
        _p5_del: function (v) {

            // Looping through all attributes we should delete.
            for (var i = 0; i < v.length; i++) {
                var a = v[i];
                if (this['_p5_del_' + a]) {

                    // Invoking specialized delete attribute function.
                    this['_p5_del_' + a]();
                } else {

                    // Default removal implementation for attributes.
                    this.el.removeAttribute(a);
                }
            }
        },


        /*
         * Then the 'specialized' versions.
         */
        _p5_del_innerValue: function() {
            if (this.el.tagName === 'TEXTAREA') {
                this.el.value = '';
            } else {
                this.el.innerHTML = '';
            }
        },


        _p5_del_checked: function() {
            this.el.checked = false;
        },


        _p5_del_class: function() {
            this.el.className = '';
        },


        _p5_del_value: function() {
            this.el.value = '';
        },


        _p5_del_style: function() {
            this.el.style.cssText = '';
        },


        /*
         * Sets the 'k' property/attribute on DOM element to 'v'.
         *
         * Will update one DOM element's attribute/property according to the return value from the server.
         * Might also delete an attribute entirely.
         */
        _set: function(k, v) {

            // First checking special cases.
            if (this[k]) {

                // This is a special case.
                this[k](v);
            } else if (k.indexOf("__p5_add_") != -1) {

                // Inserting HTML child element.
                var t = document.createElement(this.el.tagName);
                t.innerHTML = v;
                this.el.insertBefore(t.firstChild, this.el.children[parseInt(k.substring(9), 10)]);

            } else {

                // Default logic, simply setting attribute.
                this.el.setAttribute(k, v);
            }
        },


        /*
         * Serialize all form elements.
         * 
         * Will serialize all form elements beneath the 'this' element, and return as key/value object back to caller. 
         * Will correctly apply HTTP and HTML standards to decide if element should be serialized or not, such as avoiding 
         * elements that are 'disabled', or elements having no 'name' attribute, etc.
         *
         * To support 'option' elements with commas though (,) - It will URI encode option element's values.
         */
        serialize: function() {
            var val = [];
            var els = this.el.getElementsByTagName("*");
            for (var i = 0; i < els.length; i++) {
                var el = els[i];

                // Making sure element is not diabled, and that it has a 'name' attribute.
                if (!el.disabled && el.name) {

                    // Figuring out type of element, and serializing accordingly.
                    switch (el.tagName) {
                        case 'INPUT':

                            // Figuring out type of input, and serialize accordingly.
                            switch (el.type) {
                                case "submit":
                                    break; // Don't push submit input types.
                                case "checkbox":
                                case "radio":
                                    if (el.checked) { // Only push checked items.
                                        val.push ([el.name, encodeURIComponent (el.value)]);
                                    }
                                    break;
                                default: // Defaulting to push 'value'.
                                    val.push ([el.name, el.value]);
                                    break;
                            }
                            break;
                        case 'TEXTAREA':

                            /*
                             * Making sure we "normalize" carriage returns before we push them to server.
                             */
                            var value = el.value;
                            if (value.indexOf('\r') == -1 && value.indexOf('\n') != -1) {
                                value = value.replace('\n', '\r\n');
                            }
                            val.push ([el.name, value]);
                            break;
                        case 'SELECT':

                            // Looping through all 'option' elements, to figure out which is selected.
                            for (var i2 = 0; i2 < el.options.length; i2++) {
                                if (el.options[i2].selected) {

                                    // To support "," in values, we URI encode value.
                                    val.push ([el.name, encodeURIComponent (el.options[i2].value)]);
                                }
                            }
                            break;
                    }
                }
            }
            return val;
        },


        /*
         * Sends one request to the server.
         * 
         * Creates one HTTP Ajax request, and sends it to the server.
         */
        _r: function(evt, opt) {

            // Finding form
            var form = this._form();

            // Creating our xhr object
            var xhr = new XMLHttpRequest();
            xhr.open('POST', form.el.action, true);
            xhr.setRequestHeader('Content-Type', 'application/x-www-form-urlencoded');
            var t = this;
            xhr.onreadystatechange = function() {
                if (xhr.readyState === 4) {
                    t._done(xhr);
                }
            };

            // Serializing form before we call 'onbefore'.
            var pars = form.serialize();
            opt.onbefore.apply(this, [pars, evt]);

            // Sending request, making sure we pass in the widget that raised the event, and its event name.
            // Also making sure we put our __VIEWSTATE back into business, since it was (highly likely) removed when page was rendered.
            var body = "_p5_event=" + evt + "&_p5_widget=" + this.el.id;
            var vs = false;
            for (var idx = 0; idx < pars.length; idx++) {
                body += "&";
                var val = pars[idx];
                body += val[0] + "=" + encodeURIComponent(val[1]);
                if (val[0] == '__VIEWSTATE')
                    vs = true;
            }
            xhr.send(vs ? body : '__VIEWSTATE=&' + body);
        },


        /*
         * Raise the given 'evt' event with the given 'options'.
         * 
         * Will create an http ajax request, and send to the server, raising the given 'evt' event on the widget. 
         * Pass in options to further control how your request is being initiated.
         *
         * Example;
         *
         * p5.$('my_id').raise('onclick', {
         *   onbefore:     function(pars, evt) { // do stuff just before the request is being sent},
         *   onsuccess:    function(serverReturn, evt) { // do stuff after successful return, but before the DOM is updated },
         *   onerror:      function(statusCode, statusText, responseHtml, evt) { // du stuff in case of error }
         * });
         * 
         * The above example will raise the event 'onclick' on the server side widget with the id 
         * of 'my_id'. It will also add up the custom parameters found in the 'parameters' parts.
         * the 'and_even_an_object' will be serialized as JSON with the HTTP POST request id of 
         * 'and_even_an_object', and the other parameters will simply be transferred as is, with 
         * their names being the http post request id used to extract them on the server side.
         *
         * 'onbefore' will be called just before the http request is sent.
         * 'onsuccess' will be called when response returns, but before request is evaluated.
         * 'onerror' will be called if something goes wrong with the request.
         * Unless your 'onerror' handler returns true, then all chained requests will be stopped.
         *
         * All options are optional, and may be excluded entirely.
         *
         * The request might not be posted immediately, since all requests are queued up, to make 
         * sure that there is only one active request at the time. This is because form elements 
         * and other elements might change as a consequence of one request, which would change how 
         * the next request posts data to the server.
         *
         * If there are multiple requests in your chain, and an error occurs, then all other 
         * chained requests will be removed from the chain and never posted.
         *
         * If the DOM element that originally creates a request is removed before it is executed 
         * and initiated, then the request will never be posted, but simply moved out of the queue.
         *
         * On the server side, there must exist a widget with the same id as the DOM element you're
         * initiating the request on behalf of, that have an event handler for the 'evt' you raise, 
         * otherwise an error will be returned.
         */
        raise: function (evt, opt) {

            // Applying default options.
            opt = p5.extend({

                // Invoked before HTTP request is sent, with parameters as object, and event name as evt.
                onbefore: function( /*pars, evt*/) {},

                // Invoked after HTTP request is returned, but before DOM is updated, with JSON object as parameter, 
                // and evt as event name.
                onsuccess: function( /*serverReturn, evt*/) {},

                // Invoked if an error occurs during the HTTP request, with status code, status text, server response, and event name.
                onerror: this.onerror

            }, opt);

            // Adding to chain.
            p5._chain.push({
                evt: evt,
                el: this,
                opt: opt
            });

            // Processing chain, but only if there's exactly one request in chain, since otherwise chain 
            // will continue by itself, until the whole chain is empty.
            if (p5._chain.length === 1) {
                p5._next();
            }
            return this;
        },


        /*
         * Default error handler, shows the 'blue (yellow on windows) screen of death', in a modal window.
         */
        onerror: function(statusCode, statusText, responseHtml) {

            // Creating a semi-transparent wrapper element, to stuff our error iframe into
            var err = document.createElement("div");
            err.id = "__p5_error";
            err.className = "p5-exception";

            // Creating a button, such that we can close our error window
            var btn = document.createElement("button");
            btn.innerHTML = "Close";
            btn.onclick = function() {
                var el = p5.$("__p5_error").el;
                el.parentNode.removeChild(el);
            };
            btn.onkeyup = function(e) {
                if (e.keyCode === 27) {
                    var el = p5.$("__p5_error").el;
                    el.parentNode.removeChild(el);
                }
            };
            err.innerHTML = responseHtml;
            err.appendChild(btn);
            var body = document.getElementsByTagName("body")[0];
            body.appendChild(err);
            btn.focus();
        },


        /*
         * Executed when an HTTP Ajax response is returned from the server.
         * 
         * Checks the status of the response, and calls either 'onsuccess' or 'onerror', depending upon the HTTP Status code 
         * returned from the server, before it evaluates the return value from the server, and initiates the next request in the chain, 
         * if there are any more requests in chain.
         */
        _done: function(xhr) {

            // Removing current request from chain
            var cur = p5._chain[0];
            var opt = cur.opt;

            if (xhr.status >= 200 && xhr.status < 300) {

                // Success, calling 'onsuccess' before response is evaluated.
                var json = eval("(" + xhr.responseText + ")");
                opt.onsuccess.apply(this, [json, cur.evt]);

                // ORDER COUNTS!!

                // Removing all removed widgets from DOM.
                var arr = json._p5_del || [];
                var el;
                for (var idx = 0; idx < arr.length; idx++) {
                    el = p5.$(arr[idx]).el;
                    el.parentNode.removeChild(el);
                }

                // Updating all properties and attributes.
                arr = json.__p5_change || {};
                for (var idxEl in arr) {
                    if (arr.hasOwnProperty(idxEl)) {
                        el = p5.$(idxEl);
                        var p5Change = arr[idxEl];
                        for (var idxAtr in p5Change) {
                            if (p5Change.hasOwnProperty(idxAtr)) {
                                el._set(idxAtr, p5Change[idxAtr]);
                            }
                        }
                    }
                }

                // Inserting all stylesheet files sent from server.
                arr = json.__p5_css_files || [];
                for (var idxCss = 0; idxCss < arr.length; idxCss++) {
                    el = document.createElement("link");
                    var href = arr[idxCss];
                    el.href = href;
                    el.rel = "stylesheet";
                    el.type = "text/css";
                    var head = document.getElementsByTagName("head")[0];
                    head.appendChild(el);
                }

                // Inserting all JavaScript objects sent from server.
                // This can be both files and JavaScript inline inclusions.
                // After this is done, we execute the results from 'send-script'.
                this._incScr(json.__p5_js_objects || [], json.__p5_scripts || []);

                // Removing current request from queue
                p5._chain.splice(0, 1);

            } else {

                var cont = opt.onerror.apply(this, [xhr.status, xhr.statusText, xhr.responseText, cur.evt]);
                if (cont === true) {
                    p5._chain.splice(0, 1);
                } else {
                    p5._chain = [];
                }
            }

            // Processing next request.
            p5._next();
        },

        _incScr: function(arr, sent) {
            if (arr.length > 0) {
                var T = this;
                if (arr[0]['Item2'] == true) {

                    // JavaScript file.
                    var xhr2 = new XMLHttpRequest();
                    xhr2.open("GET", arr[0]['Item1'], true);
                    xhr2.onload = function() {
                        if (xhr2.readyState === 4) {
                            if (xhr2.status === 200) {
                                eval.call(window, xhr2.responseText);
                                arr.splice(0, 1);
                                T._incScr(arr, sent);
                            } else {
                                throw "Couldn't download JavaScript file; '" + arr[0]['Item1'] + "'";
                            }
                        }
                    };
                    xhr2.onerror = function() {
                        throw "Couldn't download JavaScript file; '" + arr[0]['Item1'] + "'";
                    };
                    xhr2.send(null);
                } else {

                    // JavaScript object inclusion.
                    eval.call(window, arr[0]['Item1']);
                    arr.splice(0, 1);
                    this._incScr(arr, sent);
                }
            } else {

                // Finally, in the end, executing all the JavaScript sent from server
                for (var idxScript = 0; idxScript < sent.length; idxScript++) {
                    eval.call(window, sent[idxScript]);
                }
            }
        }
    };


    /*
     * Holds our chain of HTTP Ajax requests.
     */
    p5._chain = [];


    /*
     * Initiaties the next request in chain.
     *
     * Requests will be initiatied in 'first in, first out' order, meaning the 0th request will be initiated before 
     * the 1st request, and so on.
     */
    p5._next = function() {

        // Checking if we have anymore HTTP requests in our chain.
        if (p5._chain.length > 0) {
            var cur = p5._chain[0];

            // Checking if DOM element is still around, or if a previous request has removed it.
            var el = p5.$(cur.el.el.id);
            if (!el.el) {

                // Element was removed from DOM, and request cannot be initiated
                // skipping this request, and initiating the next one instead
                p5._chain.splice(0, 1)[0];
                p5._next();
            } else {
                el._r(cur.evt, cur.opt);
            }
        }
    };
})();
