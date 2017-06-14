MySQL support in Phosphorus Five
===============

This component contains the adapter for MySQL, which allows you to connect to a MySQL database, and select, update, insert, etc records,
the same way you would from e.g. C#. It contains the following public Active Events.

* [p5.mysql.connect]
* [p5.mysql.select]
* [p5.mysql.insert]
* [p5.mysql.update]
* [p5.mysql.delete]
* [p5.mysql.execute]
* [p5.mysql.scalar]
* [p5.mysql.transaction.begin]
* [p5.mysql.transaction.rollback]
* [p5.mysql.transaction.commit]
* [p5.mysql.database.get]
* [p5.mysql.database.set]

Of the above events, the most important one is *[p5.mysql.connect]*. It allows you to connect to a MySQL database, either by providing the actual
connection properties directly, such as `server=localhost;User Id=root;password=YOUR_MY_SQL_PASSWORD;database=YOUR_DATABASE` - Or by
referencing a named connection string from your web.config, by wrapping its name in square brackets, such as the following illustrates `[your_database]`.

If you have a named connection string in your web.config, and you wish to select some records from some table, you could do something like the following.

```
p5.mysql.connect:[your_database]
  p5.mysql.select:@"select * from your_table limit 10"
```

The above would return something like the following to you

```
p5.mysql.connect:[your_database]
  p5.mysql.select
    row
      id:uint:17277
      username:root
      word:a
    row
      id:uint:17278
      username:root
      word:abonnement
    row
      id:uint:17279
      username:root
      word:abonnere
[snip]
```

Notice how the invocation to *[p5.mysql.select]* must be wrapped inside your *[p5.mysql.connect]* lambda object. This is because your connection is
determincally closed, when you leave the scope of your *[p5.mysql.connect]*. The *[p5.mysql.connect]* scope is just a normal plain lambda object, and
can contain any Active Event invocations legally allowed to evaluate outside of its scope.

All MySQL events you want to raise, will have to exist inside of a *[p5.mysql.connect]* invocation - Either directly, or indirectly.

## [p5.mysql.insert], [p5.mysql.update], [p5.mysql.delete] and [p5.mysql.execute]

The *[p5.mysql.insert]* Active Event will insert a record into your database, and return the ID of the inserted record as an *[id]* child. The other
three events, will not return anything to the caller, besides from the affected records as the value of your invocation node. Which one you use,
is really optional. But I do recommend to explicitly use the correct event, to make your code more easily understood from a semantic point of view,
according to what your SQL statement actually does.

## [p5.mysql.scalar]

This Active Event will simply execute a scalar SQL statement, such as `select count(*) from some_table`, and returns the result.

## [p5.mysql.transaction.xxx]

These three events allows you to create a database transaction, which if an exception occurs from within, will automatically rollback the entire
job. This is a nifty feature to make sure either everything is successfully executed, or nothing is executed at all. An example of usage is given 
below.

```
p5.mysql.connect:[your_database]
  p5.mysql.transaction.begin
    // Do some SQL insertions, updates and deletions here.
    // If an exception occurs, your database is rolled back, and stays unchanged.

    // Then as your last piece of code, invoke the following
    p5.mysql.transaction.commit
```

Unless the last line in the above code is raised, nothing will change in your database, and your entire job will be rolled back. You can
of course explicitly rollback a transaction, using *[p5.mysql.transaction.rollback]* - But this is rarely needed, since it's the default behaviour,
and done automatically if an exception is raised, that makes you leave the scope of your *[p5.mysql.transaction.begin]* lambda object, without
havin explicitly invoked *[p5.mysql.transaction.commit]*.

In fact, you should probably rarely, if ever, explicitly invoke the *[p5.mysql.transaction.rollback]* yourself, but rather resort to raising
an exception, if something goes wrong inside of your transaction lambda object.

## Changing and retrieving your database

The *[p5.mysql.database.get]* and *[p5.mysql.database.set]* Active Events, simply allows you to get and set the currently active database.

## Best practices

You should probably not supply the database connection string directly into your code, but rather rely on a web.config setting, and reference it
using the square bracket notation demonstrated above. If you do, the correct way to create a MySQL connection string in your web.config, is as follows.

```xml
  <connectionStrings>
    <add 
      name="your_database" 
      connectionString="server=localhost;User Id=root;password=YOUR_PASSWORD;database=YOUR_DATABASE" 
      providerName="MySql.Data.MySqlClient" />
  </connectionStrings>
```

Stuff the above settings into your web.config, somewhere for instance above your `<system.web>` parts. Then reference the connection string indirectly
from your code as the following illustrates.

```
p5.mysql.connect:[your_database]
```

