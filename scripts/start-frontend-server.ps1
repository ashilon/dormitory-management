$ErrorActionPreference = 'Stop'

Set-Location "$PSScriptRoot\..\Frontend"

if (Get-Command python -ErrorAction SilentlyContinue) {
    Write-Host "Starting frontend server on http://localhost:5500 using python..."
    python -m http.server 5500
    exit $LASTEXITCODE
}

if (Get-Command py -ErrorAction SilentlyContinue) {
    Write-Host "Starting frontend server on http://localhost:5500 using py..."
    py -m http.server 5500
    exit $LASTEXITCODE
}

Write-Error "Python was not found (python/py). Install Python or use VS Code Live Server."
exit 1
