<#Find Greatest Common Denominator#>
function GetMultiplyingArray
{
	Param(
		[Parameter(Mandatory=$true)]
		[long] $InputNumber,
		
		[Parameter(Mandatory=$true)]
		[long] $length
		)

	$thisMultArray = [System.Collections.ArrayList]@()
	$PatternIndex = 0
	for($i = 1; $i -le $length; $i++)
	{
		if($i%$InputNumber -eq 0)
		{
			$PatternIndex = ($PatternIndex+1)%$Pattern.Count
		}	
	

		$thisMultArray+= $Pattern[$PatternIndex]
	}
	
	return $thisMultArray

}

function MultiplyArrays
{
	Param(
		[Parameter(Mandatory=$true)]
		[int] $InputNumber,
		
		[Parameter(Mandatory=$true)]
		[System.Collections.ArrayList] $inputArray
		)
		
	$multArray = $MultArrays[$InputNumber]
	
	
	$total= 0
	for($i=0; $i -lt $inputArray.Count; $i++)
	{
		$total += $inputArray[$i]*$multArray[$i]
	}
	
	return ([math]::abs($total))%10
}



<#Parse Input#>
$file = Get-Content Advent16_input.txt
$Input0 = [System.Collections.ArrayList]@()
$MultArrays = @{};
$Pattern= @(0,1,0,-1)


for($i = 0; $i -lt $file.Length; $i++)
{
	$Input0 += [int]($file.Substring($i,1))
	$array = GetMultiplyingArray -InputNumber ($i+1) -length ($file.Length)
	$MultArrays += @{($i+1) = $array}
}

<#foreach($input in $MultArrays.Keys)
{
	Write-Host "For input $input, array is " $MultArrays[$input]
}#>




<#$count = 1
$Input = @($Input0)
Write-Host "Input" $Input

while($count -le 100)
{
	$newInput = @()
	for($i = 0; $i -lt $Input.count; $i++)
	{
		$newInput += (MultiplyArrays -InputNumber ($i+1) -inputArray $Input)
	}
	$Input = $newInput
	$count++
}
Write-Host "New Input after" ($count-1) "phases: " $newInput

pause#>
<#Part 2#>
$file = Get-Content Advent16_input.txt
$offset = [long]($file.Substring(0,7))
$Input0 = $Input0*10000
Write-Host "Offset: "$offset

$count = 1
$Input = @($Input0)
while($count -le 100)
{

	$InputCopy = @($Input)
	for($i = ($Input.count-2); $i -ge $offset; $i--)
	{
		if($i -gt $Input.count/2)
		{
			$Input[$i] = [math]::abs(($Input[$i+1] +$Input[$i])%10)
		}
		else
		{
	
			$mult = @(0,1,0,-1)
			$additive = 0
			$count = 0
			$MultIndex = 0
			for($j = ($i); $j -lt $InputCopy.count; $j++)
			{
				if($count%($i+1) -eq 0)
				{
					$MultIndex = ($MultIndex+1)%$mult.count
				}
				$additive += $mult[$MultIndex]*$InputCopy[$j]
				
				$count++
			}
			
			$Input[$i] = [math]::abs(($additive)%10)
		}

	}
	
	

	Write-Host "LOOP" $count
	$count++
}

Write-Host $Input.Count
Write-Host $Input[$offset .. ($offset+7)]
