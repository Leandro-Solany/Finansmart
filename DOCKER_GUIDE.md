# ?? Guia Docker & Docker Compose - Finansmart

## ?? Pré-requisitos

1. **Docker Desktop** instalado
   - Windows: https://docs.docker.com/desktop/install/windows-install/
   - Mac: https://docs.docker.com/desktop/install/mac-install/
   - Linux: https://docs.docker.com/desktop/install/linux-install/

2. **Docker Compose** (já incluído no Docker Desktop)

3. **Conta Oracle** (para baixar imagem do Oracle Database)
   - Acesse: https://container-registry.oracle.com/
   - Faça login e aceite os termos da imagem `database/express`

## ?? Quick Start

### 1. Configurar Variáveis de Ambiente

```bash
# Copiar arquivo de exemplo
cp .env.example .env

# Editar .env com suas configurações
# No Windows:
notepad .env

# No Linux/Mac:
nano .env
```

### 2. Login no Oracle Container Registry

```bash
docker login container-registry.oracle.com
# Username: seu-email-oracle
# Password: sua-senha-oracle
```

### 3. Iniciar Todos os Serviços

```bash
# Iniciar em background
docker-compose up -d

# Ou iniciar com logs visíveis
docker-compose up
```

### 4. Verificar Status

```bash
# Ver todos os containers
docker-compose ps

# Ver logs
docker-compose logs -f

# Ver logs de um serviço específico
docker-compose logs -f finansmart-app
```

### 5. Acessar a Aplicação

- **Aplicação**: http://localhost:8080
- **Swagger**: http://localhost:8080/swagger
- **Nginx (proxy)**: http://localhost
- **Oracle EM Express**: https://localhost:5500/em
- **Portainer**: http://localhost:9000

## ?? Serviços Incluídos

### 1. finansmart-app
**Aplicação .NET 8**
- Porta: 8080
- Connection string configurada automaticamente
- Restart automático em caso de falha

### 2. oracle-db
**Oracle Database Express Edition 21c**
- Porta: 1521 (SQL)
- Porta: 5500 (Enterprise Manager)
- Usuário padrão: system
- Senha: definida no .env
- Healthcheck automático

### 3. nginx
**Reverse Proxy**
- Porta: 80 (HTTP)
- Porta: 443 (HTTPS - se configurado SSL)
- Load balancing
- SSL termination

### 4. portainer
**Gerenciamento de Containers**
- Porta: 9000
- Interface web para gerenciar containers
- Dashboard com métricas

## ?? Comandos Úteis

### Gerenciamento de Containers

```bash
# Parar todos os serviços
docker-compose down

# Parar e remover volumes (ATENÇÃO: apaga dados!)
docker-compose down -v

# Reiniciar um serviço específico
docker-compose restart finansmart-app

# Reconstruir imagens
docker-compose build

# Reconstruir e iniciar
docker-compose up --build -d

# Ver uso de recursos
docker stats
```

### Logs e Debugging

```bash
# Logs em tempo real
docker-compose logs -f

# Logs dos últimos 100 linhas
docker-compose logs --tail=100

# Logs de um serviço específico
docker-compose logs finansmart-app

# Entrar no container
docker exec -it finansmart-app bash

# Entrar no Oracle
docker exec -it finansmart-oracle-db sqlplus system/OraclePassword123@XE
```

### Banco de Dados

```bash
# Executar migrations (dentro do container)
docker exec -it finansmart-app dotnet ef database update

# Backup do banco Oracle
docker exec finansmart-oracle-db sh -c 'exp system/OraclePassword123@XE file=/tmp/backup.dmp full=y'
docker cp finansmart-oracle-db:/tmp/backup.dmp ./backup.dmp

# Restore do banco
docker cp ./backup.dmp finansmart-oracle-db:/tmp/backup.dmp
docker exec finansmart-oracle-db sh -c 'imp system/OraclePassword123@XE file=/tmp/backup.dmp full=y'
```

## ?? Configuração de Produção

### 1. Usar Docker Secrets

```yaml
# docker-compose.prod.yml
version: '3.8'
services:
  finansmart-app:
    secrets:
      - oracle_password
    environment:
      - ConnectionStrings__DatabaseConnection=file:/run/secrets/oracle_password

secrets:
  oracle_password:
    file: ./secrets/oracle_password.txt
```

### 2. Configurar SSL no Nginx

