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

	$Facing = 0
	$steps = 0
	
	$IndexesVisited = @(GetIndexes $ExistingKeysWithSteps)
	
	$index = $startingIdx
	while(-not (TraveledToAllAvailableSpaces -MapString $MapString -IndexesVisited $IndexesVisited -refIndex ([ref]$index) -PassDoors $PassDoors))
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
						Write-Host "these keys needed at index $NextIdx :  $keys" 
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
			}
		}
		
		$Facing = ($Facing+1)%4
		<#$startingIdx = $NextIdx#>
		
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
		[ref] $refIndex,
		
		[Parameter(Mandatory=$true)]
		[bool] $PassDoors
		)
		
	foreach ($ind in $IndexesVisited)
	{
		$fourPos = @(($ind - $width), ($ind+1), ($ind + $width), ($ind - 1))
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
				
 				return $false
			}
		}
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
	foreach($key in (GetKeys -ExistingKeysWithSteps $FromOrigin))
	{
		$BaseSteps = $FromOrigin[$key]
		$indexOfKey = $MapStr.IndexOf($key)
		$indexOfDoor = $MapStr.IndexOf($key.ToUpper())
		$NewKeySteps = $FromOrigin
		$NewKeySteps.Remove($key) 
		
		$NewMapStr = $MapStr.Remove($indexOfKey,1).Insert($indexOfKey,".")
		
		if($indexOfDoor -ne -1)
		{
			$NewMapStr = $NewMapStr.Remove($indexOfDoor,1).Insert($indexOfDoor,".")
		}		
		
		Write-Host "Starting Recurse"
		$steps = RecurseGetMinStepsToAllKeys -MapString $NewMapStr -StartIndex $indexOfKey -ExistingKeysWithSteps $NewKeySteps -TabCount 0
		$TotalSteps = $BaseSteps + $steps
		
		if($minsteps -eq $null -or ($TotalSteps -lt $minSteps))
		{
			$minSteps = $TotalSteps
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
		[string] $string
		)
	
	
	$output = ("`t")*($AllKeys.Count - $string.length)
	Write-Host "$output" ($string.length)
	if($string.length -eq 0)
	{
		return @()
	}	
	if($string.length -eq 1)
	{
		return @($string)
	}

	$combos = @()
	for($i = 0; $i -lt $string.length; $i++)
	{
		$starter = $string.Substring($i,1)
		if($starter -ne $string -and $i -ne ($string.length -1))
		{
			$rest = $string.Remove($i,1)
			
			
			
			if($InvalidSubStringHash[$starter] -contains $rest)
			{
				continue
			}
			
			$req = $FromOriginWithDoorCodes[$MapStr0.IndexOf($starter)]["keys"]
			
			if($req.count -gt 0)
			{
				$stringreq = "$req"
				if($rest -match "[$stringreq]")
				{
					$InvalidSubStringHash[$starter] += @($rest)
					continue
				}
			}
		}
		
		
		$otherCharCombos = RecurseGetAllPossibleCombinations -string $string.Remove($i,1)
		
		
		foreach($combo in $otherCharCombos)
		{
			$combos += @($starter+$combo)
		}
	}
	
	return $combos

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


Write-Host "Indexes from start = " $IndexesVisitedFromStart


Write-Host "Keys = " ($FromOriginWithDoorCodes.Keys)
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

Write-Host "AllKeys : " "$AllKeys"
$allCombos = RecurseGetAllPossibleCombinations -string "$AllKeys"
Write-Host "Of "($allCombos.count)" Possible combinations"

$validCombos = @()

foreach($combo in $allCombos)
{
	if(ValidateCombo -combination $combo)
	{
		$validCombos += "@"+$combo
	}
}



Write-Host "Only" ($validCombos.count) "are valid"

$minSteps = $null
$MinCombos = @()
foreach($combo in $validCombos)
{
	$RunningTotal = 0
	$possible = $true
	for($i = 1; $i -lt $combo.length; $i++)
	{
		$RunningTotal += $StepsHash[$combo.Substring($i - 1,1)][$combo.Substring($i,1)]
		
		if($minSteps  -ne $null -and $RunningTotal -gt $minSteps)
		{
			$possible = $false
			break
		}
	}
	
	if(!$possible)
	{
		continue
	}
	
	if($minSteps  -eq $null -or $RunningTotal -lt $minSteps)
	{
		$MinCombos = @($combo)
		$minSteps = $RunningTotal
	}
	elseif ($RunningTotal -eq $minSteps)
	{
		$MinCombos += @($combo)
	}
}

Write-Host "Of the valid combinations, only "($MinCombos.count) "have the minimum steps of " $minSteps
Write-Host "They are:"
$MinCombos


<#
$visitedKeys = [System.collections.ArrayList]@("@")
$steps = RecurseGetMinStepsToAllKeys -Key "@" -VisitedKeys $visitedKeys -TabCount 0#>
Write-Host "Guess at min steps is" $minSteps
