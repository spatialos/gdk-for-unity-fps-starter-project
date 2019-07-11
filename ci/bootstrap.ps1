# Self-elevate the script if required
if (-Not ([Security.Principal.WindowsPrincipal] [Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole] 'Administrator')) {
    if ([int](Get-CimInstance -Class Win32_OperatingSystem | Select-Object -ExpandProperty BuildNumber) -ge 6000) {
        $CommandLine = "-File `"" + $MyInvocation.MyCommand.Path + "`" " + $MyInvocation.UnboundArguments
        Start-Process -Wait -FilePath PowerShell.exe -Verb Runas -ArgumentList $CommandLine
        Exit
 }
}

$ProjectRoot = (Resolve-Path $(Join-Path $PSScriptRoot "../")).Path
cd $ProjectRoot

# ====================== Shared CI ==================== #

$TargetSharedCiDirectory = $ExecutionContext.SessionState.Path.GetUnresolvedProviderPathFromPSPath($ProjectRoot + ".shared-ci")

if (Test-Path -Path $TargetSharedCiDirectory)
{
    Remove-Item -Force -Recurse -Path $TargetSharedCiDirectory 
}

mkdir $TargetSharedCiDirectory

$SharedCiFile = Resolve-Path $(Join-Path $ProjectRoot "ci/shared-ci.pinned")
$SharedCiVersion = Get-Content $SharedCiFile

Push-Location $TargetSharedCiDirectory
    git init
    git remote add origin git@github.com:spatialos/gdk-for-unity-shared-ci.git
    git fetch --depth 20 origin master
    git checkout $SharedCiVersion
Pop-Location

# ====================== GDK for Unity ==================== #


$TargetGdkDirectory = $ExecutionContext.SessionState.Path.GetUnresolvedProviderPathFromPSPath($ProjectRoot + "\..\gdk-for-unity")

if (Test-Path -Path $TargetGdkDirectory)
{
    Remove-Item -Force -Recurse -Path $TargetGdkDirectory 
}

mkdir $TargetGdkDirectory

$PinnedGdkFile = Resolve-Path $(Join-Path $ProjectRoot "gdk.pinned")
$TargetGdkVersion = Get-Content $PinnedGdkFile

Push-Location $TargetGdkDirectory
    git init
    git remote add origin git@github.com:spatialos/gdk-for-unity.git
    git fetch --depth 20 origin feature/npm-test
    git checkout $TargetGdkVersion
    & ./init.ps1
Pop-Location


$PackageTarget = Resolve-Path $(Join-Path $ProjectRoot "/workers/unity/Packages")
$PackageSource = Resolve-Path $(Join-Path $TargetGdkDirectory "/workers/unity/Packages")

dotnet run -p ./.shared-ci/tools/PackageSymLinker/PackageSymLinker.csproj -- `
    --s $PackageSource `
    --t $PackageTarget `
