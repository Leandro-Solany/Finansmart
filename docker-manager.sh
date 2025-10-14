#!/bin/bash

# Script de gerenciamento Docker para Finansmart
# Uso: ./docker-manager.sh [comando]

set -e

# Cores para output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Função para printar com cor
print_info() {
    echo -e "${BLUE}[INFO]${NC} $1"
}

print_success() {
    echo -e "${GREEN}[SUCCESS]${NC} $1"
}

print_warning() {
    echo -e "${YELLOW}[WARNING]${NC} $1"
}

print_error() {
    echo -e "${RED}[ERROR]${NC} $1"
}

# Verificar se Docker está instalado
check_docker() {
    if ! command -v docker &> /dev/null; then
        print_error "Docker não está instalado!"
        exit 1
    fi
    
    if ! docker info &> /dev/null; then
        print_error "Docker daemon não está rodando!"
        exit 1
    fi
    
    print_success "Docker está rodando"
}

# Iniciar ambiente de desenvolvimento
dev_start() {
    print_info "Iniciando ambiente de desenvolvimento..."
    docker-compose -f docker-compose.yml -f docker-compose.dev.yml up -d
    print_success "Ambiente de desenvolvimento iniciado!"
    print_info "Aplicação: http://localhost:8080"
    print_info "Swagger: http://localhost:8080/swagger"
}

# Iniciar ambiente de produção
prod_start() {
    print_info "Iniciando ambiente de produção..."
    docker-compose -f docker-compose.yml -f docker-compose.prod.yml up -d
    print_success "Ambiente de produção iniciado!"
}

# Parar todos os serviços
stop() {
    print_info "Parando todos os serviços..."
    docker-compose down
    print_success "Serviços parados"
}

# Parar e remover volumes
clean() {
    print_warning "ATENÇÃO: Isso vai remover todos os dados!"
    read -p "Tem certeza? (s/N) " -n 1 -r
    echo
    if [[ $REPLY =~ ^[Ss]$ ]]; then
        print_info "Limpando ambiente..."
        docker-compose down -v
        print_success "Ambiente limpo"
    else
        print_info "Operação cancelada"
    fi
}

# Ver logs
logs() {
    SERVICE=${1:-}
    if [ -z "$SERVICE" ]; then
        docker-compose logs -f
    else
        docker-compose logs -f "$SERVICE"
    fi
}

# Ver status
status() {
    docker-compose ps
}

# Executar testes
test() {
    print_info "Executando testes..."
    docker-compose -f docker-compose.yml -f docker-compose.test.yml up --abort-on-container-exit
    print_success "Testes concluídos"
}

# Rebuild
rebuild() {
    print_info "Reconstruindo imagens..."
    docker-compose build --no-cache
    print_success "Imagens reconstruídas"
}

# Backup do banco
backup() {
    BACKUP_FILE="backup_$(date +%Y%m%d_%H%M%S).dmp"
    print_info "Criando backup: $BACKUP_FILE"
    docker exec finansmart-oracle-db sh -c 'exp system/${ORACLE_PASSWORD:-OraclePassword123}@XE file=/tmp/backup.dmp full=y'
    docker cp finansmart-oracle-db:/tmp/backup.dmp "./$BACKUP_FILE"
    print_success "Backup criado: $BACKUP_FILE"
}

# Restore do banco
restore() {
    BACKUP_FILE=$1
    if [ -z "$BACKUP_FILE" ]; then
        print_error "Especifique o arquivo de backup"
        exit 1
    fi
    
    if [ ! -f "$BACKUP_FILE" ]; then
        print_error "Arquivo não encontrado: $BACKUP_FILE"
        exit 1
    fi
    
    print_info "Restaurando backup: $BACKUP_FILE"
    docker cp "$BACKUP_FILE" finansmart-oracle-db:/tmp/backup.dmp
    docker exec finansmart-oracle-db sh -c 'imp system/${ORACLE_PASSWORD:-OraclePassword123}@XE file=/tmp/backup.dmp full=y'
    print_success "Backup restaurado"
}

# Shell interativo
shell() {
    SERVICE=${1:-finansmart-app}
    print_info "Abrindo shell em $SERVICE..."
    docker exec -it "$SERVICE" bash
}

# Mostrar ajuda
show_help() {
    cat << EOF
Finansmart Docker Manager

Uso: ./docker-manager.sh [comando] [argumentos]

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
    ./docker-manager.sh dev
    ./docker-manager.sh logs finansmart-app
    ./docker-manager.sh restore backup_20240101_120000.dmp
    ./docker-manager.sh shell oracle-db

EOF
}

# Main
check_docker

case "${1:-help}" in
    dev)
        dev_start
        ;;
    prod)
        prod_start
        ;;
    stop)
        stop
        ;;
    clean)
        clean
        ;;
    logs)
        logs "${2:-}"
        ;;
    status)
        status
        ;;
    test)
        test
        ;;
    rebuild)
        rebuild
        ;;
    backup)
        backup
        ;;
    restore)
        restore "${2:-}"
        ;;
    shell)
        shell "${2:-}"
        ;;
    help|*)
        show_help
        ;;
esac
