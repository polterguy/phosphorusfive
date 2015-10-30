
/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details.
 */


/*
 * main namespace for unit tests
 */
var tests = {};


/*
 * called when a unit test fissles
 */
tests.setError = function(id) {
  var el = p5.$(id);
  el.el.value = 'failed';
  el.el.className = 'failed';
  el.el.disabled = 'disabled';
};


/*
 * called when a unit test succeeds
 */
tests.setSuccess = function(id) {
  var el = p5.$(id);
  el.el.value = 'success';
  el.el.className = 'success';
  el.el.disabled = 'disabled';
};


/*
 * used to count objects in object
 */
tests.countMembers = function(obj) {
  var count = 0;
  for (var idx in obj) {
    count += 1;
  }
  return count;
};


/*
 * runs all unit tests
 */
tests.run_all = function(id) {
  var els = document.getElementsByTagName('input');
  for (var idx = 0; idx < els.length; idx++) {
    if (els[idx].id.indexOf('invoke') == 0 && els[idx].className == 'undetermined') {
      // this is a unit test
      try {
      tests[els[idx].id]();
      } catch (err) {
        // do nothing, handled in 'onerror' handlers
      }
    }
  }
};


/*
 * invokes an empty event handler, asserting it returns nothing
 */
tests.invoke_empty = function(event) {
  var el = p5.$('sandbox_invoke_empty');
  el.raise('sandbox_invoke_empty_onclick', {
    onerror: function(statusCode, statusText, responseHtml, evt) {
      tests.setError('invoke_empty');
    },

    onsuccess: function(serverReturn, evt) {
      if (!tests.countMembers(serverReturn) == 0) {
        tests.setError('invoke_empty');
      } else {
        tests.setSuccess('invoke_empty');
      }
    }
  });
};


/*
 * invokes an empty event handler that throws an exception, asserting it calls 'onerror'
 */
tests.invoke_exception = function(event) {
  var el = p5.$('sandbox_invoke_exception');
  el.raise('sandbox_invoke_exception_onclick', {
    onerror: function(statusCode, statusText, responseHtml, evt) {
      tests.setSuccess('invoke_exception');
      return true;
    },

    onsuccess: function(serverReturn, evt) {
      tests.setError('invoke_exception');
    }
  });
};


/*
 * invokes a non-existing event handler, asserting 'onerror' is called
 */
tests.invoke_non_existing = function(event) {
  var el = p5.$('sandbox_invoke_non_existing');
  el.raise('sandbox_invoke_non_existing_onclick', {
    onerror: function(statusCode, statusText, responseHtml, evt) {
      tests.setSuccess('invoke_non_existing');
      return true;
    },

    onsuccess: function(serverReturn, evt) {
      tests.setError('invoke_non_existing');
    }
  });
};


/*
 * invokes an event handler not marked as WebMethod, asserting 'onerror' is called
 */
tests.invoke_no_webmethod = function(event) {
  var el = p5.$('sandbox_invoke_no_webmethod');
  el.raise('sandbox_invoke_no_webmethod_onclick', {
    onerror: function(statusCode, statusText, responseHtml, evt) {
      tests.setSuccess('invoke_no_webmethod');
      return true;
    },

    onsuccess: function(serverReturn, evt) {
      tests.setError('invoke_no_webmethod');
    }
  });
};


/*
 * invokes an event handler as 'onclick'
 */
tests.invoke_normal = function(event) {
  var el = p5.$('sandbox_invoke_normal');
  el.raise('onclick', {
    onerror: function(statusCode, statusText, responseHtml, evt) {
      tests.setError('invoke_normal');
    },

    onsuccess: function(serverReturn, evt) {
      tests.setSuccess('invoke_normal');
    }
  });
};


/*
 * invokes an event handler not marked as WebMethod, asserting 'onerror' is called
 */
