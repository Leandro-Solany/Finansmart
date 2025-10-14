# Script de gerenciamento Docker para Finansmart (PowerShell)
# Uso: .\docker-manager.ps1 [comando]

param(
    [Parameter(Position=0)]
    [string]$Command = "help",
    
    [Parameter(Position=1)]
    [string]$Argument = ""
)

# Cores para output
function Write-Info {
    param([string]$Message)
    Write-Host "[INFO] $Message" -ForegroundColor Blue
}

function Write-Success {
    param([string]$Message)
    Write-Host "[SUCCESS] $Message" -ForegroundColor Green
}

function Write-Warning-Custom {
    param([string]$Message)
    Write-Host "[WARNING] $Message" -ForegroundColor Yellow
}

function Write-Error-Custom {
    param([string]$Message)
    Write-Host "[ERROR] $Message" -ForegroundColor Red
}

# Verificar se Docker está instalado
function Test-Docker {
    try {
        docker info | Out-Null
        Write-Success "Docker está rodando"
        return $true
    }
    catch {
        Write-Error-Custom "Docker não está instalado ou não está rodando!"
        return $false
    }
}

# Iniciar ambiente de desenvolvimento
function Start-Dev {
    Write-Info "Iniciando ambiente de desenvolvimento..."
    docker-compose -f docker-compose.yml -f docker-compose.dev.yml up -d
    Write-Success "Ambiente de desenvolvimento iniciado!"
    Write-Info "Aplicação: http://localhost:8080"
    Write-Info "Swagger: http://localhost:8080/swagger"
}

# Iniciar ambiente de produção
function Start-Prod {
    Write-Info "Iniciando ambiente de produção..."
    docker-compose -f docker-compose.yml -f docker-compose.prod.yml up -d
    Write-Success "Ambiente de produção iniciado!"
}

# Parar todos os serviços
function Stop-Services {
    Write-Info "Parando todos os serviços..."
    docker-compose down
    Write-Success "Serviços parados"
}

# Parar e remover volumes
function Clean-Environment {
    Write-Warning-Custom "ATENÇÃO: Isso vai remover todos os dados!"
    $confirmation = Read-Host "Tem certeza? (s/N)"
    if ($confirmation -eq 's' -or $confirmation -eq 'S') {
        Write-Info "Limpando ambiente..."
        docker-compose down -v
        Write-Success "Ambiente limpo"
    }
    else {
        Write-Info "Operação cancelada"
    }
}

# Ver logs
function Show-Logs {
    param([string]$Service)
    
    if ([string]::IsNullOrEmpty($Service)) {
        docker-compose logs -f
    }
    else {
        docker-compose logs -f $Service
    }
}

# Ver status
function Show-Status {
    docker-compose ps
}

# Executar testes
function Run-Tests {
    Write-Info "Executando testes..."
    docker-compose -f docker-compose.yml -f docker-compose.test.yml up --abort-on-container-exit
    Write-Success "Testes concluídos"
}

# Rebuild
function Rebuild-Images {
    Write-Info "Reconstruindo imagens..."
    docker-compose build --no-cache
    Write-Success "Imagens reconstruídas"
}

# Backup do banco
function Backup-Database {
    $backupFile = "backup_$(Get-Date -Format 'yyyyMMdd_HHmmss').dmp"
    Write-Info "Criando backup: $backupFile"
    
    docker exec finansmart-oracle-db sh -c 'exp system/${ORACLE_PASSWORD:-OraclePassword123}@XE file=/tmp/backup.dmp full=y'
    docker cp finansmart-oracle-db:/tmp/backup.dmp ".\$backupFile"
    
    Write-Success "Backup criado: $backupFile"
}

# Restore do banco
function Restore-Database {
    param([string]$BackupFile)
    
    if ([string]::IsNullOrEmpty($BackupFile)) {
        Write-Error-Custom "Especifique o arquivo de backup"
        return
    }
    
    if (-not (Test-Path $BackupFile)) {
        Write-Error-Custom "Arquivo não encontrado: $BackupFile"
        return
    }
    
    Write-Info "Restaurando backup: $BackupFile"
    docker cp $BackupFile finansmart-oracle-db:/tmp/backup.dmp
    docker exec finansmart-oracle-db sh -c 'imp system/${ORACLE_PASSWORD:-OraclePassword123}@XE file=/tmp/backup.dmp full=y'
    
    Write-Success "Backup restaurado"
}

# Shell interativo
function Open-Shell {
    param([string]$Service = "finansmart-app")
    
    Write-Info "Abrindo shell em $Service..."
    docker exec -it $Service bash
}

# Mostrar ajuda
function Show-Help {
    @"
Finansmart Docker Manager (PowerShell)

Uso: .\docker-manager.ps1 [comando] [argumentos]

Comandos:
    dev         Iniciar ambiente de desenvolvimento
    prod        Iniciar ambiente de produção
    stop        Parar todos os serviços
    clean       Parar e remover volumes (CUIDADO!)
    logs        Ver logs (opcional: especificar serviço)
    status      Ver status dos containers
    test        Executar testes
    rebuild     Reconstruir imagens
    backup      Criar backup do banco de dados
    restore     Restaurar backup (requer arquivo)
    shell       Abrir shell em container (opcional: especificar serviço)
    help        Mostrar esta ajuda

Exemplos:
    .\docker-manager.ps1 dev
    .\docker-manager.ps1 logs finansmart-app
    .\docker-manager.ps1 restore backup_20240101_120000.dmp
    .\docker-manager.ps1 shell oracle-db

"@
}

# Main
if (-not (Test-Docker)) {
    exit 1
}

switch ($Command.ToLower()) {
    "dev" {
        Start-Dev
    }
    "prod" {
        Start-Prod
    }
    "stop" {
        Stop-Services
    }
    "clean" {
        Clean-Environment
    }
    "logs" {
        Show-Logs -Service $Argument
    }
    "status" {
        Show-Status
    }
    "test" {
        Run-Tests
    }
    "rebuild" {
        Rebuild-Images
    }
    "backup" {
        Backup-Database
    }
    "restore" {
        Restore-Database -BackupFile $Argument
    }
    "shell" {
        Open-Shell -Service $Argument
    }
    default {
        Show-Help
    }
}
