Handling CSV files
===============

This folder contains the Active Events necessary to read CSV files automatically. There are two basic Active Events, that converts between CSV and lambda.

* [p5.csv.csv2lambda] - Converts from CSV to lambda
* [p5.csv.lambda2csv] - Converts from lambda to CSV

The *[load-file]* Active Event, will automatically invoke the *[p5.csv.csv2lambda]* for CSV files, unless you explicitly tell it not to.
To convert a CSV file, you can use the following code.

```
load-file:~/documents/private/sample.csv
```

The above code, assumes you have a CSV file in your private documents folder.

The *[p5.csv.lambda2csv]* Active Event, will reverse the process, and creates a piece of CSV text, which you can save or do whatever you wish 
with, after invocation. Example are given below.

```
p5.csv.lambda2csv
  item1
    name:Thomas
    status:cool
  item2
    name:John
    status:male
  item3
    name:jane
    status:female
```

The above invocation will return the following.

```
p5.csv.lambda2csv:@"name,status
Thomas,cool
John,male
jane,female
"
```