tests.invoke_multiple = function(event) {
  var success = 'maybe';
  var el = p5.$('sandbox_invoke_multiple');
  el.raise('onclick', {
    onerror: function(statusCode, statusText, responseHtml, evt) {
      tests.setError('invoke_multiple');
      success = 'no';
    },

    onsuccess: function(serverReturn, evt) {
      if (success == 'maybe' && serverReturn.__p5_change.sandbox_invoke_multiple.innerValue != 'x') {
        success = 'no';
      }
    }
  });
  el.raise('onclick', {
    onerror: function(statusCode, statusText, responseHtml, evt) {
      tests.setError('invoke_multiple');
      success = 'no';
    },

    onsuccess: function(serverReturn, evt) {
      if (success == 'maybe' && serverReturn.__p5_change.sandbox_invoke_multiple.innerValue != 'xx') {
        success = 'no';
      }
    }
  });
  el.raise('onclick', {
    onerror: function(statusCode, statusText, responseHtml, evt) {
      tests.setError('invoke_multiple');
      success = 'no';
    },

    onsuccess: function(serverReturn, evt) {
      if (success == 'maybe' && serverReturn.__p5_change.sandbox_invoke_multiple.innerValue != 'xxx') {
        success = 'no';
      }
    }
  });
  el.raise('onclick', {
    onerror: function(statusCode, statusText, responseHtml, evt) {
      tests.setError('invoke_multiple');
      success = 'no';
    },

    onsuccess: function(serverReturn, evt) {
      if (success == 'maybe' && serverReturn.__p5_change.sandbox_invoke_multiple.innerValue != 'xxxx') {
        success = 'no';
      }
    }
  });
  el.raise('onclick', {
    onerror: function(statusCode, statusText, responseHtml, evt) {
      tests.setError('invoke_multiple');
      success = 'no';
    },

    onsuccess: function(serverReturn, evt) {
      if (success == 'maybe' && serverReturn.__p5_change.sandbox_invoke_multiple.innerValue != 'xxxxx') {
        tests.setError('invoke_multiple');
      } else {
        if (success == 'maybe') {
          tests.setSuccess('invoke_multiple');
        } else {
          tests.setError('invoke_multiple');
        }
      }
    }
  });
};


/*
 * invokes an event handler changing content of widget
 */
tests.invoke_javascript = function(event) {
  var el = p5.$('sandbox_invoke_javascript');
  el.raise('sandbox_invoke_javascript_onclick', {
    onerror: function(statusCode, statusText, responseHtml, evt) {
      tests.setError('invoke_javascript');
    },

    onbefore: function(pars, evt) {
      pars.push (['mumbo','mumbo']);
    },

    onsuccess: function(serverReturn, evt) {
      if (tests.countMembers(serverReturn) != 1 || tests.countMembers(serverReturn.__p5_change) != 1) {
        tests.setError('invoke_javascript');
        return;
      }
      if (serverReturn.__p5_change.sandbox_invoke_javascript.innerValue != 'mumbo jumbo') {
        tests.setError('invoke_javascript');
        return;
      }
      tests.setSuccess('invoke_javascript');
    }
  });
};


/*
 * invokes an event handler changing content of widget
 */
tests.invoke_change_content = function(event) {
  var el = p5.$('sandbox_invoke_change_content');
  el.raise('sandbox_invoke_change_content_onclick', {
    onerror: function(statusCode, statusText, responseHtml, evt) {
      tests.setError('invoke_change_content');
    },

    onsuccess: function(serverReturn, evt) {
      if (tests.countMembers(serverReturn) != 1 || tests.countMembers(serverReturn.__p5_change) != 1) {
        tests.setError('invoke_change_content');
        return;
      }
      if (serverReturn.__p5_change.sandbox_invoke_change_content.innerValue != 'new value') {
        tests.setError('invoke_change_content');
        return;
      }
      tests.setSuccess('invoke_change_content');
    }
  });
};


/*
 * invokes an event handler changing two attributes of widget
 */
tests.invoke_change_two_properties = function(event) {
  var el = p5.$('sandbox_invoke_change_two_properties');
  el.raise('sandbox_invoke_change_two_properties_onclick', {
    onerror: function(statusCode, statusText, responseHtml, evt) {
      tests.setError('invoke_change_two_properties');
    },

    onsuccess: function(serverReturn, evt) {
      if (tests.countMembers(serverReturn) != 1 || tests.countMembers(serverReturn.__p5_change) != 1) {
        tests.setError('invoke_change_two_properties');
        return;
      }
      if (serverReturn.__p5_change.sandbox_invoke_change_two_properties.class != 'new value 1') {
        tests.setError('invoke_change_two_properties');
        return;
      }
      if (serverReturn.__p5_change.sandbox_invoke_change_two_properties.innerValue != 'new value 2') {
        tests.setError('invoke_change_two_properties');
        return;
      }
      tests.setSuccess('invoke_change_two_properties');
    }
  });
};


