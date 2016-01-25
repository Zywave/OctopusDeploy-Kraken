Write-Host "Writing appsettings.deploy.json file"
@"
{
  "AppSettings": {
    "OctopusServerAddress": "$OctopusServerAddress"
  },
  "Data": {
    "DefaultConnection": {
      "ConnectionString": "$DatabaseConnectionString"
    }
  }
}
"@ | Out-File ".\approot\src\Kraken\appsettings.deploy.json"