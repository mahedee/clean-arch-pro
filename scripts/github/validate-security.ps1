# GitHub Security Validation Script
# This script validates that sensitive files are properly ignored by git

Write-Host "`n🔐 GitHub Security Validation" -ForegroundColor Cyan
Write-Host "=============================" -ForegroundColor Cyan

$securityResults = @()

# Files that MUST be ignored
$criticalFiles = @(
    "scripts/github/config/github-token.json",
    "scripts/github/github-issues.log"
)

# Test each critical file
foreach ($file in $criticalFiles) {
    Write-Host "`n📝 Testing: $file" -ForegroundColor Yellow
    
    if (Test-Path $file) {
        # File exists, check if it's ignored
        $ignored = git check-ignore $file 2>$null
        if ($ignored) {
            Write-Host "   ✅ SECURE: File exists and is properly ignored" -ForegroundColor Green
            $securityResults += @{File = $file; Status = "SECURE"; Message = "Ignored by git"}
        } else {
            Write-Host "   ❌ RISK: File exists but is NOT ignored!" -ForegroundColor Red
            $securityResults += @{File = $file; Status = "RISK"; Message = "File is tracked by git"}
        }
    } else {
        Write-Host "   ℹ️  INFO: File does not exist (OK)" -ForegroundColor Blue
        $securityResults += @{File = $file; Status = "OK"; Message = "File does not exist"}
    }
}

# Test git status for any sensitive patterns
Write-Host "`n🔍 Checking git status for sensitive files..." -ForegroundColor Yellow
$gitStatus = git status --porcelain
$sensitivePatterns = @("token", "secret", "key", "password", "credential")

$sensitiveFound = $false
foreach ($line in $gitStatus) {
    foreach ($pattern in $sensitivePatterns) {
        if ($line -match $pattern) {
            Write-Host "   ⚠️  WARNING: Potential sensitive file in git: $line" -ForegroundColor Yellow
            $sensitiveFound = $true
        }
    }
}

if (-not $sensitiveFound) {
    Write-Host "   ✅ No sensitive files detected in git status" -ForegroundColor Green
}

# Summary
Write-Host "`n📊 Security Validation Summary" -ForegroundColor Cyan
Write-Host "===============================" -ForegroundColor Cyan

$secureCount = ($securityResults | Where-Object {$_.Status -eq "SECURE" -or $_.Status -eq "OK"}).Count
$riskCount = ($securityResults | Where-Object {$_.Status -eq "RISK"}).Count

foreach ($result in $securityResults) {
    $color = switch ($result.Status) {
        "SECURE" { "Green" }
        "OK" { "Blue" }
        "RISK" { "Red" }
        default { "White" }
    }
    Write-Host "   $($result.Status): $($result.File) - $($result.Message)" -ForegroundColor $color
}

Write-Host "`n🎯 Overall Security Status:" -ForegroundColor Cyan
if ($riskCount -eq 0) {
    Write-Host "   ✅ SECURE: All sensitive files are properly protected" -ForegroundColor Green
} else {
    Write-Host "   ❌ RISK: $riskCount sensitive files are not properly protected!" -ForegroundColor Red
}

Write-Host "`n💡 Security Tips:" -ForegroundColor Cyan
Write-Host "   - Never commit files containing tokens, passwords, or API keys" -ForegroundColor Gray
Write-Host "   - Always use .gitignore to exclude sensitive configuration files" -ForegroundColor Gray
Write-Host "   - Use environment variables or secure vaults for production secrets" -ForegroundColor Gray
Write-Host "   - Regularly audit your repository for accidentally committed secrets" -ForegroundColor Gray

Write-Host "`n🔒 Security validation completed!" -ForegroundColor Green