/*
 * invokes an event handler adding an attribute, then a new event handler removing the same attribute
 */
tests.invoke_add_remove = function(event) {
  var el = p5.$('sandbox_invoke_add_remove');
  el.raise('sandbox_invoke_add_remove_1_onclick', {
    onerror: function(statusCode, statusText, responseHtml, evt) {
      tests.setError('invoke_add_remove');
    },

    onsuccess: function(serverReturn, evt) {
      if (tests.countMembers(serverReturn) != 1 || tests.countMembers(serverReturn.__p5_change) != 1) {
        tests.setError('invoke_add_remove');
        return;
      }
      if (serverReturn.__p5_change.sandbox_invoke_add_remove.class != 'new value 1') {
        tests.setError('invoke_add_remove');
        return;
      }

      // removing attribute 
      el.raise('sandbox_invoke_add_remove_2_onclick', {
        onerror: function(statusCode, statusText, responseHtml, evt) {
          tests.setError('invoke_add_remove');
        },

        onsuccess: function(serverReturn, evt) {
          if (tests.countMembers(serverReturn) != 1 || tests.countMembers(serverReturn.__p5_change) != 1) {
            tests.setError('invoke_add_remove');
            return;
          }
          if (serverReturn.__p5_change.sandbox_invoke_add_remove.__p5_del[0] != 'class') {
            tests.setError('invoke_add_remove');
            return;
          }
          tests.setSuccess('invoke_add_remove');
        }
      });
    }
  });
};


/*
 * invokes an event handler that adds and removes the same attribute in the same request
 */
tests.invoke_add_remove_same = function(event) {
  var el = p5.$('sandbox_invoke_add_remove_same');
  el.raise('sandbox_invoke_add_remove_same_onclick', {
    onerror: function(statusCode, statusText, responseHtml, evt) {
      tests.setError('invoke_add_remove_same');
    },

    onsuccess: function(serverReturn, evt) {
      if (tests.countMembers(serverReturn) != 0) {
        tests.setError('invoke_add_remove_same');
        return;
      }

      tests.setSuccess('invoke_add_remove_same');
    }
  });
};


/*
 * changes an attribute twice on the server
 */
tests.invoke_change_twice = function(event) {
  var el = p5.$('sandbox_invoke_change_twice');
  el.raise('sandbox_invoke_change_twice_onclick', {
    onerror: function(statusCode, statusText, responseHtml, evt) {
      tests.setError('invoke_change_twice');
    },

    onsuccess: function(serverReturn, evt) {
      if (tests.countMembers(serverReturn) != 1 || tests.countMembers(serverReturn.__p5_change) != 1) {
        tests.setError('invoke_change_twice');
        return;
      }
      if (serverReturn.__p5_change.sandbox_invoke_change_twice.class != 'jumbo') {
        tests.setError('invoke_change_twice');
        return;
      }

      tests.setSuccess('invoke_change_twice');
    }
  });
};


/*
 * changes an attribute declared in markup on server
 */
tests.invoke_change_markup_attribute = function(event) {
  var el = p5.$('sandbox_invoke_change_markup_attribute');
  el.raise('sandbox_invoke_change_markup_attribute_onclick', {
    onerror: function(statusCode, statusText, responseHtml, evt) {
      tests.setError('invoke_change_markup_attribute');
    },

    onsuccess: function(serverReturn, evt) {
      if (tests.countMembers(serverReturn) != 1 || tests.countMembers(serverReturn.__p5_change) != 1) {
        tests.setError('invoke_change_markup_attribute');
        return;
      }
      if (serverReturn.__p5_change.sandbox_invoke_change_markup_attribute.class != 'bar') {
        tests.setError('invoke_change_markup_attribute');
        return;
      }

      tests.setSuccess('invoke_change_markup_attribute');
    }
  });
};


/*
 * removes an attribute declared in markup on server
 */
