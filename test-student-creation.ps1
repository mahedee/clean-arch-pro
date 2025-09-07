# Test student creation API
$headers = @{
    "Content-Type" = "application/json"
}

$body = @'
{
  "fullName": "Mahedee Hasan",
  "dateOfBirth": "2001-09-06T20:00:17.125Z",
  "email": "mh@gmail.com",
  "phoneNumber": "+1-437-224-3287",
  "address": {
    "street": "ddd",
    "city": "dd",
    "state": "dd",
    "zipCode": "12345",
    "country": "Canada"
  }
}
'@

try {
    Write-Host "Creating student via API..."
    $response = Invoke-RestMethod -Uri "http://localhost:5152/api/students" -Method POST -Headers $headers -Body $body
    Write-Host "Student created successfully!" -ForegroundColor Green
    $response | ConvertTo-Json -Depth 3
} catch {
    Write-Host "Error creating student:" -ForegroundColor Red
    Write-Host $_.Exception.Message
    if ($_.Exception.Response) {
        $reader = New-Object System.IO.StreamReader($_.Exception.Response.GetResponseStream())
        $responseBody = $reader.ReadToEnd()
        Write-Host "Response body: $responseBody"
    }
}
