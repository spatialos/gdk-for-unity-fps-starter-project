# Self-elevate the script if required
if (-Not ([Security.Principal.WindowsPrincipal] [Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole] 'Administrator')) {
    if ([int](Get-CimInstance -Class Win32_OperatingSystem | Select-Object -ExpandProperty BuildNumber) -ge 6000) {
        $CommandLine = "-File `"" + $MyInvocation.MyCommand.Path + "`" " + $MyInvocation.UnboundArguments
        Start-Process -FilePath PowerShell.exe -Verb Runas -ArgumentList $CommandLine
        Exit
 }
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

Write-Host "Finished symlinking."
Write-Host -NoNewLine "Press any key to continue...";
$null = $Host.UI.RawUI.ReadKey('NoEcho,IncludeKeyDown');