tests.invoke_remove_markup_attribute = function(event) {
  var el = p5.$('sandbox_invoke_remove_markup_attribute');
  el.raise('sandbox_invoke_remove_markup_attribute_onclick', {
    onerror: function(statusCode, statusText, responseHtml, evt) {
      tests.setError('invoke_remove_markup_attribute');
    },

    onsuccess: function(serverReturn, evt) {
      if (tests.countMembers(serverReturn) != 1 || tests.countMembers(serverReturn.__p5_change) != 1) {
        tests.setError('invoke_remove_markup_attribute');
        return;
      }
      if (serverReturn.__p5_change.sandbox_invoke_remove_markup_attribute.__p5_del.length != 1) {
        tests.setError('invoke_remove_markup_attribute');
        return;
      }
      if (serverReturn.__p5_change.sandbox_invoke_remove_markup_attribute.__p5_del[0] != 'class') {
        tests.setError('invoke_remove_markup_attribute');
        return;
      }

      tests.setSuccess('invoke_remove_markup_attribute');
    }
  });
};


/*
 * removes an attribute declared in markup on server, then invokes new event handler that adds the same attribute back up again
 */
tests.invoke_remove_add_markup_attribute = function(event) {
  var el = p5.$('sandbox_invoke_remove_add_markup_attribute');
  el.raise('sandbox_invoke_remove_add_markup_attribute_1_onclick', {
    onerror: function(statusCode, statusText, responseHtml, evt) {
      tests.setError('invoke_remove_add_markup_attribute');
    },

    onsuccess: function(serverReturn, evt) {
      if (tests.countMembers(serverReturn) != 1 || tests.countMembers(serverReturn.__p5_change) != 1) {
        tests.setError('invoke_remove_add_markup_attribute');
        return;
      }
      if (serverReturn.__p5_change.sandbox_invoke_remove_add_markup_attribute.__p5_del.length != 1) {
        tests.setError('invoke_remove_add_markup_attribute');
        return;
      }
      if (serverReturn.__p5_change.sandbox_invoke_remove_add_markup_attribute.__p5_del[0] != 'class') {
        tests.setError('invoke_remove_add_markup_attribute');
        return;
      }

      el.raise('sandbox_invoke_remove_add_markup_attribute_2_onclick', {
        onerror: function(statusCode, statusText, responseHtml, evt) {
          tests.setError('invoke_remove_add_markup_attribute');
        },

        onsuccess: function(serverReturn, evt) {
          if (tests.countMembers(serverReturn) != 1 || tests.countMembers(serverReturn.__p5_change) != 1) {
            tests.setError('invoke_remove_add_markup_attribute');
            return;
          }
          if (serverReturn.__p5_change.sandbox_invoke_remove_add_markup_attribute.class != 'bar') {
            tests.setError('invoke_remove_add_markup_attribute');
            return;
          }

          tests.setSuccess('invoke_remove_add_markup_attribute');
        }
      });
    }
  });
};


/*
 * concatenate long attribute and verify only changes are returned
 */
tests.invoke_concatenate_long_attribute = function(event) {
  var el = p5.$('sandbox_invoke_concatenate_long_attribute');
  el.raise('sandbox_invoke_concatenate_long_attribute_onclick', {
    onerror: function(statusCode, statusText, responseHtml, evt) {
      tests.setError('invoke_concatenate_long_attribute');
    },

    onsuccess: function(serverReturn, evt) {
      if (tests.countMembers(serverReturn) != 1 || tests.countMembers(serverReturn.__p5_change) != 1) {
        tests.setError('invoke_concatenate_long_attribute');
        return;
      }
      if (serverReturn.__p5_change.sandbox_invoke_concatenate_long_attribute.class.length != 2) {
        tests.setError('invoke_concatenate_long_attribute');
        return;
      }
      if (serverReturn.__p5_change.sandbox_invoke_concatenate_long_attribute.class[0] != 37) {
        tests.setError('invoke_concatenate_long_attribute');
        return;
      }
      if (serverReturn.__p5_change.sandbox_invoke_concatenate_long_attribute.class[1] != 'qwerty') {
        tests.setError('invoke_concatenate_long_attribute');
        return;
      }

      tests.setSuccess('invoke_concatenate_long_attribute');
    }
  });
};


