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
			
			Write-Host "`nIndex $NextIdx" -foregroundcolor cyan
			Write-Host "Testing ("$StartWidth","$StartHeight")"
			Write-Host "Char Result is: "$NextCharResult
		}
		
		
		
		if($NextCharResult -eq 1 -or $NextCharResult -eq 2)
		{
			$steps++
			
			if($IndexesVisited -notcontains $NextIdx)
			{
				$IndexesVisited += $NextIdx
			}
				
			if(-not ($ExistingKeysWithSteps.Keys -contains $NextIdx))
			{
				$ExistingKeysWithSteps += @{$NextIdx = @{"steps" = $steps}}
			}
			elseif($ExistingKeysWithSteps[$NextIdx]["steps"] -eq $null)
			{
				$ExistingKeysWithSteps[$NextIdx]["steps"] = $steps
			}
			
			if($NextCharResult -eq 2)
			{
				$PortalIdx = $PortalIndexes[$NextIdx]["index"]
				
				if($IndexesVisited -notcontains $PortalIdx)
				{
					$IndexesVisited += $PortalIdx
				}			
			
				if(-not ($ExistingKeysWithSteps.Keys -ccontains $PortalIdx))
				{
					$ExistingKeysWithSteps += @{$PortalIdx = @{"steps" = ($steps+1)}}
				}
				elseif($ExistingKeysWithSteps[$PortalIdx]["steps"] -eq $null)
				{
					$ExistingKeysWithSteps[$PortalIdx]["steps"] = ($steps+1)
				}
			}
		}		
		elseif($NextCharResult -eq 5)
		{
			$steps++
			$ExistingKeysWithSteps += @{"END" = @{"steps" = $steps}}
			return $ExistingKeysWithSteps
		}
	}
	return $ExistingKeysWithSteps
}

