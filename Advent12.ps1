$debug = $false

<#Find Greatest Common Denominator#>
function getGcd
{
	Param(
		[Parameter(Mandatory=$true)]
		[int] $a,
		
		[Parameter(Mandatory=$true)]
		[int] $b
		)
		
	if($a -eq 0)
	{
		return $b;
	}
	if($b -eq 0)
	{
		return $a;
	}
	
	if($a -eq $b)
	{
		return $a
	}
	
	if($a -gt $b)
	{
		return getGcd -a ($a-$b) -b $b
	}
	return getGcd -a $a -b ($b-$a)
		
}

<#Get Arc Tan, ie. angle of slope, #>
<#correcting for negative x or y #>
<#and only returning positve angles#>
function getATanDegree
{
	Param(
		[Parameter(Mandatory=$true)]
		[int] $x,
		
		[Parameter(Mandatory=$true)]
		[int] $y
		)
		
	if($x -ne 0)
	{
		if($x -ge 0)
		{
			$angRad = [math]::atan($y/$x)
		}
		else
		{
			$angRad = [math]::atan($y/$x) + [math]::pi
		}
	}
	elseif($y -ge 0)
	{
		$angRad = [math]::pi/2
	}
	else
	{
		$angRad = -([math]::pi)/2
	}
	
	if($angRad -lt 0)
	{
		$angRad += 2*([math]::pi)
	}
	return $angRad/[math]::pi *180
}

function MatchPosAndVel
{
	Param(
		[Parameter(Mandatory=$true)]
		[System.Array] $pos1,
		
		[Parameter(Mandatory=$true)]
		[System.Array] $y
		)
		
}



$file = Get-Content Advent12_input.txt

$Planet = @{"Position"= @(0,0,0); "Velocity" = @(0,0,0)}

$Planets = @(@{"Position"= @(0,0,0); "Velocity" = @(0,0,0)}, `
			 @{"Position"= @(0,0,0); "Velocity" = @(0,0,0)}, `
			 @{"Position"= @(0,0,0); "Velocity" = @(0,0,0)}, `
			 @{"Position"= @(0,0,0); "Velocity" = @(0,0,0)}	)

$Planets2 = @(@{"Position"= @(0,0,0); "Velocity" = @(0,0,0)}, `
			 @{"Position"= @(0,0,0); "Velocity" = @(0,0,0)}, `
			 @{"Position"= @(0,0,0); "Velocity" = @(0,0,0)}, `
			 @{"Position"= @(0,0,0); "Velocity" = @(0,0,0)}	)			 
			 
for($i= 0; $i -lt $file.Count; $i++)
{
	$file[$i]
	$withoutBraces = $file[$i].Substring(1,$file[$i].Length-2)
	$Parse = $withoutBraces.Split(",=")

	$Planets[$i]["Position"][0] = [int]($Parse[1])
	$Planets[$i]["Position"][1] = [int]($Parse[3])
	$Planets[$i]["Position"][2] = [int]($Parse[5])
	$Planets2[$i]["Position"][0] = [int]($Parse[1])
	$Planets2[$i]["Position"][1] = [int]($Parse[3])
	$Planets2[$i]["Position"][2] = [int]($Parse[5])
}

foreach($p in $Planets)
{
	$pos = $p["Position"]
	$vel = $p["Velocity"]
	
	Write-Host "`nPosition" $pos
	Write-Host "Velocity" $vel
}

