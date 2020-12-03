<#Translate relative commands to #>
<#2D Euclidean points #>
function CreateLine 
{
    Param
    (
         [Parameter(Mandatory=$true, Position=0)]
         [String[]] $wire
    )
	
	$line = @()
	$line += @{steps = 0; values = @()}
	
	<#Setup first point#>
	$last = @(0,0)
	$line[0].values += $last
	
	for($i = 0; $i -lt $wire.Count; $i++)
	{
		$command = $wire[$i]
		$temp = $last
	
		<#Insert Next point, determined by command with cumulative steps#>
		if ($command[0] -eq "U")
		{
			$temp[1] += [int]$command.Substring(1)
			$line += @{steps = $line[$line.Count-1].steps + [int]$command.Substring(1); values = @()}
			$line[$line.Count-1].values += $temp
		}
		elseif ($command[0] -eq "D")
		{
			$temp[1] -= [int]$command.Substring(1)
			$line += @{steps = $line[$line.Count-1].steps + [int]$command.Substring(1); values = @()}
			$line[$line.Count-1].values += $temp
		}
		elseif ($command[0] -eq "R")
		{
			$temp[0] += [int]$command.Substring(1)
			$line += @{steps = $line[$line.Count-1].steps + [int]$command.Substring(1); values = @()}
			$line[$line.Count-1].values += $temp
		}
		elseif ($command[0] -eq "L")
		{
			$temp[0] -= [int]$command.Substring(1)
			$line += @{steps = $line[$line.Count-1].steps + [int]$command.Substring(1); values = @()}
			$line[$line.Count-1].values += $temp
		}
		
		$last = $temp
	}
	
	return $line
}

<#Take endpoints of two lines and determine if intersection exists#>
<#If not, return null, if so return the intersection#>
function FindIntersection
{
    Param
    (
         [Parameter(Mandatory=$true, Position=0)]
         [int[]] $Line1Point1,
         [Parameter(Mandatory=$true, Position=1)]
         [int[]] $Line1Point2,
         [Parameter(Mandatory=$true, Position=2)]
         [int[]] $Line2Point1,
         [Parameter(Mandatory=$true, Position=3)]
         [int[]] $Line2Point2
    )
	
	$Line1Ranges = GetRanges -Point1 $Line1Point1 -Point2 $Line1Point2
	$Line2Ranges = GetRanges -Point1 $Line2Point1 -Point2 $Line2Point2

	if($Line1Ranges -eq $null -or $Line2Ranges -eq $null)
	{
		return $null
	}
	
	$Line1XRange = $Line1Ranges[0].values
	$Line1YRange = $Line1Ranges[1].values
	$Line2XRange = $Line2Ranges[0].values
	$Line2YRange = $Line2Ranges[1].values
	
	if(($Line1XRange[0] -gt $Line2XRange[1]) -or ($Line1XRange[1] -lt $Line2XRange[0]))
	{
		return $null
	}
	if(($Line1YRange[0] -gt $Line2YRange[1]) -or ($Line1YRange[1] -lt $Line2YRange[0]))
	{
		return $null
	}
	
	<#Overlap between both ranges#>
	$OverlapXRange = @()
	$OverlapYRange = @()
	$OverlapPoint = @()
	
	<#Overlap X Min#>
	if($Line1XRange[0] -gt $Line2XRange[0])
	{
		$OverlapXRange += $Line1XRange[0]
	}
	else
	{
		$OverlapXRange += $Line2XRange[0]
	}
	
	<#Overlap X Max#>
	if($Line1XRange[1] -lt $Line2XRange[1])
	{
		$OverlapXRange += $Line1XRange[1]
	}
	else
	{
		$OverlapXRange += $Line2XRange[1]
	}

	<#Overlap Y Min#>
	if($Line1YRange[0] -gt $Line2YRange[0])
	{
		$OverlapYRange += $Line1YRange[0]
	}
	else
	{
		$OverlapYRange += $Line2YRange[0]
	}
	
	<#Overlap Y Max#>
	if($Line1YRange[1] -lt $Line2YRange[1])
	{
		$OverlapYRange += $Line1YRange[1]
	}
	else
	{
		$OverlapYRange += $Line2YRange[1]
	}
	
	<#Take lower part of overlap as intersection point#>
	<#This works in this toy problem, as I assume that #>
	<#Intersections will only occur between a vertical #>
	<#line of one wire and a horizontal line of the other #>
	$OverlapPoint += $OverlapXRange[0]
	$OverlapPoint += $OverlapYRange[0]
	
	return $OverlapPoint
}

