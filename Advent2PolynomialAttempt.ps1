function PerformOperation
{
	Param(
		[Parameter(Mandatory=$true)]
		[String[]] $array,
		
		[Parameter(Mandatory=$true)]
		[int] $commandIndex
		)


	Write-Host $commandIndex
	$command = $array[$commandIndex]
	
	$Operand1Address = [int]$array[$commandIndex+1]
	$Operand2Address = [int]$array[$commandIndex+2]
	$ResultAddress = [int]$array[$commandIndex+3]
	
	$variable1 = $StoredVariables[$Operand1Address]
	$variable2 = $StoredVariables[$Operand2Address]
	
	$Operand1 = [int]$array[$Operand1Address]
	$Operand2 = [int]$array[$Operand2Address]
	
	if($command -eq "1")
	{
		Write-Host "Add" $Operand1 "and" $Operand2 -foregroundcolor red
		if($variable1 -ne $null)
		{
			if($variable2 -ne $null)
			{
				$StoredVariables[$ResultAddress] = AddArrays -array1 $variable1 -array2 $variable2
			}
			else
			{
				$StoredVariables[$ResultAddress] = AddConstant -num $Operand1 -array $variable1
			}
		}
		elseif($variable2 -ne $null)
		{
			$StoredVariables[$ResultAddress] = AddConstant -num $Operand2 -array $variable2
		}
	
		$answer = $Operand1 + $Operand2
		$array[$ResultAddress] = $answer.ToString();
		
		Write-Host "Postition" $ResultAddress "set to" $answer
		return PerformOperation -array $array -commandIndex ($commandIndex +4)
	}
	elseif($command -eq "2")
	{
		Write-Host "Multiply" $Operand1 "by" $Operand2 -foregroundcolor red
		if($variable1 -ne $null)
		{
			if($variable2 -ne $null)
			{
				$StoredVariables[$ResultAddress] = MultiplyArrays -array1 $variable1 -array2 $variable2
			}
			else
			{
				$StoredVariables[$ResultAddress] = Multiply -array $variable1 -multiplier $Operand1
			}
		}
		elseif($variable2 -ne $null)
		{
			$StoredVariables[$ResultAddress] = Multiply -array $variable2 -multiplier $Operand2
		}
	
		$answer = $Operand1 * $Operand2
		$array[$ResultAddress] = $answer.ToString();
		
		Write-Host "Postition" $ResultAddress "set to" $answer
		return PerformOperation -array $array -commandIndex ($commandIndex +4)
	}
	elseif($command -eq "99")
	{
		return $array
	}
	else
	{
		Write-Host "Incorrect Command.  Recieved:" $array[$commandIndex]
		return @()
	}

}

function AddArrays
{
	Param(
		[Parameter(Mandatory=$true)]
		[Object[]] $array1,
		
		[Parameter(Mandatory=$true)]
		[Object[]] $array2
		)
	
	Write-Host "Adding Arrays"
	$answer = @(@(0,0,0),@(0,0,0),@(0,0,0))
	
	for( $i = 0; $i -lt $array1.Count; $i++)
	{
		for($j =0; $j -lt $array1[$i].Count; $j++)
		{
			$answer[$i][$j] = $array1[$i][$j] + $array2[$i][$j]
		}
	}
	
	return $answer;
}

function AddConstant
{
	Param(
		[Parameter(Mandatory=$true)]
		[int] $num,
		
		[Parameter(Mandatory=$true)]
		[Object[]] $array
		)
	
	
	Write-Host "Adding Constant"
	$array[0][0] = $array[0][0] + $num
	for( $i = 0; $i -lt $array.Count; $i++)
	{
		Write-Host $array[$i]
	}
	

	return $array
}

function MultiplyArrays
{
	Param(
		[Parameter(Mandatory=$true)]
		[Object[]] $array1,
		
		[Parameter(Mandatory=$true)]
		[Object[]] $array2
		)
		
	Write-Host "Multiply Arrays"	
	$answer = $null
	for( $i = 0; $i -lt $array1.Count; $i++)
	{
		for($j =0; $j -lt $array1[$i].Count; $j++)
		{
			$answerPortion1 = ShiftArray -array $array2 -AdvanceX $i -AdvanceY $j
			$answerPortion2 = Multiply -array $answerPortion1 -multiplier $array1[$i][$j]
			
			if($answer -eq $null)
			{
				$answer = $answerPortion2
			}
			else
			{
				$answer = AddVariables -array1 $answer -array2 $answerPortion2
			}
		}
	}
	
	return $answer;
}

function Multiply
{
	Param(
		[Parameter(Mandatory=$true)]
		[Object[]] $array,
		
		[Parameter(Mandatory=$true)]
		[int] $multiplier
		)
		
	Write-Host "Multiply Constant "	$multiplier
	
	for( $i = 0; $i -lt $array.Count; $i++)
	{
		for($j =0; $j -lt $array[$i].Count; $j++)
		{
			$array[$i][$j] = $array[$i][$j]*$multiplier
		}
	}
	
	for( $i = 0; $i -lt $array.Count; $i++)
	{
		Write-Host $array[$i]
	}
	return $array;
}


function ShiftArray
{
	Param(	[Parameter(Mandatory=$true)]
			[Object[]] $array,
		
			[Parameter(Mandatory=$true)]
			[int] $AdvanceX,
			
			[Parameter(Mandatory=$true)]
			[int] $AdvanceY
		)
	
	
	Write-Host "Shift Array"
	$answer = @()	
	for( $i = 0; $i -lt $array.Count; $i++)
	{
		for($j =0; $j -lt $array[$i].Count; $j++)
		{
			$answer[$i][$j] += 0
		}
	}
	
	$xIndex = 0
	$yIndex = 0
	for( $i = AdvanceX; $i -lt $answer.Count; $i++)
	{
		for($j = AdvanceY; $j -lt $answer[$i].Count; $j++)
		{
			$answer[$i][$j] += $array[$xIndex][$yIndex]
			$yIndex++
		}
		$xIndex++
	}
	
	return $answer
}


$StoredVariables = @()

$file = Get-Content Advent2_input.txt
$test = $file.Split(",")

for($i = 0; $i -lt $test.Count; $i++)
{
	$StoredVariables += $null
}

$X = @(@(0,0,0),@(1,0,0),@(0,0,0))
$Y = @(@(0,1,0),@(0,0,0),@(0,0,0))

$X.GetType()

$StoredVariables[1] = $X
$StoredVariables[2] = $Y
$finalArray = PerformOperation -array $test -commandIndex 0
Write-Host "Answer is :" $finalArray[0]

$EndResult = $StoredVariables[0]
Write-Host "Polynomial:" 
Write-Host $EndResult[0][0]"+"

if($EndResult[1][0] -ne 0)
{
	Write-Host $EndResult[1][0]"*x+"
}
if($EndResult[2][0] -ne 0)
{
	Write-Host $EndResult[2][0]"*x^2+"
}
if($EndResult[0][1] -ne 0)
{
	Write-Host $EndResult[0][1]"*y+"
}
if($EndResult[1][1] -ne 0)
{
	Write-Host $EndResult[1][0]"*x*y+"
}
if($EndResult[2][1] -ne 0)
{
	Write-Host $EndResult[1][0]"*x^2*y+"
}
if($EndResult[0][2] -ne 0)
{
	Write-Host $EndResult[0][1]"*y^2+"
}
if($EndResult[1][2] -ne 0)
{
	Write-Host $EndResult[1][0]"*x*y^2+"
}
if($EndResult[2][2] -ne 0)
{
	Write-Host $EndResult[1][0]"*x^2*y^2+"
}
