# =====================================================================
# Local SonarQube Code Analysis Script for Task Management System
# =====================================================================

param (
    [string]$SonarUrl   = "http://localhost:9000",
    [string]$SonarToken = ""
)

Write-Host "==========================================================" -ForegroundColor Cyan
Write-Host " Starting SonarQube Analysis for Task Management System " -ForegroundColor Cyan
Write-Host "==========================================================" -ForegroundColor Cyan

# 1. Check if dotnet-sonarscanner is installed
$sonarInstalled = Get-Command dotnet-sonarscanner -ErrorAction SilentlyContinue

if (-not $sonarInstalled) {
    Write-Host "`nInstalling dotnet-sonarscanner global tool..." -ForegroundColor Yellow
    dotnet tool install --global dotnet-sonarscanner
} else {
    Write-Host "`ndotnet-sonarscanner is already installed." -ForegroundColor Green
}

# 2. Prompts if SonarToken is empty
if ([string]::IsNullOrWhiteSpace($SonarToken)) {
    $SonarToken = Read-Host -Prompt "Enter your SonarQube / SonarCloud User Token"
}

# 3. Begin Sonar Analysis
Write-Host "`n[Step 1/4] Starting SonarScanner..." -ForegroundColor Cyan
dotnet-sonarscanner begin `
    /k:"TaskManagementSystem" `
    /n:"Task Management System (.NET Core & React)" `
    /v:"1.0" `
    /d:sonar.host.url="$SonarUrl" `
    /d:sonar.token="$SonarToken" `
    /d:sonar.cs.opencover.reportsPaths="**/coverage.opencover.xml" `
    /d:sonar.exclusions="**/bin/**,**/obj/**,**/node_modules/**,**/Migrations/**"

# 4. Build Solution
Write-Host "`n[Step 2/4] Building .NET Solution..." -ForegroundColor Cyan
dotnet build TaskManagement.sln --configuration Release

# 5. Run Unit Tests with Code Coverage
Write-Host "`n[Step 3/4] Running xUnit Tests & Collecting Code Coverage..." -ForegroundColor Cyan
dotnet test TaskManagement.Tests/TaskManagement.Tests.csproj --configuration Release /p:CollectCoverage=true /p:CoverletOutputFormat=opencover

# 6. End Sonar Analysis
Write-Host "`n[Step 4/4] Uploading Results to SonarQube Server..." -ForegroundColor Cyan
dotnet-sonarscanner end /d:sonar.token="$SonarToken"

Write-Host "`n==========================================================" -ForegroundColor Green
Write-Host " SonarQube Analysis Completed Successfully!               " -ForegroundColor Green
Write-Host " View results at: $SonarUrl/dashboard?id=TaskManagementSystem" -ForegroundColor Green
Write-Host "==========================================================" -ForegroundColor Green
