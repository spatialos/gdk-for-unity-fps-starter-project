# Self-elevate the script if required
if (-Not ([Security.Principal.WindowsPrincipal] [Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole] 'Administrator')) {
    if ([int](Get-CimInstance -Class Win32_OperatingSystem | Select-Object -ExpandProperty BuildNumber) -ge 6000) {
        $CommandLine = "-File `"" + $MyInvocation.MyCommand.Path + "`" " + $MyInvocation.UnboundArguments
        Start-Process -FilePath PowerShell.exe -Verb Runas -ArgumentList $CommandLine
        Exit
 }
}

function Select-Clone-Method
{
    $HTTPS_URI = "https://github.com/spatialos/gdk-for-unity.git"
    $SSH_URI = "git@github.com:spatialos/gdk-for-unity.git"

    Write-Host @"
Select a cloning method.

Press 1 for HTTPS ($($HTTPS_URI))
Press 2 for SSH   ($($SSH_URI))
Press 3 to Quit
"@

    do 
    {
        $Selection = Read-Host "Please make a selection "
        switch ($Selection)
        {
            '1' {
                return $HTTPS_URI
            }
            '2' {
                return $SSH_URI
            }
        }
    }
    until ($selection -eq '3')

    return -1
}

cd $PSScriptRoot

$TargetDirectory = $ExecutionContext.SessionState.Path.GetUnresolvedProviderPathFromPSPath($PSScriptRoot + "/../gdk-for-unity")

Write-Warning @"

This script will create the following directory to set up the SpatialOS GDK for Unity: $($TargetDirectory)
*** If such a directory already exists it will be deleted. ***
Are you ready to proceed? (y/n)
"@
$Confirmation = Read-Host

if ($Confirmation -ne 'y')
{
    Write-Host "Stopping the setup process."
    exit 0
}

$CloneUri = Select-Clone-Method

if ($CloneUri -eq -1)
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

$PinnedVersion = Get-Content ($PSScriptRoot + "/gdk.pinned")

Write-Host "Checking out pinned version: $($PinnedVersion)" -ForegroundColor Yellow

Push-Location -Path "$TargetDirectory"
    & git checkout "$PinnedVersion"
    
    if ($LastExitCode -ne 0)
    {
        Pop-Location
        exit 1
    } 
Pop-Location

Write-Host "Finished cloning the GDK for Unity." -ForegroundColor Green

Write-Host "Downloading SDK for GDK for Unity.." -ForegroundColor Yellow

& "$TargetDirectory/init.ps1"

Write-Host "Finished downloading SDK for GDK for Unity.." -ForegroundColor Green


# Resolve gdk-for-unity packages path
$PKGPath = Resolve-Path $(Join-Path $PSScriptRoot "workers/unity/Packages")
$PKGSrc = Resolve-Path $(Join-Path $TargetDirectory "workers/unity/Packages")
# Link package
function LinkPackage($package)
{
    $SourcePath = Join-Path $PKGSrc $package
    $DestinationPath = Join-Path $PKGPath $package
    if (!(Test-Path $DestinationPath))
    {
        New-Item -Path $DestinationPath -ItemType SymbolicLink -Value $SourcePath
    }
}

# Find all packages
$packages = Get-ChildItem -Path $PKGSrc -Directory -Force

# Link each package
foreach ($package in $packages)
{
    LinkPackage $package
}

Write-Host "Finished symlinking." -ForegroundColor Green
Write-Host -NoNewLine "Press any key to continue...";
$null = $Host.UI.RawUI.ReadKey('NoEcho,IncludeKeyDown');
