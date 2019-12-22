<#Find Greatest Common Denominator#>
function GetAvailableSpaces
{
	Param(
		[Parameter(Mandatory=$true)]
		[string] $MapString,
		
		[Parameter(Mandatory=$true)]
		[int] $startingIdx,
		
		[Parameter(Mandatory=$true)]
		[system.collections.hashtable] $ExistingKeysWithSteps,
		
		[Parameter(Mandatory=$true)]
		[bool] $PassDoors
		)

	$steps = 0
	
	$IndexesVisited = @(GetIndexes $ExistingKeysWithSteps)
	
	$Facing = 0
	$index = $startingIdx
	$CompletedIndex = @()
	
	while(-not (TraveledToAllAvailableSpaces -MapString $MapString -IndexesVisited $IndexesVisited -refCompletedIndexes ([ref]$CompletedIndex) -refIndex ([ref]$index) -Facing ([ref]$Facing) -PassDoors $PassDoors))
	{
		$startingIdx = $index
		if($ExistingKeysWithSteps.Keys -contains $startingIdx -and $ExistingKeysWithSteps[$startingIdx]["steps"] -ne $null)
		{
			$steps = $ExistingKeysWithSteps[$startingIdx]["steps"]
		}
		else
		{
			$steps = 0
		}
		
		if($ExistingKeysWithSteps.Keys -contains $startingIdx -and $ExistingKeysWithSteps[$startingIdx]["keys"] -ne $null)
		{
			$keys = $ExistingKeysWithSteps[$startingIdx]["keys"]
		}
		else
		{
			$keys = @()
		}
		
		
		if($debug)
		{
			Write-Host "`nReceived $index, starting at"
			PrintIndexInCoordinates -Index $index
			Write-Host "steps here is $steps"
			Write-Host "keys required here are: "$keys
		}
			
		
		if($Facing -eq 0) <#North#>
		{
			$NextIdx = $startingIdx +  $Width
		}
		elseif($Facing -eq 1) <#East#>
		{
			$NextIdx =$startingIdx + 1
		}
		elseif($Facing -eq 2) <#South#>
		{
			$NextIdx = $startingIdx - $Width
		}
		elseif($Facing -eq 3) <#West#>
		{
			$NextIdx =$startingIdx - 1
		}
		
		
		$NextCharResult = GetCharAtIndx -MapString $MapString -Idx $NextIdx
		if($debug)
		{
			$StartHeight = [math]::floor($NextIdx/$Width)
			$StartWidth = $NextIdx%$Width
			
			Write-Host "Index $NextIdx" -foregroundcolor cyan
			Write-Host "Testing ("$StartWidth","$StartHeight")"
			Write-Host "Char Result is: "$NextCharResult
		}
		
		if($NextCharResult -eq 1 -or $NextCharResult -eq 2 -or ($NextCharResult -eq 3 -and $PassDoors))
		{
			$steps++
			
			if($IndexesVisited -notcontains $NextIdx)
			{
				$IndexesVisited += $NextIdx
			}
			

			if($NextCharResult -eq 3 -and $PassDoors)
			{
				if($keys -notcontains ($MapStr0[$NextIdx]))
				{
					$keys += ([string]($MapStr0[$NextIdx])).ToLower()
					if($debug)
					{
						Write-Host "Found door these keys needed at index $NextIdx :  $keys" -foregroundcolor red
					}
				}
			}			
			
			if(-not ($ExistingKeysWithSteps.Keys -ccontains $NextIdx))
			{
				$ExistingKeysWithSteps += @{$NextIdx = @{"steps" = $steps; "keys" = $keys}}
			}
			elseif($ExistingKeysWithSteps[$NextIdx]["steps"] -eq $null)
			{
				$ExistingKeysWithSteps[$NextIdx]["steps"] = $steps
			}
			
			if($NextCharResult -eq 2)
			{
				$key = $MapString[$NextIdx]
				if(-not ($ExistingKeysWithSteps.Keys -ccontains "$key"))
				{
					$ExistingKeysWithSteps += @{"$key"=$steps}
				}
				elseif($ExistingKeysWithSteps["$key"] -eq $null)
				{
					$ExistingKeysWithSteps["$key"] = $steps
				}
				
				if($keys -notcontains ($MapStr0[$NextIdx]))
				{
					$keys += ([string]($MapStr0[$NextIdx])).ToLower()
					if($debug)
					{
						Write-Host "Key Found: these keys needed at index $NextIdx :  $keys" -foregroundcolor green
					}
				}		
			}
		}
		
		$ExistingKeysWithSteps[$NextIdx]["keys"] = $keys
		
		if($debug)
		{
			Write-Host "Setting $keys to $starting index"
			$test = $ExistingKeysWithSteps[$startingIdx]["keys"]
			Write-Host "Retrieval Test $test"
		}
		
	}
	return $ExistingKeysWithSteps
}

