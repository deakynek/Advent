$ShowDebugMessages = $false
$writeOutput = $true


function PerformOperation
{
	Param(
		[Parameter(Mandatory=$true)]
		[String[]] $array,
		
		[Parameter(Mandatory=$true)]
		[long] $commandIndex,

        [Parameter(Mandatory=$true)]
		[bool] $feedbackEnabled,

        [Parameter(Mandatory=$true)]
		[ref] $nextIndexWhenFeedbackRecieved,

        [Parameter(Mandatory=$true)]
		[ref] $relativeBase
		)
		
	<#Set up variables, and default operands #>
	$UnchangedInputArray = $array
	$command = $array[$commandIndex]
	if($ShowDebugMessages)
	{
		Write-Host "Command" $command -foregroundcolor blue -nonewline
	}
	
	while($commandIndex+3 -gt ($array.Count - 1))
	{
		$array += "0"
	}
	$Operand1Address = [long]$array[$commandIndex+1]
	$Operand2Address = [long]$array[$commandIndex+2]
	$Operand3Address = [long]$array[$commandIndex+3]

	
	$OperandCount = 0
	$InputType = @()
	if($command.Length -gt 2)
	{
		$specialMode = $true
		<#New Command Type#>
		$comm = $command.Substring($command.Length-2,2)
		
		$intComm = [long]$comm
		
	}
	else
	{
		$intComm = [long]$command
	}
	
	
	if(($intComm -eq 3) -or ($intComm -eq 4) -or ($intComm -eq 9))
	{
		$OperandCount = 1
		if($ShowDebugMessages)
		{	
			Write-Host " "$array[$commandIndex+1] -foregroundcolor blue
		}
	}
	elseif(($intComm -eq 5) -or ($intComm -eq 6))
	{
		$OperandCount = 2
		if($ShowDebugMessages)
		{	
			Write-Host " "$array[$commandIndex+1] $array[$commandIndex+2] -foregroundcolor blue
		}			
	}
	elseif(($intComm -eq 1) -or ($intComm -eq 2) -or ($intComm -eq 7) -or ($intComm -eq 8))
	{
		$OperandCount = 3
		if($ShowDebugMessages)
		{	
			Write-Host " "$array[$commandIndex+1] $array[$commandIndex+2] $array[$commandIndex+3] -foregroundcolor blue 
		}
	}
	elseif($ShowDebugMessages)
	{	
		Write-Host""
	}
		
	if($specialMode)
	{
		for($i = 1; $i -le $OperandCount; $i++)
		{
			$index = ($command.Length - 2 - $i)
			if($index -ge 0)
			{
				$temp = $command[$index]
				$InputType += $temp
			}
			else
			{
				$InputType += 0
			}
		}
	}
	$command = $intComm	
	
	
	if($specialMode -and ($InputType.Count -gt 0))
	{
	
		if($ShowDebugMessages)
		{	
			Write-Host "OP1 InputType: " ($InputType[0]) -foregroundColor darkred
		}
		if($InputType[0] -eq "0")
		{
			while($Operand1Address -gt ($array.Count - 1))
			{
				$array += @("0")
			}
			$Operand1 = [long]$array[$Operand1Address]
		}
		elseif($InputType[0] -eq "1")
		{
			$Operand1 = $Operand1Address
		}
		elseif($InputType[0] -eq "2")
		{
			while(($Operand1Address + $relativeBase.Value) -gt ($array.Count - 1))
			{
				$array += @("0")
			}
			$Operand1 = [long]$array[$Operand1Address + $relativeBase.Value]
		}
		

	}
	elseif(!$specialMode)
	{
		while($Operand1Address -gt ($array.Count - 1))
		{
			$array += @("0")
		}	
		$Operand1 = [long]$array[$Operand1Address]
	}
	if($ShowDebugMessages)
	{	
		Write-Host "Op 1: "$Operand1
	}	
	
	
	if($specialMode -and ($InputType.Count -gt 1))
	{
		
		if($ShowDebugMessages)
		{	
			Write-Host "OP2 InputType: " ($InputType[1]) -ForegroundColor darkred
		}
		
		if($InputType[1] -eq "0")
		{
			while($Operand2Address -gt ($array.Count - 1))
			{
				$array += @("0")
			}
			$Operand2 = [long]$array[$Operand2Address]
		}	
		elseif($InputType[1] -eq "1")
		{
			$Operand2 = $Operand2Address
		}
		elseif($InputType[1] -eq "2")
		{
			$Operand2 = [long]$array[$Operand2Address]
			while(($Operand2Address + $relativeBase.Value) -gt ($array.Count - 1))
			{
				$array += @("0")
			}
			$Operand2 = [long]$array[$Operand2Address + $relativeBase.Value]
	
		}

		if($ShowDebugMessages)
		{	
			Write-Host "Op 2: "$Operand2
		}
	}
	elseif(!$specialMode -and $OperandCount -gt 1)
	{
		while($Operand2Address -gt ($array.Count - 1))
		{
			$array += @("0")
		}	
		$Operand2 = [long]$array[$Operand2Address]
		

		if($ShowDebugMessages)
		{	
			Write-Host "Op 2: "$Operand2
		}
	}	
	
	if($specialMode -and ($InputType.Count -gt 2))
	{
		if($debug)
		{
			Write-Host "OP3 InputType: " ($InputType[2]) -ForegroundColor darkred
		}
		
		if($InputType[2] -eq "0")
		{
			while($Operand3Address -gt ($array.Count - 1))
			{
				$array += @("0")
			}
		}	
		elseif($InputType[2] -eq "2")
		{
			while($Operand3Address+$relativeBase.Value -gt ($array.Count - 1))
			{
				$array += @("0")
			}
			$Operand3Address += $relativeBase.Value
		}
	}
	elseif(!$specialMode -and $OperandCount -gt 1)
	{
		while($Operand3Address -gt ($array.Count - 1))
		{
			$array += @("0")
		}	
	}
	if($debug)
	{
		Write-Host "Op 3 Address : "$Operand3Address
	}
	
	$OutputIndex = (($EngineIndex+1) % ($Outputs.Count))
	$nextCommandIndex = $commandIndex + $OperandCount + 1
	
	if($command -eq 1)
	{
		<#Find Sum of Operand1 and Operand2, Store at Operand3Address#>
        if($ShowDebugMessages)
        {
		    Write-Host "Add" $Operand1 "and" $Operand2 -foregroundcolor red
        }
	
		$answer = $Operand1 + $Operand2
		$array[$Operand3Address] = $answer.ToString();
		
        if($ShowDebugMessages)
        {
		    Write-Host "Postition" $Operand3Address "set to" $answer
        }
	}
	elseif($command -eq 2)
	{
		<#Find Product of Operand1 and Operand2, Store at Operand3Address#>
        if($ShowDebugMessages)
        {
		    Write-Host "Multiply" $Operand1 "by" $Operand2 -foregroundcolor red
        }
	
		$answer = $Operand1 * $Operand2
		$array[$Operand3Address] = $answer.ToString();
		
        if($ShowDebugMessages)
        {
		    Write-Host "Postition" $Operand3Address "set to" $answer
        }
	}
	elseif($command -eq 3)
	{
		<#Get input, Either Engine Id or preceding Engines Output#>
        if($EngineSoftwares[$EngineArray[$EngineIndex]]["IdSet"])
        {
			
            if($ShowDebugMessages)
            {
                Write-Host "Inputing Last Output:" -ForegroundColor Cyan
                Write-Host "Output Array " $Outputs
                Write-Host "Output Index " $OutputIndex
            }
			
			$input = $Outputs[$EngineIndex].ToString()
		}
        else
        {
            if($ShowDebugMessages)
            {
                Write-Host "Inputing Engine Number:" -ForegroundColor Cyan
                Write-Host "Engine Array " $EngineArray
                Write-Host "Engine Index " $EngineIndex
            }
			
			$input = $EngineArray[$EngineIndex].ToString()
			$EngineSoftwares[$EngineArray[$EngineIndex]]["IdSet"]= $true
        }

		$InputAddress = $Operand1Address
		
		if($ShowDebugMessages)
		{
			Write-Host "Operand 1 Address = " $Operand1Address
		}
		if($specialMode -and ($InputType[0]-eq "2"))
		{
			
			
			$InputAddress +=  $relativeBase.Value
			if($ShowDebugMessages)
            {
				Write-Host "Relative Base Address = " $relativeBase.Value
				Write-Host "Inputing "$input" to relativeBase address " ($InputAddress) -ForegroundColor Cyan
				Write-Host "Input Address = " $InputAddress
			}
			
			
			$array[$InputAddress] = $input.ToString()
		}
		else
		{
			if($ShowDebugMessages)
            {
				Write-Host "Inputing "$input" to address " ($Operand1) -ForegroundColor Cyan			
			}
		}
		
		$array[$InputAddress]  = $input.ToString()
	}
	elseif($command -eq 4)
	{
		<#Ouput Operand 1 to OutputsArray, holding execution of this script until going#>
		<#through all other Engines and returning to this one#>
		if($ShowDebugMessages -or $writeOutput)
		{
			Write-Host "Outputing Info" -foregroundcolor yellow
			Write-Host  $Operand1 -foregroundcolor yellow
		}
		
		$Outputs[$OutputIndex] = $Operand1
	
        if($feedbackEnabled -ne $null -and $feedbackEnabled)
        {

            $nextIndexWhenFeedbackRecieved.Value = ($commandIndex +2)
            if($ShowDebugMessages)
            {
                Write-Host "Feedback Index :"($nextIndexWhenFeedbackRecieved.Value)
            }
            return $array
        }

	}
	elseif($command -eq 5)
	{
		<#If Operand 1 is equal to 0, Jump to index Operand2#>
		if($Operand1 -ne 0)
		{
            if($ShowDebugMessages)
            {
			    Write-Host "Jump to index" $Operand2 -foregroundcolor red
            }
			
			$nextCommandIndex = $Operand2
		}
	}
	elseif($command -eq 6)
	{
		<#If Operand 1 equals 0, Jump to index Operand2#>
		if($Operand1 -eq 0)
		{
            if($ShowDebugMessages)
            {
			    Write-Host "Jumping to index " $Operand2
            }
			$nextCommandIndex = $Operand2
		}
	}
	elseif($command -eq 7)
	{
		<#If Operand 1 is less than Operand 2, Set 1 to Operand3Address#>
		<#otherwise set 0 to Operand3Address#>
        if($ShowDebugMessages)
        {
		    Write-Host "Is" $Operand1 "less than" $Operand2 -foregroundcolor red
        }
		if($Operand1 -lt $Operand2)
		{
			$array[$Operand3Address] = "1";
		}
		else
		{
			$array[$Operand3Address] = "0";
		}
		
        if($ShowDebugMessages)
        {
		`	Write-Host "Postition" $Operand3Address "set to" $array[$Operand3Address]
        }
	}
	elseif($command -eq 8)
	{
		<#If Operand 1 is equal to Operand 2, Set 1 to Operand3Address#>
		<#otherwise set 0 to Operand3Address#>
        if($ShowDebugMessages)
        {
		    Write-Host "Is" $Operand1 "equal to" $Operand2 -foregroundcolor red
        }
		if($Operand1 -eq $Operand2)
		{
			$array[$Operand3Address] = "1";
		}
		else
		{
			$array[$Operand3Address] = "0";
		}

		if($ShowDebugMessages)
        {
		    Write-Host "Postition" $Operand3Address "set to" $array[$Operand3Address];
        }
	}
	elseif($command -eq 9)
	{
        if($ShowDebugMessages)
        {
			Write-Host "Relative base was " ($relativeBase.Value)", set to" ($relativeBase.Value+$Operand1)
        }	
		$relativeBase.Value = $relativeBase.Value + $Operand1
	}
	elseif($command -eq 99)
	{
		<#End script#>

		$nextIndexWhenFeedbackRecieved.value = 0
		pause
		return $array
	}
	else
	{
		Write-Host "Incorrect Command.  Recieved:" $command
		return @()
	}
	
	try{
	return PerformOperation -array $array `
							-commandIndex $nextCommandIndex `
							-feedbackEnabled $feedbackEnabled `
							-nextIndexWhenFeedbackRecieved $nextIndexWhenFeedbackRecieved `
							-relativeBase $relativeBase
	}
	catch{
			
			if($ShowDebugMessages)
			{
				Write-Host "OVERLOAD OVERLOAD" -foregroundColor RED
				Write-Host "Trying to run operation" $nextCommandIndex
				Write-Host "With Relative Base" $relativeBase.value
			}
			
			
			$nextIndexWhenFeedbackRecieved.Value = $nextCommandIndex
			return $UnchangedInputArray
			
	}
}

$file = Get-Content Advent9_input.txt
$Outputs =@(0,1)
$EngineArray = @(2)
$EngineIndex = 0
$EngineSoftwares = @{}
$EngineSoftwares[2] = @{"IdSet" = $false; "array"= @(); "index" = @()}

$nextIndex = ([ref](0))
$thisIndex = 0
$relativeBase = ([ref](0))
$oppArray = $file.Split(",")

do
{
	$thisIndex = $nextIndex.Value
	
	$oppArray = PerformOperation -array $oppArray `
							-commandIndex $thisIndex `
							-feedbackEnabled $false `
							-nextIndexWhenFeedbackRecieved $nextIndex `
							-relativeBase $relativeBase
}
while($nextIndex.Value -ne 0)
						
Write-Host "Output " $Outputs