/*
 * create attribute, then concatenate value
 */
tests.invoke_create_concatenate_long_attribute = function(event) {
  var el = p5.$('sandbox_invoke_create_concatenate_long_attribute');
  el.raise('sandbox_invoke_create_concatenate_long_attribute_1_onclick', {
    onerror: function(statusCode, statusText, responseHtml, evt) {
      tests.setError('invoke_create_concatenate_long_attribute');
    },

    onsuccess: function(serverReturn, evt) {
      if (tests.countMembers(serverReturn) != 1 || tests.countMembers(serverReturn.__p5_change) != 1) {
        tests.setError('invoke_create_concatenate_long_attribute');
        return;
      }
      if (serverReturn.__p5_change.sandbox_invoke_create_concatenate_long_attribute.class != 'x1234567890') {
        tests.setError('invoke_create_concatenate_long_attribute');
        return;
      }

      el.raise('sandbox_invoke_create_concatenate_long_attribute_2_onclick', {
        onerror: function(statusCode, statusText, responseHtml, evt) {
          tests.setError('invoke_create_concatenate_long_attribute');
        },

        onsuccess: function(serverReturn, evt) {
          if (tests.countMembers(serverReturn) != 1 || tests.countMembers(serverReturn.__p5_change) != 1) {
            tests.setError('invoke_create_concatenate_long_attribute');
            return;
          }
          if (serverReturn.__p5_change.sandbox_invoke_create_concatenate_long_attribute.class.length != 2) {
            tests.setError('invoke_create_concatenate_long_attribute');
            return;
          }
          if (serverReturn.__p5_change.sandbox_invoke_create_concatenate_long_attribute.class[0] != 11) {
            tests.setError('invoke_create_concatenate_long_attribute');
            return;
          }
          if (serverReturn.__p5_change.sandbox_invoke_create_concatenate_long_attribute.class[1] != 'abcdefghijklmnopqrstuvwxyz') {
            tests.setError('invoke_create_concatenate_long_attribute');
            return;
          }

          tests.setSuccess('invoke_create_concatenate_long_attribute');
        }
      });
    }
  });
};


/*
 * change attribute of container's child
 */
tests.invoke_change_container_child = function(event) {
  var el = p5.$('sandbox_invoke_change_container_child_child');
  el.raise('sandbox_invoke_change_container_child_child_onclick', {
    onerror: function(statusCode, statusText, responseHtml, evt) {
      tests.setError('invoke_change_container_child');
    },

    onsuccess: function(serverReturn, evt) {
      if (tests.countMembers(serverReturn) != 1 || tests.countMembers(serverReturn.__p5_change) != 1) {
        tests.setError('invoke_change_container_child');
        return;
      }
      if (serverReturn.__p5_change.sandbox_invoke_change_container_child_child.class == 'bar') {
        tests.setError('invoke_change_container_child');
        return;
      }

      tests.setSuccess('invoke_change_container_child');
    }
  });
};


/*
 * make container widget visible and verify child is also visible
 */
tests.invoke_make_container_visible = function(event) {
  var el = p5.$('sandbox_invoke_make_container_visible');
  el.raise('sandbox_invoke_make_container_visible_onclick', {
    onerror: function(statusCode, statusText, responseHtml, evt) {
      tests.setError('invoke_make_container_visible');
    },

    onsuccess: function(serverReturn, evt) {
      if (tests.countMembers(serverReturn) != 1 || tests.countMembers(serverReturn.__p5_change) != 1) {
        tests.setError('invoke_make_container_visible');
        return;
      }
      if (serverReturn.__p5_change.sandbox_invoke_make_container_visible.outerHTML.indexOf('foo') == -1) {
        tests.setError('invoke_make_container_visible');
        return;
      }
      if (serverReturn.__p5_change.sandbox_invoke_make_container_visible.outerHTML.indexOf('strong') == -1) {
        tests.setError('invoke_make_container_visible');
        return;
      }

      tests.setSuccess('invoke_make_container_visible');
    }
  });
};


/*
 * make container widget visible and verify child is still invisible
 */
