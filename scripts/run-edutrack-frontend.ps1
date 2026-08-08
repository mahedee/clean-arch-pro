#!/usr/bin/env pwsh
# Builds and runs the EduTrack Angular frontend

$ErrorActionPreference = "Stop"

# Refresh PATH from registry so npm/node are available in new terminal windows
$env:PATH = [System.Environment]::GetEnvironmentVariable('PATH', 'Machine') + ';' +
            [System.Environment]::GetEnvironmentVariable('PATH', 'User')

$root  = Split-Path $PSScriptRoot -Parent
$uiDir = Join-Path $root "frontend\edutrack-ui"

if (-not (Get-Command npm -ErrorAction SilentlyContinue)) {
    Write-Error "Node.js / npm not found. Install from https://nodejs.org"
    exit 1
}

Write-Host "==> Installing frontend dependencies..." -ForegroundColor Cyan
Set-Location $uiDir
npm install --prefer-offline --silent

Write-Host "==> Starting frontend (http://localhost:4200)..." -ForegroundColor Green
# Uses the local node_modules/.bin/ng via the npm start script
npm start
