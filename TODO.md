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


watchf <watchId> <fileSpec>

watchSpec:
  "preset"
  {preset:'preset'}

regex <fileSpec> <regexSpec> <options>
   if fileSpec does not contain Watcher, an anonymous watcher is used

fileSpec:
  "path/to/file.txt" ... file path
  {path:'path/to/file.txt'}
  {preset:'name'}
  {newest:{folder:'c:/myfolder',recursive:true}}
  {watch:'watchId'} ... resolves to file path via a named FileWatcher

  If no special watcher id assigned, hash from full file path will is used to identify the file context.

regexSpec:
  "pattern"        .... string considered regex pattern
      contains
      ^begins.*
      ends.*$
      ^exact$
  {preset:'name'} .... settings loaded from RegexSpec\{id}.json
  {pattern:'contains'}


windowSpec:
  {title:'pattern'}
  {title:{preset:'name'}}
  {title:{pattern:'name',ignoreCase:true}}
  //{hwnd:9273823}
  {preset:'name'}
  "preset_id"  ... converted to {preset: 'preset_id'}

areaSpecs:
  "preset_id"   ... string considered preset id
  [1,2,3,4]
  ['10%',20,30,'40%']
  {rect:[1,2,3,4]}
  {rect:{X:1,Y:2,W:3,H:4]}
  {window:'preset'}}
  {window:{preset:'name',rect:[1,2,3,4]}}
  {window:{title:'pattern'}
  {window:{title:{preset:'name'}}}
  {screen:{id:1}}
  {screen:{id:1,rect:[1,2,3,4]}}


