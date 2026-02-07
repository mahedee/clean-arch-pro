#!/usr/bin/env pwsh

<#
.SYNOPSIS
    Generates comprehensive code coverage reports for the EduTrack solution
.DESCRIPTION
    This script runs all tests with coverage collection, generates HTML reports,
    and opens the results in the default browser.
.PARAMETER OutputDir
    Directory where coverage reports will be generated (default: ./CoverageReport)
.PARAMETER HistoryDir
    Directory to store coverage history for trend analysis (default: ./CoverageHistory)
.PARAMETER OpenReport
    Whether to open the generated report in the browser (default: true)
.PARAMETER Threshold
    Minimum coverage threshold percentage (default: 80)
.EXAMPLE
    .\generate-coverage.ps1
    Generates coverage report with default settings
.EXAMPLE
    .\generate-coverage.ps1 -Threshold 90 -OutputDir "Reports"
    Generates coverage report with 90% threshold in Reports directory
#>

param(
    [string]$OutputDir = "CoverageReport",
    [string]$HistoryDir = "CoverageHistory", 
    [bool]$OpenReport = $true,
    [int]$Threshold = 80
)

Write-Host "🧪 EduTrack Code Coverage Report Generator" -ForegroundColor Cyan
Write-Host "=========================================" -ForegroundColor Cyan

# Check if we're in the correct directory
if (-not (Test-Path "EduTrack.sln")) {
    Write-Error "❌ EduTrack.sln not found. Please run this script from the backend/EduTrack directory."
    exit 1
}

# Clean previous reports
if (Test-Path $OutputDir) {
    Write-Host "🧹 Cleaning previous reports..." -ForegroundColor Yellow
    Remove-Item -Recurse -Force $OutputDir
}

if (Test-Path "TestResults") {
    Write-Host "🧹 Cleaning previous test results..." -ForegroundColor Yellow
    Remove-Item -Recurse -Force "TestResults"
}

# Check if ReportGenerator is installed
try {
    $null = Get-Command "reportgenerator" -ErrorAction Stop
    Write-Host "✅ ReportGenerator found" -ForegroundColor Green
} catch {
    Write-Host "📦 Installing ReportGenerator..." -ForegroundColor Yellow
    dotnet tool install -g dotnet-reportgenerator-globaltool
    if ($LASTEXITCODE -ne 0) {
        Write-Error "❌ Failed to install ReportGenerator"
        exit 1
    }
}

# Run tests with coverage collection
Write-Host "`n🧪 Running tests with coverage collection..." -ForegroundColor Green
Write-Host "Threshold: $Threshold%" -ForegroundColor Gray

$testCommand = "dotnet test --collect:`"XPlat Code Coverage`" --results-directory ./TestResults --verbosity normal --configuration Release"
Write-Host "Command: $testCommand" -ForegroundColor Gray

Invoke-Expression $testCommand

if ($LASTEXITCODE -ne 0) {
    Write-Error "❌ Tests failed or coverage collection failed"
    exit 1
}

# Find coverage files
Write-Host "`n🔍 Searching for coverage files..." -ForegroundColor Green
$coverageFiles = Get-ChildItem -Recurse -Include "coverage.cobertura.xml" -Path "./TestResults"

if ($coverageFiles.Count -eq 0) {
    Write-Error "❌ No coverage files found! Make sure tests ran successfully."
    exit 1
}

Write-Host "Found $($coverageFiles.Count) coverage file(s):" -ForegroundColor Green
foreach ($file in $coverageFiles) {
    Write-Host "  📄 $($file.FullName)" -ForegroundColor Gray
}

# Get Git information for tagging
$gitHash = ""
$gitBranch = ""
try {
    $gitHash = git rev-parse --short HEAD 2>$null
    $gitBranch = git rev-parse --abbrev-ref HEAD 2>$null
    if ($gitHash) {
        Write-Host "Git: $gitBranch @ $gitHash" -ForegroundColor Gray
    }
} catch {
    Write-Host "⚠️  Git information not available" -ForegroundColor Yellow
}

# Generate comprehensive reports
Write-Host "`n📊 Generating coverage reports..." -ForegroundColor Green

$reportTypes = @(
    "Html",
    "HtmlSummary", 
    "Badges",
    "JsonSummary",
    "CsvSummary",
    "MarkdownSummaryGithub"
)

$coverageReports = ($coverageFiles | ForEach-Object { $_.FullName }) -join ";"
$tag = if ($gitHash) { "$gitBranch-$gitHash" } else { Get-Date -Format "yyyy-MM-dd-HHmm" }