function GoEverywhereYouCanOnALevel
{
	Param(
		[Parameter(Mandatory=$true)]
		[string] $MapString,
		
		[Parameter(Mandatory=$true)]
		[int] $startingIdx,
		
		[Parameter(Mandatory=$true)]
		[system.collections.hashtable] $ExistingKeysWithSteps,
		
		[Parameter(Mandatory=$true)]
		[bool] $PassDoors,
		
		[Parameter(Mandatory=$true)]
		[int] $CurrentLevel,
		
		[Parameter(Mandatory=$true)]
		[ref] $GlobalLevelCopy,
		
		[Parameter(Mandatory=$true)]
		[ref] $CompletedIndex,
		
		[Parameter(Mandatory=$true)]
		[ref] $GoingDown
		)

	$steps = 0
	
	$IndexesVisited = @(GetIndexes $ExistingKeysWithSteps)
	
	$Facing = 0
	$index = $startingIdx
	
	
	while(-not (TraveledToAllAvailableSpaces -MapString $MapString -IndexesVisited $IndexesVisited -refCompletedIndexes $CompletedIndex	-refIndex ([ref]$index) -Facing ([ref]$Facing) -PassDoors $PassDoors))
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
			
			Write-Host "`nIndex $NextIdx" -foregroundcolor cyan
			Write-Host "Testing ("$StartWidth","$StartHeight")"
			Write-Host "Char Result is: "$NextCharResult
		}
		
		
		if($NextCharResult -eq 5 -and $CurrentLevel -ne 0)
		{
			Write-Host "Found ZZ on level $CurrentLevel, should keep going"
			$NextCharResult = 1
		}
		
		
		if($NextCharResult -eq 1 -or $NextCharResult -eq 2)
		{
			$steps++
			
			if($IndexesVisited -notcontains $NextIdx)
			{
				$IndexesVisited += $NextIdx
			}
				
			if(-not ($ExistingKeysWithSteps.Keys -contains $NextIdx))
			{
				$ExistingKeysWithSteps += @{$NextIdx = @{"steps" = $steps}}
			}
			elseif($ExistingKeysWithSteps[$NextIdx]["steps"] -eq $null)
			{
				$ExistingKeysWithSteps[$NextIdx]["steps"] = $steps
			}
			
			if($NextCharResult -eq 2)
			{	
				$PortalIndex = $PortalIndexes[$NextIdx]["index"]
				$NextLevel = $CurrentLevel + ($PortalIndexes[$NextIdx]["Effect"])
				$PortalName = $PortalIndexes[$NextIdx]["Name"]
				
				if(($PortalIndexes[$NextIdx]["Effect"]) -lt 0)
				{
					$GoingDown.Value = $true
				}
				
				
				if($NextLevel -ge 0)
				{
					Write-Host "Going through $PortalName on level $CurrentLevel, going to level $NextLevel"
					Write-Host "Effect is " ($PortalIndexes[$NextIdx]["Effect"])
					
					if($GlobalLevelCopy.Value.Keys -notcontains $NextLevel)
					{
						$NewLevel= New-Object system.collections.hashtable
						$NewLevel += @{$PortalIndex = @{"steps" =($steps+1);"keys" = @()}}
						$NewLevel += @{"completedIndexes" = @()}
						
						
						$GlobalLevelCopy.Value = $GlobalLevelCopy.Value + @{$NextLevel = $NewLevel}
						
						if($debug)
						{
							Write-Host "`nTest Count After"  ($GlobalLevelCopy.Value.Keys.Count)
							Write-Host "Keys are " ($GlobalLevelCopy.Value.Keys)
						}
					}
					elseif($GlobalLevelCopy.Value[$NextLevel].Keys -notcontains $PortalIndex)
					{
						$GlobalLevelCopy.Value[$NextLevel] += @{$PortalIndex = @{"steps" = ($steps+1)}}
						if($debug)
						{					
							Write-Host "`n2) Test Count After"  ($GlobalLevelCopy.Value.Keys.Count)
							Write-Host "Keys are " ($GlobalLevelCopy.Value.Keys)
						}
					}
					elseif($GlobalLevelCopy.Value[$NextLevel][$PortalIndex]["steps"] -eq $null)
					{
						$GlobalLevelCopy.Value[$NextLevel][$PortalIndex]["steps"] = ($steps+1)
					}
				}
			}
		}		
		elseif($NextCharResult -eq 5)
		{
			$steps++
			$ExistingKeysWithSteps += @{"END" = @{"steps" = $steps}}
			return $ExistingKeysWithSteps
		}
	}
	
	if($debug)
	{
		Write-Host "`3) Leaving Count After"  ($GlobalLevelCopy.Value.Keys.Count)
		Write-Host "Keys are " ($GlobalLevelCopy.Value.Keys)
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
	
	if($Idx -eq ([System.Collections.ArrayList]($Portals["ZZ"].Keys))[0])
	{
		return 5
	}
	elseif($NextChar -eq "#")
	{
		return 0 
	}
	elseif($NextChar -eq ".")
	{
		return 1
	}
	elseif($NextChar -eq "@")
	{
		return 2
	}
	else
	{
		return 99
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

function PrintMap
{
	Param(
		[Parameter(Mandatory=$true)]
		[string] $Map
		)
		
	$lines = [math]::floor($map.length / $width)
	
	Write-Host "Lines $lines"
	for($i = 1; $i -lt $lines.count; $i++)
	{
		$Map.Insert($i*($width)+($i-1), "`r`n")
	}
	
	$Map
		
}

<#Parse Input#>
$file = Get-Content Advent20_input.txt
$Height = $file.Count
$Width = $file[0].Length
$debug = $false

$MapStr0 = ""
foreach($line in $file)
{
	$MapStr0 += $line
}


$Portals = @{}
$PortalIndexes = @{}
for($i = 0; $i -lt $MapStr0.length; $i++)
{
	if($MapStr0.Substring($i,1) -notmatch "\w")
	{
		continue
	}
	
	$Name = $null
	$Facing = $null
	$index = $null
	
	if(($MapStr0.Substring($i,3) -match "\w\w\."))
	{
		$Name = $MapStr0.Substring($i,2)
		$Facing = 3
		$index = ($i+2)
	}
	elseif($MapStr0.Substring(($i-1),3) -match "\.\w\w")
	{
		$Name = $MapStr0.Substring($i,2)
		$Facing = 1
		$index = ($i-1)
	}
	elseif(($i + $width) -lt $MapStr0.length -and $MapStr0.Substring(($i + $width),1) -match "\w")
	{
		if($i -lt $width -or (($i + 2*($width)) -lt $MapStr0.length -and $MapStr0.Substring(($i + 2*($width)),1) -match "\."))
		{
			$Name = $MapStr0.Substring($i,1) + $MapStr0.Substring(($i + $width),1)
			$Facing = 0
			$index = $i + 2*($width)
		}
		elseif(($MapStr0.Substring(($i - $width)),1) -match "\.")
		{
			$Name = $MapStr0.Substring($i,1) + $MapStr0.Substring(($i + $width),1)
			$Facing = 2
			$index = $i - ($width)
		}
	}
	
	$IndexHeight = [math]::floor($Index/$Width)
	$IndexWidth = $Index%$Width
	
	$LevelEffect = 1
	if($IndexHeight -eq 2 -or $IndexHeight -eq ($Height - 3) -or $IndexWidth -eq 2 -or $IndexWidth -eq ($Width - 3))
	{
		$LevelEffect = -1
	}
	
	if($Name -ne $null)
	{
		if($Portals.Keys -contains $Name)
		{
			$OldIndex = ([system.collections.arraylist]($Portals[$Name].keys))[0]
		
			$PortalIndexes += @{$OldIndex = @{"index" = $index; "Effect"= (-1 * $LevelEffect); "name" = $Name}}
			$PortalIndexes += @{$index  = @{"index" = $OldIndex; "Effect"= $LevelEffect; "name" = $Name}}
			$Portals[$Name] += @{$index = $Facing}
		}
		else
		{
			$Portals += @{$Name = @{$index = $Facing}}
		}
	}
}

foreach($port in $Portals.Keys)
{
	Write-Host "`nName of Portal is $port" -foregroundcolor green
	foreach($ind in $Portals[$port].keys)
	{
		$Facing = $Portals[$port][$ind]
		
		if($Facing -eq 0)
		{
			$dir = "North"
		}
		elseif($Facing -eq 1)
		{
			$dir = "East"
		}
		elseif($Facing -eq 2)
		{
			$dir = "South"
		}
		elseif($Facing -eq 3)
		{
			$dir = "West"
		}
		
		Write-Host "Facing $dir at Index: $ind position:"
		PrintIndexInCoordinates -index $ind
	}
	
}

foreach($ind in $PortalIndexes.Keys)
{
	Write-Host "`nBecause of Portals, index $ind is 1 step from "($PortalIndexes[$ind]["index"])
	Write-Host "Moving from"
	PrintIndexInCoordinates -index $ind
	
	Write-Host "to"
	PrintIndexInCoordinates -index ($PortalIndexes[$ind]["index"])
	
	Write-Host "Adds " ($PortalIndexes[$ind]["Effect"]) "to the level"
	
	
	$MapStr0 = $MapStr0.Remove($ind,1).Insert($ind,"@")
	
}
pause

$mapwithoutLetters = $MapStr0 -replace "\w","#"




$FirstIndex = $null
foreach($ind in $Portals["AA"].Keys)
{
	$FirstIndex = $ind
	$break
}


Write-Host "Starting Index = $FirstIndex"


$x = $Portals.Remove("AA")

<#
$AvaiableSpacesWithSteps = New-Object system.collections.hashtable
$AvaiableSpacesWithSteps += @{$FirstIndex = @{"steps" =0;"keys" = @()}}
$debug = $false
$AtoZ = GetAvailableSpaces -MapString $mapwithoutLetters -startingIdx $FirstIndex -ExistingKeysWithSteps $AvaiableSpacesWithSteps  -PassDoors $True

Write-Host "Steps Guess "  ($AtoZ["END"]["steps"])
#>

$Level0 = New-Object system.collections.hashtable
$Level0 += @{$FirstIndex = @{"steps" =0;"keys" = @();}}
$Level0 += @{"completedIndexes" = @()}
$GlobalLevels = @{0 = $Level0}

do
{
	$Levels = $GlobalLevels.Keys.Count
	Write-Host "`nCurrent Level count" $Levels -foregroundcolor green
	Write-Host "Levels are " ($GlobalLevels.Keys)
	
	for($i =0; $i -lt $Levels; $i++)
	{
		Write-Host "Level" $i -foregroundcolor cyan
		$GoingDown = $false
	
		$temp = GoEverywhereYouCanOnALevel -MapString $mapwithoutLetters -startingIdx 0 -ExistingKeysWithSteps $GlobalLevels[$i]  -PassDoors $True -CurrentLevel $i -GlobalLevelCopy ([ref] $GlobalLevels) -CompletedIndex ([ref] $GlobalLevels[$i]["completedIndexes"]) -GoingDown ([ref]$GoingDown)
		
		$debug = $false
		
		if($GlobalLevels.Keys -contains $i)
		{
			$GlobalLevels[$i] = $temp
			
			if($GoingDown)
			{
				for($j =($i - 1); $j -ge 0; $j--)
				{
					$GoingDown = $false
					Write-Host "`t Level" $j -foregroundcolor yellow
					$temp2 = GoEverywhereYouCanOnALevel -MapString $mapwithoutLetters -startingIdx 0 -ExistingKeysWithSteps $GlobalLevels[$j]  -PassDoors $True -CurrentLevel $j -GlobalLevelCopy ([ref] $GlobalLevels) -CompletedIndex ([ref] $GlobalLevels[$j]["completedIndexes"]) -GoingDown ([ref]$GoingDown)
					
					$GlobalLevels[$j] = $temp2
					
					if($GoingDown -ne $true)
					{
						break
					}
				}
			}
		}
		
		if($GlobalLevels[0].Keys -contains "END")
		{
			break
		}
		
	}

}
while($GlobalLevels[0].Keys -notcontains "END")

Write-Host "Steps Guess "  ($GlobalLevels[0]["END"]["steps"])


