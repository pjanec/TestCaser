@echo off
SET TC=%~dp0bin\Debug\net5.0-windows\tc.exe -d %~dp0Data
::%TC% clean || goto :eof
%TC% clear && echo OK || echo FAIL
%TC% case BatchTest1 && echo OK || echo FAIL
%TC% watchf ig IgLog.txt && echo OK || echo FAIL
%TC% watchf file2 {newest:'./*.log',recursive:true} && echo OK || echo FAIL
%TC% regex ig HelloDolly && echo OK || echo FAIL
%TC% findimg pattern2.png && echo OK || echo FAIL
%TC% saveimg img1 {'Area':{'X':10,'Y':20,'Width':100,'Height':100}}
%TC% report results && echo OK || echo FAIL
start Data\Results\Results.html

