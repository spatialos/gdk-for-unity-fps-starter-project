# Self-elevate the script if required
if (-Not ([Security.Principal.WindowsPrincipal] [Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole] 'Administrator'))
{
    Write-Error "This script has to be executed as Administrator."
    exit
}

# Resolve gdk-for-unity packages path
$GDKPath = Resolve-Path $(Join-Path $PSScriptRoot "../../../gdk-for-unity/workers/unity/Packages")
$PKGPath = Resolve-Path $(Join-Path $PSScriptRoot "../../workers/unity/Packages")

# Link package
function LinkPackage($package)
{
    $SourcePath = Join-Path $GDKPath $package
    $DestinationPath = Join-Path $PKGPath $package
    if (!(Test-Path $DestinationPath))
    {
        New-Item -Path $DestinationPath -ItemType SymbolicLink -Value $SourcePath
    }
}

# Find all packages
$packages = Get-ChildItem -Path $GDKPath -Directory -Force

# Link each package
foreach ($package in $packages)
{
    LinkPackage $package
}