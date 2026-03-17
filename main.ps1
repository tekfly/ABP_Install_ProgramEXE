$ProgressPreference = 'SilentlyContinue'

# 1. Define os caminhos
$downloadsPath = Join-Path $env:USERPROFILE "Downloads\UiPath"
$exePath = Join-Path $downloadsPath "ABP_Installer.exe"

# 2. Verifica se a pasta existe, se não, cria-a
if (-not (Test-Path -Path $downloadsPath)) {
    Write-Host "A criar a pasta: $downloadsPath" -ForegroundColor Yellow
    New-Item -ItemType Directory -Path $downloadsPath -Force
}
# 3. URL do executável (Certifica-te que este link é o "Direct Download" do Release)
$url = "https://github.com/tekfly/ABP_Install_ProgramEXE/releases/download/Prerelease/ABP_Install_ProgramEXE.exe"

Write-Host "A descarregar o instalador para a pasta Downloads\UiPath..." -ForegroundColor Green

# 4. Faz o download
curl.exe -L -o "$exePath" "$url"

# 5. Executa o programa e espera que ele feche
$process = Start-Process -FilePath $exePath -Verb RunAs -PassThru -Wait

# 6. Remove o executável após fechar (opcional)
#if ($process.HasExited) {
#    Remove-Item $exePath -Force -ErrorAction SilentlyContinue
#}
