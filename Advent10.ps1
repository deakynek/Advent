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

$file = Get-Content Advent10_input.txt
$file2 = Get-Content Advent10_input.txt

$max =0
$maxPoint = @(0,0)
$yRange = @(0..($file.Count-1))
$xRange = @(0..($file[0].Length-1))

Write-Host "xRange " $xRange
Write-Host "yRange " $yRange

foreach($x1 in $xRange)
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
					
					<#Get lowest integer representation of slope#>
					if($gcd -ne 0)
					{
						$xdiff = $xdiff/$gcd
						$ydiff = $ydiff/$gcd
					}
					
					$mult = 1
					
					<#Count nearest asteroid along the line connecting#>
					<#Ass1 and Ass2, but mark all asteroids on this line #>
					<#so we do not visit them again #>
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
							<#Replace this asteroid with marker#>
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

Write-Host "Max: " $max