tests.invoke_make_container_visible_invisible_child = function(event) {
  var el = p5.$('sandbox_invoke_make_container_visible_invisible_child');
  el.raise('sandbox_invoke_make_container_visible_invisible_child_onclick', {
    onerror: function(statusCode, statusText, responseHtml, evt) {
      tests.setError('invoke_make_container_visible_invisible_child');
    },

    onsuccess: function(serverReturn, evt) {
      if (tests.countMembers(serverReturn) != 1 || tests.countMembers(serverReturn.__p5_change) != 1) {
        tests.setError('invoke_make_container_visible_invisible_child');
        return;
      }
      if (serverReturn.__p5_change.sandbox_invoke_make_container_visible_invisible_child.outerHTML.indexOf('foo') != -1) {
        tests.setError('invoke_make_container_visible_invisible_child');
        return;
      }
      if (serverReturn.__p5_change.sandbox_invoke_make_container_visible_invisible_child.outerHTML.indexOf('strong') != -1) {
        tests.setError('invoke_make_container_visible_invisible_child');
        return;
      }

      tests.setSuccess('invoke_make_container_visible_invisible_child');
    }
  });
};


/*
 * add child to container widget
 */
tests.invoke_add_child = function(event) {
  var el = p5.$('sandbox_invoke_add_child');
  el.raise('sandbox_invoke_add_child_onclick', {
    onerror: function(statusCode, statusText, responseHtml, evt) {
      tests.setError('invoke_add_child');
    },

    onsuccess: function(serverReturn, evt) {
      if (tests.countMembers(serverReturn) != 1 || tests.countMembers(serverReturn.__p5_change) != 1) {
        tests.setError('invoke_add_child');
        return;
      }
      if (serverReturn.__p5_change.sandbox_invoke_add_child.__p5_add_1.indexOf ('howdy world') == -1) {
        tests.setError('invoke_add_child');
        return;
      }
      if (serverReturn.__p5_change.sandbox_invoke_add_child.__p5_add_1.indexOf ('strong') == -1) {
        tests.setError('invoke_add_child');
        return;
      }

      tests.setSuccess('invoke_add_child');
    }
  });
};


/*
 * insert child to container widget at top
 */
tests.invoke_insert_child = function(event) {
  var el = p5.$('sandbox_invoke_insert_child');
  el.raise('sandbox_invoke_insert_child_onclick', {
    onerror: function(statusCode, statusText, responseHtml, evt) {
      tests.setError('invoke_insert_child');
    },

    onsuccess: function(serverReturn, evt) {
      if (tests.countMembers(serverReturn) != 1 || tests.countMembers(serverReturn.__p5_change) != 1) {
        tests.setError('invoke_insert_child');
        return;
      }
      if (serverReturn.__p5_change.sandbox_invoke_insert_child.__p5_add_0.indexOf ('howdy world') == -1) {
        tests.setError('invoke_insert_child');
        return;
      }
      if (serverReturn.__p5_change.sandbox_invoke_insert_child.__p5_add_0.indexOf ('strong') == -1) {
        tests.setError('invoke_insert_child');
        return;
      }

      tests.setSuccess('invoke_insert_child');
    }
  });
};


/*
 * add child to container widget, check it exists in a new request
 */
tests.invoke_add_child_check_exist = function(event) {
  var el = p5.$('sandbox_invoke_add_child_check_exist');
  el.raise('sandbox_invoke_add_child_check_exist_1_onclick', {
    onerror: function(statusCode, statusText, responseHtml, evt) {
      tests.setError('invoke_add_child_check_exist');
    },

    onsuccess: function(serverReturn, evt) {
      if (tests.countMembers(serverReturn) != 1 || tests.countMembers(serverReturn.__p5_change) != 1) {
        tests.setError('invoke_add_child_check_exist');
        return;
      }
      if (serverReturn.__p5_change.sandbox_invoke_add_child_check_exist.__p5_add_1.indexOf ('howdy world') == -1) {
        tests.setError('invoke_add_child_check_exist');
        return;
      }
      if (serverReturn.__p5_change.sandbox_invoke_add_child_check_exist.__p5_add_1.indexOf ('strong') == -1) {
        tests.setError('invoke_add_child_check_exist');
        return;
      }

      el.raise('sandbox_invoke_add_child_check_exist_2_onclick', {
        onerror: function(statusCode, statusText, responseHtml, evt) {
          tests.setError('invoke_add_child_check_exist');
        },

        onsuccess: function(serverReturn, evt) {
          if (tests.countMembers(serverReturn) != 1 || tests.countMembers(serverReturn.__p5_change) != 1) {
            tests.setError('invoke_add_child_check_exist');
            return;
          }
          if (serverReturn.__p5_change.sandbox_invoke_add_child_check_exist.__p5_add_2.indexOf ('howdy world 2') == -1) {
            tests.setError('invoke_add_child_check_exist');
            return;
          }
          if (serverReturn.__p5_change.sandbox_invoke_add_child_check_exist.__p5_add_2.indexOf ('strong') == -1) {
            tests.setError('invoke_add_child_check_exist');
            return;
          }

          tests.setSuccess('invoke_add_child_check_exist');
        }
      });
    }
  });
};


