$ErrorActionPreference = "Stop"

Write-Host ""
Write-Host "========================================"
Write-Host " AI Banking Workspace - Validation"
Write-Host "========================================"
Write-Host ""

if (-not (Test-Path ".\AiWorkspace.sln")) {
    throw "AiWorkspace.sln was not found."
}

Write-Host "[1/4] Checking .NET SDK..."
dotnet --version

if ($LASTEXITCODE -ne 0) {
    throw ".NET SDK verification failed."
}

Write-Host "[2/4] Restoring dependencies..."
dotnet restore .\AiWorkspace.sln

if ($LASTEXITCODE -ne 0) {
    throw "Dependency restore failed."
}

Write-Host "[3/4] Building solution..."
dotnet build .\AiWorkspace.sln --no-restore

if ($LASTEXITCODE -ne 0) {
    throw "Solution build failed."
}

Write-Host "[4/4] Running tests..."
dotnet test .\AiWorkspace.sln --no-build

if ($LASTEXITCODE -ne 0) {
    throw "Test execution failed."
}

Write-Host ""
Write-Host "========================================"
Write-Host " Validation completed successfully!"
Write-Host "========================================"
