function RecurseMakeElement
{
	Param(
		[Parameter(Mandatory=$true)]
		[string] $elementName,
		
		[Parameter(Mandatory=$true)]
		[long] $ammountNeeded
		)
	
	if($equations.Keys -notcontains $elementName)
	{
		if($elementName -eq $ore)
		{
			if(HaveEnoughExtra -elementName $ore -ammount $ammountNeeded)
			{
				RemoveFromExtra -elementName $ore -ammount $ammountNeeded
				return $ammountNeeded
			}
			else
			{
				return 0
			}
		}
		else
		{
			if($debug)
			{
				Write-Host "No formula for $elementName"
				Write-Host "Formulas" $equations.Keys
			}
			return 0
		}
	}
	if($debug)
	{
		Write-Host "Makeing $ammountNeeded of $elementName"
	}
	
	$equation = $equations[$elementName]
	$equationEl = $equation["elements"]
	$equationAm = $equation["amounts"]
	
	if(HaveEnoughExtra -elementName $elementName -ammount $ammountNeeded)
	{
		RemoveFromExtra -elementName $elementName -ammount $ammountNeeded
		return $ammountNeeded
	}
	

	$extraAm = GetExtraCount -elementName $elementName
	$ammountNeeded -= $extraAm
	if($debug)
	{	
		Write-Host "Need $ammountNeeded"
	}
	$mult = [math]::ceiling($AmmountNeeded/($equationAm[0]))
	
	if($AmmountNeeded -ne 0)
	{	
		for($j = 1; $j -lt $equationEl.Count; $j++)
		{
			if(HaveEnoughExtra -elementName ($equationEl[$j]) -ammountNeeded ($mult*($equationAm[$j])))
			{
				RemoveFromExtra -elementName ($equationEl[$j]) -ammountNeeded ($mult*($equationAm[$j]))
			}
			else
			{
				$ammountMade = RecurseMakeElement -elementName ($equationEl[$j]) -ammountNeeded ($mult*($equationAm[$j]))
				
				if($ammountMade -gt ($mult*($equationAm[$j])))
				{
					AddToExtra -elementName ($equationEl[$j]) -ammount ($ammountMade - $mult*($equationAm[$j]))
				}
			}
		}
	}
	
	return ($mult*($equationAm[0])+$extraAm)
}

function RecurseMakeTheMostOfThisElement
{
	Param(
		[Parameter(Mandatory=$true)]
		[string] $elementName,
		
		[Parameter(Mandatory=$true)]
		[int] $max
		
		)
		
	if($elementName -eq $ore)
	{
		return;
	}
	$equation = $equations[$elementName]
	$equationEl = $equation["elements"]
	$equationAm = $equation["amounts"]
	
	
	$multPossible = $null
	$minNonZeroMult = $null
	for($i = 1; $i -lt $equationEl.count; $i++)
	{
		$couldMake = (GetExtraCount -elementName ($equationEl[$i]))/($equationAm[$i])
		$thisMult = [math]::floor($couldMake)
		
		
		if(($multPossible -eq $null) -or ($thisMult -lt $multPossible))
		{
			$multPossible = $thisMult
		}
		if(($thisMult -ne 0) -and (($minNonZeroMult -eq $null) -or ($thisMult -lt $minNonZeroMult)))
		{
			$minNonZeroMult = $thisMult
		}		
	}
	Write-Host "Trying to make $elementName, can make" ($multPossible*$equationAm[0])
	Write-Host "but limit is $max"
	if($multPossible -gt 0)
	{
		if($max -ne 0 -and $max -lt ($multPossible* $equationAm[0]))
		{
			$multPossible = [math]::ceiling($max/($equationAm[0]))
		}
		Write-Host "Adding" ($multPossible* $equationAm[0]) "$elementName"
		AddToExtra -elementName $elementName -ammount ($multPossible* $equationAm[0])
		
		for($i = 1; $i -lt $equationEl.count; $i++)
		{
			Write-Host "`tUsing" ($multPossible* $equationAm[$i]) ""($equationEl[$i])
			RemoveFromExtra -elementName $equationEl[$i] -ammount ($multPossible*$equationAm[$i])
		}
	}
	else
	{
		for($i = 1; $i -lt $equationEl.count; $i++)
		{
			$couldMake = (GetExtraCount -elementName ($equationEl[$i]))/($equationAm[$i])
			$thisMult = [math]::floor($couldMake)		
		
			if($thisMult -eq 0)
			{
				RecurseMakeTheMostOfThisElement -elementName $equationEl[$i] -max ($minNonZeroMult*($equationAm[$i]))
			}
		}
	}
	
	
}


