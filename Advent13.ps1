$ShowDebugMessages = $false
$writeOutput = $false
$writeInput = $false
$ShowJumps = $false


function PerformOperation
{
	Param(
		[Parameter(Mandatory=$true)]
		[String[]] $array,
		
		[Parameter(Mandatory=$true)]
		[ref] $commandIndex,

        [Parameter(Mandatory=$true)]
		[ref] $relativeBase
		)
		
	<#Set up variables, and default operands #>
	$UnchangedInputArray = $array
	$command = $array[$commandIndex.Value]
	if($ShowDebugMessages)
	{
		Write-Host "`n`nInput Index = " ($commandIndex.Value) -foregroundColor darkGreen
		Write-Host "Command" $command -foregroundcolor blue -nonewline
	}
	
	while($commandIndex.Value+3 -gt ($array.Count - 1))
	{
		$array += "0"
	}
	$Operand1Address = [long]$array[$commandIndex.Value+1]
	$Operand2Address = [long]$array[$commandIndex.Value+2]
	$Operand3Address = [long]$array[$commandIndex.Value+3]

	
	$OperandCount = 0
	$InputType = @()
	if($command.Length -gt 2)
	{
		<#New Command Type#>
		$comm = $command.Substring($command.Length-2,2)
		
		$intComm = [long]$comm
		
	}
	else
	{
		$intComm = [long]$command
	}
	
		
	
	if(($intComm -eq 3) -or ($intComm -eq 4) -or ($intComm -eq 9))
	{
		$OperandCount = 1
		if($ShowDebugMessages)
		{	
			Write-Host " "$array[$commandIndex.Value+1] -foregroundcolor blue
		}
	}
	elseif(($intComm -eq 5) -or ($intComm -eq 6))
	{
		$OperandCount = 2
		if($ShowDebugMessages)
		{	
			Write-Host " "$array[$commandIndex.Value+1] $array[$commandIndex.Value+2] -foregroundcolor blue
		}			
	}
	elseif(($intComm -eq 1) -or ($intComm -eq 2) -or ($intComm -eq 7) -or ($intComm -eq 8))
	{
		$OperandCount = 3
		if($ShowDebugMessages)
		{	
			Write-Host " "$array[$commandIndex.Value+1] $array[$commandIndex.Value+2] $array[$commandIndex.Value+3] -foregroundcolor blue 
		}
	}
	elseif($ShowDebugMessages)
	{	
		Write-Host""
	}
		
	if($ShowDebugMessages)
	{	
		Write-Host "IntComm = " $intComm "`t" -ForegroundColor green
	}
	
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
	
	<#Set Operand 1 based on type#>
	if($InputType.Count -gt 0)
	{
		if($ShowDebugMessages)
		{	
			Write-Host "OP1 InputType: " ($InputType[0]) -foregroundColor darkred
		}
		
		$Operand1 = $Operand1Address
		if($InputType[0] -eq "0")
		{
			while($Operand1Address -gt ($array.Count - 1))
			{
				$array += @("0")
			}
			
			if($intComm -ne 3)
			{
				$Operand1 = [long]$array[$Operand1Address]
			}
		}
		elseif($InputType[0] -eq "2")
		{
			while(($Operand1Address + $relativeBase.Value) -gt ($array.Count - 1))
			{
				$array += @("0")
			}
			
			$Operand1 = [long]$array[$Operand1Address + $relativeBase.Value]
			if($intComm -eq 3)
			{
				$Operand1 = $Operand1Address + $relativeBase.Value
			}
		}
		
		if($ShowDebugMessages)
		{	
			Write-Host "Op 1: "$Operand1
		}
	}	
	
	
	if($InputType.Count -gt 1)
	{
		if($ShowDebugMessages)
		{	
			Write-Host "OP2 InputType: " ($InputType[1]) -ForegroundColor darkred
		}
		
		
		if($InputType[1] -eq "0")
		{
			while($Operand2Address -gt ($array.Count - 1))
			{
				$array += @("0")
			}
			$Operand2 = [long]$array[$Operand2Address]
		}	
		elseif($InputType[1] -eq "1")
		{
			$Operand2 = $Operand2Address
		}
		elseif($InputType[1] -eq "2")
		{
			$Operand2 = [long]$array[$Operand2Address]
			while(($Operand2Address + $relativeBase.Value) -gt ($array.Count - 1))
			{
				$array += @("0")
			}
			$Operand2 = [long]$array[$Operand2Address + $relativeBase.Value]
		}

		if($ShowDebugMessages)
		{	
			Write-Host "Op 2: "$Operand2
		}
	}
	
	if($InputType.Count -gt 2)
	{
		if($ShowDebugMessages)
		{
			Write-Host "OP3 InputType: " ($InputType[2]) -ForegroundColor darkred
		}
		
		if($InputType[2] -eq "0")
		{
			while($Operand3Address -gt ($array.Count - 1))
			{
				$array += @("0")
			}
		}	
		elseif($InputType[2] -eq "2")
		{
			while($Operand3Address+$relativeBase.Value -gt ($array.Count - 1))
			{
				$array += @("0")
			}
			$Operand3Address += $relativeBase.Value
		}
		
		if($ShowDebugMessages)
		{
			Write-Host "Op 3 Address : "$Operand3Address -ForegroundColor darkred
		}
	}
	
	
	$OutputIndex = (($EngineIndex+1) % ($EngineArray.Count))
	$commandIndex.Value= $commandIndex.Value + $OperandCount + 1
	
	
	if($intComm -eq 1)
	{
		<#Find Sum of Operand1 and Operand2, Store at Operand3Address#>
        if($ShowDebugMessages)
        {
		    Write-Host "Add" $Operand1 "and" $Operand2 -foregroundcolor red
        }
	
		$answer = $Operand1 + $Operand2
		$array[$Operand3Address] = $answer.ToString();
		
        if($ShowDebugMessages)
        {
		    Write-Host "Position" $Operand3Address "set to" $answer
        }
	}
	elseif($intComm -eq 2)
	{
		<#Find Product of Operand1 and Operand2, Store at Operand3Address#>
        if($ShowDebugMessages)
        {
		    Write-Host "Multiply" $Operand1 "by" $Operand2 -foregroundcolor red
        }
	
		$answer = $Operand1 * $Operand2
		$array[$Operand3Address] = $answer.ToString();
		
        if($ShowDebugMessages)
        {
		    Write-Host "Position" $Operand3Address "set to" $answer
        }
	}
	elseif($intComm -eq 3)
	{
		if($EngineSoftwares[$EngineArray[$EngineIndex]]["PromptInput"])
		{
			$input = Read-Host ($EngineSoftwares[$EngineArray[$EngineIndex]]["PromptMessage"])
			$EngineSoftwares[$engine]["PrintOutput"] = $true
		}
		<#Get input, Either Engine Id or preceding Engines Output#>
        elseif($EngineSoftwares[$EngineArray[$EngineIndex]]["IdSet"])
        {
			$PrevEngineIndex = ($EngineIndex + $EngineArray.Count - 1) % ($EngineArray.Count)
            if($ShowDebugMessages -or $writeInput)
            {
                Write-Host "Inputing Last Output:" -ForegroundColor Cyan
                Write-Host "Output of Engine " $EngineArray[$PrevEngineIndex] "was" ($EngineSoftwares[$EngineArray[$PrevEngineIndex]]["Output"])
                Write-Host "Output Index " $OutputIndex
            }
			
			$input = ($EngineSoftwares[$EngineArray[$PrevEngineIndex]]["Output"]).ToString()
		}
        else
        {
            if($ShowDebugMessages -or $writeInput)
            {
                Write-Host "Inputing Engine Number:" -ForegroundColor Cyan
                Write-Host "Engine Array " $EngineArray
                Write-Host "Engine Index " $EngineIndex
            }
			
			$input = $EngineArray[$EngineIndex].ToString()
			$EngineSoftwares[$EngineArray[$EngineIndex]]["IdSet"]= $true
        }

		
		if($ShowDebugMessages)
		{
			Write-Host "Inputing "$input" to address " ($Operand1) -ForegroundColor Cyan		
		}
		
		$array[$Operand1]  = $input.ToString()
	}
	elseif($intComm -eq 4)
	{
		<#Ouput Operand 1 to OutputsArray, holding execution of this script until going#>
		<#through all other Engines and returning to this one#>
		if($ShowDebugMessages -or $writeOutput)
		{
			Write-Host "Outputting Info" -foregroundcolor yellow
			Write-Host  $Operand1 -foregroundcolor yellow
		}
		
		$EngineSoftwares[$EngineArray[$EngineIndex]]["Output"]=  $Operand1
		$EngineSoftwares[$EngineArray[$EngineIndex]]["executeNextEngine"]=  $true
		
		if($ShowDebugMessages)
		{
			Write-Host "Feedback Index :"($commandIndex.Value)
		}
	}
	elseif($intComm -eq 5)
	{
		if($ShowDebugMessages)
        {
		    Write-Host "Is" $Operand1 "not equal to 0" -foregroundcolor red
        }
		<#If Operand 1 is not equal to 0, Jump to index Operand2#>
		if($Operand1 -ne 0)
		{
            if($ShowDebugMessages)
            {
			    Write-Host "`tJump to index" $Operand2 -foregroundcolor red
            }
			
			$commandIndex.Value = $Operand2
		}
	}
	elseif($intComm -eq 6)
	{
		if($ShowDebugMessages)
        {
		    Write-Host "Is" $Operand1 "equal to 0" -foregroundcolor red
        }
		<#If Operand 1 equals 0, Jump to index Operand2#>
		if($Operand1 -eq 0)
		{
            if($ShowDebugMessages)
            {
			    Write-Host "`tJumping to index " $Operand2
            }
			$commandIndex.Value = $Operand2
		}
	}
	elseif($intComm -eq 7)
	{
		<#If Operand 1 is less than Operand 2, Set 1 to Operand3Address#>
		<#otherwise set 0 to Operand3Address#>
        if($ShowDebugMessages -or $ShowJumps)
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
		
        if($ShowDebugMessages -or $ShowJumps)
        {
		`	Write-Host "`tPosition" $Operand3Address "set to" $array[$Operand3Address]
        }
	}
	elseif($intComm -eq 8)
	{
		<#If Operand 1 is equal to Operand 2, Set 1 to Operand3Address#>
		<#otherwise set 0 to Operand3Address#>
        if($ShowDebugMessages)
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

		if($ShowDebugMessages)
        {
		    Write-Host "`tPosition" $Operand3Address "set to" $array[$Operand3Address];
        }
	}
	elseif($intComm -eq 9)
	{
        if($ShowDebugMessages)
        {
			Write-Host "Relative base was " ($relativeBase.Value)", set to" ($relativeBase.Value+$Operand1)
        }	
		$relativeBase.Value = $relativeBase.Value + $Operand1
	}
	elseif($intComm -eq 99)
	{
		<#End script#>

		$EngineSoftwares[$EngineArray[$EngineIndex]]["ScriptComplete"]=  $true
		if($withPauses)
		{
			pause
		}
	}
	else
	{
		Write-Host "Incorrect Command.  Received:" $intComm "of type" ($intComm.GetType())
		$EngineSoftwares[$EngineArray[$EngineIndex]]["ScriptComplete"] = $true
		return @()
	}
	
	if($ShowDebugMessages)
	{
		Write-Host "Output Index = " ($commandIndex.Value) -foregroundColor darkGreen
	}
	return $array
}