$reportCommand = @(
    "reportgenerator",
    "-reports:`"$coverageReports`"",
    "-targetdir:`"$OutputDir`"",
    "-reporttypes:`"$($reportTypes -join ';')`"",
    "-sourcedirs:`"src`"",
    "-historydir:`"$HistoryDir`"",
    "-title:`"EduTrack Code Coverage Report`"",
    "-tag:`"$tag`"",
    "-assemblyfilters:`"+EduTrack.*`"",
    "-classfilters:`"-*.Tests;-*.Test`""
)

Write-Host "ReportGenerator command:" -ForegroundColor Gray
Write-Host ($reportCommand -join " ") -ForegroundColor Gray

$reportResult = & $reportCommand[0] $reportCommand[1..($reportCommand.Length-1)]

if ($LASTEXITCODE -ne 0) {
    Write-Error "❌ Failed to generate coverage report"
    exit 1
}

# Parse coverage summary
$summaryFile = Join-Path $OutputDir "Summary.json"
if (Test-Path $summaryFile) {
    try {
        $summary = Get-Content $summaryFile | ConvertFrom-Json
        $lineCoverage = [math]::Round($summary.coverage.linecoverage, 2)
        $branchCoverage = [math]::Round($summary.coverage.branchcoverage, 2)
        $methodCoverage = [math]::Round($summary.coverage.methodcoverage, 2)
        
        Write-Host "`n📈 Coverage Summary:" -ForegroundColor Green
        Write-Host "  📏 Line Coverage:   $lineCoverage%" -ForegroundColor $(if ($lineCoverage -ge $Threshold) { "Green" } else { "Red" })
        Write-Host "  🌿 Branch Coverage: $branchCoverage%" -ForegroundColor $(if ($branchCoverage -ge $Threshold) { "Green" } else { "Yellow" })
        Write-Host "  🎯 Method Coverage: $methodCoverage%" -ForegroundColor $(if ($methodCoverage -ge $Threshold) { "Green" } else { "Yellow" })
        
        if ($lineCoverage -lt $Threshold) {
            Write-Host "`n⚠️  Line coverage ($lineCoverage%) is below threshold ($Threshold%)" -ForegroundColor Red
        } else {
            Write-Host "`n✅ Coverage meets threshold requirements!" -ForegroundColor Green
        }
    } catch {
        Write-Host "⚠️  Could not parse coverage summary" -ForegroundColor Yellow
    }
}

# List generated files
Write-Host "`n📁 Generated Reports:" -ForegroundColor Green
$reportFiles = @(
    "index.html",
    "Summary.json", 
    "badge_linecoverage.svg",
    "badge_branchcoverage.svg",
    "badge_methodcoverage.svg"
)

foreach ($file in $reportFiles) {
    $filePath = Join-Path $OutputDir $file
    if (Test-Path $filePath) {
        Write-Host "  📄 $file" -ForegroundColor Gray
    }
}

# Open report in browser
$indexPath = Join-Path $OutputDir "index.html"
if ($OpenReport -and (Test-Path $indexPath)) {
    Write-Host "`n🌐 Opening coverage report..." -ForegroundColor Green
    
    try {
        if ($IsWindows) {
            Start-Process $indexPath
        } elseif ($IsMacOS) {
            & open $indexPath
        } else {
            & xdg-open $indexPath
        }
        Write-Host "✅ Report opened in default browser" -ForegroundColor Green
    } catch {
        Write-Host "⚠️  Could not open browser automatically" -ForegroundColor Yellow
        Write-Host "📂 Report location: $indexPath" -ForegroundColor Gray
    }
} else {
    Write-Host "📂 Report location: $indexPath" -ForegroundColor Gray
}

# Show next steps
Write-Host "`n🎯 Next Steps:" -ForegroundColor Cyan
Write-Host "  1. Review the HTML report for detailed coverage analysis" -ForegroundColor Gray
Write-Host "  2. Identify uncovered lines (highlighted in red)" -ForegroundColor Gray
Write-Host "  3. Add tests for critical uncovered code paths" -ForegroundColor Gray
Write-Host "  4. Re-run this script to track improvement" -ForegroundColor Gray

if (Test-Path (Join-Path $OutputDir "badge_linecoverage.svg")) {
    Write-Host "`n🏆 Coverage Badges:" -ForegroundColor Cyan
    Write-Host "  Add these badges to your README.md:" -ForegroundColor Gray
    Write-Host "  ![Line Coverage](./CoverageReport/badge_linecoverage.svg)" -ForegroundColor Gray
    Write-Host "  ![Branch Coverage](./CoverageReport/badge_branchcoverage.svg)" -ForegroundColor Gray
}

Write-Host "`n✅ Coverage report generation completed successfully!" -ForegroundColor Green