function GetCharAtIndx
{
	Param(
		[Parameter(Mandatory=$true)]
		[string] $MapString,
		
		[Parameter(Mandatory=$true)]
		[int] $Idx
		)
	
	$NextChar = $MapString[$Idx]
	
	if($NextChar -eq "#")
	{
		return 0 
	}
	elseif($NextChar -eq ".")
	{
		return 1
	}
	elseif($NextChar -cmatch "[a-z]")
	{
		return 2
	}
	elseif($NextChar -cmatch "[A-Z]")
	{
		return 3
	}
	else
	{
		return 4
	}
}

function TraveledToAllAvailableSpaces
{
	Param(
		[Parameter(Mandatory=$true)]
		[string] $MapString,
		
		[Parameter(Mandatory=$true)]
		[System.Collections.ArrayList] $IndexesVisited,
		
		[Parameter(Mandatory=$true)]
		[ref] $refCompletedIndexes,
		
		[Parameter(Mandatory=$true)]
		[ref] $refIndex,
		
		[Parameter(Mandatory=$true)]
		[ref] $Facing,
		
		[Parameter(Mandatory=$true)]
		[bool] $PassDoors
		)
		
	
	$uncompleted = Compare-Object $IndexesVisited $refCompletedIndexes.Value | where {$_.sideindicator -eq "<="} | % {$_.inputobject}
	
	foreach ($ind in $uncompleted)
	{
	
		$fourPos = @(($ind + $width), ($ind+1), ($ind - $width),($ind - 1))
		foreach($pos in $fourPos)
		{
			$char = $MapString[$pos]
		
			if($char -eq "#" -or (($char -cmatch "[A-Z]") -and (-not $PassDoors)))
			{
				continue
			}
			
			if($IndexesVisited -notcontains $pos)
			{
				if($debug)
				{
					Write-Host "Point not set"
					PrintIndexInCoordinates -Index $pos
				}
				
				$StartHeight = [math]::floor($pos/$Width)
				$StartWidth = $pos%$Width
				$refIndex.Value = $ind
				$Facing.Value = $fourPos.IndexOf($pos)
				
 				return $false
			}
		}
		
		$refCompletedIndexes.Value += $ind
	}
	
	return $true
}

function ClearSteps
{
	Param(
		[Parameter(Mandatory=$true)]
		[system.collections.hashtable] $ExistingKeysWithSteps
		)
		
		
	foreach($key in $ExistingKeysWithSteps.Keys)
	{
		$ExistingKeysWithSteps[$key] = $null
	}
}

function GetKeys
{
	Param(
		[Parameter(Mandatory=$true)]
		[system.collections.hashtable] $ExistingKeysWithSteps
		)
		
	$keys = [System.Collections.ArrayList]@()
	foreach($key in $ExistingKeysWithSteps.Keys)
	{
		if($key -is [string] -and $key -match "^\w$")
		{
			$keys += $key
		}
	}
	
	return $keys
}

function GetIndexes
{
	Param(
		[Parameter(Mandatory=$true)]
		[system.collections.hashtable] $ExistingKeysWithSteps
		)
		
	$keys =  [System.Collections.ArrayList]@()
	foreach($key in $ExistingKeysWithSteps.Keys)
	{
		if($key -is [int])
		{
			$keys += $key
		}
	}
	
	return $keys
}

function GetKeyIndex
{
	Param(
		
		[Parameter(Mandatory=$true)]
		[int] $StartIndex,
		
		[Parameter(Mandatory=$true)]
		[System.Collections.ArrayList] $KeysArray
		
		)
		
	foreach($key in $StepsHash[($MapStr0[$StartIndex]).ToString()].Keys)
	{
		if($key.length -ne $KeysArray.count)
		{
			continue
		}
		
		$match = $true
		for($i = 0; $i -lt $KeysArray.count; $i++)
		{
			if(-not ($key -cmatch $KeysArray[$i]))
			{
				$match = $false
				break
			}
		}
		
		if($match)
		{
			return $key
		}
	}
	
	return $null
}


