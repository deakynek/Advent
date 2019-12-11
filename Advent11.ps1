$ShowDebugMessages = $true
$writeOutput = $false


function PerformOperation
{
	Param(
		[Parameter(Mandatory=$true)]
		[String[]] $array,
		
		[Parameter(Mandatory=$true)]
		[ref] $commandIndex,

        [Parameter(Mandatory=$true)]
		[ref] $relativeBase
		)
		
	<#Set up variables, and default operands #>
	$UnchangedInputArray = $array
	$command = $array[$commandIndex.Value]
	if($ShowDebugMessages)
	{
		Write-Host "Command" $command -foregroundcolor blue -nonewline
	}
	
	while($commandIndex.Value+3 -gt ($array.Count - 1))
	{
		$array += "0"
	}
	$Operand1Address = [long]$array[$commandIndex.Value+1]
	$Operand2Address = [long]$array[$commandIndex.Value+2]
	$Operand3Address = [long]$array[$commandIndex.Value+3]

	
	$OperandCount = 0
	$InputType = @()
	if($command.Length -gt 2)
	{
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
			Write-Host " "$array[$commandIndex.Value+1] -foregroundcolor blue
		}
	}
	elseif(($intComm -eq 5) -or ($intComm -eq 6))
	{
		$OperandCount = 2
		if($ShowDebugMessages)
		{	
			Write-Host " "$array[$commandIndex.Value+1] $array[$commandIndex.Value+2] -foregroundcolor blue
		}			
	}
	elseif(($intComm -eq 1) -or ($intComm -eq 2) -or ($intComm -eq 7) -or ($intComm -eq 8))
	{
		$OperandCount = 3
		if($ShowDebugMessages)
		{	
			Write-Host " "$array[$commandIndex.Value+1] $array[$commandIndex.Value+2] $array[$commandIndex.Value+3] -foregroundcolor blue 
		}
	}
	elseif($ShowDebugMessages)
	{	
		Write-Host""
	}
		

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
	
	<#Set Operand 1 based on type#>
	if($InputType.Count -gt 0)
	{
		if($ShowDebugMessages)
		{	
			Write-Host "OP1 InputType: " ($InputType[0]) -foregroundColor darkred
		}
		
		$Operand1 = $Operand1Address
		if($InputType[0] -eq "0")
		{
			while($Operand1Address -gt ($array.Count - 1))
			{
				$array += @("0")
			}
			
			if($intComm -ne 3)
			{
				$Operand1 = [long]$array[$Operand1Address]
			}
		}
		elseif($InputType[0] -eq "2")
		{
			while(($Operand1Address + $relativeBase.Value) -gt ($array.Count - 1))
			{
				$array += @("0")
			}
			
			$Operand1 = [long]$array[$Operand1Address + $relativeBase.Value]
			if($intComm -eq 3)
			{
				$Operand1 = $Operand1Address + $relativeBase.Value
			}
		}
		
		if($ShowDebugMessages)
		{	
			Write-Host "Op 1: "$Operand1
		}
	}	
	
	
	if($InputType.Count -gt 1)
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
	
	if($InputType.Count -gt 2)
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
	if($debug)
	{
		Write-Host "Op 3 Address : "$Operand3Address
	}
	
	$OutputIndex = (($EngineIndex+1) % ($EngineArray.Count))
	$commandIndex.Value= $commandIndex.Value + $OperandCount + 1
	
	
	if($intComm -eq 1)
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
		    Write-Host "Position" $Operand3Address "set to" $answer
        }
	}
	elseif($intComm -eq 2)
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
		    Write-Host "Position" $Operand3Address "set to" $answer
        }
	}
	elseif($intComm -eq 3)
	{
		<#Get input, Either Engine Id or preceding Engines Output#>
        if($EngineSoftwares[$EngineArray[$EngineIndex]]["IdSet"])
        {
			$PrevEngineIndex = ($EngineIndex + $EngineArray.Count - 1) % ($EngineArray.Count)
            if($ShowDebugMessages)
            {
                Write-Host "Inputing Last Output:" -ForegroundColor Cyan
                Write-Host "Output of Engine " $EngineArray[$PrevEngineIndex] "was" ($EngineSoftwares[$EngineArray[$PrevEngineIndex]]["Output"])
                Write-Host "Output Index " $OutputIndex
            }
			
			$input = ($EngineSoftwares[$EngineArray[$PrevEngineIndex]]["Output"]).ToString()
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

		
		if($ShowDebugMessages)
		{
			Write-Host "Inputing "$input" to address " ($Operand1) -ForegroundColor Cyan		
		}
		
		$array[$Operand1]  = $input.ToString()
	}
	elseif($intComm -eq 4)
	{
		<#Ouput Operand 1 to OutputsArray, holding execution of this script until going#>
		<#through all other Engines and returning to this one#>
		if($ShowDebugMessages -or $writeOutput)
		{
			Write-Host "Outputting Info" -foregroundcolor yellow
			Write-Host  $Operand1 -foregroundcolor yellow
		}
		
		$EngineSoftwares[$EngineArray[$EngineIndex]]["Output"]=  $Operand1
		$EngineSoftwares[$EngineArray[$EngineIndex]]["executeNextEngine"]=  $true
		
		if($ShowDebugMessages)
		{
			Write-Host "Feedback Index :"($commandIndex.Value)
		}
	}
	elseif($intComm -eq 5)
	{
		<#If Operand 1 is equal to 0, Jump to index Operand2#>
		if($Operand1 -ne 0)
		{
            if($ShowDebugMessages)
            {
			    Write-Host "Jump to index" $Operand2 -foregroundcolor red
            }
			
			$commandIndex.Value = $Operand2
		}
	}
	elseif($intComm -eq 6)
	{
		<#If Operand 1 equals 0, Jump to index Operand2#>
		if($Operand1 -eq 0)
		{
            if($ShowDebugMessages)
            {
			    Write-Host "Jumping to index " $Operand2
            }
			$commandIndex.Value = $Operand2
		}
	}
	elseif($intComm -eq 7)
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
	elseif($intComm -eq 8)
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
	elseif($intComm -eq 9)
	{
        if($ShowDebugMessages)
        {
			Write-Host "Relative base was " ($relativeBase.Value)", set to" ($relativeBase.Value+$Operand1)
        }	
		$relativeBase.Value = $relativeBase.Value + $Operand1
	}
	elseif($intComm -eq 99)
	{
		<#End script#>

		$EngineSoftwares[$EngineArray[$EngineIndex]]["ScriptComplete"]=  $true
		if($withPauses)
		{
			pause
		}
	}
	else
	{
		Write-Host "Incorrect Command.  Received:" $intComm "of type" ($intComm.GetType())
		$EngineSoftwares[$EngineArray[$EngineIndex]]["ScriptComplete"] = $true
		return @()
	}
	return $array
}



