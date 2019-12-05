
function matchNumber
{
	Param(
		[Parameter(Mandatory=$true)]
		[String] $numstr

	
}



$file = Get-Content Advent5_input.txt
$test = $file.Split(",")

