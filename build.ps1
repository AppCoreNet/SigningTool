[CmdletBinding()]
param (
  [Parameter(ValueFromRemainingArguments=$True)]
  [string[]] $Arguments
)

$ErrorActionPreference = "Stop"

$ToolsPath = Join-Path $PSScriptRoot "build" -Resolve | Join-Path -ChildPath "tools"
$Cake = Join-Path $ToolsPath "dotnet-cake"
$CakeVersion = "0.37.0"
$CakeArgs = @("--paths_tools=$ToolsPath")

function Exec {
  param ([ScriptBlock] $ScriptBlock)
  & $ScriptBlock
  If ($LASTEXITCODE -ne 0) {
    Write-Error "Execution failed with exit code $LASTEXITCODE"
  }
}

# bootstrap cake
If (!(Test-Path "$ToolsPath/.store/cake.tool")) {
  Exec { & dotnet tool install --tool-path $ToolsPath Cake.Tool --version $CakeVersion }
  Exec { & $Cake @CakeArgs --bootstrap }
}

# build
Exec { & $Cake @CakeArgs @Arguments }