function RecurseGetMinStepsToAllKeys
{
	Param(
		[Parameter(Mandatory=$true)]
		[string] $Key,
		
		[Parameter(Mandatory=$true)]
		[system.collections.arrayList] $visitedKeys,
		
		[Parameter(Mandatory=$true)]
		[int] $TabCount
		)
	
	$tab ="`t"
	if($True)
	{
		Write-Host ($tab*$TabCount)"STARTING AT" ($Key) -foregroundcolor CYAN
	}

	$NextKeyArray = [System.Collections.ArrayList] @(GetKeys -ExistingKeysWithSteps ($StepsHash[$key]))
	
	$reqKeys = @()
	if($FromOriginWithDoorCodes[$MapStr0.IndexOf($key)]["keys"].Count -gt 0)
	{
		$reqKeys = $FromOriginWithDoorCodes[$MapStr0.IndexOf($key)]["keys"]
	}
	
	
	foreach($pastKey in $visitedKeys)
	{
		$x = $NextKeyArray.Remove($pastKey)
	}
	
	if($debug)
	{
		Write-Host ($tab*$TabCount)"Next Options: "$NextKeyArray -foregroundcolor green
		Write-Host ($tab*$TabCount)"Keys Already Gotten : "$visitedKeys -foregroundcolor yellow
		Write-Host ($tab*$TabCount)"Required for $Key : "$reqKeys -foregroundcolor red
		Write-Host ($tab*$TabCount)"Remaining Keys :"($TotalKeyCount - $visitedKeys.Count) " of $TotalKeyCount total" -foregroundcolor darkred
	}
	
	foreach($reqKey in $reqKeys)
	{
		if($visitedKeys -notcontains $reqKey)
		{
			if($debug)
			{
				Write-Host ($tab*$TabCount)"1) Returning NULL" -foregroundcolor red 
				Write-Host ($tab*$TabCount)"of Type nothing" -foregroundcolor red
			}
			return $null
		}
	}

	


	
	if($NextKeyArray.count -gt 0 -and $visitedKeys.count + $NextKeyArray.count -eq $TotalKeyCount)
	{	
		$ind = GetKeyIndex -StartIndex ($MapStr0.IndexOf($key)) -KeysArray @($NextKeyArray)
		if($ind -ne $null)
		{
			if($debug -and ($StepsHash[$Key]["$NextKeyArray"]) -ne $null)
			{
				Write-Host ($tab*$TabCount)""($Key) "to $NextKeyArray predetermined to be " ($StepsHash[$Key]["$NextKeyArray"]) -foregroundcolor DarkCyan
				Write-Host ($tab*$TabCount)"2) Returning " ($StepsHash[$Key]["$NextKeyArray"])  -foregroundcolor red 
				Write-Host ($tab*$TabCount)"of Type " ($StepsHash[$Key]["$NextKeyArray"]).GetType() -foregroundcolor red	
			}

			return $StepsHash[$Key]["$NextKeyArray"]
		}
	}
	
	$minSteps = $null
	$NewSteps = 0
	if($NextKeyArray.Count -gt 0)
	{
		if($NextKeyArray.Count -eq 1 -and ($visitedKeys.count) -eq ($TotalKeyCount))
		{
			if($debug)
			{
				Write-Host ($tab*$TabCount)"3) Returning " ($StepsHash[$key][$NextKeyArray[0]])  -foregroundcolor red 
				Write-Host ($tab*$TabCount)"of Type " ($StepsHash[$key][$NextKeyArray[0]]).GetType() -foregroundcolor red	
			}
			return $StepsHash[$key][$NextKeyArray[0]]
		}
	
		foreach($nextKey in $NextKeyArray)
		{
			$x = $visitedKeys.Add($nextKey)
			$baseSteps = $StepsHash[$key][$nextKey]
			
			if($debug -and $baseSteps -ne $null)
			{
				Write-Host ($tab*$TabCount)"Steps From $key to $nextKey : $baseSteps"
				Write-Host ($tab*$TabCount)"Type baseSteps =" ($baseSteps.GetType())
			}
			
			
			$NewSteps = RecurseGetMinStepsToAllKeys -Key $nextKey -visitedKeys $visitedKeys -TabCount ($TabCount+1)
			
			if($debug -and $NewSteps -ne $null)
			{			
				Write-Host ($tab*$TabCount)"Steps Returned: $NewSteps"
				Write-Host ($tab*$TabCount)"Type returned steps =" ($NewSteps.GetType())
			}
			
			
			$x = $visitedKeys.Remove($nextKey)
			if($NewSteps -eq $null)
			{
				continue
			}
			
			$TotalSteps = $baseSteps + $NewSteps
			if($minSteps -eq $null -or $TotalSteps -lt $minSteps)
			{
				$minSteps = $TotalSteps
			}
		}
	}
	else
	{
		if($debug -and $NewSteps -ne $null)
		{
			Write-Host ($tab*$TabCount)"4) Returning " (0) -foregroundcolor red
			Write-Host ($tab*$TabCount)"of Type Int"  -foregroundcolor red	
		}
		return 0
	}
	
	if($NextKeyArray.Count -ne 1)
	{
		$StepsHash[$Key] += @{"$NextKeyArray" = $minSteps}
	}
	
	if($debug -and $minSteps -ne $null)
	{
		Write-Host ($tab*$TabCount)"5) Returning " ($minSteps) -foregroundcolor red
		Write-Host ($tab*$TabCount)"of Type " ($minSteps.GetType()) -foregroundcolor red
	}
	return ($minSteps)
}

