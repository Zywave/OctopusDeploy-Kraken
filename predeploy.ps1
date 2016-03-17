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

If (-Not [string]::IsNullOrEmpty($StdoutLogEnabled)) {
	$config = [xml](Get-Content "./wwwroot/web.config")
	$node = $config.SelectNodes("configuration/system.webServer/httpPlatform")
	$node.SetAttribute("stdoutLogEnabled", $StdoutLogEnabled)
	$config.Save("./wwwroot/web.config")
}