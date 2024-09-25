for ($i=0;$i -lt 10000; $i++)
{
    Write-Host Calling $i
    Invoke-WebRequest https://ps-testing.azurewebsites.net/home/index/h$i | Out-Null
    Start-Sleep -Milliseconds 50
}