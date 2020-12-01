<#Find Greatest Common Denominator#>
function getGcd2
{
	Param(
		[Parameter(Mandatory=$true)]
		[long] $a,
		
		[Parameter(Mandatory=$true)]
		[long] $b
		)

	if($b -eq 0)
	{
		return $a
	}
	return getGcd2 -a $b  -b ($a%$b)
}

<#Find Greatest Common Denominator of Numbers in a list#>
function gcdFromArray
{
	Param(
		[Parameter(Mandatory=$true)]
		[System.Collections.ArrayList] $array
		)

	$n = 0;
	for( $i=0; $i -lt $array.count; $i++)
	{
		$n = getGcd2 -a $array[$i] -b $n
	}
  return $n;
}

<#Find Least Common Multiple#>
function lcm2
{
	Param(
		[Parameter(Mandatory=$true)]
		[long] $a,
		
		[Parameter(Mandatory=$true)]
		[long] $b
		)
  return $a*$b / (getGcd2 -a $a -b $b)
}

<#Find Least Common Multiple of Numbers in a list#>
function lcmFromArray
{
	Param(
		[Parameter(Mandatory=$true)]
		[System.Collections.ArrayList] $array
		)
		
	$n = 1;
	for($i=0; $i -lt $array.count; $i++)
	{
		$n = lcm2 -a ($array[$i]) -b $n
	}
	return $n;
}


<#Parse Input#>
$file = Get-Content Advent12_input.txt

$Planet = @{"Position"= @(0,0,0); "Velocity" = @(0,0,0)}
$Planets = @(@{"Position"= @(0,0,0); "Velocity" = @(0,0,0)}, `
			 @{"Position"= @(0,0,0); "Velocity" = @(0,0,0)}, `
			 @{"Position"= @(0,0,0); "Velocity" = @(0,0,0)}, `
			 @{"Position"= @(0,0,0); "Velocity" = @(0,0,0)}	)

			 
$InitialState = @(@{"Position"= @(0,0,0); "Velocity" = @(0,0,0)}, `
				@{"Position"= @(0,0,0); "Velocity" = @(0,0,0)}, `
				@{"Position"= @(0,0,0); "Velocity" = @(0,0,0)}, `
				@{"Position"= @(0,0,0); "Velocity" = @(0,0,0)})			 
			 
for($i= 0; $i -lt $file.Count; $i++)
{
	$withoutBraces = $file[$i].Substring(1,$file[$i].Length-2)
	$Parse = $withoutBraces.Split(",=")

	$Planets[$i]["Position"][0] = [int]($Parse[1])
	$Planets[$i]["Position"][1] = [int]($Parse[3])
	$Planets[$i]["Position"][2] = [int]($Parse[5])
	$InitialState[$i]["Position"][0] = [int]($Parse[1])
	$InitialState[$i]["Position"][1] = [int]($Parse[3])
	$InitialState[$i]["Position"][2] = [int]($Parse[5])
}

<#Part 1#>
Write-Host "Part 1" -foregroundcolor red
$steps = 1
while($steps -le 1000)
{
	<#Change vel#>
	for($i = 0; $i -lt $Planets.Count; $i++)
	{
		for($j = 0; $j -lt $Planets.Count; $j++)
		{
			if($i -eq $j)
			{
				continue
			}
		
			$PlanetPos1 = $Planets[$i]["Position"]
			$PlanetPos2 = $Planets[$j]["Position"]
			
			if($PlanetPos1[0] -lt $PlanetPos2[0])
			{
				$Planets[$i]["Velocity"][0] += 1
			}
			elseif ($PlanetPos1[0] -gt $PlanetPos2[0])
			{
				$Planets[$i]["Velocity"][0] -= 1
			}
			
			if($PlanetPos1[1] -lt $PlanetPos2[1])
			{
				$Planets[$i]["Velocity"][1] += 1
			}
			elseif ($PlanetPos1[1] -gt $PlanetPos2[1])
			{
				$Planets[$i]["Velocity"][1] -= 1
			}	

			if($PlanetPos1[2] -lt $PlanetPos2[2])
			{
				$Planets[$i]["Velocity"][2] += 1
			}
			elseif ($PlanetPos1[2] -gt $PlanetPos2[2])
			{
				$Planets[$i]["Velocity"][2] -= 1
			}
		}
	}

	<#Change Pos#>
	for($i = 0; $i -lt $Planets.Count; $i++)
	{
		$PlanetVel = $Planets[$i]["Velocity"]
		
		$Planets[$i]["Position"][0] += $PlanetVel[0]
		$Planets[$i]["Position"][1] += $PlanetVel[1]
		$Planets[$i]["Position"][2] += $PlanetVel[2]
	}
	
	$steps++
}