function GetMinStepsFromOrigin
{
	$minsteps = $null
	$stringOfAllKeys = "$AllKeys"
	$MinKey = ""
	
	foreach($key in (GetKeys -ExistingKeysWithSteps $FromOrigin))
	{
		$BaseSteps = $FromOrigin[$key]
		
		
		$ExtraSteps = $StepsHash[$key][$stringOfAllKeys.Remove($stringOfAllKeys.IndexOf($key),1)]
		if($debug)
		{
			Write-Host "origin -> $key : $baseSteps"
			Write-Host "Min $key -> "($stringOfAllKeys.Remove($stringOfAllKeys.IndexOf($key),1))" : $ExtraSteps"
		}
		
		if($ExtraSteps -ne $null -and ($minsteps -eq $null -or ($BaseSteps +$ExtraSteps) -lt $minsteps))
		{
			$minSteps = ($BaseSteps +$ExtraSteps)
			$MinKey = $key
		}
	}

	Write-Host "$t Min Origin -> $stringOfAllKeys : $minSteps" -foregroundcolor yellow
	Write-Host "$t Min $starter -> $MinKey" -foregroundcolor cyan	
	return $minsteps
}


function PrintIndexInCoordinates
{
	Param(
		[Parameter(Mandatory=$true)]
		[int] $Index
		)

		$StartHeight = [math]::floor($Index/$Width)
		$StartWidth = $Index%$Width
		Write-Host "("$StartWidth","$StartHeight")"
		
}

function RecurseFindMinStepsBasedOnKeyCombo
{
	Param(
		[Parameter(Mandatory=$true)]
		[string] $string,
		[Parameter(Mandatory=$true)]
		[int] $TabCount
		)
	
	$t = ("`t")*$TabCount
	if($string.length -eq 0)
	{
		return
	}	
	if($string.length -eq 1)
	{
		return
	}
	
	if($InvalidStringArray -contains $string)
	{
		return
	}
	
	Write-Host "$t Testing String $string"

	$Combos = @()
	$noneValid = $true
	for($i = 0; $i -lt $string.length; $i++)
	{
		$starter = $string.Substring($i,1)
		$rest = $string.Remove($i,1)
		
		
		if($starter -ne $string)
		{
			if($InvalidSubStringHash[$starter] -contains $rest)
			{
				if($debug)
				{
					Write-Host "$t $starter -> $rest"
				}
				continue
			}
			if($debug)
			{
				Write-Host "$t Taking out $starter"
			}
			$req = $FromOriginWithDoorCodes[$MapStr0.IndexOf($starter)]["keys"]
			
			if($req.count -gt 0)
			{
				$stringreq = "$req"
				if($rest -match "[$stringreq]")
				{
					if($debug)
					{
						Write-Host "$t found one of the following letters [$stringreq] in $rest" -foregroundcolor red
						Write-Host "$t $starter -> $rest"  -foregroundcolor red
					}
					$InvalidSubStringHash[$starter] += @($rest)
					continue
				}
			}
		}
		if($debug)
		{
			Write-Host "$t $starter -> $rest" -foregroundcolor green
		}
		if($rest.length -gt 1)
		{	if($StepsHash[$starter].Keys -notcontains $rest)
			{	 

				if($debug)
				{
					Write-Host "$t Cannot find in " ($StepsHash[$starter].Keys)  -foregroundcolor darkred
					Write-Host "$t finding combos for $rest" -foregroundcolor red
				}
				
				RecurseFindMinStepsBasedOnKeyCombo -string $rest -TabCount ($TabCount+1)
				
				$minsteps = $null
				$minKey = ""
				for($j = 0; $j -lt $rest.Length; $j++)
				{
					$PrevMin = $StepsHash[$rest.Substring($j,1)][$rest.Remove($j,1)]
					if($PrevMin -ne $null)
					{
						
						$steps = $StepsHash[$starter][$rest.Substring($j,1)] + $PrevMin
						
						if($debug)
						{
							Write-Host "$t Previous min "($rest.Substring($j,1))" -> "($rest.Remove($j,1))"was $PrevMin" 
							Write-Host "$t Steps $starter -> " ($rest.Substring($j,1)) ": "($StepsHash[$starter][$rest.Substring($j,1)])
						}
						if($minsteps -eq $null -or ($steps) -lt $minsteps)
						{
							$minSteps = ($steps)
							$minKey= ($rest.Substring($j,1))
						}
					}
				}
				Write-Host "$t Min $starter -> $rest : $minSteps" -foregroundcolor yellow
				Write-Host "$t Min $starter -> $minKey" -foregroundcolor cyan
				if($minsteps -ne $null)
				{
					if($debug)
					{
						Write-Host "$t setting Minsteps from $starter for $rest" -foregroundcolor yellow
					}
					$StepsHash[$starter][$rest] = $minsteps
				}
			}
		}
		$noneValid = $false
	}
	
	if($noneValid)
	{
		$InvalidStringArray += $string
	}
}

function WhoseKeyIsThisKey
{
	Param(
		[Parameter(Mandatory=$true)]
		[string] $key
		)

	
	foreach($robot in $RobotKeys.Keys)
	{
		if($RobotKeys[$robot] -contains $key)
		{
			return $robot
		}
	}
	
	return $null
}