<#Given two points of a line, return the #>
<#range of x values, and the range of y values#>
<#Order will be from smallest to largest#>
function GetRanges
{
    Param
    (
         [Parameter(Mandatory=$true, Position=0)]
         [int[]] $Point1,
         [Parameter(Mandatory=$true, Position=1)]
         [int[]] $Point2
	)
	
	$Ranges = @()
	$XRange = @()
	$YRange = @()
	
	<#X Range#>
	if($Point1[0] -gt $Point2[0])
	{
		$XRange += $Point2[0]
		$XRange += $Point1[0]
	}
	else
	{
		$XRange += $Point1[0]
		$XRange += $Point2[0]
	}
	
	<#Y Range#>
	if($Point1[1] -gt $Point2[1])
	{
		$YRange += $Point2[1]
		$YRange += $Point1[1]
	}
	else
	{
		$YRange += $Point1[1]
		$YRange += $Point2[1]
	}	
	
	$Ranges += @{name = 'xrange'; values = @()}
	$Ranges[0].values  += $XRange
	$Ranges += @{name = 'yrange'; values = @()}
	$Ranges[1].values  += $YRange
	
	return $Ranges
}


<#Script Start#>
<#Parse File#>
$file = Get-Content Advent3_input.txt
$wire1 = $file[0].Split(",")
$wire2 = $file[1].Split(",")

<#Translate commands to Euclidean Points#>
$line1 = CreateLine -wire $wire1
$line2 = CreateLine -wire $wire2

<# Write-Host "Line 1: steps"
Write-Host $line1.steps
Write-Host "Line 2: steps"
Write-Host $line2.steps #>


$intersections = @()
$leastDist = $null
$leastSteps = $null


<#Compare every line segment from Wire 1 to every line segment of Wire 2 to see if there's an intersection#>
For ($i =1; $i -lt $line1.Count; $i++)
{
	For ($j = 1; $j -lt $line2.Count; $j++)
	{		
		$intersection = FindIntersection -Line1Point1 ($line1[$i-1].values) -Line1Point2 ($line1[$i].values) -Line2Point1 ($line2[$j-1].values) -Line2Point2 ($line2[$j].values)
		
		if ($intersection -ne $null)
		{
			<#Find distance of intersection to origin#>
			$Dist = [Math]::Abs($intersection[0]) + [Math]::Abs($intersection[1])
			
			if($Dist -eq 0)
			{
				<#Origin itself does not count#>
				continue
			}

			<#Calculate steps by adding cumulative steps to get to last point plus Euclidean Distance (which will be equal to steps in this toy problem) from#>
			<#last point to intersection#>
			$Steps1 = $line1[$i-1].steps + [Math]::Sqrt([Math]::Pow($line1[$i-1].values[0]-$intersection[0],2) + [Math]::Pow($line1[$i-1].values[1]-$intersection[1],2))
			$Steps2 = $line2[$j-1].steps + [Math]::Sqrt([Math]::Pow($line2[$j-1].values[0]-$intersection[0],2) + [Math]::Pow($line2[$j-1].values[1]-$intersection[1],2))
			$TotalSteps = $Steps1+$Steps2
			Write-Host "Intersection:" $intersection
			Write-Host "`tDist:`t"$Dist
			Write-Host "`tSteps:`t"$TotalSteps			
			
			
			if($leastSteps -eq $null)
			{
				$leastSteps = $TotalSteps
			}
			elseif ($TotalSteps -lt $leastSteps)
			{
				$leastSteps = $TotalSteps
			}
			
			if($leastDist -eq $null)
			{
				$leastDist = $Dist
			}
			elseif ($Dist -lt $leastDist)
			{
				$leastDist = $Dist
			}
		}
	}
}

Write-Host "Least Dist of Intersection =" $leastDist
Write-Host "Least Steps of Intersection =" $leastSteps
