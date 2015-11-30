/*
 * CodeMirror module for editing Hyperlisp files
 */

(function(mod) {
  if (typeof exports == "object" && typeof module == "object") {
    mod(require("../../lib/codemirror"));
  } else if (typeof define == "function" && define.amd) {
    define(["../../lib/codemirror"], mod);
  } else {
    mod(CodeMirror);
  }
})(function(CodeMirror) {
"use strict";

CodeMirror.defineMode("hyperlisp", function() {
  return {

    /*
     * defines the different CSS class names for the different types of entities in Hyperlisp
     */
    styles: {
      comment:'comment',
      mcomment:'comment',
      operator:'atom',
      type:'atom',
      keyword:'keyword',
      activeevent:'property',
      variable:'variable',
      lesser_keyword:'variable-2',
      string:'string',
      mstring:'string',
      expression:'atom',
      value:'attribute',
      error:'error'
    },

    /*
     * invoked by CodeMirror to see how we should indent current line of code
     */
    indent: function (state, textAfter) {
      return state.indent;
    },

    blankLine: function(state) {
      if (state.oldIndent == null) {
        state.indent = 0;
      }
    },

    /*
     * initial state of parser, invoked by CodeMirror
     */
    startState: function() {
      return {
        mode:'name',
        indent:0
      };
    },

    /*
     * tokenizer main function, invoked by CodeMirror
     */
    token: function(stream, state) {
      switch (state.mode) {
        case 'name':
          return this._nameMode (stream, state);
        case 'value':
          return this._valueMode (stream, state);
        case 'mcomment':
          return this._parseMComment (stream, state);
        case 'mstring':
          return this._parseMString (stream, state);
        case 'error':
          stream.skipToEnd();
          return this.styles.error;
      }
    },

    /*
     * invoked when parser is parsing a "name" entity
     */
    _nameMode: function (stream, state) {

      // figuring out indentation
      var pos = 0;
      while (stream.peek() == ' ') {
        pos += 1;
        stream.next();
      }
      if (pos % 2 != 0) {
        state.mode = 'error';
        return this.styles.error;
      }
      state.indent = pos;

      // figuring out which type of element this is
      var retVal = null;
      var cr = stream.next();
      switch (cr) {
        case '"':
          retVal = this._parseString (stream, state);
          break;
        case '@':
          cr = stream.next();
          if (cr == '"') {
            state.mode = 'mstring';
            state.oldIndent = state.indent;
            state.indent = 0;
            retVal = this._parseMString (stream, state);
          }
          break;
        case ':':
          if (stream.peek() != null) {
            state.mode = 'value';
          } else {
            state.mode = 'name';
          }
          retVal = this.styles.operator;
          break;
        case '/':
          if (stream.peek() == '/') {
            retVal = this._parseComment(stream, state);
          } else if (stream.peek() == '*') {
            state.oldIndent = pos;
            state.indent = pos + 1;
            state.mode = 'mcomment';
            retVal = this._parseMComment(stream, state);
          }
          break;
        default:
          var word = cr;
          while (true) {
            cr = stream.peek();
            if (cr == null) {
              stream.next();
              break;
            } else if (cr == ':') {
              break;
            } else {
              stream.next();
            }
            word += cr;
          }
          if (cr == ':') {
            state.mode = 'value';
          }
          if (word != null && word.length > 0) {
            retVal = this._parseKeyword (word, state);
          }
          break;
      }
      return retVal;
    },

    /*
     * invoked to check to see if parser has found a "keyword" or an "active event" invocation
     */
    _parseKeyword: function (word, state) {
      switch (word) {

        // first p5.lambda keywords that requires indentation
        case 'eval':
        case 'login':
        case 'eval-mutable':
        case 'add':
        case 'src':
        case 'if':
        case 'else-if':
        case 'else':
        case 'set':
        case 'while':
        case 'for-each':
        case 'set-event':
        case 'set-protected-event':
        case 'insert-before':
        case 'insert-after':
        case 'lock':
        case 'fork':
        case 'wait':
        case 'split':
        case 'join':
        case 'can-convert':
        case '+':
        case '-':
        case '/': // TODO: why doesn't this work ...?
        case '*':
        case '%':
          state.indent += 2;
          return this.styles.keyword;

        // Afterwards keywords not needing indentation
        case 'sleep':
        case 'user':
        case 'logout':
        case 'roles':
          return this.styles.keyword;

        // Then "lesser" keywords that needs indentation
        case 'move-file':
        case 'copy-file':
        case 'move-folder':
        case 'copy-folder':
        case 'save-file':
        case 'save-text-file':
        case 'save-binary-file':
        case 'update-data':
        case 'insert-data':
        case 'set-session':
        case 'set-application':
        case 'set-http-header':
        case 'set-widget-property':
        case 'set-cache':
        case 'set-cookie':
        case 'create-widget':
        case 'create-literal-widget':
        case 'create-void-widget':
        case 'set-widget-ajax-event':
        case 'set-widget-lambda-event':
        case 'return-value':
        case 'lambda2lisp':
        case 'gzip':
        case 'gunzip':
        case 'set-viewstate':
          state.indent += 2;
          return this.styles.lesser_keyword;

        // lesser keywords not needing indentation
        case 'sleep':
        case 'clear-widget':
        case 'delete-widget':
        case 'get-parent-widget':
        case 'get-children-widgets':
        case 'list-widgets':
        case 'get-widget-ajax-event':
        case 'get-widget-lambda-event':
        case 'delete-widget-ajax-event':
        case 'delete-widget-lambda-event':
        case 'list-widget-ajax-events':
        case 'list-widget-lambda-events':
        case 'include-javascript':
        case 'send-javascript':
        case 'include-javascript-file':
        case 'include-stylesheet-file':
        case 'set-title':
        case 'get-title':
        case 'set-location':
        case 'get-location':
        case 'get-events':
        case 'lisp2lambda':
        case 'file-exist':
        case 'load-file':
        case 'load-text-file':
        case 'load-binary-file':
        case 'delete-file':
        case 'create-folder':
        case 'folder-exist':
        case 'list-files':
        case 'list-folders':
        case 'delete-folder':
        case 'select-data':
        case 'delete-data':
        case 'get-session':
        case 'list-session':
        case 'get-application':
        case 'list-application':
        case 'echo':
        case 'echo-file':
        case 'get-widget-property':
        case 'delete-widget-property':
        case 'list-widget-properties':
        case 'list-viewstate':
        case 'get-viewstate':
        case 'get-cache':
        case 'list-cache':
        case 'get-cookie':
        case 'list-cookies':
        case 'get-http-header':
        case 'list-http-headers':
        case 'get-http-param':
        case 'list-http-params':
        case 'get-http-method':
        case 'get-request-body':
        case 'delete-events':
        case 'list-events':
        case 'set-http-status':
        case 'set-http-status-code':
          return this.styles.lesser_keyword;

        // then "widget types"
        case 'literal':
        case 'container':
        case 'void':
          state.indent += 2;
          return this.styles.lesser_keyword;

        // then DOM events
        case 'oninit':
        case 'onclick':
        case 'ondblclick':
        case 'onmouseover':
        case 'onmouseout':
        case 'onfocus':
        case 'onblur':
        case 'onmousedown':
        case 'onmouseenter':
        case 'onmouseleave':
        case 'onmousemove':
        case 'onmouseover':
        case 'onmouseout':
        case 'onmouseup':
        case 'onkeydown':
        case 'onkeypress':
        case 'onkeyup':
        case 'onchange':
          state.indent += 2;
          return this.styles.lesser_keyword;

        // then "custom properties" that requires indentation
        case 'widgets':
        case 'events':
          state.indent += 2;
          return this.styles.lesser_keyword;

        // default handling
        default:
          if (word[0] == '_') {
            return this.styles.variable;
          } else if (word.indexOf('.') != -1) {
            /// \todo check for existing Active Event
            state.indent += 2;
            return this.styles.activeevent;
          }
          break;
      }
    },

    /*
     * invoked when parser is parsing a "value" entity
     */
    _valueMode: function (stream, state) {
      if (this.is_ex === true) {
        state.mode = 'name';
        this.is_ex = false;
        return this._parseExpression (stream, state);
      }
      var cr = stream.next();
      var retVal = 'value';
      switch (cr) {
        case '"':
          retVal = this._parseString (stream, state);
          state.mode = 'name';
          break;
        case '@':
          cr = stream.next();
          if (cr == '"') {
            state.mode = 'mstring';
            if (state.oldIndent == null) {
              state.oldIndent = state.indent;
            }
            state.indent = 0;
            retVal = this._parseMString (stream, state);
          }
          break;
        case ':':
          retVal = this.styles.operator;
          if (stream.peek() == null) {
            state.mode = 'name';
          }
          break;
        default:
          while (true) {
            cr = stream.peek();
            if (cr == null) {
              stream.next();
              state.mode = 'name';
              break;
            } else if (cr == ':') {
              if (stream.string.substring(stream.start, stream.pos) == 'x') {
                this.is_ex = true;
              }
              retVal = this.styles.type;
              break;
            } else {
              stream.next();
            }
          }
          break;
      }
      return retVal;
    },

    /*
     * invoked when parser is parsing a single line string entity, either as "name" or as "value"
     */
    _parseString: function (stream, state) {
      var cr = stream.next();
      var prev = '';
      while (true) {
        if (cr == '"' && prev != '\\') {
          stream.eatSpace();
          if (stream.peek() != null && stream.peek() != ':') {
            state.mode = 'error';
            stream.skipToEnd();
            return this.styles.error;
          }
          break;
        }
        if (cr == null) {
          state.mode = 'error';
          return this.styles.error;
        }
        prev = cr;
        cr = stream.next();
      }
      return this.styles.string;
    },

    /*
     * invoked when parser is parsing a multi line string entity, either as "name" or as "value"
     */
    _parseMString: function (stream, state) {
      var cr = stream.next();
      while (cr != null) {
        if (cr == '"' && stream.peek() != '"') {
          var line = stream.current();
          if (line.indexOf('@"') == 0) {
            line = line.substring(2);
          }
          if ((line.length - line.replace(/"+$/, "").length) % 2 == 1) {
            state.mode = 'name';
            state.indent = state.oldIndent;
            delete state.oldIndent;
            break;
          }
        }
        cr = stream.next();
      }
      cr = stream.peek();
      if (cr != null && cr != ':') {
        state.mode = 'error';
        return this.styles.error;
      }
      return this.styles.mstring;
    },

    /*
     * invoked when parser is parsing an expression as a "value" entity
     */
    _parseExpression: function (stream, state) {

      // TODO: implement support for multiline expressions here ...
      stream.skipToEnd();
      return this.styles.expression;
    },

    /*
     * invoked when parser is parsing a single line comment
     */
    _parseComment: function (stream, state) {
      stream.skipToEnd();
      return this.styles.comment;
    },

    /*
     * invoked when parser is parsing multi line comment
     */
    _parseMComment: function (stream, state) {
      stream.skipToEnd();
      var cur = stream.current();
      if (cur.indexOf('*/', cur.length - 2) != -1) {

        // end of comment
        state.indent = state.oldIndent;
        delete state.oldIndent;
        state.mode = 'name';
      } else if (cur.indexOf('*/') != -1) {

        // characters behind comment on same line, which is an error in Hyperlisp
        state.mode = 'error';
        return this.styles.error;
      }
      return this.styles.mcomment;
    }
  };
});

CodeMirror.defineMIME("text/x-hyperlisp", "hyperlisp");

});
