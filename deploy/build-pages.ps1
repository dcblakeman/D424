param(
    [string]$OutputDir = "public"
)

$ErrorActionPreference = "Stop"

$repoRoot = Split-Path -Parent $PSScriptRoot
$siteRoot = Join-Path $PSScriptRoot "site"
$outputPath = if ([System.IO.Path]::IsPathRooted($OutputDir)) {
    $OutputDir
} else {
    Join-Path $repoRoot $OutputDir
}

$assetsPath = Join-Path $outputPath "assets"
$templatePath = Join-Path $siteRoot "index.template.html"
$stylesPath = Join-Path $siteRoot "styles.css"
$iconSource = Join-Path $repoRoot "Resources\AppIcon\appicon.png"
$wireframeSource = Join-Path $repoRoot "Docs\Wireframes\Courses Screen@1x.png"

if (Test-Path $outputPath) {
    Remove-Item -Recurse -Force $outputPath
}

New-Item -ItemType Directory -Path $assetsPath -Force | Out-Null
Copy-Item $stylesPath (Join-Path $outputPath "styles.css")
Copy-Item $iconSource (Join-Path $assetsPath "appicon.png")

if (Test-Path $wireframeSource) {
    Copy-Item $wireframeSource (Join-Path $assetsPath "courses-screen.png")
}

$commit = if ($env:CI_COMMIT_SHORT_SHA) {
    $env:CI_COMMIT_SHORT_SHA
} else {
    git -C $repoRoot rev-parse --short HEAD
}

$timestamp = [DateTime]::UtcNow.ToString("yyyy-MM-dd HH:mm 'UTC'")
$deploymentUrl = "https://wgu-gitlab-environment.gitlab.io/student-repos/dcblakeman/d424-software-engineering-capstone/"
$repoUrl = "https://gitlab.com/wgu-gitlab-environment/student-repos/dcblakeman/d424-software-engineering-capstone"
$releaseUrl = "https://github.com/dcblakeman/D424/releases/tag/v1.0.2"
$apkUrl = "https://github.com/dcblakeman/D424/releases/download/v1.0.2/com.dcblakeman.collegecoursetracker-Signed.apk"

$html = Get-Content $templatePath -Raw
$html = $html.Replace("{{COMMIT}}", $commit.Trim())
$html = $html.Replace("{{BUILD_TIMESTAMP}}", $timestamp)
$html = $html.Replace("{{DEPLOYMENT_URL}}", $deploymentUrl)
$html = $html.Replace("{{REPO_URL}}", $repoUrl)
$html = $html.Replace("{{RELEASE_URL}}", $releaseUrl)
$html = $html.Replace("{{APK_URL}}", $apkUrl)

Set-Content -Path (Join-Path $outputPath "index.html") -Value $html -NoNewline

$notFound = @"
<!doctype html>
<html lang="en">
<head>
  <meta charset="utf-8">
  <meta http-equiv="refresh" content="0; url=./index.html">
  <title>College Course Tracker</title>
</head>
<body>
  <p>Redirecting to the College Course Tracker deployment page...</p>
</body>
</html>
"@
Set-Content -Path (Join-Path $outputPath "404.html") -Value $notFound -NoNewline
Set-Content -Path (Join-Path $outputPath "health.txt") -Value "College Course Tracker deployment generated from commit $($commit.Trim()) at $timestamp"
