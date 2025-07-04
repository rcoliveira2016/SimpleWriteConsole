param(
    [Parameter(Mandatory=$true)]
    [string]$ApiKey,

    [string]$RepoUrl = "https://github.com/rcoliveira2016/SimpleWriteConsole.git",
    [string]$LocalFolder = "../SimpleWriteConsole"
)

# Clonar repositório se não existir
if (-not (Test-Path $LocalFolder)) {
    Write-Host "Clonando repositório $RepoUrl..."
    git clone $RepoUrl $LocalFolder
} else {
    Write-Host "Pasta '$LocalFolder' já existe. Pulando clone."
}

# Entrar na pasta
Set-Location $LocalFolder

# Restaurar dependências
Write-Host "Restaurando dependências..."
dotnet restore

# Compilar projeto em Release
Write-Host "Compilando projeto..."
dotnet build -c Release

# Gerar pacote NuGet
Write-Host "Gerando pacote NuGet..."
dotnet pack -c Release -o ./nupkg

# Publicar pacote(s)
$packageFiles = Get-ChildItem -Path "./nupkg" -Filter *.nupkg
if ($packageFiles.Count -eq 0) {
    Write-Error "Nenhum pacote .nupkg encontrado para publicar."
    exit 1
}

foreach ($pkg in $packageFiles) {
    Write-Host "Publicando pacote $($pkg.Name)..."
    Write-Host "Api Key: '$ApiKey'"
    dotnet nuget push $pkg.FullName -k $ApiKey --source https://api.nuget.org/v3/index.json --skip-duplicate
}

Write-Host "Processo finalizado com sucesso."
