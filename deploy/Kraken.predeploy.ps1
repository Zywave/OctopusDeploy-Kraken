Write-Host "Writing appsettings.deploy.json file"
$contents = @"
{
	"AppSettings": {
		"OctopusServerAddress": "$OctopusServerAddress"
	}
"@
If (-Not [string]::IsNullOrEmpty($DatabaseConnectionString)) {
$contents = $contents + @"
,
	"Data": {
		"DefaultConnection": {
			"ConnectionString": "$DatabaseConnectionString"
		}
	}
"@
}
$contents = $contents + @"

}
"@
$contents | Out-File "./approot/src/Kraken/appsettings.deploy.json"