function RecurseFindMinStepsWith4RobotsBasedOnKeyCombo
{
	Param(
		[Parameter(Mandatory=$true)]
		[string] $string,
		[Parameter(Mandatory=$true)]
		[int] $TabCount
		)
	
	$t = ("`t")*$TabCount
	if($string.length -eq 0)
	{
		return
	}	
	if($string.length -eq 1)
	{
		return
	}
	
	if($InvalidStringArray -contains $string)
	{
		return
	}
	
	Write-Host "$t Testing String $string"

	$Combos = @()
	$noneValid = $true
	for($i = 0; $i -lt $string.length; $i++)
	{
		$starter = $string.Substring($i,1)
		$starterRobot = WhoseKeyIsThisKey $starter
		
		$rest = $string.Remove($i,1)
		
		
		if($starter -ne $string)
		{
			if($InvalidSubStringHash[$starter] -contains $rest)
			{
				if($debug)
				{
					Write-Host "$t $starter -> $rest"
				}
				continue
			}
			if($debug)
			{
				Write-Host "$t Taking out $starter"
			}
			$req = $FromOriginWithDoorCodes[$MapStr0.IndexOf($starter)]["keys"]
			
			if($req.count -gt 0)
			{
				$stringreq = "$req"
				if($rest -match "[$stringreq]")
				{
					if($debug)
					{
						Write-Host "$t found one of the following letters [$stringreq] in $rest" -foregroundcolor red
						Write-Host "$t $starter -> $rest"  -foregroundcolor red
					}
					$InvalidSubStringHash[$starter] += @($rest)
					continue
				}
			}
		}
		if($debug)
		{
			Write-Host "$t $starter -> $rest" -foregroundcolor green
		}
		if($rest.length -gt 1)
		{	if($StepsHash[$starter].Keys -notcontains $rest)
			{	 

				if($debug)
				{
					Write-Host "$t Cannot find in " ($StepsHash[$starter].Keys)  -foregroundcolor darkred
					Write-Host "$t finding combos for $rest" -foregroundcolor red
				}
				
				RecurseFindMinStepsWith4RobotsBasedOnKeyCombo -string $rest -TabCount ($TabCount+1)
				
				$minsteps = $null
				$minKey = ""
				$minPos = @{"TL" = $null; "TR" = $null; "BL" = $null; "BR"=$null}
				for($j = 0; $j -lt $rest.Length; $j++)
				{
					$PrevMoveKey = $rest.Substring($j,1)
					$LastRobot = WhoseKeyIsThisKey $PrevMoveKey
					
					$LastMove = $StepsHash[$PrevMoveKey][$rest.Remove($j,1)]
					
					
					
					
					if($LastMove -ne $null -and $LastMove["steps"] -ne $null)
					{
						$PrevMin = $LastMove["steps"]					
						$ThisRobotLastIndex = $LastMove[$starterRobot]
						
						if($debug)
						{
							Write-Host "$t Previous min "($PrevMoveKey)" -> "($rest.Remove($j,1))"was $PrevMin" 	
							Write-Host "$t The robot moving $starter, $starterRobot was last at $ThisRobotLastIndex"
						}
						
						if($ThisRobotLastIndex -ne $null)
						{
							$steps = $StepsHash[$starter][$ThisRobotLastIndex]["steps"] + $PrevMin
							if($debug)
							{
								Write-Host "$t $StarterRobot Steps $starter -> " ($ThisRobotLastIndex) ": "($StepsHash[$starter][$ThisRobotLastIndex]["steps"]) -foregroundcolor darkgreen				
							}
						}
						else
						{
							$steps = $PrevMin
							if($debug)
							{							
								Write-Host "$t $StarterRobot did not have to move"
							}
						}
						
						if($debug)
						{
							Write-Host "$t Steps $starter -> " ($PrevMoveKey) ": $steps"
						}
						if($minsteps -eq $null -or ($steps) -lt $minsteps)
						{
							$minPos["TL"] = $LastMove["TL"]
							$minPos["TR"] = $LastMove["TR"]
							$minPos["BL"] = $LastMove["BL"]
							$minPos["BR"] = $LastMove["BR"]
							$minPos[$LastRobot] = $PrevMoveKey
							$minPos[$starterRobot] = $starter
							
							$minSteps = ($steps)
							$minKey= $PrevMoveKey
						}
					}
				}
				Write-Host "$t Min $starter -> $rest : $minSteps" -foregroundcolor yellow
				Write-Host "$t Min $starter -> $minKey" -foregroundcolor cyan

				if($debug)
				{
					Write-Host "$t setting Minsteps from $starter for $rest to $steps, Last Pos TR:"($minPos["TR"])" TL:"($minPos["TL"])" BR:"($minPos["BR"])" BL:"($minPos["BL"]) -foregroundcolor yellow
				}
				$StepsHash[$starter][$rest] = @{"steps" = $minsteps; "TL"= $minPos["TL"];"TR"=$minPos["TR"];"BL"=$minPos["BL"];"BR"=$minPos["BR"]}
			}
		}
		$noneValid = $false
	}
	
	if($noneValid)
	{
		$InvalidStringArray += $string
	}
}

