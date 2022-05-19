# Stack trace to result details
If the comand execution ends with an exception, save the stack trace to the result detail
https://stackoverflow.com/questions/65830407/format-stack-trace-to-manually-return-in-api-response
https://github.com/atifaziz/StackTraceFormatter

# Evaluating regex match
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


