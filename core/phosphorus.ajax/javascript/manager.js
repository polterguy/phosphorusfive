
/*
 * phosphorus five, copyright 2014 - Mother Earth, Jannah, Gaia
 * phosphorus five is licensed as mitx11, see the enclosed LICENSE file for details
 */

(function() {


    /*
     * main namespace
     * 
     * all javascript functionality in phosphorus.ajax can be found in 
     * the 'pf' namespace
     * 
     * as a general rule, all functions and properties starting with an 
     * underscore ('_') are not meant to be used directly, but are for 
     * internal use in the library. all other methods can be used for 
     * your convenience as you wish
     */
    window.pf = {};


    /*
     * extends 'orig' with values from 'obj'
     * 
     * extends the 'orig' object with all values from the 'obj' object 
     * and returns 'orig' back to caller
     */
    window.pf.extend = function(orig, obj) {
        for (var p in obj) {
            if (obj.hasOwnProperty(p)) {
                orig[p] = obj[p];
            }
        }
        return orig;
    };


    /*
     * returns a 'pf.element' wrapping a dom element
     * 
     * pass in the 'id' of the element you wish to wrap
     * alternatively; pass in the dom element directly
     */
    window.pf.$ = function(id) {
        if (id.parentNode) {
            return new window.pf.element(id);
        } else {
            return new window.pf.element(document.getElementById(id));
        }
    };


    /*
     * raise server side event
     *
     * raises a server side event for a dom element
     * 'event' is the dom event
     */
    window.pf.e = function(event) {
        var el = window.pf.$(event.currentTarget);
        el.raise("on" + event.type);
        event.preventDefault();
    };


    /*
     * returns the new value for element property/attribute
     *
     * the server can return 5 possible different values for updating properties and attributes;
     * 
     *   - string            (which contains the new value of the property/attribute)
     * 
     *   - [number]          (which means the existing value is to have everything removed 
     *                        starting from 'number' position)
     * 
     *   - [string]          (which means the existing value should have the given 'string' 
     *                        concatenated to its existing value)
     *
     *   - [number, string]  (which means the given string it to be shortened from 'number' 
     *                        position and have 'string' concatenated to its existing value)
     *
     *   - null              (property/attribute set to null)
     *
     * this function returns the new value of the property/attribute according to the 'val'
     * object given
     */
    window.pf._getChange = function(old, val) {
        if (val !== null) {
            if (typeof val === "object") {
                if (val.length === 2) {
                    return old.substring(0, val[0]) + val[1]; // removing from 'number' and concatenating 'string'
                } else {
                    if (typeof val[0] === "number") {
                        return old.substring(0, val[0]); // removing from 'number'
                    } else {
                        return old + val[0]; // only concatenating to existing value
                    }
                }
            }
        }
        return val; // completely new value
    };


    /*
     * wraps a dom element
     *
     * the pf.element type wraps a dom element with some handy 
     * helper functions for raising server side http ajax requests, 
     * and help handle the return from the server to update the dom
     */
    window.pf.element = function(el) {
        this.el = el;
    };

    window.pf.element.prototype = {


        /*
         * returns named ancestor
         *
         * returns the first ancestor it can find with the given 'name'.
         * useful for finding the form for an element, to find out where 
         * we should post or http request
         */
        _getAncestor: function(name) {
            var n = this.el;
            while (n.tagName.toLowerCase() !== name) {
                n = n.parentNode;
            }
            return window.pf.$(n);
        },


        /*
         * these functions are JSON handlers for changing attributes given from server as JSON
         */
        Tag: function(value) {
            var oldHtml = this.el.outerHTML;
            var nHtml = "<" + value + oldHtml.substring(this.el.tagName.length + 1);
            nHtml = nHtml.substring(0, nHtml.length - (this.el.tagName.length + 1));
            nHtml += value + ">";
            var id = this.el.id;
            this.el.outerHTML = nHtml;
            this.el = window.pf.$(id).el; // updating element, since previous element is now gone
        },


        outerHTML: function(value) {
            var id = this.el.id;
            this.el.outerHTML = window.pf._getChange(this.el.outerHTML, value);
            this.el = window.pf.$(id).el; // updating element, since previous element is now gone
        },


        innerValue: function(value) {
            if (this.el.tagName === "TEXTAREA") {
                this.el.value = window.pf._getChange(this.el.value, value);
            } else {
                this.el.innerHTML = window.pf._getChange(this.el.innerHTML, value);
            }
        },


        "class": function(value) {
            this.el.className = window.pf._getChange(this.el.className, value);
        },


        value: function(value) {
            if (this.el.tagName.toLowerCase () == 'select') {
                value = window.pf._getChange(function(){
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
                this.el.value = window.pf._getChange(this.el.value, value);
            }
        },


        style: function(value) {
            this.el.style.cssText = window.pf._getChange(this.el.style.cssText, value);
        },


        /*
         * these next functions are handlers for deleting attributes
         */
        __pf_del: function(value) {
            for (var idx = 0; idx < value.length; idx++) {
                var atr = value[idx];
                if (this["__pf_del_" + atr]) {
                    this["__pf_del_" + atr]();
                } else {
                    this.el.removeAttribute(atr);
                }
            }
        },


        __pf_del_innerValue: function() {
            if (this.el.tagName === "TEXTAREA") {
                this.el.value = "";
            } else {
                this.el.innerHTML = "";
            }
        },


        __pf_del_selected: function() {
            this.el.selected = false;
        },


        __pf_del_checked: function() {
            this.el.checked = false;
        },


        __pf_del_class: function() {
            this.el.className = "";
        },


        __pf_del_value: function() {
            this.el.value = "";
        },


        __pf_del_style: function() {
            this.el.style.cssText = "";
        },


        /*
         * sets the 'key' property/attribute on dom element with 'value'
         *
         * will update one dom element's attribute/property according to 
         * the return value from the server. might also delete an attribute
         * entirely. uses pf._getChange internally to figure out how to 
         * update the given 'key' attribute. this is the main function in 
         * javascript to update dom elements according to the return value 
         * from the server after an ajax http request
         */
        _set: function(key, value) {

            // first checking special cases
            if (this[key]) {

                // this is a special case
                this[key](value);
            } else {
                if (key.indexOf("__pf_add_") !== -1) {

                    // inserting html child widget
                    var pos = parseInt(key.substring(9), 10);
                    var fragment = document.createDocumentFragment();
                    var tmpEl = document.createElement("div");
                    tmpEl.innerHTML = value;
                    fragment.appendChild(tmpEl.firstChild);
                    this.el.insertBefore(fragment, this.el.children[pos]);
                } else {

                    // checking if this is simply an "add attribute" setter
                    if (value === null) {

                        // simply adding empty attribute
                        this.el[key] = true;
                    } else {

                        // default logic, simply setting attribute, with no fuzz
                        this.el.setAttribute(key, window.pf._getChange(this.el[key], value));
                    }
                }
            }
        },


        /*
         * serialize all form elements
         * 
         * will serialize all form elements beneath the 'this' element and 
         * return as key/value object back to caller. will correctly apply 
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
                            break; // don't push submit input types
                        case "checkbox":
                        case "radio":
                            if (el.checked) { // only push checked items
                                val.push ([el.name, el.value]);
                            }
                            break;
                        default: // defaulting to push value to server
                            val.push ([el.name, el.value]);
                            break;
                        }
                        break;
                    case "textarea":
                        val.push ([el.name, el.value]);
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
         * sends one request to the server
         * 
         * creates one http ajax request, and sends it to the server
         */
        _raise: function(evt, options) {

            // finding form
            var form = this._getAncestor("form");

            // creating our xhr object
            var xhr = new XMLHttpRequest();
            xhr.open("POST", form.el.action, true);
            xhr.setRequestHeader("Content-Type", "application/x-www-form-urlencoded");
            var t = this;
            xhr.onreadystatechange = function() {
                if (xhr.readyState === 4) {
                    t._done(xhr);
                }
            };

            // serializing form before we call 'onbefore'
            var pars = form.serialize();
            options.onbefore.apply(this, [pars, evt]);

            // sending request
            var body = "__pf_event=" + evt + "&__pf_widget=" + this.el.id;
            for (var idx = 0; idx < pars.length; idx++) {
                body += "&";
                var val = pars[idx];
                body += val[0] + "=" + encodeURIComponent(val[1]);
            }
            xhr.send(body);
        },


        /*
         * raise the given 'evt' event with the given 'options'
         * 
         * will create an http ajax request, and send to the server, raising 
         * the given 'evt' event on the widget. pass in options to further 
         * control how your request is being initiated
         *
         * example;
         *
         * pf.$('my_id').raise('onclick', {
         *   onbefore:     function(pars, evt) { // do stuff just before the request is being sent},
         *   onsuccess:    function(serverReturn, evt) { // do stuff after successful return, but before dom is updated },
         *   onerror:      function(statusCode, statusText, responseHtml, evt) { // du stuff in case of error }
         * });
         * 
         * the above example will raise the event 'onclick' on the server side widget with the id 
         * of 'my_id'. it will also add up the custom parameters found in the 'parameters' parts.
         * the 'and_even_an_object' will be serialized as json with the http post request id of 
         * 'and_even_an_object', and the other parameters will simply be transferred as is, with 
         * their names being the http post request id used to extract them on the server side
         *
         * 'onbefore' will be called just before the http request is sent
         * 'onsuccess' will be called when response returns, but before request is evaluated
         * 'onerror' will be called if something goes wrong with the request. unless your 'onerror' 
         * handler returns true, then all chained requests will be stopped
         *
         * all options are optional, and may be excluded entirely
         *
         * the request might not be posted immediately, since all requests are queued up, to make 
         * sure that there is only one active request at the time. this is because form elements 
         * and other elements might change as a consequence of one request, which would change how 
         * the next request posts data to the server
         *
         * if there are multiple requests in your chain, and an error occurs, then all other 
         * chained requests will be removed from the chain and not posted
         *
         * if the dom element that originally creates a request is removed before it is executed 
         * and initiated, then the request will never be posted, but simply moved out of the queue
         *
         * on the server side, there must exist a widget with the same id as the dom element you're
         * initiating the request on behalf of, that have an event handler for the 'evt' you raise, 
         * otherwise an error will be returned
         */
        raise: function(evt, options) {
            options = window.pf.extend({

                // invoked before http request is sent, with parameters as object, and event name as evt
                onbefore: function( /*pars, evt*/) {},

                // invoked after http request is returned, but before dom is updated, with json object as parameter, and evt as event name
                onsuccess: function( /*serverReturn, evt*/) {},

                // invoked if an error occurs during http request, with status code, status text, server response and event name
                onerror: this.onerror

            }, options);

            // adding to chain
            window.pf._chain.push({
                evt: evt,
                el: this,
                options: options
            });

            // processing chain, but only if there's exactly one request in chain, since otherwise chain 
            // will continue by itself until whole chain is empty
            if (window.pf._chain.length === 1) {
                window.pf._next();
            }
        },


        /*
         * default error handler, shows the 'blue (yellow on windows) screen of death', in a modal window
         */
        onerror: function(statusCode, statusText, responseHtml) {
            var pAct = document.activeElement;

            // creating a semi-transparent wrapper element, to stuff our error iframe into
            var err = document.createElement("div");
            err.id = "__pf_error";
            err.style.cssText = "position:fixed;top:0;left:0;background-color:rgba(0,0,0,.7);height:100%;width:100%;z-index:10000;";

            // creating an iframe for simplicity, such that we can "dump" error HTML into it, without having to parsee anything
            var ifr = document.createElement("iframe");
            ifr.style.cssText = "height:90%;width:90%;margin:2% 5% 2% 5%;";
            err.appendChild(ifr);

            // creating a button, such that we can close our error window
            var btn = document.createElement("button");
            btn.innerHTML = "close";
            btn.style.cssText = "position:absolute;top:5px;right:5px;z-index:10001;display:block;width:100px;height:36px;font-size:18px;";
            btn.onclick = function() {
                var el = window.pf.$("__pf_error").el;
                el.parentNode.removeChild(el);
                pAct.focus();
            };
            btn.onkeyup = function(e) {
                if (e.keyCode === 27) {
                    var el = window.pf.$("__pf_error").el;
                    el.parentNode.removeChild(el);
                    pAct.focus();
                }
            };
            err.appendChild(btn);
            var body = document.getElementsByTagName("body")[0];
            body.appendChild(err);

            // we have to postpone this bugger till iframe is "attached"
            ifr.contentDocument.documentElement.innerHTML = responseHtml;
            btn.focus();
        },


        /*
         * executed when http ajax response is returned from server
         * 
         * checks the status of the response, and calls either 'onsuccess' or 
         * 'onerror' depending upon the http status code returned from the server 
         * before it evaluates the return value from the server, and initiates the 
         * next request in the chain, if there are any more requests in chain
         *
         * for internal use only
         */
        _done: function(xhr) {

            // removing current request from chain
            var cur = window.pf._chain[0];
            var options = cur.options;

            if (xhr.status >= 200 && xhr.status < 300) {

                // success, calling 'onsuccess' before response is evaluated
                var json = eval("(" + xhr.responseText + ")");
                options.onsuccess.apply(this, [json, cur.evt]);

                // ORDER COUNTS!!

                // removing all removed widgets from dom
                var arr = json.__pf_del || [];
                var el;
                for (var idx = 0; idx < arr.length; idx++) {
                    el = window.pf.$(arr[idx]).el;
                    el.parentNode.removeChild(el);
                }

                // updating all properties and attributes
                arr = json.__pf_change || {};
                for (var idxEl in arr) {
                    if (arr.hasOwnProperty(idxEl)) {
                        el = window.pf.$(idxEl);
                        var pfCssFiles = arr[idxEl];
                        for (var idxAtr in pfCssFiles) {
                            if (pfCssFiles.hasOwnProperty(idxAtr)) {
                                el._set(idxAtr, pfCssFiles[idxAtr]);
                            }
                        }
                    }
                }

                // inserting all stylesheet files sent from server
                arr = json.__pf_css_files || [];
                for (var idxCss = 0; idxCss < arr.length; idxCss++) {
                    el = document.createElement("link");
                    var href = arr[idxCss];
                    el.href = href;
                    el.rel = "stylesheet";
                    el.type = "text/css";
                    var head = document.getElementsByTagName("head")[0];
                    head.appendChild(el);
                }

                // inserting all JavaScript files sent from server
                arr = json.__pf_js_files || [];
                var idxScript;
                for (idxScript = 0; idxScript < arr.length; idxScript++) {
                    var xhr2 = new XMLHttpRequest();
                    xhr2.open("GET", arr[idxScript], false);
                    xhr2.onload = function() {
                        if (xhr2.readyState === 4) {
                            if (xhr2.status === 200) {
                                eval(xhr2.responseText);
                            } else {
                                alert("couldn't download JavaScript file; '" + arr[idxScript] + "'");
                            }
                        }
                    };
                    xhr2.onerror = function() {
                        alert("couldn't download JavaScript file; '" + arr[idxScript] + "'");
                    };
                    xhr2.send(null);
                }

                // executing all the JavaScript sent from server
                arr = json.__pf_script || [];
                for (idxScript = 0; idxScript < arr.length; idxScript++) {
                    eval(arr[idxScript]);
                }

                // removing current request from queue
                window.pf._chain.splice(0, 1);
            } else {

                var cont = options.onerror.apply(this, [xhr.status, xhr.statusText, xhr.responseText, cur.evt]);
                if (cont === true) {
                    window.pf._chain.splice(0, 1);
                } else {
                    window.pf._chain = [];
                }
            }

            // processing next request
            window.pf._next();
        }
    };


    /*
     * holds our chain of http ajax requests
     *
     * for internal use only
     */
    window.pf._chain = [];


    /*
     * initiaties the next request in chain
     *
     * requests will be initiatied in 'first in, first out' order, 
     * meaning the 0th request will be initiated before the 1st 
     * request, and so on
     *
     * for internal use only
     */
    window.pf._next = function() {

        // checking if we have anymore htttp requests in our chain
        if (window.pf._chain.length > 0) {
            var cur = window.pf._chain[0];

            // checking if dom element is still around, or if a previous request has removed it
            var el = window.pf.$(cur.el.el.id);
            if (!el.el) {
                // dom element was removed from dom, and request cannot be initiated
                // skipping this request, and initiating the next one instead
                window.pf._chain.splice(0, 1)[0];
                window.pf._next();
            } else {
                el._raise(cur.evt, cur.options);
            }
        }
    };

})();