function GetMinStepsFor4Robots
{
	$minsteps = $null
	$stringOfAllKeys = "$AllKeys"
	$MinKey = ""
	
	
	
	foreach($key in $AllKeys)
	{
		$FirstRobotToMove = WhoseKeyIsThisKey $key
		$BaseSteps = $StepsHash[$FirstRobotToMove][$key]
		
		<#Not Possible for first move#>
		if($BaseSteps -eq $null)
		{
			continue
		}
		if($debug)
		{
			Write-Host "`n$FirstRobotToMove origin -> $key : $BaseSteps" -foregroundcolor green
		}
		
		$rest = $stringOfAllKeys.Remove($stringOfAllKeys.IndexOf($key),1)
		$PreviousEntry = $StepsHash[$key][$rest]
		if($PreviousEntry -eq $null)
		{
			continue
		}		
		
		
		$otherRobotSteps = 0
		foreach($robot in  $RobotStartingPositions.Keys)
		{
			if($robot -eq $FirstRobotToMove)
			{
				continue
			}
		
			$otherRobotLastPos = $StepsHash[$key][$rest]["$robot"]
			
			$thisRobotSteps = $StepsHash[$robot][$otherRobotLastPos]
			if($debug)
			{
				Write-Host "$robot origin -> $otherRobotLastPos : $thisRobotSteps" -foregroundcolor cyan
			}
			$otherRobotSteps += $thisRobotSteps
			
		}
		

		
		$PreviousSteps =  $PreviousEntry["Steps"]
		$TotalSteps = $BaseSteps + $PreviousSteps + $otherRobotSteps
		
		if($debug)
		{
			Write-Host "Min $key -> $rest : $PreviousSteps"
			Write-Host "Total: $TotalSteps" -foregroundcolor red
		}
		
		if($PreviousEntry -ne $null -and ($minsteps -eq $null -or $TotalSteps -lt $minsteps))
		{
			$minSteps = $TotalSteps
			$MinKey = $key
		}
	}

	return $minsteps
}

function ValidateCombo
{
	Param(
		[Parameter(Mandatory=$true)]
		[string] $combination
		)
		
	for($i = 0; $i -lt $combination.length; $i++)
	{
		$req = $FromOriginWithDoorCodes[$MapStr0.IndexOf($combination.Substring($i,1))]["keys"]
		if($i -lt $req.count)
		{
			return $false
		}
		
		foreach($reqKey in $req)
		{
			if($combination.Substring(0,($i)) -notmatch $reqKey)
			{
				return $false
			}
		}
	}
	
	return $true
}

function DoPart1
{
	$StartIndex = $MapStr0.IndexOf("@")
	$MapStr = $MapStr0.Remove($StartIndex,1).Insert($StartIndex,".")
	$StartHeight = [math]::floor($StartIndex/$Width)
	$StartWidth = $StartIndex%$Width

	$IndexesVisited = @($StartIndex)
	Write-Host "HEIGHT: $Height WIDTH: $Width"
	Write-Host "Start Pos ("$StartHeight","$StartWidth")"

	$AvaiableSpacesWithSteps = New-Object system.collections.hashtable
	$AvaiableSpacesWithSteps += @{$StartIndex = @{"steps" =0;"keys" = @()}}

	$FromOriginWithDoorCodes = GetAvailableSpaces -MapString $MapStr -startingIdx $StartIndex -ExistingKeysWithSteps $AvaiableSpacesWithSteps  -PassDoors $True 
	$FromOrigin = GetAvailableSpaces -MapString $MapStr -startingIdx $StartIndex -ExistingKeysWithSteps $AvaiableSpacesWithSteps  -PassDoors $False 

	$EntryKeys = GetKeys $FromOrigin 

	foreach($key in $FromOriginWithDoorCodes.Keys)
	{
		<#if($key -is [int])
		{
			$StartHeight = [math]::floor($key/$Width)
			$StartWidth = $key%$Width
			Write-Host "Index ("$StartWidth","$StartHeight") is "($FromOriginWithDoorCodes[$key]["steps"])" steps away"
			$k = ($FromOriginWithDoorCodes[$key]["keys"])
			Write-Host "And Requires these keys" ("$k")
		}#>
		if($key -is [string])
		{
			Write-Host "`nKey $key is "($FromOriginWithDoorCodes[$key])" steps away from origin"
			$req = $FromOriginWithDoorCodes[$MapStr0.IndexOf($key)]["keys"]
			Write-Host "`tAnd requires these Keys: $req"
		}
	}
	pause

	$ofs = ""
	$StepsHash = @{}
	$StepsHash += @{"@" = (GetAvailableSpaces -MapString $MapStr -startingIdx $StartIndex -ExistingKeysWithSteps $AvaiableSpacesWithSteps  -PassDoors $false )}

	$TotalKeyCount = 1
	$AllKeys = ($MapStr | Select-String "[a-z]" -AllMatches -CaseSensitive).Matches.Value
	$AllKeys = $AllKeys | Sort-Object{$FromOriginWithDoorCodes[$MapStr0.IndexOf($_)]["keys"].Count} -Descending

	$InvalidSubStringHash = @{}
	$InvalidStringArray = @()
	foreach($key in $AllKeys)
	{
		$indexOfKey = $MapStr0.IndexOf($key)
		$MapStr1 = $MapStr.Remove($indexOfKey,1).Insert($indexOfKey,".")
		
		$indexOfDoor = $MapStr0.IndexOf($key.ToUpper)
		if($indexOfDoor -ne -1)
		{
			$MapStr1 = $MapStr1.Remove($indexOfDoor,1).Insert($indexOfDoor,".")
		}
		
		$AvaiableSpacesWithSteps = New-Object system.collections.hashtable
		$AvaiableSpacesWithSteps += @{$indexOfKey = @{"steps"=0; "keys" =@()}}
		
		
		Write-Host $key -foregroundcolor green
		$StepsHash += @{$key = (GetAvailableSpaces -MapString $MapStr1 -startingIdx $indexOfKey -ExistingKeysWithSteps $AvaiableSpacesWithSteps -PassDoors $true)}
		$InvalidSubStringHash = @{$key = @()}
		
		$req = $FromOriginWithDoorCodes[$MapStr0.IndexOf($key)]["keys"]
		foreach($reqKey in $req)
		{
			if($StepsHash[$key].Keys -contains $reqKey)
			{
				$StepsHash[$key][$reqKey] = $null
			}
		}
		
		$TotalKeyCount++
	}



	foreach($key in $StepsHash.Keys)
	{
		Write-Host "`nLooking at Key $key" -foregroundcolor green 
		if($FromOriginWithDoorCodes[$MapStr0.IndexOf($key)]["keys"].Count -gt 0)
		{
			$reqKeys = $FromOriginWithDoorCodes[$MapStr0.IndexOf($key)]["keys"]
			Write-Host "`t(Requires these keys first) $reqKeys" -foregroundcolor red
		}
		
		foreach($hashkey in $StepsHash[$key].Keys)
		{
			if($hashkey -is [string])
			{
				Write-Host "`tKey $hashkey is "($StepsHash[$key][$hashkey])" steps away"
			}
		}
	}

	pause

	$TempCombos = @{}
	RecurseFindMinStepsBasedOnKeyCombo -string "$AllKeys" -TabCount 1
	$steps = GetMinStepsFromOrigin

	Write-Host "`nGuess at min steps is" $steps
}