/*
 * insert child to container widget at top, check it exists in a new request
 */
tests.invoke_insert_child_check_exist = function(event) {
  var el = p5.$('sandbox_invoke_insert_child_check_exist');
  el.raise('sandbox_invoke_insert_child_check_exist_1_onclick', {
    onerror: function(statusCode, statusText, responseHtml, evt) {
      tests.setError('invoke_insert_child_check_exist');
    },

    onsuccess: function(serverReturn, evt) {
      if (tests.countMembers(serverReturn) != 1 || tests.countMembers(serverReturn.__p5_change) != 1) {
        tests.setError('invoke_insert_child_check_exist');
        return;
      }
      if (serverReturn.__p5_change.sandbox_invoke_insert_child_check_exist.__p5_add_0.indexOf ('howdy world') == -1) {
        tests.setError('invoke_insert_child_check_exist');
        return;
      }
      if (serverReturn.__p5_change.sandbox_invoke_insert_child_check_exist.__p5_add_0.indexOf ('strong') == -1) {
        tests.setError('invoke_insert_child_check_exist');
        return;
      }

      el.raise('sandbox_invoke_insert_child_check_exist_2_onclick', {
        onerror: function(statusCode, statusText, responseHtml, evt) {
          tests.setError('invoke_insert_child_check_exist');
        },

        onsuccess: function(serverReturn, evt) {
          if (tests.countMembers(serverReturn) != 1 || tests.countMembers(serverReturn.__p5_change) != 1) {
            tests.setError('invoke_insert_child_check_exist');
            return;
          }
          if (serverReturn.__p5_change.sandbox_invoke_insert_child_check_exist.__p5_add_1.indexOf ('howdy world 2') == -1) {
            tests.setError('invoke_add_child_check_exist');
            return;
          }
          if (serverReturn.__p5_change.sandbox_invoke_insert_child_check_exist.__p5_add_1.indexOf ('strong') == -1) {
            tests.setError('invoke_insert_child_check_exist');
            return;
          }

          tests.setSuccess('invoke_insert_child_check_exist');
        }
      });
    }
  });
};


/*
 * insert child to container widget at top, check it exists in a new request
 */
tests.invoke_append_remove = function(event) {
  var el = p5.$('sandbox_invoke_append_remove');
  el.raise('sandbox_invoke_append_remove_onclick', {
    onerror: function(statusCode, statusText, responseHtml, evt) {
      tests.setError('invoke_append_remove');
    },

    onsuccess: function(serverReturn, evt) {
      if (tests.countMembers(serverReturn) != 2 || tests.countMembers(serverReturn.__p5_change) != 1) {
        tests.setError('invoke_append_remove');
        return;
      }
      if (serverReturn.__p5_change.sandbox_invoke_append_remove.__p5_add_0.indexOf ('howdy world') == -1) {
        tests.setError('invoke_append_remove');
        return;
      }
      if (serverReturn.__p5_change.sandbox_invoke_append_remove.__p5_add_0.indexOf ('strong') == -1) {
        tests.setError('invoke_append_remove');
        return;
      }
      if (serverReturn.__p5_del.length != 1) {
        tests.setError('invoke_append_remove');
        return;
      }
      if (serverReturn.__p5_del[0] != 'sandbox_invoke_append_remove_child') {
        tests.setError('invoke_append_remove');
        return;
      }

      tests.setSuccess('invoke_append_remove');
    }
  });
};


