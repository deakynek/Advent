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
		if($ExistingKeysWithSteps.Keys -contains $startingIdx -and $ExistingKeysWithSteps[$startingIdx] -ne $null)
		{
			$steps = $ExistingKeysWithSteps[$startingIdx]
		}
		else
		{
			$steps = 0
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
			
			
			if(-not ($ExistingKeysWithSteps.Keys -ccontains $NextIdx))
			{
				$ExistingKeysWithSteps += @{$NextIdx=$steps}
			}
			elseif($ExistingKeysWithSteps[$NextIdx] -eq $null)
			{
				$ExistingKeysWithSteps = $steps
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
		[system.collections.hashtable] $ExistingKeysWithSteps
		)
		
	$keys = @()
	foreach($key in $ExistingKeysWithSteps.Keys)
	{
		if($key -is [string])
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
		
	$keys = @()
	foreach($key in $ExistingKeysWithSteps.Keys)
	{
		if($key -is [int])
		{
			$keys += $key
		}
	}
	
	return $keys
}


function RecurseGetMinStepsToAllKeys
{
	Param(
		[Parameter(Mandatory=$true)]
		[string] $MapString,
		
		[Parameter(Mandatory=$true)]
		[int] $StartIndex,
		
		[Parameter(Mandatory=$true)]
		[system.collections.hashtable] $ExistingKeysWithSteps,
		
		[Parameter(Mandatory=$true)]
		[int] $TabCount
		
		)
		
	
	$Spaces = GetAvailableSpaces -MapString $MapString -startingIdx $StartIndex -ExistingKeysWithSteps $ExistingKeysWithSteps -PassDoors $false

	$KeysArray = GetKeys -ExistingKeysWithSteps $Spaces
	$minSteps = $null

	$tab ="`t"
	if($StepsHash[($MapStr0[$StartIndex])].Keys -contains ("$KeysArray"))
	{
		return $StepsHash[($MapStr0[$StartIndex])]["$KeysArray"]
	}
	
	
	
	Write-Host ($tab*$TabCount)""$KeysArray -foregroundcolor green
	if($KeysArray.Count -gt 0)
	{
		foreach($key in $KeysArray)
		{
			Write-Host "Moving From "($MapStr0[$StartIndex]) "to " $key
			Write-Host "Available is " $StepsHash[($MapStr0[$StartIndex])].Keys
			pause
			$baseSteps = $StepsHash[($MapStr0[$StartIndex])][$key]
			
			$indexOfKey = $MapString.IndexOf($key)
			$indexOfDoor = $MapString.IndexOf($key.ToUpper())
			
			if($debug)
			{
				Write-Host ($tab*$TabCount)"$baseSteps steps from " ($MapStr0[$StartIndex]) "to" $key
				Write-Host "key" $key
				Write-Host "Key Pos"
				PrintIndexInCoordinates -Index $indexOfKey
				Write-Host "Door Pos"
				PrintIndexInCoordinates -Index $indexOfDoor
			}
			
			
			$NewMapStr = $MapString.Remove($indexOfKey,1).Insert($indexOfKey,".")
			
			if($indexOfDoor -ne -1)
			{
				$NewMapStr = $NewMapStr.Remove($indexOfDoor,1).Insert($indexOfDoor,".")
			}
			
			$steps = RecurseGetMinStepsToAllKeys -MapString $NewMapStr -StartIndex $indexOfKey -TabCount ($TabCount+1) -ExistingKeysWithSteps $Spaces
			$TotalSteps = $baseSteps + $steps
			
			
			
			if($TotalSteps -ne $null -and ($minSteps -eq $null -or $TotalSteps -lt $minSteps))
			{
				$minSteps = $TotalSteps
			}
			
			if($debug)
			{
				Write-Host ($tab*$TabCount)"This Steps "$steps
				Write-Host ($tab*$TabCount)"Base Steps "$baseSteps
				Write-Host ($tab*$TabCount)"TotalSteps Steps "$TotalSteps
				Write-Host ($tab*$TabCount)"MinSteps " $minSteps
			}
		}
		
		if($minSteps -ne $null)
		{
			$StepsHash[$MapStr0[$StartIndex]] += @{"$KeysArray" = $minSteps}
		}
		return $minSteps
		
	}
	else
	{
		return 0
	}
}

function GetMinStepsFromOrigin
{
	

	$minsteps = $null
	foreach($key in (GetKeys -ExistingKeysWithSteps $FromOrigin))
	{
		$BaseSteps = $FromOrigin[$key]
		$indexOfKey = $MapStr.IndexOf($key)
		$indexOfDoor = $MapStr.IndexOf($key.ToUpper())
		
		$NewMapStr = $MapStr.Remove($indexOfKey,1).Insert($indexOfKey,".")
		
		if($indexOfDoor -ne -1)
		{
			$NewMapStr = $NewMapStr.Remove($indexOfDoor,1).Insert($indexOfDoor,".")
		}		
		
		Write-Host "Starting Recurse"
		$steps = RecurseGetMinStepsToAllKeys -MapString $NewMapStr -StartIndex $StartIndex -ExistingKeysWithSteps $FromOrigin -TabCount 0
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
$AvaiableSpacesWithSteps += @{$StartIndex = 0}

$IndexesVisitedFromStart = @($StartIndex)
$FromOrigin = GetAvailableSpaces -MapString $MapStr -startingIdx $StartIndex -ExistingKeysWithSteps $AvaiableSpacesWithSteps  -PassDoors $False 


Write-Host "Indexes from start = " $IndexesVisitedFromStart


Write-Host "Keys = " ($FromOrigin.Keys)
foreach($key in $FromOrigin.Keys)
{
	if($key -is [int])
	{
		$StartHeight = [math]::floor($key/$Width)
		$StartWidth = $key%$Width
		Write-Host "Index ("$StartWidth","$StartHeight") is "($FromOrigin[$key])" steps away"
	}
	elseif($key -is [string])
	{
		Write-Host "Key $key is "($FromOrigin[$key])" steps away"
	}
}
pause

$ofs = ""
$StepsHash = @{}

foreach($key in ($MapStr | Select-String "[a-z]" -AllMatches -CaseSensitive).Matches.Value)
{
	$indexOfKey = $MapStr0.IndexOf($key)	
	$AvaiableSpacesWithSteps = New-Object system.collections.hashtable
	$AvaiableSpacesWithSteps += @{$indexOfKey = 0}
	
	
	Write-Host $key -foregroundcolor green
	$debug = $true
	$StepsHash += @{$key = (GetAvailableSpaces -MapString $MapStr -startingIdx $indexOfKey -ExistingKeysWithSteps $AvaiableSpacesWithSteps -PassDoors $true)}
	pause
}

foreach($key in $StepsHash["a"].Keys)
{
	if($key -is [string])
	{
		Write-Host "Key $key is "($FromOrigin[$key])" steps away from key a"
	}
}

Write-Host "A"
Write-Host "Keys are"$StepsHash["a"].Keys
Write-Host "Values are"$StepsHash["a"].Values

GetKeys $StepsHash["a"]

pause


$steps = GetMinStepsFromOrigin
Write-Host "Guess at min steps is" $steps
