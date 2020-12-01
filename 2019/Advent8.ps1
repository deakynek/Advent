$file = (Get-Content Advent8_input.txt).ToString()
$Height = 6
$Width = 25
$Area = $Height * $Width

$Layers = @()
for($i = 0; $i -lt $file.Length/$Area; $i++)
{
    $Layers += $file.Substring($Area*$i,$Area)
}

<#Part 1#>
$ZEROS = $null
$Ones = $null
$Twos = $null
foreach ($lay in $Layers)
{
    $Matches = Select-String -InputObject $lay -Pattern "0" -AllMatches
    $count = $Matches.Matches.Count
    
    if ($ZEROS -eq $null -or $count -lt $ZEROS )
    {
        $ZEROS = $count
        $Matches = Select-String -InputObject $lay -Pattern "1" -AllMatches
        $Ones = $Matches.Matches.Count
		
        $Matches = Select-String -InputObject $lay -Pattern "2" -AllMatches
        $Twos = $Matches.Matches.Count
    }

}

Write-Host "Min zeros: " $ZEROS
Write-Host "On the ones and twos:" ($Ones*$Twos)

<#Part 2#>
$Final = [System.Collections.ArrayList](0..($Layers[0].Length-1))
$TotalIndexes = @(0..($Layers[0].Length-1))
$UsedIndexes = @()

<#Find Result of Overlapped Layers#>
foreach ($lay in $Layers)
{
    foreach($index in $TotalIndexes | Where-Object{$UsedIndexes -NotContains $_})
    {
        if($lay[$index] -ne "2")
        {
            $Final[$index] = $lay.Substring($index,1)
            $UsedIndexes += $index
        }
    }
	
	if($UsedIndexes.Count -eq $TotalIndexes.Count)
	{
		break
	}
}

<#"Draw" Final Layer Overlap#>
For($i = 0; $i -lt $Final.Count; $i++)
{
    if($i % $Width  -eq 0)
    {
        Write-Host ""
    }

    if($Final[$i] -eq "0")
    {
        Write-Host -NoNewLine "0" -foregroundcolor black
    }
    else
    {
        Write-Host -NoNewLine "0" -foregroundcolor white
    }
}