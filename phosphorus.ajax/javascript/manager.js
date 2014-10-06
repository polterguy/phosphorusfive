/*
 * phosphorus five, copyright 2014 - thomas@magixilluminate.com
 * phosphorus five is licensed as mitx11, see the enclosed LICENSE file for details
 */

(function() {

  // main namespace
  pf = {};

  // 'oo' helper functions
  // make sure init is invoked when constructing new objects from classes
  pf.clazz = function() {
    return function() {
      if (this.init) {
        return this.init.apply(this, arguments);
      }
    };
  };

  // extends the given lhs with every property from rhs and return lhs
  pf.extend = function(orig, obj) {
    for (var p in obj) {
      orig[p] = obj[p];
    }
    return orig;
  };

  // returns a pf.element wrapping the given id html element
  pf.$ = function(id) {
    if (id.parentNode) {
      return new pf.element(id);
    } else {
      return new pf.element(document.getElementById(id));
    }
  };

  // creates and executes a new ajax request, used as event handler code for html onxxx attributes
  pf.e = function(event) {
    var el = pf.$(event.target);
    el.raise('on' + event.type);
  };

  // utility namespace
  pf.util = {};

  // returns the new value according to the given val
  pf.util.getChange = function(old, val) {
    if (val === null) {
      return;
    }
    if (typeof val === 'object') {
      if (val.length == 2) {
        return old.substring(0, val[0]) + val[1]; // trimming old, concatenating new
      } else {
        if (typeof val[0] === 'number') {
          return old.substring(old, val[0]); // only trimming old
        } else {
          return old + val[0]; // only concatenating
        }
      }
    } else {
      return val;
    }
  };

  // element type
  pf.element = pf.clazz();
  pf.element.prototype = {

    // initializer, el is expected to be either an id of an element, or a dom element
    init: function(el) {
      this.el = el;
    },

    // returns the first ancestor with the given name
    getNamedParent: function(name) {
      var n = this.el;
      while (n.tagName.toLowerCase() != name) {
        n = n.parentNode;
      }
      return pf.$(n);
    },

    // sets the given key attribute or property to the given value
    setVal: function(key, value) {

      // special handlers for some of our attributes
      switch(key) {
        case '__pf_del':
          this.el.removeAttribute(value);
          break;
        case 'tagName':
          var oldHtml = this.el.outerHTML;
          var nHtml = '<' + value + oldHtml.substring(this.el.tagName.length + 1);
          nHtml = nHtml.substring(0, nHtml.length - (this.el.tagName.length + 1));
          nHtml += value + '>';
          var id = this.el.id;
          this.el.outerHTML = nHtml;
          this.el = pf.$(id).el; // updating element since previous element is now gone
          break;
        case 'outerHTML':
          var id = this.el.id;
          this.el.outerHTML = pf.util.getChange(this.el.outerHTML, value);
          this.el = pf.$(id).el; // updating element since previous element is now gone
          break;
        case 'innerHTML':
          this.el.innerHTML = pf.util.getChange(this.el.innerHTML, value);
          break;
        default:
          this.el.setAttribute(key, pf.util.getChange(this.el[key], value));
          break;
      }
    },

    // serialize all form elements beneath this element
    serialize: function() {
      var val = {};
      var els = this.el.getElementsByTagName('*');
      for (var i = 0; i < els.length; i++) {
        var el = els[i];
        if (!el.disabled && el.name) {
          switch (el.tagName.toLowerCase()) {
            case 'input':
              switch (el.type) {
                case 'submit':
                  break; // don't push submit input types
                case 'checkbox':
                case 'radio':
                  if (el.checked) { // only push checked items
                    val[el.name] = el.value;
                  }
                  break;
                default: // defaulting to push value to server
                  val[el.name] = el.value;
                  break;
              } break;
            case 'textarea':
              val[el.name] = el.value;
              break;
            case 'select':
              for (var i = 0; i < el.options.length; i++) {
                if (el.options[i].selected) {
                  val[el.name] = el.options[i].value;
                  break;
                }
              } break;
          }
        }
      }
      return val;
    },

    // creates an xhr request towards the server
    _raise: function(evt, options) {

      // serializing form and other parameters
      var form = this.getNamedParent('form');
      var pars = form.serialize();
      pars['__pf_ajax'] = 1;
      pars['__pf_evt'] = evt;
      pars['__pf_wdg'] = this.el.id;

      // creating our xhr object
      var xhr = new XMLHttpRequest();
      xhr.open('POST', form.el.action, true);
      xhr.setRequestHeader("Content-Type", "application/x-www-form-urlencoded");
      var T = this;
      xhr.onreadystatechange = function() {
        if (xhr.readyState == 4) { T.done(xhr); }
      };

      // invoking onbefore
      options.onbefore.apply(this, [pars, evt]);

      // sending request
      var body = '';
      for(var idx in pars) {
        if (body != '') {
          body += '&';
        }
        body += idx + '=' + encodeURIComponent(pars[idx]);
      }
      xhr.send(body);
    },

    // adds up up an ajax request into the chain of ajax requests
    // and invokes the request, unless there are already other requests waiting
    raise: function(evt, options) {
      options = pf.extend({

        // invoked before http request is sent, with parameters as object, and event name as evt
        onbefore: function(/*pars, evt*/){},

        // invoked after http request is returned, but before dom is updated, with json object as parameter, and evt as event name
        onsuccess: function(/*json, evt*/){},

        // invoked if an error occurs during http request, with status code, status text, server response and event name
        onerror: function(/*code, status, response, evt*/){}
      }, options);

      // adding to chain
      pf._chain.push({
        evt: evt,
        el: this,
        options: options
      });

      // processing chain, but only if there's exactly one request in chain, since otherwise chain 
      // will continue by itself until whole chain is empty
      if (pf._chain.length == 1) { pf._next(); }
    },

    // invoked when server event is done executing
    done: function(xhr) {

      // removing current request from chain
      var cur = pf._chain.splice(0, 1)[0];
      var options = cur.options;

      if (xhr.status >= 200 && xhr.status < 300) {

        // success
        var json = eval('(' + xhr.responseText +')');
        options.onsuccess.apply(this, [json, cur.evt]);
        for (var idxEl in json['widgets']) {
          var el = pf.$(idxEl);
          for (var idxAtr in json['widgets'][idxEl]) {
            el.setVal(idxAtr, json['widgets'][idxEl][idxAtr]);
          }
        }
      } else {

        // error
        options.onerror.apply(this, [xhr.status, xhr.statusText, xhr.responseText, cur.evt]);
      }

      // processing next request
      pf._next();
    }
  };

  // chain of http requests
  pf._chain = [];
  
  // processes one http request
  pf._next = function() {

    // checking if we have anymore htttp requests in our chain
    if (pf._chain.length > 0) {
      var cur = pf._chain[0];
      cur.el._raise(cur.evt, cur.options);
    }
  };

})();
