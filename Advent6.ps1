
function RecurseGetIndirectOrbits
{
    Param
    (
         [Parameter(Mandatory=$true, Position=0)]
         [int] $IndirectCount,
         [Parameter(Mandatory=$true, Position=1)]
         [string] $Orbiter
    )
	
	if($Orbits.keys -contains $Orbiter)
	{
		$IndirectCount += $Orbits[$Orbiter].Count
		$IndirectCount += RecurseGetIndirectOrbits -IndirectCount 0 -Orbiter $Orbits[$Orbiter]
	}
	
	return $IndirectCount

}

function GetListOfIndirectOrbits
{
    Param
    (
         [Parameter(Mandatory=$true, Position=0)]
         [string] $Orbiter
    )
	
	$OrbitsList = @()
	$tempOrbiter = $Orbiter
	
	While($Orbits.Keys -contains $tempOrbiter)
	{
		$tempOrbiter = $Orbits[$tempOrbiter]
		$OrbitsList += $tempOrbiter
	}
	
	return $OrbitsList
}


$Orbits = @{}

$file = Get-Content Advent6_input.txt
for ($i = 0; $i -lt $file.Count; $i++)
{
	$planets = $file[$i].Split(")")
	$Orbits[$planets[1]] += $planets[0]
}



<#Part 1#>
$DirectCount = $Orbits.Keys.Count
$IndirectCount = 0

foreach($key in @($Orbits.Keys))
{
	$IndirectCount += RecurseGetIndirectOrbits -IndirectCount 0 -Orbiter $Orbits[$key]
}

 
 
Write-host "DirectCount:" $DirectCount
Write-host "IndirectCount:" $IndirectCount
$total = $DirectCount + $IndirectCount

Write-host "Total:" $total
 
<#Part 2#>
$YouOrbits = GetListOfIndirectOrbits -Orbiter "YOU"
Write-host "Your Orbits" $YouOrbits

$SantaOrbits = GetListOfIndirectOrbits -Orbiter "SAN"

Write-host "Santa Orbits" $SantaOrbits
 
$minDistance = $null
for($i = 0; $i -lt  $YouOrbits.Count; $i++)
{
	for($j =0; $j -lt $SantaOrbits.Count; $j++)
	{
		if($YouOrbits[$i] -eq $SantaOrbits[$j])
		{
			
<# 			Write-host "Mass =" $YouOrbits[$i]
			Write-host "You Dist " $i
			Write-host "San Dist " $j #>
			
			$dist = $i + $j
			if($minDistance -eq $null -or $minDistance -gt $dist)
			{
				$minDistance = $dist
			}
		}
	}
}

Write-host "Min Dist:" $minDistance