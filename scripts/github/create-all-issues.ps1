# Batch Create GitHub Issues
# This script creates all remaining sample issues

param(
    [Parameter(Mandatory=$false)]
    [switch]$DryRun
)

$scriptPath = $PSScriptRoot
$mainScript = Join-Path $scriptPath "create-github-issue.ps1"

# List of remaining issues to create
$issuesToCreate = @(
    "doc-student-api.json",
    "enhancement-error-handling.json"
)

Write-Host "`n🚀 Batch Creating GitHub Issues" -ForegroundColor Cyan
Write-Host "=================================" -ForegroundColor Cyan

$createdIssues = @()
$failedIssues = @()

foreach ($issueFile in $issuesToCreate) {
    Write-Host "`n📝 Creating issue: $issueFile" -ForegroundColor Yellow
    
    try {
        if ($DryRun) {
            $result = & $mainScript -IssueFile $issueFile -DryRun
        } else {
            $result = & $mainScript -IssueFile $issueFile
        }
        
        if ($LASTEXITCODE -eq 0) {
            $createdIssues += $issueFile
            Write-Host "✅ Success: $issueFile" -ForegroundColor Green
        } else {
            $failedIssues += $issueFile
            Write-Host "❌ Failed: $issueFile" -ForegroundColor Red
        }
    } catch {
        $failedIssues += $issueFile
        Write-Host "❌ Error creating $issueFile`: $($_.Exception.Message)" -ForegroundColor Red
    }
    
    # Small delay between requests to be nice to GitHub API
    Start-Sleep -Seconds 2
}

# Summary
Write-Host "`n📊 Batch Creation Summary" -ForegroundColor Cyan
Write-Host "=========================" -ForegroundColor Cyan
Write-Host "Total Issues: $($issuesToCreate.Count)" -ForegroundColor White
Write-Host "Created Successfully: $($createdIssues.Count)" -ForegroundColor Green
Write-Host "Failed: $($failedIssues.Count)" -ForegroundColor Red

if ($createdIssues.Count -gt 0) {
    Write-Host "`n✅ Successfully Created:" -ForegroundColor Green
    foreach ($issue in $createdIssues) {
        Write-Host "  - $issue" -ForegroundColor Green
    }
}

if ($failedIssues.Count -gt 0) {
    Write-Host "`n❌ Failed Issues:" -ForegroundColor Red
    foreach ($issue in $failedIssues) {
        Write-Host "  - $issue" -ForegroundColor Red
    }
}

Write-Host "`n🎉 Batch creation completed!" -ForegroundColor Cyan
