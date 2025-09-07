# GitHub Issue Creator - Enhanced Configuration-Driven System
# Created: 2024-01-15
# Purpose: Create GitHub issues using templates and configuration

param(
    [Parameter(Mandatory=$false)]
    [string]$ConfigFile = ".\config\config.json",
    
    [Parameter(Mandatory=$false)]
    [string]$IssueFile,
    
    [Parameter(Mandatory=$false)]
    [string]$Template,
    
    [Parameter(Mandatory=$false)]
    [switch]$DryRun,
    
    [Parameter(Mandatory=$false)]
    [switch]$ListTemplates,
    
    [Parameter(Mandatory=$false)]
    [switch]$ListIssues,
    
    [Parameter(Mandatory=$false)]
    [switch]$ValidateOnly
)

# Set error action preference
$ErrorActionPreference = "Stop"

# Import required modules
try {
    Import-Module PowerShellGet -Force -ErrorAction SilentlyContinue
} catch {
    Write-Warning "PowerShellGet module not available"
}

# Function to write colored output
function Write-ColorOutput {
    param(
        [string]$Message,
        [string]$Color = "White",
        [switch]$NoNewline
    )
    
    if ($PSVersionTable.PSVersion.Major -ge 7) {
        if ($NoNewline) {
            Write-Host $Message -ForegroundColor $Color -NoNewline
        } else {
            Write-Host $Message -ForegroundColor $Color
        }
    } else {
        Write-Host $Message
    }
}

