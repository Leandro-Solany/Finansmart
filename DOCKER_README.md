# ?? Docker Files - Finansmart

## ?? Arquivos Criados

### Arquivos Principais

1. **`docker-compose.yml`** - Configura��o base do Docker Compose
   - Aplica��o Finansmart (.NET 8)
   - Oracle Database Express 21c
   - Nginx reverse proxy
   - Portainer (gerenciamento)

2. **`Dockerfile`** - Build da aplica��o .NET
   - Multi-stage build otimizado
   - Suporta hot reload em dev
   - Produ��o otimizada

3. **`.env.example`** - Template de vari�veis de ambiente
   - Configura��es do Oracle
   - Configura��es da aplica��o
   - Portas dos servi�os

### Ambientes Espec�ficos

4. **`docker-compose.dev.yml`** - Desenvolvimento
   - Hot reload habilitado
   - Volumes montados para c�digo
   - Logs detalhados

5. **`docker-compose.prod.yml`** - Produ��o
   - M�ltiplas r�plicas
   - Limites de recursos
   - Healthchecks configurados

6. **`docker-compose.test.yml`** - Testes
   - Banco de dados em mem�ria
   - Execu��o autom�tica de testes
   - Cleanup autom�tico

### Scripts Auxiliares

7. **`docker-manager.sh`** - Script Bash para Linux/Mac
   - Comandos simplificados
   - Backup e restore
   - Gerenciamento de logs

8. **`docker-manager.ps1`** - Script PowerShell para Windows
   - Mesmas funcionalidades do .sh
   - Sintaxe PowerShell

### Configura��es

9. **`nginx/nginx.conf`** - Configura��o do Nginx
   - Reverse proxy
   - SSL ready
   - Health checks

10. **`init-scripts/`** - Scripts de inicializa��o do Oracle
    - `01-create-user.sql` - Cria usu�rio FINANSMART
    - `healthcheck.sql` - Verifica sa�de do banco

11. **`DOCKER_GUIDE.md`** - Guia completo de uso
    - Quick start
    - Comandos �teis
    - Troubleshooting
    - Best practices

## ?? Quick Start

### 1. Preparar Ambiente

```bash
# Copiar arquivo de exemplo
cp .env.example .env

# Editar com suas configura��es
notepad .env  # Windows
nano .env     # Linux/Mac
```

### 2. Iniciar (Desenvolvimento)

```bash
# Linux/Mac
./docker-manager.sh dev

# Windows PowerShell
.\docker-manager.ps1 dev

# Ou diretamente com docker-compose
docker-compose -f docker-compose.yml -f docker-compose.dev.yml up -d
```

### 3. Acessar

- **App**: http://localhost:8080
- **Swagger**: http://localhost:8080/swagger
- **Portainer**: http://localhost:9000
- **Oracle EM**: https://localhost:5500/em

## ?? Comandos R�pidos

### Usando Scripts (Recomendado)

```bash
# Linux/Mac
./docker-manager.sh [comando]

# Windows
.\docker-manager.ps1 [comando]
```

**Comandos dispon�veis:**
- `dev` - Iniciar desenvolvimento
- `prod` - Iniciar produ��o
- `stop` - Parar servi�os
- `clean` - Limpar tudo (CUIDADO!)
- `logs` - Ver logs
- `status` - Ver status
- `test` - Executar testes
- `backup` - Backup do banco
- `restore` - Restaurar backup
- `shell` - Abrir terminal

### Usando Docker Compose Diretamente

```bash
# Desenvolvimento
docker-compose -f docker-compose.yml -f docker-compose.dev.yml up -d

# Produ��o
docker-compose -f docker-compose.yml -f docker-compose.prod.yml up -d

# Testes
docker-compose -f docker-compose.yml -f docker-compose.test.yml up --abort-on-container-exit

# Parar
docker-compose down

# Ver logs
docker-compose logs -f

# Ver status
docker-compose ps
```

## ?? Configura��o

### Vari�veis de Ambiente (.env)

```env
# Oracle
ORACLE_PASSWORD=SuaSenhaSegura123
ORACLE_CONNECTION_STRING=User Id=FINANSMART;Password=FinansmartPassword123;Data Source=oracle-db:1521/XE;

# App
ASPNETCORE_ENVIRONMENT=Development
ASPNETCORE_URLS=http://+:8080

# Portas
APP_PORT=8080
ORACLE_PORT=1521
```

### Customizar Portas

Edite `docker-compose.yml`:

