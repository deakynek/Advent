function PerformOperation
{
    Param
    (
         [Parameter(Mandatory=$true, Position=0)]
         [String[]] $array,
         [Parameter(Mandatory=$true, Position=1)]
         [int] $commandIndex
    )
	
	<# Write-Host $commandIndex #>
	$in0 = [int]$array[($commandIndex)]
	$in1 = [int]$array[($commandIndex+1)]
	$in2 = [int]$array[($commandIndex+2)]
	$in3 = [int]$array[($commandIndex+3)]
	<# Write-Host $in0 $in1 $in2 $in3 #>
	
	
	$a = [int]$array[$in1]
	$b = [int]$array[$in2]
	$c = $array[$in3]
	
	$command = $array[$commandIndex]
	
	if($command -eq "1")
	{
		<# Write-Host "Command was add" #>
		$answer = $a + $b
		$array[$in3] = $answer.ToString();
		
		<# Write-Host $a "+" $b "=" $answer
		Write-Host "Writing " $answer "to Position" $in3
		Write-Host "" #>
		return PerformOperation $array ($commandIndex +4)
	}
	elseif($command -eq "2")
	{
<# 		Write-Host "Command was multiply" #>
		$answer = $a * $b
		$array[$in3] = $answer.ToString();
		
<# 		Write-Host $a "*" $b "=" $answer
		Write-Host "Writing " $answer "to Position" $in3
		Write-Host "" #>
		return PerformOperation $array ($commandIndex +4)
	}
	elseif($command -eq "99")
	{
		<# Write-Host "Command was end" #>
		return $array
	}
	else
	{
		Write-Host "Incorrect Command.  Received:" $array[$commandIndex]
		return @()
	}

}

$file = Get-Content Advent2_input.txt
$test = $file.Split(",")

For ($i =0; $i -le 99; $i++)
{
	For ($j = 0; $j -le 99; $j++)
	{
		$test = $file.Split(",")
		$test[1] = $i.ToString()
		$test[2] = $j.ToString()
		
		$finalArray = PerformOperation -array $test -commandIndex 0
		$ans = $finalArray[0]
		
		<# Write-Host "I" $i "J" $j "Final" $ans #>
		if ($ans -eq "19690720")
		{
			Write-Host "Possible Answer is :" $i $j
		}
	}
}

<# Write-Host "Test of Length:" $test.Length

$finalArray = PerformOperation -array $test -commandIndex 0

Write-Output "Answer is :" $finalArray[0] #>