function HaveEnoughExtra
{
	Param(
		[Parameter(Mandatory=$true)]
		[string] $elementName,
		
		[Parameter(Mandatory=$true)]
		[long] $ammountNeeded
		)
		
	if($Extras["elements"] -notcontains $elementName)
	{
		return $false;
	}
	
	return ($Extras["amounts"][($Extras["elements"].IndexOf($elementName))]) -ge $ammountNeeded
}

function RemoveFromExtra
{
	Param(
		[Parameter(Mandatory=$true)]
		[string] $elementName,
		
		[Parameter(Mandatory=$true)]
		[long] $ammountNeeded
		)
		
	if($Extras["elements"] -notcontains $elementName)
	{
		Write-Host "Could not remove $elementName, could not be found" 
		return;
	}

	if(HaveEnoughExtra -elementName $elementName -ammountNeeded $ammountNeeded)
	{
		SetExtra -elementName $elementName -ammount ((GetExtraCount -elementName $elementName) - $ammountNeeded)
	}
	else
	{
		SetExtra -elementName $elementName -ammount 0
	}
}

function AddToExtra
{
	Param(
		[Parameter(Mandatory=$true)]
		[string] $elementName,
		
		[Parameter(Mandatory=$true)]
		[long] $ammount
		)
		
	SetExtra -elementName $elementName -ammount ((GetExtraCount -elementName $elementName)+$ammount)
}

function SetExtra
{
	Param(
		[Parameter(Mandatory=$true)]
		[string] $elementName,
		
		[Parameter(Mandatory=$true)]
		[long] $ammount
		)
		
	if($Extras["elements"] -contains $elementName)
	{
		$Extras["amounts"][$Extras["elements"].IndexOf($elementName)] = $ammount
	}
	else
	{
		$Extras["elements"] += $elementName
		$Extras["amounts"] += $ammount
	}
}

function GetExtraCount
{
	Param(
		[Parameter(Mandatory=$true)]
		[string] $elementName
		)
		
	if($Extras["elements"] -contains $elementName)
	{
		return $Extras["amounts"][$Extras["elements"].IndexOf($elementName)]
	}

	return 0
}

function CompareToExtras
{
	Param(
		[Parameter(Mandatory=$true)]
		[System.Collections.ArrayList] $elements,
		
		[Parameter(Mandatory=$true)]
		[System.Collections.ArrayList] $ammounts
		)

		

	for($i = 0; $i -lt $Extras["elements"].Count; $i++)
	{
		if($elements -notcontains $Extras["elements"][$i])
		{
			return $false
		}
	}
	
	for($i = 0; $i -lt $elements.Count; $i++)
	{
		if($Extras["elements"] -notcontains $elements[$i])
		{
			return $false
		}
	}
	
	for($i = 0; $i -lt $Extras["elements"].Count; $i++)
	{
		if($Extras["amounts"][$i] -ne $ammounts[$elements.IndexOf($Extras["elements"][$i])])
		{
			return $false
		}
	}
	
	return $true
		
}

function WriteOutElements
{
	Param(
		[Parameter(Mandatory=$true)]
		[System.Collections.ArrayList] $elements,
		
		[Parameter(Mandatory=$true)]
		[System.Collections.ArrayList] $ammounts,
		
		[Parameter(Mandatory=$true)]
		[string] $color
		)

	for($i = 0; $i -lt $elements.Count; $i++)
	{
		Write-Host ""($elements[$i])": "($ammounts[$i]) -foregroundcolor $color
	}

		
}
<#Parse Input#>
$file = Get-Content Advent14_input.txt