```yaml
services:
  finansmart-app:
    ports:
      - "8081:8080"  # Mudar porta do host
```

## ?? Estrutura de Servi�os

```
???????????????????????????????????????????
?         Nginx (Reverse Proxy)           ?
?         Port: 80, 443                   ?
???????????????????????????????????????????
               ?
               ?
???????????????????????????????????????????
?      Finansmart App (.NET 8)            ?
?      Port: 8080                         ?
?      Replicas: 1 (dev) / 2+ (prod)      ?
???????????????????????????????????????????
               ?
               ?
???????????????????????????????????????????
?      Oracle Database Express 21c         ?
?      Port: 1521 (SQL)                   ?
?      Port: 5500 (EM Express)            ?
???????????????????????????????????????????

    ????????????????????????????????????
    ?   Portainer (Gerenciamento)      ?
    ?   Port: 9000                     ?
    ????????????????????????????????????
```

## ?? Seguran�a

### Checklist

- [ ] Copiar `.env.example` para `.env`
- [ ] Alterar `ORACLE_PASSWORD` no `.env`
- [ ] N�o commitar `.env` no Git (j� est� no .gitignore)
- [ ] Configurar SSL no Nginx para produ��o
- [ ] Usar Docker secrets em produ��o
- [ ] Limitar recursos dos containers
- [ ] Fazer backup regular do Oracle

### SSL no Nginx

```bash
# Gerar certificado auto-assinado (dev)
mkdir -p nginx/ssl
openssl req -x509 -nodes -days 365 -newkey rsa:2048 \
  -keyout nginx/ssl/key.pem \
  -out nginx/ssl/cert.pem

# Descomentar se��o HTTPS no nginx/nginx.conf
```

## ?? Testes

```bash
# Executar todos os testes
./docker-manager.sh test

# Ou manualmente
docker-compose -f docker-compose.yml -f docker-compose.test.yml up --abort-on-container-exit

# Ver resultados
docker-compose logs finansmart-app
```

## ?? Backup e Restore

### Backup

```bash
# Usando script
./docker-manager.sh backup

# Ou manualmente
docker exec finansmart-oracle-db sh -c 'exp system/SuaSenha@XE file=/tmp/backup.dmp full=y'
docker cp finansmart-oracle-db:/tmp/backup.dmp ./backup.dmp
```

### Restore

```bash
# Usando script
./docker-manager.sh restore backup.dmp

# Ou manualmente
docker cp backup.dmp finansmart-oracle-db:/tmp/backup.dmp
docker exec finansmart-oracle-db sh -c 'imp system/SuaSenha@XE file=/tmp/backup.dmp full=y'
```

## ?? Troubleshooting

### Oracle n�o inicia

```bash
# Verificar mem�ria dispon�vel
docker stats

# Oracle precisa de pelo menos 2GB
# Aumentar em Docker Desktop > Settings > Resources > Memory
```

### Porta j� em uso

```bash
# Windows
netstat -ano | findstr :8080

# Linux/Mac
lsof -i :8080

# Mudar porta no docker-compose.yml
```

### Logs de erro

```bash
# Ver logs detalhados
./docker-manager.sh logs

# Ou de um servi�o espec�fico
./docker-manager.sh logs finansmart-app
./docker-manager.sh logs oracle-db
```

### Resetar tudo

```bash
# CUIDADO: Apaga todos os dados!
./docker-manager.sh clean

# Ou
docker-compose down -v
```

## ?? Performance

### Desenvolvimento

- Hot reload autom�tico
- Sem otimiza��es de build
- Logs detalhados

### Produ��o

- Build otimizado
- M�ltiplas r�plicas
- Limites de recursos
- Healthchecks ativos

## ?? Documenta��o Completa

Para mais detalhes, consulte:
- **`DOCKER_GUIDE.md`** - Guia completo de uso
- **Docker Docs**: https://docs.docker.com/
- **Docker Compose**: https://docs.docker.com/compose/
- **Oracle Container**: https://container-registry.oracle.com/

## ?? Suporte

1. Verificar logs: `./docker-manager.sh logs`
2. Verificar status: `./docker-manager.sh status`
3. Reiniciar: `./docker-manager.sh stop && ./docker-manager.sh dev`
4. Consultar `DOCKER_GUIDE.md` para troubleshooting detalhado

---

**Criado para**: Finansmart Project  
**Docker Version**: 20.10+  
**Compose Version**: 3.8  
**Status**: ? Pronto para uso
