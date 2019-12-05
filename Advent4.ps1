function PerformOperation ($array, $commandIndex)
{

	Write-Output $commandIndex
	$command = $array[$commandIndex]
	
	if($command -eq "1")
	{
		$answer = [int]$array[$commandIndex+1] + [int]$array[$commandIndex+2]
		$array[$commandIndex+3] = $answer.ToString();
		Write-Output "Postition" ($commandIndex+3) "set to" ($array[$commandIndex+3])
		return PerformOperation $array ($commandIndex +4)
	}
	elseif($command -eq "2")
	{
		$answer = [int]$array[$commandIndex+1] * [int]$array[$commandIndex+2]
		$array[$commandIndex+3] = $answer.ToString();
		
		Write-Output "Postition" ($commandIndex+3) "set to" ($array[$commandIndex+3])
		return PerformOperation $array ($commandIndex +4)
	}
	elseif($command -eq "99")
	{
		return $array
	}
	else
	{
		Write-Output "Incorrect Command.  Recieved:" $array[$commandIndex]
		return @()
	}

}

function matchNumber
{
	Param(
		[Parameter(Mandatory=$true)]
		[String] $numstr
	)
	
	if ($numstr.Length -eq 0)
	{
		return $false
	}
	
	$testRegEX = [regex]::Match($numstr, $regex)
	
	if(!$testRegEX.Success)
	{
		return $false
	}
	elseif($testRegEX.Length -gt 2)
	{
		$sub = $numstr.Substring($testRegEX.Length+$testRegEX.Index)
		if($sub.Length -gt 0)
		{
			matchNumber -numstr $numstr.Substring($testRegEX.Length+$testRegEX.Index)
		}
		else
		{
			return $false
		}
	}
	else
	{
		return $true
	}
	
}

$total = 0
$regex = "(\d)\1+"
for ($i= 271973; $i -le 785961; $i++)
{
	$str = $i.ToString()
	$doubleFound = $false
	$increase = $true
	
	for ($j = 1; $j -lt $str.length; $j++)
	{
		if(matchNumber $str)
		{
			$doubleFound = $true
		}
		else
		{
			continue
		}
		
		$numAfter = [int]($str.Substring($j,1))
		$numBefore = [int]($str.Substring($j-1,1))
		if($numAfter -lt $numBefore)
		{
			$increase = $false
		}
	}
	
	if($increase -and $doubleFound)
	{
		Write-Host $i
		$total++
	}
	
} 

Write-Host "Total " $total

