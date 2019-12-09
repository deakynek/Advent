$debug = $false


function PerformOperation
{
	Param(
		[Parameter(Mandatory=$true)]
		[String[]] $array,
		
		[Parameter(Mandatory=$true)]
		[int] $commandIndex,

        [Parameter(Mandatory=$true)]
		[bool] $feedbackEnabled,

        [Parameter(Mandatory=$true)]
		[ref] $nextIndexWhenFeedbackRecieved
		)
		
	<#Set up variables, and default operands #>
	$command = $array[$commandIndex]
	$Operand1Address = [int]$array[$commandIndex+1]
	$Operand2Address = [int]$array[$commandIndex+2]
	$Operand3Address = [int]$array[$commandIndex+3]
	$immediateMode = $false
	
	$OperandCount = 0
	$InputType = @()
	if($command.Length -gt 2)
	{
		$immediateMode = $true
		<#New Command Type#>
		$comm = $command.Substring($command.Length-2,2)
		
		$intComm = [int]$comm
		
	}
	else
	{
		$intComm = [int]$command
	}
	
	
	if(($intComm -eq 1) -or ($intComm -eq 2) -or ($intComm -eq 7) -or ($intComm -eq 8))
	{
		$OperandCount = 3
	}
	if(($intComm -eq 5) -or ($intComm -eq 6))
	{
		$OperandCount = 2
	}
	if(($intComm -eq 3) -or ($intComm -eq 4))
	{
		$OperandCount = 1
	}
		
	if($immediateMode)
	{
		for($i = 1; $i -le $OperandCount; $i++)
		{
			$index = ($command.Length - 2 - $i)
			if($index -ge 0)
			{
				$temp = $command[$index]
				$InputType += $temp
			}
			else
			{
				$InputType += 0
			}
		}
	}
	$command = $intComm	
	
	$Operand1 = [int]$array[$Operand1Address]
	$Operand2 = [int]$array[$Operand2Address]
	
	if($immediateMode -and ($InputType.Count -gt 0) -and ($InputType[0] -eq "1"))
	{
		$Operand1 = $Operand1Address
	}
	if($immediateMode -and ($InputType.Count -gt 1) -and ($InputType[1] -eq "1"))
	{
		$Operand2 = $Operand2Address
	}	

	
	$OutputIndex = (($EngineIndex+1) % ($Outputs.Count))
	$nextCommandIndex = $commandIndex + $OperandCount + 1
	
	if($command -eq 1)
	{
		<#Find Sum of Operand1 and Operand2, Store at Operand3Address#>
        if($debug)
        {
		    Write-Host "Add" $Operand1 "and" $Operand2 -foregroundcolor red
        }
	
		$answer = $Operand1 + $Operand2
		$array[$Operand3Address] = $answer.ToString();
		
        if($debug)
        {
		    Write-Host "Postition" $Operand3Address "set to" $answer
        }
	}
	elseif($command -eq 2)
	{
		<#Find Product of Operand1 and Operand2, Store at Operand3Address#>
        if($debug)
        {
		    Write-Host "Multiply" $Operand1 "by" $Operand2 -foregroundcolor red
        }
	
		$answer = $Operand1 * $Operand2
		$array[$Operand3Address] = $answer.ToString();
		
        if($debug)
        {
		    Write-Host "Postition" $Operand3Address "set to" $answer
        }
	}
	elseif($command -eq 3)
	{
		<#Get input, Either Engine Id or preceding Engines Output#>
        if($EngineSoftwares[$EngineArray[$EngineIndex]]["IdSet"])
        {
            if($debug)
            {
                Write-Host "Inputing Last Output:" $Outputs[$EngineIndex] "to "$Operand1Address -ForegroundColor Cyan
                Write-Host "Output Array " $Outputs
                Write-Host "Output Index " $OutputIndex
            }
			
           $array[$Operand1Address] = $Outputs[$EngineIndex].ToString()
		}
        else
        {
            if($debug)
            {
                Write-Host "Inputing Engine Number:" $EngineArray[$EngineIndex] "to "$Operand1Address -ForegroundColor Cyan
                Write-Host "Engine Array " $EngineArray
                Write-Host "Engine Index " $EngineIndex
            }
			
		    $array[$Operand1Address] = $EngineArray[$EngineIndex].ToString()
			$EngineSoftwares[$EngineArray[$EngineIndex]]["IdSet"]= $true
        }

        if($debug)
        {
            Write-Host "Setting "$array[$Operand3Address] "to"$Operand3Address
        }
	}
	elseif($command -eq 4)
	{
		<#Ouput Operand 1 to OutputsArray, holding execution of this script until going#>
		<#through all other Engines and returning to this one#>
		if($debug)
		{
			Write-Host "Outputing Info" -foregroundcolor yellow
			Write-Host $array[$Operand1Address] -foregroundcolor yellow
		}
		
		$Outputs[$OutputIndex] = $Operand1
	
        if($feedbackEnabled -ne $null -and $feedbackEnabled)
        {

            $nextIndexWhenFeedbackRecieved.Value = ($commandIndex +2)
            if($debug)
            {
                Write-Host "Recurse Index :"($nextIndexWhenFeedbackRecieved.Value)
            }
            return $array
        }

	}
	elseif($command -eq 5)
	{
		<#If Operand 1 is equal to 0, Jump to index Operand2#>
		if($Operand1 -ne 0)
		{
            if($debug)
            {
			    Write-Host "Jump to index" $Operand2 -foregroundcolor red
            }
			
			$nextCommandIndex = $Operand2
		}
	}
	elseif($command -eq 6)
	{
		<#If Operand 1 equals 0, Jump to index Operand2#>
		if($Operand1 -eq 0)
		{
            if($debug)
            {
			    Write-Host "Jumping to index " $Operand2
            }
			$nextCommandIndex = $Operand2
		}
	}
	elseif($command -eq 7)
	{
		<#If Operand 1 is less than Operand 2, Set 1 to Operand3Address#>
		<#otherwise set 0 to Operand3Address#>
        if($debug)
        {
		    Write-Host "Is" $Operand1 "less than" $Operand2 -foregroundcolor red
        }
		if($Operand1 -lt $Operand2)
		{
			$array[$Operand3Address] = "1";
		}
		else
		{
			$array[$Operand3Address] = "0";
		}
		
        if($debug)
        {
		`	Write-Host "Postition" $Operand3Address "set to" $array[$Operand3Address]
        }
	}
	elseif($command -eq 8)
	{
		<#If Operand 1 is equal to Operand 2, Set 1 to Operand3Address#>
		<#otherwise set 0 to Operand3Address#>
        if($debug)
        {
		    Write-Host "Is" $Operand1 "equal to" $Operand2 -foregroundcolor red
        }
		if($Operand1 -eq $Operand2)
		{
			$array[$Operand3Address] = "1";
		}
		else
		{
			$array[$Operand3Address] = "0";
		}

		if($debug)
        {
		    Write-Host "Postition" $Operand3Address "set to" $array[$Operand3Address];
        }
	}
	elseif($command -eq 99)
	{
		<#End script#>
        if($feedbackEnabled -ne $null -and $feedbackEnabled)
        {
		    $nextIndexWhenFeedbackRecieved.value = 0
        }
		return $array
	}
	else
	{
		Write-Host "Incorrect Command.  Recieved:" $command
		return @()
	}
	
	return PerformOperation -array $array `
							-commandIndex $nextCommandIndex `
							-feedbackEnabled $feedbackEnabled `
							-nextIndexWhenFeedbackRecieved $nextIndexWhenFeedbackRecieved
}

$file = Get-Content Advent7_input.txt
$test = $file.Split(",")

<#Part one#>
$EngineIdList = [System.Collections.ArrayList](0..4)
$EngineSoftwares =@{}
foreach($id in $EngineIdList)
{
    $EngineSoftwares[$id] = @{"IdSet" = $false; "array"= @(); "index" = @()}
}

$maxout = 0
foreach ($i in $EngineIdList)
{
    foreach($j in ($EngineIdList | Where-Object{$_ -ne $i}))
    {
        foreach($k in ($EngineIdList | Where-Object{$_ -ne $i -and $_ -ne $j}))
        {
            foreach($l in ($EngineIdList | Where-Object{$_ -ne $i -and $_ -ne $j -and $_ -ne $k}))
            {
                foreach($m in ($EngineIdList | Where-Object{$_ -ne $i -and $_ -ne $j -and $_ -ne $k -and $_ -ne $l}))
                {
                    $EngineArray = @($i,$j,$k,$l,$m)
                    $EngineIndex = 0
                    $Outputs = @(0,0,0,0,0)

					
                    Write-Host "Engine Array " $EngineArray -foregroundcolor "Cyan"
                    foreach($engine in $EngineArray)
                    {
						$EngineSoftwares[$engine]["IdSet"] = $false
                        $arr = PerformOperation -array $file.Split(",") `
												-commandIndex 0 `
												-feedbackEnabled $false `
												-nextIndexWhenFeedbackRecieved ([ref](0))

                        $EngineIndex ++
                    }
                    if([int]($Outputs[0]) -gt $maxout)
                    {
                        $maxout = $Outputs[0]
                    }
                }
            }
        }
    }
}

Write-Host "Max Output Part 1`t"$maxout -foregroundcolor red

<#Part two#>



<#Populate Blank Collections#>
$EngineIdList = [System.Collections.ArrayList](5..9)
$EngineSoftwares =@{}
foreach($id in $EngineIdList)
{
    $EngineSoftwares[$id] = @{"IdSet" = $false; "array"= @(); "index" = @()}
}

$maxout = 0
foreach ($i in $EngineIdList)
{
    foreach($j in ($EngineIdList | Where-Object{$_ -ne $i}))
    {
        foreach($k in ($EngineIdList | Where-Object{$_ -ne $i -and $_ -ne $j}))
        {
            foreach($l in ($EngineIdList | Where-Object{$_ -ne $i -and $_ -ne $j -and $_ -ne $k}))
            {
                foreach($m in ($EngineIdList | Where-Object{$_ -ne $i -and $_ -ne $j -and $_ -ne $k -and $_ -ne $l}))
                {
                    $EngineArray = @($i,$j,$k,$l,$m)
                    
					<#Reset all engine softwares#>
					foreach($engine in $EngineArray)
					{
						$EngineSoftwares[$engine]["array"] = $file.Split(",")
						$EngineSoftwares[$engine]["index"] = 0
						$EngineSoftwares[$engine]["IdSet"] = $false
					}
                    $Outputs = @(0,0,0,0,0)


                    Write-Host "Engine Array " $EngineArray
					
                    do
                    {
                        $EngineIndex = 0
                        foreach($engine in $EngineArray)
                        {
                            $Index = [ref]($EngineSoftwares[$engine]["index"])
                            $EngineSoftwares[$engine]["array"] = PerformOperation 	-array $EngineSoftwares[$engine]["array"] `
																					-commandIndex ([ref]($EngineSoftwares[$engine]["index"])) `
																					-feedbackEnabled $true `
																					-nextIndexWhenFeedbackRecieved $Index
                            $EngineSoftwares[$engine]["index"] = $Index.value
                            $EngineIndex++
                        }

                    }while($EngineSoftwares[$m]["index"] -ne 0  )
                    
                    <#Write-Host "Output: "$Outputs#>
                    if([int]($Outputs[0]) -gt $maxout)
                    {
                        $maxout = $Outputs[0]
                    }
                }
            }
        }
    }
}

Write-Host "Max Output Part 2:`t"$maxout -foregroundcolor red

