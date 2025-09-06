# GitHub Issue Creator - Simplified Version
# Created: 2025-09-06
# Purpose: Create GitHub issues with simplified template system

param(
    [Parameter(Mandatory=$false)]
    [string]$ConfigFile = ".\config\config.json",
    
    [Parameter(Mandatory=$false)]
    [string]$IssueFile,
    
    [Parameter(Mandatory=$false)]
    [switch]$DryRun,
    
    [Parameter(Mandatory=$false)]
    [switch]$ListIssues
)

# Set error action preference
$ErrorActionPreference = "Stop"

# Function to write colored output
function Write-ColorOutput {
    param(
        [string]$Message,
        [string]$Color = "White"
    )
    
    if ($PSVersionTable.PSVersion.Major -ge 7) {
        Write-Host $Message -ForegroundColor $Color
    } else {
        Write-Host $Message
    }
}

# Function to log messages
function Write-Log {
    param(
        [string]$Message,
        [string]$Level = "INFO"
    )
    
    $timestamp = Get-Date -Format "yyyy-MM-dd HH:mm:ss"
    $logMessage = "[$timestamp] [$Level] $Message"
    
    switch ($Level) {
        "ERROR" { Write-ColorOutput $logMessage -Color "Red" }
        "WARN"  { Write-ColorOutput $logMessage -Color "Yellow" }
        "INFO"  { Write-ColorOutput $logMessage -Color "Green" }
        default { Write-ColorOutput $logMessage }
    }
}

# Function to load configuration
function Get-Configuration {
    param([string]$ConfigPath)
    
    Write-Log "Loading configuration from: $ConfigPath"
    
    if (-not (Test-Path $ConfigPath)) {
        throw "Configuration file not found: $ConfigPath"
    }
    
    try {
        $configContent = Get-Content $ConfigPath -Raw | ConvertFrom-Json
        Write-Log "Configuration loaded successfully"
        return $configContent
    } catch {
        throw "Failed to parse configuration file: $($_.Exception.Message)"
    }
}

# Function to get GitHub token
function Get-GitHubToken {
    $tokenPath = Join-Path $PSScriptRoot $config.github.tokenFile
    
    if (Test-Path $tokenPath) {
        Write-Log "Reading token from file: $tokenPath"
        
        # Check if it's a JSON file
        if ($config.github.tokenFormat -eq "json") {
            try {
                $tokenJson = Get-Content $tokenPath -Raw | ConvertFrom-Json
                $tokenProperty = $config.github.tokenProperty
                if ($tokenJson.$tokenProperty) {
                    $token = $tokenJson.$tokenProperty.Trim()
                    Write-Log "Token loaded from JSON property: $tokenProperty"
                    return $token
                } else {
                    throw "Token property '$tokenProperty' not found in JSON file"
                }
            } catch {
                throw "Failed to parse JSON token file: $($_.Exception.Message)"
            }
        } else {
            # Plain text file
            $token = (Get-Content $tokenPath -Raw).Trim()
            return $token
        }
    }
    
    # Try environment variable
    $envToken = $env:GITHUB_TOKEN
    if ($envToken) {
        Write-Log "Using token from environment variable"
        return $envToken
    }
    
    throw "GitHub token not found. Please create '$tokenPath' or set GITHUB_TOKEN environment variable"
}

# Function to validate GitHub token
function Test-GitHubToken {
    param([string]$Token)
    
    Write-Log "Validating GitHub token..."
    
    try {
        $headers = @{
            "Authorization" = "Bearer $Token"
            "Accept" = $config.github.apiVersion
            "User-Agent" = "EduTrack-IssueCreator/1.0"
        }
        
        $response = Invoke-RestMethod -Uri "$($config.github.apiUrl)/user" -Headers $headers -Method Get
        Write-Log "Token validation successful. Authenticated as: $($response.login)" -Level "INFO"
        return $true
    } catch {
        Write-Log "Token validation failed: $($_.Exception.Message)" -Level "ERROR"
        return $false
    }
}

