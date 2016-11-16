Contains CSV file helper Active Events
===============

This directory contains the code that creates the import CSV file Active event, called [sys42.csv.import]. This Active Event simply
imports a CSV file, and puts it into the [p5.data](/plugins/p5.data/) database. Pass in [_arg] as file to import.

Notice, the event expects the CSV file to have column names as the first row in the file.

Example usage.

```
sys42.csv.import:~/documents/private/sample.csv
```