function HasPointBeenPainted
{
	Param(
		[Parameter(Mandatory=$true)]
		[int] $x,
		
		[Parameter(Mandatory=$true)]
		[int] $y
		)
		
	for ($i = 0; $i -lt $Points["X"].Count; $i++)
	{
		if($Points["X"][$i] -ne $x)
		{
			continue
		}
		elseif($Points["Y"][$i] -ne $y)
		{
			continue
		}
		else
		{
			return $i
		}
	}
	
	return $null
}


$file = Get-Content Advent13_input.txt
$EngineArray = @(1)
$EngineIndex = 0
$EngineSoftwares = @{}
$Output = 1
$width = 0
$EngineSoftwares[$EngineArray[0]] = @{"IdSet" = $true; "array"= $file.Split(","); "index" = 0; "relativeBase" = 0; "Output"=0; "ScriptComplete" = $false; "executeNextEngine" = $false; "PromptInput" = $false; "PromptMessage" = "-1 = L, 0 = none, 1 = R"}
do
{
	$EngineIndex = 0
	$engine = $EngineArray[0]
	$EngineSoftwares[$engine]["executeNextEngine"] = $false
	do
	{
		$IndexReference = ([ref]($EngineSoftwares[$engine]["index"]))
		$BaseReference = ([ref]($EngineSoftwares[$engine]["relativeBase"]))
		
		$EngineSoftwares[$engine]["array"] = PerformOperation 	-array $EngineSoftwares[$engine]["array"] `
																		-commandIndex $IndexReference `
																		-relativeBase $BaseReference
																		
		
		$EngineSoftwares[$engine]["index"] = $IndexReference.Value				
		$EngineSoftwares[$engine]["relativeBase"] = $BaseReference.Value
	}
	while($EngineSoftwares[$engine]["executeNextEngine"] -eq $false -and $EngineSoftwares[$engine]["ScriptComplete"] -eq $false)
		
	if($Output -eq 1 -and $EngineSoftwares[$engine]["executeNextEngine"])
	{
		$Output++
		
		if(($EngineSoftwares[$engine]["Output"]) -ne -1)
		{
			$xpos = ($EngineSoftwares[$engine]["Output"])
		}
		else
		{
			$ScoreOutput = $true
		}
	}
	elseif($Output -eq 2 -and $EngineSoftwares[$engine]["executeNextEngine"])
	{
		$Output++
		$ypos = ($EngineSoftwares[$engine]["Output"])
		if($ScoreOutput -and ($EngineSoftwares[$engine]["Output"]) -ne 0)
		{
			$ScoreOutput = $false
		}
	}
	elseif($Output -eq 3 -and $EngineSoftwares[$engine]["executeNextEngine"])
	{	
		$Output=1
		
		if($ScoreOutput)
		{
			$BlockCount -= 1
			Write-Host "$BlockCount blocks remaining. Score:" ($EngineSoftwares[$engine]["Output"])
			$score = ($EngineSoftwares[$engine]["Output"])
		}
		else
		{
			if($EngineSoftwares[$engine]["Output"] -eq 2)
			{
				$BlockCount++
			}
		}
	}
		
}while($EngineSoftwares[$EngineArray[0]]["ScriptComplete"] -eq $false)