<#Calc Energy Part 1#>
$Energy = 0
for($i = 0; $i -lt $Planets.Count; $i++)
{
	$PlanetVel = $Planets[$i]["Velocity"]
	$PlanetPos = $Planets[$i]["Position"]
	
	$KE = 0
	$KE += [math]::abs($PlanetVel[0])
	$KE += [math]::abs($PlanetVel[1])
	$KE += [math]::abs($PlanetVel[2])
	
	$PE = 0
	$PE += [math]::abs($PlanetPos[0])
	$PE += [math]::abs($PlanetPos[1])
	$PE += [math]::abs($PlanetPos[2])
	
	$Energy += $KE *$PE
}

Write-Host "Total Energy $Energy after 1000 steps`n`n" -foregroundcolor green

<#Reset#>
$Planets = @(@{"Position"= @(0,0,0); "Velocity" = @(0,0,0)}, `
			 @{"Position"= @(0,0,0); "Velocity" = @(0,0,0)}, `
			 @{"Position"= @(0,0,0); "Velocity" = @(0,0,0)}, `
			 @{"Position"= @(0,0,0); "Velocity" = @(0,0,0)}	)

for($i= 0; $i -lt $file.Count; $i++)
{
	$withoutBraces = $file[$i].Substring(1,$file[$i].Length-2)
	$Parse = $withoutBraces.Split(",=")

	$Planets[$i]["Position"][0] = [int]($Parse[1])
	$Planets[$i]["Position"][1] = [int]($Parse[3])
	$Planets[$i]["Position"][2] = [int]($Parse[5])
	
	<#Write-Host "`nPlanet $i : "
	Write-Host "Position: "($Planets[$i]["Position"][0])","($Planets[$i]["Position"][1])","($Planets[$i]["Position"][2])")"
	Write-Host "Velocity: "($Planets[$i]["Velocity"][0])","($Planets[$i]["Velocity"][1])","($Planets[$i]["Velocity"][2])")"	#>
}


<#Part 2#>
$steps = 1
$match = $false
$xmatch = $false
$ymatch = $false
$zmatch = $false
$xsteps = 0
$ysteps = 0
$zsteps = 0

