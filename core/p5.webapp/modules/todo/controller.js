

/*
 * Main module for our Angular app.
 */
angular.module ('todoApp', [])

  /*
   * Our only controller, responsible for invoking our server side back end,
   * and databinding our view.
   */
  .controller ('TodoListController', function ($scope, $http) {

    /*
     * Fetching existing items from our server.
     */
    var todoList = this;
    $http.get ('/hyper-core/mysql/todo/items/select').
      then (function successCallback (response) {
        todoList.todos = response.data;
    });
 
    /*
     * Invoked by our view when a new item has been added by user.
     * Updates our view with the item, and invokes server-side back end,
     * by issuing an 'insert' command towards our REST ORM.
     */
    todoList.addTodo = function () {

      /*
       * Invoking our server-side method, to insert item into our database.
       *
       * Notice, server-side back end requires a 'PUT' request.
       */
      $http ({
        method:'PUT', 
        url: '/hyper-core/mysql/todo/items/insert', 

        /*
         * Our server-side requires the data for our insert operation to be URL encoded,
         * hence we'll need to transform from the default serialization logic of Angular,
         * which is JSON, to URL encoded data.
         */
        headers: {'Content-Type': 'application/x-www-form-urlencoded'},
        transformRequest: function (obj) {
          var str = [];
          for (var p in obj) {
            str.push (encodeURIComponent (p) + '=' + encodeURIComponent (obj [p]));
          }
          return str.join ('&');
        },
        data: {description: document.getElementById ('newItem').value}
      }).then (function successCallback (response) {

        /*
         * Pushing our new item into our list of items, and making sure
         * we set our textbox' value to empty.
         */
        todoList.todos.push ({
          description: todoList.todoText, 
          id:response.data.id
        });
        todoList.todoText = '';
      });
    };
 
    /*
     * Invoked by our view when an item is deleted.
     */
    todoList.delete = function (id) {

      /*
       * Deleting our clicked item from our list of items from our view.
       */
      for(var idx = 0; idx < todoList.todos.length; idx++) {
        if (todoList.todos [idx].id === id) {
          todoList.todos.splice (idx,1);
          break;
        }
      }

      /*
       * Invoking our server-side method, to delete the item from our database.
       *
       * Notice, server-side back end requires a 'DELETE' request.
       */
      $http.delete('/hyper-core/mysql/todo/items/delete?id=' + id);
    }
  });
