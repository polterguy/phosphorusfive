#!/usr/bin/env python
import sys
import re

if (len(sys.argv) < 2):
    print "No input file"
else:
    f = open(sys.argv[1])
    line = f.readline()
    while line:
        re1 = re.compile("^\s*\[ActiveEvent\s*\(\s*Name\s*=\s*\"(.*)\"\)]")
        re1.search(line)
        line = re1.sub(r"/// Active Event: \"<strong>\1</strong>\" \\ingroup ActiveEvents \n", line)
        re2 = re.compile("(\[\S*\])")
        re2.search(line)
        sys.stdout.write(re2.sub(r"<strong>\1</strong>", line))
        #sys.stdout.write(line)
        line = f.readline()
    f.close()
