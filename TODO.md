# command for writing result
 - if we determine ourselves if the test case step has passed/failed
 - and we want to record it to the results for later report

# Evaluating regex match
 - regexf saves the Match object (in scriban-accessible form) its results
    - string[] Groups
 - additional regexf parameter = pass/nopass condition if reges found a match
    - scriban expression getting the match object an returning true/false
    - if no expressioon defined, IsMatch value is returned
    - otherwise the expression is evaluated if there was a match
    - example
       "string.to_double(Groups[1]) > 12.7"

# Each command in spearate file/class

## Each command result line as separate class
 - instead of string[] details use class with props
 - derived from basic ResultLine, adding command-specific fields
 - serializing into json, saved as part of result line in test case result file
 - htmpl template gets the derived ResultLine object with named props, no need for details[1], details[2] etc.

## Each command parameters as a class
 - loads itself from string[]
 - available to the html renderer - possibility to dump it to html to identify the checked item in the result report

# Client/Server 
 - simple TCP client sends command to the server and waits for the result
 - server stays loaded in memory
 - server can monitor something that does not like initialization every time a command is issued
    - like keeping RemoteDump connection to the tested apps etc.