<#Parse Input#>
$file = Get-Content Advent18_input.txt
$Height = $file.Count
$Width = $file[0].Length
$debug = $false

$MapStr0 = ""
foreach($line in $file)
{
	$MapStr0 += $line
}


$StartIndex = $MapStr0.IndexOf("@")
$MapStr = $MapStr0.Remove($StartIndex,1).Insert($StartIndex,".")
$StartHeight = [math]::floor($StartIndex/$Width)
$StartWidth = $StartIndex%$Width

$IndexesVisited = @($StartIndex)
Write-Host "HEIGHT: $Height WIDTH: $Width"
Write-Host "Start Pos ("$StartHeight","$StartWidth")"

$AvaiableSpacesWithSteps = New-Object system.collections.hashtable
$AvaiableSpacesWithSteps += @{$StartIndex = @{"steps" =0;"keys" = @()}}

$FromOriginWithDoorCodes = GetAvailableSpaces -MapString $MapStr -startingIdx $StartIndex -ExistingKeysWithSteps $AvaiableSpacesWithSteps  -PassDoors $True 

<#function for Part1#>
<#DoPart1#>

<#Set up map for part 2#>
$MapStr = $MapStr.Remove($StartIndex+1,1).Insert($StartIndex+1,"#")
$MapStr = $MapStr.Remove($StartIndex - 1,1).Insert($StartIndex -1,"#")
$MapStr = $MapStr.Remove($StartIndex + $Width,1).Insert($StartIndex+$Width,"#")
$MapStr = $MapStr.Remove($StartIndex - $Width,1).Insert($StartIndex - $Width,"#")

$StartTopLeftRobotIndex = $StartIndex - 1 - $Width
$StartTopRightRobotIndex = $StartIndex + 1 - $Width
$StartBottomLeftRobotIndex = $StartIndex - 1 + $Width
$StartBottomRightRobotIndex = $StartIndex + 1 + $Width
$RobotStartingPositions = @{"TL" = $StartTopLeftRobotIndex; "TR" = $StartTopRightRobotIndex; "BL" = $StartBottomLeftRobotIndex; "BR" = $StartBottomRightRobotIndex }


$ofs = ""
$StepsHash = @{}
$RobotKeys = @{}

foreach($robotStart in $RobotStartingPositions.Keys)
{
	$AvaiableSpacesWithSteps = New-Object system.collections.hashtable
	$AvaiableSpacesWithSteps += @{($RobotStartingPositions[$robotStart]) = @{"steps" =0;"keys" = @()}}
	$StepsHash += @{$robotStart = (GetAvailableSpaces -MapString $MapStr -startingIdx $RobotStartingPositions[$robotStart] -ExistingKeysWithSteps $AvaiableSpacesWithSteps  -PassDoors $true )}
	
	
	$RobotKeys += @{$robotStart = GetKeys $StepsHash[$robotStart]}
}