```bash
# Gerar certificado auto-assinado (desenvolvimento)
mkdir -p nginx/ssl
openssl req -x509 -nodes -days 365 -newkey rsa:2048 \
  -keyout nginx/ssl/key.pem \
  -out nginx/ssl/cert.pem

# Para produção, use Let's Encrypt
# https://letsencrypt.org/
```

### 3. Limitar Recursos

```yaml
# Adicionar ao docker-compose.yml
services:
  finansmart-app:
    deploy:
      resources:
        limits:
          cpus: '1.0'
          memory: 512M
        reservations:
          cpus: '0.5'
          memory: 256M
```

## ?? Monitoramento

### Healthchecks

```bash
# Verificar health de todos os containers
docker-compose ps

# Testar healthcheck manualmente
curl http://localhost:8080/health
```

### Métricas com Portainer

1. Acesse: http://localhost:9000
2. Crie uma conta admin
3. Conecte ao Docker local
4. Visualize métricas, logs e gerenciamento

## ?? Workflow de Desenvolvimento

### 1. Desenvolvimento Local

```bash
# Iniciar apenas o banco de dados
docker-compose up -d oracle-db

# Rodar a aplicação localmente
dotnet run

# Acessar: http://localhost:5000
```

### 2. Desenvolvimento com Hot Reload

```yaml
# docker-compose.dev.yml
services:
  finansmart-app:
    volumes:
      - .:/app
      - /app/bin
      - /app/obj
    environment:
      - DOTNET_USE_POLLING_FILE_WATCHER=1
```

```bash
docker-compose -f docker-compose.yml -f docker-compose.dev.yml up
```

### 3. Testes com Docker

```bash
# Rodar testes dentro do container
docker-compose run --rm finansmart-app dotnet test

# Rodar testes de integração
docker-compose -f docker-compose.yml -f docker-compose.test.yml up --abort-on-container-exit
```

## ?? Troubleshooting

### Problema: Oracle não inicia

```bash
# Verificar logs
docker-compose logs oracle-db

# Verificar memória (Oracle precisa de pelo menos 2GB)
docker stats

# Aumentar memória no Docker Desktop
# Settings > Resources > Memory > 4GB ou mais
```

### Problema: Conexão recusada

```bash
# Verificar se o Oracle está pronto
docker exec finansmart-oracle-db sqlplus -L system/OraclePassword123@XE as sysdba

# Aguardar inicialização completa (pode levar 2-3 minutos)
docker-compose logs -f oracle-db
```

### Problema: Porta em uso

```bash
# Verificar quem está usando a porta
# Windows:
netstat -ano | findstr :8080

# Linux/Mac:
lsof -i :8080

# Mudar porta no docker-compose.yml
ports:
  - "8081:8080"  # Host:Container
```

## ?? Performance

### Otimizações de Produção

```yaml
# docker-compose.prod.yml
services:
  finansmart-app:
    deploy:
      replicas: 3
      update_config:
        parallelism: 1
        delay: 10s
      restart_policy:
        condition: on-failure
        max_attempts: 3
```

### Limpeza de Recursos

```bash
# Remover containers parados
docker container prune

# Remover imagens não usadas
docker image prune -a

# Remover volumes não usados
docker volume prune

# Limpeza completa (CUIDADO!)
docker system prune -a --volumes
```

## ?? Segurança

### Checklist de Segurança

- [ ] Não commitar .env no Git
- [ ] Usar secrets para senhas em produção
- [ ] Configurar SSL no Nginx
- [ ] Atualizar imagens regularmente
- [ ] Limitar recursos dos containers
- [ ] Usar network isolation
- [ ] Fazer backup regular do Oracle
- [ ] Configurar firewall no host

### Scan de Vulnerabilidades

```bash
# Instalar Trivy
# https://github.com/aquasecurity/trivy

# Scan da imagem
trivy image finansmart:latest

# Scan do filesystem
trivy fs .
```

## ?? Recursos Adicionais

- **Docker Docs**: https://docs.docker.com/
- **Docker Compose Docs**: https://docs.docker.com/compose/
- **Oracle Container Registry**: https://container-registry.oracle.com/
- **Best Practices**: https://docs.docker.com/develop/dev-best-practices/

## ?? Suporte

Para problemas específicos:
1. Verificar logs: `docker-compose logs -f`
2. Verificar saúde: `docker-compose ps`
3. Reiniciar serviços: `docker-compose restart`
4. Recriar containers: `docker-compose up --force-recreate`

---

**Criado para**: Finansmart Project  
**Versão Docker**: 20.10+  
**Versão Compose**: 3.8
