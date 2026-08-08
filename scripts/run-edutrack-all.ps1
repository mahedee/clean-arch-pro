#!/usr/bin/env pwsh
# Starts both the EduTrack backend and frontend in separate terminal windows.
# Run this script from the repository root or the scripts folder.

$ErrorActionPreference = "Stop"
$scripts = $PSScriptRoot

function Start-InNewWindow {
    param([string]$Title, [string]$Script)

    $args = "-NoExit -NoProfile -ExecutionPolicy Bypass -File `"$Script`""
    Start-Process pwsh -ArgumentList $args -WindowStyle Normal `
        -ErrorAction Stop
    Write-Host "  Started: $Title" -ForegroundColor Green
}

Write-Host ""
Write-Host "===========================================" -ForegroundColor Cyan
Write-Host "  EduTrack — Starting All Applications"     -ForegroundColor Cyan
Write-Host "===========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "  Backend  -> http://localhost:6100"
Write-Host "  Swagger  -> http://localhost:6100/swagger"
Write-Host "  Frontend -> http://localhost:4200"
Write-Host ""

Start-InNewWindow "EduTrack Backend"  (Join-Path $scripts "run-edutrack-backend.ps1")

# Brief pause so the backend begins startup before the frontend opens
Start-Sleep -Seconds 2

Start-InNewWindow "EduTrack Frontend" (Join-Path $scripts "run-edutrack-frontend.ps1")

Write-Host ""
Write-Host "Both applications are starting in separate windows." -ForegroundColor Yellow
Write-Host "Press Ctrl+C in each window to stop them."          -ForegroundColor Yellow
Write-Host ""