Write-Host "Block Count $BlockCount" -ForegroundColor green


<#Part 2#>
$EngineSoftwares[$EngineArray[0]] = @{"IdSet" = $true; "array"= $file.Split(","); "index" = 0; "relativeBase" = 0; "Output"=0; "ScriptComplete" = $false; "executeNextEngine" = $false; "PromptInput" = $false; "PromptMessage" = "-1 = L, 0 = none, 1 = R"}
$EngineSoftwares[$EngineArray[0]]["array"][0] = 2
$Output=1
$score = 0
$ScoreOutput = $false
$Input = 0
$ballPosX = 0
$PaddleX = 0

$PrintBlocks = 50

<#Needed because score outputs 0 at start#>
$BlockCount++
$Points = @{"X"=@(); "Y"=@()}
$Pixels = @()
$Loaded = $false
$RenderOutput=""
do
{
	$EngineIndex = 0
	$engine = $EngineArray[0]
	$EngineSoftwares[$engine]["executeNextEngine"] = $false
	$Rendering = $false
	do
	{
		$IndexReference = ([ref]($EngineSoftwares[$engine]["index"]))
		$BaseReference = ([ref]($EngineSoftwares[$engine]["relativeBase"]))
		
		$EngineSoftwares[$engine]["array"] = PerformOperation 	-array $EngineSoftwares[$engine]["array"] `
																		-commandIndex $IndexReference `
																		-relativeBase $BaseReference
																		
		
		$EngineSoftwares[$engine]["index"] = $IndexReference.Value				
		$EngineSoftwares[$engine]["relativeBase"] = $BaseReference.Value
	}
	while($EngineSoftwares[$engine]["executeNextEngine"] -eq $false -and $EngineSoftwares[$engine]["ScriptComplete"] -eq $false)
		

	
	
	if($Output -eq 1 -and $EngineSoftwares[$engine]["executeNextEngine"])
	{
		$Output++
		
		if(($EngineSoftwares[$engine]["Output"]) -ne -1)
		{
			$xpos = ($EngineSoftwares[$engine]["Output"])
		}
		else
		{
			$ScoreOutput = $true
		}
	}
	elseif($Output -eq 2 -and $EngineSoftwares[$engine]["executeNextEngine"])
	{
		$Output++
		$ypos = ($EngineSoftwares[$engine]["Output"])
		if($ScoreOutput -and ($EngineSoftwares[$engine]["Output"]) -ne 0)
		{
			$ScoreOutput = $false
		}
	}
	elseif($Output -eq 3 -and $EngineSoftwares[$engine]["executeNextEngine"])
	{	
		$Output=1
		$UpdateScreen = $false
		if($ScoreOutput)
		{
			$BlockCount -= 1
			
			if($BlockCount %$PrintBlocks -eq 0)
			{
				<#Write-Host "$BlockCount blocks remaining. Score:" ($EngineSoftwares[$engine]["Output"])#>
				<#Render Page#>
				<#$UpdateScreen = $true#>
			}
			$score = ($EngineSoftwares[$engine]["Output"])
		}
		else
		{
			$index = HasPointBeenPainted -x $xpos -y $ypos
	
			if(($EngineSoftwares[$engine]["Output"]) -eq 3)
			{
				$char1 = "="
				<#Write-Host "Paddle Goes to ($xpos,$ypos)" -foregroundColor red#>
				
				$PaddleX = $xpos
			}
			elseif(($EngineSoftwares[$engine]["Output"]) -eq 4)
			{
				$char1 = "O"
				$UpdateScreen = ($EngineSoftwares[$engine]["PrintOutput"]) 
				<#Write-Host "Ball Goes to ($xpos,$ypos)" -foregroundColor red#>
				$ballPosX = $xpos
				$Rendering = $true
				if($ballPosX -gt $PaddleX)
				{
					$Input = 1
				}
				elseif($ballPosX -lt $PaddleX)
				{
					$Input = -1
				}
				else
				{
					$Input = 0
				}
			}
			elseif(($EngineSoftwares[$engine]["Output"]) -eq 0)
			{
				$char1 = " "
			}
			
			
			if($index -eq $null)
			{
				$Points["X"] += ($xpos)
				$Points["Y"] += ($ypos)
				$Pixels += ($EngineSoftwares[$engine]["Output"])
			}
			else
			{
				$Pixels[$index] = ($EngineSoftwares[$engine]["Output"])
				
				<#Character count in output: Carriage return + 0 based ypos + 0 based xpos#>
				if($Loaded)
				{
					$RenderIndex = ($ypos+1) + $ypos*$width + ($xpos+1)
					$RenderOutput= $RenderOutput.Remove($RenderIndex,1).Insert($RenderIndex,$char1)
					<#Write-Host "Rendering Char" $char1#>
					if($Rendering)
					{
						Write-Host $RenderOutput -nonewline
					}
				}
				
				if(!$Loaded)
				{
					$UpdateScreen = $true
					$Rendering = $true
				}
			}
		}
		

		
		if($UpdateScreen)
		{
			$xRange = $Points["X"] | Measure-Object -maximum -minimum
			$yRange = $Points["Y"] | Measure-Object -maximum -minimum		
			
			Write-Host "YMin" ($yRange.minimum) "YMax" ($yRange.maximum)
			Write-Host "XMin" ($xRange.minimum) "XMax" ($xRange.maximum)
			$width = $xRange.maximum - $xRange.minimum + 1
			
			pause
			
			if($Rendering)
			{
				$RenderOutput="`r"
			}
			for($y = $yRange.minimum; $y -le $yRange.maximum; $y++)
			{
				if($Rendering)
				{
					$RenderOutput+="`n"
				}
				for($x = $xRange.minimum; $x -le $xRange.maximum; $x++)
				{
					for($i = 0; $i -lt $Pixels.Count; $i++)
					{
						if(($Points["X"][$i]) -ne $x -or ($Points["Y"][$i]) -ne $y)
						{
							continue
						}
						
						$color = "white"
						if(($Pixels[$i]) -eq 0)
						{
							$char = " "
						}		
						elseif(($Pixels[$i]) -eq 1)
						{
							$char = "|"
						}				
						elseif(($Pixels[$i]) -eq 2)
						{
							$char = "#"
						}				
						elseif(($Pixels[$i]) -eq 3)
						{
							$char = "="
							$color = "green"
						}				
						elseif(($Pixels[$i]) -eq 4)
						{
							$char = "O"
							$color = "yellow"
						}
						
						if($Rendering)
						{
							$RenderOutput += $char
						}
						elseif($UpdateScreen)
						{
							Write-Host $char -nonewline -foregroundColor $color
						}						
					}
				}
				if($UpdateScreen)
				{				
					Write-Host ""
				}
			}
			$Loaded = $true
		}
	}
	$EngineSoftwares[$engine]["Output"] = $Input
		
}while($EngineSoftwares[$EngineArray[0]]["ScriptComplete"] -eq $false  -or $BlockCount -gt 0)

Write-Host "`nGAME OVER" -ForegroundColor red

<#Write-Host "Number of blocks=" $twos#>
Write-Host "Score :" $score -ForegroundColor green