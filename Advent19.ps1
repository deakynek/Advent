$ShowDebugMessages = $false
$writeOutput = $false
$writeInput = $false
$ShowJumps = $false


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
		Write-Host "`n`nInput Index = " ($commandIndex.Value) -foregroundColor darkGreen
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
		
	if($ShowDebugMessages)
	{	
		Write-Host "IntComm = " $intComm "`t" -ForegroundColor green
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
		if($ShowDebugMessages)
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
		
		if($ShowDebugMessages)
		{
			Write-Host "Op 3 Address : "$Operand3Address -ForegroundColor darkred
		}
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
		if($EngineSoftwares[$EngineArray[$EngineIndex]]["PromptInput"])
		{
			if($EngineSoftwares[$EngineArray[$EngineIndex]]["Input"] -ne $null)
			{
				$input  = $EngineSoftwares[$EngineArray[$EngineIndex]]["Input"]
				$EngineSoftwares[$EngineArray[$EngineIndex]]["InputConsumed"] = $true
				if($ShowDebugMessages -or $writeInput)
				{				
					Write-Host "Inputing $input to address" $Operand1
				}
			}
			else
			{
				$input = Read-Host ($EngineSoftwares[$EngineArray[$EngineIndex]]["PromptMessage"])
				$EngineSoftwares[$engine]["PrintOutput"] = $true
			}
		}
		<#Get input, Either Engine Id or preceding Engines Output#>
        elseif($EngineSoftwares[$EngineArray[$EngineIndex]]["IdSet"])
        {
			$PrevEngineIndex = ($EngineIndex + $EngineArray.Count - 1) % ($EngineArray.Count)
            if($ShowDebugMessages -or $writeInput)
            {
                Write-Host "Inputing Last Output:" -ForegroundColor Cyan
                Write-Host "Output of Engine " $EngineArray[$PrevEngineIndex] "was" ($EngineSoftwares[$EngineArray[$PrevEngineIndex]]["Output"])
                Write-Host "Output Index " $OutputIndex
            }
			
			$input = ($EngineSoftwares[$EngineArray[$PrevEngineIndex]]["Output"]).ToString()
		}
        else
        {
            if($ShowDebugMessages -or $writeInput)
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
		if($ShowDebugMessages)
        {
		    Write-Host "Is" $Operand1 "not equal to 0" -foregroundcolor red
        }
		<#If Operand 1 is not equal to 0, Jump to index Operand2#>
		if($Operand1 -ne 0)
		{
            if($ShowDebugMessages)
            {
			    Write-Host "`tJump to index" $Operand2 -foregroundcolor red
            }
			
			$commandIndex.Value = $Operand2
		}
	}
	elseif($intComm -eq 6)
	{
		if($ShowDebugMessages)
        {
		    Write-Host "Is" $Operand1 "equal to 0" -foregroundcolor red
        }
		<#If Operand 1 equals 0, Jump to index Operand2#>
		if($Operand1 -eq 0)
		{
            if($ShowDebugMessages)
            {
			    Write-Host "`tJumping to index " $Operand2
            }
			$commandIndex.Value = $Operand2
		}
	}
	elseif($intComm -eq 7)
	{
		<#If Operand 1 is less than Operand 2, Set 1 to Operand3Address#>
		<#otherwise set 0 to Operand3Address#>
        if($ShowDebugMessages -or $ShowJumps)
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
		
        if($ShowDebugMessages -or $ShowJumps)
        {
		`	Write-Host "`tPosition" $Operand3Address "set to" $array[$Operand3Address]
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
		    Write-Host "`tPosition" $Operand3Address "set to" $array[$Operand3Address];
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
	
	if($ShowDebugMessages)
	{
		Write-Host "Output Index = " ($commandIndex.Value) -foregroundColor darkGreen
	}
	return $array
}

function SetPoint
{
	Param(
		[Parameter(Mandatory=$true)]
		[System.collections.arraylist] $Pos,
		
		[Parameter(Mandatory=$true)]
		[int] $marker,
		
		[Parameter(Mandatory=$true)]
		[int] $steps,
		
		[Parameter(Mandatory=$true)]
		[string] $color
		)
	
	$x = $Pos[0]
	$y = $Pos[1]
	
	$index = $null
	for ($i = 0; $i -lt $Points["X"].Count; $i++)
	{
		if($Points["X"][$i] -ne $x)
		{
			continue
		}
		elseif($Points["Y"][$i] -ne $y)
		{
			continue
		}
		else
		{
			$index = $i
			break
		}
	}
	
	if($index -ne $null)
	{
		$Points["Marker"][$index] = $marker
		$Points["color"][$index] = $color
	}
	else
	{
		$Points["X"] += $x
		$Points["Y"] += $y
		$Points["Marker"] += $marker
		$Points["color"] += $color
		$Points["steps"] += $steps
	}
}

function SetSteps
{
	Param(
		[Parameter(Mandatory=$true)]
		[System.collections.arraylist] $Pos,
		
		[Parameter(Mandatory=$true)]
		[int] $steps
		)
		
	$x = $Pos[0]
	$y = $Pos[1]
	
	$index = $null
	for ($i = 0; $i -lt $Points["X"].Count; $i++)
	{
		if($Points["X"][$i] -ne $x)
		{
			continue
		}
		elseif($Points["Y"][$i] -ne $y)
		{
			continue
		}
		else
		{
			$index = $i
			break
		}
	}
	
	if($index -ne $null)
	{
		$Points["steps"][$index] = $steps
	}
}


function GetPos1StepForward
{
	Param(
		[Parameter(Mandatory=$true)]
		[System.collections.arraylist] $currentPos,
		
		[Parameter(Mandatory=$true)]
		[int] $direction
		)
	
	$x = $currentPos[0]
	$y = $currentPos[1]
	if($debug)
	{
		Write-Host "CurrentPos is ($x,$y)" -foregroundColor cyan
	}
	
	if($direction -eq 1)
	{
		$y++
	}
	elseif($direction -eq 2)
	{
		$y -= 1
	}
	elseif($direction -eq 3)
	{
		$x -= 1
	}
	elseif($direction -eq 4)
	{
		$x++
	}
	
	if($debug)
	{
		Write-Host "New Position is ($x,$y)" -foregroundColor yellow
	}
	return @($x,$y)
}

function CalibrateSteps
{
	Param(
		[Parameter(Mandatory=$true)]
		[int] $steps
		)
		
	for($i = 0; $i -lt $Points["steps"].Count; $i++)
	{
		$Points["steps"][$i] = $steps - $Points["steps"][$i] 
	}
}

function CheckPointIsIntersection
{
	Param(
		[Parameter(Mandatory=$true)]
		[System.collections.arraylist] $currentPos,
		[Parameter(Mandatory=$true)]
		[int] $MarkerInt
		)
	
	$TestPoints = @(	(GetPos1StepForward -currentPos $currentPos -direction 1),`
						(GetPos1StepForward -currentPos $currentPos -direction 2),`
						(GetPos1StepForward -currentPos $currentPos -direction 3),`
						(GetPos1StepForward -currentPos $currentPos -direction 4))
	
	$currentMarkerInt = $null
	for($i = 0; $i -lt $Points["X"].Count; $i++)
	{
		if($Points["X"][$i] -eq $currentPos[0] -and $Points["Y"][$i] -eq $currentPos[1])
		{
			$currentMarkerInt  = $Points["Marker"][$i]
			break
		}
	}
	if($currentMarkerInt -eq $null -or $currentMarkerInt -ne $MarkerInt)
	{
		return $false
	}
	
	
	foreach($point in $TestPoints)
	{
		$PointPassed = $false
		for($i = 0; $i -lt $Points["X"].Count; $i++)
		{
			if($Points["X"][$i] -eq $point[0] -and`
			   $Points["Y"][$i] -eq $point[1] -and`
			   $Points["Marker"][$i] -eq $MarkerInt)
			{
				$PointPassed = $true
				break
			}
		}
		
		if(-not $PointPassed)
		{
			return $false
		}
	}
	
	return $true
}

function CountIntersections
{
	$xRange = $Points["X"] | Measure-Object -maximum -minimum
	$yRange = $Points["Y"] | Measure-Object -maximum -minimum
	
	Write-Host "X MIN: "($xRange.minimum) "`tX MAX: "($xRange.maximum)
	Write-Host "Y MIN: "($yRange.minimum) "`tY MAX: "($yRange.maximum)
	
	$intersectionCount = 0
	
	for($y = $yRange.maximum; $y -ge $yRange.minimum; $y -= 1)
	{
		for($x = $xRange.minimum; $x -le $xRange.maximum; $x++)
		{
			for($i = 0; $i -lt $Points["X"].Count; $i++)
			{
				if(($Points["X"][$i]) -ne $x -or ($Points["Y"][$i]) -ne $y )
				{
					continue
				}
				
				if( CheckPointIsIntersection -currentPos @($x,$y) -marker ([int][char]"#"))
				{
					$intersectionCount++
				}
			}
		}
	}
	
	return $intersectionCount
}

function DrawScreen
{
	$xRange = $Points["X"] | Measure-Object -maximum -minimum
	$yRange = $Points["Y"] | Measure-Object -maximum -minimum
	
	Write-Host "X MIN: "($xRange.minimum) "`tX MAX: "($xRange.maximum)
	Write-Host "Y MIN: "($yRange.minimum) "`tY MAX: "($yRange.maximum)
	
	$alignmentParam = 0
	for($y = $yRange.maximum; $y -ge $yRange.minimum; $y -= 1)
	{
		for($x = $xRange.minimum; $x -le $xRange.maximum; $x++)
		{
			$pointDrawn = $false
			for($i = 0; $i -lt $Points["X"].Count; $i++)
			{
				if(($Points["X"][$i]) -ne $x -or ($Points["Y"][$i]) -ne $y)
				{
					continue
				}
				
				$MarkerInt = $Points["Marker"][$i]
				
				if( CheckPointIsIntersection -currentPos @($x,$y) -marker ([int][char]"#"))
				{
					$alignmentParam += [math]::abs($x*$y)
				}				
				
				
				$MarkerChar = [char]$MarkerInt
				Write-Host $MarkerChar -nonewline
				
				$pointDrawn = $true
				break
			}
			
			
			if(!$pointDrawn)
			{
				Write-Host " " -nonewline
			}
		}
		Write-Host ""
	}
	
	
	return $alignmentParam
}

function GetMarker
{
	Param(
		[Parameter(Mandatory=$true)]
		[System.collections.arraylist] $Pos
		)
		
	for($i = 0; $i -lt $Points["X"].Count; $i++)
	{
		if(($Points["X"][$i]) -ne $Pos[0] -or ($Points["Y"][$i]) -ne $Pos[1])
		{
			continue
		}
		
		return $Points["Marker"][$i]
	}
	return $null
}

function NextToScaffold
{
	Param(
		[Parameter(Mandatory=$true)]
		[System.collections.arraylist] $Pos,
		[Parameter(Mandatory=$true)]
		[int] $Facing
		)
	
	$scaffoldMarker = [int][char]"#"
	if(($Facing % 2) -eq 0)
	{
		return (GetMarker -Pos @(($Pos[0]+1),$Pos[1])) -eq $scaffoldMarker -or`
				(GetMarker -Pos @(($Pos[0] - 1),$Pos[1])) -eq $scaffoldMarker
	}
	else
	{
		return (GetMarker -Pos @(($Pos[0]),($Pos[1] - 1))) -eq $scaffoldMarker -or`
				(GetMarker -Pos @(($Pos[0]),($Pos[1]+1))) -eq $scaffoldMarker
	}

}

function NextTurnToScaffold
{
	Param(
		[Parameter(Mandatory=$true)]
		[System.collections.arraylist] $Pos,
		[Parameter(Mandatory=$true)]
		[int] $Facing
		)
	
	$scaffoldMarker = [int][char]"#"
	if($Facing -eq 0)
	{
		if((GetMarker -Pos @(($Pos[0]+1),($Pos[1]))) -eq $scaffoldMarker)
		{
			return 1
		}
		elseif((GetMarker -Pos @(($Pos[0]-1),($Pos[1]))) -eq $scaffoldMarker)
		{
			return -1
		}
	}
	elseif($Facing -eq 1)
	{
		if((GetMarker -Pos @(($Pos[0]),($Pos[1]+1))) -eq $scaffoldMarker)
		{
			return -1
		}
		elseif((GetMarker -Pos @(($Pos[0]),($Pos[1]-1))) -eq $scaffoldMarker)
		{
			return 1
		}
	}
	elseif($Facing -eq 2)
	{
		if((GetMarker -Pos @(($Pos[0]+1),($Pos[1]))) -eq $scaffoldMarker)
		{
			return -1
		}
		elseif((GetMarker -Pos @(($Pos[0]-1),($Pos[1]))) -eq $scaffoldMarker)
		{
			return 1
		}
	}
	elseif($Facing -eq 3)
	{
		if((GetMarker -Pos @(($Pos[0]),($Pos[1]+1))) -eq $scaffoldMarker)
		{
			return 1
		}
		elseif((GetMarker -Pos @(($Pos[0]),($Pos[1]-1))) -eq $scaffoldMarker)
		{
			return -1
		}
	}	
}

function DistanceToNextHole
{
	Param(
		[Parameter(Mandatory=$true)]
		[System.collections.arraylist] $Pos,
		[Parameter(Mandatory=$true)]
		[int] $Facing
		)

	$scaffoldMarker = [int][char]"#"
	$count = 1
	
	if($Facing -eq 0)
	{
		while((GetMarker -Pos @($Pos[0],($Pos[1]+$count))) -eq $scaffoldMarker)
		{
			$count++
		}
	}
	elseif($Facing -eq 1)
	{
		while((GetMarker -Pos @(($Pos[0]+$count),$Pos[1])) -eq $scaffoldMarker)
		{
			$count++
		}	
	}
	elseif($Facing -eq 2)
	{
		while((GetMarker -Pos @($Pos[0],($Pos[1]-$count))) -eq $scaffoldMarker)
		{
			$count++
		}
	}
	elseif($Facing -eq 3)
	{
		while((GetMarker -Pos @(($Pos[0]-$count),$Pos[1])) -eq $scaffoldMarker)
		{
			$count++
		}
	}
	
	return ($count - 1)
}


function ProcessInstructions
{		
	$xRange = $Points["X"] | Measure-Object -maximum -minimum
	$yRange = $Points["Y"] | Measure-Object -maximum -minimum
	
	<#FindStart and Direction Facing#>
	
	$FacingIndex = $null
	$start = @(0,0)
	$startFound = $false
	for($y = $yRange.maximum; $y -ge $yRange.minimum; $y -= 1)
	{
		for($x = $xRange.minimum; $x -le $xRange.maximum; $x++)
		{
			for($i = 0; $i -lt $Points["X"].Count; $i++)
			{
				if(($Points["X"][$i]) -ne $x -or ($Points["Y"][$i]) -ne $y)
				{
					continue
				}
				
				if($Points["Marker"][$i] -eq [int][char]"^")
				{
					$FacingIndex = 0
				}
				elseif($Points["Marker"][$i] -eq [int][char]">")
				{
					$FacingIndex = 1
				}
				elseif($Points["Marker"][$i] -eq [int][char]"v")
				{
					$FacingIndex = 2
				}
				elseif($Points["Marker"][$i] -eq [int][char]"<")
				{
					$FacingIndex = 3
				}
				
				if($FacingIndex -ne $null)
				{
					$start = @($x,$y)
					$startFound = $true
					break
				}
			}
			
			if($startFound)
			{
				break
			}
		}
		
		if($startFound)
		{
			break
		}	
	}
	
	$CommandList = ""
	
	Write-Host "Start is at ("$start[0]","$start[1]")"
	Write-Host "Facing is $FacingIndex"
	$robotPos = $start
	while(NextToScaffold -Pos $robotPos -Facing $FacingIndex)
	{
		$NextTurn = NextTurnToScaffold -Pos $robotPos -Facing $FacingIndex
		$FacingIndex = ($FacingIndex+4+$NextTurn)%4
		
		if($NextTurn -eq 1)
		{
			$CommandList += "R, "
		}
		elseif($NextTurn -eq -1)
		{
			$CommandList += "L, "
		}
		
		$dist = DistanceToNextHole -Pos $robotPos -Facing $FacingIndex
		
		$CommandList += "$dist, "
		
		if($FacingIndex -eq 0)
		{
			$robotPos[1] += $dist
		}
		elseif($FacingIndex -eq 1)
		{
			$robotPos[0] += $dist
		}
		elseif($FacingIndex -eq 2)
		{
			$robotPos[1] = $robotPos[1] - $dist
		}
		elseif($FacingIndex -eq 3)
		{
			$robotPos[0] = $robotPos[0] - $dist
		}
		
	}
	
	Write-Host "Command List: " 
	Write-Host $CommandList
	
	
}

function TestPoint
{
	Param(
		[Parameter(Mandatory=$true)]
		[int] $x,
		[Parameter(Mandatory=$true)]
		[int] $y
		)
		
	Write-Host "Testing point ( $x , $y )"
	$EngineSoftwares[$EngineArray[0]] = @{"IdSet" = $true; "array"= $file.Split(","); "index" = 0; "relativeBase" = 0; "Output"=1; "ScriptComplete" = $false; "executeNextEngine" = $false; "PromptInput" = $true; "PromptMessage" = "Give input"; "Input" = $null; "InputConsumed" = $false}
	
	$EngineSoftwares[$EngineArray[0]]["Input"] = $x
	
	do
	{

		$EngineIndex = 0
		$engine = $EngineArray[0]
		$EngineSoftwares[$engine]["executeNextEngine"] = $false
		
		if($EngineSoftwares[$engine]["InputConsumed"])
		{
			$EngineSoftwares[$engine]["Input"] = $y
		}
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
		while($EngineSoftwares[$engine]["executeNextEngine"] -eq $false -and $EngineSoftwares[$engine]["ScriptComplete"] -eq $false -and $EngineSoftwares[$engine]["InputConsumed"] -eq $false)
		
		if($false)
		{
			Write-Host "Output was : " ($EngineSoftwares[$engine]["Output"])
		}
		
		if($EngineSoftwares[$engine]["executeNextEngine"])
		{
			$outInt = [int]($EngineSoftwares[$engine]["Output"])
		}
		
	}while($EngineSoftwares[$engine]["ScriptComplete"] -eq $false)

	return $outInt
}


$file = Get-Content Advent19_input.txt
$EngineArray = @(1)
$EngineIndex = 0
$EngineSoftwares = @{}
$Output = 1
$width = 0
$Directions = @(1,4,2,3)
$DirectionIndex = 0
$CurrentPos = @(0,0)

$Points = @{"X" = @(0); "Y" = @(0); "Marker" = @("0"); "color" =@("white"); "steps" = @(0) }
$OxNotReached = $true
$LoopCount = 1
$DrawLoopCount = 1000

$debug = $false
$steps = 0
$maxSteps = 0
$pointsCheck = $false
$calibrated = $false


<#Hard coded solution found by hand for this specific problem.  Not ideal, but enough to get by#>
$IntCodeInput = @()
$Xs = @(0..49)
$Ys = @(0..49)

<#Part1#>
<#
$TractorCount = 0
foreach($x in $Xs)
{
	foreach($y in $Ys)
	{

		$outInt = TestPoint -x $x -y $y

		
		$outStr =  " "
		if($outInt -eq 0)
		{
			$outStr = "."
		}
		elseif($outInt -eq 1)
		{
			$outStr = "#"
			Write-Host "Setting Point ( $x , -$y ) to $outStr"
			$TractorCount++
		}
		
		$Points["X"]+=$x
		$Points["Y"]+= -1 * $y
		$Points["Marker"] += [int][char]$outStr
		$Points["color"] += "white"
	}
}
$alignmentParams = DrawScreen
Write-Host "Counted $TractorCount spaces effected by the tractor beam"
#>
$size = 99
$CurrentPoint = @(0,$size)
$UpperLeftCorner = @()
$found = $false
do
{
	Write-Host "Testing Point : ("($CurrentPoint[0])","($CurrentPoint[1])")"
	$outInt = TestPoint -x ($CurrentPoint[0]) -y ($CurrentPoint[1])
	
	if($outInt -eq 1)
	{
		if($CurrentPoint[1] -gt $size)
		{
			$TestUpperRightCorner = TestPoint -x ($CurrentPoint[0] +$size) -y ($CurrentPoint[1] - $size)
			
			if($TestUpperRightCorner -eq 1)
			{
				$found = $true
				$UpperLeftCorner += $CurrentPoint[0]
				$UpperLeftCorner += ($CurrentPoint[1] - $size)
				continue
			}
		}
		
		$CurrentPoint[1] += 1
	}
	elseif($outInt -eq 0)
	{
		$CurrentPoint[0] += 1
	}	
}while($found -eq $false)

Write-Host "Upper Left Corner is ("($UpperLeftCorner[0])","(-1*$UpperLeftCorner[1])")" -foregroundColor green
$UpperRightCorner = @(($CurrentPoint[0] +$size), (-1*($CurrentPoint[1] - $size)))
$LowerRightCorner = @(($CurrentPoint[0] +$size), (-1*($CurrentPoint[1])))

Write-Host "Upper Right Corner is ("($UpperRightCorner[0])","($UpperRightCorner[1])")"
Write-Host "Lower Right Corner is ("($LowerRightCorner[0])","($LowerRightCorner[1])")"
Write-Host "Lower Left Corner is ("($CurrentPoint[0])","(-1*$CurrentPoint[1])")"

<#
SetPoint -pos @($UpperLeftCorner[0], -1*$UpperLeftCorner[1]) -marker ([int][char]"F") -steps 0 -color "blue" 
SetPoint -pos @($CurrentPoint[0], -1*$CurrentPoint[1]) -marker ([int][char]"L") -steps 0 -color "blue" 
SetPoint -pos $UpperRightCorner -marker ([int][char]"7") -steps 0 -color "blue" 
SetPoint -pos $LowerRightCorner -marker ([int][char]"J") -steps 0 -color "blue" 
$alignmentParams = DrawScreen#>
