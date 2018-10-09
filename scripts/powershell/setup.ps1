$ErrorActionPreference = "Stop"

function Select-Clone-Method
{
    Write-Host @"
Select a cloning method.

Press 1 for HTTPS
Press 2 for SSH
Press 3 to Quit
"@

    do 
    {
        $Selection = Read-Host "Please make a selection "
        switch ($Selection)
        {
            '1' {
                return "https://github.com/spatialos/gdk-for-unity.git"
            }
            '2' {
                return "git@github.com:spatialos/gdk-for-unity.git"
            }
        }
    }
    until ($selection -eq '3')

    return -1
}

Write-Host @"

Welcome to the SpatialOS GDK for Unity FPS Starter Project setup script.
"@

$TargetDirectory = $ExecutionContext.SessionState.Path.GetUnresolvedProviderPathFromPSPath($PSScriptRoot + "/../../../gdk-for-unity")

$CloneUri = Select-Clone-Method

if ($CloneUri -eq -1)
{
    Write-Host "Stopping the setup process."
    exit 0
}

Write-Warning @"

This script will create the following directory: $($TargetDirectory)
*** If such a directory already exists it will be deleted. ***
Are you ready to proceed? (y/n)
"@
$Confirmation = Read-Host

if ($Confirmation -ne 'y')
{
    Write-Host "Stopping the setup process."
    exit 0
}

if (Test-Path -Path $TargetDirectory)
{
    Write-Host "Deleting existing directory at $($TargetDirectory)..." -ForegroundColor Yellow
    Remove-Item $TargetDirectory -Force -Recurse
}

Write-Host "Cloning SpatialOS GDK for Unity" -ForegroundColor Yellow
& git clone $CloneUri $TargetDirectory

if ($LastExitCode -ne 0)
{
    exit 1
} 

$PinnedVersion = Get-Content ($PSScriptRoot + "/../../gdk.pinned")

Write-Host "Checking out pinned version: $($PinnedVersion)" -ForegroundColor Yellow

Push-Location -Path "$TargetDirectory"
    & git checkout "$PinnedVersion"
    
    if ($LastExitCode -ne 0)
    {
        Pop-Location
        exit 1
    } 
Pop-Location

Write-Host "Finished!" -ForegroundColor Green