# Function to log messages
function Write-Log {
    param(
        [string]$Message,
        [string]$Level = "INFO",
        [switch]$NoConsole
    )
    
    $timestamp = Get-Date -Format "yyyy-MM-dd HH:mm:ss"
    $logMessage = "[$timestamp] [$Level] $Message"
    
    if ($config.logging.enabled -and $config.logging.logFile) {
        Add-Content -Path $config.logging.logFile -Value $logMessage
    }
    
    if (-not $NoConsole) {
        switch ($Level) {
            "ERROR" { Write-ColorOutput $logMessage -Color "Red" }
            "WARN"  { Write-ColorOutput $logMessage -Color "Yellow" }
            "INFO"  { Write-ColorOutput $logMessage -Color "Green" }
            "DEBUG" { if ($VerbosePreference -eq 'Continue') { Write-ColorOutput $logMessage -Color "Gray" } }
            default { Write-ColorOutput $logMessage }
        }
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

# Function to validate GitHub token
function Test-GitHubToken {
    param([string]$Token)
    
    Write-Log "Validating GitHub token..."
    
    if ($config.security.validateToken) {
        if ($Token.Length -lt $config.security.tokenMinLength) {
            throw "Token length is below minimum required: $($config.security.tokenMinLength)"
        }
    }
    
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

# Function to load template
function Get-IssueTemplate {
    param([string]$TemplateName)
    
    $templatePath = Join-Path $PSScriptRoot $config.issue.templateDirectory $TemplateName
    
    Write-Log "Loading template: $templatePath"
    
    if (-not (Test-Path $templatePath)) {
        throw "Template not found: $templatePath"
    }
    
    try {
        $template = Get-Content $templatePath -Raw | ConvertFrom-Json
        Write-Log "Template loaded successfully: $TemplateName"
        Write-Log "DEBUG: Template title: '$($template.title)'" -Level "DEBUG" 
        Write-Log "DEBUG: Template body length: $($template.body.Length)" -Level "DEBUG"
        return $template
    } catch {
        throw "Failed to parse template file: $($_.Exception.Message)"
    }
}

# Function to load issue definition
function Get-IssueDefinition {
    param([string]$IssueFileName)
    
    $issuePath = Join-Path $PSScriptRoot $config.issue.issueDirectory $IssueFileName
    
    Write-Log "Loading issue definition: $issuePath"
    
    if (-not (Test-Path $issuePath)) {
        throw "Issue definition not found: $issuePath"
    }
    
    try {
        $issue = Get-Content $issuePath -Raw | ConvertFrom-Json
        Write-Log "Issue definition loaded successfully: $IssueFileName"
        return $issue
    } catch {
        throw "Failed to parse issue definition: $($_.Exception.Message)"
    }
}

# Function to substitute variables in template
function Expand-Template {
    param(
        [object]$Template,
        [object]$Variables
    )
    
    Write-Log "Expanding template with variables..."
    
    # Convert Variables to hashtable if it's a PSCustomObject
    if ($Variables -is [PSCustomObject]) {
        $variableHash = @{}
        $Variables.PSObject.Properties | ForEach-Object {
            $variableHash[$_.Name] = $_.Value
        }
        $Variables = $variableHash
    }
    
    # Create a copy of the template and directly substitute variables
    $expandedTemplate = @{
        title = $Template.title
        body = $Template.body
        labels = @($Template.labels)
        assignees = @($Template.assignees)
        milestone = $Template.milestone
    }
    
    Write-Log "DEBUG: Initial title: '$($expandedTemplate.title)'" -Level "DEBUG"
    Write-Log "DEBUG: Initial body length: $($expandedTemplate.body.Length)" -Level "DEBUG"
    
    # Add automatic variables
    $Variables["timestamp"] = (Get-Date -Format "yyyy-MM-dd HH:mm:ss UTC")
    
    # Replace variables in title
    foreach ($key in $Variables.Keys) {
        $placeholder = "{{$key}}"
        $value = $Variables[$key]
        
        if ($expandedTemplate.title -match [regex]::Escape($placeholder)) {
            $expandedTemplate.title = $expandedTemplate.title -replace [regex]::Escape($placeholder), $value
        }
        
        if ($expandedTemplate.body -match [regex]::Escape($placeholder)) {
            $expandedTemplate.body = $expandedTemplate.body -replace [regex]::Escape($placeholder), $value
        }
        
        # Handle labels array
        for ($i = 0; $i -lt $expandedTemplate.labels.Count; $i++) {
            if ($expandedTemplate.labels[$i] -match [regex]::Escape($placeholder)) {
                $expandedTemplate.labels[$i] = $expandedTemplate.labels[$i] -replace [regex]::Escape($placeholder), $value
            }
        }
    }
    
    Write-Log "Template expansion completed"
    
    # Convert to PSCustomObject for consistency
    return [PSCustomObject]$expandedTemplate
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
    
    $body = @{
        title = $IssueData.title
        body = $IssueData.body
        labels = $IssueData.labels
        assignees = $IssueData.assignees
    }
    
    if ($IssueData.milestone) {
        $body.milestone = $IssueData.milestone
    }
    
    $bodyJson = $body | ConvertTo-Json -Depth 10
    
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

# Function to list available templates
function Show-Templates {
    Write-ColorOutput "`nAvailable Templates:" -Color "Cyan"
    Write-ColorOutput "===================" -Color "Cyan"
    
    $templateDir = Join-Path $PSScriptRoot $config.issue.templateDirectory
    
    if (Test-Path $templateDir) {
        $templates = Get-ChildItem $templateDir -Filter "*.json"
        foreach ($template in $templates) {
            Write-ColorOutput "  - $($template.Name)" -Color "Yellow"
        }
    } else {
        Write-ColorOutput "  No templates found in: $templateDir" -Color "Red"
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

# Function to validate issue data
function Test-IssueData {
    param([object]$IssueData)
    
    Write-Log "Validating issue data..."
    
    $errors = @()
    
    if ($config.validation.requireTitle -and (-not $IssueData.title -or $IssueData.title.Trim() -eq "")) {
        $errors += "Title is required"
    }
    
    if ($config.validation.requireBody -and (-not $IssueData.body -or $IssueData.body.Trim() -eq "")) {
        $errors += "Body is required"
    }
    
    if ($IssueData.title -and $IssueData.title.Length -lt $config.validation.minTitleLength) {
        $errors += "Title is too short (minimum: $($config.validation.minTitleLength))"
    }
    
    if ($IssueData.title -and $IssueData.title.Length -gt $config.validation.maxTitleLength) {
        $errors += "Title is too long (maximum: $($config.validation.maxTitleLength))"
    }
    
    if ($IssueData.body -and $IssueData.body.Length -lt $config.validation.minBodyLength) {
        $errors += "Body is too short (minimum: $($config.validation.minBodyLength))"
    }
    
    if (-not $config.validation.allowEmptyLabels -and (-not $IssueData.labels -or $IssueData.labels.Count -eq 0)) {
        $errors += "Labels are required"
    }
    
    if ($errors.Count -gt 0) {
        Write-Log "Validation failed:" -Level "ERROR"
        foreach ($error in $errors) {
            Write-Log "  - $error" -Level "ERROR"
        }
        return $false
    }
    
    Write-Log "Issue data validation passed"
    return $true
}

# Main execution
try {
    Write-ColorOutput "`n🚀 GitHub Issue Creator - Enhanced System" -Color "Cyan"
    Write-ColorOutput "=========================================" -Color "Cyan"
    
    # Load configuration
    $config = Get-Configuration -ConfigPath $ConfigFile
    
    # Handle list operations
    if ($ListTemplates) {
        Show-Templates
        exit 0
    }
    
    if ($ListIssues) {
        Show-Issues
        exit 0
    }
    
    # Validate required parameters
    if (-not $IssueFile -and -not $Template) {
        Write-Log "Either -IssueFile or -Template parameter is required" -Level "ERROR"
        Write-ColorOutput "`nUsage Examples:" -Color "Yellow"
        Write-ColorOutput "  .\create-github-issue.ps1 -IssueFile 'task-t001a-completion.json'" -Color "Gray"
        Write-ColorOutput "  .\create-github-issue.ps1 -Template 'default-issue.json'" -Color "Gray"
        Write-ColorOutput "  .\create-github-issue.ps1 -ListTemplates" -Color "Gray"
        Write-ColorOutput "  .\create-github-issue.ps1 -ListIssues" -Color "Gray"
        exit 1
    }
    
    # Get GitHub token
    $token = Get-GitHubToken
    
    # Validate token
    if (-not (Test-GitHubToken -Token $token)) {
        throw "GitHub token validation failed"
    }
    
    # Process issue creation
    if ($IssueFile) {
        # Load issue definition
        $issueDefinition = Get-IssueDefinition -IssueFileName $IssueFile
        
        # Load template
        $template = Get-IssueTemplate -TemplateName $issueDefinition.template
        
        # Expand template with variables
        $issueData = Expand-Template -Template $template -Variables $issueDefinition.variables
        
        # Debug the expanded data
        Write-Log "DEBUG: Expanded title: '$($issueData.title)'" -Level "DEBUG"
        Write-Log "DEBUG: Expanded body length: $($issueData.body.Length)" -Level "DEBUG"
        Write-Log "DEBUG: Expanded labels count: $($issueData.labels.Count)" -Level "DEBUG"
        
        # Add additional labels if specified
        if ($issueDefinition.additionalLabels) {
            if ($issueData.labels) {
                $currentLabels = @($issueData.labels)
                $additionalLabels = @($issueDefinition.additionalLabels)
                $issueData.labels = $currentLabels + $additionalLabels
            } else {
                $issueData | Add-Member -NotePropertyName "labels" -NotePropertyValue @($issueDefinition.additionalLabels) -Force
            }
        }
        
        # Remove duplicate labels if labels exist
        if ($issueData.labels) {
            $issueData.labels = $issueData.labels | Select-Object -Unique
        }
        
    } elseif ($Template) {
        # Load template only (interactive mode would go here)
        $template = Get-IssueTemplate -TemplateName $Template
        Write-ColorOutput "Template loaded. Interactive variable input not implemented yet." -Color "Yellow"
        Write-ColorOutput "Template variables: $($template.variables -join ', ')" -Color "Gray"
        exit 0
    }
    
    # Validate issue data
    if ($ValidateOnly) {
        $isValid = Test-IssueData -IssueData $issueData
        if ($isValid) {
            Write-ColorOutput "✅ Issue data validation passed" -Color "Green"
        } else {
            Write-ColorOutput "❌ Issue data validation failed" -Color "Red"
            exit 1
        }
        exit 0
    }
    
    if (-not (Test-IssueData -IssueData $issueData)) {
        throw "Issue data validation failed"
    }
    
    # Create the issue
    $result = New-GitHubIssue -IssueData $issueData -Token $token
    
    # Output results
    if ($config.output.exportResults) {
        $resultsFile = Join-Path $PSScriptRoot $config.output.resultsFile
        $result | ConvertTo-Json -Depth 10 | Out-File $resultsFile
        Write-Log "Results exported to: $resultsFile"
    }
    
    Write-ColorOutput "`n✅ Issue creation completed successfully!" -Color "Green"
    Write-ColorOutput "Issue #$($result.number): $($result.html_url)" -Color "Cyan"
    
} catch {
    Write-Log "Script execution failed: $($_.Exception.Message)" -Level "ERROR"
    Write-ColorOutput "`n❌ Error: $($_.Exception.Message)" -Color "Red"
    exit 1
}