# Function to load and process simplified issue definition
function Get-SimpleIssueData {
    param([string]$IssueFileName)
    
    $issuePath = Join-Path $PSScriptRoot $config.issue.issueDirectory $IssueFileName
    
    Write-Log "Loading issue definition: $issuePath"
    
    if (-not (Test-Path $issuePath)) {
        throw "Issue definition not found: $issuePath"
    }
    
    try {
        $issueDefinition = Get-Content $issuePath -Raw | ConvertFrom-Json
        Write-Log "Issue definition loaded successfully: $IssueFileName"
        
        # Create simple issue data structure
        $issueData = @{
            title = $issueDefinition.title
            body = $issueDefinition.body
            labels = @($issueDefinition.labels)
            assignees = @($issueDefinition.assignees)
        }
        
        if ($issueDefinition.milestone) {
            $issueData.milestone = $issueDefinition.milestone
        }
        
        Write-Log "Issue data processed successfully"
        return $issueData
        
    } catch {
        throw "Failed to parse issue definition: $($_.Exception.Message)"
    }
}

# Function to create GitHub issue
function New-GitHubIssue {
    param(
        [object]$IssueData,
        [string]$Token
    )
    
    Write-Log "Creating GitHub issue: $($IssueData.title)"
    
    $headers = @{
        "Authorization" = "Bearer $Token"
        "Accept" = $config.github.apiVersion
        "Content-Type" = "application/json"
        "User-Agent" = "EduTrack-IssueCreator/1.0"
    }
    
    $uri = "$($config.github.apiUrl)/repos/$($config.github.owner)/$($config.github.repository)/issues"
    
    $bodyJson = $IssueData | ConvertTo-Json -Depth 10
    
    if ($DryRun) {
        Write-Log "DRY RUN - Would create issue with data:" -Level "INFO"
        Write-ColorOutput $bodyJson -Color "Cyan"
        return @{ number = "DRY-RUN"; html_url = "https://github.com/dry-run" }
    }
    
    try {
        $response = Invoke-RestMethod -Uri $uri -Headers $headers -Method Post -Body $bodyJson
        Write-Log "Issue created successfully: #$($response.number)" -Level "INFO"
        Write-Log "Issue URL: $($response.html_url)" -Level "INFO"
        return $response
    } catch {
        Write-Log "Failed to create issue: $($_.Exception.Message)" -Level "ERROR"
        throw
    }
}

# Function to list available issues
function Show-Issues {
    Write-ColorOutput "`nAvailable Issue Definitions:" -Color "Cyan"
    Write-ColorOutput "=============================" -Color "Cyan"
    
    $issueDir = Join-Path $PSScriptRoot $config.issue.issueDirectory
    
    if (Test-Path $issueDir) {
        $issues = Get-ChildItem $issueDir -Filter "*.json"
        foreach ($issue in $issues) {
            Write-ColorOutput "  - $($issue.Name)" -Color "Yellow"
        }
    } else {
        Write-ColorOutput "  No issue definitions found in: $issueDir" -Color "Red"
    }
}

# Main execution
try {
    Write-ColorOutput "`n🚀 GitHub Issue Creator - Simplified System" -Color "Cyan"
    Write-ColorOutput "===========================================" -Color "Cyan"
    
    # Load configuration
    $config = Get-Configuration -ConfigPath $ConfigFile
    
    # Handle list operations
    if ($ListIssues) {
        Show-Issues
        exit 0
    }
    
    # Validate required parameters
    if (-not $IssueFile) {
        Write-Log "IssueFile parameter is required" -Level "ERROR"
        Write-ColorOutput "`nUsage Examples:" -Color "Yellow"
        Write-ColorOutput "  .\create-issue-simple.ps1 -IssueFile 'simple-bug.json'" -Color "Gray"
        Write-ColorOutput "  .\create-issue-simple.ps1 -IssueFile 'simple-feature.json' -DryRun" -Color "Gray"
        Write-ColorOutput "  .\create-issue-simple.ps1 -ListIssues" -Color "Gray"
        exit 1
    }
    
    # Get GitHub token
    $token = Get-GitHubToken
    
    # Validate token
    if (-not (Test-GitHubToken -Token $token)) {
        throw "GitHub token validation failed"
    }
    
    # Load and process issue
    $issueData = Get-SimpleIssueData -IssueFileName $IssueFile
    
    # Create the issue
    $result = New-GitHubIssue -IssueData $issueData -Token $token
    
    Write-ColorOutput "`n✅ Issue creation completed successfully!" -Color "Green"
    Write-ColorOutput "Issue #$($result.number): $($result.html_url)" -Color "Cyan"
    
} catch {
    Write-Log "Script execution failed: $($_.Exception.Message)" -Level "ERROR"
    Write-ColorOutput "`n❌ Error: $($_.Exception.Message)" -Color "Red"
    exit 1
}