$equations = @{}
foreach($line in $file)
{
	$equation = $line.Split("=>")
	$result = $equation[2].Split(" ,")
	$sum = $equation[0].Split(" ,")
	
	$elements = $()
	$amounts = $()
	
	for($i = 0; $i -lt $result.Count; $i++)
	{
		if($result[$i] -eq $null -or ($result -eq "") -or ($result -match "\s"))
		{
			continue
		}
		
		try
		{
			$amount = [int]($result[$i])
			if($amount -eq 0)
			{
				continue
			}
			
			$amounts += @($amount)
			$elements += @($result[($i+1)])
			break
		}
		catch
		{
			continue
		}
	}
	
	for($i = 0; $i -lt $sum.Count; $i++)
	{
		if($sum[$i] -eq $null)
		{
			continue
		}
		
		try
		{
			$amount = [int]($sum[$i])
			if($amount -eq 0)
			{
				continue
			}		
			$amounts += @($amount)
			$elements += @($sum[($i+1)])
			$i++
		}
		catch
		{
			continue
		}
	}
	$thisEquation = @{"elements" = $elements; "amounts" = $amounts}
	$equations += @{($elements[0]) = $thisEquation}
}

<# Test Dictionary
foreach($eq in $equations.Keys)
{
	$el = $equations[$eq]["elements"]
	$am = $equations[$eq]["amounts"]
	Write-Host "element" $el "counts" ($el.count)
	Write-Host "ammounts" $am "counts" ($am.count)
	
	Write-Host "To make "($am[0])" of "($el[0])" you need: " -nonewline
	
	for($i = 1; $i -lt $am.count; $i++)
	{
		Write-Host " "($am[$i])" of "($el[$i])","-nonewline
	}
	
	Write-Host ""
}#>


Write-Host "Keys " ($equations.Keys)

$elementsNeeded =  @{"elements" = ([System.Collections.ArrayList]@("FUEL")); "amounts" = ([System.Collections.ArrayList]@(1))}
$Extras =  @{"elements" = ([System.Collections.ArrayList]@()); "amounts" = ([System.Collections.ArrayList]@())}
do
{
	$copyEl = [System.Collections.ArrayList]@()
	$copyAm = [System.Collections.ArrayList]@()
	for($i = 0; $i -lt $elementsNeeded["elements"].count; $i++)
	{
		if($equations.Keys -notcontains $elementsNeeded["elements"][$i])
		{
			Write-Host "Could not find" ($elementsNeeded["elements"][$i])
			if($copyEl -contains  ($elementsNeeded["elements"][$i]))
			{
				$copyAm[$copyEl.IndexOf(($elementsNeeded["elements"][$i]))] += ($elementsNeeded["amounts"][$i])
			}
			else
			{
				$k= $copyEl.Add(($elementsNeeded["elements"][$i]))
				$k= $copyAm.Add(($elementsNeeded["amounts"][$i]))
			}			
			
			continue
		}
		
		$AmmountNeeded = ($elementsNeeded["amounts"][$i])
		$ElementNeeded = ($elementsNeeded["elements"][$i])
		
		$equation = $equations[$elementsNeeded["elements"][$i]]
		$equationEl = $equation["elements"]
		$equationAm = $equation["amounts"]
		
		Write-Host "Eq El "$equationEl -foregroundcolor cyan
		Write-Host "Eq Am" $equationAm -foregroundcolor cyan
		
		if($Extras["elements"] -contains $ElementNeeded)
		{
			$extraAm = ($Extras["amounts"][$Extras["elements"].IndexOf($ElementNeeded)])
		}
		else
		{
			$extraAm = 0
		}
		
		if($extraAm -ne 0)
		{
			Write-Host "HAD "$extraAm" OF "($elementsNeeded["elements"][$i]) "LYING AROUND" -foregroundcolor green
		}
		
		if($extraAm -gt $AmmountNeeded)
		{
			$AmmountNeeded = 0
			if($Extras["elements"] -contains $ElementNeeded)
			{
				$Extras["amounts"][$Extras["elements"].IndexOf($ElementNeeded)] = $extraAm - $AmmountNeeded
			}
		}
		else
		{
			$AmmountNeeded -= $extraAm
			
			if($Extras["elements"] -contains $ElementNeeded)
			{
				$Extras["amounts"][$Extras["elements"].IndexOf($ElementNeeded)] = 0
			}
			else
			{
				$Extras["elements"] += $ElementNeeded
				$Extras["amounts"] += 0
			}
		}
		
		$mult = [math]::ceiling($AmmountNeeded/($equationAm[0]))
		
		
		$elementsAdded = [System.Collections.ArrayList]@()
		$ammountAdded = [System.Collections.ArrayList]@()
		if($AmmountNeeded -ne 0)
		{
			if($Extras["elements"] -contains $ElementNeeded)
			{
				$Extras["amounts"][$Extras["elements"].IndexOf($ElementNeeded)] = ($equationAm[0])*$mult - $AmmountNeeded
			}
			else
			{
				$Extras["elements"] += $ElementNeeded
				$Extras["amounts"] += ($equationAm[0])*$mult - $AmmountNeeded
			}		
		
			for($j = 1; $j -lt $equationEl.Count; $j++)
			{
				$k=  $elementsAdded.Add(($equationEl[$j])) 
				$k=  $ammountAdded.Add($mult*($equationAm[$j]))
			}
			
			Write-Host "To get "($elementsNeeded["amounts"][$i])" units of "($elementsNeeded["elements"][$i])
			Write-Host "`tNeed to run equation" $mult "times"
			Write-Host "`tProducing " ($Extras["amounts"][$Extras["elements"].IndexOf($ElementNeeded)]) "extra"
		}
		
		Write-Host "El Added "$elementsAdded
		Write-Host "Am Added "$ammountAdded		
		for($j = 0; $j -lt $elementsAdded.Count; $j++)
		{
			if($copyEl -contains  ($elementsAdded[$j]))
			{
				$copyAm[$copyEl.IndexOf(($elementsAdded[$j]))] += ($ammountAdded[$j])
			}
			else
			{
				$k= $copyEl.Add(($elementsAdded[$j]))
				$k= $copyAm.Add(($ammountAdded[$j]))
			}
		}
	}

	Write-Host "CopyEl "$copyEl -foregroundcolor red
	Write-Host "CopyAm" $copyAm -foregroundcolor red
	$elementsNeeded["elements"] = $copyEl
	$elementsNeeded["amounts"] = $copyAm	

}while ($elementsNeeded["elements"].count -gt 1)

