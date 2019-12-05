function RecurseFuelOfFuel($fuel)
{
	$intRound = (($fuel/3)- (($fuel/3)%1) -2)
	
	
	if( $intRound -gt 0)
	{
		return $intRound + (RecurseFuelOfFuel $intRound)
	}
	else
	{
		return 0
	}

}

$file = Get-Content Advent1_input.txt

$total = 0 
$file | ForEach-Object {
	Write-Output "Weight is : " $_
	$integer = [int]($_)
	$intRound = (($integer/3)- (($integer/3)%1) -2)
	
	Write-Output "Fuel for this weight is:" $intRound
	if( $intRound -gt 0)
	{
		$total += $intRound + (RecurseFuelOfFuel $intRound)
	}
}


Write-Output "Total Fuel is :" $total