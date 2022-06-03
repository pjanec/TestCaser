Add-Type -Path "$PSScriptRoot\bin\Debug\net5.0-windows\tc.dll"
$TC = new-object -TypeName TestCaser.TC -ArgumentList "$PSScriptRoot\Data"

$TC.Run( "clear" )
$TC.Run( "screenshot", "img1", "{Area:{rect:{X:10,Y:20,Width:1000,Height:100}}}" )
$TC.Run( "report", "results" )

Invoke-Expression $PSScriptRoot\Data\Results\Results.html
