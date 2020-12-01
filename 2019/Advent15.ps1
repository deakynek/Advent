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
			$input = Read-Host ($EngineSoftwares[$EngineArray[$EngineIndex]]["PromptMessage"])
			$EngineSoftwares[$engine]["PrintOutput"] = $true
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

function CheckPointIsComplete
{
	Param(
		[Parameter(Mandatory=$true)]
		[System.collections.arraylist] $currentPos
		)
	
	$TestPoints = @(	$currentPos,`
						(GetPos1StepForward -currentPos $currentPos -direction 1),`
						(GetPos1StepForward -currentPos $currentPos -direction 2),`
						(GetPos1StepForward -currentPos $currentPos -direction 3),`
						(GetPos1StepForward -currentPos $currentPos -direction 4))
	
	foreach($point in $TestPoints)
	{
		$PointPassed = $false
		for($i = 0; $i -lt $Points["X"].Count; $i++)
		{
			if($Points["X"][$i] -eq $point[0] -and $Points["Y"][$i] -eq $point[1])
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

function DrawScreenAndCheckComplete
{
	$xRange = $Points["X"] | Measure-Object -maximum -minimum
	$yRange = $Points["Y"] | Measure-Object -maximum -minimum
	
	Write-Host "X MIN: "($xRange.minimum) "`tX MAX: "($xRange.maximum)
	Write-Host "Y MIN: "($yRange.minimum) "`tY MAX: "($yRange.maximum)
	$RenderOutput = ""
	$pointsComplete = $true
	
	for($y = $yRange.maximum; $y -ge $yRange.minimum; $y -= 1)
	{
		for($x = $xRange.minimum; $x -le $xRange.maximum; $x++)
		{
			if($x -eq 0 -and $y -eq 0)
			{
				Write-Host "^" -nonewline -foregroundColor darkGreen
			}
			else
			{
				$pointDrawn = $false
				for($i = 0; $i -lt $Points["X"].Count; $i++)
				{
					if(($Points["X"][$i]) -ne $x -or ($Points["Y"][$i]) -ne $y)
					{
						continue
					}
					
					$MarkerInt = $Points["Marker"][$i]
					if($MarkerInt -eq 0)
					{
						Write-Host "." -nonewline
						
					}
					elseif($MarkerInt -eq 1)
					{
						$OutputChar= (($Points["Steps"][$i])%10).ToString()
						
						$Tens = [math]::floor(($Points["Steps"][$i])/10)
						$TensDigit = ($Tens%10)
						
						$color= "gray"
						if($TensDigit -eq 1)
						{
							$color = "DarkRed"
						}
						elseif($TensDigit -eq 2)
						{
							$color = "magenta"
						}
						elseif($TensDigit -eq 3)
						{
							$color = "DarkCyan"
						}
						elseif($TensDigit -eq 4)
						{
							$color = "DarkGray"
						}
						elseif($TensDigit -eq 5)
						{
							$color = "red"
						}
						elseif($TensDigit -eq 6)
						{
							$color = "blue"
						}
						elseif($TensDigit -eq 7)
						{
							$color = "yellow"
						}
						elseif($TensDigit -eq 8)
						{
							$color = "white"
						}
						elseif($TensDigit -eq 9)
						{
							$color = "cyan"
						}
						Write-Host $OutputChar  -nonewline -foregroundColor $color
					
						$pointsComplete = $pointsComplete -and (CheckPointIsComplete -currentPos @($Points["X"][$i], $Points["Y"][$i]))
						
					}
					elseif($MarkerInt -eq 2)
					{
						Write-Host "&" -nonewline -foregroundColor green
					}
					else
					{
						Write-Host " " -nonewline
					}
					$pointDrawn = $true
					break
				}
				
				if(!$pointDrawn)
				{
					Write-Host " " -nonewline
				}
			}
		}
		Write-Host ""
	}
	
	return $pointsComplete
}

function GetSteps
{
	Param(
		[Parameter(Mandatory=$true)]
		[System.collections.arraylist] $Pos
		)
		
	for($i = 0; $i -lt $Points["X"].Count; $i++)
	{
		if($Points["X"][$i] -eq $Pos[0] -and $Points["Y"][$i] -eq $Pos[1])
		{
			return $Points["steps"][$i]
		}
	}
	
	return $null
	
}


$file = Get-Content Advent15_input.txt
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
$EngineSoftwares[$EngineArray[0]] = @{"IdSet" = $true; "array"= $file.Split(","); "index" = 0; "relativeBase" = 0; "Output"=1; "ScriptComplete" = $false; "executeNextEngine" = $false; "PromptInput" = $false; "PromptMessage" = ""}
$pointsCheck = $false
$calibrated = $false
do
{
	$steps++
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
	if($debug)
	{
		Write-Host "Output was : " ($EngineSoftwares[$engine]["Output"])
	}
	$newPos = GetPos1StepForward -currentPos $CurrentPos -direction $Directions[$directionIndex]
	if([int]($EngineSoftwares[$engine]["Output"]) -eq 0 -and $EngineSoftwares[$engine]["executeNextEngine"])
	{
		<#Hit a wall#>
		SetPoint -Pos $newPos -marker 0 -color "white" -steps 0
		
		$DirectionIndex++
		$DirectionIndex = ($DirectionIndex++)%$Directions.Count
		if($debug)
		{
			Write-Host "Hit a wall"
			Write-Host "Going" -nonewline
			if($Directions[$DirectionIndex] -eq 1)
			{
				Write-Host "North"
			}
			elseif($Directions[$DirectionIndex] -eq 2)
			{
				Write-Host "South"
			}
			elseif($Directions[$DirectionIndex] -eq 3)
			{
				Write-Host "West"
			}
			elseif($Directions[$DirectionIndex] -eq 4)
			{
				Write-Host "East"
			}
		}
	}
	elseif([int]($EngineSoftwares[$engine]["Output"])-eq 1 -and $EngineSoftwares[$engine]["executeNextEngine"])
	{
		if($debug)
		{
			Write-Host "Keep Going"
		}
		SetPoint -Pos $newPos -marker 1 -color "white" -steps $steps
		$currentPos = $newPos
		$DirectionIndex = ($DirectionIndex+$Directions.Count-1)%($Directions.Count)
		
		
	}
	elseif([int]($EngineSoftwares[$engine]["Output"]) -eq 2 -and $EngineSoftwares[$engine]["executeNextEngine"])
	{	
		SetPoint -Pos $newPos -marker 2 -color "white" -steps $steps
		
		$currentPos = $newPos
		Write-Host "Reached Oxygen at ("($currentPos[0])","($currentPos[1])") in $steps"
		
		if(!$calibrated)
		{
			CalibrateSteps -steps $steps
			Write-Host "Steps here are now " (GetSteps -Pos $currentPos)
			$calibrated = $true
		}
	}
	
	if((GetSteps -Pos $currentPos) -gt $steps)
	{
		SetSteps -Pos $currentPos -steps $steps
	}
	
	if($steps -gt $maxSteps)
	{
		$maxSteps = $steps
	}
	
	$prevSteps= GetSteps -Pos $currentPos
	if($prevSteps -ne $null)
	{
		$steps = $prevSteps
	}
	
	
	
	$EngineSoftwares[$engine]["Output"] = $Directions[$DirectionIndex].ToString()
	
	
	
	if($LoopCount % $DrawLoopCount -eq 0)
	{
		Write-Host "LoopCount = " $LoopCount
		Write-Host "Steps = " $maxSteps
		
		
		$pointsCheck = DrawScreenAndCheckComplete
		Write-Host "All Points Complete = $pointsCheck`n`n"
	}
	$LoopCount++
	
}while(-not $pointsCheck)

$u = DrawScreenAndCheckComplete

$stepsRange = $Points["steps"] | Measure-Object -maximum -minimum

Write-Host "Steps MIN: "($stepsRange.minimum) "`tSteps MAX: "($stepsRange.maximum)

