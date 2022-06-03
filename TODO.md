# Implement sqlq command
 - Executes a SQL query, fetches the records and calls a scriban script to evaluate the final result
 - Allow storing connection strings as ConnStringSpec
   - support for different backend types (sqlserver, sqlite, mysql...)
 - Allow storing sql queries as SqlQuerySpec

# Implement mongodbq command
 - Executes a mongodb query query, fetches the records and calls a scriban script to evaluate the final result
 - Allow storing connection strings as ConnStringSpec
 - Allow storing queries as MongodbQuerySpec

# Custom command as scriban script??
 - test caser functions made available to scriban
   - providing specs as scriban objects (in addition to parsing the json)
   - writing results to report; providing custom HTML template for the details section
   - scanning files for regex
   - grabbing screen to file
   - locating image on the screen
   - sending web requests
 - CommandSpec to specify the scriban script file
 - Implement all commands via scriban, i.e. no built-in commands? Not necessary but should be possible.


# [DONE] TestCaser commands callable via TC object from DLL
 - Expose TC object and its Exec function
   - Easier implementation
   - Still should provide much faster execution (if we cache the TC object) than loading the TC assembly every time from scrath
 - Exec Function takes an array of strings, corresponding to the command line arguments of the commands
 - Alternatively, instead of an array of string, the function might take one single dictionary<string,object>
   containing 
     - key = command argument name
     - value
          - if strings, it will be passed to the funtion as is and the function wil interpret it (usually as a JSON)
          - if a dictionary<string,object>, it will be first converted to JSON object and passed to the function

# [DONE] Evaluating regex match
 - regexf saves the Match object (in scriban-accessible form) its results
    - string[] Groups
 - additional regexf parameter = pass/nopass condition if reges found a match
    - scriban expression getting the match object an returning true/false
    - if no expressioon defined, IsMatch value is returned
    - otherwise the expression is evaluated if there was a match
    - example
       "string.to_double(Groups[1]) > 12.7"

# Client/Server 
 - simple TCP client sends command to the server and waits for the result
 - server stays loaded in memory
 - server can monitor something that does not like initialization every time a command is issued
    - like keeping RemoteDump connection to the tested apps etc.


