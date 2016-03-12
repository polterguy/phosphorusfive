
/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details.
 */

(function() {


    /*
     * Main namespace
     * 
     * All javascript functionality in p5.ajax can be found in 
     * the 'p5' namespace
     * 
     * As a general rule, all functions and properties starting with an 
     * underscore ('_') are not meant to be used directly, but are for 
     * internal use in the library. All other methods can be used for 
     * your convenience as you wish
     */
    window.p5 = {};


    /*
     * Extends 'orig' with values from 'obj'
     * 
     * Extends the 'orig' object with all values from the 'obj' object 
     * and returns 'orig' back to caller
     */
    window.p5.extend = function(orig, obj) {
        for (var p in obj) {
            if (obj.hasOwnProperty(p)) {
                orig[p] = obj[p];
            }
        }
        return orig;
    };


    /*
     * Returns a 'p5.element' wrapping a dom element
     * 
     * Pass in the 'id' of the element you wish to wrap
     * alternatively; pass in the dom element directly
     */
    window.p5.$ = function(id) {
        if (id.parentNode) {
            return new window.p5.element(id);
        } else {
            return new window.p5.element(document.getElementById(id));
        }
    };


    /*
     * Raise server side event
     *
     * Raises a server side event for a dom element
     * 'event' is the dom event
     */
    window.p5.e = function(event) {
        var el = window.p5.$(event.currentTarget);
        el.raise("on" + event.type);
        event.preventDefault();
        event.stopPropagation();
    };


    /*
     * Returns the new value for element property/attribute
     *
     * The server can return 5 possible different values for updating properties and attributes;
     * 
     *   - string            Which contains the new value of the property/attribute)
     * 
     *   - [number]          Which means the existing value is to have everything removed 
     *                       starting from 'number' position
     * 
     *   - [string]          Which means the existing value should have the given 'string' 
     *                       concatenated to its existing value
     *
     *   - [number, string]  Which means the given string it to be shortened from 'number' 
     *                       position and have 'string' concatenated to its existing value
     *
     *   - null              Property/attribute set to null
     *
     * This function returns the new value of the property/attribute according to the 'val'
     * object given
     */
    window.p5._getChange = function(old, val) {
        if (val !== null) {
            /*
             * Making sure we normalize carriage returns in old values before calculating offset
             */
            if (old != null && old.indexOf != null && old.indexOf('\r') == -1) {
                old = old.replace ('\n', '\r\n');
            }
            var decodeHtml = function (val) {
                var txt = document.createElement ('textarea');
                txt.innerHTML = val;
                return txt.value;
            };
            if (typeof val === "object") {
                if (val.length === 2) {
                    return old.substring(0, val[0]) + decodeHtml (val[1]); // Removing from 'number' and concatenating 'string'
                } else {
                    if (typeof val[0] === "number") {
                        return old.substring(0, val[0]); // Removing from 'number'
                    } else {
                        return old + decodeHtml (val[0]); // Only concatenating to existing value
                    }
                }
            }
        }
        return val; // Completely new value
    };


    /*
     * Wraps a dom element
     *
     * The p5.element type wraps a dom element with some handy 
     * helper functions for raising server side http ajax requests, 
     * and help handle the return from the server to update the dom
     */
    window.p5.element = function(el) {
        this.el = el;
    };

    window.p5.element.prototype = {


        /*
         * Returns named ancestor
         *
         * Returns the first ancestor it can find with the given 'name'.
         * useful for finding the form for an element, to find out where 
         * we should post or http request
         */
        _getAncestor: function(name) {
            var n = this.el;
            while (n.tagName.toLowerCase() !== name) {
                n = n.parentNode;
            }
            return window.p5.$(n);
        },


        /*
         * These functions are specialized JSON handlers for changing attributes given from server as JSON
         */
        Tag: function(value) {
            var oldHtml = this.el.outerHTML;
            var nHtml = "<" + value + oldHtml.substring(this.el.tagName.length + 1);
            nHtml = nHtml.substring(0, nHtml.length - (this.el.tagName.length + 1));
            nHtml += value + ">";
            var id = this.el.id;
            this.el.outerHTML = nHtml;
            this.el = window.p5.$(id).el; // updating element, since previous element is now gone
        },


        outerHTML: function(value) {
            var id = this.el.id;
            this.el.outerHTML = window.p5._getChange(this.el.outerHTML, value);
            this.el = window.p5.$(id).el; // updating element, since previous element is now gone
        },


        innerValue: function(value) {
            if (this.el.tagName === "TEXTAREA") {
                this.el.value = window.p5._getChange(this.el.value, value);
            } else {
                this.el.innerHTML = window.p5._getChange(this.el.innerHTML, value);
            }
        },


        "class": function(value) {
            this.el.className = window.p5._getChange(this.el.className, value);
        },


        value: function(value) {
            if (this.el.tagName.toLowerCase () == 'select') {
                value = window.p5._getChange(function(){
                    var ret = '';
                    for (var idxEl = 0; idxEl < this.el.children.length; idxEl++) {
                        if (this.el.children[idxEl].selected) {
                            ret += this.el.children[idxEl].value + ',';
                        }
                    }
                    return ret.substring (0, ret.length - 2);
                }.apply (this), value);
                for (var idxEl = 0; idxEl < this.el.children.length; idxEl++) {
                    this.el.children[idxEl].selected = false;
                }
                var splits = value.split (',');
                for (var idxVal = 0; idxVal < splits.length; idxVal++) {
                    for (var idxEl = 0; idxEl < this.el.children.length; idxEl++) {
                        if (splits[idxVal] == this.el.children[idxEl].value || splits[idxVal] == this.el.children[idxEl].innerHTML) {
                            this.el.children[idxEl].selected = true;
                        }
                    }
                }
            } else {
                this.el.value = window.p5._getChange(this.el.value, value);
            }
        },


        style: function(value) {
            this.el.style.cssText = window.p5._getChange(this.el.style.cssText, value);
        },


        /*
         * These next functions are handlers for deleting attributes
         */
        _p5_del: function(value) {
            for (var idx = 0; idx < value.length; idx++) {
                var atr = value[idx];
                if (this["_p5_del_" + atr]) {
                    this["_p5_del_" + atr]();
                } else {
                    this.el.removeAttribute(atr);
                }
            }
        },


        _p5_del_innerValue: function() {
            if (this.el.tagName === "TEXTAREA") {
                this.el.value = "";
            } else {
                this.el.innerHTML = "";
            }
        },


        _p5_del_selected: function() {
            this.el.selected = false;
        },


        _p5_del_checked: function() {
            this.el.checked = false;
        },


        _p5_del_class: function() {
            this.el.className = "";
        },


        _p5_del_value: function() {
            this.el.value = "";
        },


        _p5_del_style: function() {
            this.el.style.cssText = "";
        },


        /*
         * Sets the 'key' property/attribute on dom element with 'value'
         *
         * Will update one dom element's attribute/property according to 
         * the return value from the server. might also delete an attribute
         * entirely. uses p5._getChange internally to figure out how to 
         * update the given 'key' attribute. This is the main function in 
         * javascript to update dom elements according to the return value 
         * from the server after an ajax http request
         */
        _set: function(key, value) {

            // First checking special cases
            if (this[key]) {

                // This is a special case
                this[key](value);
            } else {
                if (key.indexOf("__p5_add_") !== -1) {

                    // Inserting html child widget
                    var pos = parseInt(key.substring(9), 10);
                    var fragment = document.createDocumentFragment();
                    var tmpEl = document.createElement(this.el.tagName);
                    tmpEl.innerHTML = value;
                    fragment.appendChild(tmpEl.firstChild);
                    this.el.insertBefore(fragment, this.el.children[pos]);
                } else {

                    // Checking if this is simply an "add attribute" setter
                    if (value === null) {

                        // Simply adding empty attribute
                        this.el[key] = true;
                    } else {

                        // Default logic, simply setting attribute, with no fuzz
                        this.el.setAttribute(key, window.p5._getChange(this.el[key], value));
                    }
                }
            }
        },


        /*
         * Serialize all form elements
         * 
         * Will serialize all form elements beneath the 'this' element and 
         * return as key/value object back to caller. Will correctly apply 
         * http and html standards to decide if element should be serialized 
         * or not, such as avoiding elements that are 'disabled' and have no 
         * 'name' attribute, etc
         */
        serialize: function() {
            var val = [];
            var els = this.el.getElementsByTagName("*");
            for (var i = 0; i < els.length; i++) {
                var el = els[i];
                if (!el.disabled && el.name) {
                    switch (el.tagName.toLowerCase()) {
                    case "input":
                        switch (el.type) {
                        case "submit":
                            break; // Don't push submit input types
                        case "checkbox":
                        case "radio":
                            if (el.checked) { // Only push checked items
                                val.push ([el.name, el.value]);
                            }
                            break;
                        default: // Defaulting to push value to server
                            val.push ([el.name, el.value]);
                            break;
                        }
                        break;
                    case "textarea":
                        /*
                         * Making sure we "normalize" carriage returns before we push them to server
                         */
                        var value = el.value;
                        if (value.indexOf('\r') == -1) {
                            value = value.replace('\n', '\r\n');
                        }
                        val.push ([el.name, value]);
                        break;
                    case "select":
                        for (var i2 = 0; i2 < el.options.length; i2++) {
                            if (el.options[i2].selected) {
                                val.push ([el.name, el.options[i2].value]);
                            }
                        }
                        break;
                    }
                }
            }
            return val;
        },


        /*
         * Sends one request to the server
         * 
         * Creates one http ajax request, and sends it to the server
         */
        _raise: function(evt, options) {

            // Finding form
            var form = this._getAncestor("form");

            // Creating our xhr object
            var xhr = new XMLHttpRequest();
            xhr.open("POST", form.el.action, true);
            xhr.setRequestHeader("Content-Type", "application/x-www-form-urlencoded");
            var t = this;
            xhr.onreadystatechange = function() {
                if (xhr.readyState === 4) {
                    t._done(xhr);
                }
            };

            // Serializing form before we call 'onbefore'
            var pars = form.serialize();
            options.onbefore.apply(this, [pars, evt]);

            // Sending request
            var body = "_p5_event=" + evt + "&_p5_widget=" + this.el.id;
            for (var idx = 0; idx < pars.length; idx++) {
                body += "&";
                var val = pars[idx];
                body += val[0] + "=" + encodeURIComponent(val[1]);
            }
            xhr.send(body);
        },


        /*
         * Raise the given 'evt' event with the given 'options'
         * 
         * Will create an http ajax request, and send to the server, raising 
         * the given 'evt' event on the widget. pass in options to further 
         * control how your request is being initiated
         *
         * Example;
         *
         * p5.$('my_id').raise('onclick', {
         *   onbefore:     function(pars, evt) { // do stuff just before the request is being sent},
         *   onsuccess:    function(serverReturn, evt) { // do stuff after successful return, but before dom is updated },
         *   onerror:      function(statusCode, statusText, responseHtml, evt) { // du stuff in case of error }
         * });
         * 
         * The above example will raise the event 'onclick' on the server side widget with the id 
         * of 'my_id'. It will also add up the custom parameters found in the 'parameters' parts.
         * the 'and_even_an_object' will be serialized as json with the http post request id of 
         * 'and_even_an_object', and the other parameters will simply be transferred as is, with 
         * their names being the http post request id used to extract them on the server side
         *
         * 'onbefore' will be called just before the http request is sent
         * 'onsuccess' will be called when response returns, but before request is evaluated
         * 'onerror' will be called if something goes wrong with the request. Unless your 'onerror' 
         * handler returns true, then all chained requests will be stopped
         *
         * All options are optional, and may be excluded entirely
         *
         * The request might not be posted immediately, since all requests are queued up, to make 
         * sure that there is only one active request at the time. This is because form elements 
         * and other elements might change as a consequence of one request, which would change how 
         * the next request posts data to the server
         *
         * If there are multiple requests in your chain, and an error occurs, then all other 
         * chained requests will be removed from the chain and not posted
         *
         * If the dom element that originally creates a request is removed before it is executed 
         * and initiated, then the request will never be posted, but simply moved out of the queue
         *
         * On the server side, there must exist a widget with the same id as the dom element you're
         * initiating the request on behalf of, that have an event handler for the 'evt' you raise, 
         * otherwise an error will be returned
         */
        raise: function(evt, options) {
            options = window.p5.extend({

                // Invoked before http request is sent, with parameters as object, and event name as evt
                onbefore: function( /*pars, evt*/) {},

                // Invoked after http request is returned, but before dom is updated, with json object as parameter, and evt as event name
                onsuccess: function( /*serverReturn, evt*/) {},

                // Invoked if an error occurs during http request, with status code, status text, server response and event name
                onerror: this.onerror

            }, options);

            // Adding to chain
            window.p5._chain.push({
                evt: evt,
                el: this,
                options: options
            });

            // Processing chain, but only if there's exactly one request in chain, since otherwise chain 
            // will continue by itself until whole chain is empty
            if (window.p5._chain.length === 1) {
                window.p5._next();
            }
            return this;
        },


        /*
         * Default error handler, shows the 'blue (yellow on windows) screen of death', in a modal window
         */
        onerror: function(statusCode, statusText, responseHtml) {
            var pAct = document.activeElement;

            // Creating a semi-transparent wrapper element, to stuff our error iframe into
            var err = document.createElement("div");
            err.id = "__p5_error";
            err.style.cssText = "position:fixed;top:0;left:0;background-color:rgba(0,0,0,.7);height:100%;width:100%;z-index:10000;";

            // Creating an iframe for simplicity, such that we can "dump" error HTML into it, without having to parsee anything
            var ifr = document.createElement("iframe");
            ifr.style.cssText = "height:90%;width:90%;margin:2% 5% 2% 5%;";
            err.appendChild(ifr);

            // Creating a button, such that we can close our error window
            var btn = document.createElement("button");
            btn.innerHTML = "close";
            btn.style.cssText = "position:absolute;top:5px;right:5px;z-index:10001;display:block;width:100px;height:36px;font-size:18px;";
            btn.onclick = function() {
                var el = window.p5.$("__p5_error").el;
                el.parentNode.removeChild(el);
                pAct.focus();
            };
            btn.onkeyup = function(e) {
                if (e.keyCode === 27) {
                    var el = window.p5.$("__p5_error").el;
                    el.parentNode.removeChild(el);
                    pAct.focus();
                }
            };
            err.appendChild(btn);
            var body = document.getElementsByTagName("body")[0];
            body.appendChild(err);

            // We have to postpone this bugger till iframe is "attached"
            ifr.contentDocument.documentElement.innerHTML = responseHtml;
            btn.focus();
        },


        /*
         * Executed when http ajax response is returned from server
         * 
         * Checks the status of the response, and calls either 'onsuccess' or 
         * 'onerror' depending upon the http status code returned from the server 
         * before it evaluates the return value from the server, and initiates the 
         * next request in the chain, if there are any more requests in chain
         *
         * For internal use only
         */
        _done: function(xhr) {

            // Removing current request from chain
            var cur = window.p5._chain[0];
            var options = cur.options;

            if (xhr.status >= 200 && xhr.status < 300) {

                // Success, calling 'onsuccess' before response is evaluated
                var json = eval("(" + xhr.responseText + ")");
                options.onsuccess.apply(this, [json, cur.evt]);

                // ORDER COUNTS!!

                // Removing all removed widgets from dom
                var arr = json._p5_del || [];
                var el;
                for (var idx = 0; idx < arr.length; idx++) {
                    el = window.p5.$(arr[idx]).el;
                    el.parentNode.removeChild(el);
                }

                // Updating all properties and attributes
                arr = json.__p5_change || {};
                for (var idxEl in arr) {
                    if (arr.hasOwnProperty(idxEl)) {
                        el = window.p5.$(idxEl);
                        var p5Change = arr[idxEl];
                        for (var idxAtr in p5Change) {
                            if (p5Change.hasOwnProperty(idxAtr)) {
                                el._set(idxAtr, p5Change[idxAtr]);
                            }
                        }
                    }
                }

                // Inserting all stylesheet files sent from server
                arr = json._p5_css_files || [];
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
                // This can be both files and javascript inline inclusions.
                // After this is done, we execute the results from "send-script"
                this._includeScripts(json._p5_js_objects || [], json._p5_script || []);

                // Removing current request from queue
                window.p5._chain.splice(0, 1);
            } else {

                var cont = options.onerror.apply(this, [xhr.status, xhr.statusText, xhr.responseText, cur.evt]);
                if (cont === true) {
                    window.p5._chain.splice(0, 1);
                } else {
                    window.p5._chain = [];
                }
            }

            // Processing next request
            window.p5._next();
        },

        _includeScripts: function(arr, sent) {
            if (arr.length > 0) {
                var T = this;
                if (arr[0]['Item2'] === true) {

                    // JavaScript file
                    var xhr2 = new XMLHttpRequest();
                    xhr2.open("GET", arr[0]['Item1'], true);
                    xhr2.onload = function() {
                        if (xhr2.readyState === 4) {
                            if (xhr2.status === 200) {
                                eval(xhr2.responseText);
                                arr.splice(0, 1);
                                T._includeScripts(arr, sent);
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

                    // JavaScript object inclusion
                    eval(arr[0]['Item1']);
                    arr.splice(0, 1);
                    this._includeScripts(arr, sent);
                }
            } else {

                // Finally, in the end, executing all the JavaScript sent from server
                for (var idxScript = 0; idxScript < sent.length; idxScript++) {
                    eval(sent[idxScript]);
                }
            }
        }
    };


    /*
     * Holds our chain of http ajax requests
     *
     * For internal use only
     */
    window.p5._chain = [];


    /*
     * Initiaties the next request in chain
     *
     * Requests will be initiatied in 'first in, first out' order, 
     * meaning the 0th request will be initiated before the 1st 
     * request, and so on
     *
     * For internal use only
     */
    window.p5._next = function() {

        // Checking if we have anymore htttp requests in our chain
        if (window.p5._chain.length > 0) {
            var cur = window.p5._chain[0];

            // Checking if dom element is still around, or if a previous request has removed it
            var el = window.p5.$(cur.el.el.id);
            if (!el.el) {

                // Element was removed from DOM, and request cannot be initiated
                // skipping this request, and initiating the next one instead
                window.p5._chain.splice(0, 1)[0];
                window.p5._next();
            } else {
                el._raise(cur.evt, cur.options);
            }
        }
    };
})();