$file = Get-Content Advent11_input.txt
$Outputs =@(0,0)
$EngineArray = @(0)
$EngineIndex = 0
$EngineSoftwares = @{}
$EngineSoftwares[$EngineArray[0]] = @{"IdSet" = $false; "array"= $file.Split(","); "index" = 0; "relativeBase" = 0; "Output"=0; "ScriptComplete" = $false; "executeNextEngine" = $false}


Write-Host "Engine Array " $EngineArray
$FirstOutput = $false
$CurrentPosititon = @(0,0)
$Facing = 0
$DirectionCount = 4

$Points = @{"X"=@(); "Y"=@()}
$C = @()
$HighestIndex = 0

do
{
	$EngineIndex = 0
	
	

		$engine = $EngineArray[0]
		$EngineSoftwares[$engine]["executeNextEngine"] = $false
		do
		{
			$IndexReference = ([ref]($EngineSoftwares[$engine]["index"]))
			$BaseReference = ([ref]($EngineSoftwares[$engine]["relativeBase"]))
			
			$EngineSoftwares[$engine]["array"] = PerformOperation 	-array $EngineSoftwares[$engine]["array"] `
																			-commandIndex $IndexReference `
																			-relativeBase $BaseReference
																			
			
			$EngineSoftwares[$engine]["index"] = $IndexReference.Value				
			$EngineSoftwares[$engine]["relativeBase"] = $BaseReference.Value
		}
		while($EngineSoftwares[$engine]["executeNextEngine"] -eq $false -and $EngineSoftwares[$engine]["ScriptComplete"] -eq $false)
		
		if($EngineSoftwares[$engine]["index"] -gt $HighestIndex)
		{
			$HighestIndex =  $EngineSoftwares[$engine]["index"]
			Write-Host "HighestIndex " $HighestIndex
		}
		
		if($FirstOutput -eq $false -and $EngineSoftwares[$engine]["executeNextEngine"])
		{
			$FirstOutput = $true
			$PaintThisColor = $EngineSoftwares[$engine]["Output"]
			
			$TestPointWasPainted = (0..($Points["X"].Count-1)) | Where-Object{$Points["X"][$_] -eq $CurrentPosititon[0] -and $Points["X"][$_] -eq $CurrentPosititon[1]}
			
			if($PaintThisColor -eq 0)
			{
				Write-Host "`tPaint Black"
			}
			elseif($PaintThisColor -eq 1)
			{
				Write-Host "`tPaint White"
			}
			
			if($TestPointWasPainted -eq $null)
			{
				$Points["X"] += $CurrentPosititon[0]
				$Points["Y"] += $CurrentPosititon[1]
                $C += $PaintThisColor
			}
			else
			{
                
                $C[$TestPointWasPainted[0]] = $PaintThisColor
			}
			
		}
		elseif($EngineSoftwares[$engine]["executeNextEngine"])
		{
			$FirstOutput = $false
			$MoveToThisSpace = $EngineSoftwares[$engine]["Output"]

			if($MoveToThisSpace -eq "0")
			{
				$Facing = ($Facing+1)%$DirectionCount
			}
			elseif($MoveToThisSpace -eq "1")
			{
				$Facing = ($Facing+1)%$DirectionCount
			}
			
			if($Facing -eq 0)
			{
				Write-Host "Go up"
				$CurrentPosititon[1] = $CurrentPosititon[1]+1
			}
			elseif($Facing -eq 1)
			{
				Write-Host "Go right"
				$CurrentPosititon[0] = $CurrentPosititon[0]+1
			}
			elseif($Facing -eq 2)
			{
				Write-Host "Go down"
				$CurrentPosititon[1] = $CurrentPosititon[1]-1
			}
			elseif($Facing -eq 3)
			{
			
				Write-Host "Go left"
				$CurrentPosititon[0] = $CurrentPosititon[0] - 1
			}
			
			Write-Host "Going to Point  ("$CurrentPosititon[0]","$CurrentPosititon[1]")"
			
			$TestPointWasPainted = (0..($Points["X"].Count-1)) | Where-Object{$Points["X"][$_] -eq $CurrentPosititon[0] -and $Points["X"][$_] -eq $CurrentPosititon[1]}			
			if($TestPointWasPainted -eq $null)
			{
				$EngineSoftwares[$engine]["Output"] = 0
			}
			else
			{
                $EngineSoftwares[$engine]["Output"] = $C[$TestPointWasPainted[0]]
			}
		}


}while($EngineSoftwares[0]["ScriptComplete"] -eq $false )

WriteHost "Total Points" ($C.Count)

