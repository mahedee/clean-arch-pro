#!/usr/bin/env pwsh

<#
.SYNOPSIS
    Clone a GitHub issue by creating a new issue with the same title and body.

.AUTHOR
    Mahedee Hasan

.DESCRIPTION
    This script uses GitHub CLI to retrieve an existing issue and create a new one
    with the same content, adding a note that it was cloned from the original.
    After successful creation, it closes the original issue with a comment referencing the new issue.

.PARAMETER IssueNumber
    The issue number to clone.

.PARAMETER Repository
    The repository in format "owner/repo". Defaults to "mahedee/clean-arch-pro".

.EXAMPLE
    .\clone_issue.ps1 -IssueNumber 7
    
.EXAMPLE
    .\clone_issue.ps1 -IssueNumber 7 -Repository "username/repository"

.EXAMPLE
    .\clone_issue.ps1 9
#>

param(
    [Parameter(Mandatory=$true)]
    [int]$IssueNumber,
    
    [Parameter(Mandatory=$false)]
    [string]$Repository = "mahedee/clean-arch-pro"
)

# Check if GitHub CLI is available
try {
    $null = Get-Command gh -ErrorAction Stop
} catch {
    Write-Error "GitHub CLI (gh) is not found. Please install GitHub CLI and ensure it's in your PATH."
    exit 1
}

try {
    Write-Host "Retrieving issue #$IssueNumber from repository $Repository..."
    
    # Get issue title
    $title = gh issue view $IssueNumber -R $Repository --json title -q .title
    if ($LASTEXITCODE -ne 0) {
        Write-Error "Failed to retrieve issue #$IssueNumber. Please check if the issue exists and you have access to the repository."
        exit 1
    }
    
    # Get issue body
    $body = gh issue view $IssueNumber -R $Repository --json body -q .body
    if ($LASTEXITCODE -ne 0) {
        Write-Error "Failed to retrieve issue body for #$IssueNumber."
        exit 1
    }
    
    # Create the cloned issue body with reference to original
    $clonedBody = "$body`n`nCloned from #$IssueNumber"
    
    Write-Host "Creating new issue with title: $title"
    
    # Create new issue and capture the result
    $createOutput = gh issue create -R $Repository --title "$title" --body "$clonedBody" 2>&1
    
    if ($LASTEXITCODE -eq 0) {
        # Extract issue number from URL (format: https://github.com/owner/repo/issues/123)
        if ($createOutput -match '/issues/(\d+)$') {
            $newIssueNumber = $matches[1]
            Write-Host "Successfully created new issue #$newIssueNumber!" -ForegroundColor Green
            
            # Close the original issue with a comment
            Write-Host "Closing original issue #$IssueNumber..."
            $closeComment = "Created new issue #$newIssueNumber"
            
            gh issue close $IssueNumber -R $Repository --comment "$closeComment"
            
            if ($LASTEXITCODE -eq 0) {
                Write-Host "Successfully closed original issue #$IssueNumber with reference to new issue #$newIssueNumber!" -ForegroundColor Green
            } else {
                Write-Warning "New issue #$newIssueNumber was created, but failed to close the original issue #$IssueNumber."
            }
        } else {
            Write-Host "New issue created successfully, but couldn't extract issue number from output: $createOutput" -ForegroundColor Yellow
            Write-Warning "Please manually close issue #$IssueNumber if needed."
        }
    } else {
        Write-Error "Failed to create the cloned issue. Error: $createOutput"
        exit 1
    }
    
} catch {
    Write-Error "An error occurred: $($_.Exception.Message)"
    exit 1
}