$steps = 1
$match = $false
while($steps -le 10000000)
{
	if($steps % 1000 -eq 0)
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
	
	for($i= 0; $i -lt $file.Count; $i++)
	{
		$Planets2[$i]["Velocity"][0] = $Planets2[$i]["Velocity"][0]
		$Planets2[$i]["Velocity"][1] = $Planets2[$i]["Velocity"][1]
		$Planets2[$i]["Velocity"][2] = $Planets2[$i]["Velocity"][2]
		$Planets2[$i]["Position"][0] = $Planets[$i]["Position"][0]
		$Planets2[$i]["Position"][1] = $Planets[$i]["Position"][1]
		$Planets2[$i]["Position"][2] = $Planets[$i]["Position"][2]
	}
	
	
	$steps2 = ($steps)
	do
	{
		<#Change Pos#>
		for($i = 0; $i -lt $Planets2.Count; $i++)
		{
			$PlanetVel = $Planets2[$i]["Velocity"]
			
			$Planets2[$i]["Position"][0] -= $PlanetVel[0]
			$Planets2[$i]["Position"][1] -= $PlanetVel[1]
			$Planets2[$i]["Position"][2] -= $PlanetVel[2]
		}
	
		<#Change vel#>
		for($i = 0; $i -lt $Planets2.Count; $i++)
		{
			for($j = 0; $j -lt $Planets2.Count; $j++)
			{
				if($i -eq $j)
				{
					continue
				}
			
				$PlanetPos1 = $Planets2[$i]["Position"]
				$PlanetPos2 = $Planets2[$j]["Position"]
				
				if($PlanetPos1[0] -lt $PlanetPos2[0])
				{
					$Planets[$i]["Velocity"][0] -= 1
				}
				elseif ($PlanetPos1[0] -gt $PlanetPos2[0])
				{
					$Planets[$i]["Velocity"][0] += 1
				}
				
				if($PlanetPos1[1] -lt $PlanetPos2[1])
				{
					$Planets[$i]["Velocity"][1] -= 1
				}
				elseif ($PlanetPos1[1] -gt $PlanetPos2[1])
				{
					$Planets[$i]["Velocity"][1] += 1
				}	

				if($PlanetPos1[2] -lt $PlanetPos2[2])
				{
					$Planets[$i]["Velocity"][2] -= 1
				}
				elseif ($PlanetPos1[2] -gt $PlanetPos2[2])
				{
					$Planets[$i]["Velocity"][2] += 1
				}
			}
		}
		
		$match = $true
		for($i= 0; $i -lt $file.Count; $i++)
		{
			if($match -eq $false)
			{
				continue
			}
			
			if($Planets2[$i]["Velocity"][0] -ne $Planets[$i]["Velocity"][0] -or `
				$Planets2[$i]["Velocity"][1] -ne $Planets[$i]["Velocity"][1] -or `
				$Planets2[$i]["Velocity"][2] -ne $Planets[$i]["Velocity"][2] -or `
				$Planets2[$i]["Position"][0] -ne $Planets[$i]["Position"][0] -or `
				$Planets2[$i]["Position"][1] -ne $Planets[$i]["Position"][1] -or `
				$Planets2[$i]["Position"][2] -ne $Planets[$i]["Position"][2])
			{
				$match = $false
			}
			
		}
		
		
		if($match)
		{
			Write-Host "steps til first repeat" $steps
			break
		}
		$steps2 -= 1
	}
	while($steps2 -ge 1)
	
	if($match)
	{
		break
	}
	$steps++
}

<#Calc Energy#>

<#$Energy = 0
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

Write-Host "Total Energy" $Energy#>




<#foreach($x1 in $xRange)
{
	foreach($y1 in $yRange)
	{
		if($file[$y1][$x1] -eq "#")
		{
			if($debug)
			{
				Write-Host "Asteroid 1 ("$x1","$y1")"
			}
			$FoundAsteroid = 0;
			$file2 = Get-Content Advent10_input.txt
			
			$pausing = $false
			if($debug -and $x1 -eq 5 -and $y1 -eq 8)
			{
				$pausing = $true
			}
			foreach($x2 in $xRange)
			{
				foreach($y2 in $yRange)
				{
					if($pausing)
					{
						Write-Host "Asteroid 2 ("$x2","$y2")"
					}
				
					if(	($x1 -eq $x2 -and $y1 -eq $y2) -or `
						($file2[$y2][$x2] -ne "#") )
					{
						continue
					}
					
					$found = $false
					$xdiff = $x2-$x1
					$ydiff = $y2-$y1
					$gcd = getGcd -a ([math]::abs($xdiff)) -b ([math]::abs($ydiff))
					
					#><#Get lowest integer representation of slope
					if($gcd -ne 0)
					{
						$xdiff = $xdiff/$gcd
						$ydiff = $ydiff/$gcd
					}
					
					$mult = 1
					
					<#Count nearest asteroid along the line connecting
					<#Ass1 and Ass2, but mark all asteroids on this line 
					<#so we do not visit them again 
					while(	$xRange -contains ($x1 + $mult*$xdiff) -and `
							$yRange -contains ($y1 + $mult*$ydiff))
					{
						if($pausing)
						{
							Write-Host "yDiff " $yDiff
							Write-Host "xDiff " $xDiff
							Write-Host "Checking Asteroid  ("($x1 + $mult*$xdiff)","($y1 + $mult*$ydiff)")"
						}
						if ($file2[$y1 + $mult*$ydiff][$x1 + $mult*$xdiff] -eq "#")
						{
							if(!$found)
							{
								$found = $true
								$FoundAsteroid++
							}
							<#Replace this asteroid with marker
							$file2[$y1 + $mult*$ydiff] = $file2[$y1 + $mult*$ydiff].Remove($x1 + $mult*$xdiff,1).Insert($x1 + $mult*$xdiff,"L")
						}
						$mult++
					}
				}
			}
			
			if ($FoundAsteroid -gt $max)
			{
				$max = $FoundAsteroid
				Write-Host "Asteroid 1 ("$x1","$y1")"
				$maxPoint = @($x1,$y1)
				Write-Host "New Max = " $max -foregroundcolor red
			}
		
		}
	}
}
$angles = @{}
$file2 = Get-Content Advent10_input.txt
$file2


foreach($x1 in $xRange)
{
	foreach($y1 in $yRange)
	{
		if($file[$y1][$x1] -eq "#" -and `
			!($x1 -eq $maxPoint[0] -and $y1 -eq $maxPoint[1]))
		{
			$xdiff = $x1-$maxPoint[0]
			$ydiff = $y1-$maxPoint[1]
			
			
			$angDeg = getATan -x ($xdiff) -y (-$ydiff)
			
			$gcd = getGcd -a ([math]::abs($xdiff)) -b ([math]::abs($ydiff))
			if($gcd -ne 0)
			{
				$xdiff = $xdiff/$gcd
				$ydiff = $ydiff/$gcd
			}
			
			$found = $false
			$mult = 1
			$xValues = @()
			$yValues = @()
			while(	$xRange -contains ($maxPoint[0] + $mult*$xdiff) -and `
					$yRange -contains ($maxPoint[1] + $mult*$ydiff))
			{
				$newX = $maxPoint[0] + $mult*$xdiff
				$newY = $maxPoint[1] + $mult*$ydiff
				if ($file2[$newY][$newX] -eq "#")
				{
					$found = $true
					$file2[$newY] = $file2[$newY].Remove($newX,1).Insert($newX,"L")
					
					$xValues += $newX 
					$yValues += $newY
				}
				$mult++
			}
			
			if($found)
			{
				$angles[$angDeg] = @{"X" = $xValues; "Y" = $yValues}
			}
		}
	}
}


Write-Host "Max Point: ("$maxPoint[0]","$maxPoint[1]")"

$valueExists = $true
$index = 0
$count = 0
while($valueExists)
{
	$valueExists = $false
	foreach($ang in $angles.Keys | Where-Object {$_ -le 90}| Sort-Object -descending)
	{
		if($index -lt $angles[$ang]["X"].count)
		{
			$valueExists = $true
			$count++
			Write-Host "Asteroid #$count Destroyed " -nonewline
			Write-Host "`tX" ($angles[$ang]["X"][$index]) -nonewline
			Write-Host " Y" ($angles[$ang]["Y"][$index]) -nonewline
			
			Write-Host " Angle: " $ang
		}	
	}

	foreach($ang in $angles.Keys | Where-Object {$_ -gt 90}| Sort-Object -descending)
	{
		if($index -lt $angles[$ang]["X"].count)
		{
			$valueExists = $true
			$count++
			Write-Host "Asteroid #$count Destroyed " -nonewline
			Write-Host "`tX" ($angles[$ang]["X"][$index]) -nonewline
			Write-Host " Y" ($angles[$ang]["Y"][$index]) -nonewline
			
			Write-Host " Angle: " $ang
		}	
	}
	
	$index++
}

Write-Host "Max: " $max#>