Write-Host "Elements "($elementsNeeded["elements"])
Write-Host "Ammounts" ($elementsNeeded["amounts"])

$FuelUsingUsualMethod = [math]::floor(1000000000000/($elementsNeeded["amounts"][0]))
Write-Host "With 1,000,000,000,000 ORE can make $FuelUsingUsualMethod fuel"
Write-Host "With this much extra:"

$ore = "ORE"
$unitsOreRemaining = (1000000000000 - ($FuelUsingUsualMethod*($elementsNeeded["amounts"][0])))
if($Extras["elements"] -contains $ore)
{
	$Extras["amounts"][$Extras["elements"].IndexOf($ore)] = $unitsOreRemaining
}
else
{
	$Extras["elements"] += $ore
	$Extras["amounts"] += $unitsOreRemaining
}	


$fuel = "FUEL"
if($Extras["elements"] -contains $fuel)
{
	$Extras["amounts"][$Extras["elements"].IndexOf($fuel)] = $FuelUsingUsualMethod
}
else
{
	$Extras["elements"] += $fuel
	$Extras["amounts"] += $FuelUsingUsualMethod
}	

for($i =0; $i -lt $Extras["elements"].count; $i++)
{
	if(($Extras["elements"][$i] -eq $ore) -or ($Extras["elements"][$i] -eq $fuel))
	{
		Write-Host ""($Extras["amounts"][$i])" units of " $Extras["elements"][$i]
	}
	elseif($Extras["amounts"][$i] -ne 0)
	{
	
		$Extras["amounts"][$i] = ($Extras["amounts"][$i] * $FuelUsingUsualMethod)
		Write-Host ""($Extras["amounts"][$i])" units of " $Extras["elements"][$i]
	}
}

pause

$loopCount = 1
$debug = $false
$fuelEquation = $equations[$fuel]
$fuelEquationEl = $fuelEquation["elements"]
$fuelEquationAm = $fuelEquation["amounts"]
$LoopCountOutput = 1000



