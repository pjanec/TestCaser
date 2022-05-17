# Test Caser

A tool for evaluation of a success/failure of an individual test steps performed as part of automatic UI testing.


This tool is able to:

* Check log files for presence of a regular expression
* Check if the screen contains an image
* Generate a HTML test report with the results

It does NOT perform:

* Simulating user input/actions - this is left for the control script to do



### Integration with any test tool

The tool is command-line based so it can be started from whatever other testing application or script.

The control script calls this tool every time when it is needed to check the expected outcome of a recently executed test step.

This tool returns error code specifying whether the the step has passed or failed base on defined criteria.



Example of a command sequence in a control script file of an arbitrary testing tool

('tc' stands for invocation of our test tool)

```powershell
# setup the test case
&tc clear # clears all results saved so far
&tc case "MyTestCase" # under what name to group the results of the tests to be  made

# reproduce some test case situation, for example opening some new window
&notepad "myDocument.txt"
    
# try to find and image on the screen, remember result, end if failed
&tc findimg myNotepadPattern.png || goto :end

# execute another test step
...

# check the result
&tc ...


:end
# generate test report
&tc report results 

```



### Test report

![TestResultExample](../TFS/Bagira-Systems.wiki/.attachments/TestResultExample.png)

