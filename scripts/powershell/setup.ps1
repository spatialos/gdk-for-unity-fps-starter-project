$ErrorActionPreference = "Stop"

Write-Host @"

Welcome to the SpatialOS GDK for Unity FPS Starter Project setup script.
"@

$TargetDirectory = $ExecutionContext.SessionState.Path.GetUnresolvedProviderPathFromPSPath($PSScriptRoot + "/../../../gdk-for-unity")

Write-Warning @"

This script will create the following directory: $($TargetDirectory)
If such a directory already exists it will be deleted.
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
& git clone "https://github.com/spatialos/gdk-for-unity.git" $TargetDirectory

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
        exit 1
    } 
Pop-Location

Write-Host "Finished!" -ForegroundColor Green