$TotalKeyCount = 1
$AllKeys = ($MapStr | Select-String "[a-z]" -AllMatches -CaseSensitive).Matches.Value
$AllKeys = $AllKeys | Sort-Object{$FromOriginWithDoorCodes[$MapStr0.IndexOf($_)]["keys"].Count} -Descending

$InvalidSubStringHash = @{}
$InvalidStringArray = @()
foreach($key in $AllKeys)
{
	$indexOfKey = $MapStr0.IndexOf($key)
	$MapStr1 = $MapStr.Remove($indexOfKey,1).Insert($indexOfKey,".")
	
	$indexOfDoor = $MapStr0.IndexOf($key.ToUpper)
	if($indexOfDoor -ne -1)
	{
		$MapStr1 = $MapStr1.Remove($indexOfDoor,1).Insert($indexOfDoor,".")
	}
	
	$AvaiableSpacesWithSteps = New-Object system.collections.hashtable
	$AvaiableSpacesWithSteps += @{$indexOfKey = @{"steps"=0; "keys" =@()}}
	
	
	Write-Host $key -foregroundcolor green
	$StepsHash += @{$key = (GetAvailableSpaces -MapString $MapStr1 -startingIdx $indexOfKey -ExistingKeysWithSteps $AvaiableSpacesWithSteps -PassDoors $true)}
	$InvalidSubStringHash = @{$key = @()}
	
	$hashKeys = @($StepsHash[$key].Keys)
	$RobotForThisKey = WhoseKeyIsThisKey $key
	foreach($foundKey in $hashKeys)
	{
		if($foundKey -notmatch "^\w$")
		{
			continue
		}
	
		$steps = $StepsHash[$key][$foundKey]

		$StepsHash[$key].Remove($foundKey)
		
		$StepsHash[$key] += @{$foundKey = @{"steps" = $steps; "TL"=$null; "TR"=$null;  "BR"=$null;  "BL"=$null}}
		
		$StepsHash[$key][$foundKey][$RobotForThisKey] = $key
	}
	$req = $FromOriginWithDoorCodes[$MapStr0.IndexOf($key)]["keys"]
	foreach($reqKey in $req)
	{
		if($StepsHash[$key].Keys -contains $reqKey)
		{
			$StepsHash[$key][$reqKey] = @{"steps" = $null; "TL"=$null; "TR"=$null;  "BR"=$null;  "BL"=$null}
		}
		else
		{
			$StepsHash[$key] += @{$reqKey = @{"steps" = $null; "TL"=$null; "TR"=$null;  "BR"=$null;  "BL"=$null}}
		}
	}
	
	foreach($otherRobotKey in $AllKeys)
	{
		if((WhoseKeyIsThisKey $key) -ne (WhoseKeyIsThisKey $otherRobotKey) -and $StepsHash[$key].Keys -notcontains $otherRobotKey)
		{
			$StepsHash[$key] += @{$otherRobotKey = @{"steps" = 0; "TL"=$null; "TR"=$null;  "BR"=$null;  "BL"=$null}}
			$OtherRobotPos = WhoseKeyIsThisKey $otherRobotKey
			$StepsHash[$key][$otherRobotKey][$OtherRobotPos] = $otherRobotKey
			$StepsHash[$key][$otherRobotKey][$RobotForThisKey] = $key
		}
	}
	
	$TotalKeyCount++
}



foreach($key in $StepsHash.Keys)
{
	if($key.Length -gt 1)
	{
		$idx = $RobotStartingPositions[$key]
	}
	else
	{
		$idx = $MapStr0.IndexOf($key)
	}
	
	Write-Host "`nLooking at Key $key, Index $idx" -foregroundcolor green 
	if($FromOriginWithDoorCodes[$idx]["keys"].Count -gt 0)
	{
		$reqKeys = $FromOriginWithDoorCodes[$idx]["keys"]
		Write-Host "`t(Requires these keys first) $reqKeys" -foregroundcolor red
	}
	
	foreach($hashkey in $StepsHash[$key].Keys)
	{
		if($hashkey -is [string] -and $StepsHash[$key][$hashkey] -ne $null)
		{
			Write-Host "`tKey $hashkey is "($StepsHash[$key][$hashkey]["steps"])" steps away and is at TL:"($StepsHash[$key][$hashkey]["TL"])" TR:"($StepsHash[$key][$hashkey]["TR"])" BR:"($StepsHash[$key][$hashkey]["BR"])" BL:"($StepsHash[$key][$hashkey]["BL"])
		}
	}
}

pause
$debug = $false
RecurseFindMinStepsWith4RobotsBasedOnKeyCombo -string "$AllKeys" -TabCount 1
$debug = $true
$steps = GetMinStepsFor4Robots

Write-Host "`nGuess at min steps is" $steps -foregroundcolor yellow
