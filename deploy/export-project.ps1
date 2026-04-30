param(
    [string]$OutputZip = ""
)

$ErrorActionPreference = "Stop"

$repoRoot = Split-Path -Parent $PSScriptRoot
$defaultOutput = Join-Path $repoRoot "deploy\artifacts\CollegeCourseTracker-Task4-Export.zip"
$zipPath = if ($OutputZip) { $OutputZip } else { $defaultOutput }
$zipDir = Split-Path -Parent $zipPath

New-Item -ItemType Directory -Path $zipDir -Force | Out-Null

if (Test-Path $zipPath) {
    Remove-Item $zipPath -Force
}

git -C $repoRoot archive --format=zip --output=$zipPath HEAD
Write-Host "Created project export at $zipPath"
