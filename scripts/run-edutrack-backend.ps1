#!/usr/bin/env pwsh
# Builds and runs the EduTrack backend API

$ErrorActionPreference = "Stop"

# Refresh PATH from registry so dotnet is available in new terminal windows
$env:PATH = [System.Environment]::GetEnvironmentVariable('PATH', 'Machine') + ';' +
            [System.Environment]::GetEnvironmentVariable('PATH', 'User')

$root       = Split-Path $PSScriptRoot -Parent
$apiProject = Join-Path $root "backend\EduTrack\src\EduTrack.Api\EduTrack.Api.csproj"
$solution   = Join-Path $root "backend\EduTrack\EduTrack.sln"

Write-Host "==> Building backend..." -ForegroundColor Cyan
dotnet build $solution -c Release --nologo -v quiet
if ($LASTEXITCODE -ne 0) { Write-Error "Backend build failed."; exit 1 }

Write-Host "==> Starting backend API (http://localhost:6100)..." -ForegroundColor Green
dotnet run --project $apiProject --launch-profile "http" --no-build
