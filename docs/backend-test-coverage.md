# Backend Test Coverage Report

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download) installed
- `reportgenerator` global tool installed (the script installs it automatically if missing):

```bash
dotnet tool install -g dotnet-reportgenerator-globaltool
```

---

## Quick Start — Automated Script

The easiest way is to use the provided PowerShell script from the solution root:

```powershell
cd src/backend/EduTrack
.\generate-coverage.ps1
```

The script:
1. Cleans previous results
2. Runs all tests with `XPlat Code Coverage` collection
3. Generates an HTML report (+ badges, JSON, CSV, Markdown summaries)
4. Opens the report in your default browser

---

## Script Options

```powershell
# Custom output directory and 90% threshold
.\generate-coverage.ps1 -OutputDir "Reports" -Threshold 90

# Generate without opening the browser
.\generate-coverage.ps1 -OpenReport $false
```

| Parameter | Default | Description |
|-----------|---------|-------------|
| `-OutputDir` | `CoverageReport` | Folder for generated HTML report |
| `-HistoryDir` | `CoverageHistory` | Folder for historical trend data |
| `-OpenReport` | `$true` | Open the report in the browser after generation |
| `-Threshold` | `80` | Minimum coverage % (build fails below this) |

---

## Manual Steps

### Step 1 — Run tests and collect coverage

```bash
cd src/backend/EduTrack
dotnet test --collect:"XPlat Code Coverage" \
            --results-directory ./TestResults \
            --settings coverlet.runsettings
```

Coverage XML files are saved under `TestResults/` (Cobertura format).

### Step 2 — Generate the HTML report

```bash
reportgenerator \
  -reports:"TestResults/**/coverage.cobertura.xml" \
  -targetdir:"CoverageReport" \
  -reporttypes:"Html;HtmlSummary;Badges" \
  -assemblyfilters:"+EduTrack.*"
```

### Step 3 — Open the report

Open `CoverageReport/index.html` in a browser.

---

## Coverage Scope

The `coverlet.runsettings` file configures what is included and excluded:

- **Included:** `EduTrack.Application`, `EduTrack.Domain`, `EduTrack.Infrastructure`, `EduTrack.Api`
- **Excluded:** test assemblies, `Migrations/`, `Program.cs`, generated code

---

## Report Formats Generated

| Format | File | Use |
|--------|------|-----|
| HTML | `CoverageReport/index.html` | Interactive browser view |
| HTML Summary | `CoverageReport/summary.html` | Quick overview |
| JSON Summary | `CoverageReport/Summary.json` | CI integration |
| Markdown | `CoverageReport/SummaryGithub.md` | GitHub PR comments |
| Badges | `CoverageReport/*.svg` | README badges |