do
{		

	$OldElements = [System.Collections.ArrayList]@($Extras["elements"])
	$OldAmmounts = [System.Collections.ArrayList]@($Extras["amounts"])
	$oldFuelAmmount = $Extras["amounts"][$Extras["elements"].IndexOf($fuel)]
	
	RecurseMakeTheMostOfThisElement -elementName $fuel -max 0
	
	<#Make MaxAmmount of Fuel#>
	<#$multPossible = $null
	for($i = 1; $i -lt $fuelEquationEl.count; $i++)
	{
		$couldMake = (GetExtraCount -elementName ($fuelEquationEl[$i]))/($fuelEquationAm[$i])
		$thisMult = [math]::floor($couldMake)
		if(($multPossible -eq $null) -or ($thisMult -lt $multPossible))
		{
			$multPossible = $thisMult
		}
		
		if($debug)
		{
			Write-Host "Have " (GetExtraCount -elementName ($fuelEquationEl[$i])) "of " ($fuelEquationEl[$i])
			Write-Host "Need " ($fuelEquationAm[$i]) "for" $fuelEquationAm[0] "Fuel"
			Write-Host "could make $couldMake of "$fuelEquationEl[$i]
			Write-Host "this mult"  $thisMult -foregroundcolor green
		}
	}
	
	if($debug)
	{
		Write-Host "mult possible"  $multPossible -foregroundcolor red
	}
	for($i = 1; $i -lt $fuelEquationEl.count; $i++)
	{
		RemoveFromExtra -elementName ($fuelEquationEl[$i]) -ammount ($multPossible*($fuelEquationAm[$i]))
		
		if($debug)
		{
			Write-Host "Element Count Before" (GetExtraCount -elementName ($fuelEquationEl[$i]))
			Write-Host "Removing" ($multPossible*($fuelEquationAm[$i]))
			Write-Host "`tElement Count After" (GetExtraCount -elementName ($fuelEquationEl[$i]))
		}
	}
	
	AddToExtra -elementName $fuel -ammount ($multPossible*$fuelEquationAm[0])
	if($debug)
	{
		Write-Host "Adding "($multPossible*$fuelEquationAm[0])" Fuel"
	}#>
	
	
	
	
	
	<#$elementsAdded = [System.Collections.ArrayList]@()
	$ammountAdded = [System.Collections.ArrayList]@()
	for($i = 1; $i -lt $fuelEquationEl.count; $i++)
	{
		$ammountMade = RecurseMakeElement -elementName ($fuelEquationEl[$i]) -ammountNeeded ($fuelEquationAm[$i])
		$elementsAdded += ($fuelEquationEl[$i])
		$ammountAdded += $ammountMade
		if($debug)
		{
			Write-Host "Adding " $ammountMade " element "($fuelEquationEl[$i])"and needed " ($fuelEquationAm[$i])
		}
	}
	
	$success = $true
	for($i = 0; $i -lt $elementsAdded.count; $i++)
	{
		if($debug)
		{
			Write-Host "Added "($ammountAdded[$i]) " element "($elementsAdded[$i])"and needed " ($fuelEquationAm[$i+1])
		}	
		
		if($ammountAdded[$i] -lt ($fuelEquationAm[$i+1]))
		{
			$success = $false
			break
		}
	}
	
	if($success)
	{
		AddToExtra -elementName $fuel -ammount 1
		for($i = 1; $i -lt $fuelEquationEl.count; $i++)
		{
			AddToExtra -elementName $fuelEquationEl[$i] -ammount $ammountAdded[$i-1]
			RemoveFromExtra -elementName $fuelEquationEl[$i] -ammount $fuelEquationAm[$i]
		}
	}#>

	
	$newFuelAmmount = $Extras["amounts"][$Extras["elements"].IndexOf($fuel)]
	
	
	Write-Host "Old Elements" -foregroundcolor cyan
	WriteOutElements -elements $OldElements -ammounts $OldAmmounts -color cyan
	
	Write-Host "New Elements" -foregroundcolor yellow
	WriteOutElements -elements ($Extras["elements"]) -ammounts ($Extras["amounts"]) -color yellow	
	
	if($loopCount%$LoopCountOutput -eq 0)
	{
		Write-Host "Made " ($newFuelAmmount-$oldFuelAmmount) "Fuel in loop " $loopCount "Total" $newFuelAmmount
	}
	$loopCount++
}
while(-not (CompareToExtras -elements ($OldElements) -ammounts ($OldAmmounts)))

Write-Host "Final Count" $oldFuelAmmount


