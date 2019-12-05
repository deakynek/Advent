
function matchNumber
{
	Param(
		[Parameter(Mandatory=$true)]
		[String] $numstr
		)

	
}

function PerformOperation
{
	Param(
		[Parameter(Mandatory=$true)]
		[String[]] $array,
		
		[Parameter(Mandatory=$true)]
		[int] $commandIndex
		)
	$command = $array[$commandIndex]
	
	$Operand1Address = [int]$array[$commandIndex+1]
	$Operand2Address = [int]$array[$commandIndex+2]
	$ResultAddress = [int]$array[$commandIndex+3]
	
	$specialMode = $false
	
	$OperandCount = 0
	$Commands = @()
	if($command.Length -gt 1)
	{
		$specialMode = $true
		<#New Command Type#>
		$comm = $command.Substring($command.Length-2,2)
		
		$intComm = [int]$comm
		
		$tempComm = $intComm.ToString()
		if(($tempComm -eq "1") -or ($tempComm -eq "2") -or ($tempComm -eq "7") -or ($tempComm -eq "8"))
		{
			$OperandCount = 3
		}
		if(($tempComm -eq "5") -or ($tempComm -eq "6"))
		{
			$OperandCount = 2
		}
		if(($tempComm -eq "3") -or ($tempComm -eq "4"))
		{
			$OperandCount = 1
		}
		
		for($i = 1; $i -le $OperandCount; $i++)
		{
			$index = ($command.Length - 2 - $i)
			if($index -ge 0)
			{
				$temp = $command[$index]
				$Commands += $temp
			}
			else
			{
				$Commands += 0
			}
		}
		
		$command = $tempComm
	}
	
	$Operand1 = [int]$array[$Operand1Address]
	$Operand2 = [int]$array[$Operand2Address]
	
	if($specialMode -and ($Commands[0] -eq "1"))
	{
		$Operand1 = $Operand1Address
	}
	if($specialMode -and ($Commands[1] -eq "1"))
	{
		$Operand2 = $Operand2Address
	}		
	
	
	if($command -eq "1")
	{
		Write-Host "Add" $Operand1 "and" $Operand2 -foregroundcolor red
	
		$answer = $Operand1 + $Operand2
		$array[$ResultAddress] = $answer.ToString();
		
		Write-Host "Postition" $ResultAddress "set to" $answer
		return PerformOperation -array $array -commandIndex ($commandIndex +4)
	}
	elseif($command -eq "2")
	{
		Write-Host "Multiply" $Operand1 "by" $Operand2 -foregroundcolor red
	
		$answer = $Operand1 * $Operand2
		$array[$ResultAddress] = $answer.ToString();
		
		Write-Host "Postition" $ResultAddress "set to" $answer
		return PerformOperation -array $array -commandIndex ($commandIndex +4)
	}
	elseif($command -eq "3")
	{
		$Input = Read-Host -Prompt "Gimme a number baby"
		$array[$ResultAddress] = $Input
		return PerformOperation -array $array -commandIndex ($commandIndex +2)
	}
	elseif($command -eq "4")
	{
		Write-Host "Outputing Info" -foregroundcolor yellow
		if($specialMode -and ($Commands[0] -eq "1"))
		{
			Write-Host $Operand1Address -foregroundcolor yellow
		}
		else
		{
			Write-Host $array[$Operand1Address] -foregroundcolor yellow
		}
	
		
		return PerformOperation -array $array -commandIndex ($commandIndex +2)
	}
	elseif($command -eq "5")
	{
		if($Operand1 -ne 0)
		{
			Write-Host "Jump to index" $Operand2 -foregroundcolor red
			return PerformOperation -array $array -commandIndex $Operand2
		}
		else
		{
			return PerformOperation -array $array -commandIndex ($commandIndex +3)
		}
	}
	elseif($command -eq "6")
	{
		if($Operand1 -eq 0)
		{
			Write-Host "Jumping to index " $Operand2
			return PerformOperation -array $array -commandIndex $Operand2
		}
		else
		{
			return PerformOperation -array $array -commandIndex ($commandIndex +3)
		}
	}
	elseif($command -eq "7")
	{
		Write-Host "Is" $Operand1 "less than" $Operand2 -foregroundcolor red
		if($Operand1 -lt $Operand2)
		{
			$array[$ResultAddress] = "1";
		}
		else
		{
			$array[$ResultAddress] = "0";
		}
		
		Write-Host "Postition" $ResultAddress "set to" $array[$ResultAddress]
		return PerformOperation -array $array -commandIndex ($commandIndex +4)
	}
	elseif($command -eq "8")
	{
		Write-Host "Is" $Operand1 "equal to" $Operand2 -foregroundcolor red
		if($Operand1 -eq $Operand2)
		{
			$array[$ResultAddress] = "1";
		}
		else
		{
			$array[$ResultAddress] = "0";
		}
		
		Write-Host "Postition" $ResultAddress "set to" $array[$ResultAddress];
		return PerformOperation -array $array -commandIndex ($commandIndex +4)
	}
	elseif($command -eq "99")
	{
		return $array
	}
	else
	{
		Write-Host "Incorrect Command.  Recieved:" $command
		return @()
	}

}

$file = Get-Content Advent5_input.txt
$test = $file.Split(",")

$finalArray = PerformOperation -array $test -commandIndex 0

