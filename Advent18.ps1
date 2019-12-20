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
		[system.collections.hashtable] $ExistingKeysWithSteps,
		
		[Parameter(Mandatory=$true)]
		[System.Collections.ArrayList] $KeysVisited
		)
		
	$keys = [System.Collections.ArrayList]@()
	foreach($key in $ExistingKeysWithSteps.Keys)
	{
		if($key -is [string] -and $key -match "^\w$" -and $KeysVisited -notcontains $key)
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

	$NextKeyArray = [System.Collections.ArrayList] @(GetKeys -ExistingKeysWithSteps ($StepsHash[$key]) -KeysVisited $visitedKeys)
	
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
	$visited = @(0)
	
	foreach($key in (GetKeys -ExistingKeysWithSteps $FromOrigin -KeysVisited $visited))
	{
		$BaseSteps = $FromOrigin[$key]
		Write-Host "$baseSteps from origin to $key"
		
		$ExtraSteps = $StepsHash[$key][$stringOfAllKeys.Remove($stringOfAllKeys.IndexOf($key),1)]
		Write-Host "$ExtraSteps from $key to last Key"
		
		if($ExtraSteps -ne $null -and ($minsteps -eq $null -or ($BaseSteps +$ExtraSteps) -lt $minsteps))
		{
			$minSteps = ($BaseSteps +$ExtraSteps)
		}
	}

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

function RecurseGetAllPossibleCombinations
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
		return @()
	}	
	if($string.length -eq 1)
	{
		return @($string)
	}
	
	Write-Host "$t Testing String $string"

	$Combos = @()
	for($i = 0; $i -lt $string.length; $i++)
	{
		$starter = $string.Substring($i,1)
		$rest = $string.Remove($i,1)
		if($starter -ne $string -and $i -ne ($string.length -1))
		{
			if($InvalidSubStringHash[$starter] -contains $rest)
			{
				Write-Host "$t skipping $rest"
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
						Write-Host "$t found one of the following letters [$stringreq] in $rest"
					}
					$InvalidSubStringHash[$starter] += @($rest)
					Write-Host "$t skipping $rest"
					continue
				}
			}
		}
		
		if($rest.length -gt 1)
		{	if($StepsHash[$starter].Keys -notcontains $rest)
			{	 

				if($debug)
				{
					Write-Host "$t Cannot find in " ($StepsHash[$starter].Keys)  -foregroundcolor darkred
					Write-Host "$t finding combos for $rest" -foregroundcolor red
				}
				
				RecurseGetAllPossibleCombinations -string $rest -TabCount ($TabCount+1)
				
				$minsteps = $null
				for($j = 0; $j -lt $rest.Length; $j++)
				{
					$PrevMin = $StepsHash[$rest.Substring($j,1)][$rest.Remove($j,1)]
					if($PrevMin -ne $null)
					{
						Write-Host "Previous minimum from "($rest.Substring($j,1))" with these keys remaining "($rest.Remove($j,1))"was $PrevMin" 
						$steps = $StepsHash[$starter][$rest.Substring($j,1)] + $PrevMin
						Write-Host "Steps from $starter to " ($rest.Substring($j,1)) "is "($StepsHash[$starter][$rest.Substring($j,1)])
						if($minsteps -eq $null -or ($steps) -lt $minsteps)
						{
							$minSteps = ($steps)
						}
					}
				}
				Write-Host "Minimum from $starter to $rest is $minSteps steps"
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
	}
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

$IndexesVisitedFromStart = @($StartIndex)
$FromOriginWithDoorCodes = GetAvailableSpaces -MapString $MapStr -startingIdx $StartIndex -ExistingKeysWithSteps $AvaiableSpacesWithSteps  -PassDoors $True 
$FromOrigin = GetAvailableSpaces -MapString $MapStr -startingIdx $StartIndex -ExistingKeysWithSteps $AvaiableSpacesWithSteps  -PassDoors $False 

$VisitedKeys = @(0)
$EntryKeys = GetKeys $FromOrigin $VisitedKeys


Write-Host "Indexes from start = " $IndexesVisitedFromStart

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
RecurseGetAllPossibleCombinations -string "$AllKeys" -TabCount 0
$steps = GetMinStepsFromOrigin

Write-Host "Guess at min steps is" $steps