Write-Host "Part 2" -foregroundcolor red
while(!($xmatch  -and $ymatch -and $zmatch))
{
	if($steps % 10000 -eq 0)
	{
		Write-Host "Steps" $steps
	}

	<#Change vel#>
	for($i = 0; $i -lt $Planets.Count; $i++)
	{
		for($j = 0; $j -lt $Planets.Count; $j++)
		{
			if($i -eq $j)
			{
				continue
			}
		
			$PlanetPos1 = $Planets[$i]["Position"]
			$PlanetPos2 = $Planets[$j]["Position"]
			
			if($PlanetPos1[0] -lt $PlanetPos2[0])
			{
				$Planets[$i]["Velocity"][0] += 1
			}
			elseif ($PlanetPos1[0] -gt $PlanetPos2[0])
			{
				$Planets[$i]["Velocity"][0] -= 1
			}
			
			if($PlanetPos1[1] -lt $PlanetPos2[1])
			{
				$Planets[$i]["Velocity"][1] += 1
			}
			elseif ($PlanetPos1[1] -gt $PlanetPos2[1])
			{
				$Planets[$i]["Velocity"][1] -= 1
			}	

			if($PlanetPos1[2] -lt $PlanetPos2[2])
			{
				$Planets[$i]["Velocity"][2] += 1
			}
			elseif ($PlanetPos1[2] -gt $PlanetPos2[2])
			{
				$Planets[$i]["Velocity"][2] -= 1
			}
		}
	}

	<#Change Pos#>
	for($i = 0; $i -lt $Planets.Count; $i++)
	{
		$PlanetVel = $Planets[$i]["Velocity"]
		
		$Planets[$i]["Position"][0] += $PlanetVel[0]
		$Planets[$i]["Position"][1] += $PlanetVel[1]
		$Planets[$i]["Position"][2] += $PlanetVel[2]
	}
	
	$thisXMatch = $true
	$thisYMatch = $true
	$thisZMatch = $true
	
	<#This system is deterministic, so for it to be cyclical, eventually#>
	<#it will return to the starting point.  But, x,y,and z are independent#>
	<#so the number of steps needed to repeat in each dimension are also independent#>
	for($i= 0; $i -lt $file.Count; $i++)
	{
		if($InitialState[$i]["Velocity"][0] -ne $Planets[$i]["Velocity"][0] -or 
			$InitialState[$i]["Position"][0] -ne $Planets[$i]["Position"][0])
		{
			$thisXMatch = $false
		}
		
		if($InitialState[$i]["Velocity"][1] -ne $Planets[$i]["Velocity"][1] -or `
			$InitialState[$i]["Position"][1] -ne $Planets[$i]["Position"][1])
		{
			$thisYMatch = $false
		}
		
		if($InitialState[$i]["Velocity"][2] -ne $Planets[$i]["Velocity"][2] -or `
			$InitialState[$i]["Position"][2] -ne $Planets[$i]["Position"][2])
		{
			$thisZMatch = $false
		}		
	}
	
	if(!$xmatch -and $thisXMatch)
	{
		Write-Host "X Match at " $steps -foregroundcolor blue
		$xsteps = $steps
		
		Write-Host "State at X Match" -foregroundcolor darkgreen
		for($i= 0; $i -lt $file.Count; $i++)
		{
			Write-Host "Planet $i : " -foregroundcolor darkgreen
			Write-Host "Position: ("($Planets[$i]["Position"][0])","($Planets[$i]["Position"][1])","($Planets[$i]["Position"][2])")" -foregroundcolor darkgreen
			Write-Host "Velocity: ("($Planets[$i]["Velocity"][0])","($Planets[$i]["Velocity"][1])","($Planets[$i]["Velocity"][2])")"	-foregroundcolor darkgreen
			Write-Host ""
		}
	}
	if(!$ymatch -and $thisYMatch)
	{
		Write-Host "Y Match at " $steps -foregroundcolor blue
		$ysteps = $steps
		
		Write-Host "State at Y Match" -foregroundcolor darkgreen
		for($i= 0; $i -lt $file.Count; $i++)
		{
			Write-Host "Planet $i : " -foregroundcolor darkgreen
			Write-Host "Position: ("($Planets[$i]["Position"][0])","($Planets[$i]["Position"][1])","($Planets[$i]["Position"][2])")" -foregroundcolor darkgreen
			Write-Host "Velocity: ("($Planets[$i]["Velocity"][0])","($Planets[$i]["Velocity"][1])","($Planets[$i]["Velocity"][2])")"	-foregroundcolor darkgreen
			Write-Host ""
		}		
		
	}
	if(!$zmatch -and $thisZMatch)
	{
		Write-Host "Z Match at " $steps -foregroundcolor blue
		$zsteps = $steps
		
		Write-Host "State at Z Match" -foregroundcolor darkgreen
		for($i= 0; $i -lt $file.Count; $i++)
		{
			Write-Host "Planet $i : " -foregroundcolor darkgreen
			Write-Host "Position: ("($Planets[$i]["Position"][0])","($Planets[$i]["Position"][1])","($Planets[$i]["Position"][2])")" -foregroundcolor darkgreen
			Write-Host "Velocity: ("($Planets[$i]["Velocity"][0])","($Planets[$i]["Velocity"][1])","($Planets[$i]["Velocity"][2])")"	-foregroundcolor darkgreen
			Write-Host ""
		}			
	}
	
	
	
	$xmatch = $xmatch -or $thisXMatch
	$ymatch = $ymatch -or $thisYMatch
	$zmatch = $zmatch -or $thisZMatch
	$steps++
}

Write-Host "XSteps: $xsteps steps" 
Write-Host "YSteps: $ysteps steps" 
Write-Host "ZSteps: $zsteps steps" 


for($i= 0; $i -lt $file.Count; $i++)
{
	Write-Host "`nPlanet $i : "
	Write-Host "Position: "($Planets[$i]["Position"][0])","($Planets[$i]["Position"][1])","($Planets[$i]["Position"][2])")"
	Write-Host "Velocity: "($Planets[$i]["Velocity"][0])","($Planets[$i]["Velocity"][1])","($Planets[$i]["Velocity"][2])")"	
}


$LcmXYZ = lcmFromArray -array (@($xsteps,$ysteps,$zsteps))

Write-Host "Total steps before System Repeat : $LcmXYZ" -ForegroundColor Green