/*
 * removes a child from a container
 */
tests.invoke_remove_child = function(event) {
  var el = p5.$('sandbox_invoke_remove_child');
  el.raise('sandbox_invoke_remove_child_onclick', {
    onerror: function(statusCode, statusText, responseHtml, evt) {
      tests.setError('invoke_remove_child');
    },

    onsuccess: function(serverReturn, evt) {
      if (tests.countMembers(serverReturn) != 1) {
        tests.setError('invoke_remove_child');
        return;
      }
      if (serverReturn.__p5_del.length != 1) {
        tests.setError('invoke_remove_child');
        return;
      }
      if (serverReturn.__p5_del[0] != 'sandbox_invoke_remove_child_child') {
        tests.setError('invoke_remove_child');
        return;
      }

      tests.setSuccess('invoke_remove_child');
    }
  });
};


/*
 * removes multiple children from container
 */
tests.invoke_remove_multiple = function(event) {
  var el = p5.$('sandbox_invoke_remove_multiple');
  el.raise('sandbox_invoke_remove_multiple_onclick', {
    onerror: function(statusCode, statusText, responseHtml, evt) {
      tests.setError('invoke_remove_multiple');
    },

    onsuccess: function(serverReturn, evt) {
      if (tests.countMembers(serverReturn) != 1) {
        tests.setError('invoke_remove_multiple');
        return;
      }
      if (serverReturn.__p5_del.length != 2) {
        tests.setError('invoke_remove_multiple');
        return;
      }
      if (serverReturn.__p5_del[0] != 'sandbox_invoke_remove_multiple_child1' && serverReturn.__p5_del[1] != 'sandbox_invoke_remove_multiple_child1') {
        tests.setError('invoke_remove_multiple');
        return;
      }
      if (serverReturn.__p5_del[0] != 'sandbox_invoke_remove_multiple_child2' && serverReturn.__p5_del[1] != 'sandbox_invoke_remove_multiple_child2') {
        tests.setError('invoke_remove_multiple');
        return;
      }

      tests.setSuccess('invoke_remove_multiple');
    }
  });
};


/*
 * removes three children from container and its children, and adds up two new controls
 */
tests.invoke_remove_many = function(event) {
  var el = p5.$('sandbox_invoke_remove_many');
  el.raise('sandbox_invoke_remove_many_onclick', {
    onerror: function(statusCode, statusText, responseHtml, evt) {
      tests.setError('invoke_remove_many');
    },

    onsuccess: function(serverReturn, evt) {
      if (tests.countMembers(serverReturn) != 2) {
        tests.setError('invoke_remove_many');
        return;
      }
      if (serverReturn.__p5_del.length != 3) {
        tests.setError('invoke_remove_many');
        return;
      }
      if (serverReturn.__p5_del[0] != 'sandbox_invoke_remove_many_2') {
        tests.setError('invoke_remove_many');
        return;
      }
      if (serverReturn.__p5_del[1] != 'sandbox_invoke_remove_many_6') {
        tests.setError('invoke_remove_many');
        return;
      }
      if (serverReturn.__p5_del[2] != 'sandbox_invoke_remove_many_9') {
        tests.setError('invoke_remove_many');
        return;
      }
      if (serverReturn.__p5_change.sandbox_invoke_remove_many.__p5_add_0.indexOf('howdy') == -1) {
        tests.setError('invoke_remove_many');
        return;
      }
      if (serverReturn.__p5_change.sandbox_invoke_remove_many_5.__p5_add_2.indexOf('world') == -1) {
        tests.setError('invoke_remove_many');
        return;
      }

      el.raise('sandbox_invoke_remove_many_verify_onclick', {
        onerror: function(statusCode, statusText, responseHtml, evt) {
          tests.setError('invoke_remove_many');
        },

        onsuccess: function(serverReturn, evt) {
          if (tests.countMembers(serverReturn) != 0) {
            tests.setError('invoke_remove_many');
            return;
          }
          tests.setSuccess('invoke_remove_many');
        }
      });
    }
  });
};
